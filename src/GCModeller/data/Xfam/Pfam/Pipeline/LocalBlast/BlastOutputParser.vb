Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus

Namespace Pipeline.LocalBlast

    Public Module BlastOutputParser

        ''' <summary>
        ''' PfamA as query, alignment with protein sequence as subjects
        ''' </summary>
        ''' <param name="query"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' 这个方向的比较结果比较准确，但是后续会面临一个数据量比较大的按照蛋白编号分组的问题出现
        ''' 在这里一个query就是一个pfam结构域
        ''' </remarks>
        <Extension>
        Public Iterator Function ParseDomainQuery(query As Query) As IEnumerable(Of PfamHit)
            Dim pfamHit$ = query.QueryName

            For Each hit As SubjectHit In query.SubjectHits.SafeQuery
                Dim queryName = hit.Name.GetTagValue(, trim:=True)
                Dim queryId = queryName.Name
                Dim queryDescribe = queryName.Value
                ' 因为比对的方向是pfam vs protein
                ' 所以subject location是pfam在目标蛋白序列上的位置
                Dim location As Location = hit.SubjectLocation

                Yield New PfamHit With {
                    .description = queryDescribe,
                    .HitName = pfamHit,
                    .QueryName = queryId,
                    .query_length = hit.Length,
                    .hit_length = query.QueryLength,
                    .length_hit = hit.LengthQuery,
                    .length_query = hit.LengthHit,
                    .length_hsp = hit.Score.Gaps.Denominator,
                    .evalue = hit.Score.Expect,
                    .identities = hit.Score.Identities,
                    .positive = hit.Score.Positives,
                    .score = hit.Score.Score,
                    .start = location.left,
                    .ends = location.right
                }
            Next
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
            Dim location As Location
            Dim pfamHit As PfamHit

            For Each hit As SubjectHit In query.SubjectHits.SafeQuery
                ' 因为比对的方向是protein vs pfam
                ' 所以query location是pfam在目标蛋白序列上的位置
                location = hit.QueryLocation
                pfamHit = New PfamHit With {
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
                    .start = location.left,
                    .ends = location.right
                }

                Yield pfamHit
            Next
        End Function
    End Module
End Namespace