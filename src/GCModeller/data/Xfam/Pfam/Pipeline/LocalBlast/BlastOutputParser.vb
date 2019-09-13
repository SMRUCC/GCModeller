Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus

Namespace Pipeline.LocalBlast

    Public Module BlastOutputParser

        ''' <summary>
        ''' PfamA as query, alignment with protein sequence as subjects
        ''' </summary>
        ''' <param name="query"></param>
        ''' <returns></returns>
        <Extension>
        Public Iterator Function ParseDomainQuery(query As Query) As IEnumerable(Of PfamHit)

        End Function

        ''' <summary>
        ''' The protein sequence as query input, alignment with pfamA domain sequence as subjects.
        ''' </summary>
        ''' <param name="query"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' 这个只需要解析query就好了，不需要做group就可以构建出一条蛋白的结构域注释结果
        ''' 在这里，一个hit，就是一个pfam结构域
        ''' </remarks>
        <Extension>
        Public Iterator Function ParseProteinQuery(query As Query) As IEnumerable(Of PfamHit)
            Dim queryName = query.QueryName.GetTagValue(, trim:=True)
            Dim queryId = queryName.Name
            Dim queryDescribe = queryName.Value

            For Each hit As SubjectHit In query.SubjectHits.SafeQuery
                Yield New PfamHit With {
                    .description = queryDescribe,
                    .HitName = hit.Name,
                    .evalue = hit.Score.Expect,
                    .QueryName = queryId,
                    .score = hit.Score.Score,
                    .query_length = query.QueryLength,
                    .length_query = hit.LengthQuery,
                    .hit_length = hit.Length,
                    .length_hit = hit.LengthHit,
                    .length_hsp = hit.Score.Gaps.Denominator,
                    .identities = hit.Score.Identities,
                    .positive = hit.Score.Positives,
                    .start = hit.SubjectLocation.Left,
                    .ends = hit.SubjectLocation.Right
                }
            Next
        End Function
    End Module
End Namespace