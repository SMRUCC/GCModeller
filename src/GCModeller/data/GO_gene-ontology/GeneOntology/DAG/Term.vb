Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace DAG

    Public Class Term : Implements sIdEnumerable

        Public Property xref As NamedValue(Of String)()
        ''' <summary>
        ''' 当前的这个term是子节点，则这个属性内的所有的节点都是这个节点的父节点
        ''' </summary>
        ''' <returns></returns>
        Public Property is_a As is_a()
        Public Property synonym As synonym()
        Public Property relationship As Relationship()
        Public Property id As String Implements sIdEnumerable.Identifier

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace