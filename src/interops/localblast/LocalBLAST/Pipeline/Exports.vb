Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus
Imports SMRUCC.genomics.Interops.NCBI.Extensions.Tasks.Models

Namespace Pipeline

    Public Module Exports

        <Extension>
        Public Iterator Function ExportHitsResult(blast As IEnumerable(Of Query), Optional grepName As Func(Of String, NamedValue(Of String)) = Nothing) As IEnumerable(Of HitCollection)
            For Each query As Query In blast.SafeQuery
                Yield New HitCollection With {
                    .QueryName = query.QueryName,
                    .description = query.QueryName,
                    .hits = query.SubjectHits _
                        .ReadSubjectHits(grepName) _
                        .ToArray
                }
            Next
        End Function

        <Extension>
        Private Iterator Function ReadSubjectHits(subjects As IEnumerable(Of SubjectHit), grepName As Func(Of String, NamedValue(Of String))) As IEnumerable(Of Hit)
            Dim hitname As String
            Dim tag As String

            For Each subj As SubjectHit In subjects.SafeQuery
                If grepName Is Nothing Then
                    hitname = subj.Name
                    tag = hitname
                Else
                    With grepName(subj.Name)
                        tag = .Name
                        hitname = .Value
                    End With
                End If

                Yield New Hit With {
                    .hitName = hitname,
                    .identities = subj.Score.Identities,
                    .positive = subj.Score.Positives,
                    .tag = tag,
                    .evalue = subj.Score.Expect,
                    .gaps = subj.Score.Gaps,
                    .score = subj.Score.Score
                }
            Next
        End Function
    End Module
End Namespace