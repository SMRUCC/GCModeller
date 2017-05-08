#Region "Microsoft.VisualBasic::45a3c948bce5804e9dff9aad0524bd8c, ..\interops\localblast\LocalBLAST\LocalBLAST\BlastOutput\Reader\Blast+\2.2.28\v228.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
'       xie (genetics@smrucc.org)
' 
' Copyright (c) 2016 GPL3 Licensed
' 
' 
' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.Text
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Extensions
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.ComponentModel
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.Views
Imports SMRUCC.genomics.SequenceModel

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

        Public Overrides Function Save(Optional FilePath As String = "", Optional Encoding As Encoding = Nothing) As Boolean
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
            Return (From query As Query In Queries Select __generateLine(query, coverage, identities)).ToArray
        End Function

        ''' <summary>
        ''' 导出最佳的符合条件的
        ''' </summary>
        ''' <param name="query"></param>
        ''' <param name="coverage"></param>
        ''' <param name="identities"></param>
        ''' <returns></returns>
        Private Shared Function __generateLine(query As Query, coverage As Double, identities As Double) As BestHit
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
                    .Score = Score.RawScore
                    .evalue = Score.Expect
                    .identities = Score.Identities.Value
                    .Positive = Score.Positives.Value
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
        Public Shared Function SBHLines(Query As Query, coverage As Double, identities As Double) As LocalBLAST.Application.BBH.BestHit()
            Dim Besthits As SubjectHit() = Query.GetBesthits(coverage, identities)

            If Besthits.IsNullOrEmpty Then
                Return New BestHit() {EmptyHit(Query)}
            Else
                Return ExportBesthits(Query.QueryName, Query.QueryLength, Besthits)
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
            Return LinqAPI.Exec(Of BestHit) <=
 _
                From hit As SubjectHit
                In query.SubjectHits
                Select New BestHit With {
                    .QueryName = query.QueryName,
                    .HitName = hit.Name,
                    .evalue = hit.Score.Expect,
                    .hit_length = hit.Length,
                    .identities = hit.Score.Identities.Value,
                    .length_hit = hit.LengthHit,
                    .length_hsp = hit.LengthQuery,
                    .length_query = hit.LengthQuery,
                    .Positive = hit.Score.Positives.Value,
                    .query_length = query.QueryLength,
                    .Score = hit.Score.Score
               }
        End Function

        ''' <summary>
        ''' Exports all of the hits which it meet the condition of threshold.(导出所有的单项最佳)
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function ExportAllBestHist(Optional coverage As Double = 0.5, Optional identities_cutoff As Double = 0.15) As LocalBLAST.Application.BBH.BestHit()
            Dim LQuery = LinqAPI.Exec(Of BestHit) <= From query As Query
                                                     In Queries
                                                     Select SBHLines(query, coverage, identities_cutoff) '
            Return LQuery
        End Function

        Public Shared Function ExportBesthits(QueryName As String, QueryLength As Integer, Besthits As SubjectHit()) As LocalBLAST.Application.BBH.BestHit()
            Dim locusId As String = QueryName.Split.First
            Dim def As String = Mid(QueryName, Len(locusId) + 1).Trim
            Dim RowQuery As BestHit() = LinqAPI.Exec(Of BestHit) <=
 _
                From besthit As SubjectHit
                In Besthits
                Let Score As Score = besthit.Score
                Select New BestHit With {
                    .QueryName = locusId,
                    .HitName = besthit.Name.Trim,
                    .query_length = QueryLength,
                    .hit_length = besthit.Length,
                    .Score = Score.RawScore,
                    .evalue = Score.Expect,
                    .identities = Score.Identities.Value,
                    .Positive = Score.Positives.Value,
                    .length_hit = besthit.LengthHit,
                    .length_query = besthit.LengthQuery,
                    .length_hsp = besthit.Score.Gaps.Denominator,
                    .description = def
                }

            Return RowQuery
        End Function

        Public Shared Function EmptyHit(Query As Query) As LocalBLAST.Application.BBH.BestHit
            Dim Row As New Application.BBH.BestHit With {
                .QueryName = Query.QueryName,
                .HitName = HITS_NOT_FOUND
            }
            Dim locusId As String = Query.QueryName.Split.First
            Dim def As String = Mid(Query.QueryName, Len(locusId) + 1).Trim

            Row.QueryName = locusId
            Row.description = def

            Return Row
        End Function

        ''' <summary>
        ''' 根据Query检查完整性
        ''' </summary>
        ''' <param name="source">主要是使用到Query序列之中的Title属性</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function CheckIntegrity(source As FASTA.FastaFile) As Boolean
            Dim LQuery =
                LinqAPI.DefaultFirst(Of Query()) <= From Fasta As FASTA.FastaToken
                                                    In source.AsParallel
                                                    Let GetQuery = __checkIntegrity(Fasta, Me.Queries)
                                                    Where GetQuery.IsNullOrEmpty  ' 空集合表示没有匹配的项目，则可能是不完整的结果
                                                    Select GetQuery
            Dim test As Boolean = LQuery Is Nothing  ' 不为空值，说明有空的记录，即匹配不上的记录，则说明blast操作是被中断的，需要重新做
            Return test
        End Function

        Private Shared Function __checkIntegrity(Fasta As FASTA.FastaToken, Queries As Query()) As Query()
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
