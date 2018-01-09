Namespace SequenceLogo

    ''' <summary>
    ''' Alphabet model in the drawing motif model, nt for 4 and aa for 20
    ''' </summary>
    Public Class Alphabet : Implements IComparable

        ''' <summary>
        ''' A alphabet character which represents one residue.(可以代表本残基的字母值)
        ''' </summary>
        ''' <returns></returns>
        Public Property Alphabet As Char
        ''' <summary>
        ''' The relative alphabet frequency at this site position.
        ''' </summary>
        ''' <returns></returns>
        Public Property RelativeFrequency As Double

        ''' <summary>
        ''' Sorts for the logo drawing
        ''' </summary>
        ''' <param name="obj"></param>
        ''' <returns></returns>
        Public Function CompareTo(obj As Object) As Integer Implements IComparable.CompareTo
            If obj Is Nothing Then
                Return 1
            ElseIf obj.GetType <> GetType(Alphabet) Then
                Return 1
            End If

            Dim n As Double = DirectCast(obj, Alphabet).RelativeFrequency

            If RelativeFrequency > n Then
                Return 1
            Else
                Return -1
            End If
        End Function

        ''' <summary>
        ''' The height of letter a in column i Is given by
        ''' 
        ''' ```
        '''    height = f(a,i) x R(i)
        ''' ```
        ''' (该残基之中本类型的字母的绘制的高度)
        ''' </summary>
        ''' <returns></returns>
        Public Function Height(Ri As Double) As Integer
            Return CInt(Me.RelativeFrequency * Ri)
        End Function

        Public Overrides Function ToString() As String
            Return $"{Alphabet} --> {RelativeFrequency}"
        End Function
    End Class
End Namespace