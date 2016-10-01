Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Data.GeneOntology.OBO

Namespace DAG

    Public Module Builder

        <Extension>
        Public Function BuildTree(file As IEnumerable(Of OBO.Term)) As Dictionary(Of Term)
            Dim tree As New Dictionary(Of Term)

            For Each x As OBO.Term In file
                tree += New Term With {
                    .id = x.id,
                    .is_a = x.is_a.ToArray(Function(s) New is_a(s$)),
                    .relationship = x.relationship.ToArray(Function(s) New Relationship(s$)),
                    .synonym = x.synonym.ToArray(Function(s) New synonym(s$)),
                    .xref = x.xref.ToArray(AddressOf xrefParser)
                }
            Next

            Return tree
        End Function

        Private Function xrefParser(s$) As NamedValue(Of String)
            Dim tokens$() = CommandLine.GetTokens(s$)
            Dim id = tokens(Scan0).Split(":"c)

            Return New NamedValue(Of String) With {
                .Name = id(Scan0),
                .x = id(1%),
                .Description = tokens.Get(1%)
            }
        End Function

        Public Function BuildTree(path$) As Dictionary(Of Term)
            Return GO_OBO.Open(path).BuildTree
        End Function
    End Module
End Namespace