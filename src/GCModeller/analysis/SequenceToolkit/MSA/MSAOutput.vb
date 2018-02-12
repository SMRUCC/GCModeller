Imports System.IO
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Structure MSAOutput

    Dim names$()
    Dim MSA$()
    Dim cost#

    Public Overrides Function ToString() As String
        With New MemoryStream
            Print(, New StreamWriter(.ByRef))
            Return .ToArray.UTF8String
        End With
    End Function

    Public Function ToFasta() As FastaFile
        Dim MSA = Me.MSA
        Dim seqs = names _
            .Select(Function(name, i)
                        Return New FastaSeq With {
                            .Headers = {name},
                            .SequenceData = MSA(i)
                        }
                    End Function)

        Return New FastaFile(seqs)
    End Function

    Public Sub Print(Optional maxNameWidth% = 10, Optional dev As TextWriter = Nothing)
        Dim n = MSA.Length
        Dim names = Me.names.ToArray
        Dim out As TextWriter = dev Or Console.Out.AsDefault

        For i As Integer = 0 To n - 1
            names(i) = Mid(names(i), 1, maxNameWidth)
            names(i) = names(i) & New String(" "c, maxNameWidth - names(i).Length)
            out.WriteLine(names(i) & vbTab & MSA(i))
        Next

        Dim conserved$ = ""

        For j As Integer = 0 To MSA(0).Length - 1
            Dim index% = j
            Dim column = MSA.Select(Function(s) s(index)).ToArray

            If column.Distinct.Count = 1 Then
                conserved &= "*"
            Else
                conserved &= " "
            End If
        Next

        If Not Trim(conserved).StringEmpty Then
            out.WriteLine(New String(" "c, maxNameWidth) & vbTab & conserved)
        End If

        out.Flush()
    End Sub
End Structure
