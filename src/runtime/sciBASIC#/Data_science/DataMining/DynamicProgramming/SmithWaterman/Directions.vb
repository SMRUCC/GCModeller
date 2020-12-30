Namespace SmithWaterman

    ''' <summary>
    ''' Constants of directions.
    ''' Multiple directions are stored by bits.
    ''' The zero direction is the starting point.
    ''' </summary>
    Public Class Directions

        ''' <summary>
        ''' 0001
        ''' </summary>
        Public Const DR_LEFT As Integer = 1
        ''' <summary>
        ''' 0010
        ''' </summary>        
        Public Const DR_UP As Integer = 2
        ''' <summary>
        ''' 0100
        ''' </summary>        
        Public Const DR_DIAG As Integer = 4
        ''' <summary>
        ''' 1000
        ''' </summary>        
        Public Const DR_ZERO As Integer = 8

    End Class
End Namespace