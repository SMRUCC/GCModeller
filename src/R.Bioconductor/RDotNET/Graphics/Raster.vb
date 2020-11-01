Namespace Graphics
    Public Class Raster
        Private raster As Color(,)

        Public Sub New(ByVal width As Integer, ByVal height As Integer)
            raster = New Color(width - 1, height - 1) {}
        End Sub

        Default Public Property Item(ByVal x As Integer, ByVal y As Integer) As Color
            Get
                Return raster(x, y)
            End Get
            Set(ByVal value As Color)
                raster(x, y) = value
            End Set
        End Property

        Public ReadOnly Property Width As Integer
            Get
                Return raster.GetLength(1)
            End Get
        End Property

        Public ReadOnly Property Height As Integer
            Get
                Return raster.GetLength(0)
            End Get
        End Property
    End Class
End Namespace
