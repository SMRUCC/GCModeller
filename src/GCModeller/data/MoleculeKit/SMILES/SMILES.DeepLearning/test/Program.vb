Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports Microsoft.VisualBasic.ApplicationServices.Debugging
Imports Microsoft.VisualBasic.MachineLearning.DataStorage.PT
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports Microsoft.VisualBasic.MachineLearning.Transformer
Imports SMRUCC.genomics.Molecule.SMILES.DeepLearning.Corpus
Imports SMRUCC.genomics.Molecule.SMILES.DeepLearning.Features
Imports SMRUCC.genomics.Molecule.SMILES.DeepLearning.Trainer
Imports SMRUCC.genomics.Molecule.SMILES.SmilesSyntax
Imports std = System.Math

''' <summary>
''' P0 组件控制台测试：分词器 / 词表 / 写出器 / 算子 / 模型 / 权重映射 / 数据管道。
''' </summary>
''' <remarks>
''' 结果通过 <see cref="DebugAudit"/> 记录并落盘为 JSON 报告。
''' 注意 <c>Check</c>/<c>AreEqual</c> 带 <c>Conditional("DEBUG")</c>，因此必须以 Debug 配置运行。
''' </remarks>
Module Program

    ''' <summary>测试语料（真实 SMILES，覆盖芳香、电荷、卤素、环）。</summary>
    Private ReadOnly corpus As String() = {
        "CCO",
        "c1ccccc1O",
        "CC(=O)O",
        "ClCCBr",
        "C1CCCCC1",
        "N#CC",
        "CCN(CC)CC",
        "O=C(O)CCCCC(=O)O",
        "c1ccc2[nH]ccc2c1",
        "C[N+](C)(C)C"
    }

    Private workDir As String

    ''' <summary>已执行的断言条数（用于在报告中给出"确实跑了多少条断言"）。</summary>
    Private assertions As Integer

    ''' <summary>记录一条布尔断言。</summary>
    Private Sub checkThat(condition As Boolean, message As String)
        assertions += 1
        Call DebugAudit.Check(condition, message)
    End Sub

    ''' <summary>记录一条相等断言。</summary>
    Private Sub equalTo(Of T)(expected As T, actual As T, message As String)
        assertions += 1
        Call DebugAudit.AreEqual(expected, actual, message)
    End Sub

    ''' <summary>执行一个测试分组，并记录开始/结束事件（失败不会中断后续分组）。</summary>
    Private Sub run(name As String, test As Action)
        Dim before As Integer = DebugAudit.FailureCount

        Call DebugAudit.Mark($"{name}.start")

        Try
            Call test()
        Catch ex As Exception
            Call checkThat(False, $"{name}: 未处理异常 {ex.GetType().Name}: {ex.Message}{ControlChars.NewLine}{ex.StackTrace}")
        End Try

        Call DebugAudit.Mark($"{name}.done", $"failures={DebugAudit.FailureCount - before}")
    End Sub

    Sub Main(args As String())
        workDir = Path.Combine(Environment.CurrentDirectory, ".test")
        Call Directory.CreateDirectory(workDir)

        Dim modelPath As String = If(args.Length > 0,
                                     args(0),
                                     "C:\Users\Administrator\Downloads\trfm_12_23000.pkl")

        Call DebugAudit.Reset()
        Call run("tokenizer", AddressOf testTokenizer)
        Call run("vocab", AddressOf testVocab)
        Call run("writer", AddressOf testSmilesWriter)
        Call run("ops", AddressOf testTensorOps)
        Call run("model", AddressOf testModel)
        Call run("state_dict", AddressOf testStateDict)
        Call run("pipeline", AddressOf testPipeline)
        Call run("official", Sub() testOfficialWeights(modelPath))
        Call DebugAudit.Mark("assertions.total", assertions)

        Dim report As String = Path.Combine(workDir, "smiles-transformer-p0-report.json")

        Call DebugAudit.SaveReport(report)
        Console.WriteLine()
        Console.WriteLine($"audit failures: {DebugAudit.FailureCount}")
        Console.WriteLine($"report: {report}")
    End Sub

#Region "分词器"

    Private Sub testTokenizer()
        Call equalTo("C ( = O ) C C ( Br ) C [ N + ] C N",
                       SmilesTokenizer.SplitJoined("C(=O)CC(Br)C[N+]CN"),
                       "tokenizer: 与 Python test_randomizer 一致")

        Call equalTo("%10 C C", SmilesTokenizer.SplitJoined("%10CC"), "tokenizer: %nn 环闭合号")
        Call equalTo("Si si Se se", SmilesTokenizer.SplitJoined("SisiSese"), "tokenizer: 大小写敏感的二字母元素")
        Call equalTo("C", SmilesTokenizer.SplitJoined("C"), "tokenizer: 单字符")
        Call equalTo(0, SmilesTokenizer.Split("").Length, "tokenizer: 空串返回空数组")
        Call equalTo("C @ @ H", SmilesTokenizer.SplitJoined("C@@H"), "tokenizer: 方括号内部逐字符拆散")
    End Sub

#End Region

#Region "词表"

    Private Sub testVocab()
        Dim lines As String() = {"C C O", "C N", "C C O"}
        Dim vocab As WordVocab = WordVocab.FromCorpus(lines)

        Call equalTo(8, vocab.Count, "vocab: 5 个特殊 token + 3 个语料 token")
        Call equalTo(WordVocab.PadIndex, 0, "vocab: pad=0")
        Call equalTo(WordVocab.MaskIndex, 4, "vocab: mask=4")
        Call equalTo(5, vocab.SToI("C"), "vocab: 先字典序再频次降序 → C 在 5")
        Call equalTo(6, vocab.SToI("O"), "vocab: O 频次高，排在 N 之前")
        Call equalTo(7, vocab.SToI("N"), "vocab: N 在 7")

        Dim small As WordVocab = WordVocab.FromCorpus(lines, maxSize:=1)

        Call equalTo(6, small.Count, "vocab: maxSize 不含特殊 token")

        Dim filtered As WordVocab = WordVocab.FromCorpus(lines, minFreq:=2)

        Call equalTo(7, filtered.Count, "vocab: minFreq 过滤只出现一次的 token")

        Dim seq As Integer() = vocab.ToSeq({"C", "X"}, seqLen:=6, withEos:=True, withSos:=True)

        Call equalTo(6, seq.Length, "vocab: 填充到定长")
        Call equalTo(WordVocab.SosIndex, seq(0), "vocab: 首部 sos")
        Call equalTo(5, seq(1), "vocab: 首 token 的 id")
        Call equalTo(WordVocab.UnkIndex, seq(2), "vocab: 未登录 token → unk")
        Call equalTo(WordVocab.EosIndex, seq(3), "vocab: 尾部 eos")
        Call equalTo(WordVocab.PadIndex, seq(5), "vocab: 其余位置 pad")

        Dim json As String = Path.Combine(workDir, "vocab.json")

        Call vocab.SaveJson(json)

        Dim restored As WordVocab = WordVocab.LoadJson(json)

        Call equalTo(vocab.Count, restored.Count, "vocab: JSON 往返长度一致")

        For Each kv As KeyValuePair(Of String, Integer) In vocab.SToI
            Call equalTo(kv.Value, restored.SToI(kv.Key), $"vocab: JSON 往返 token '{kv.Key}'")
        Next

        ' 词表与分词器组合：语料行既可以是原始 SMILES，也可以是已分词的行
        AuditVocabFromRawSmiles()
    End Sub

    Private Sub AuditVocabFromRawSmiles()
        Dim vocab As WordVocab = WordVocab.FromCorpus({"CCO", "c1ccccc1"})

        Call checkThat(vocab.ContainsKey("c"), "vocab: 原始 SMILES 会被分词（芳香 c 存在）")
        Call checkThat(vocab.ContainsKey("O"), "vocab: 原始 SMILES 会被分词（O 存在）")
    End Sub

#End Region

#Region "写出器"

    Private Sub testSmilesWriter()
        Dim graph = MoleculeIO.ParseSmiles("c1ccccc1O")
        Dim first As String = MoleculeIO.WriteSmiles(graph)
        Dim second As String = MoleculeIO.WriteSmiles(graph)

        Call equalTo(first, second, "writer: 默认选项保持确定性")

        Dim reparsed = MoleculeIO.ParseSmiles(first)

        Call equalTo(graph.MolKey(), reparsed.MolKey(), "writer: 默认输出 round-trip 后同构")

        Dim aromatic As String = SmilesWriter.Write(graph, New SmilesWriteOptions With {.EmitAromatic = True})

        Call checkThat(aromatic.Contains("c"), $"writer: 芳香原子输出小写（{aromatic}）")
        Call checkThat(Not aromatic.Contains("C"), $"writer: 芳香体系不输出大写碳（{aromatic}）")

        ' 随机枚举：同一分子 100 次随机化应得到多条不同写法，且全部与原文同构
        Const aspirin As String = "CC(=O)Oc1ccccc1C(=O)O"

        ' 注意：芳香 SMILES 的 Kekulé 化结果不唯一，因此这里用"分子式 + 原子数 + 键数"
        ' 作为同构判据，而不是依赖某一次 Kekulé 分配的 MolKey。
        Dim reference = MoleculeIO.ParseSmiles(aspirin)
        Dim expectedFormula As String = reference.MolecularFormula()
        Dim expectedAtoms As Integer = reference.AtomCount
        Dim expectedBonds As Integer = reference.BondCount
        Dim distinct As New HashSet(Of String)(StringComparer.Ordinal)
        Dim enumerator As New SmilesEnumerator(seed:=2024)
        Dim mismatched As Integer = 0

        For i As Integer = 1 To 100
            Dim randomized As String = enumerator.Randomize(aspirin)

            If String.IsNullOrEmpty(randomized) Then Continue For

            distinct.Add(randomized)

            Dim enumerated = MoleculeIO.ParseSmiles(randomized)

            If enumerated.MolecularFormula() <> expectedFormula OrElse
               enumerated.AtomCount <> expectedAtoms OrElse
               enumerated.BondCount <> expectedBonds Then
                mismatched += 1
            End If
        Next

        Call equalTo(0, mismatched, "writer: 所有随机化结果与原文同构（分子式/原子数/键数）")

        Call DebugAudit.Snapshot(DebugAudit.V(distinct.Count))
        Call checkThat(distinct.Count >= 80, $"writer: 100 次随机枚举至少 80 种不同写法（实际 {distinct.Count}）")

        Call checkThat(SmilesEnumeratorIsNullForInvalid(), "writer: 非法 SMILES 的随机化返回 Nothing")
    End Sub

    Private Function SmilesEnumeratorIsNullForInvalid() As Boolean
        Dim enumerator As New SmilesEnumerator(seed:=1)

        Return enumerator.Randomize("C1CC") Is Nothing
    End Function

#End Region

#Region "算子"

    Private Sub testTensorOps()
        ' GELU：与 tanh 近似公式逐项对比
        Dim x As New Tensor(New Double() {-0.5, 0.0, 0.3, 1.2}, 4)
        Dim y As Tensor = TensorOpsEx.Gelu(x)
        Dim maxDiff As Double = 0.0

        For i As Integer = 0 To 3 Step 1
            Dim xv As Double = x.Data(i)
            Dim u As Double = TensorOpsEx.GeluC * (xv + TensorOpsEx.GeluCubic * xv * xv * xv)
            Dim expected As Double = 0.5 * xv * (1.0 + std.Tanh(u))

            maxDiff = std.Max(maxDiff, std.Abs(expected - y.Data(i)))
        Next

        Call checkThat(maxDiff < 1.0E-14, $"ops: GELU 与手算一致（maxDiff={maxDiff:E3}）")

        ' GELU 反向：有限差分校验
        Dim geluForward As Func(Of Tensor, Tensor) = Function(t) TensorOpsEx.Gelu(t)
        Dim geluBackward As Func(Of Tensor, Tensor, Tensor) = Function(t, g) TensorOpsEx.GeluBackward(g, t)
        Dim geluDiff As Double = finiteDiffCheck(geluForward, geluBackward, x)

        Call checkThat(geluDiff < 1.0E-6, $"ops: GELU 反向与有限差分一致（maxDiff={geluDiff:E3}）")

        ' LayerNorm：有偏方差 + eps 在 sqrt 内
        Dim features As Integer = 4
        Dim input As New Tensor(New Double() {1.0, 2.0, 3.0, 4.0, -1.0, 0.5, 2.5, 3.5}, 2, features)
        Dim mean As Double() = Nothing
        Dim invStd As Double() = Nothing
        Dim ln As Tensor = TensorOpsEx.LayerNormForward(input, Nothing, Nothing, mean, invStd, 1.0E-5)
        Dim manual As New List(Of Double)

        For blk As Integer = 0 To 1
            Dim offset As Integer = blk * features
            Dim mu As Double = 0.0

            For i As Integer = 0 To features - 1
                mu += input.Data(offset + i)
            Next

            mu /= features

            Dim vari As Double = 0.0

            For i As Integer = 0 To features - 1
                vari += (input.Data(offset + i) - mu) ^ 2
            Next

            vari /= features

            For i As Integer = 0 To features - 1
                manual.Add((input.Data(offset + i) - mu) / std.Sqrt(vari + 1.0E-5))
            Next
        Next

        Dim lnDiff As Double = 0.0

        For i As Integer = 0 To manual.Count - 1
            lnDiff = std.Max(lnDiff, std.Abs(manual(i) - ln.Data(i)))
        Next

        Call checkThat(lnDiff < 1.0E-12, $"ops: LayerNorm 与手算一致（maxDiff={lnDiff:E3}）")

        ' LayerNorm 反向：有限差分校验（gamma=1, beta=0）
        Dim lnForward As Func(Of Tensor, Tensor) =
            Function(t) TensorOpsEx.LayerNormForward(t, Nothing, Nothing, mean, invStd, 1.0E-5)
        Dim lnBackward As Func(Of Tensor, Tensor, Tensor) =
            Function(t, g)
                Dim mg As Double() = Nothing
                Dim isd As Double() = Nothing
                Dim dGamma As Tensor = Nothing
                Dim dBeta As Tensor = Nothing

                Call TensorOpsEx.LayerNormForward(t, Nothing, Nothing, mg, isd, 1.0E-5)

                Return TensorOpsEx.LayerNormBackward(g, t, Nothing, mg, isd, dGamma, dBeta)
            End Function
        Dim lnGradDiff As Double = finiteDiffCheck(lnForward, lnBackward, input)

        Call checkThat(lnGradDiff < 1.0E-6, $"ops: LayerNorm 反向与有限差分一致（maxDiff={lnGradDiff:E3}）")

        ' NLL：log_softmax + ignore_index
        Dim logits As New Tensor(New Double() {2.0, 0.5, -1.0, 0.1, 0.2, 0.3}, 2, 3)
        Dim logProbs As Tensor = TensorOpsEx.LogSoftmaxLastDim(logits)
        Dim dLogits As Tensor = Nothing
        ' 这里用 -1 作为 ignore 下标：两个位置（1 和 0）都要参与损失与分母
        Dim loss As Double = TensorOpsEx.NllLoss(logProbs, {1, 0}, -1, dLogits)

        ' 第一行目标为 1（flat index 1），第二行目标为 0（flat index 3），分母为 2
        Dim expectedLoss As Double = -(logProbs.Data(1) + logProbs.Data(3)) / 2.0

        Call checkThat(std.Abs(expectedLoss - loss) < 1.0E-14, $"ops: NLL 与手算一致（{loss:F6} vs {expectedLoss:F6}）")
        Call checkThat(std.Abs(dLogits.Data(0)) > 0.0, "ops: NLL 产生非零梯度")

        Dim sumExp As Double = 0.0

        For i As Integer = 0 To 2
            sumExp += std.Exp(logProbs.Data(i))
        Next

        Call checkThat(std.Abs(sumExp - 1.0) < 1.0E-12, "ops: log_softmax 归一化正确")
    End Sub

    ''' <summary>
    ''' 用中心差分校验一个算子的反向传播（只抽样前 4 个元素，控制运行时间）。
    ''' </summary>
    Private Function finiteDiffCheck(forward As Func(Of Tensor, Tensor),
                                     backward As Func(Of Tensor, Tensor, Tensor),
                                     x As Tensor) As Double
        Const eps As Double = 1.0E-6

        Dim upstream As New Tensor(x.Shape)

        For i As Integer = 0 To upstream.Length - 1
            upstream.Data(i) = 1.0
        Next

        Call upstream.MarkHostModified()

        Dim analytic As Tensor = backward(x, upstream)
        Dim maxDiff As Double = 0.0
        Dim probes As Integer = std.Min(4, x.Length)

        For i As Integer = 0 To probes - 1
            Dim original As Double = x.Data(i)

            x.Data(i) = original + eps
            Call x.MarkHostModified()

            Dim plus As Double = sum(forward(x))

            x.Data(i) = original - eps
            Call x.MarkHostModified()

            Dim minus As Double = sum(forward(x))

            x.Data(i) = original
            Call x.MarkHostModified()

            Dim numeric As Double = (plus - minus) / (2.0 * eps)

            maxDiff = std.Max(maxDiff, std.Abs(numeric - analytic.Data(i)))
        Next

        Return maxDiff
    End Function

    Private Function sum(t As Tensor) As Double
        Dim total As Double = 0.0

        For i As Integer = 0 To t.Length - 1
            total += t.Data(i)
        Next

        Return total
    End Function

#End Region

#Region "模型"

    Private ReadOnly SmallConfig As ModelConfig = newConfig()

    Private Class ModelConfig
        Public Property HiddenSize As Integer
        Public Property Layers As Integer
        Public Property Heads As Integer
        Public Property Dff As Integer
        Public Property SeqLen As Integer
    End Class

    Private Function newConfig() As ModelConfig
        Return New ModelConfig With {.HiddenSize = 16, .Layers = 1, .Heads = 2, .Dff = 16, .SeqLen = 24}
    End Function

    Private Function newModel(vocabSize As Integer, Optional seed As Integer? = 42) As SequenceTransformer
        Return New SequenceTransformer(vocabSize,
                                       SmallConfig.HiddenSize,
                                       SmallConfig.Layers,
                                       SmallConfig.Heads,
                                       SmallConfig.Dff,
                                       SmallConfig.SeqLen,
                                       0.1,
                                       False,
                                       seed)
    End Function

    Private Sub testModel()
        Dim vocab As WordVocab = WordVocab.FromCorpus(corpus)
        Dim model As SequenceTransformer = newModel(vocab.Count)

        Call checkThat(model.ParameterCount() > 0, "model: 参数数量可枚举")

        Dim ids As Integer(,) = buildIds(vocab, {corpus(0), corpus(1)}, SmallConfig.SeqLen)
        Dim logProbs As Tensor = model.Forward(ids)

        Call equalTo(2, logProbs.Shape(0), "model: 输出 batch 维")
        Call equalTo(SmallConfig.SeqLen, logProbs.Shape(1), "model: 输出 time 维")
        Call equalTo(vocab.Count, logProbs.Shape(2), "model: 输出词表维")

        Dim normalized As Boolean = True

        For b As Integer = 0 To 1
            For t As Integer = 0 To SmallConfig.SeqLen - 1
                Dim s As Double = 0.0
                Dim offset As Integer = (b * SmallConfig.SeqLen + t) * vocab.Count

                For v As Integer = 0 To vocab.Count - 1
                    s += std.Exp(logProbs.Data(offset + v))
                Next

                If std.Abs(s - 1.0) > 1.0E-9 Then normalized = False
            Next
        Next

        Call checkThat(normalized, "model: 每个位置输出是合法分布")

        ' 训练若干步，损失必须下降
        Dim batchIds As Integer(,) = buildIds(vocab, corpus, SmallConfig.SeqLen)
        Dim firstLoss As Double = model.TrainStep(batchIds, 0.01, 1)
        Dim lastLoss As Double = firstLoss

        For i As Integer = 2 To 30
            lastLoss = model.TrainStep(batchIds, 0.01, i)
        Next

        Call DebugAudit.Snapshot(DebugAudit.V(firstLoss), DebugAudit.V(lastLoss))
        Call checkThat(lastLoss < firstLoss * 0.9, $"model: 30 步后损失下降（{firstLoss:F3} → {lastLoss:F3}）")

        ' Encode：4H 维指纹
        Dim encoded As Double(,) = model.Encode(batchIds, chunkSize:=3)

        Call equalTo(corpus.Length, encoded.GetLength(0), "model: 指纹行数")
        Call equalTo(SmallConfig.HiddenSize * 4, encoded.GetLength(1), "model: 指纹列数 = 4H")

        Dim finite As Boolean = True

        For r As Integer = 0 To encoded.GetLength(0) - 1
            For c As Integer = 0 To encoded.GetLength(1) - 1
                If Double.IsNaN(encoded(r, c)) OrElse Double.IsInfinity(encoded(r, c)) Then finite = False
            Next
        Next

        Call checkThat(finite, "model: 指纹数值有限")
    End Sub

    Private Function buildIds(vocab As WordVocab, smiles As String(), seqLen As Integer) As Integer(,)
        Dim ids(smiles.Length - 1, seqLen - 1) As Integer

        For i As Integer = 0 To smiles.Length - 1
            Dim origin As Integer = 0
            Dim seq As Integer() = vocab.ToSeq(SmilesTokenizer.Split(smiles(i)), seqLen, True, True, origin)

            For t As Integer = 0 To seqLen - 1
                ids(i, t) = seq(t)
            Next
        Next

        Return ids
    End Function

#End Region

#Region "权重映射"

    Private Sub testStateDict()
        Dim vocab As WordVocab = WordVocab.FromCorpus(corpus)
        Dim model As SequenceTransformer = newModel(vocab.Count, seed:=7)
        Dim ids As Integer(,) = buildIds(vocab, corpus, SmallConfig.SeqLen)
        Dim reference As Tensor = model.Predict(ids)

        ' 1) 导出 → 新模型加载 → 前向必须逐位一致
        Dim exported As Dictionary(Of String, Tensor) = TransformerStateDict.Export(model)
        Dim fresh As SequenceTransformer = newModel(vocab.Count, seed:=99)
        Dim report As TransformerLoadReport = TransformerStateDict.Apply(fresh, exported)

        Call checkThat(report.IsPerfectMatch,
                    $"state_dict: 键集合完全匹配（{report}）")
        Call checkThat(exported.ContainsKey("embed.weight"), "state_dict: 导出包含 embed.weight")
        Call checkThat(exported.ContainsKey("trfm.encoder.layers.0.self_attn.in_proj_weight"),
                    "state_dict: 导出包含 fused QKV 权重")
        Call checkThat(exported.ContainsKey("trfm.decoder.layers.0.multihead_attn.out_proj.bias"),
                    "state_dict: 导出包含交叉注意力 bias")
        Call checkThat(exported.ContainsKey("out.weight"), "state_dict: 导出包含输出层")

        Dim reloaded As Tensor = fresh.Predict(ids)
        Dim maxDiff As Double = 0.0

        For i As Integer = 0 To reference.Length - 1
            maxDiff = std.Max(maxDiff, std.Abs(reference.Data(i) - reloaded.Data(i)))
        Next

        Call checkThat(maxDiff < 1.0E-12, $"state_dict: 重新加载后前向逐位一致（maxDiff={maxDiff:E3}）")

        ' 2) 落盘为 .pt 再经 PtLegacyFile 自动识别读回
        Dim ptFile As String = Path.Combine(workDir, "smiles-transformer.pt")

        Call PtStateDict.FromTensors(exported, PtElementType.Float32).Save(ptFile)

        Call equalTo(PtContainerFormat.Zip, PtLegacyFile.DetectFormat(ptFile), "state_dict: 写出的是 zip 归档")

        Dim loaded As PtStateDict = PtLegacyFile.LoadStateDict(ptFile)
        Dim fresh2 As SequenceTransformer = newModel(vocab.Count, seed:=123)
        Dim report2 As TransformerLoadReport = TransformerStateDict.Apply(fresh2, loaded.AsTensors())

        Call checkThat(report2.IsPerfectMatch, $"state_dict: 从 .pt 读回的键集合完全匹配（{report2}）")

        Dim reloaded2 As Tensor = fresh2.Predict(ids)
        Dim maxDiff2 As Double = 0.0

        For i As Integer = 0 To reference.Length - 1
            maxDiff2 = std.Max(maxDiff2, std.Abs(reference.Data(i) - reloaded2.Data(i)))
        Next

        ' fp32 落盘会带来量化误差，阈值取 1e-5
        Call checkThat(maxDiff2 < 1.0E-5, $"state_dict: .pt 往返后前向一致（maxDiff={maxDiff2:E3}）")

        ' 3) 形状不匹配必须被拒绝
        Dim broken As New Dictionary(Of String, Tensor)(exported, StringComparer.Ordinal)
        Dim keys As List(Of String) = broken.Keys.ToList()
        Dim target As String = keys.First(Function(k) k.EndsWith("embed.weight", StringComparison.Ordinal))

        broken(target) = New Tensor(New Double() {0.0, 0.0}, 2)

        Call checkThat(throws(Of InvalidDataException)(Sub() TransformerStateDict.Apply(newModel(vocab.Count), broken)),
                    "state_dict: 形状不匹配时抛出 InvalidDataException")
    End Sub

    Private Function throws(Of TException As Exception)(code As Action) As Boolean
        Try
            Call code()
        Catch ex As TException
            Return True
        Catch ex As Exception
            Return False
        End Try

        Return False
    End Function

#End Region

#Region "数据管道"

    Private Sub testPipeline()
        Dim vocab As WordVocab = WordVocab.FromCorpus(corpus)
        Dim loader As New SequenceDataLoader(corpus, vocab, SmallConfig.SeqLen, enumerate:=True, seed:=5)

        Call equalTo(corpus.Length, loader.Count, "loader: 语料条数")

        Dim batches As New List(Of SmilesBatch)

        For Each batch As SmilesBatch In loader.Batches(4, shuffle:=True)
            batches.Add(batch)
        Next

        Call equalTo(3, batches.Count, "loader: 10 条样本按 4 分批得到 3 批")
        Call equalTo(SmallConfig.SeqLen, batches(0).Ids.GetLength(1), "loader: 批次宽度等于定长")

        Dim sosOk As Boolean = True

        For b As Integer = 0 To batches.Count - 1
            For r As Integer = 0 To batches(b).Size - 1
                If batches(b).Ids(r, 0) <> WordVocab.SosIndex Then sosOk = False
            Next
        Next

        Call checkThat(sosOk, "loader: 每条样本以 sos 开头")

        ' 指纹批量抽取
        Dim model As SequenceTransformer = newModel(vocab.Count)
        Dim extractor As New SmilesTransformerFingerprint(model, vocab, SmallConfig.SeqLen)
        Dim fingerprint As SmilesFingerprint = extractor.Encode(corpus.Take(3), batchSize:=2)

        Call equalTo(3, fingerprint.Count, "fingerprint: 行数")
        Call equalTo(SmallConfig.HiddenSize * 4, fingerprint.Width, "fingerprint: 列数")

        Dim csv As String = Path.Combine(workDir, "smiles-fingerprint.csv")

        Call fingerprint.SaveCsv(csv)

        Call checkThat(File.Exists(csv), "fingerprint: CSV 已写出")
        Call checkThat(File.ReadAllLines(csv).Length = 4, "fingerprint: CSV 含表头 + 3 行")

        ' 端到端：2 个 epoch 的小样本预训练
        Dim model2 As SequenceTransformer = newModel(vocab.Count, seed:=11)
        Dim options As New SmilesPretrainOptions With {
            .Epochs = 2,
            .BatchSize = 4,
            .LearningRate = 0.005,
            .SequenceLength = SmallConfig.SeqLen,
            .EnumerateSmiles = False,
            .ValidateEverySteps = 2,
            .Seed = 3,
            .CheckpointPath = Path.Combine(workDir, "checkpoint.pt")
        }
        Dim trainer As New SmilesPretrainer(model2, vocab, options)
        Dim messages As New List(Of String)

        trainer.Logger = Sub(message) messages.Add(message)

        Dim history = trainer.Train(corpus, corpus.Take(4))

        Call checkThat(history.Count >= 4, "trainer: 训练历史非空")
        Call checkThat(trainer.BestValidationLoss.HasValue, "trainer: 记录了验证损失")
        Call checkThat(File.Exists(options.CheckpointPath), "trainer: 最优权重已落盘")
        Call checkThat(messages.Count > 0, "trainer: 日志回调被调用")
        Call DebugAudit.Snapshot(DebugAudit.V(history.Count),
                                 DebugAudit.V(If(trainer.BestValidationLoss, Double.NaN)))
    End Sub

#End Region

#Region "官方权重"

    ''' <summary>
    ''' 用官方预训练权重（smiles-transformer 的 <c>trfm_*.pkl</c>）做端到端校验。
    ''' </summary>
    ''' <remarks>
    ''' 由于没有配套的 <c>vocab.pkl</c>，无法还原 token→id 的语义映射，所以这里不做"下游指标"，
    ''' 而是利用该模型的架构特性做**与词表无关的正确性判定**：
    ''' 它是"编码器与解码器共享同一份输入、且解码器不加因果掩码"的自编码器，
    ''' 因此权重映射正确时，模型应当近乎完美地复制输入 → NLL 远低于均匀分布基线 <c>ln(V)</c>；
    ''' 若 QKV 拆分顺序、Linear 转置或 LayerNorm 参数错位，NLL 会退化到 <c>≈ ln(V)</c>。
    ''' 同时用 <c>ln(V)</c> 与"随机初始化模型"的结果作为对照基线。
    ''' </remarks>
    Private Sub testOfficialWeights(modelPath As String)
        If Not File.Exists(modelPath) Then
            Call DebugAudit.Mark("official.skipped", $"{modelPath} 不存在")

            Return
        End If

        Dim fileSize As New FileInfo(modelPath)
        Dim format As PtContainerFormat = PtLegacyFile.DetectFormat(modelPath)

        Call DebugAudit.Mark("official.file", $"{fileSize.Name}, {fileSize.Length / 1048576}MB, {format}")
        Call equalTo(PtContainerFormat.Legacy, format, "official: 官方文件是 torch legacy 归档")

        Dim tensors As Dictionary(Of String, Tensor) = PtLegacyFile.LoadStateDict(modelPath).AsTensors()

        Call DebugAudit.Mark("official.tensors", tensors.Count)
        Call checkThat(tensors.Count > 0, "official: 解析出张量")
        Call checkThat(tensors.ContainsKey("embed.weight"), "official: 含 embed.weight")
        Call checkThat(tensors.ContainsKey("out.weight"), "official: 含 out.weight")
        Dim positional As Tensor = positionalBuffer(tensors)

        Call checkThat(positional IsNot Nothing, "official: 含位置编码 buffer")
        Call checkThat(tensors.ContainsKey("trfm.encoder.layers.0.self_attn.in_proj_weight"),
                       "official: 含 fused QKV 权重")

        Dim vocabSize As Integer = tensors("embed.weight").Shape(0)
        Dim hidden As Integer = tensors("embed.weight").Shape(1)
        Dim layers As Integer = countEncoderLayers(tensors)
        Dim dff As Integer = tensors("trfm.encoder.layers.0.linear1.weight").Shape(0)
        Dim seqLen As Integer = officialSequenceLength(tensors, 220)

        Call DebugAudit.Mark("official.shape",
                             $"V={vocabSize}, H={hidden}, dff={dff}, T={seqLen}, layers={layers}, pe={String.Join("x", positional.Shape)}")
        Call equalTo(256, hidden, "official: 模型宽度为 256")
        Call equalTo(4, layers, "official: 编码器/解码器各 4 层")
        Call equalTo(256, dff, "official: 前馈宽度为 256")

        ' 位置编码公式一致性（官方 buffer 只生成到 max_len=5000，取前 seqLen 行比较）
        Dim peDiff As Double = positionalEncodingDiff(positional, seqLen, hidden)

        ' 官方 buffer 以 fp32 存储，因此比较阈值取 1e-4（fp32 相对精度约 1e-7，绝对值随位置增大）
        Call DebugAudit.Mark("official.pe_diff", peDiff)
        Call checkThat(peDiff < 1.0E-4, $"official: 位置编码与标准正弦公式一致（maxDiff={peDiff:E3}）")

        ' 固定 id 序列（用于指纹相关测试）
        Dim ids As Integer(,) = officialSampleIds(vocabSize, 48, 6)
        Dim baseline As Double = std.Log(vocabSize)

        ' 判定权重映射是否正确（与词表标签无关）：
        ' 该模型是"输入即输出"的自编码器，因此对**分布内的 id 序列**会给出高度尖锐的分布。
        ' 词表按频次降序排列，id=6 的经验探针（重复 token）在官方权重下 NLL≈0.11，
        ' 说明它对应一个可在语料中连续重复的常见 token；用同一条序列做跨头数比较，
        ' 映射正确/头数正确的组合会显著胜出，映射错误则退化为 ln(V)。
        Dim probe As Integer() = repeatedTokenIds(6, 16)
        Dim peaks As New Dictionary(Of Integer, Double)()
        Dim chosen As SequenceTransformer = Nothing
        Dim loadReport As TransformerLoadReport = Nothing

        For Each heads As Integer In New Integer() {2, 4, 8}
            If hidden Mod heads <> 0 Then Continue For

            Try
                Dim candidate As New SequenceTransformer(vocabSize, hidden, layers, heads, dff, seqLen, 0.1, False, 42)
                Dim report As TransformerLoadReport = TransformerStateDict.Apply(candidate, tensors, strict:=False)
                Dim loss As Double = sequenceLoss(candidate, probe)
                Dim greedyLoss As Double = sequenceLoss(candidate, greedyDecode(candidate, 16))

                peaks(heads) = loss

                If loadReport Is Nothing OrElse report.IsPerfectMatch Then loadReport = report
                If heads = 4 Then chosen = candidate

                Call DebugAudit.Mark($"official.probe.heads{heads}",
                                     $"probe_nll={loss:F4}, greedy_nll={greedyLoss:F4}, dk={candidate.HeadDim}")
            Catch ex As Exception
                Call checkThat(False, $"official: heads={heads}（dk={hidden \ heads}）前向失败：{ex.GetType().Name}: {ex.Message}")
            End Try
        Next

        Call DebugAudit.Mark("official.load", loadReport.ToString())
        Call checkThat(loadReport.IsPerfectMatch,
                       $"official: state_dict 键名映射完全匹配（{loadReport}）")

        If peaks.Count > 0 Then
            Dim bestHeads As Integer = peaks.OrderBy(Function(kv) kv.Value).First().Key
            Dim bestLoss As Double = peaks(bestHeads)

            Call DebugAudit.Mark("official.best_heads", bestHeads)
            Call equalTo(4, bestHeads, "official: 探针损失最小的头数与论文配置一致（nhead=4）")
            Call checkThat(bestLoss < 1.0,
                           $"official: 分布内序列的 NLL({bestLoss:F4}) 远低于均匀分布基线({baseline:F4})")
        End If

        ' 对照组：随机初始化模型的同一探针（应接近 ln(V)）
        Dim randomModel As New SequenceTransformer(vocabSize, hidden, layers, 4, dff, seqLen, 0.1, False, 7)
        Dim randomLoss As Double = sequenceLoss(randomModel, probe)

        Call DebugAudit.Mark("official.random_baseline", randomLoss)
        Call checkThat(randomLoss > 3.0,
                       $"official: 随机初始化模型在同一序列上接近均匀（{randomLoss:F4} vs 基线 {baseline:F4}）")

        If chosen Is Nothing Then Return

        ' 经验探针：词表按频次降序排列，id 5 极可能是最高频 token（脂肪碳 C）。
        ' 若权重映射正确，像 "CCC…" 这样的序列属于语料分布内，NLL 会明显低于 ln(V)；
        ' 映射错误时输出退化，NLL 会贴着均匀分布基线。
        For token As Integer = 5 To 9
            Dim probeIds As Integer() = repeatedTokenIds(token, 16)

            Call DebugAudit.Mark($"official.probe.id{token}",
                                 $"nll={sequenceLoss(chosen, probeIds):F4}, baseline={baseline:F4}")
        Next

        ' 指纹：维度、有限性、确定性与可区分性
        Dim encoded As Double(,) = chosen.Encode(ids, chunkSize:=3)

        Call equalTo(ids.GetLength(0), encoded.GetLength(0), "official: 指纹行数")
        Call equalTo(hidden * 4, encoded.GetLength(1), "official: 指纹维度 4H=1024")

        Dim again As Double(,) = chosen.Encode(ids, chunkSize:=3)
        Dim repeatDiff As Double = 0.0
        Dim finite As Boolean = True

        For r As Integer = 0 To encoded.GetLength(0) - 1
            For c As Integer = 0 To encoded.GetLength(1) - 1
                If Double.IsNaN(encoded(r, c)) OrElse Double.IsInfinity(encoded(r, c)) Then finite = False

                repeatDiff = std.Max(repeatDiff, std.Abs(encoded(r, c) - again(r, c)))
            Next
        Next

        Call checkThat(finite, "official: 指纹数值有限")
        Call checkThat(repeatDiff < 1.0E-12, $"official: 指纹确定（重复调用 maxDiff={repeatDiff:E3}）")

        Dim rowDiff As Double = 0.0

        For c As Integer = 0 To encoded.GetLength(1) - 1
            rowDiff = std.Max(rowDiff, std.Abs(encoded(0, c) - encoded(1, c)))
        Next

        Call checkThat(rowDiff > 1.0E-6, $"official: 不同输入得到不同指纹（行差={rowDiff:E3}）")

        ' 导出后重新加载，验证序列化闭环
        Dim exported As Dictionary(Of String, Tensor) = TransformerStateDict.Export(chosen)
        Dim reloaded As New SequenceTransformer(vocabSize, hidden, layers, 4, dff, seqLen, 0.1, False, 1)
        Dim reloadReport As TransformerLoadReport = TransformerStateDict.Apply(reloaded, exported)

        Call checkThat(reloadReport.IsPerfectMatch, "official: 导出→重载键集合一致")

        Dim reference As Tensor = chosen.Predict(ids)
        Dim restored As Tensor = reloaded.Predict(ids)
        Dim predictDiff As Double = 0.0

        For i As Integer = 0 To reference.Length - 1
            predictDiff = std.Max(predictDiff, std.Abs(reference.Data(i) - restored.Data(i)))
        Next

        Call checkThat(predictDiff < 1.0E-12, $"official: 导出→重载后前向逐位一致（maxDiff={predictDiff:E3}）")
    End Sub

    ''' <summary>统计 <c>trfm.encoder.layers.N</c> 的层数。</summary>
    Private Function countEncoderLayers(tensors As Dictionary(Of String, Tensor)) As Integer
        Dim maxLayer As Integer = -1

        For Each name As String In tensors.Keys
            If Not name.StartsWith("trfm.encoder.layers.", StringComparison.Ordinal) Then Continue For

            Dim rest As String = name.Substring("trfm.encoder.layers.".Length)
            Dim dot As Integer = rest.IndexOf("."c)

            If dot <= 0 Then Continue For

            Dim index As Integer = -1

            If Integer.TryParse(rest.Substring(0, dot), index) AndAlso index > maxLayer Then maxLayer = index
        Next

        Return maxLayer + 1
    End Function

    ''' <summary>
    ''' 取位置编码 buffer：官方 state_dict 里是 <c>pe.pe</c>（模块前缀 + buffer 名），
    ''' 某些实现直接叫 <c>pe</c>。
    ''' </summary>
    Private Function positionalBuffer(tensors As Dictionary(Of String, Tensor)) As Tensor
        Dim buffer As Tensor = Nothing

        If tensors.TryGetValue("pe.pe", buffer) Then Return buffer
        If tensors.TryGetValue("pe", buffer) Then Return buffer

        Return Nothing
    End Function

    ''' <summary>
    ''' 位置编码的序列长度：优先由位置编码 buffer 的形状推断（上限 220），
    ''' 官方 buffer 生成到 5000 行，因此这里取 <c>min(dim, 220)</c>。
    ''' </summary>
    Private Function officialSequenceLength(tensors As Dictionary(Of String, Tensor), fallback As Integer) As Integer
        Dim buffer As Tensor = positionalBuffer(tensors)

        If buffer Is Nothing Then Return fallback

        Dim shape As Integer() = buffer.Shape

        If shape.Length < 2 Then Return fallback

        Return std.Min(220, shape(shape.Length - 2))
    End Function

    ''' <summary>比较官方 <c>pe</c> buffer 与本地标准公式生成的位置编码（只比较前 <paramref name="seqLen"/> 行）。</summary>
    Private Function positionalEncodingDiff(pe As Tensor, seqLen As Integer, hidden As Integer) As Double
        Dim local As Tensor = TokenEmbedding.buildPositionalEncoding(seqLen, hidden)
        Dim maxDiff As Double = 0.0

        For pos As Integer = 0 To seqLen - 1
            For i As Integer = 0 To hidden - 1
                maxDiff = std.Max(maxDiff, std.Abs(pe.Data(pos * hidden + i) - local.Data(pos * hidden + i)))
            Next
        Next

        Return maxDiff
    End Function

    ''' <summary>构造固定的测试 id 序列：<c>sos + 随机 token + eos + pad</c>。</summary>
    Private Function officialSampleIds(vocabSize As Integer, time As Integer, batch As Integer) As Integer(,)
        Dim ids(batch - 1, time - 1) As Integer
        Dim rnd As New Random(2024)
        Dim minToken As Integer = std.Min(5, vocabSize - 1)

        For b As Integer = 0 To batch - 1
            ids(b, 0) = WordVocab.SosIndex

            For t As Integer = 1 To time - 2
                ids(b, t) = rnd.Next(minToken, vocabSize)
            Next

            ids(b, time - 1) = WordVocab.EosIndex
        Next

        Return ids
    End Function

    ''' <summary>计算"输入即输出"的重建 NLL（忽略 pad）。</summary>
    Private Function reconstructionLoss(model As SequenceTransformer, ids As Integer(,)) As Double
        Dim logProbs As Tensor = model.Predict(ids)
        Dim targets As Integer() = flattenIds(ids)
        Dim dLogits As Tensor = Nothing

        Return TensorOpsEx.NllLoss(logProbs, targets, WordVocab.PadIndex, dLogits)
    End Function

    ''' <summary>
    ''' 从 <c>&lt;sos&gt;</c> 开始贪心解码 <paramref name="maxTokens"/> 个 token。
    ''' </summary>
    ''' <param name="model">被测模型。</param>
    ''' <param name="maxTokens">解码长度（含首位的 sos）。</param>
    Private Function greedyDecode(model As SequenceTransformer, maxTokens As Integer) As Integer()
        Dim vocab As Integer = model.VocabSize
        Dim decoded(maxTokens - 1) As Integer

        decoded(0) = WordVocab.SosIndex

        For t As Integer = 0 To maxTokens - 2
            Dim prefix(0, t) As Integer

            For i As Integer = 0 To t
                prefix(0, i) = decoded(i)
            Next

            Dim logProbs As Tensor = model.Predict(prefix)
            Dim offset As Integer = t * vocab
            Dim best As Integer = 0
            Dim bestValue As Double = Double.NegativeInfinity

            For v As Integer = 0 To vocab - 1
                If logProbs.Data(offset + v) > bestValue Then
                    bestValue = logProbs.Data(offset + v)
                    best = v
                End If
            Next

            decoded(t + 1) = best
        Next

        Return decoded
    End Function

    ''' <summary>构造 <c>sos + token×n + eos</c> 的重复序列（词表探针）。</summary>
    ''' <param name="token">重复的 token id。</param>
    ''' <param name="length">总长度。</param>
    Private Function repeatedTokenIds(token As Integer, length As Integer) As Integer()
        Dim ids(length - 1) As Integer

        ids(0) = WordVocab.SosIndex

        For i As Integer = 1 To length - 2
            ids(i) = token
        Next

        ids(length - 1) = WordVocab.EosIndex

        Return ids
    End Function

    ''' <summary>计算一条 id 序列在模型下的平均 NLL（不忽略任何位置）。</summary>
    ''' <param name="model">被测模型。</param>
    ''' <param name="decoded">id 序列。</param>
    Private Function sequenceLoss(model As SequenceTransformer, decoded As Integer()) As Double
        Dim ids(0, decoded.Length - 1) As Integer

        For i As Integer = 0 To decoded.Length - 1
            ids(0, i) = decoded(i)
        Next

        Dim logProbs As Tensor = model.Predict(ids)
        Dim dLogits As Tensor = Nothing

        Return TensorOpsEx.NllLoss(logProbs, decoded, -1, dLogits)
    End Function

    ''' <summary>把 <c>[batch, time]</c> 按行优先展平（与 <c>[B,T,V]</c> 的展平顺序一致）。</summary>
    Private Function flattenIds(ids As Integer(,)) As Integer()
        Dim batch As Integer = ids.GetLength(0)
        Dim time As Integer = ids.GetLength(1)
        Dim targets(batch * time - 1) As Integer

        For b As Integer = 0 To batch - 1
            For t As Integer = 0 To time - 1
                targets(b * time + t) = ids(b, t)
            Next
        Next

        Return targets
    End Function

#End Region

End Module
