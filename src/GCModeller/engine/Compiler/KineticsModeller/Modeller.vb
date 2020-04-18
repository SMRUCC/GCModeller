Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports Microsoft.VisualBasic.ApplicationServices.Development
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.Language
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

        Return 0
    End Function

    Protected Overrides Function CompileImpl(args As CommandLine) As Integer
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
        Dim compoundId As Dictionary(Of String, String) = m_compiledModel.compoundIdNameIndex

        For Each enzyme As Enzyme In m_compiledModel.metabolismStructure.enzymes
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
                m_logging.WriteLine($"missing ECNumber mapping for '{enzyme.KO}'.",, MSG_TYPES.WRN)
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

        Return 0
    End Function
End Class
