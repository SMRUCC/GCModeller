#Region "Microsoft.VisualBasic::0b7cf1318bdf763bbbd37db1c1356c85, analysis\ProteinTools\ProteinMatrix\Linclust\ClusterExporter.vb"

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

    '   Total Lines: 118
    '    Code Lines: 65 (55.08%)
    ' Comment Lines: 30 (25.42%)
    '    - Xml Docs: 50.00%
    ' 
    '   Blank Lines: 23 (19.49%)
    '     File Size: 5.36 KB


    '     Module ClusterExporter
    ' 
    '         Function: ExportClusters
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' Linclust 聚类结果导出模块
'
' 将 Linclust.Cluster 返回的 ClusterResult 连同原始 FastaSeq 序列,
' 转换为两个 CSV 结果文件:
'   1. FamilyExports.csv  —— 每个簇一行(FamilyExports 集合)
'   2. SequenceCluster.csv —— 每个簇内成员一行(SequenceCluster 集合)
'
' 本模块只引用 FastaSeq 的 Title / SequenceData 字符串,不复制序列内容,
' 且使用 sciBASIC# 流式 SaveTo 写出,符合此前对大对象堆(LOH)的内存看护约束。

Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Data.Framework.IO
Imports Microsoft.VisualBasic.Data.Framework.IO.Linq
Imports SMRUCC.genomics.Model.MotifGraph.ProteinStructure
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports System.Text

Namespace Linclust

    Public Module ClusterExporter

        ''' <summary>
        ''' 将 Linclust 聚类结果导出为两个 CSV 文件。
        ''' </summary>
        ''' <param name="seqs">参与聚类的原始序列数组(与 ClusterResult 中的序列 ID 索引一一对应)</param>
        ''' <param name="result">Linclust.Cluster 返回的聚类结果</param>
        ''' <param name="outputDir">导出目录(不存在时自动创建)</param>
        ''' <returns>
        ''' 成功导出的两个文件路径:(FamilyExports.csv, SequenceCluster.csv);
        ''' 若 result 为空或异常则返回空元组。
        ''' </returns>
        ''' <remarks>
        ''' family_id 采用 1-based 编号 family_{i+1},与同仓库 FamilyCluster 约定一致。
        ''' SequenceCluster.score 取自 Cluster.memberScores(成员相对代表的比对 score;
        ''' 代表序列自身记为簇内成员比对 score 的最大值)。
        ''' </remarks>
        Public Function ExportClusters(seqs As FastaSeq(), result As ClusterResult, outputDir As String) As (familyCsv$, sequenceCsv$)
            If result Is Nothing OrElse result.clusters Is Nothing OrElse seqs Is Nothing Then
                Call Console.WriteLine("[ClusterExporter] 跳过: result 或 seqs 为空。")
                Return (Nothing, Nothing)
            End If

            Call System.IO.Directory.CreateDirectory(outputDir)

            ' ---------- 1. 生成 FamilyExports 集合(每簇一行) ----------
            Dim families As New List(Of FamilyExports)
            ' ---------- 2. 生成 SequenceCluster 集合(每成员一行) ----------
            Dim sequences As New List(Of SequenceCluster)

            Dim totalMembers As Integer = 0
            Dim tqdm As New ProgressBar

            For i As Integer = 0 To result.clusters.Count - 1
                Dim c = result.clusters(i)
                Dim familyId = $"family_{i + 1}"

                If c Is Nothing OrElse c.members Is Nothing OrElse c.representative < 0 OrElse c.representative >= seqs.Length Then
                    Call Console.WriteLine($"[ClusterExporter] 警告: 簇 #{i + 1} 数据不完整,跳过。")
                    Continue For
                End If

                Dim repr = seqs(c.representative)

                ' 簇级导出
                families.Add(New FamilyExports With {
                    .family_id = familyId,
                    .members = c.members.Count,
                    .representative = repr.Title,
                    .rep_seq = repr.SequenceData
                })

                ' 成员级导出(逐成员一行)
                For Each memberId As Integer In c.members
                    If memberId < 0 OrElse memberId >= seqs.Length Then
                        Continue For
                    End If

                    Dim m = seqs(memberId)
                    Dim score As Double = 0.0

                    If c.memberScores IsNot Nothing AndAlso c.memberScores.ContainsKey(memberId) Then
                        score = c.memberScores(memberId)
                    End If

                    sequences.Add(New SequenceCluster With {
                        .seq_title = m.Title,
                        .family_id = familyId,
                        .score = score,
                        .seq = m.SequenceData
                    })
                    totalMembers += 1
                Next

                Call tqdm.Progress(i, result.clusters.Count)
            Next

            tqdm.Finish()

            ' ---------- 3. 写出 CSV ----------
            Dim familyCsv = System.IO.Path.Combine(outputDir, "FamilyExports.csv")
            Dim sequenceCsv = System.IO.Path.Combine(outputDir, "SequenceCluster.csv")

            ' strict:=True 确保所有基本类型属性均按属性名输出为列(含无 ColumnAttribute 标注的属性)
            Call families.SaveTo(familyCsv, encoding:=Encoding.UTF8)
            Call sequences.SaveTo(sequenceCsv, encoding:=Encoding.UTF8)

            Call Console.WriteLine($"[ClusterExporter] 导出完成: {families.Count} 个簇, {totalMembers} 条成员序列。")
            Call Console.WriteLine($"    -> {familyCsv}")
            Call Console.WriteLine($"    -> {sequenceCsv}")

            Return (familyCsv, sequenceCsv)
        End Function

    End Module

End Namespace

