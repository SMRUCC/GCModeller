' Linclust 算法演示
'
' 在内存中构造若干人工蛋白序列(2 个同源家族 + 随机序列),
' 运行 Linclust 五阶段聚类流程,并打印关键结果,验证:
'   1. 同源序列被聚到同一个簇;
'   2. 每簇的代表序列(representative)是该簇中最长的成员;
'   3. 输出 k 值、簇数、每簇成员。
'
' 运行方式:作为独立入口,在 Program.Main 中调用 LinclustDemo.Run()。

Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.SequenceAlignment.BestLocalAlignment
Imports SMRUCC.genomics.Analysis.SequenceAlignment.DIAMOND
Imports SMRUCC.genomics.Model.MotifGraph.ProteinStructure
Imports SMRUCC.genomics.Model.MotifGraph.ProteinStructure.Linclust
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Module LinclustDemo

    ''' <summary>
    ''' 测试入口。可通过命令行第一个参数指定参与聚类的序列条数,
    ''' 便于在内存监控下从小规模逐步放大验证(默认 50 条,保守起步)。
    ''' </summary>
    ''' <example>
    ''' test.exe        ' 默认 50 条
    ''' test.exe 100    ' 100 条
    ''' test.exe 1000   ' 复现原始问题规模(请务必在内存监控下运行)
    ''' </example>
    Sub Run()
        Dim args As String() = Environment.GetCommandLineArgs
        Dim take As Integer = 0

        If args.Length > 1 AndAlso Integer.TryParse(args(1), take) Then
            ' 由命令行指定规模
        Else
            take = 5000
        End If

        Call Console.WriteLine($"=== Linclust 内存回归验证 (take={take}) ===")
        ' Call RunTest()
        Call RunDemo(take)
        Call Console.WriteLine("=== 运行结束 ===")
    End Sub

    Const demoFasta As String = "G:\cell-render\data\ec_numbers.fasta"

    ''' <summary>
    ''' 打印当前进程的内存占用,便于在外部监控脚本之外交叉验证内存是否稳定。
    ''' </summary>
    Private Sub ReportMemory(stage As String)
        Dim proc = Process.GetCurrentProcess()

        Call proc.Refresh()
        Call Console.WriteLine(
            $"[MEM] {stage,-22} WorkingSet={proc.WorkingSet64 / 1024 / 1024:F1}MB " &
            $"Private={proc.PrivateMemorySize64 / 1024 / 1024:F1}MB " &
            $"GCHeap={GC.GetTotalMemory(forceFullCollection:=False) / 1024 / 1024:F1}MB")
    End Sub

    ''' <summary>
    ''' 小批量受控验证入口:默认只取 50 条序列,用于在内存监控下验证
    ''' <see cref="PairAlign.AlignBestHSP"/> 的内存占用是否已经稳定。
    ''' </summary>
    ''' <param name="take">参与聚类的序列条数。</param>
    ''' <remarks>
    ''' Linclust 阶段四是 O(n^2) 的两两比对,序列条数对耗时与峰值内存都非常敏感。
    ''' 请从小规模(50/100)开始,确认内存曲线平稳后再逐步放大,
    ''' 避免一次性跑满数据集导致系统内存耗尽。
    ''' </remarks>
    Public Sub RunDemoSmall(Optional take As Integer = 50)
        Call RunDemo(take)
    End Sub

    Sub RunDemo(Optional take As Integer = 1000)
        If Not System.IO.File.Exists(demoFasta) Then
            Call Console.WriteLine($"[RunDemo] 跳过: 外部文件 {demoFasta} 不存在。")
            Return
        End If

        ' ---------- 1. 读取输入 ----------
        Dim opts As New LinclustOptions With {
            .m = 3,
            .seqidThreshold = 0.2,
            .coverage = 0.1,
            .evalue = 1
        }

        Call ReportMemory("before-read")

        ' 注意:必须使用 StreamIterator 流式读取,不能用 FastaFile.Read(...).Take(n)。
        ' FastaFile.Read 内部走的是 ReadAllLines,会把**整个** FASTA 文件读进内存
        ' (604MB 的文件在 .NET 中以 UTF-16 字符串形式驻留会膨胀到 1.6GB 以上),
        ' 之后的 .Take(n) 只是丢弃多余对象,内存已经吃掉了。
        ' 流式读取则只保留真正参与聚类的那 n 条序列。
        Dim seqs As FastaSeq()

        Using reader As New StreamIterator(demoFasta, tqdm_wrap:=False)
            seqs = reader.ReadStream.Take(take).ToArray
        End Using

        Call GC.Collect()
        Call GC.WaitForPendingFinalizers()
        Call GC.Collect()

        Call Console.WriteLine($"[RunDemo] 参与聚类的序列条数 = {seqs.Length}")
        Call Console.WriteLine($"[RunDemo] 序列长度: min={seqs.Min(Function(s) s.SequenceData.Length)}, " &
                               $"max={seqs.Max(Function(s) s.SequenceData.Length)}, " &
                               $"avg={seqs.Average(Function(s) CDbl(s.SequenceData.Length)):F0}")
        Call ReportMemory("after-read")

        ' ---------- 2. 运行聚类 ----------
        Dim timer = Stopwatch.StartNew()
        Dim result = Linclust.Cluster(seqs, opts)

        Call timer.Stop()
        Call ReportMemory("after-cluster")
        Call Console.WriteLine($"[RunDemo] 聚类耗时 = {timer.ElapsedMilliseconds} ms")

        ' 主动回收后再采样一次:若此时内存仍然居高不下,说明存在真实的对象滞留;
        ' 若回落,则此前的高位主要来自尚未回收的临时对象。
        Call GC.Collect()
        Call GC.WaitForPendingFinalizers()
        Call GC.Collect()
        Call ReportMemory("after-gc")

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
    End Sub

    Sub TestAlignment()
        Dim q = "MFAQLDTKTVYSFMDSLIDLNHYFERAKQFGYHTIGIMDKDNLYGAYHFIKGCQKNGLQPVLGLEIEILYQERQVLLNLIAQNTQGYHQLLKISTAKMSGKLHMDYFCQHLEGIAVIIPSKGWSDTLVVPFDYYIGVDQYTDLSHMDSKRQLIPLRTVRYFAQDDMETLHMLHAIRDNLSLAETPVVESDQELADCQQLTAFYQTHCPQALQNLEDLVSGIYYDFDTNLKLPHFNRDKSAKQELQDLTEAGLKEKGLWKEPYQSRLLHELVIISDMGFDDYFLIVWDLLRFGRSKGYYMGMGRGSAAGSLVAYALNITGIDPVQHDLLFERFLNKERYSMPDIDIDLPDIYRSEFLRYVRNRYGSDHSAQIVTFSTFGPKQAIRDVFKRFGVPEYELTNLTKKIGFKDSLATVYEKSISFRQVINSRTEFQKAFAIAKRIEGNPRQTSIHAAGIVMSDDALTNHIPLKSGDDMMITQYDAHAVEANGLLKMDFLGLRNLTFVQKMQEKVAKDYGCQIDITAIDLEDPQTLALFAKGDTKGIFQFEQNGAINLLKRIKPQRFEEIVATTSLNRPGASDYTTNFIKRREGQEKIDLIDPVIAPILEPTYGIMLYQEQVMQIAQVYAGFTLGKADLLRRAMSKKNLQEMQKMEEDFIASAKHLGRAEETARGLFKRMEKFAGYGFNRSHAFAYSALAFQLAYFKAHYPAVFYDIMMNYSSSDYITDALESDFQVAQVTINSIPYTDKIEASKIYMGLKNIKGLPRDFAYWIIEQRPFNSVEDFLTRTPEKYQKKVFLEPLIKIGLFDCFEPNRKKILDNLDGLLVFVNELGSLFSDSSFSWVDTKDYSVTEKYSLEQEIVGVGMSKHPLIDIAEKSTQTFTPISQLVKESEAVVLIQIDSIRIIRTKTSGQQMAFLSVNDTKKKLDVTLFPQEYAIYKDQLKEGEFYYLKGRIKERDHRLQMVCQQVQMAISQKYWLLVENHQFDSQISEILGAFPGTTPVVIHYQKNKETIALTKIQVHVTENLKEKLRPFVLKTVFR"
        Dim s = "MNSQANTPLPNSSDVEAVVTKAIHRAEQWLEIEETSASTKQLADMVHDPDGVEFTFAFVDRVARPEDNQVSAKEFAKIANPFKRTEPVPGFMSLVDSILVTAGSIAAPLLPNIVMPIARSYLRATVGHLVLDAESKALDRMLDDYRDKGFQLNLNLLGEAVLGEAEAQRRLDNTLDLLKNPRVDYVSVKASSVVSQLNHWDFEGSIERLKDRLRPLYRQAMKRDPHPFINLDMEEYKDLHLTIKLFTELLDEEEFQNLEAGIVLQAYLPDTFDALQELAEFAAQRRAKGGAKIKVRLVKGANLSMERVDSEVHDWPQAPYLTKAEVDANYIRLLDWVLQPEHADNLRIGVASHNLYHLALAHELSVVRNVEHQLDVEMLQGMSPAQSEAVRDVAGNMILYTPVVKKEDFDVAISYLVRRLEENGAKQNFLYALFTPEDDETDVTGMTPMQGQELRFRNSVRDRWETFAGSRRTQNRLKEEAEKKGCQSDGLPGNFVNEPDTDPTLPANREWALKIVDPSSDPGPAQTPEVTDPAVIDAAVARCREASAQWSQKTGAERAELLDQAAHALANNRSKLISAAVFEAGKTVAETDPEVSEAIDFARYYAESARQLDHVRGSVFTPYKSVVVTPPWNFPIAIPLGGVFAALAAGSCVIIKPAPQVLRIAEVFMEILREAGISEDLVQLVNADEAEAGKRLVSHPDVESVILTGASETAKLFRGWKPRMVINAETSGKNAIIVTPAADPDLAVADIFKSAYGHAGQKCSAASLIITVGSIGKSKRFINQLVDAVRSMKVGPGSDISTFMNGVIEEPGDKLLRGLTELDKGEKWLVKPRKLNDEGTLWSPGLRDNVKPGSWFHTHECFGPVAGIMHAETLDEAIEWQNSTGFGLTGGIHTIDVDETAYWRERVEVGNAYVERGITGAIVQRQSFGGWKNSALGSGAKAGGPNYVAQQGVWTEGDLSELAAGTLPTHITQLLREIRGLGSPALSKDDHVWLRRAAESDAYAMSTEFGVEHDKTALVVESNVFRYKPLLEPLRVRVSEGANPRDLLRLKLASAATGTELDISANPEVAREWGELGEQMRTSSDRDFAEEIAIAQSVRVRSLGKAPDEFYEAAAKSGSVILDQDVLPDGRRELLPLLLEQAISTTEHRFGYIHGLTP"
        Dim aligner As New PairAlign
        ' 用 FastaSeq 包装后调用 AlignDetailed(FastaSeq 重载,返回 Output)。
        ' 注意:AlignBestHSP 内部取 .Best 会触发底层 SimpleChaining.ChainingImpl 的"最佳链化",
        ' 其 dims = size*(size-1)\2 以 Integer 计算,正分 HSP 数量过大时(长随机序列)会触发
        ' 算术溢出(OverflowException)。AlignDetailed 仅收集全部正分 HSP 到 Output.HSP,不调用
        ' 链化,随后在此自行取得分最高的单条 HSP,绕开该溢出路径。
        Dim qSeq As New FastaSeq(q, "query")
        Dim sSeq As New FastaSeq(s, "subject")
        Dim out As Output = aligner.AlignDetailed(qSeq, sSeq)

        If out Is Nothing OrElse out.HSP Is Nothing OrElse out.HSP.Length = 0 Then
            Console.WriteLine("No significant local alignment found (no positive-scoring HSP).")
            Return
        End If

        Dim best As HSP = out.HSP(0)
        For Each h As HSP In out.HSP
            If h.score > best.score Then
                best = h
            End If
        Next

        ' 在原始比对片段上计算 identity(无 gap)
        Dim n = Math.Min(best.Query.Length, best.Subject.Length)
        Dim matches = 0
        For i = 0 To n - 1
            If best.Query(i) = best.Subject(i) Then
                matches += 1
            End If
        Next
        Dim identity = If(n > 0, CDbl(matches) / n, 0.0)

        Console.WriteLine("Best pairwise local alignment (PairAlign):")
        Console.WriteLine($"  score       = {best.score}")
        Console.WriteLine($"  LengthQuery = {best.LengthQuery}")
        Console.WriteLine($"  LengthHit   = {best.LengthHit}")
        Console.WriteLine($"  identity    ~ {identity:F3}")
        Console.WriteLine($"  query   = {Left(best.Query, 60)}...")
        Console.WriteLine($"  subject = {Left(best.Subject, 60)}...")
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
