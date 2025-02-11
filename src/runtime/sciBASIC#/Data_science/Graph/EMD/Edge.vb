Namespace EMD

    ''' <summary>
    ''' @author Telmo Menezes (telmo@telmomenezes.com)
    ''' 
    ''' </summary>
    Friend Class Edge
        Friend Sub New([to] As Integer, cost As Long)
            _to = [to]
            _cost = cost
        End Sub

        Friend _to As Integer
        Friend _cost As Long
    End Class

    Friend Class Edge0
        Friend Sub New([to] As Integer, cost As Long, flow As Long)
            _to = [to]
            _cost = cost
            _flow = flow
        End Sub

        Friend _to As Integer
        Friend _cost As Long
        Friend _flow As Long
    End Class

    Friend Class Edge1
        Friend Sub New([to] As Integer, reduced_cost As Long)
            _to = [to]
            _reduced_cost = reduced_cost
        End Sub

        Friend _to As Integer
        Friend _reduced_cost As Long
    End Class

    Friend Class Edge2
        Friend Sub New([to] As Integer, reduced_cost As Long, residual_capacity As Long)
            _to = [to]
            _reduced_cost = reduced_cost
            _residual_capacity = residual_capacity
        End Sub

        Friend _to As Integer
        Friend _reduced_cost As Long
        Friend _residual_capacity As Long
    End Class

    Friend Class Edge3
        Friend Sub New()
            _to = 0
            _dist = 0
        End Sub

        Friend Sub New([to] As Integer, dist As Long)
            _to = [to]
            _dist = dist
        End Sub

        Friend _to As Integer
        Friend _dist As Long
    End Class

End Namespace