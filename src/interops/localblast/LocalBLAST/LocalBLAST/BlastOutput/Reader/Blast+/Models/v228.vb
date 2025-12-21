#Region "Microsoft.VisualBasic::e726bf365bcd6b91acd75834e10032eb, localblast\LocalBLAST\LocalBLAST\BlastOutput\Reader\Blast+\Models\v228.vb"

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

'   Total Lines: 270
'    Code Lines: 198 (73.33%)
' Comment Lines: 41 (15.19%)
'    - Xml Docs: 100.00%
' 
'   Blank Lines: 31 (11.48%)
'     File Size: 12.41 KB


'     Class v228
' 
'         Properties: ParameterSummary, Queries
' 
'         Function: Save
'         Delegate Function
' 
'             Function: __checkIntegrity, __generateLine, __hitsOverview, CheckIntegrity, EmptyHit
'                       ExportAllBestHist, ExportBestHit, ExportBesthits, ExportOverview, Grep
'                       SBHLines
' 
' 
' 
' /********************************************************************************/

#End Region

Imports System.Text
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Extensions
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Scripting
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.ComponentModel
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.Views
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace LocalBLAST.BLASTOutput.BlastPlus

    ''' <summary>
    ''' 2.2.28版本的BLAST+程序的日志输出文件
    ''' </summary>
    ''' <remarks>
    ''' 默认的文件编码是<see cref="System.Text.Encoding.UTF8"/>
    ''' </remarks>
    Public Class v228 : Inherits LocalBLAST.BLASTOutput.IBlastOutput

        <XmlElement> Public Property Queries As Query()
        <XmlElement> Public Property ParameterSummary As ParameterSummary

        Public Function Save(FilePath As String, Optional Encoding As Encoding = Nothing) As Boolean
            Return Me.GetXml.SaveTo(FilePath, Encoding)
        End Function

        Public Overloads Shared Widening Operator CType(path As String) As v228
            Return Parser.TryParse(path)
        End Operator

        Public Delegate Function QueryParser(str As String) As Query

        Public Overrides Function Grep(Query As TextGrepMethod, Hits As TextGrepMethod) As IBlastOutput
            If Not Query Is Nothing Then
                For i As Integer = 0 To Queries.Length - 1
                    Queries(i).QueryName = Query(Queries(i).QueryName)
                Next
            End If
            If Not Hits Is Nothing Then
                For i As Integer = 0 To Queries.Length - 1
                    Dim HitList = Queries(i).SubjectHits
                    If HitList.IsNullOrEmpty Then
                        Continue For
                    End If
                    For j As Integer = 0 To HitList.Length - 1
                        HitList(j).Name = Hits(HitList(j).Name)
                    Next
                Next
            End If

            Return Me
        End Function

        ''' <summary>
        ''' 从本日志文件之中导出BestHit表格(单项最佳的)
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function ExportBestHit(Optional coverage As Double = 0.5, Optional identities As Double = 0.15) As LocalBLAST.Application.BBH.BestHit()
            Return (From query As Query In Queries Select topHitResult(query, coverage, identities)).ToArray
        End Function

        ''' <summary>
        ''' 导出最佳的符合条件的
        ''' </summary>
        ''' <param name="query"></param>
        ''' <param name="coverage"></param>
        ''' <param name="identities"></param>
        ''' <returns></returns>
        Private Shared Function topHitResult(query As Query, coverage As Double, identities As Double) As BestHit
            Dim topHit As SubjectHit = query.GetBestHit(coverage, identities)
            Dim locusId As String = query.QueryName.Split.First
            Dim def As String = Mid(query.QueryName, Len(locusId) + 1).Trim
            Dim besthit As New BestHit With {
                .QueryName = locusId,
                .description = def
            }

            If topHit Is Nothing Then
                besthit.HitName = HITS_NOT_FOUND
            Else
                Dim Score As Score = topHit.Score

                With besthit
                    .QueryName = locusId
                    .HitName = topHit.Name.Trim
                    .query_length = query.QueryLength
                    .hit_length = topHit.Length
                    .score = Score.RawScore
                    .evalue = Score.Expect
                    .identities = Score.Identities.Value
                    .positive = Score.Positives.Value
                    .length_hit = topHit.LengthHit
                    .length_query = topHit.LengthQuery
                    .length_hsp = topHit.Score.Gaps.Denominator
                End With
            End If

            Return besthit
        End Function

        ''' <summary>
        ''' 导出所有符合条件的
        ''' </summary>
        ''' <param name="Query"></param>
        ''' <param name="coverage"></param>
        ''' <param name="identities"></param>
        ''' <returns></returns>
        Public Shared Function SBHLines(Query As Query, coverage#, identities#,
                                        Optional grepHitId As TextGrepMethod = Nothing,
                                        Optional keepsRawQueryName As Boolean = False) As BestHit()

            Dim Besthits As SubjectHit() = Query.GetBesthits(coverage, identities)

            If Besthits.IsNullOrEmpty Then
                Return New BestHit() {EmptyHit(Query)}
            Else
                Return ExportBesthits(Query.QueryName, Query.QueryLength, Besthits, grepHitId, keepsRawQueryName)
            End If
        End Function

        ''' <summary>
        ''' 不做任何筛选，导出所有的比对信息
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function ExportOverview() As Overview
            Dim LQuery As Views.Query() = LinqAPI.Exec(Of Views.Query) <=
                                                                         _
                From query As Query
                In Me.Queries.AsParallel
                Let hitsOverview As BestHit() = __hitsOverview(query)
                Let getHits As BestHit() = If(hitsOverview.IsNullOrEmpty,
                    New BestHit() {
                        New BestHit With {
                            .QueryName = query.QueryName,
                            .HitName = IBlastOutput.HITS_NOT_FOUND
                        }
                    }, hitsOverview)
                Select New Views.Query With {
                    .Id = query.QueryName,
                    .Hits = getHits
                }

            Return New Overview With {
                .Queries = LQuery
            }
        End Function

        Private Shared Function __hitsOverview(query As Query) As BestHit()
            Return LinqAPI.Exec(Of BestHit) _
                                            _
                () <= From hit As SubjectHit
                      In query.SubjectHits
                      Let identity As Double = hit.Score.Identities.Value
                      Let positive As Double = hit.Score.Positives.Value
                      Select New BestHit With {
                          .QueryName = query.QueryName,
                          .HitName = hit.Name,
                          .evalue = hit.Score.Expect,
                          .hit_length = hit.Length,
                          .identities = identity,
                          .length_hit = hit.LengthHit,
                          .length_hsp = hit.LengthQuery,
                          .length_query = hit.LengthQuery,
                          .positive = positive,
                          .query_length = query.QueryLength,
                          .score = hit.Score.Score
                      }
        End Function

        ''' <summary>
        ''' Exports all of the hits which it meet the condition of threshold.(导出所有的单项最佳)
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function ExportAllBestHist(Optional coverage As Double = 0.5, Optional identities_cutoff As Double = 0.15) As BestHit()
            Dim LQuery = LinqAPI.Exec(Of BestHit) _
                                                  _
                () <= From query As Query
                      In Queries
                      Select SBHLines(query, coverage, identities_cutoff) '

            Return LQuery
        End Function

        ''' <summary>
        ''' split name with default fasta header delimiters
        ''' </summary>
        Shared ReadOnly tokenFirst As New [Default](Of TextGrepMethod)(Function(hitName) hitName.Split(" "c, "|", CChar(vbTab)).First)

        Public Shared Function ExportBesthits(QueryName$, QueryLength%, Besthits As SubjectHit(),
                                              Optional grepHitId As TextGrepMethod = Nothing,
                                              Optional keepRawQueryName As Boolean = False) As BestHit()

            Dim locusID$ = If(keepRawQueryName, QueryName, QueryName.Split(" "c, "|", CChar(vbTab)).First)
            Dim getHitId As TextGrepMethod = grepHitId Or tokenFirst
            Dim sbh As BestHit() = LinqAPI.Exec(Of BestHit) _
                                                            _
                () <= From besthit As SubjectHit
                      In Besthits
                      Let Score As Score = besthit.Score
                      Let hitName = besthit.Name.Trim
                      Let hitID As String = getHitId(hitName)
                      Let rawScore = If(Score Is Nothing, DirectCast(besthit, BlastpSubjectHit).FragmentHits.Select(Function(s) s.Score.RawScore).Average, Score.RawScore)
                      Let exp = If(Score Is Nothing, DirectCast(besthit, BlastpSubjectHit).FragmentHits.Select(Function(s) s.Score.Expect).Average, Score.Expect)
                      Let identity = If(Score Is Nothing, DirectCast(besthit, BlastpSubjectHit).FragmentHits.Select(Function(s) s.Score.Identities.Value).Average, Score.Identities.Value)
                      Let pos = If(Score Is Nothing, DirectCast(besthit, BlastpSubjectHit).FragmentHits.Select(Function(s) s.Score.Positives.Value).Average, Score.Positives.Value)
                      Let gaps = If(Score Is Nothing, DirectCast(besthit, BlastpSubjectHit).FragmentHits.Select(Function(s) s.Score.Gaps.Value).Average, Score.Gaps.Value)
                      Select New BestHit With {
                          .QueryName = locusID,
                          .HitName = hitID,
                          .query_length = QueryLength,
                          .hit_length = besthit.Length,
                          .score = rawScore,
                          .evalue = exp,
                          .identities = identity,
                          .positive = pos,
                          .length_hit = besthit.LengthHit,
                          .length_query = besthit.LengthQuery,
                          .length_hsp = gaps,
                          .description = hitName  ' 因为在进行blast搜索的时候，query还是未知的，所以描述信息这里应该是取hits的
                      }

            Return sbh
        End Function

        Public Shared Function EmptyHit(query As Query) As BestHit
            Dim locusId As String = query.QueryName.Split.First
            Dim describ As String = Mid(query.QueryName, Len(locusId) + 1).Trim
            Dim sbh As New BestHit With {
                .HitName = HITS_NOT_FOUND,
                .QueryName = locusId,
                .description = describ
            }

            Return sbh
        End Function

        ''' <summary>
        ''' 根据Query检查完整性
        ''' </summary>
        ''' <param name="source">主要是使用到Query序列之中的Title属性</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function CheckIntegrity(source As FastaFile) As Boolean
            Dim empty = LinqAPI.DefaultFirst(Of Query()) _
                                                         _
                () <= From fasta As FastaSeq
                      In source.AsParallel
                      Let list = __checkIntegrity(fasta, Me.Queries)
                      Where list.IsNullOrEmpty  ' 空集合表示没有匹配的项目，则可能是不完整的结果
                      Select list

            Return empty Is Nothing  ' 不为空值，说明有空的记录，即匹配不上的记录，则说明blast操作是被中断的，需要重新做
        End Function

        Private Shared Function __checkIntegrity(Fasta As FASTA.FastaSeq, Queries As Query()) As Query()
            Dim Title As String = Fasta.Title
            Dim GetLQuery = LinqAPI.Exec(Of Query) <=
                                                     _
                From query As Query
                In Queries
                Where FuzzyMatching(query.QueryName, Title)
                Select query

            Return GetLQuery
        End Function
    End Class
End Namespace
