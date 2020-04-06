Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.MIME.application.xml.MathML
Imports SMRUCC.genomics.Model.SBML.Level3

Namespace SBML

    Public Class SBMLInternalIndexer

        ReadOnly compartments As Dictionary(Of String, compartment)
        ReadOnly species As Dictionary(Of String, species)
        ReadOnly keggReactions As New Dictionary(Of String, List(Of SBMLReaction))
        ReadOnly formulaLambdas As New Dictionary(Of String, LambdaExpression)

        Sub New(sbml As SbmlDocument)
            Dim entries As NamedValue(Of String)()

            For Each react As SBMLReaction In sbml.sbml.model.listOfReactions
                entries = react.getIdentifiers.Where(Function(r) r.Name = "kegg.reaction").ToArray

                For Each id In entries
                    If Not keggReactions.ContainsKey(id.Value) Then
                        keggReactions.Add(id.Value, New List(Of SBMLReaction))
                    End If

                    keggReactions(id.Value).Add(react)
                Next
            Next

            For Each lambda As NamedValue(Of LambdaExpression) In sbml.mathML
                If Not lambda.Value Is Nothing Then
                    formulaLambdas.Add(lambda.Name, lambda.Value)
                End If
            Next

            compartments = sbml.sbml.model.listOfCompartments.ToDictionary(Function(c) c.id)
            species = sbml.sbml.model.listOfSpecies.ToDictionary(Function(c) c.id)
        End Sub

        ''' <summary>
        ''' 获取得到反应过程对应的动力学函数
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        Public Function getFormula(id As String) As LambdaExpression
            If formulaLambdas.ContainsKey(id) Then
                Return formulaLambdas(id)
            Else
                Return Nothing
            End If
        End Function

        Public Function getKEGGreactions(id As String) As IEnumerable(Of SBMLReaction)
            Return keggReactions.TryGetValue(id)
        End Function

        Public Iterator Function getEnzymes(react As SBMLReaction) As IEnumerable(Of species)
            If Not react.listOfModifiers.IsNullOrEmpty Then
                For Each modifier In react.listOfModifiers
                    Yield species(modifier.species)
                Next
            End If
        End Function
    End Class
End Namespace