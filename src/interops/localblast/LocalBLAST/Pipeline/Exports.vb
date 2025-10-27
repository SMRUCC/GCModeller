Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus
Imports SMRUCC.genomics.Interops.NCBI.Extensions.Tasks.Models

Namespace Pipeline

    Public Module Exports

        <Extension>
        Public Iterator Function ExportHistResult(blast As IEnumerable(Of Query)) As IEnumerable(Of HitCollection)
            For Each query As Query In blast.SafeQuery
                Yield New HitCollection With {
                    .QueryName = query.QueryName,
                    .description = query.QueryName,
                    .hits = query.SubjectHits _
                        .ReadSubjectHits _
                        .ToArray
                }
            Next
        End Function

        <Extension>
        Private Iterator Function ReadSubjectHits(subjects As IEnumerable(Of SubjectHit)) As IEnumerable(Of Hit)
            For Each subj As SubjectHit In subjects.SafeQuery
                Yield New Hit With {
                    .hitName = subj.Name,
                    .identities = subj.Score.Identities,
                    .positive = subj.Score.Positives,
                    .tag = subj.Name,
                    .evalue = subj.Score.Expect,
                    .gaps = subj.Score.Gaps,
                    .score = subj.Score.Score
                }
            Next
        End Function
    End Module
End Namespace