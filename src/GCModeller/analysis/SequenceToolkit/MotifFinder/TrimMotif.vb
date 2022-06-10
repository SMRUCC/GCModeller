Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.SequenceTools.MSA
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.Patterns

Module TrimMotif

    <Extension>
    Public Function Cleanup(motif As SequenceMotif, Optional cutoff As Double = 0.5) As SequenceMotif
        Dim bits As Double() = motif.getBits
        Dim start As Integer = Scan0
        Dim ends As Integer = bits.Length - 1

        For i As Integer = 0 To bits.Length - 1
            If bits(i) < cutoff Then
                start = i
            Else
                Exit For
            End If
        Next

        For i As Integer = bits.Length - 1 To 0 Step -1
            If bits(i) < cutoff Then
                ends = i
            Else
                Exit For
            End If
        Next

        Return New SequenceMotif With {
            .seeds = motif.seeds.Cleanup(start, ends),
            .pvalue = motif.pvalue,
            .score = motif.score,
            .region = motif.region _
                .Skip(start) _
                .Take(ends - start) _
                .ToArray
        }
    End Function

    <Extension>
    Private Function Cleanup(seeds As MSAOutput, start As Integer, ends As Integer) As MSAOutput
        Dim MSA As New List(Of String)

        For Each line As String In seeds.MSA
            Call MSA.Add(Mid(line, start + 1, ends - start))
        Next

        Return New MSAOutput With {
            .cost = seeds.cost,
            .names = seeds.names,
            .MSA = MSA.ToArray
        }
    End Function

    <Extension>
    Private Function getBits(motif As SequenceMotif) As Double()
        Dim En As Double = SequenceMotif.E(nsize:=motif.seeds.size)
        Dim MSA As FastaFile = motif.seeds.ToFasta
        Dim f As PatternModel = PatternsAPI.Frequency(MSA)
        Dim bits As Double() = motif.region _
            .Select(Function(a, i)
                        Dim aa As IPatternSite = f(i)
                        Dim hi As Double = Probability.HI(aa)
                        Dim bit As Double = SequenceMotif.CalculatesBits(hi, En, NtMol:=True)

                        Return bit
                    End Function) _
            .ToArray

        Return bits
    End Function
End Module
