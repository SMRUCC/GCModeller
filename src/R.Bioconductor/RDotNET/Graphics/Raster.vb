Namespace Graphics

    Public Class Raster
        Private raster As Color(,)

        Public Sub New(width As Integer, height As Integer)
            Me.raster = New Color(width - 1, height - 1) {}
        End Sub

        Default Public Property Item(x As Integer, y As Integer) As Color
            Get
                Return Me.raster(x, y)
            End Get
            Set(value As Color)
                Me.raster(x, y) = value
            End Set
        End Property

        Public ReadOnly Property Width() As Integer
            Get
                Return Me.raster.GetLength(1)
            End Get
        End Property

        Public ReadOnly Property Height() As Integer
            Get
                Return Me.raster.GetLength(0)
            End Get
        End Property
    End Class
End Namespace