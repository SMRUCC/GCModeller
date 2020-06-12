Imports Microsoft.VisualBasic.Serialization.JSON

Namespace DNAOrigami

    Public Class Project

        ''' <summary>
        ''' segment length
        ''' </summary>
        ''' <returns></returns>
        Public Property n As Integer = 7
        ''' <summary>
        ''' scaffolds are not circular
        ''' </summary>
        ''' <returns></returns>
        Public Property is_linear As Boolean = False
        ''' <summary>
        ''' also count reverse complementary sequences
        ''' </summary>
        ''' <returns></returns>
        Public Property get_rev_compl As Boolean = False

    End Class

    Public Class Output

        Public Property tuple As String()

        Public Property count As Double
        Public Property count_revcompl As Double
        Public Property count_corrected As Double
        Public Property count_revcompl_corrected As Double
        Public Property n_count As Double()
        Public Property n_count_revcompl As Double()

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

    End Class
End Namespace