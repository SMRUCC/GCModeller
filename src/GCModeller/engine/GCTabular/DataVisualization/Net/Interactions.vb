Imports Microsoft.VisualBasic.DataVisualization.Network.FileStream

Namespace DataVisualization

    ''' <summary>
    ''' 可以这样认为，一个Interaction对象就是一个MetabolismFlux
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Interactions : Inherits NetworkEdge

        Public Property Pathway As String
        Public Property UniqueId As String
        Public Property FluxValue As Double

        Public Overrides Function ToString() As String
            Return String.Format("[{0}]  {1}  --> {2}", UniqueId, FromNode, ToNode)
        End Function
    End Class
End Namespace