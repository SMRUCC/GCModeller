Namespace Network

    Public Class NetworkGraph(Of T As Node) : Inherits Graph(Of Node, Edge(Of T), NetworkGraph(Of T))

        Sub New()
        End Sub

        ''' <summary>
        ''' Network model copy
        ''' </summary>
        ''' <param name="nodes"></param>
        ''' <param name="edges"></param>
        Sub New(nodes As IEnumerable(Of Node), edges As IEnumerable(Of Edge(Of T)))

        End Sub
    End Class
End Namespace

