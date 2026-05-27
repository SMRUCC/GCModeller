Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Interops.NCBI.Extensions

Namespace Pipeline.LocalBlast

    Public Module DiamondHits

        <Extension>
        Public Iterator Function Parse(blastp As IEnumerable(Of DiamondAnnotation)) As IEnumerable(Of PfamHit)
            For Each hit As DiamondAnnotation In blastp
                Yield New PfamHit With {
                    .description = hit.QseqId,
                    .HitName = hit.QseqId,
                    .QueryName = hit.SseqId,
                    .start = hit.SStart,
                    .ends = hit.SEnd,
                    .evalue = hit.EValue,
                    .identities = hit.Pident,
                    .score = hit.BitScore
                }
            Next
        End Function
    End Module
End Namespace