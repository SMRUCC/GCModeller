Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace CytoscapeGraphView.Cyjs.style

    ''' <summary>
    ''' Style for cytoscape.js (*.json)
    ''' </summary>
    Public Class JSON : Implements INamedValue

        Public Property format_version As String
        Public Property generated_by As String = "GCModeller"
        Public Property target_cytoscapejs_version As String = "~2.1"
        Public Property title As String Implements INamedValue.Key
        Public Property style As style()

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

        Public Shared Function Load(path$) As Dictionary(Of JSON)
            Return path _
                .ReadAllText _
                .LoadObject(Of JSON()) _
                .ToDictionary
        End Function
    End Class

    Public Class style

        Public Property selector As String
        Public Property css As Dictionary(Of String, String)

        Public Function MySelector() As Selector
            Return New Selector(selector)
        End Function

        Public Function GetStyle() As CSSTranslator
            Return New CSSTranslator(css)
        End Function

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace