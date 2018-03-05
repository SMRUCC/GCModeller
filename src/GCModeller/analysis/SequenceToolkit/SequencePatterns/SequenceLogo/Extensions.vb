Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports ScannerMotif = SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif.Motif

Namespace SequenceLogo

    Public Module Extensions

        <Extension>
        Public Function CreateDrawingModel(motif As ScannerMotif) As DrawingModel
            Dim n% = motif.seeds.MSA.Length
            Dim E# = (1 / Math.Log(2)) * ((4 - 1) / (2 * n))

            Return New DrawingModel With {
                .Residues = motif _
                    .region _
                    .Select(Function(r)
                                Return New Residue With {
                                    .Alphabets = r.frequency _
                                        .Select(Function(b)
                                                    Return New Alphabet With {
                                                        .Alphabet = b.Key,
                                                        .RelativeFrequency = b.Value
                                                    }
                                                End Function) _
                                        .ToArray,
                                    .Position = r.index,
                                    .Bits = Residue.CalculatesBits(.ByRef, E, NtMol:=True).Bits
                                }
                            End Function) _
                    .ToArray,
                .En = E
            }
        End Function
    End Module
End Namespace