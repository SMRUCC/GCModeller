Imports System.IO
Imports Microsoft.VisualBasic.Language

Namespace siRNAHit

    Public Class BlastnMapTable

        Public Property qseqid As String
        Public Property sseqid As String
        Public Property sstart As Integer
        Public Property send As Integer
        Public Property qstart As Integer
        Public Property qend As Integer
        Public Property sstrand As String
        Public Property qseq As String
        Public Property sseq As String
        Public Property length As Integer
        Public Property evalue As Double
        Public Property bitscore As Double

        Public Shared Function Parse(s As Stream) As BlastnMapTable
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
                    End If
                Loop
            End Using
        End Function

    End Class
End Namespace