Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq

Namespace OBO

    Public Class OntologyRelations

        ''' <summary>
        ''' ```
        ''' is_a: GO:0048311 ! mitochondrion distribution
        ''' ```
        ''' </summary>
        ''' <returns></returns>
        Public Property is_a As NamedValue(Of String)()

        Sub New(term As Term)
            is_a = term.is_a.ToArray(Function(s) s.GetTagValue(" ! ", trim:=True))
        End Sub
    End Class
End Namespace