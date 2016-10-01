Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

Namespace DAG

    Public Class Term

        Public Property xref As NamedValue(Of String)()
        ''' <summary>
        ''' 当前的这个term是子节点，则这个属性内的所有的节点都是这个节点的父节点
        ''' </summary>
        ''' <returns></returns>
        Public Property is_a As is_a()
        Public Property synonym As synonym()
        Public Property relationship As Relationship()

    End Class
End Namespace