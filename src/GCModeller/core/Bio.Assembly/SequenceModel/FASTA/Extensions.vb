Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq

Namespace SequenceModel.FASTA

    Public Module Extensions

        ''' <summary>
        ''' 函数返回-1表示找不到
        ''' </summary>
        ''' <param name="fasta"></param>
        ''' <param name="idx$">对index的描述，可以是title也可以直接是index数字</param>
        ''' <returns></returns>
        <Extension>
        Public Function Index(fasta As FastaFile, idx$) As Integer
            For Each seq As SeqValue(Of FastaToken) In fasta.SeqIterator
                If idx.TextEquals((+seq).Title) Then
                    Return seq.i
                End If
            Next

            If IsNumeric(idx) Then
                Return CInt(Val(idx))
            Else
                Return -1
            End If
        End Function
    End Module
End Namespace

