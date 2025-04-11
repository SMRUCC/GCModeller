Namespace Keywords

    ''' <summary>
    ''' Axis information
    ''' </summary>
    Public MustInherit Class Spatial3D : Inherits Keyword

        Public Property x As Double
        Public Property y As Double
        Public Property z As Double
        Public Property factor As Double

        Friend Shared Function Parse(Of T As {New, Spatial3D})(str As String) As Spatial3D
            Dim cols As String() = str.StringSplit("\s+")
            Dim s As New T With {
                .x = Val(cols(0)),
                .y = Val(cols(1)),
                .z = Val(cols(2)),
                .factor = Val(cols(3))
            }

            Return s
        End Function
    End Class

    Public Class ORIGX123 : Inherits Spatial3D

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return "ORIGX1"
            End Get
        End Property

    End Class

    Public Class SCALE123 : Inherits Spatial3D

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return "SCALE1"
            End Get
        End Property

    End Class
End Namespace