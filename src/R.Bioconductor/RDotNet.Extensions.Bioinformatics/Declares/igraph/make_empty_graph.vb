Imports RDotNet.Extensions.VisualBasic.Services.ScriptBuilder

Namespace igraph

    ''' <summary>
    ''' A graph with no edges
    ''' </summary>
    <RFunc("make_empty_graph")> Public Class make_empty_graph : Inherits igraph

        ''' <summary>
        ''' Number of vertices.
        ''' </summary>
        ''' <returns></returns>
        Public Property n As Integer = 0
        ''' <summary>
        ''' Whether to create a directed graph.
        ''' </summary>
        ''' <returns></returns>
        Public Property directed As Boolean = True
    End Class

    <RFunc(NameOf(empty_graph))> Public Class empty_graph : Inherits make_empty_graph

    End Class
End Namespace