Namespace SmithWaterman

    Public Class Directions

        ''' <summary>
        ''' Constants of directions.
        ''' Multiple directions are stored by bits.
        ''' The zero direction is the starting point.
        ''' </summary>
        Public Const DR_LEFT As Integer = 1
        ' 0001
        Public Const DR_UP As Integer = 2
        ' 0010
        Public Const DR_DIAG As Integer = 4
        ' 0100
        Public Const DR_ZERO As Integer = 8
        ' 1000
    End Class
End Namespace