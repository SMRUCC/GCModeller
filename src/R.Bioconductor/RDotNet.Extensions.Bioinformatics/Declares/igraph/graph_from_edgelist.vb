Imports RDotNet.Extensions.VisualBasic.Services.ScriptBuilder
Imports RDotNet.Extensions.VisualBasic.Services.ScriptBuilder.RTypes

Namespace igraph

    ''' <summary>
    ''' graph_from_edgelist creates a graph from an edge list. Its argument is a two-column matrix, each row defines one edge.
    ''' If it is a numeric matrix then its elements are interpreted as vertex ids.
    ''' If it is a character matrix then it is interpreted as symbolic vertex names and a vertex id will be assigned to each name, and also a name vertex attribute will be added.
    ''' </summary>
    <RFunc(NameOf(graph_from_edgelist))> Public Class graph_from_edgelist : Inherits igraph

        ''' <summary>
        ''' The edge list, a two column matrix, character or numeric.
        ''' </summary>
        ''' <returns></returns>
        Public Property el As RExpression
        ''' <summary>
        ''' Whether to create a directed graph.
        ''' </summary>
        ''' <returns></returns>
        Public Property directed As Boolean = True

    End Class

    ''' <summary>
    ''' Create a graph from an edge list matrix
    ''' </summary>
    <RFunc(NameOf(from_edgelist))> Public Class from_edgelist : Inherits graph_from_edgelist

    End Class
End Namespace