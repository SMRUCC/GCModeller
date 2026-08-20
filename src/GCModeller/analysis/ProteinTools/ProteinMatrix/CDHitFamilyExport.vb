' CDHit 聚类结果导出模块
'
' 将 CDHit.FindSimilar 返回的聚类结果(SimilarHit 集合)连同原始 FastaSeq 序列,
' 转换为两个 CSV 结果文件:
'   1. FamilyExports.csv  —— 每个簇一行(FamilyExports 集合)
'   2. SequenceCluster.csv —— 每个簇内成员一行(SequenceCluster 集合)
'
' 本模块只引用 FastaSeq 的 Title / SequenceData 字符串,不复制序列内容,
' 且使用 sciBASIC# 流式 SaveTo 写出,符合此前对大对象堆(LOH)的内存看护约束。
'
' 与 Linclust.ClusterExporter 的差异:
'   Linclust 的聚类结果以整数索引(memberId)引用序列,本模块使用的 CDHit
'   聚类结果以序列 Title 为键(SimilarHit.SeqID / Similar 字典键均为 Title),
'   故反查采用 Title -> FastaSeq 字典,而非整数下标。

Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm
Imports Microsoft.VisualBasic.Data.Framework
Imports SMRUCC.genomics.Analysis.SequenceAlignment
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Module CDHitFamilyExport

    ''' <summary>
    ''' 将 CDHit.FindSimilar 返回的聚类结果导出为两个 CSV 文件。
    ''' </summary>
    ''' <param name="seqs">参与聚类的原始序列数组(用于按 Title 反查序列内容)</param>
    ''' <param name="clusters">CDHit.FindSimilar(threshold) 返回的聚类簇</param>
    ''' <param name="outputDir">导出目录(不存在时自动创建)</param>
    ''' <returns>
    ''' 成功导出的两个文件路径:(FamilyExports.csv, SequenceCluster.csv);
    ''' 若 clusters 或 seqs 为空则返回空元组。
    ''' </returns>
    ''' <remarks>
    ''' family_id 采用 1-based 编号 family_{i+1},与同仓库 FamilyCluster / Linclust 约定一致。
    ''' CDHit 的 FindSimilar 中代表序列自身未写入 Similar 字典,故导出时显式将代表序列
    ''' 作为簇内第 1 个成员:self 行 score 取簇内相似度最大值(Single 簇记 0),
    ''' 其余成员行 score 取其在 Similar 字典中的相似度。
    ''' </remarks>
    Public Function ExportClusters(seqs As FastaSeq(), clusters As IEnumerable(Of SimilarHit), outputDir As String) As (familyCsv$, sequenceCsv$)
        Dim familyCsv = System.IO.Path.Combine(outputDir, "FamilyExports.csv")
        Dim sequenceCsv = System.IO.Path.Combine(outputDir, "SequenceCluster.csv")
        Dim totalMembers As Integer = 0

        With seqs.ExportClustersInternal(clusters, totalMembers:=totalMembers)
            Call .family.SaveTo(familyCsv, encoding:=Encoding.UTF8)
            Call .clusters.SaveTo(sequenceCsv, encoding:=Encoding.UTF8)

            Call Console.WriteLine($"[CDHitFamilyExport] 导出完成: { .family.Length} 个簇, {totalMembers} 条成员序列。")
            Call Console.WriteLine($"    -> {familyCsv}")
            Call Console.WriteLine($"    -> {sequenceCsv}")
        End With

        Return (familyCsv, sequenceCsv)
    End Function

    <Extension>
    Private Function ExportClustersInternal(seqs As FastaSeq(), clusters As IEnumerable(Of SimilarHit), ByRef totalMembers As Integer) As (family As FamilyExports(), clusters As SequenceCluster())
        ' ---------- 建立 Title -> FastaSeq 反查表(仅保存引用,不复制序列内容) ----------
        Dim seqByTitle As Dictionary(Of String, FastaSeq) = seqs.ToDictionary(Function(s) s.Title)
        ' ---------- 1. 生成 FamilyExports 集合(每簇一行) ----------
        Dim families As New List(Of FamilyExports)
        ' ---------- 2. 生成 SequenceCluster 集合(每成员一行) ----------
        Dim sequences As New List(Of SequenceCluster)
        ' 物化 clusters 以便统计总数与多遍遍历
        Dim clusterList As SimilarHit() = clusters.ToArray

        For Each i As Integer In TqdmWrapper.Range(0, clusterList.Length)
            Dim c = clusterList(i)
            Dim familyId = $"family_{i + 1}"

            If c Is Nothing OrElse String.IsNullOrEmpty(c.SeqID) Then
                Call Console.WriteLine($"[CDHitFamilyExport] 警告: 簇 #{i + 1} 数据不完整,跳过。")
                Continue For
            End If

            If Not seqByTitle.ContainsKey(c.SeqID) Then
                Call Console.WriteLine($"[CDHitFamilyExport] 警告: 簇 #{i + 1} 代表序列 Title={c.SeqID} 未在原始序列中找到,跳过。")
                Continue For
            End If

            Dim repr = seqByTitle(c.SeqID)
            ' 代表序列相对于自身的相似度记为其簇内相似度最大值(空簇记 0)
            Dim reprScore As Double = 0.0
            If c.Similar IsNot Nothing AndAlso c.Similar.Values.Any Then
                reprScore = c.Similar.Values.Max
            End If

            ' 簇级导出:成员数 = 代表(1) + 相似成员(Similar.Count)
            Dim memberCount = If(c.IsUniqued, 1, 1 + c.Similar.Count)

            families.Add(New FamilyExports With {
                .family_id = familyId,
                .members = memberCount,
                .representative = repr.Title,
                .rep_seq = repr.SequenceData
            })

            ' 成员级导出:代表作为第 1 个成员行
            sequences.Add(New SequenceCluster With {
                .seq_title = repr.Title,
                .family_id = familyId,
                .score = reprScore,
                .seq = repr.SequenceData
            })
            totalMembers += 1

            ' 其余相似成员逐行导出
            If c.Similar IsNot Nothing Then
                For Each kv In c.Similar
                    Dim memberTitle = kv.Key
                    If Not seqByTitle.ContainsKey(memberTitle) Then
                        Call Console.WriteLine($"[CDHitFamilyExport] 警告: 成员 Title={memberTitle} 未在原始序列中找到,跳过。")
                        Continue For
                    End If

                    Dim m = seqByTitle(memberTitle)
                    sequences.Add(New SequenceCluster With {
                        .seq_title = m.Title,
                        .family_id = familyId,
                        .score = kv.Value,
                        .seq = m.SequenceData
                    })
                    totalMembers += 1
                Next
            End If
        Next

        Return (families.ToArray, sequences.ToArray)
    End Function

    <Extension>
    Public Function ExportClusters(seqs As FastaSeq(), clusters As IEnumerable(Of SimilarHit)) As (family As FamilyExports(), clusters As SequenceCluster())
        If clusters Is Nothing OrElse seqs Is Nothing Then
            Call "[CDHitFamilyExport] 跳过: clusters 或 seqs 为空。".warning
            Return (Nothing, Nothing)
        Else
            Return seqs.ExportClustersInternal(clusters, -1)
        End If
    End Function
End Module
