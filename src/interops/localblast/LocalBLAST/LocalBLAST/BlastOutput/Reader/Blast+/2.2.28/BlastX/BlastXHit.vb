Imports System.Text
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.Views

Namespace LocalBLAST.BLASTOutput.BlastPlus.BlastX

    Public Class BlastXHit : Inherits BBH.BestHit

        Public Property Frame As String

        Sub New()
        End Sub

        Sub New(query As Components.Query, hit As Components.HitFragment)
            Me.evalue = hit.Score.Expect
            Me.Frame = hit.ReadingFrameOffSet
            Me.HitName = hit.HitName
            Me.hit_length = hit.HitLen
            Me.identities = hit.Score.Identities
            Me.length_hit = hit.SubjectLength
            Me.length_hsp = hit.SubjectLength
            Me.length_query = hit.QueryLoci.FragmentSize
            Me.Positive = hit.Score.Positives
            Me.QueryName = query.QueryName
            Me.query_length = query.QueryLength
            Me.Score = hit.Score.Score
        End Sub

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

        Public Shared Function CreateObjects(query As Components.Query) As BlastXHit()
            Dim LQuery As BlastXHit() = LinqAPI.Exec(Of BlastXHit) <=
                From x As Components.HitFragment
                In query.Hits
                Select New BlastXHit(query, x)

            Return LQuery
        End Function
    End Class
End Namespace
