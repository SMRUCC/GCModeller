Imports SMRUCC.genomics.Visualize.Cytoscape.CytoscapeGraphView.Cyjs

Namespace Upload

    Public Class CyjsUpload

        Public Property data As cyjsdata
        Public Property elements As networkElement

        Sub New(cyjs As Cyjs, title As String)
            data = New cyjsdata With {
                .name = If(title, App.NextTempName)
            }
            elements = New networkElement With {
                .edges = cyjs.elements.edges.Select(Function(a) New cyjsedge With {.data = New edgeData2 With {.interaction = a.data.interaction, .source = a.data.source, .target = a.data.target}}).ToArray,
                .nodes = cyjs.elements.nodes.Select(Function(a) New cyjsNode With {.data = New nodeData2 With {.common = a.data.common, .id = a.data.id}}).ToArray
            }
        End Sub

    End Class

    Public Class networkElement
        Public Property nodes As cyjsNode()
        Public Property edges As cyjsedge()
    End Class

    Public Class cyjsNode
        Public Property data As nodeData2
    End Class

    Public Class cyjsedge
        Public Property data As edgeData2
    End Class

    Public Class edgeData2
        Public Property source As String
        Public Property target As String
        Public Property interaction As String
    End Class

    Public Class nodeData2
        Public Property id As String
        Public Property common As String
    End Class

    Public Class cyjsdata
        Public Property name As String = App.NextTempName
    End Class
End Namespace
