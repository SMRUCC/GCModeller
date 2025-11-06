#Region "Microsoft.VisualBasic::8df896f14aeea2f7bf73750f21efe4a4, localblast\LocalBLAST\LocalBLAST\LocalBLAST\Application\NtMapping\MapsAPI.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 281
    '    Code Lines: 183 (65.12%)
    ' Comment Lines: 62 (22.06%)
    '    - Xml Docs: 67.74%
    ' 
    '   Blank Lines: 36 (12.81%)
    '     File Size: 12.11 KB


    '     Module MapsAPI
    ' 
    '         Function: __createObject, CreateObject, (+2 Overloads) Export, GetCoverage, setUnique
    '                   TrimAssembly, UniqueAlignment, Where
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Parallel.Linq
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.Abstract
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus

Namespace LocalBLAST.Application.NtMapping

    Public Module MapsAPI

        ''' <summary>
        ''' ```vbnet
        ''' Math.Abs(map.QueryRight - map.QueryLeft) / map.QueryLength
        ''' ```
        ''' </summary>
        ''' <param name="map"></param>
        ''' <returns></returns>
        <Extension> Public Function GetCoverage(map As BlastnMapping) As Double
            Return Math.Abs(map.QueryRight - map.QueryLeft) / map.QueryLength
        End Function

        Public Function Where(full As Boolean,
                              perfect As Boolean,
                              unique As Boolean,
                              Optional identities# = 0.9,
                              Optional logics As Logics = Logics.OrElse) As Func(Of BlastnMapping, Boolean)

            Dim tests As New List(Of Func(Of BlastnMapping, Boolean))

            If full Then
                tests += Function(x As BlastnMapping) x.AlignmentFullLength
            End If
            If perfect Then
                tests += Function(x As BlastnMapping) x.PerfectAlignment
            End If
            If unique Then
                tests += Function(x As BlastnMapping) x.Unique
            End If

            identities *= 100

            Dim preTest As Func(Of BlastnMapping, Boolean) = BuildAll(Of BlastnMapping)(logics, tests.ToArray)
            Dim test As Func(Of BlastnMapping, Boolean) = Function(x) preTest(x) AndAlso (x.identitiesValue >= identities)

            Return test
        End Function

        ''' <summary>
        ''' 从blastn日志之中导出Mapping的数据
        ''' </summary>
        ''' <param name="Query"></param>
        ''' <returns></returns>
        ''' 
        <Extension>
        Public Function CreateObject(Query As Query, topBest As Boolean) As BlastnMapping()
            Dim LQuery As BlastnMapping() = LinqAPI.Exec(Of BlastnMapping) _
 _
                () <= From hitMapping As SubjectHit
                      In Query.SubjectHits
                      Let blastnHitMapping As BlastnHit = DirectCast(hitMapping, BlastnHit)
                      Let result = Query.__createObject(blastnHitMapping)
                      Select result

            Call LQuery.setUnique

            If topBest Then
                LQuery = {LQuery.OrderByDescending(Function(m) m.Score).First}
            End If

            Return LQuery
        End Function

        ''' <summary>
        ''' Unique的判断原则：
        ''' 
        ''' 1. 假若一个query之中只含有一个hit，则为unique
        ''' 2. 假若一个query之中含有多个hit的话，假若只有一个hit是perfect类型的，则为unique
        ''' 3. 同一个query之中假若为多个perfect类型的hit的话，则不为unique
        ''' </summary>
        ''' <param name="data"></param>
        ''' <returns></returns>
        <Extension> Private Function setUnique(ByRef data As BlastnMapping()) As Boolean
            If data.Length = 1 Then
                data(Scan0).Unique = True
                Return True
            End If

            Dim perfects = data _
                .Where(Function(x) x.PerfectAlignment) _
                .ToArray

            For i As Integer = 0 To data.Length - 1
                data(i).Unique = False
            Next

            If perfects.Length = 0 Then
                Return False
            ElseIf perfects.Length = 1 Then  '只有perfect的被设置为真，其他的已经被设置为false了
                perfects(Scan0).Unique = True
                Return True
            Else
                Return False
            End If
        End Function

        ''' <summary>
        ''' 从blastn日志之中导出Mapping的数据
        ''' </summary>
        ''' <param name="Query"></param>
        ''' <param name="hitMapping"></param>
        ''' <returns></returns>
        <Extension> Private Function __createObject(query As Query, hitMapping As BlastnHit) As BlastnMapping
            Dim MappingView As New BlastnMapping With {
                .ReadQuery = query.QueryName,
                .Reference = hitMapping.Name,
                .Evalue = hitMapping.Score.Expect,
                .gapsValue = hitMapping.Score.Gaps.Value * 100,
                .GapsFraction = hitMapping.Score.Gaps.FractionExpr,
                .identitiesValue = hitMapping.Score.Identities.Value * 100,
                .IdentitiesFraction = hitMapping.Score.Identities.FractionExpr,
                .QueryLeft = hitMapping.QueryLocation.left,
                .QueryRight = hitMapping.QueryLocation.right,
                .RawScore = hitMapping.Score.RawScore,
                .Score = hitMapping.Score.Score,
                .ReferenceLeft = hitMapping.SubjectLocation.left,
                .ReferenceRight = hitMapping.SubjectLocation.right,
                .Strand = hitMapping.Strand,
                .QueryLength = query.QueryLength
            }         '.EffectiveSearchSpaceUsed = Query.EffectiveSearchSpace,
            '.H = Query.p.H,
            '.H_Gapped = Query.Gapped.H,
            '.K = Query.p.K,
            '.K_Gapped = Query.Gapped.K,
            '.Lambda = Query.p.Lambda,
            '.Lambda_Gapped = Query.Gapped.Lambda
            '}
            Return MappingView
        End Function

        ''' <summary>
        ''' 从blastn日志文件之中导出fastq对基因组的比对的结果
        ''' </summary>
        ''' <param name="blastnMapping"></param>
        ''' <param name="best">前提是query里面的hit的原有的顺序没有被破坏掉</param>
        ''' <returns></returns>
        <Extension>
        Public Function Export(blastnMapping As v228,
                               Optional best As Boolean = False,
                               Optional track$ = Nothing,
                               Optional parallel As Boolean = True) As BlastnMapping()

            Dim out As BlastnMapping()

            If Not best Then
                out = blastnMapping.Queries.Export
            Else
                out = Export(
                    blastnMapping _
                    .Queries _
                    .Select(Function(query) As Query
                                Dim copy As New Query(query)

                                If copy.SubjectHits.IsNullOrEmpty Then
                                    copy.SubjectHits = {}
                                Else
                                    copy.SubjectHits = {
                                    copy.SubjectHits.First
                                }
                                End If

                                Return copy
                            End Function), parallel)
            End If

            If Not String.IsNullOrEmpty(track) Then
                For Each x As BlastnMapping In out
                    x.Extensions = New Dictionary(Of String, String)
                    x.Extensions.Add(NameOf(track), track)
                Next
            End If

            Return out
        End Function

        <Extension>
        Public Function Export(lstQuery As IEnumerable(Of Query), Optional parallel As Boolean = True, Optional topBest As Boolean = False) As BlastnMapping()
            Dim LQuery As BlastnMapping() = lstQuery _
                .Populate(parallel:=parallel) _
                .Select(Function(q) MapsAPI.CreateObject(q, topBest)) _
                .ToVector

            Return LQuery
        End Function

        ''' <summary>
        ''' 按照条件 <see cref="BlastnMapping.Unique"/>=TRUE and <see cref="BlastnMapping.PerfectAlignment"/>=TRUE
        ''' 进行可用的alignment mapping结果的筛选
        ''' </summary>
        ''' <returns></returns>
        ''' 
        <Extension>
        Public Function TrimAssembly(data As IEnumerable(Of BlastnMapping)) As BlastnMapping()
            Dim sw = Stopwatch.StartNew
            Call $"Start of running {NameOf(TrimAssembly)} action...".debug
            Dim LQuery As BlastnMapping() =
                LQuerySchedule.LQuery(Of
                    BlastnMapping,
                    BlastnMapping)(data,
                                   Function(x) x,
                            where:=Function(x) x.Unique AndAlso
                            x.PerfectAlignment).ToArray
            Call $"[Job DONE!] .....{sw.ElapsedMilliseconds}ms.".debug
            Return LQuery
        End Function

        ''' <summary>
        ''' 为每一个query都赋值一个唯一的hit结果,如果需要比较真实的比对结果,则可以使用包含有重复值的besthit导出方法
        ''' </summary>
        ''' <param name="blast">假设query和hit的id编号都是已经经过修剪了的</param>
        ''' <returns></returns>
        ''' 
        <Extension>
        Public Function UniqueAlignment(blast As IEnumerable(Of BBHIndex)) As Dictionary(Of String, BBHIndex)
            ' 按照query来分组
            Dim alignhits As Dictionary(Of String, BBHIndex()) = blast _
                .GroupBy(Function(q) q.QueryName) _
                .ToDictionary(Function(g) g.Key,
                              Function(group)
                                  Return group _
                                      .OrderByDescending(Function(hit) hit.identities) _
                                      .ToArray
                              End Function)
            Dim uniqueSubjects As New Index(Of String)
            Dim outhits As New Dictionary(Of String, BBHIndex)

            ' 第一次循环,直接取出最好的结果
            For Each query In alignhits
                Dim hits As BBHIndex() = query.Value

                ' 因为在前面分组的时候是按照得分降序排序了的
                ' 所以只需要取出第一个即可
                If hits(Scan0).identities = 1 AndAlso uniqueSubjects(hits(Scan0).HitName) = -1 Then
                    ' 是完全一致的序列,并且编号也没有重复,则直接作为结果
                    outhits.Add(query.Key, hits(Scan0))
                    uniqueSubjects += hits(Scan0).HitName
                End If
            Next

            ' 第二次循环,对剩下的query取出top结果
            For Each query In alignhits.Where(Function(q) Not outhits.ContainsKey(q.Key))
                Dim hits As BBHIndex() = query.Value

                If hits.Any(Function(hit) hit.HitName <> IBlastOutput.HITS_NOT_FOUND) Then
                    ' 从前往后,一直取到没有重复的结果
                    Dim top As New BBHIndex With {.identities = -1}

                    For Each hit As BBHIndex In hits
                        If uniqueSubjects(hit.HitName) = -1 AndAlso hit.identities > top.identities Then
                            top = hit
                        End If
                    Next

                    If top.identities > -1 Then
                        outhits.Add(query.Key, top)
                        uniqueSubjects += top.HitName
                    Else
                        ' 这个query没有结果,则不添加到结果输出
                    End If
                Else
                    ' 这个query没有结果,则不添加到结果输出
                End If
            Next

            Return outhits
        End Function
    End Module
End Namespace
