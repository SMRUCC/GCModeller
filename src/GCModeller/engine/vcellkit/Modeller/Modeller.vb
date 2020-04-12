#Region "Microsoft.VisualBasic::e548dba8f1546e0b1e92d90425985fcd, vcellkit\Modeller\Modeller.vb"

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

' Module Modeller
' 
'     Function: applyKinetics, LoadVirtualCell
' 
'     Sub: createKineticsDbCache
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Scripting.MathExpression
Imports Microsoft.VisualBasic.MIME.application.xml.MathML
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports SMRUCC.genomics.Data.SABIORK
Imports SMRUCC.genomics.Data.SABIORK.SBML
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2
Imports SMRUCC.genomics.Model.SBML.Level3
Imports SMRUCC.Rsharp.Runtime

''' <summary>
''' virtual cell network kinetics modeller
''' </summary>
<Package("vcellkit.modeller", Category:=APICategories.UtilityTools, Publisher:="xie.guigang@gcmodeller.org")>
Module Modeller

    ' ((kcat * E) * S) / (Km + S)

    <Extension>
    Private Function compoundIdNameIndex(vcell As VirtualCell) As Dictionary(Of String, String)
        Dim index As New Dictionary(Of String, String)

        For Each cpd In vcell.metabolismStructure.compounds
            For Each name As String In {cpd.name}.JoinIterates(cpd.otherNames)
                If Not index.ContainsKey(name) Then
                    Call index.Add(name, cpd.ID)
                End If
            Next
        Next

        Return index
    End Function

    ''' <summary>
    ''' apply the kinetics parameters from the sabio-rk database.
    ''' </summary>
    ''' <param name="vcell"></param>
    ''' <returns></returns>
    <ExportAPI("apply.kinetics")>
    Public Function applyKinetics(vcell As VirtualCell, Optional cache$ = "./.cache", Optional env As Environment = Nothing) As VirtualCell
        Dim keggEnzymes = htext.GetInternalResource("ko01000").Hierarchical _
            .EnumerateEntries _
            .Where(Function(key) Not key.entryID.StringEmpty) _
            .GroupBy(Function(enz) enz.entryID) _
            .ToDictionary(Function(KO) KO.Key,
                          Function(ECNumber)
                              Return ECNumber.ToArray
                          End Function)
        Dim numbers As BriteHText()
        Dim reactions As IEnumerable(Of SBMLReaction)
        Dim compoundId As Dictionary(Of String, String) = vcell.compoundIdNameIndex

        For Each enzyme As Enzyme In vcell.metabolismStructure.enzymes
            Dim kineticList As New List(Of SBMLInternalIndexer)
            Dim kinetics As SbmlDocument

            If keggEnzymes.ContainsKey(enzyme.KO) Then
                numbers = keggEnzymes(enzyme.KO)

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

                    kineticList += New SBMLInternalIndexer(kinetics)
                Next
            Else
                env.AddMessage($"missing ECNumber mapping for '{enzyme.KO}'.", MSG_TYPES.WRN)
            End If

            For Each react As Catalysis In enzyme.catalysis
                For Each index In kineticList
                    reactions = index.getKEGGreactions(react.reaction)

                    If Not reactions Is Nothing Then
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
                    End If
                Next
            Next
        Next

        Return vcell
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    <Extension>
    Private Iterator Function parseKineticsParameters(reaction As SBMLReaction， index As SBMLInternalIndexer, KO$, compoundId As Dictionary(Of String, String)) As IEnumerable(Of KineticsParameter)
        Dim locals As Dictionary(Of String, localParameter) = reaction.kineticLaw.listOfLocalParameters.ToDictionary(Function(a) a.id)
        Dim local As localParameter
        Dim id As String
        Dim enzyme As species

        For Each require As String In reaction.kineticLaw.math.apply.ci _
            .Skip(1) _
            .Select(AddressOf Strings.Trim)

            If locals.ContainsKey(require) Then
                local = locals(require)
                ' 常数值
                Yield New KineticsParameter With {
                    .name = local.id,
                    .target = local.name,
                    .value = local.value
                }
            Else
                ' 代谢物浓度变量对象
                id = index.getKEGGCompoundId(require)

                If id.StringEmpty Then
                    ' 可能是酶分子
                    ' 也包含有kegg代谢物id
                    enzyme = index.getSpecies(require)

                    If compoundId.ContainsKey(enzyme.name) Then
                        Yield New KineticsParameter With {
                            .name = require,
                            .target = compoundId(enzyme.name),
                            .value = Double.NaN,
                            .isModifier = True
                        }
                    Else
                        ' enzyme?
                        Yield New KineticsParameter With {
                            .name = require,
                            .target = KO,
                            .value = Double.NaN,
                            .isModifier = True
                        }
                    End If
                Else
                    ' 是代谢物反应底物或者产物
                    Yield New KineticsParameter With {
                        .name = require,
                        .target = id,
                        .value = Double.NaN
                    }
                End If
            End If
        Next
    End Function

    <ExportAPI("cacheOf.enzyme_kinetics")>
    Public Sub createKineticsDbCache(Optional export$ = "./")
        Call htext.GetInternalResource("ko01000").QueryByECNumbers(export).ToArray
    End Sub

    ''' <summary>
    ''' read the virtual cell model file
    ''' </summary>
    ''' <param name="path"></param>
    ''' <returns></returns>
    <ExportAPI("read.vcell")>
    Public Function LoadVirtualCell(path As String) As VirtualCell
        Return path.LoadXml(Of VirtualCell)
    End Function

    <ExportAPI("zip")>
    Public Function WriteZipAssembly(vcell As VirtualCell, file As String) As Boolean
        Return ZipAssembly.WriteZip(vcell, file)
    End Function
End Module
