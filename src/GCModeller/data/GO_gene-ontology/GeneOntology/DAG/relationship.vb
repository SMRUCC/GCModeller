Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

Namespace DAG

    Public Structure Relationship

        Public type As OntologyRelations
        Public parent As NamedValue(Of String)
        Public parentName As String

        Sub New(value$)

        End Sub

        Public Overrides Function ToString() As String
            Return $"relationship: {type.ToString} {parent.Name}:{parent.x} ! {parentName}"
        End Function
    End Structure
End Namespace