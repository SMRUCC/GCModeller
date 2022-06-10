Imports System.Runtime.CompilerServices

Module TrimMotif

    <Extension>
    Public Function Cleanup(motif As SequenceMotif, Optional cutoff As Double = 0.3) As SequenceMotif
        Dim bits As Double() = motif.getBits

    End Function

    <Extension>
    Private Function getBits(motif As SequenceMotif) As Double()
        Dim En As Double = SequenceMotif.E(nsize:=motif.seeds.size)
        Dim bits As Double() = motif.region _
            .Select(Function(a)
                        Return SequenceMotif.CalculatesBits(a.Hi, En, NtMol:=True)
                    End Function) _
            .ToArray

        Return bits
    End Function
End Module
