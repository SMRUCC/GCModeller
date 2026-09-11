' /********************************************************************************/
'
'  Rockhopper —— RNA 片段模型
'
'  复刻自原始 Rockhopper Java 源码 RNA.java：一个极简的转录区段（start/stop/strand）。
'
' /********************************************************************************/

Namespace Core

    ''' <summary>
    ''' 极简的 RNA 区段模型：起始、终止与链方向。
    ''' </summary>
    Friend Class RNA

        Public ReadOnly Property Start As Integer
        Public ReadOnly Property [Stop] As Integer
        Public ReadOnly Property Strand As Char

        Public Sub New(start As Integer, [stop] As Integer, strand As Char)
            Me.Start = start
            Me.[Stop] = [stop]
            Me.Strand = strand
        End Sub

        Public Overloads Function ToString(Optional delimiter As String = vbTab) As String
            Return $"{Start}{delimiter}{[Stop]}{delimiter}{Strand}"
        End Function

    End Class

End Namespace
