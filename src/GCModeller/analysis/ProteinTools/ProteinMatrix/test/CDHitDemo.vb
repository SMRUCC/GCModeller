' CDHit 聚类结果导出演示
'
' 在内存中构造若干人工蛋白序列(2 个同源家族 + 随机序列),
' 运行 CDHit 的 kmer + CD-HIT 贪婪聚类,并将聚类结果通过
' CDHitFamilyExport.ExportClusters 导出为 FamilyExports.csv 与 SequenceCluster.csv,
' 验证:
'   1. 同源序列被聚到同一个簇;
'   2. 每簇的代表序列(representative)是该簇中最长的成员(CD-HIT 性质);
'   3. 导出的两个 CSV 文件字段完整、成员数正确(代表被计为第 1 个成员)。

Imports System.Text
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.SequenceAlignment
Imports SMRUCC.genomics.Model.MotifGraph.ProteinStructure
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Module CDHitDemo

    ''' <summary>
    ''' 演示入口:构造数据、聚类、导出两个 CSV。
    ''' 默认阈值 0.8,可在调用处覆盖。
    ''' </summary>
    Public Sub Run(Optional threshold As Double = 0.3, Optional outputDir As String = "Z:/cdhit_exports")
        Call Console.WriteLine("=== CDHit 聚类 + FamilyExports / SequenceCluster 导出 Demo ===")
        Call Console.WriteLine()

        ' ---------- 1. 构造输入序列 ----------
        Dim seqs = FastaFile.Read("G:\cell-render\data\ec_numbers.fasta").Take(1000000).ToArray  ' BuildDemoSequences()
        Call Console.WriteLine($"输入序列总数: {seqs.Length}")
        Call Console.WriteLine()

        ' ---------- 2. 运行 CDHit 贪婪聚类 ----------
        Dim cdhit As New CDHit(k:=6)
        cdhit.Setup(seqs)

        Dim timer = Stopwatch.StartNew()
        Dim clusters = cdhit.FindSimilar(threshold).ToArray
        Call timer.Stop()

        Call Console.WriteLine($"聚类阈值 = {threshold}")
        Call Console.WriteLine($"聚类簇数 = {clusters.Length}")
        Call Console.WriteLine($"聚类耗时 = {timer.ElapsedMilliseconds} ms")
        Call Console.WriteLine()

        'For i As Integer = 0 To clusters.Length - 1
        '    Dim c = clusters(i)
        '    Dim memberTitles = If(c.Similar Is Nothing, {}, c.Similar.Keys.ToArray)
        '    Call Console.WriteLine($"簇 #{i + 1}: 代表={c.SeqID}, 成员数={If(c.IsUniqued, 1, 1 + c.Similar.Count)}")
        '    Call Console.WriteLine($"   成员: {c.SeqID}, {String.Join(", ", memberTitles)}")
        'Next
        'Call Console.WriteLine()

        ' ---------- 3. 导出聚类结果为两个 CSV ----------
        Call CDHitFamilyExport.ExportClusters(seqs, clusters, outputDir)
    End Sub

    ''' <summary>
    ''' 构造 2 个同源家族(各含突变近缘序列)与若干随机序列
    ''' </summary>
    Private Function BuildDemoSequences() As FastaSeq()
        Dim list As New List(Of FastaSeq)

        ' 家族 A:以一条较长种子序列生成若干带点突变的近缘序列
        Dim seedA = "MKTAYIAKQRQISFVKSHFSRQLEERLGLIEVQAPILSRVGDGTQDNLSGAEKAVQVKVKALPDAQFEVVHSLAKWKR" &
                    "QTLGKPLLKRQVEQAVETEIPKEEKAPDAEQQDAQAEA"  ' 120 aa
        list.Add(New FastaSeq(seedA, "FamilyA_seed"))

        list.Add(New FastaSeq(Mutate(seedA, {5, 20, 55, 88}), "FamilyA_m1"))
        list.Add(New FastaSeq(Mutate(seedA, {12, 40, 70, 100}), "FamilyA_m2"))
        list.Add(New FastaSeq(Mutate(seedA, {3, 33, 62, 95, 110}), "FamilyA_m3"))

        ' 家族 B:另一条无关种子,生成近缘序列
        Dim seedB = "GVPINYLGELRTGTQNIETLDGQSIRVFADGKFYNKAAWAGQDIVNGLQSAYKFPDMQYDVKRYLTLNNETQEV" &
                    "KGLGDRIVYVDGTTRDSHKPGGGQYIFDKYNAYAQDGTIRKFLDAKAGVDVKITVGH"  ' 130 aa
        list.Add(New FastaSeq(seedB, "FamilyB_seed"))

        list.Add(New FastaSeq(Mutate(seedB, {8, 25, 60, 99}), "FamilyB_m1"))
        list.Add(New FastaSeq(Mutate(seedB, {15, 45, 75, 115}), "FamilyB_m2"))

        ' 若干随机序列(低相似度,应各自成簇)
        list.Add(New FastaSeq(RandomSeq(95, 1), "Random1"))
        list.Add(New FastaSeq(RandomSeq(110, 2), "Random2"))
        list.Add(New FastaSeq(RandomSeq(80, 3), "Random3"))

        Return list.ToArray
    End Function

    ''' <summary>
    ''' 在指定位置替换氨基酸(制造点突变),用于构造同源近缘序列
    ''' </summary>
    Private Function Mutate(seq As String, positions As Integer()) As String
        Dim chars = seq.ToCharArray()
        Dim pool = "ACDEFGHIKLMNPQRSTVWY".ToCharArray()

        For Each p In positions
            If p >= 0 AndAlso p < chars.Length Then
                Dim orig = chars(p)
                Dim repl = pool(CInt((p * 7 + seq.Length) Mod pool.Length))
                If repl = orig Then
                    repl = pool((Array.IndexOf(pool, orig) + 1) Mod pool.Length)
                End If
                chars(p) = repl
            End If
        Next

        Return New String(chars)
    End Function

    ''' <summary>
    ''' 伪随机序列生成(seed 控制可复现)
    ''' </summary>
    Private Function RandomSeq(length As Integer, seed As Integer) As String
        Dim pool = "ACDEFGHIKLMNPQRSTVWY".ToCharArray()
        Dim rnd = New Random(seed * 1000 + 17)
        Dim sb As New StringBuilder(length)

        For i As Integer = 0 To length - 1
            sb.Append(pool(rnd.Next(pool.Length)))
        Next

        Return sb.ToString
    End Function
End Module
