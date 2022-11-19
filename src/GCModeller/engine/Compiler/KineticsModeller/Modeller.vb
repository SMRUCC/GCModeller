#Region "Microsoft.VisualBasic::42e60df8bcc5ef95ed7324753f84aeaa, GCModeller\engine\Compiler\KineticsModeller\Modeller.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 164
    '    Code Lines: 130
    ' Comment Lines: 5
    '   Blank Lines: 29
    '     File Size: 6.38 KB


    ' Class Modeller
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: CompileImpl, doKineticsQuery, PreCompile
    ' 
    '     Sub: applyForEnzyme, applyReactionKinetics
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports Microsoft.VisualBasic.ApplicationServices.Development
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Scripting.MathExpression
Imports Microsoft.VisualBasic.MIME.application.xml.MathML
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports SMRUCC.genomics.Data.SABIORK
Imports SMRUCC.genomics.Data.SABIORK.SBML
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2
Imports SMRUCC.genomics.GCModeller.CompilerServices

''' <summary>
''' apply the kinetics parameters from the sabio-rk database.
''' </summary>
Public Class Modeller : Inherits Compiler(Of VirtualCell)

    ReadOnly cache$

    Dim keggEnzymes As Dictionary(Of String, BriteHText())
    Dim compoundId As Dictionary(Of String, String)

    Sub New(rawModel As VirtualCell, kineticsCache As String)
        cache = kineticsCache
        m_compiledModel = rawModel
    End Sub

    Protected Overrides Function PreCompile(args As CommandLine) As Integer
        Dim info As New StringBuilder

        Using writer As New StringWriter(info)
            Call CLITools.AppSummary(GetType(Modeller).Assembly.FromAssembly, "", "", writer)
        End Using

        m_logging.WriteLine(info.ToString)

        compoundId = m_compiledModel.compoundIdNameIndex
        keggEnzymes = htext.GetInternalResource("ko01000").Hierarchical _
            .EnumerateEntries _
            .Where(Function(key) Not key.entryID.StringEmpty) _
            .GroupBy(Function(enz) enz.entryID) _
            .ToDictionary(Function(KO) KO.Key,
                          Function(ECNumber)
                              Return ECNumber.ToArray
                          End Function)

        Return 0
    End Function

    Protected Overrides Function CompileImpl(args As CommandLine) As Integer
        For Each enzyme As Enzyme In m_compiledModel.metabolismStructure.enzymes
            Call applyForEnzyme(enzyme)
        Next

        Return 0
    End Function

    Private Iterator Function doKineticsQuery(KO_term As String) As IEnumerable(Of SBMLInternalIndexer)
        Dim numbers As BriteHText() = keggEnzymes(KO_term)
        Dim kinetics As SbmlDocument

        For Each number As String In numbers.Select(Function(num) num.parent.classLabel.Split.First)
            Dim q As New Dictionary(Of QueryFields, String) From {
                {QueryFields.ECNumber, number}
            }
            Dim xml As String = docuRESTfulWeb.searchKineticLawsRawXml(q, cache)

            If xml.StringEmpty Then
                Continue For
            Else
                kinetics = SbmlDocument.LoadDocument(xml)
            End If

            If kinetics Is Nothing Then
                Continue For
            End If

            Yield New SBMLInternalIndexer(kinetics)
        Next
    End Function

    Private Sub applyReactionKinetics(enzyme As Enzyme, react As Catalysis, kineticList As SBMLInternalIndexer())
        For Each index As SBMLInternalIndexer In kineticList
            Dim reactions As IEnumerable(Of SBMLReaction) = index.getKEGGreactions(react.reaction)

            If reactions Is Nothing Then
                Continue For
            End If

            For Each target As SBMLReaction In reactions
                ' 如何查找匹配最优的催化动力学过程？
                Dim refId As String = "KL_" & target.kineticLaw.annotation.sabiork.kineticLawID
                Dim formula As LambdaExpression = index.getFormula(refId)
                Dim conditions As experimentalConditions

                If formula Is Nothing OrElse target.kineticLaw.annotation.sabiork.experimentalConditions Is Nothing Then
                    Continue For
                Else
                    conditions = target.kineticLaw.annotation.sabiork.experimentalConditions
                End If

                react.formula = New FunctionElement With {
                    .lambda = formula.lambda.ToString,
                    .name = target.kineticLaw.sboTerm,
                    .parameters = formula.parameters
                }

                formula.parameters = formula.parameters _
                    .OrderByDescending(Function(name) name.Length) _
                    .ToArray
                react.parameter = target _
                    .parseKineticsParameters(index, enzyme.KO, compoundId) _
                    .ToArray

                If conditions.pHValue Is Nothing Then
                    react.PH = 7
                Else
                    react.PH = conditions.pHValue.startValuepH
                End If

                If conditions.temperature Is Nothing Then
                    react.temperature = 37
                Else
                    react.temperature = conditions.temperature.startValueTemperature
                End If

                Exit For
            Next
        Next
    End Sub

    Private Sub applyForEnzyme(enzyme As Enzyme)
        Dim kineticList As SBMLInternalIndexer()

        If keggEnzymes.ContainsKey(enzyme.KO) Then
            kineticList = doKineticsQuery(enzyme.KO).ToArray

            For Each react As Catalysis In enzyme.catalysis
                Call applyReactionKinetics(enzyme, react, kineticList)

                If react.formula Is Nothing Then
                    ' 采用标准的米氏方程么？
                    react.PH = 7
                    react.temperature = 36
                    react.formula = New FunctionElement With {
                        .name = "Michaelis-Menten equation",
                        .parameters = {"Vmax", "S", "Km"},
                        .lambda = "(Vmax * S) / (Km + S)"
                    }
                    react.parameter = {
                        New KineticsParameter With {.name = "Vmax", .target = "Vmax", .value = 100},
                        New KineticsParameter With {.name = "S", .target = "S", .value = Double.NaN},
                        New KineticsParameter With {.name = "Km", .target = "Km", .value = 0.5}
                    }
                End If
            Next
        Else
            m_logging.WriteLine($"missing ECNumber mapping for '{enzyme.KO}'.",, MSG_TYPES.WRN)
        End If
    End Sub
End Class
