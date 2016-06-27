Namespace LocalBLAST.BLASTOutput.Standard

    Partial Class BLASTOutput

        ''' <summary>
        ''' 获取本日志文件中的最好的序列比对匹配结果
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function ExportBestHit(Optional coverage As Double = 0.5, Optional identities_cutoff As Double = 0.15) As LocalBLAST.Application.BBH.BestHit()
            Dim LQuery = From Query In Me.Queries.AsParallel
                         Let BestHit = GetBestHit(Query, identities_cutoff)
                         Let Row = GenerateRow(Query, BestHit)
                         Select Row
                         Order By Row.QueryName Ascending '
            Return LQuery.ToArray
        End Function

        Private Shared Function GenerateRow(Query As Query, BestHit As Hit) As LocalBLAST.Application.BBH.BestHit
            If BestHit Is Nothing Then
                Return New LocalBLAST.Application.BBH.BestHit With {.QueryName = Query.QueryName, .HitName = HITS_NOT_FOUND, .query_length = Query.Length}
            Else
                Dim Score As LocalBLAST.BLASTOutput.ComponentModel.Score = BestHit.Score
                Return New LocalBLAST.Application.BBH.BestHit With {
                    .QueryName = Query.QueryName, .HitName = BestHit.Name, .query_length = Query.Length, .hit_length = BestHit.Length, .Score = Score.Score, .evalue = Score.Expect,
                    .identities = Score.Identities.Value, .Positive = Score.Positives.Value, .length_hsp = Score.Gaps.Value}
            End If
        End Function

        Private Shared Function GetBestHit(Query As Query, identities As Double) As Hit
            If Query.Hits Is Nothing OrElse Query.Hits.Count = 0 Then
                Return Nothing
            Else
                Dim LQuery = From Hit In Query.Hits
                             Where (Hit.Score.Identities > identities And Query.Length / Hit.Length > 0.5)
                             Select Hit
                             Order By Hit.Score.Expect Ascending '
                Dim Result = LQuery.ToArray
                If Result.Count > 0 Then
                    Return Result.First '找到了最佳的匹配
                Else
                    Return Nothing '在匹配项中没有找到任何符合条件的匹配项
                End If
            End If
        End Function
    End Class
End Namespace