Imports Microsoft.VisualBasic.Serialization.JSON

Namespace CytoscapeGraphView.Cyjs.style

    ''' <summary>
    ''' Style for cytoscape.js (*.json)
    ''' </summary>
    Public Class JSON

        Public Property format_version As String
        Public Property generated_by As String = "GCModeller"
        Public Property target_cytoscapejs_version As String = "~2.1"
        Public Property title As String
        Public Property style As style()

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class

    Public Class style

        Public Property selector As String
        Public Property css As Dictionary(Of String, String)

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace