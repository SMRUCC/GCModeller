Namespace Models

    ''' <summary>
    ''' 逐位置 read 起始覆盖度记录。
    ''' </summary>
    ''' <remarks>
    ''' 字段名称与原始 TSSAR Perl 脚本生成的 coverage 文件保持一致
    ''' （``position``、``coverage``），以便与既有流程互操作。
    ''' </remarks>
    Public Class CoverageProfile

        ''' <summary>
        ''' 基因组位置（1-based）。
        ''' </summary>
        Public Property position As Integer

        ''' <summary>
        ''' 该位置上的 read 起始计数。
        ''' </summary>
        Public Property coverage As Double

        Public Overrides Function ToString() As String
            Return $"{position}->{coverage}"
        End Function
    End Class
End Namespace
