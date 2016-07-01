#Region "Microsoft.VisualBasic::a8d5ddff9d484be1ff486b5b0d803443, ..\interops\localblast\LocalBLAST\LocalBLAST\BlastOutput\Reader\Blast+\2.2.28\v228.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
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
Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Extensions
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Terminal.Utility
Imports Microsoft.VisualBasic.Text
Imports Microsoft.VisualBasic.Text.Similarity
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
                For i As Integer = 0 To Queries.Count - 1
                    Queries(i).QueryName = Query(Queries(i).QueryName)
                Next
            End If
            If Not Hits Is Nothing Then
                For i As Integer = 0 To Queries.Count - 1
                    Dim HitList = Queries(i).SubjectHits
                    If HitList.IsNullOrEmpty Then
                        Continue For
                    End If
                    For j As Integer = 0 To HitList.Count - 1
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
            Return (From Query As Query In Queries Select __generateLine(Query, coverage, identities)).ToArray
        End Function

        ''' <summary>
        ''' 导出最佳的符合条件的
        ''' </summary>
        ''' <param name="query"></param>
        ''' <param name="coverage"></param>
        ''' <param name="identities"></param>
        ''' <returns></returns>
        Private Shared Function __generateLine(query As Query, coverage As Double, identities As Double) As BestHit
            Dim BestHit As SubjectHit = query.GetBestHit(coverage, identities)
            Dim Row As BestHit = New BestHit With {
                    .QueryName = query.QueryName
            }

            If BestHit Is Nothing Then
                Row.HitName = HITS_NOT_FOUND
            Else
                Dim Score As Score = BestHit.Score
                Row.HitName = BestHit.Name.Trim
                Row.query_length = query.QueryLength
                Row.hit_length = BestHit.Length
                Row.Score = Score.RawScore
                Row.evalue = Score.Expect
                Row.identities = Score.Identities.Value
                Row.Positive = Score.Positives.Value
                Row.length_hit = BestHit.LengthHit
                Row.length_query = BestHit.LengthQuery
                Row.length_hsp = BestHit.Score.Gaps.Denominator
            End If

            Return Row
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
            Dim LQuery As Views.Query() =
                LinqAPI.Exec(Of Views.Query) <= From query As Query
                                                In Me.Queries.AsParallel
                                                Let hitsOverview As BestHit() = __hitsOverview(query)
                                                Let getHits As BestHit() =
                                                    If(hitsOverview.IsNullOrEmpty,
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
            Dim Overview As New Overview With {
                .Queries = LQuery
            }
            Return Overview
        End Function

        Private Shared Function __hitsOverview(query As Query) As BestHit()
            Return LinqAPI.Exec(Of BestHit) <=
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
            Dim LQuery = From Query As Query In Queries Select SBHLines(Query, coverage, identities_cutoff) '
            Return LQuery.ToArray.MatrixToVector
        End Function

        Public Shared Function ExportBesthits(QueryName As String, QueryLength As Integer, Besthits As SubjectHit()) As LocalBLAST.Application.BBH.BestHit()
            Dim locusId As String = QueryName.Split.First
            Dim def As String = Mid(QueryName, Len(locusId) + 1).Trim
            Dim RowQuery = (From besthit In Besthits
                            Let Score As Score = besthit.Score
                            Let Row As BestHit = New BestHit With {
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
                            Select Row).ToArray
            Return RowQuery
        End Function

        Public Shared Function EmptyHit(Query As Query) As LocalBLAST.Application.BBH.BestHit
            Dim Row As LocalBLAST.Application.BBH.BestHit =
                New Application.BBH.BestHit With {
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
        ''' <param name="QuerySource">主要是使用到Query序列之中的Title属性</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function CheckIntegrity(QuerySource As FASTA.FastaFile) As Boolean
            Dim LQuery = (From Fasta As FASTA.FastaToken
                          In QuerySource.AsParallel
                          Let GetQuery = __checkIntegrity(Fasta, Me.Queries)
                          Where GetQuery.IsNullOrEmpty
                          Select GetQuery).ToArray.Count
            Return Not LQuery > 0 '大于零，说明有空的记录，即匹配不上的记录，则说明blast操作是被中断的，需要重新做
        End Function

        Private Shared Function __checkIntegrity(Fasta As FASTA.FastaToken, Queries As Query())
            Dim Title As String = Fasta.Title
            Dim GetLQuery = (From Query As Query
                             In Queries
                             Where FuzzyMatchString.Equals(Query.QueryName, Title)
                             Select Query).ToArray
            Return GetLQuery
        End Function
    End Class
End Namespace
