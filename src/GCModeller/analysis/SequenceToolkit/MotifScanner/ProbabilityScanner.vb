Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.DataMining.DynamicProgramming.SmithWaterman
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Abstract.Probability
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Public Module ProbabilityScanner

    <Extension>
    Public Function ScanSites(prob As IEnumerable(Of Residue), target As FastaSeq, Optional cutoff# = 0.6, Optional minW% = 6) As SimpleSegment()
        Dim core As New GSW(Of Residue)(prob.ToArray, target.ToResidues, AddressOf Compare, AddressOf Residue.Max)
        Dim result = core.GetMatches(cutoff * core.MaxScore)
        Dim out = result _
            .OrderByDescending(Function(m) m.Score) _
            .Where(Function(m) (m.ToB - m.FromB) >= minW) _
            .Select(Function(m)
                        Dim frag = target.CutSequenceLinear(m.RefLoci)
                        frag.ID = m.Score
                        Return frag
                    End Function) _
            .ToArray

        Return out
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function RefLoci(m As Match) As Location
        Return New Location(m.FromB, m.ToB)
    End Function

    Private Function Compare(prob As Residue, base As Residue) As Integer
        For Each b As Char In nt
            If base.frequency(b) = 1.0R Then
                Return prob.frequency(b) * 10
            End If
        Next

        With prob.frequency.ToArray
            Return .ByRef(Which.Min(.Values)) _
                   .Value * 10
        End With
    End Function

    ReadOnly nt = {"A"c, "T"c, "G"c, "C"c}

    <Extension>
    Public Function ToResidues(seq As FastaSeq) As Residue()
        Return seq _
            .SequenceData _
            .Select(Function(base)
                        Dim frq As New Dictionary(Of Char, Double)

                        For Each b In nt
                            If base = b Then
                                frq(b) = 1
                            Else
                                frq(b) = 0
                            End If
                        Next

                        Return New Residue With {
                            .frequency = frq
                        }
                    End Function) _
            .ToArray
    End Function
End Module
