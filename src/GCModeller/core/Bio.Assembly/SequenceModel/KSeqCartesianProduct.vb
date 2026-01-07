Imports System.IO
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports SMRUCC.genomics.SequenceModel.Polypeptides

Namespace SequenceModel

    ''' <summary>
    ''' kmer sequence generator
    ''' </summary>
    Public Class KSeqCartesianProduct

        ReadOnly alphabet As Char()

        Sub New(seqtype As SeqTypes)
            Select Case seqtype
                Case SeqTypes.DNA : alphabet = {"A", "T", "G", "C"}
                Case SeqTypes.RNA : alphabet = {"A", "U", "G", "C"}
                Case SeqTypes.Protein
                    alphabet = Polypeptide.ToChar.Values.ToArray
                Case Else
                    Throw New InvalidDataException("unknown sequence data type!")
            End Select
        End Sub

        ''' <summary>
        ''' 根据序列类型和长度生成所有可能的排列组合
        ''' </summary>
        ''' <returns>包含所有排列组合结果的列表</returns>
        Public Iterator Function KmerSeeds(k As Integer) As IEnumerable(Of String)
            If k <= 0 Then
                Return
            ElseIf k = 1 Then
                For Each c As Char In alphabet
                    Yield CStr(c)
                Next
            Else
                Dim nddata As Char()() = New Char(k - 1)() {}

                For i As Integer = 0 To k - 1
                    nddata(i) = alphabet.ToArray
                Next

                ' create kmers via N-dimension cartesian product
                For Each chs As Char() In NDimensionCartesianProduct.CreateMultiCartesianProduct(Of Char)(nddata)
                    Yield New String(chs)
                Next
            End If
        End Function
    End Class
End Namespace