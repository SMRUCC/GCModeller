Imports System.Globalization
Imports System.IO
Imports Microsoft.VisualBasic.Language

Namespace siRNAHit

    Public Class BlastnMapTable

        Public Property qseqid As String ' 0
        Public Property sseqid As String ' 1
        Public Property sstart As Integer ' 2
        Public Property send As Integer '3
        Public Property qstart As Integer '4
        Public Property qend As Integer '5
        Public Property sstrand As String '6
        Public Property qseq As String '7
        Public Property sseq As String '8
        Public Property length As Integer '9
        Public Property evalue As Double '10
        Public Property bitscore As Double '11

        Public Shared Iterator Function Parse(s As Stream) As IEnumerable(Of BlastnMapTable)
            Dim line As Value(Of String) = ""

            Using reader As New StreamReader(InputFile)
                Do While Not (line = reader.ReadLine) Is Nothing
                    line = line.Trim()

                    If String.IsNullOrWhiteSpace(line) Then
                        Continue Do
                    End If

                    Dim cols As String() = line.Split(vbTab)

                    If cols.Length < 12 Then
                        Continue Do
                    Else
                        Dim qseqid As String = cols(0)
                        Dim sseqid As String = cols(1)
                        Dim sstart As String = cols(2)
                        Dim send As String = cols(3)
                        Dim sstrand As String = cols(6)
                        Dim qseq As String = cols(7)
                        Dim sseq As String = cols(8)
                        Dim evalueStr As String = cols(10)

                        ' E-value 解析：BLASTN 常输出科学计数法（如 2e-07），
                        ' 用 InvariantCulture 避免系统区域设置（如德语逗号小数点）干扰
                        Dim evalue As Double

                        If Double.TryParse(evalueStr, NumberStyles.Float, CultureInfo.InvariantCulture, evalue) Then
                            Yield New BlastnMapTable With {
                                .evalue = evalue,
                                .length = cols(9).ParseInteger,
                                .bitscore = cols(11).ParseDouble,
                                .qend = cols(5).ParseInteger,
                                .sstrand = sstrand,
                                .qseq = qseq,
                                .qseqid = qseqid,
                                .qstart = cols(4).ParseInteger,
                                .send = send,
                                .sseq = sseq,
                                .sseqid = sseqid,
                                .sstart = sstart
                            }
                        End If
                    End If
                Loop
            End Using
        End Function

    End Class
End Namespace