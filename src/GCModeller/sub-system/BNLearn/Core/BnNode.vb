Namespace Core

    ''' <summary>
    ''' 贝叶斯网络节点 —— 对应一个基因
    ''' </summary>
    Public Class BnNode

        ''' <summary>节点名称（基因名）</summary>
        Public Property Name As String = ""

        ''' <summary>节点索引</summary>
        Public Property Index As Integer = -1

        ''' <summary>父节点索引列表（上游调控基因）</summary>
        Public Property Parents As New List(Of Integer)()

        ''' <summary>子节点索引列表（下游靶基因）</summary>
        Public Property Children As New List(Of Integer)()

        ''' <summary>该节点的条件概率分布参数</summary>
        Public Property CPD As BnCPD = Nothing

        ''' <summary>节点层级（拓扑排序后）</summary>
        Public Property Level As Integer = 0

        Public Overrides Function ToString() As String
            Return String.Format("{0}(parents=[{1}])", Name, String.Join(",", Parents))
        End Function

    End Class
End Namespace