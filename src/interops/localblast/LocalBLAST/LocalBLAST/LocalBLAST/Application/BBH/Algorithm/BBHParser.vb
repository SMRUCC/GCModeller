﻿#Region "Microsoft.VisualBasic::5728d309f4a03e1b35ad88a31850e862, LocalBLAST\LocalBLAST\LocalBLAST\Application\BBH\Algorithm\BBHParser.vb"

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

    '     Module BBHParser
    ' 
    '         Function: __bhHash, __export, __generateBBH, __topBesthit, BBHScore
    '                   BBHTop, EnzymeClassification, get_DiReBh, GetBBHTop, GetDirreBhAll
    '                   (+2 Overloads) GetDirreBhAll2, hashSet, MapsNames, QueryNames, SBHScore
    '                   StripTopBest, TopHit
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.Extensions
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.ListExtensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.Expasy.AnnotationsTool
Imports SMRUCC.genomics.Assembly.Expasy.Database
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.Abstract

Namespace LocalBLAST.Application.BBH

    ''' <summary>
    ''' BBH解析的时候，是不会区分方向的，所以只要保证编号是一致的就会解析出结果，这个不需要担心
    ''' </summary>
    <Package("BBHParser", Publisher:="xie.guigang@gcmodeller.org")>
    Public Module BBHParser

        <Extension>
        Private Function hashSet(source As IEnumerable(Of BestHit),
                                identities As Double,
                                coverage As Double) As Dictionary(Of String, Dictionary(Of String, BestHit))

            Dim hash = (From x As BestHit
                        In source
                        Where x.IsMatchedBesthit(identities, coverage)
                        Select x
                        Group x By x.QueryName Into Group) _
                             .ToDictionary(Function(x) x.QueryName,
                                           Function(x) (From hit As BestHit
                                                        In x.Group
                                                        Select hit
                                                        Group hit By hit.HitName Into Group) _
                                                             .ToDictionary(Function(xx) xx.HitName,
                                                                           Function(xx) xx.Group.First))
            Return hash
        End Function

        ''' <summary>
        ''' 导出所有的双向最佳比对结果
        ''' </summary>
        ''' <param name="bhQvS"></param>
        ''' <param name="bhSvQ"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' 取出所有符合条件的单向最佳的记录
        ''' </remarks>
        '''
        <ExportAPI("BBH.All")>
        Public Function GetDirreBhAll2(bhSvQ As BestHit(),
                                       bhQvS As BestHit(),
                                       Optional identities As Double = -1,
                                       Optional coverage As Double = -1) As BiDirectionalBesthit()

            Dim sHash As Dictionary(Of String, Dictionary(Of String, BestHit)) = bhSvQ.hashSet(identities, coverage)
            Dim qHash As Dictionary(Of String, Dictionary(Of String, BestHit)) = bhQvS.hashSet(identities, coverage)
            Dim result As New List(Of BiDirectionalBesthit)

            VBDebugger.Mute = True

            For Each qId As String In (qHash.Keys.AsList + bhSvQ.Select(Function(x) x.HitName)).Distinct
                If String.IsNullOrEmpty(qId) OrElse String.Equals(qId, "HITS_NOT_FOUND") Then
                    Continue For
                End If

                If Not qHash.ContainsKey(qId) Then
                    result += New BiDirectionalBesthit With {.QueryName = qId}
                Else
                    For Each hit As BestHit In qHash(qId).Values
                        If Not sHash.ContainsKey(hit.HitName) Then
                            result += New BiDirectionalBesthit With {.QueryName = qId}
                        Else
                            If Not sHash(hit.HitName).ContainsKey(qId) Then
                                result += New BiDirectionalBesthit With {.QueryName = qId}
                            Else
                                Dim subject = sHash(hit.HitName)(qId)

                                result += New BiDirectionalBesthit With {
                                    .QueryName = qId,
                                    .HitName = hit.HitName,
                                    .Identities = Math.Max(hit.identities, subject.identities),
                                    .Length = hit.length_hit,
                                    .Positive = Math.Max(hit.Positive, subject.Positive),
                                    .Description = subject.description
                                }
                            End If
                        End If
                    Next
                End If
            Next

            VBDebugger.Mute = False

            Return (From x As BiDirectionalBesthit
                    In result
                    Select x
                    Group x By x.QueryName Into Group) _
                         .Select(Function(x) If(x.Group.Count = 1,
                         x.Group.ToArray,
                         (From o As BiDirectionalBesthit
                          In x.Group
                          Where Not String.IsNullOrEmpty(o.HitName)
                          Select o).ToArray)).ToVector
        End Function

        ''' <summary>
        ''' 通过排序得到最好的比对结果，在这里假设evalue都已经满足条件了
        ''' </summary>
        ''' <param name="hits">假设这里面的hits都是通过了cutoff了的数据</param>
        ''' <returns></returns>
        <Extension> Public Function TopHit(hits As IEnumerable(Of BestHit)) As BestHit
            Dim LQuery = LinqAPI.DefaultFirst(Of BestHit) _
 _
                () <= From x As BestHit
                      In hits
                      Select x
                      Order By x.SBHScore Descending

            Return LQuery
        End Function

        ''' <summary>
        ''' 因为Evalue是评价hsp的相似度的高低的因素，而identity和coverage则是评价序列整体相似度的因素，
        ''' 所以在这里仅需要identity和coverage这两个因素来计算得分就好了
        ''' </summary>
        ''' <param name="hit"></param>
        ''' <returns></returns>
        <Extension>
        Public Function SBHScore(hit As BestHit) As Double
            ' Dim E# = If(hit.evalue = 0R, 500, -Math.Log10(hit.evalue))
            Dim score# = (hit.identities * hit.coverage) ' * E
            Return score
        End Function

        Private Function __generateBBH(hits As String(), Id As String, row As BestHit) As BiDirectionalBesthit
            If Array.IndexOf(hits, Id) > -1 Then _
                Return New BiDirectionalBesthit With {  ' 可以双向匹配
                    .QueryName = row.QueryName,
                    .HitName = row.HitName,
                    .Length = row.query_length,
                    .Identities = row.identities,
                    .Positive = row.Positive
                }

            Return New BiDirectionalBesthit With {
                .QueryName = row.QueryName,
                .HitName = "",
                .Length = row.query_length
            }
        End Function

        ''' <summary>
        ''' 导出所有的双向最佳比对结果
        ''' </summary>
        ''' <param name="SvQ"></param>
        ''' <param name="QvS"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' 取出所有符合条件的单向最佳的记录
        ''' </remarks>
        '''
        <ExportAPI("BBH.All")>
        Public Function GetDirreBhAll2(SvQ As IO.File, QvS As IO.File) As BBH.BiDirectionalBesthit()
            Dim bhSvQ As LocalBLAST.Application.BBH.BestHit() = SvQ.AsDataSource(Of LocalBLAST.Application.BBH.BestHit)(False).ToArray
            Dim bhQvS As LocalBLAST.Application.BBH.BestHit() = QvS.AsDataSource(Of LocalBLAST.Application.BBH.BestHit)(False).ToArray
            Return GetDirreBhAll2(bhSvQ, bhQvS)
        End Function

        ''' <summary>
        ''' 获取双向的最佳匹配结果.(只取出第一个最好的结果)
        ''' </summary>
        ''' <param name="QvS"></param>
        ''' <param name="SvQ"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        '''
        <ExportAPI("BBH")>
        Public Function BBHTop(SvQ As IO.File, QvS As IO.File) As BBH.BiDirectionalBesthit()
            Dim bhSvQ As LocalBLAST.Application.BBH.BestHit() = SvQ.AsDataSource(Of LocalBLAST.Application.BBH.BestHit)(False).ToArray
            Dim bhQvS As LocalBLAST.Application.BBH.BestHit() = QvS.AsDataSource(Of LocalBLAST.Application.BBH.BestHit)(False).ToArray
            Dim besthits = GetBBHTop(bhQvS, bhSvQ)

            Return besthits
        End Function

        ''' <summary>
        ''' 导出所有的双向最佳比对结果，只要能够在双方的列表之中匹配上，则认为是最佳双向匹配
        ''' </summary>
        ''' <param name="SvQ"></param>
        ''' <param name="QvS"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' 取出所有符合条件的单向最佳的记录
        ''' </remarks>
        '''
        <ExportAPI("BBH.All")>
        Public Function GetDirreBhAll(SvQ As IO.File, QvS As IO.File) As IO.File
            Dim bbh = BBHParser.GetDirreBhAll2(SvQ, QvS)
            Return bbh.ToCsvDoc(False)
        End Function

        ''' <summary>
        ''' 获取双向的最佳匹配结果.(只取出第一个最好的结果)
        ''' </summary>
        ''' <param name="QvS"></param>
        ''' <param name="SvQ"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        '''
        <ExportAPI("BBH")>
        Public Function get_DiReBh(SvQ As IO.File, QvS As IO.File) As IO.File
            Dim besthits = BBHTop(SvQ, QvS)
            Return besthits.ToCsvDoc(False)
        End Function

        ''' <summary>
        ''' 假若没有最佳比对，则HitName为空值
        ''' </summary>
        ''' <param name="Query"></param>
        ''' <param name="SubjectVsQuery"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function __topBesthit(Query As BestHit, SubjectVsQuery As BestHit()) As BiDirectionalBesthit
            If SubjectVsQuery.IsNullOrEmpty Then '匹配不上，则返回空的hitname
                Return New BiDirectionalBesthit With {
                    .QueryName = Query.QueryName,
                    .Length = Query.query_length
                }
            End If

            Dim Subject = SubjectVsQuery.First()
            Dim HitsName As String = Subject.HitName  'Subject对象为反向比对结果，其Hitname属性自然为正向比对的Query对象属性
            Dim BestHit = New BiDirectionalBesthit With {
                .QueryName = Query.QueryName,
                .Length = Query.query_length
            }

            If String.Equals(Query.QueryName, HitsName) Then '可以双向匹配
                BestHit.HitName = Query.HitName
                BestHit.Identities = Math.Max(Query.identities, Subject.identities)
                BestHit.Positive = Math.Max(Query.Positive, Subject.Positive)
            End If

            Return BestHit
        End Function

        ''' <summary>
        ''' 按照阈值构建最好的比对结果
        ''' </summary>
        ''' <param name="source"></param>
        ''' <param name="identities"></param>
        ''' <param name="coverage"></param>
        ''' <returns></returns>
        <Extension>
        Private Function __bhHash(source As IEnumerable(Of BestHit),
                                  identities As Double,
                                  coverage As Double) As Dictionary(Of String, BestHit)

            Return (From x As BestHit
                    In source
                    Where x.IsMatchedBesthit(identities, coverage)
                    Select x
                    Group x By x.QueryName Into Group) _
                         .ToDictionary(Function(x) x.QueryName,
                                       Function(x) x.Group.TopHit)
        End Function

        <Extension>
        Public Function BBHScore(bbh As BiDirectionalBesthit) As Double
            Return bbh.Length * bbh.Identities
        End Function

        <Extension>
        Public Function StripTopBest(bbh As IEnumerable(Of BiDirectionalBesthit), Optional score As Func(Of BiDirectionalBesthit, Double) = Nothing) As BiDirectionalBesthit()
            Dim evaluate As Func(Of BiDirectionalBesthit, Double) = score Or New Func(Of BiDirectionalBesthit, Double)(AddressOf BBHScore).AsDefault
            Dim queries = bbh _
                .GroupBy(Function(h) h.QueryName) _
                .Select(Function(g)
                            Dim list = g.ToArray

                            If list.Length = 1 Then
                                Return list.First
                            Else
                                Return list.OrderByDescending(evaluate).First
                            End If
                        End Function) _
                .ToArray

            Return queries
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function QueryNames(Of T As I_BlastQueryHit)(maps As IEnumerable(Of T)) As String()
            Return maps.Select(Function(q) q.QueryName).ToArray
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function MapsNames(Of T As I_BlastQueryHit)(maps As IEnumerable(Of T)) As String()
            Return maps.Select(Function(q) q.HitName).ToArray
        End Function

        ''' <summary>
        ''' HITS_NOT_FOUND
        ''' </summary>
        Public Const HITS_NOT_FOUND$ = NameOf(HITS_NOT_FOUND)

        ''' <summary>
        ''' Only using the first besthit paired result for the orthology data, if the query have no matches then using an empty string for the hit name.
        ''' (只使用第一个做为最佳的双向结果，假若匹配不上，Hitname属性会为空字符串)
        ''' </summary>
        ''' <param name="qvs"></param>
        ''' <param name="svq"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' query: 待注释的query，功能未知
        ''' subject: 数据库之中的已知功能的蛋白，则bbh的输出结果的注释信息来自于subject.
        ''' 
        ''' 因为qvs表示query vs subject，所以sbh的注释结果来自于subject，则在这里的注释功能描述结果则来自于qvs之中
        ''' </remarks>
        <ExportAPI("BBH")>
        Public Function GetBBHTop(qvs As BestHit(), svq As BestHit(), Optional identities# = -1, Optional coverage# = -1) As BiDirectionalBesthit()
            Dim qHash As Dictionary(Of String, BestHit) = qvs.__bhHash(identities, coverage)
            Dim shash As Dictionary(Of String, BestHit) = svq.__bhHash(identities, coverage)
            Dim result As New List(Of BiDirectionalBesthit)

            VBDebugger.Mute = True

            ' 得到所有的query编号
            ' 然后按照这个编号来查找匹配的结果
            For Each qId As String In (qHash.Keys.AsList + shash.Values.Select(Function(x) x.HitName)) _
                .Distinct _
                .Where(Function(id)
                           Return Not id.StringEmpty AndAlso Not id.TextEquals(HITS_NOT_FOUND)
                       End Function)

                Dim query As BestHit = qHash.TryGetValue(qId)

                If query Is Nothing Then
                    result += New BiDirectionalBesthit With {
                        .QueryName = qId,
                        .HitName = HITS_NOT_FOUND
                    }
                Else
                    ' 按照query对应的hitName在subject列表之中查找
                    ' 得到subject，然后再比较一下queryName是否是一致的
                    ' 如果一致，则存在一个匹配结果
                    Dim subject As BestHit = shash.TryGetValue(query.HitName)

                    If subject Is Nothing Then
                        result += New BiDirectionalBesthit With {
                            .QueryName = qId,
                            .HitName = HITS_NOT_FOUND
                        }
                    ElseIf Not subject.HitName.TextEquals(query.QueryName) Then
                        ' 找到了目标，但是结果不一致
                        result += New BiDirectionalBesthit With {
                            .QueryName = qId,
                            .HitName = HITS_NOT_FOUND
                        }
                    Else
                        result += New BiDirectionalBesthit With {
                            .QueryName = qId,
                            .HitName = query.HitName,
                            .Identities = Math.Max(query.identities, subject.identities),
                            .Length = query.length_hit,
                            .Positive = Math.Max(query.Positive, subject.Positive),
                            .Description = query.description
                        }
                    End If
                End If
            Next

            VBDebugger.Mute = False

            Return result.ToArray
        End Function

        <ExportAPI("EnzymeClassification")>
        Public Function EnzymeClassification(Expasy As NomenclatureDB, bh As BBH.BestHit()) As T_EnzymeClass_BLAST_OUT()
            Dim EnzymeClasses As T_EnzymeClass_BLAST_OUT() =
                API.GenerateBasicDocument(Expasy.Enzymes)
            Dim LQuery As T_EnzymeClass_BLAST_OUT() =
                LinqAPI.Exec(Of T_EnzymeClass_BLAST_OUT) <= From enzPre As T_EnzymeClass_BLAST_OUT
                                                            In EnzymeClasses.AsParallel
                                                            Select enzPre.__export(bh)
            Return LQuery
        End Function

        <Extension>
        Private Function __export(enzPre As T_EnzymeClass_BLAST_OUT, bh As BBH.BestHit()) As T_EnzymeClass_BLAST_OUT()
            Dim getbhLQuery As BestHit() =
                LinqAPI.Exec(Of BestHit) <= From hit As BBH.BestHit
                                            In bh
                                            Where String.Equals(
                                                hit.HitName,
                                                enzPre.uniprot,
                                                StringComparison.OrdinalIgnoreCase)
                                            Select hit

            If Not getbhLQuery.IsNullOrEmpty Then
                Dim Linq As T_EnzymeClass_BLAST_OUT() =
                    LinqAPI.Exec(Of T_EnzymeClass_BLAST_OUT) <= From bhItem As BBH.BestHit
                                                                In getbhLQuery
                                                                Select New T_EnzymeClass_BLAST_OUT With {
                                                                    .Class = enzPre.Class,
                                                                    .EValue = bhItem.evalue,
                                                                    .Identity = bhItem.identities,
                                                                    .ProteinId = bhItem.QueryName,
                                                                    .uniprot = enzPre.uniprot
                                                                }
                Return Linq
            Else
                Return Nothing
            End If
        End Function
    End Module
End Namespace
