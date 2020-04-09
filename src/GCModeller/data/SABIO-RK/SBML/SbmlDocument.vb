Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.MIME.application.xml.MathML
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Model.SBML.Level3

Namespace SBML

    Public Class SbmlDocument

        Public Property sbml As XmlFile(Of SBMLReaction)
        Public Property mathML As NamedValue(Of LambdaExpression)()

        Public Shared Function LoadDocument(path As String) As SbmlDocument
            Dim text As String = path.SolveStream

            If text.Trim(" "c, ASCII.TAB, ASCII.CR, ASCII.LF) = "No results found for query" Then
                Return Nothing
            End If

            Dim sbml As XmlFile(Of SBMLReaction) = text.LoadFromXml(Of XmlFile(Of SBMLReaction))
            Dim math = MathMLParser.ParseMathML(sbmlText:=text).ToArray
            Dim formulas As NamedValue(Of LambdaExpression)() = math _
                .Select(Function(a)
                            Return New NamedValue(Of LambdaExpression) With {
                                .Name = a.Name,
                                .Description = a.Description,
                                .Value = LambdaExpression.FromMathML(a.Value)
                            }
                        End Function) _
                .ToArray

            Return New SbmlDocument With {
                .sbml = sbml,
                .mathML = formulas
            }
        End Function
    End Class
End Namespace