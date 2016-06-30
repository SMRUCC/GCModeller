Imports RDotNet.Extensions.VisualBasic

Namespace plot3D

    ''' <summary>
    ''' Plots arrows, segments, points, lines, polygons, rectangles and boxes in a 3D perspective plot or in 2D.
    ''' </summary>
    Public MustInherit Class plot3D : Inherits IRToken

        Sub New()
            MyBase.Requires = {"plot3D"}
        End Sub
    End Class
End Namespace