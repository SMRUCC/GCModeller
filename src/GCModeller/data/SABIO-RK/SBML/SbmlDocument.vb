Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.MIME.application.xml.MathML
Imports SMRUCC.genomics.Model.SBML.Level3

Namespace SBML

    Public Class SbmlDocument

        Public Property sbml As XmlFile(Of SBMLReaction)
        Public Property mathML As NamedValue(Of LambdaExpression)()

        Public Shared Function LoadDocument(path As String) As SbmlDocument
            Dim sbml As XmlFile(Of SBMLReaction) = path.LoadXml(Of XmlFile(Of SBMLReaction))
            Dim math = MathMLParser.ParseMathML(sbmlText:=path.ReadAllText).ToArray
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