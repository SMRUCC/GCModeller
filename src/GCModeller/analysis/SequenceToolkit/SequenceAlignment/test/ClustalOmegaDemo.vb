Imports System.Diagnostics
Imports System.IO
Imports Microsoft.VisualBasic.ApplicationServices.Debugging
Imports SMRUCC.genomics.Analysis.SequenceAlignment.Clustal
Imports SMRUCC.genomics.Analysis.SequenceAlignment.MSA.Tabular
Imports SMRUCC.genomics.SequenceModel.FASTA

''' <summary>
''' Clustal Omega 流水线的控制台验证程序。
''' </summary>
''' <remarks>
''' 所有断言都通过 <see cref="DebugAudit"/> 记录，运行结束之后会在输出目录下生成
''' <c>clustal-omega.audit.json</c> 的测试报告。断言失败不会中断程序，便于一次跑完全部用例。
''' </remarks>
Module ClustalOmegaDemo

    Private ReadOnly output As String = "./clustal-omega-test/"

    Sub Main()
        Call Directory.CreateDirectory(output)

        DebugAudit.Reset()
        DebugAudit.ThrowOnFailure = False
        DebugAudit.MaxEvents = 1024

        Call VBDebugger.EchoLine("=== Clustal Omega pipeline verification ===")

        Call CaseNucleotideSample()
        Call CaseProteinSample()
        Call CaseFormatRoundTrip()
        Call CaseIterations()
        Call CaseClusteredRun()
        Call CaseDegenerateInputs()

        Call VBDebugger.EchoLine($"audit failures: {DebugAudit.FailureCount}")
        Call VBDebugger.EchoLine(DebugAudit.Dump(includeRecords:=False))

        Call DebugAudit.SaveReport(output & "clustal-omega.audit.json")

        Call Console.WriteLine()
        Call Console.WriteLine($"report -> {Path.GetFullPath(output & "clustal-omega.audit.json")}")
        Call Console.WriteLine($"failures -> {DebugAudit.FailureCount}")
    End Sub

    ''' <summary>
    ''' 核酸小样例：与 MSA/README.md 中的示例一致
    ''' </summary>
    Private Sub CaseNucleotideSample()
        Dim seqs As String() = {
            "ATGATCCGAATTCAT",
            "ATGAATTCGGATCAT",
            "ATGAATTCGGATCAT",
            "ATGATCCGAATTCAT",
            "ATGAGCAGGATTCAT",
            "ATGAATCCTGCTCAT"
        }
        Dim names As String() = {"XAC0334", "XAC0333", "XCC0315", "XCC0316", "Smlt2583", "Smlt2584"}

        Using scope As DebugAuditScope = DebugAudit.Watch("nucleotide-sample")
            Dim result As ClustalOmegaResult = ClustalOmega.Align(seqs, names)

            Call CheckAlignment("nucleotide", result, seqs)

            DebugAudit.AreEqual(SequenceType.DNA, result.SequenceTypeUsed, "序列类型应该判定为核酸")
            DebugAudit.AreEqual(6, result.GuideTree.LeafCount, "引导树叶子数")
            DebugAudit.Check(result.Statistics.ColumnCount >= 15, "比对之后的列数不应该少于最长序列")
            DebugAudit.Check(result.Statistics.SpScore > 0, "SP 得分应该为正(核酸矩阵的匹配打分为正)")

            Call DebugAudit.Snapshot(
                DebugAudit.V(result.cost),
                DebugAudit.V(result.Statistics.ColumnCount),
                DebugAudit.V(result.Statistics.Identity))

            Call Console.WriteLine(result.ToString())

            ' 两种解码方式都应该得到合法的比对结果
            Dim viterbi As ClustalOmegaResult = ClustalOmega.Align(seqs, names,
                New ClustalOmegaOptions With {.Decode = DecodeMode.Viterbi})

            Call CheckAlignment("nucleotide-viterbi", viterbi, seqs)

            DebugAudit.Check(result.Statistics.ColumnCount = viterbi.Statistics.ColumnCount,
                             $"MAC 与 Viterbi 的列数应该一致: {result.Statistics.ColumnCount} vs {viterbi.Statistics.ColumnCount}")
        End Using
    End Sub

    ''' <summary>
    ''' 蛋白样例：验证 Gonnet 矩阵分支与类型判定
    ''' </summary>
    Private Sub CaseProteinSample()
        Dim seqs As String() = {
            "MKTAYIAKQRQISFVKSHFSRQLEERLGLIEVQAPILSRVGDGTQDNLSGAEKAVQVKVKALPDAQFEVVHSLAKWKR",
            "MKTAYIAKQRQISFVKSHFSRQLEERLGLIEVQAPILSRVGDGTQDNLSGAEKAVQVKVKALPDAQFEVVHSLAKWKR",
            "MKTAYIAKQRQISFVKSHFSRQLEERLGLIEVQAPILSRVGDGTQDNLSGAEKAVQVKVKALPDAQFEVVHSLAKWKC",
            "MSTAYIAKQRQISFVKSHFSRQLEERLGLIEVQAPILSRVGDGTQDNLSGAEKAVQVKVKALPDAQFEVVHSLAKWKR"
        }

        Using scope As DebugAuditScope = DebugAudit.Watch("protein-sample")
            Dim result As ClustalOmegaResult = ClustalOmega.Align(seqs, {"prot_a", "prot_b", "prot_c", "prot_d"})

            Call CheckAlignment("protein", result, seqs)

            DebugAudit.AreEqual(SequenceType.Protein, result.SequenceTypeUsed, "序列类型应该判定为蛋白")

            ' 蛋白默认 k-mer 长度为 2
            DebugAudit.AreEqual(2, result.KmerSizeUsed, "蛋白的默认 k-mer 长度")

            Call DebugAudit.Snapshot(
                DebugAudit.V(result.Statistics.SpScore),
                DebugAudit.V(result.Statistics.Identity))

            Call Console.WriteLine(result.ToString())
        End Using
    End Sub

    ''' <summary>
    ''' 四种输出格式的写出与回读
    ''' </summary>
    Private Sub CaseFormatRoundTrip()
        Dim seqs As String() = {
            "ATGGCAATTCCGGTT",
            "ATGGCAATTCCAGTT",
            "ATGGCCATTCCGGTT",
            "ATGGCAATTCCTGTT"
        }
        Dim names As String() = {"s1", "s2", "s3", "s4"}

        Dim result As ClustalOmegaResult = ClustalOmega.Align(seqs, names)

        Using scope As DebugAuditScope = DebugAudit.Watch("format-roundtrip")
            ' FASTA
            Dim fastaPath As String = output & "roundtrip.fasta"
            Call result.SaveAs(MSAFormat.Fasta, fastaPath)

            Dim reloaded As FastaFile = FastaFile.Read(fastaPath)
            DebugAudit.AreEqual(result.MSA.Length, reloaded.Count, "FASTA 回读的序列条数")

            For i As Integer = 0 To result.MSA.Length - 1
                DebugAudit.AreEqual(result.MSA(i), reloaded(i).SequenceData, $"FASTA 回读第 {i} 行")
            Next

            ' Clustal
            Dim clustalPath As String = output & "roundtrip.aln"
            Call result.SaveAs(MSAFormat.Clustal, clustalPath)

            Dim clustalText As String = File.ReadAllText(clustalPath)
            DebugAudit.Check(clustalText.StartsWith("CLUSTAL"), "Clustal 输出的首行标记")
            DebugAudit.Check(clustalText.Contains("*"), "Clustal 输出应该包含保守列标记")

            ' Stockholm 与既有 Reader 的往返
            Dim stockholmPath As String = output & "roundtrip.sto"
            Call result.SaveAs(MSAFormat.Stockholm, stockholmPath)

            Dim blocks As Stockholm() = Reader.Read(stockholmPath).ToArray()
            DebugAudit.AreEqual(1, blocks.Length, "Stockholm 应该解析出一个比对块")

            If blocks.Length = 1 AndAlso blocks(0).msa IsNot Nothing Then
                DebugAudit.AreEqual(result.MSA.Length, blocks(0).msa.MSA.Length, "Stockholm 回读的行数")

                For i As Integer = 0 To result.MSA.Length - 1
                    DebugAudit.AreEqual(result.MSA(i), blocks(0).msa.MSA(i), $"Stockholm 回读第 {i} 行")
                Next
            End If

            ' PHYLIP
            Dim phylipPath As String = output & "roundtrip.phy"
            Call result.SaveAs(MSAFormat.Phylip, phylipPath)

            Dim phylipLines As String() = File.ReadAllLines(phylipPath)
            DebugAudit.Check(phylipLines(0).Trim = $"{result.MSA.Length} {result.MSA(0).Length}", "PHYLIP 头部")

            ' Newick
            Dim newick As String = result.TreeNewick()
            Call File.WriteAllText(output & "roundtrip.nwk", newick)

            DebugAudit.Check(newick.EndsWith(";"), "Newick 应该以分号结尾")
            DebugAudit.Check(newick.Contains("s1"), "Newick 应该包含叶子名称")
        End Using
    End Sub

    ''' <summary>
    ''' 迭代精炼不应该让 SP 得分变差
    ''' </summary>
    Private Sub CaseIterations()
        Dim seqs As String() = BuildMutatedSequences(template:="ATGGCAATTCCGGTTACGTACGTAGCTAGCTAGCATCGATCGATCGTAGCTAGCTA", count:=24, rate:=0.12, seed:=7)

        Dim plain As ClustalOmegaResult = ClustalOmega.Align(seqs, Nothing, New ClustalOmegaOptions With {.Iterations = 0})
        Dim refined As ClustalOmegaResult = ClustalOmega.Align(seqs, Nothing, New ClustalOmegaOptions With {.Iterations = 2})

        Using scope As DebugAuditScope = DebugAudit.Watch("iterations")
            Call CheckAlignment("iterations-plain", plain, seqs)
            Call CheckAlignment("iterations-refined", refined, seqs)

            DebugAudit.Check(refined.Statistics.SpScore >= plain.Statistics.SpScore - 0.0001,
                             $"精炼之后的 SP 得分不应该下降: {plain.Statistics.SpScore} -> {refined.Statistics.SpScore}")

            Call DebugAudit.Snapshot(
                DebugAudit.V(plain.Statistics.SpScore),
                DebugAudit.V(refined.Statistics.SpScore))
        End Using
    End Sub

    ''' <summary>
    ''' 强制走 k-means 分簇分支，并做一次规模冒烟测试
    ''' </summary>
    Private Sub CaseClusteredRun()
        Dim seqs As String() = BuildMutatedSequences(template:="ATGAGCGATCCGTTAGCATTAGCGATCGATCGATCGGCTAGCTAGCTAGCATCGTAGCTAGCTAAGCTAGCTAGCTAGCTA",
                                                     count:=120, rate:=0.1, seed:=11)

        Using scope As DebugAuditScope = DebugAudit.Watch("clustered")
            Dim options As New ClustalOmegaOptions With {
                .ClusterSize = 32,
                .Iterations = 0,
                .MacRamMb = 256
            }

            Dim watch As Stopwatch = Stopwatch.StartNew()
            Dim result As ClustalOmegaResult = ClustalOmega.Align(seqs, Nothing, options)
            watch.Stop()

            Call CheckAlignment("clustered", result, seqs)

            DebugAudit.Check(result.ClusterSizes IsNot Nothing AndAlso result.ClusterSizes.Length >= 3,
                             $"应该被切分为至少 3 个簇，实际 {If(result.ClusterSizes Is Nothing, 0, result.ClusterSizes.Length)}")
            DebugAudit.AreEqual(120, result.GuideTree.LeafCount, "引导树叶子数")
            DebugAudit.Check(result.MergeCount = 119, $"渐进合并次数应该等于 N-1，实际 {result.MergeCount}")
            DebugAudit.Check(watch.ElapsedMilliseconds < 120000, $"120 条序列的比对应该在两分钟内完成，实际 {watch.ElapsedMilliseconds} ms")

            Call DebugAudit.Mark("clustered-elapsed", watch.ElapsedMilliseconds)
            Call DebugAudit.Snapshot(
                DebugAudit.V(result.Statistics.ColumnCount),
                DebugAudit.V(result.ProfileCells),
                DebugAudit.V(result.MacFallbacks),
                DebugAudit.V(watch.ElapsedMilliseconds))

            Call Console.WriteLine(result.Statistics.ToString())
        End Using
    End Sub

    ''' <summary>
    ''' 退化输入：空集合、单条序列、全部相同
    ''' </summary>
    Private Sub CaseDegenerateInputs()
        Using scope As DebugAuditScope = DebugAudit.Watch("degenerate")
            Dim empty As ClustalOmegaResult = ClustalOmega.Align(New String() {})
            DebugAudit.AreEqual(0, empty.MSA.Length, "空输入的结果行数")
            DebugAudit.AreEqual(0.0R, empty.cost, "空输入的 SP 得分")

            Dim oneSeq As ClustalOmegaResult = ClustalOmega.Align(New String() {"ACGTACGT"})
            DebugAudit.AreEqual(1, oneSeq.MSA.Length, "单序列输入的结果行数")
            DebugAudit.AreEqual("ACGTACGT", oneSeq.MSA(0), "单序列输入的内容")

            Dim same As ClustalOmegaResult = ClustalOmega.Align(New String() {"ACGT", "ACGT", "ACGT"})
            DebugAudit.AreEqual(3, same.MSA.Length, "全同序列的结果行数")
            DebugAudit.AreEqual(3, same.GuideTree.LeafCount, "全同序列的树叶子数")

            For i As Integer = 0 To same.MSA.Length - 1
                DebugAudit.AreEqual("ACGT", same.MSA(i), $"全同序列的第 {i} 行")
            Next
        End Using
    End Sub

    ' ------------------------------------------------------------------
    ' 断言辅助
    ' ------------------------------------------------------------------

    Private Sub CheckAlignment(name As String, result As ClustalOmegaResult, input As String())
        DebugAudit.AreEqual(input.Length, result.MSA.Length, $"{name}: 结果行数")

        If result.MSA.Length = 0 Then
            Return
        End If

        Dim columns As Integer = result.MSA(0).Length

        For i As Integer = 0 To result.MSA.Length - 1
            DebugAudit.AreEqual(columns, result.MSA(i).Length, $"{name}: 第 {i} 行的长度")

            Dim restored As String = New String(result.MSA(i).Where(Function(c) c <> "-"c).ToArray())

            DebugAudit.AreEqual(input(i), restored, $"{name}: 第 {i} 行去掉空位之后应该还原输入")
        Next
    End Sub

    ''' <summary>
    ''' 生成一组以 <paramref name="template"/> 为祖先的确定性突变序列
    ''' </summary>
    Private Function BuildMutatedSequences(template As String, count As Integer,
                                           rate As Double, seed As Integer) As String()
        Dim rnd As New Random(seed)
        Dim letters As Char() = {"A"c, "C"c, "G"c, "T"c}
        Dim seqs As String() = New String(count - 1) {}

        For i As Integer = 0 To count - 1
            Dim chars As Char() = template.ToCharArray()

            For p As Integer = 0 To chars.Length - 1
                If rnd.NextDouble() < rate Then
                    chars(p) = letters(rnd.Next(letters.Length))
                End If
            Next

            seqs(i) = New String(chars)
        Next

        Return seqs
    End Function
End Module
