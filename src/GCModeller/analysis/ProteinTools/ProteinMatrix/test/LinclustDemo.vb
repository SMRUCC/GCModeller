' Linclust 算法演示
'
' 在内存中构造若干人工蛋白序列(2 个同源家族 + 随机序列),
' 运行 Linclust 五阶段聚类流程,并打印关键结果,验证:
'   1. 同源序列被聚到同一个簇;
'   2. 每簇的代表序列(representative)是该簇中最长的成员;
'   3. 输出 k 值、簇数、每簇成员。
'
' 运行方式:作为独立入口,在 Program.Main 中调用 LinclustDemo.Run()。

Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.Model.MotifGraph.ProteinStructure.Linclust
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Model.MotifGraph.ProteinStructure

Public Module LinclustDemo

    Sub Run()
        ' Call RunTest()
        Call RunDemo()
    End Sub

    Sub RunDemo()
        ' ---------- 2. 运行聚类 ----------
        Dim opts As New LinclustOptions With {
            .m = 20,
            .seqidThreshold = 0.9,
            .coverage = 0.8,
            .evalue = 0.001
        }
        Dim seqs = FastaFile.Read("G:\cell-render\data\ec_numbers.fasta").Take(10000).ToArray
        Dim result = Linclust.Cluster(seqs, opts)

        ' ---------- 3. 打印结果 ----------
        Call Console.WriteLine($"自动选择的 k-mer 长度 k = {result.k}")
        Call Console.WriteLine($"聚类簇数 = {result.nClusters}")
        Call Console.WriteLine()

        For i As Integer = 0 To result.clusters.Count - 1
            Dim c = result.clusters(i)
            Dim repr = seqs(c.representative)
            Dim memberTitles = c.members _
                .Select(Function(id) seqs(id).Title) _
                .ToArray()

            Call Console.WriteLine($"簇 #{i + 1}: 代表={repr.Title} (len={repr.SequenceData.Length}), 成员数={c.members.Count}")
            Call Console.WriteLine($"   成员: {String.Join(", ", memberTitles)}")
        Next
        Call Console.WriteLine()
        Call Pause()
    End Sub

    ''' <summary>
    ''' 演示入口:构造数据、聚类、打印结果
    ''' </summary>
    Public Sub RunTest()
        Call Console.WriteLine("=== Linclust 蛋白序列无监督聚类 Demo ===")
        Call Console.WriteLine()

        ' ---------- 1. 构造输入序列 ----------
        Dim seqs = BuildDemoSequences()
        Call Console.WriteLine($"输入序列总数: {seqs.Length}")

        For Each s In seqs
            Call Console.WriteLine($"  - {s.Title,-18} len={s.SequenceData.Length}")
        Next
        Call Console.WriteLine()

        ' ---------- 2. 运行聚类 ----------
        Dim opts As New LinclustOptions With {
            .m = 20,
            .seqidThreshold = 0.9,
            .coverage = 0.8,
            .evalue = 0.001
        }

        Dim result = Linclust.Cluster(seqs, opts)

        ' ---------- 3. 打印结果 ----------
        Call Console.WriteLine($"自动选择的 k-mer 长度 k = {result.k}")
        Call Console.WriteLine($"聚类簇数 = {result.nClusters}")
        Call Console.WriteLine()

        For i As Integer = 0 To result.clusters.Count - 1
            Dim c = result.clusters(i)
            Dim repr = seqs(c.representative)
            Dim memberTitles = c.members _
                .Select(Function(id) seqs(id).Title) _
                .ToArray()

            Call Console.WriteLine($"簇 #{i + 1}: 代表={repr.Title} (len={repr.SequenceData.Length}), 成员数={c.members.Count}")
            Call Console.WriteLine($"   成员: {String.Join(", ", memberTitles)}")
        Next
        Call Console.WriteLine()

        ' ---------- 4. 基本断言(验证算法正确性) ----------
        Call Verify(seqs, result)
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

        ' 近缘成员:少量点突变(替换若干残基,保持 >90% 一致性)
        list.Add(New FastaSeq(Mutate(seedA, {5, 20, 55, 88}), "FamilyA_m1"))
        list.Add(New FastaSeq(Mutate(seedA, {12, 40, 70, 100}), "FamilyA_m2"))
        list.Add(New FastaSeq(Mutate(seedA, {3, 33, 62, 95, 110}), "FamilyA_m3"))

        ' 家族 B:另一条无关种子,生成近缘序列
        Dim seedB = "GVPINYLGELRTGTQNIETLDGQSIRVFADGKFYNKAAWAGQDIVNGLQSAYKFPDMQYDVKRYLTLNNETQEV" &
                    "KGLGDRIVYVDGTTRDSHKPGGGQYIFDKYNAYAQDGTIRKFLDAKAGVDVKITVGH"  ' 130 aa
        list.Add(New FastaSeq(seedB, "FamilyB_seed"))

        list.Add(New FastaSeq(Mutate(seedB, {8, 25, 60, 99}), "FamilyB_m1"))
        list.Add(New FastaSeq(Mutate(seedB, {15, 45, 75, 115}), "FamilyB_m2"))

        ' 若干随机序列(低相似度,应各自成簇或并入无关簇)
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
        Dim sb As New Text.StringBuilder(length)

        For i As Integer = 0 To length - 1
            sb.Append(pool(rnd.Next(pool.Length)))
        Next

        Return sb.ToString
    End Function

    ''' <summary>
    ''' 基本正确性验证:代表序列应为各自簇内最长成员;每个簇非空
    ''' </summary>
    Private Sub Verify(seqs As FastaSeq(), result As ClusterResult)
        Dim ok = True

        If result.nClusters <= 0 Then
            Call Console.WriteLine("[断言失败] 聚类结果为空")
            ok = False
        End If

        For Each c As Cluster In result.clusters
            If c.members Is Nothing OrElse c.members.Count = 0 Then
                Call Console.WriteLine("[断言失败] 存在空簇")
                ok = False
                Continue For
            End If

            If Not c.members.Contains(c.representative) Then
                Call Console.WriteLine("[断言失败] 代表序列不在成员列表中")
                ok = False
            End If

            Dim maxLen = c.members.Max(Function(id) seqs(id).SequenceData.Length)
            Dim reprLen = seqs(c.representative).SequenceData.Length

            If reprLen < maxLen Then
                Call Console.WriteLine($"[断言失败] 代表 {seqs(c.representative).Title} (len={reprLen}) 不是簇中最长(最长={maxLen})")
                ok = False
            End If
        Next

        Call Console.WriteLine()
        If ok Then
            Call Console.WriteLine("[验证通过] 代表序列均为各簇最长成员,簇划分有效。")
        Else
            Call Console.WriteLine("[验证未通过] 请检查算法参数或实现。")
        End If
    End Sub
End Module
