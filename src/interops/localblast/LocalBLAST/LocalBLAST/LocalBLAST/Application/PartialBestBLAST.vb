Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Text
Imports Microsoft.VisualBasic.DocumentFormat.Csv

Namespace LocalBLAST.Application

    ''' <summary>
    ''' 部分最佳比对BLAST
    ''' </summary>
    ''' <remarks></remarks>
    Public Class PartialBestBLAST

        ''' <summary>
        ''' 本地BLAST的中间服务
        ''' </summary>
        ''' <remarks></remarks>
        Protected Friend LocalBLAST As Global.LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.Programs.BLASTPlus
        Protected Friend WorkDir As String

        Sub New(LocalBLAST As LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.Programs.BLASTPlus, WorkDir As String)
            Me.WorkDir = WorkDir
            Me.LocalBLAST = LocalBLAST
        End Sub

        ''' <summary>
        ''' 执行BLASTP操作，返回单项部分匹配的最佳匹配的蛋白质列表
        ''' </summary>
        ''' <param name="Query"></param>
        ''' <param name="Subject">匹配上Query的部分区域但是Subject对象为完全匹配的目标</param>
        ''' <param name="HitsGrepMethod">对Hit蛋白质序列的FASTA数据库中的基因号的解析方法</param>
        ''' <param name="QueryGrepMethod">对Query蛋白质序列的FASTA数据库中的基因号的解析方法</param>
        ''' <returns>返回双相匹配的BestHit列表</returns>
        ''' <remarks></remarks>
        Public Function Peformance(Query As String, Subject As String,
                                   QueryGrepMethod As TextGrepMethod, HitsGrepMethod As TextGrepMethod,
                                   Optional e As String = "1e-3", Optional ExportAll As Boolean = False) As DocumentStream.File

            Call LocalBLAST.FormatDb(Subject, LocalBLAST.MolTypeProtein).Start(WaitForExit:=True)
            '在这里将evalue值设置为较大的一个数值是因为做部分比对的时候，Query会有一部分无法被比对上，会导致evalue值过大，取较小的evalue值会将某些阳性数据丢弃
            Call LocalBLAST.Blastp(Query, Subject, String.Format("{0}/{1}_.vs._{2}.txt", WorkDir, FileIO.FileSystem.GetName(Query), FileIO.FileSystem.GetName(Subject)), 1000000).Start(WaitForExit:=True)
            Dim LogOutput = DirectCast(LocalBLAST.GetLastLogFile, BLASTOutput.BlastPlus.v228)
            Call LogOutput.Grep(QueryGrepMethod, HitsGrepMethod)

            Dim result = If(ExportAll, ExportAllPartialBesthit(LogOutput), ExportPartialBesthit(LogOutput))
            Return result
        End Function

        Public Shared Function ExportPartialBesthit(LogOutput As BLASTOutput.BlastPlus.v228) As DocumentStream.File
            Dim List As List(Of LocalBLAST.Application.BBH.BestHit) = New List(Of LocalBLAST.Application.BBH.BestHit)
            For Each Query In LogOutput.Queries
                Dim cols = GetPartialBesthit(Query)
                If cols.IsNullOrEmpty Then
                    Call List.Add(cols.First)
                Else
                    Call List.Add(New Application.BBH.BestHit With {.QueryName = Query.QueryName, .HitName = BLASTOutput.IBlastOutput.HITS_NOT_FOUND})
                End If
            Next

            Return List.ToCsvDoc(False)
        End Function

        Public Shared Function ExportAllPartialBesthit(LogOutput As NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus.v228) As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File
            Dim List As List(Of LocalBLAST.Application.BBH.BestHit) = New List(Of LocalBLAST.Application.BBH.BestHit)
            For Each Query In LogOutput.Queries
                Call List.AddRange(GetPartialBesthit(Query))
            Next

            Return List.ToCsvDoc(False)
        End Function

        Protected Friend Shared Function GetPartialBesthit(Query As LocalBLAST.BLASTOutput.BlastPlus.Query) As LocalBLAST.Application.BBH.BestHit()
            If Query.SubjectHits.IsNullOrEmpty Then
                Return New LocalBLAST.Application.BBH.BestHit() {}
            Else
                Dim LQuery = (From hit In Query.SubjectHits Where System.Math.Abs(hit.LengthHit - hit.Length) / hit.Length < 0.05 AndAlso (hit.Hsp.First.Query.Left >= 0 AndAlso hit.Hsp.Last.Query.Right <= Query.QueryLength) Select hit).ToArray
                Dim result = (From hit In LQuery Let score = hit.Score
                              Let partialBesthit = New LocalBLAST.Application.BBH.BestHit With {
                                  .evalue = score.Expect, .hit_length = hit.Length, .HitName = hit.Name, .identities = score.Identities.Value,
                                  .length_hit = hit.LengthHit, .length_hsp = score.Gaps.Denominator, .length_query = score.Gaps.Numerator, .Positive = score.Positives.Value,
                                  .query_length = Query.QueryLength, .QueryName = Query.QueryName, .Score = score.RawScore}
                              Select partialBesthit Order By partialBesthit.Score Descending).ToArray
                Return result
            End If
        End Function
    End Class
End Namespace