#Region "Microsoft.VisualBasic::9abc2786cf5ed553b99e47c5bd259d64, ..\localblast\LocalBLAST\LocalBLAST\LocalBLAST\Application\PartialBestBLAST.vb"

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

Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.Extensions
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus

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
        Protected Friend LocalBLAST As LocalBLAST.Programs.BLASTPlus
        Protected Friend WorkDIR As String

        Sub New(LocalBLAST As LocalBLAST.Programs.BLASTPlus, workDIR As String)
            Me.WorkDIR = workDIR
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
                                   Optional e As String = "1e-3", Optional ExportAll As Boolean = False) As IO.File

            Call LocalBLAST.FormatDb(Subject, LocalBLAST.MolTypeProtein).Start(waitForExit:=True)
            '在这里将evalue值设置为较大的一个数值是因为做部分比对的时候，Query会有一部分无法被比对上，会导致evalue值过大，取较小的evalue值会将某些阳性数据丢弃
            Call LocalBLAST.Blastp(Query, Subject, String.Format("{0}/{1}_.vs._{2}.txt", WorkDIR, FileIO.FileSystem.GetName(Query), FileIO.FileSystem.GetName(Subject)), 1000000).Start(waitForExit:=True)
            Dim LogOutput = DirectCast(LocalBLAST.GetLastLogFile, BLASTOutput.BlastPlus.v228)
            Call LogOutput.Grep(QueryGrepMethod, HitsGrepMethod)

            Dim result = If(ExportAll, ExportAllPartialBesthit(LogOutput), ExportPartialBesthit(LogOutput))
            Return result
        End Function

        Public Shared Function ExportPartialBesthit(LogOutput As BLASTOutput.BlastPlus.v228) As IO.File
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

        Public Shared Function ExportAllPartialBesthit(LogOutput As v228) As IO.File
            Dim List As New List(Of LocalBLAST.Application.BBH.BestHit)
            For Each Query In LogOutput.Queries
                Call List.AddRange(GetPartialBesthit(Query))
            Next

            Return List.ToCsvDoc(False)
        End Function

        Protected Friend Shared Function GetPartialBesthit(Query As Query) As BBH.BestHit()
            If Query.SubjectHits.IsNullOrEmpty Then
                Return New BBH.BestHit() {}
            Else
                Dim LQuery As SubjectHit() =
                    LinqAPI.Exec(Of SubjectHit) <= From hit As SubjectHit
                                                   In Query.SubjectHits
                                                   Where Math.Abs(hit.LengthHit - hit.Length) / hit.Length < 0.05 AndAlso
                                                       (hit.Hsp.First.Query.Left >= 0 AndAlso
                                                       hit.Hsp.Last.Query.Right <= Query.QueryLength)
                                                   Select hit
                Dim result As BBH.BestHit() =
                    LinqAPI.Exec(Of BBH.BestHit) <= From hit As SubjectHit
                                                    In LQuery
                                                    Let score = hit.Score
                                                    Let partialBesthit = New BBH.BestHit With {
                                                        .evalue = score.Expect,
                                                        .hit_length = hit.Length,
                                                        .HitName = hit.Name,
                                                        .identities = score.Identities.Value,
                                                        .length_hit = hit.LengthHit,
                                                        .length_hsp = score.Gaps.Denominator,
                                                        .length_query = score.Gaps.Numerator,
                                                        .Positive = score.Positives.Value,
                                                        .query_length = Query.QueryLength,
                                                        .QueryName = Query.QueryName,
                                                        .Score = score.RawScore
                                                    }
                                                    Select partialBesthit
                                                    Order By partialBesthit.Score Descending
                Return result
            End If
        End Function
    End Class
End Namespace
