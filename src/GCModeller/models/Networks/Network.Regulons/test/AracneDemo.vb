Imports System.IO
Imports DebugAudit = Microsoft.VisualBasic.ApplicationServices.Debugging.DebugAudit
Imports DebugAuditScope = Microsoft.VisualBasic.ApplicationServices.Debugging.DebugAuditScope
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.Model.Network.Regulons.ARACNE
Imports SMRUCC.genomics.Model.Network.Regulons.LinearRegression
Imports std = System.Math

''' <summary>
''' ARACNE 基因调控网络推断模块的 demo 与自检程序。
''' 
''' ```
''' dotnet run --project test.vbproj -- --aracne
''' dotnet run --project test.vbproj -- --aracne --matrix "K:\hsa_grn\20261025\GRN_expression_matrix_metacell30k_18533genes_x_30409cells.csv"
''' ```
''' 
''' 合成数据的用例覆盖：
''' 
''' 1. 四种互信息估计器的正确性、对称性与非线性检测能力；
''' 2. 合成的 TF-靶基因网络重建，以及 DPI 对链式间接边的剔除；
''' 3. 置换零分布阈值的合理性；
''' 4. TF 先验驱动的方向赋予与各种导出格式；
''' 5. DPI 容差(epsilon)与 TF 导向模式的行为；
''' 6. 输入数据合法性检查；
''' 7. (可选) 真实单细胞表达矩阵之上的推断。
''' </summary>
Module AracneDemo

    Private Const AUDIT_REPORT As String = "aracne-audit-report.json"

    ''' <summary>
    ''' 默认的转录因子先验列表(基因符号)
    ''' </summary>
    Private Const TF_FILE As String = "G:\vcc2026\data\Homo_sapiens_TF.txt"

    ''' <summary>
    ''' 默认的 curated TF-target 先验网络
    ''' </summary>
    Private Const PRIOR_FILE As String = "G:\vcc2026\data\TF_target_prior_curated.csv"

    ''' <summary>
    ''' 合成网络的构建结果
    ''' </summary>
    Public Class SyntheticNetwork

        ''' <summary>
        ''' 表达矩阵(行=基因、列=样本)
        ''' </summary>
        ''' <returns></returns>
        Public Property matrix As Matrix

        ''' <summary>
        ''' 转录因子列表
        ''' </summary>
        ''' <returns></returns>
        Public Property tfs As String()

        ''' <summary>
        ''' 真实的调控边
        ''' </summary>
        ''' <returns></returns>
        Public Property truth As (regulator As String, target As String)()

    End Class

    ''' <summary>
    ''' ARACNE demo 的入口
    ''' </summary>
    ''' <param name="args">命令行参数</param>
    Public Sub RunAracneDemo(args As String())
        Dim report As String = ArgValue(args, "--report", Path.Combine(Path.GetTempPath(), AUDIT_REPORT))

        DebugAudit.MaxEvents = 2048

        Console.WriteLine("== ARACNE gene regulatory network inference ==")
        Console.WriteLine("   MI estimators : gaussian copula / adaptive partitioning / equal-frequency histogram / kernel density")
        Console.WriteLine("   threshold     : permutation null distribution (or fixed)")
        Console.WriteLine("   filters       : DPI (standard / transcription-factor guided)")
        Console.WriteLine("   prior         : TF list + curated TF-target network (threshold relaxation, top-K and DPI protection)")
        Console.WriteLine()

        RunCase("case A1: mutual information estimators", AddressOf CaseEstimators)
        RunCase("case A2: rebuild a synthetic TF-target network", AddressOf CaseSyntheticNetwork)
        RunCase("case A3: permutation significance threshold", AddressOf CaseThreshold)
        RunCase("case A4: TF direction assignment and exports", AddressOf CaseDirectionExport)
        RunCase("case A5: DPI tolerance semantics", AddressOf CaseDpiTolerance)
        RunCase("case A6: input validation", AddressOf CaseValidation)
        RunCase("case A8: curated prior knowledge (TF list + TF-target evidence)", AddressOf CasePriorKnowledge)
        RunCase("case A9: confidence-graded prior protection", AddressOf CaseGradedProtection)

        Dim matrixFile As String = ArgValue(args, "--matrix")

        If Not String.IsNullOrEmpty(matrixFile) Then
            RunCase("case A7: ARACNE on the real expression matrix", Sub() CaseRealMatrix(matrixFile, args))
        Else
            Console.WriteLine("   (skip case A7: pass --matrix <csv> to run ARACNE on a real expression matrix)")
            Console.WriteLine()
        End If

        Call DebugAudit.SaveReport(report)

        Console.WriteLine($"audit report -> {report}")
        Console.WriteLine($"ARACNE demo: {DebugAudit.FailureCount} failure(s), exit code = {If(DebugAudit.FailureCount = 0, 0, 1)}")
    End Sub

    ''' <summary>
    ''' 用例 A1：四种互信息估计器
    ''' </summary>
    Private Sub CaseEstimators()
        Dim rng As New Random(101)
        Dim n As Integer = 400
        Dim x As Double() = New Double(n - 1) {}
        Dim negative As Double() = New Double(n - 1) {}
        Dim nonlinear As Double() = New Double(n - 1) {}
        Dim independent As Double() = New Double(n - 1) {}

        For i As Integer = 0 To n - 1
            x(i) = Gauss(rng)
            negative(i) = -x(i)
            nonlinear(i) = x(i) * x(i)
            independent(i) = Gauss(rng) * (rng.NextDouble() + 0.5R)
        Next

        ' 1. copula 估计器只依赖 |r|，因此 x 与 -x 的互信息必须等于 x 与 x 的互信息
        Dim miSelf As Double = MutualInformation.GaussianCopula(x, x)
        Dim miNegative As Double = MutualInformation.GaussianCopula(x, negative)
        Dim miIndependent As Double = MutualInformation.GaussianCopula(x, independent)
        Dim miNonlinear As Double = MutualInformation.GaussianCopula(x, nonlinear)
        Dim miAdaptive As Double = MutualInformation.AdaptivePartitioning(x, nonlinear)
        Dim miHistogram As Double = MutualInformation.EqualFrequencyHistogram(x, nonlinear, 4)
        Dim miKernel As Double = MutualInformation.KernelDensity(x, nonlinear)

        Console.WriteLine($"     >> copula: self={miSelf:F4}, -x={miNegative:F4}, independent={miIndependent:F4}, x^2={miNonlinear:F4}")
        Console.WriteLine($"     >> x^2   : adaptive={miAdaptive:F4}, histogram={miHistogram:F4}, kernel={miKernel:F4}")

        Call DebugAudit.Test(miSelf = miNegative, $"the copula estimator is invariant to the sign: {miSelf:F6} = {miNegative:F6}")
        Call DebugAudit.Test(miSelf > 10, $"the perfectly correlated genes give an unbounded MI: {miSelf:F2}")
        Call DebugAudit.Test(miIndependent < 0.05R, $"the independent genes give almost no mutual information: {miIndependent:F4}")
        Call DebugAudit.Test(miNonlinear < 0.05R, $"the copula estimator misses the symmetric non-linear relation: {miNonlinear:F4}")
        Call DebugAudit.Test(miAdaptive > 0.1R, $"the adaptive partitioning detects the non-linear relation: {miAdaptive:F4}")
        Call DebugAudit.Test(miHistogram > 0.05R, $"the equal-frequency histogram detects the non-linear relation: {miHistogram:F4}")
        Call DebugAudit.Test(miKernel > 0.05R, $"the kernel density estimator detects the non-linear relation: {miKernel:F4}")

        ' 2. 自适应分区对调两个变量之后的结果必须基本一致
        Dim swapped As Double = MutualInformation.AdaptivePartitioning(nonlinear, x)

        Call DebugAudit.Test(std.Abs(swapped - miAdaptive) < 0.05R,
            $"the adaptive partitioning is symmetric: {miAdaptive:F4} ~ {swapped:F4}")

        ' 3. 单调非线性关系：pearson 低估了依赖强度，秩变换(spearman)可以完全捕捉
        Dim cube As Double() = x.Select(Function(v) v * v * v).ToArray
        Dim miCube As Double = MutualInformation.GaussianCopula(x, cube)
        Dim miCubeRank As Double = MutualInformation.GaussianCopula(
            MutualInformation.Ranks(x), MutualInformation.Ranks(cube))

        Console.WriteLine($"     >> monotone x^3: copula={miCube:F4}, rank copula={miCubeRank:F4}")

        Call DebugAudit.Test(miCubeRank > miCube + 0.5R,
            $"the rank transform exposes the monotone dependence: {miCubeRank:F4} > {miCube:F4}")

        ' 4. 边缘熵
        Dim entropy As Double = MutualInformation.Entropy(x, 4)

        Call DebugAudit.Test(entropy > 1.0R, $"the marginal entropy of a gaussian variable is about 2 bits: {entropy:F4}")
    End Sub

    ''' <summary>
    ''' 用例 A2：从合成的 TF-靶基因网络之中重建调控关系，并检查 DPI 对链式间接边的剔除
    ''' </summary>
    Private Sub CaseSyntheticNetwork()
        Dim synth As SyntheticNetwork = BuildSyntheticNetwork(nSample:=300, seed:=7)
        Dim net As AracneNetwork = Infer(synth)
        Dim edges As AracneEdge() = net.Edges
        Dim predicted As HashSet(Of String) = New HashSet(Of String)(edges.Select(Function(e) $"{e.regulator}->{e.target}"))
        Dim truth As HashSet(Of String) = New HashSet(Of String)(synth.truth.Select(Function(t) $"{t.regulator}->{t.target}"))
        Dim hit As Integer = truth.Where(Function(t) predicted.Contains(t)).Count()
        Dim recall As Double = hit / CDbl(truth.Count)
        Dim precision As Double = If(predicted.Count = 0, 0.0R, hit / CDbl(predicted.Count))

        Console.WriteLine($"     >> {synth.matrix.size} genes x {synth.matrix.sample_count} samples")
        Console.WriteLine($"     >> {truth.Count} true edges, {predicted.Count} predicted edges, " &
                          $"recall={recall:F3}, precision={precision:F3}")
        Console.WriteLine($"     >> {net.Statistics.ToString}")

        Call DebugAudit.Test(edges.Length > 0, $"{edges.Length} regulation edges inferred")
        Call DebugAudit.Test(recall > 0.8R, $"the true edges are recovered: recall = {recall:F3}")
        Call DebugAudit.Test(precision > 0.6R, $"the predicted network is dominated by true edges: precision = {precision:F3}")

        ' 链式关系 TF_A -> CHAIN_MED -> CHAIN_Y 之中，TF_A 与 CHAIN_Y 之间是一条间接边：
        ' 它在 DPI 之前存在(互信息高于阈值)，在 DPI 之后被剔除
        Dim u As Integer = Array.IndexOf(net.Genes, CHAIN_TF)
        Dim med As Integer = Array.IndexOf(net.Genes, CHAIN_MED)
        Dim y As Integer = Array.IndexOf(net.Genes, CHAIN_Y)
        Dim before As Double = If(net.MiMatrixBeforeDpi Is Nothing, 0.0R, net.MiMatrixBeforeDpi(u, y))
        Dim after As Double = net.MiMatrix(u, y)

        Console.WriteLine($"     >> indirect edge {CHAIN_TF}-{CHAIN_Y}: MI before DPI = {before:F4}, after DPI = {after:F4} " &
                          $"(direct links: {CHAIN_TF}-{CHAIN_MED} = {net.MiMatrix(u, med):F4}, {CHAIN_MED}-{CHAIN_Y} = {net.MiMatrix(med, y):F4})")

        Call DebugAudit.Test(before > 0.0R, $"the indirect edge exists before DPI: MI = {before:F4}")
        Call DebugAudit.Test(after = 0.0R, $"the indirect edge is removed by DPI: MI = {after:F4}")
        Call DebugAudit.Test(net.MiMatrix(u, med) > 0.0R AndAlso net.MiMatrix(med, y) > 0.0R,
            "the two direct links of the chain survive the DPI filtering")
        Call DebugAudit.Test(net.Statistics.dpi.RemovedEdges.Count > 0,
            $"{net.Statistics.dpi.RemovedEdges.Count} indirect edges are removed by {net.Statistics.dpi.Passes} DPI pass(es)")

        ' 相互独立的随机基因不应该有任何调控关系
        Dim randomGenes As String() = {RND_1, RND_2, RND_3}
        Dim randomEdges As AracneEdge() = edges _
            .Where(Function(e) randomGenes.Contains(e.regulator) OrElse randomGenes.Contains(e.target)) _
            .ToArray

        Call DebugAudit.Test(randomEdges.Length <= 1,
            $"{randomEdges.Length} noise gene(s) are (almost) not connected: " &
            String.Join(", ", randomEdges.Select(Function(e) e.UID)))
    End Sub

    ''' <summary>
    ''' 用例 A3：置换零分布阈值
    ''' </summary>
    Private Sub CaseThreshold()
        Dim synth As SyntheticNetwork = BuildSyntheticNetwork(nSample:=300, seed:=7)
        Dim net As AracneNetwork = Infer(synth)
        Dim tau As MiThreshold = net.Threshold

        Console.WriteLine($"     >> {tau.ToString}")
        Console.WriteLine($"     >> null mean = {tau.nullMean:F4}, sd = {tau.nullSD:F4}, max = {tau.nullMax:F4}, " &
                          $"signal ratio = {tau.SignalRatio:F1}")

        Call DebugAudit.Test(tau.mode = MiThresholdMode.Permutation, "the threshold is estimated from the permutation null distribution")
        Call DebugAudit.Test(tau.value > 0.0R, $"the estimated threshold is positive: {tau.value:F4}")
        Call DebugAudit.Test(tau.value > tau.nullMean, $"the threshold is above the null mean: {tau.value:F4} > {tau.nullMean:F4}")
        Call DebugAudit.Test(tau.value < net.MiMatrixBeforeDpi.MaxMi,
            $"the threshold does not wipe out all of the edges: {tau.value:F4} < {net.MiMatrixBeforeDpi.MaxMi:F4}")
        Call DebugAudit.Test(tau.SignalRatio > 2.0R, $"the threshold is far above the noise level: x{tau.SignalRatio:F1}")

        ' 使用相同的数值作为固定阈值，结果应当基本一致
        Dim fixedNet As AracneNetwork = synth.matrix.InferAracneNetwork(New AracneOptions With {
            .silent = True,
            .transcriptionFactors = synth.tfs,
            .thresholdMode = MiThresholdMode.Fixed,
            .miThreshold = tau.value,
            .maxNeighbors = 10
        })
        Dim gap As Double = std.Abs(fixedNet.Edges.Length - net.Edges.Length) / CDbl(std.Max(1, net.Edges.Length))

        Console.WriteLine($"     >> fixed tau = {tau.value:F4}: {fixedNet.Edges.Length} edges vs {net.Edges.Length} edges (permutation)")

        Call DebugAudit.Test(gap < 0.25R, $"the fixed threshold reproduces the permutation threshold: {fixedNet.Edges.Length} vs {net.Edges.Length}")

        ' 其它估计器之下的端到端冒烟测试
        Dim adaptiveNet As AracneNetwork = synth.matrix.InferAracneNetwork(New AracneOptions With {
            .silent = True,
            .transcriptionFactors = synth.tfs,
            .miEstimator = MiEstimator.AdaptivePartitioning,
            .permutationPairs = 300,
            .maxNeighbors = 10
        })

        Console.WriteLine($"     >> adaptive partitioning: {adaptiveNet.Edges.Length} edges, {adaptiveNet.Threshold.ToString}")

        Call DebugAudit.Test(adaptiveNet.Edges.Length > 0,
            $"the adaptive partitioning estimator builds a network with {adaptiveNet.Edges.Length} edges")
        Call DebugAudit.Test(adaptiveNet.Threshold.estimator = MiEstimator.AdaptivePartitioning,
            "the permutation null distribution uses the same estimator as the network")
    End Sub

    ''' <summary>
    ''' 用例 A4：方向赋予与各种导出格式
    ''' </summary>
    Private Sub CaseDirectionExport()
        Dim synth As SyntheticNetwork = BuildSyntheticNetwork(nSample:=300, seed:=7)
        Dim net As AracneNetwork = Infer(synth)

        Call DebugAudit.Test(net.Edges.All(Function(e) e.interaction <> "undirected"),
            "no undirected edge is reported when a TF list is given")
        Call DebugAudit.Test(net.Edges.All(Function(e) e.isTfSource),
            "each edge starts from a transcription factor")
        Call DebugAudit.Test(net.Edges.Any(Function(e) e.regulator = CHAIN_TF AndAlso e.target = CHAIN_MED),
            $"the direction {CHAIN_TF} -> {CHAIN_MED} is correct")
        Call DebugAudit.Test(net.Edges.Any(Function(e) e.regulator = CHAIN_MED AndAlso e.target = CHAIN_Y),
            $"the chain direction {CHAIN_MED} -> {CHAIN_Y} is correct")
        Call DebugAudit.Test(Not net.Edges.Any(Function(e) e.regulator = "G_1"),
            "a non-transcription factor never acts as a regulator")

        ' TF 与 TF 之间的强相关会构成双向的 mutual 边
        Dim pair As Matrix = BuildTfPairMatrix(nSample:=200, seed:=11)
        Dim pairNet As AracneNetwork = pair.InferAracneNetwork(New AracneOptions With {
            .silent = True,
            .transcriptionFactors = {TF_PAIR_A, TF_PAIR_B},
            .permutationPairs = 200
        })

        Console.WriteLine($"     >> TF-TF network: {pairNet.Edges.Length} edge(s): " &
                          String.Join("; ", pairNet.Edges.Select(Function(e) $"{e.UID}({e.interaction})")))

        Call DebugAudit.Test(pairNet.Edges.Length = 2, "the TF-TF edge is exported in both directions")
        Call DebugAudit.Test(pairNet.Edges.All(Function(e) e.interaction = "mutual"), "the TF-TF edges are tagged as 'mutual'")

        ' CSV 导出
        Dim dir As String = Path.Combine(Path.GetTempPath(), "aracne-demo")
        Dim csv As String = Path.Combine(dir, "aracne-edges.csv")

        Call net.WriteEdges(csv)

        Dim lines As String() = File.ReadAllLines(csv)

        Call DebugAudit.Test(File.Exists(csv), $"the edge table is saved into {csv}")
        Call DebugAudit.AreEqual(lines(0), "regulator,target,mi,correlation,interaction,is_tf_source,is_tf_target,prior,regulation", "the csv header is correct")
        Call DebugAudit.AreEqual(lines.Length - 1, net.Edges.Length, "the edge table row count matches the network")
        Call DebugAudit.Test(lines.Skip(1).All(Function(l) l.Split(","c).Length = 9), "each row has 9 columns")
        Call DebugAudit.Test(lines.Skip(1).All(Function(l) l.EndsWith(",0,")), "the edges without prior evidence are flagged as 'prior=0'")

        ' 系数矩阵：行=调控因子、列=靶基因
        Dim coef As Matrix = net.CoefMatrix()

        Console.WriteLine($"     >> coefficient matrix: [{coef.size} x {coef.sample_count}], tag = {coef.tag}")

        Call DebugAudit.Test(coef.size = net.MiMatrix.NodeCount, $"the coefficient matrix rows = {coef.size} (nodes with at least one edge)")
        Call DebugAudit.AreEqual(coef.size, coef.sample_count, "the coefficient matrix is square")

        ' 与工程既有的网络对象互操作
        Dim connections As SMRUCC.genomics.Model.Network.Regulons.Connection() = net.Edges.ToConnections().ToArray
        Dim sizes = net.Edges.RegulonSizes().ToArray

        Call DebugAudit.AreEqual(connections.Length, net.Edges.Length, "each edge is converted into a network connection")
        Call DebugAudit.AreEqual(sizes.Sum(Function(s) s.size), net.Edges.Length, "the regulon sizes sum up to the edge count")
        Call DebugAudit.Test(net.Edges.TopEdges(5).Length <= 5, "the top edges are limited by the requested size")
        Call DebugAudit.Test(net.Regulations().First().mi >= net.Regulations().Last().mi, "the regulation edges are sorted by MI descending")

        Console.WriteLine("     >> top regulons: " &
                          String.Join(", ", sizes.Take(4).Select(Function(s) $"{s.regulator}({s.size})")))
    End Sub

    ''' <summary>
    ''' 用例 A5：DPI 的容差语义与 TF 导向模式
    ''' </summary>
    Private Sub CaseDpiTolerance()
        ' 构造一个“接近平局”的三元组：最弱的边只比另外两条边低 5% 左右
        Dim mi As New MiMatrix({"A", "B", "C"})

        Call mi.SetValue(0, 1, 0.84R)
        Call mi.SetValue(0, 2, 0.82R)
        Call mi.SetValue(1, 2, 0.80R)

        Dim strict As DpiResult = DpiFilter.Apply(mi, New AracneOptions With {
            .tolerance = 0.0R,
            .dpiMode = DpiMode.Standard
        })
        Dim relaxed As DpiResult = DpiFilter.Apply(mi, New AracneOptions With {
            .tolerance = 0.1R,
            .dpiMode = DpiMode.Standard
        })
        Dim guided As DpiResult = DpiFilter.Apply(mi, New AracneOptions With {
            .tolerance = 0.1R,
            .dpiMode = DpiMode.TfGuided
        }, {True, False, False})

        Console.WriteLine($"     >> epsilon=0: {strict.EdgesBefore} -> {strict.EdgesAfter} edges, removed = " &
                          String.Join("; ", strict.RemovedEdges.Select(Function(e) e.ToString)))
        Console.WriteLine($"     >> epsilon=0.1: {relaxed.EdgesBefore} -> {relaxed.EdgesAfter} edges")
        Console.WriteLine($"     >> TF guided: {guided.EdgesBefore} -> {guided.EdgesAfter} edges (A is a TF, B/C are not)")

        Call DebugAudit.AreEqual(2, strict.EdgesAfter, "epsilon=0 removes the weakest edge of a near-tie triplet")
        Call DebugAudit.AreEqual(3, relaxed.EdgesAfter, "epsilon=0.1 keeps the near-tie triplet intact")
        Call DebugAudit.AreEqual(2, guided.EdgesAfter, "the TF guided mode removes the non-TF edge even when the gap is small")
        Call DebugAudit.Test(guided.RemovedEdges.Any(Function(e) e.u = 1 AndAlso e.v = 2), "the removed edge is B-C (both non-TF)")
        Call DebugAudit.Test(strict.Passes >= 1, $"the DPI filtering runs at least one pass: {strict.Passes}")

        ' 关闭 DPI 的时候不会删除任何边
        Dim off As DpiResult = DpiFilter.Apply(mi, New AracneOptions With {.dpiMode = DpiMode.Off})

        Call DebugAudit.AreEqual(3, off.EdgesAfter, "DpiMode.Off keeps all of the edges")
        Call DebugAudit.AreEqual(0, off.RemovedEdges.Count, "DpiMode.Off removes nothing")
    End Sub

    ''' <summary>
    ''' 用例 A6：输入数据合法性检查
    ''' </summary>
    Private Sub CaseValidation()
        Dim rng As New Random(3)
        Dim vector As Double() = Enumerable.Range(0, 40).Select(Function(i) Gauss(rng)).ToArray

        Call DebugAudit.CheckThrows(Of InvalidDataException)(
            Sub()
                Dim probe As New AracneNetwork(MakeMatrix({"a"}, {vector}))
            End Sub,
            "a single gene matrix is rejected")

        Call DebugAudit.CheckThrows(Of InvalidDataException)(
            Sub()
                Dim probe As New AracneNetwork(MakeMatrix({"a", "a"}, {vector, vector}))
            End Sub,
            "duplicated gene id is rejected")

        Call DebugAudit.CheckThrows(Of InvalidDataException)(
            Sub()
                Dim few As Double() = vector.Take(10).ToArray
                Dim probe As New AracneNetwork(MakeMatrix({"a", "b"}, {few, few}))
            End Sub,
            "too few samples is rejected")

        Call DebugAudit.CheckThrows(Of InvalidDataException)(
            Sub()
                Dim probe As New AracneNetwork(MakeMatrix({"a", "b"}, {vector, vector}),
                                               New AracneOptions With {.transcriptionFactors = {"unknown_tf"}})
            End Sub,
            "an empty transcription factor intersection is rejected")

        Call DebugAudit.CheckThrows(Of InvalidDataException)(
            Sub()
                Dim constant As Double() = Enumerable.Repeat(0.5R, 40).ToArray
                Dim expr As Matrix = MakeMatrix({"a", "b"}, {constant, vector})
                Dim probe As AracneNetwork = New AracneNetwork(expr).Fit()
            End Sub,
            "a constant gene cannot be used as a regulator")
    End Sub

    ''' <summary>
    ''' 用例 A7：真实单细胞表达矩阵之上的 ARACNE 推断
    ''' 
    ''' ```
    ''' dotnet run --project test.vbproj -- --aracne --matrix "K:\hsa_grn\20261025\GRN_expression_matrix_metacell30k_18533genes_x_30409cells.csv" [--cells 800] [--genes 2000] [--tf &lt;tf.txt&gt;] [--prior &lt;prior.csv&gt;] [--prior-confidence 0.45] [--neighbors 200] [--out dir]
    ''' ```
    ''' 
    ''' ``--tf`` 默认为 ``G:\vcc2026\data\Homo_sapiens_TF.txt``，
    ''' ``--prior`` 默认为 ``G:\vcc2026\data\TF_target_prior_curated.csv``；
    ''' ``--prior-confidence`` 是“参与推断的资格门槛”(默认 0.45，即 Low 及以上)，
    ''' 而保护强度由每条边的置信度分级决定。
    ''' </summary>
    ''' <param name="file">表达矩阵 CSV 的路径</param>
    ''' <param name="args">命令行参数</param>
    Private Sub CaseRealMatrix(file As String, args As String())
        Dim cells As Integer = ArgInt(args, "--cells", 800)
        Dim genes As Integer = ArgInt(args, "--genes", 2000)
        Dim neighbors As Integer = ArgInt(args, "--neighbors", 200)
        Dim tfFile As String = ArgValue(args, "--tf", TF_FILE)
        Dim priorFile As String = ArgValue(args, "--prior", PRIOR_FILE)
        Dim priorConfidence As Double = ArgDouble(args, "--prior-confidence", 0.45R)
        Dim tfs As String() = Nothing
        Dim priorEdges As PriorEdge() = Nothing

        If Not String.IsNullOrEmpty(tfFile) AndAlso System.IO.File.Exists(tfFile) Then
            tfs = AracneMatrixCsvLoader.LoadTfList(tfFile)

            Console.WriteLine($"     >> {tfs.Length} transcription factors loaded from {Path.GetFileName(tfFile)}")
            Call DebugAudit.Test(tfs.Length > 100, $"{tfs.Length} transcription factor symbols are loaded from the prior list")
            Call DebugAudit.Test(tfs.All(Function(id) Not String.IsNullOrWhiteSpace(id) AndAlso id <> "Symbol"),
                "the TF list parser skips the header row and the blank symbols")
        Else
            Console.WriteLine("     >> no transcription factor list, the full N x N space will be evaluated")
        End If

        If Not String.IsNullOrEmpty(priorFile) AndAlso System.IO.File.Exists(priorFile) Then
            priorEdges = AracneMatrixCsvLoader.LoadPriorNetwork(priorFile, priorConfidence)

            Console.WriteLine($"     >> {priorEdges.Length} curated prior edges loaded from {Path.GetFileName(priorFile)} " &
                              $"(confidence >= {priorConfidence.ToString("G2")})")
            Call DebugAudit.Test(priorEdges.Length > 100, $"{priorEdges.Length} curated TF-target relations are loaded")
            Call DebugAudit.Test(priorEdges.All(Function(e) e.Confidence >= priorConfidence), "only the confident prior edges are kept")
        End If

        Dim loaded As AracneMatrixCsvLoader.LoadResult = AracneMatrixCsvLoader.Load(file, New AracneMatrixCsvLoader.LoadOptions With {
            .cells = cells,
            .genes = genes,
            .minNonZeroRatio = 0.1R,
            .logTransform = True,
            .transcriptionFactors = tfs
        })

        Console.WriteLine($"     >> {loaded.ToString}")
        Call DebugAudit.Test(loaded.retained > 100, $"{loaded.retained} genes are retained from the expression matrix")
        Call DebugAudit.Test(loaded.samples >= 100, $"{loaded.samples} cells are sampled from {loaded.totalSamples} cells")

        Dim options As New AracneOptions With {
            .transcriptionFactors = tfs,
            .miEstimator = MiEstimator.GaussianCopula,
            .rankTransform = True,
            .maxNeighbors = neighbors,
            .permutationPairs = 2000,
            .randomSeed = 20261031,
            .priorNetwork = priorEdges,
            .priorMinConfidence = priorConfidence
        }

        ' 先验的价值必须可以被量化：同样的数据、同样的参数，只是有没有先验
        Dim baseline As AracneNetwork = Nothing

        If priorEdges IsNot Nothing AndAlso priorEdges.Length > 0 Then
            Dim plainOptions As AracneOptions = New AracneOptions With {
                .transcriptionFactors = tfs,
                .miEstimator = MiEstimator.GaussianCopula,
                .rankTransform = True,
                .maxNeighbors = neighbors,
                .permutationPairs = 2000,
                .randomSeed = 20261031,
                .silent = True
            }

            baseline = loaded.matrix.InferAracneNetwork(plainOptions)

            Console.WriteLine($"     >> baseline (no prior): {baseline.Edges.Length} edges, " &
                              $"{baseline.Statistics.edgesAfterDpi} undirected edges after DPI")
        End If

        Dim watch As System.Diagnostics.Stopwatch = System.Diagnostics.Stopwatch.StartNew()
        Dim net As AracneNetwork = loaded.matrix.InferAracneNetwork(options)
        Dim seconds As Double = watch.Elapsed.TotalSeconds

        Console.WriteLine($"     >> {net.Statistics.ToString}")
        Console.WriteLine($"     >> threshold: {net.Threshold.ToString}")

        Call DebugAudit.Test(net.Statistics.genes > 100, $"{net.Statistics.genes} genes participate in the inference")
        Call DebugAudit.Test(net.Edges.Length > 0, $"{net.Edges.Length} regulation edges are inferred from the real data")
        Call DebugAudit.Test(net.Edges.All(Function(e) e.interaction <> "undirected"),
            "all of the edges are directed or mutual (the TF guided computation space)")
        Call DebugAudit.Test(seconds < 1800.0R, $"the inference finishes in {seconds:F1}s")

        If baseline IsNot Nothing Then
            Dim recovered As Integer = net.Statistics.priorSupportedPairs
            Dim recall As Double = If(net.Statistics.priorEdges = 0, 0.0R, recovered / CDbl(net.Statistics.priorEdges))
            Dim rescued As Integer = net.Statistics.priorProtectedEdges
            Dim extra As Integer = net.Edges.Length - baseline.Edges.Length

            ' 基线(不使用先验)能够独立召回多少先验边：这个差值才是先验的真正贡献
            Dim index As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)

            For i As Integer = 0 To net.Genes.Length - 1
                index(net.Genes(i)) = i
            Next

            Dim priorKeys As New HashSet(Of Long)(net.Edges _
                .Where(Function(e) e.prior) _
                .Select(Function(e) MiMatrix.EdgeKey(index(e.regulator), index(e.target))))

            Dim baselineSupported As Integer = baseline.Edges _
                .Select(Function(e) MiMatrix.EdgeKey(index(e.regulator), index(e.target))) _
                .Where(Function(key) priorKeys.Contains(key)) _
                .Distinct _
                .Count

            Console.WriteLine($"     >> prior: {net.Statistics.priorEdges} curated pairs in the gene space, " &
                              $"{recovered} supported pairs (recall = {recall:P1}; the baseline alone finds {baselineSupported}), " &
                              $"{rescued} rescued from DPI ({GradeText(net.Statistics.priorRescuedByConfidence)}), " &
                              $"{extra:+#;-#;0} edges vs the baseline")
            Console.WriteLine($"     >> DPI rescue rate by prior confidence: {RescueRateText(net.Statistics)}")

            Call DebugAudit.Test(net.Statistics.priorEdges > 0, "the curated prior network overlaps the inferred gene space")
            Call DebugAudit.Test(recovered > 0, $"{recovered} prior-supported regulation pairs are recovered from the data")
            Call DebugAudit.Test(recall <= 1.0R, $"the prior recall is computed on the undirected pairs: {recall:P1}")
            Call DebugAudit.Test(net.Edges.Count(Function(e) e.prior) = net.Statistics.priorSupportedEdges,
                "the prior flag of the exported edges is consistent")
            Call DebugAudit.Test(rescued > 0, $"{rescued} prior edges are rescued from the DPI filtering")
            Call DebugAudit.Test(rescued <= recovered, "the rescued edges are a subset of the prior-supported edges")
            Call DebugAudit.Test(net.Edges.Length >= baseline.Edges.Length,
                $"the prior never loses edges: {net.Edges.Length} >= {baseline.Edges.Length}")
            Call DebugAudit.Test(net.Edges.Any(Function(e) e.regulation <> ""), "the activation/inhibition type is taken from the prior")

            ' 先验造成的差异必须是“有据可查”的：新增的边几乎都来自先验或者阈值放宽
            Dim added As AracneEdge() = net.Edges _
                .Where(Function(e) Not baseline.Edges.Any(Function(b) b.regulator = e.regulator AndAlso b.target = e.target)) _
                .ToArray

            Console.WriteLine($"     >> {added.Length} edge(s) are only present with the prior, " &
                              $"{added.Count(Function(e) e.prior)} of them are prior-supported")
        End If

        Console.WriteLine("     >> top regulons:")
        For Each item In net.Edges.RegulonSizes().Take(8)
            Console.WriteLine($"        {item.regulator} ({item.size} targets)")
        Next
        Console.WriteLine("     >> top prior-supported edges:")
        For Each edge As AracneEdge In net.Edges.Where(Function(e) e.prior).TopEdges(8)
            Console.WriteLine($"        {edge.ToString}")
        Next
        Console.WriteLine("     >> top edges:")
        For Each edge As AracneEdge In net.Edges.TopEdges(8)
            Console.WriteLine($"        {edge.ToString}")
        Next

        Dim dir As String = ArgValue(args, "--out", Path.Combine(Path.GetTempPath(), "aracne-hsa-demo"))
        Dim csv As String = Path.Combine(dir, "aracne-edges.csv")

        Call net.WriteEdges(csv)

        Call DebugAudit.Test(System.IO.File.Exists(csv), $"the inferred network is exported into {csv}")

        Dim coef As Matrix = net.CoefMatrix()

        Console.WriteLine($"     >> coefficient matrix: [{coef.size} x {coef.sample_count}], tag = {coef.tag}")
        Call DebugAudit.Test(coef.size = net.MiMatrix.NodeCount, "the MI matrix is exported with the active nodes")
    End Sub

    ''' <summary>
    ''' 用例 A8：curated 先验知识(TF 列表 + TF-target 证据)对 ARACNE 算法的优化
    ''' 
    ''' 覆盖四项基于先验的优化：
    ''' 
    ''' 1. DPI 保护：curated 先验支持的真实边不会被 DPI 误删；
    ''' 2. 阈值放宽：先验边在互信息略低于置换阈值的时候仍然被保留；
    ''' 3. top-K 保护与 TF 补全：先验的调控因子会自动进入 TF 集合；
    ''' 4. 激活/抑制：先验的因果符号被写入输出边。
    ''' </summary>
    Private Sub CasePriorKnowledge()
        Dim synth As SyntheticNetwork = BuildSyntheticNetwork(nSample:=300, seed:=7)
        Dim baseline As AracneNetwork = Infer(synth)

        ' curated 先验：包含一条“会被 DPI 误删”的真实边 TF_A -> CHAIN_Y，
        ' 以及两条带有因果符号的边；最后一条的置信度不足，必须被忽略
        Dim prior As PriorEdge() = {
            New PriorEdge(CHAIN_TF, CHAIN_Y, 0.95R, 0),
            New PriorEdge(CHAIN_TF, "G_1", 0.95R, 1),
            New PriorEdge(CHAIN_TF2, "G_6", 0.95R, -1),
            New PriorEdge(CHAIN_TF, "G_2", 0.5R, 0)
        }
        Dim informed As AracneNetwork = synth.matrix.InferAracneNetwork(New AracneOptions With {
            .silent = True,
            .transcriptionFactors = synth.tfs,
            .permutationPairs = 500,
            .maxNeighbors = 10,
            .randomSeed = 1234567,
            .priorNetwork = prior,
            .priorMinConfidence = 0.9R
        })
        Dim chainEdge As AracneEdge = informed.Edges _
            .FirstOrDefault(Function(e) e.regulator = CHAIN_TF AndAlso e.target = CHAIN_Y)

        Console.WriteLine($"     >> baseline: {baseline.Edges.Length} edges, {baseline.Statistics.dpi.RemovedEdges.Count} removed by DPI")
        Console.WriteLine($"     >> with prior: {informed.Edges.Length} edges, " &
                          $"{informed.Statistics.priorEdges} prior pairs, {informed.Statistics.priorSupportedEdges} supported, " &
                          $"{informed.Statistics.priorProtectedEdges} rescued " &
                          $"(by grade: {GradeText(informed.Statistics.priorRescuedByConfidence)})")
        Console.WriteLine($"     >> rescued edge: {If(chainEdge Is Nothing, "(none)", chainEdge.ToString)}")

        Call DebugAudit.Test(Not baseline.Edges.Any(Function(e) e.regulator = CHAIN_TF AndAlso e.target = CHAIN_Y),
            "without the prior the DPI removes the true (but indirect-looking) edge TF_A -> CHAIN_Y")
        Call DebugAudit.Test(chainEdge IsNot Nothing, "with the prior the edge TF_A -> CHAIN_Y is kept")
        Call DebugAudit.Test(chainEdge IsNot Nothing AndAlso chainEdge.prior, "the kept edge is flagged as prior-supported")
        Call DebugAudit.AreEqual(3, informed.Statistics.priorEdges, "the low-confidence prior edge is ignored")
        Call DebugAudit.Test(informed.Statistics.priorProtectedEdges >= 1,
            $"{informed.Statistics.priorProtectedEdges} edge(s) are rescued from the DPI filtering by the prior")

        Dim rescuedKey As Long = MiMatrix.EdgeKey(
            Array.IndexOf(informed.Genes, CHAIN_TF),
            Array.IndexOf(informed.Genes, CHAIN_Y))

        Call DebugAudit.Test(informed.Statistics.dpi.ProtectedEdges.Any(Function(e) MiMatrix.EdgeKey(e.u, e.v) = rescuedKey),
            "the rescued edge is reported in the DPI statistics")

        ' 因果符号来自先验
        Dim activator As AracneEdge = informed.Edges.FirstOrDefault(Function(e) e.regulator = CHAIN_TF AndAlso e.target = "G_1")
        Dim inhibitor As AracneEdge = informed.Edges.FirstOrDefault(Function(e) e.regulator = CHAIN_TF2 AndAlso e.target = "G_6")

        Console.WriteLine($"     >> {If(activator Is Nothing, "(none)", activator.ToString)}")
        Console.WriteLine($"     >> {If(inhibitor Is Nothing, "(none)", inhibitor.ToString)}")

        Call DebugAudit.Test(activator IsNot Nothing AndAlso activator.regulation = "activator",
            "the curated 'Activator' relation is exported as an activator edge")
        Call DebugAudit.Test(inhibitor IsNot Nothing AndAlso inhibitor.regulation = "inhibitor",
            "the curated 'Inhibitor' relation is exported as an inhibitor edge")

        ' ---- 阈值放宽：把阈值抬到所有边的互信息之上，只有先验边可以幸存 ----
        Dim strict As New AracneOptions With {
            .silent = True,
            .transcriptionFactors = synth.tfs,
            .thresholdMode = MiThresholdMode.Fixed,
            .miThreshold = 0.95R,
            .priorNetwork = {New PriorEdge(CHAIN_TF, "G_3", 0.95R, 0)},
            .priorThresholdFactor = 0.5R,
            .maxNeighbors = 10
        }
        Dim strictPrior As AracneNetwork = synth.matrix.InferAracneNetwork(strict)
        Dim strictPlain As AracneNetwork = synth.matrix.InferAracneNetwork(New AracneOptions With {
            .silent = True,
            .transcriptionFactors = synth.tfs,
            .thresholdMode = MiThresholdMode.Fixed,
            .miThreshold = 0.95R,
            .maxNeighbors = 10
        })
        Dim relaxed As AracneEdge = strictPrior.Edges _
            .FirstOrDefault(Function(e) e.regulator = CHAIN_TF AndAlso e.target = "G_3")

        Console.WriteLine($"     >> strict tau = 0.95: {strictPlain.Edges.Length} edges without prior, " &
                          $"{strictPrior.Edges.Length} edges with prior (TF_A -> G_3 MI = " &
                          $"{If(relaxed Is Nothing, 0, relaxed.mi):F4})")

        Call DebugAudit.Test(Not strictPlain.Edges.Any(Function(e) e.regulator = CHAIN_TF AndAlso e.target = "G_3"),
            "the strict threshold drops the TF_A -> G_3 edge")
        Call DebugAudit.Test(relaxed IsNot Nothing, "the prior relaxes the threshold and keeps TF_A -> G_3")

        ' ---- TF 补全：先验的调控因子会自动进入 TF 集合 ----
        Dim augment As AracneNetwork = synth.matrix.InferAracneNetwork(New AracneOptions With {
            .silent = True,
            .transcriptionFactors = {CHAIN_TF, CHAIN_TF2},
            .priorNetwork = {New PriorEdge("G_1", "G_2", 0.95R, 0)},
            .maxNeighbors = 10
        })
        Dim noAugment As AracneNetwork = synth.matrix.InferAracneNetwork(New AracneOptions With {
            .silent = True,
            .transcriptionFactors = {CHAIN_TF, CHAIN_TF2},
            .priorNetwork = {New PriorEdge("G_1", "G_2", 0.95R, 0)},
            .priorTfAugment = False,
            .maxNeighbors = 10
        })

        Console.WriteLine($"     >> tf augment: {augment.TFs.Length} TFs (G_1 included = {augment.TFs.Contains("G_1")}), " &
                          $"{augment.Edges.Length} edges; without augment: {noAugment.TFs.Length} TFs, {noAugment.Edges.Length} edges")

        Call DebugAudit.Test(Not noAugment.TFs.Contains("G_1"), "the prior regulator is not a TF when the augmentation is disabled")
        Call DebugAudit.Test(augment.TFs.Contains("G_1"), "the prior regulator G_1 is augmented into the TF set")
        Call DebugAudit.Test(augment.Edges.Any(Function(e) e.regulator = "G_1" AndAlso e.target = "G_2"),
            "the prior edge from the augmented regulator is inferred")
    End Sub

    ''' <summary>
    ''' 用例 A9：先验置信度<b>分级</b>的 DPI 保护
    ''' 
    ''' 验证三点：
    ''' 
    ''' 1. 保护强度确实是按置信度缩放的：同样的数据，High 能被保护而 Medium/Low 不能；
    ''' 2. 端到端(从 PriorEdge 的置信度到 DPI 的判定)结果符合分级预期；
    ''' 3. 概率保护模式(<see cref="PriorProtectionMode.Sampled"/>)在固定随机种子之下可复现。
    ''' </summary>
    Private Sub CaseGradedProtection()
        ' ---- 1. DpiFilter 的判定窗口：ratio = 最弱边 / 另外两条边之中较弱的那条 ----
        Dim probes As (weakest As Double, other As Double, high As Boolean, medium As Boolean, low As Boolean)() = {
            (0.44R, 0.82R, True, False, False),
            (0.51R, 0.82R, True, True, False),
            (0.656R, 0.82R, True, True, True)
        }
        For Each probe In probes
            Dim mi As MiMatrix = MakeTriangle(probe.weakest, probe.other)
            Dim key As Long = MiMatrix.EdgeKey(0, 1)
            Dim options As New AracneOptions With {.tolerance = 0.1R, .dpiMode = DpiMode.Standard}
            Dim plain As DpiResult = DpiFilter.Apply(mi, options)
            Dim ratio As Double = probe.weakest / probe.other
            Dim probes2 As (strength As Double, grade As String, keep As Boolean)() = {
                (0.475R, "high", probe.high),
                (0.35R, "medium", probe.medium),
                (0.175R, "low", probe.low)
            }

            Call DebugAudit.AreEqual(2, plain.EdgesAfter, $"without prior the weakest edge is removed (ratio = {ratio:F3})")

            For Each item In probes2
                Dim guard As New Dictionary(Of Long, Double) From {{key, item.strength}}
                Dim guarded As DpiResult = DpiFilter.Apply(mi, options, Nothing, guard)
                Dim kept As Boolean = guarded.EdgesAfter = 3

                Call DebugAudit.Test(kept = item.keep,
                    $"ratio = {ratio:F3}, {item.grade} protection window keeps the edge = {item.keep} (actual {kept})")

                If kept Then
                    Call DebugAudit.AreEqual(1, guarded.ProtectedEdges.Count, $"{item.grade}: one edge is recorded as rescued")
                End If
            Next
        Next

        ' ---- 2. 端到端：同一个先验边，只改变置信度 ----
        Dim synth As SyntheticNetwork = BuildSyntheticNetwork(nSample:=300, seed:=7)
        Dim ratios As New Dictionary(Of Double, Boolean) From {
            {0.95R, True},
            {0.7R, True},
            {0.45R, False}
        }

        For Each item As KeyValuePair(Of Double, Boolean) In ratios
            Dim net As AracneNetwork = synth.matrix.InferAracneNetwork(New AracneOptions With {
                .silent = True,
                .transcriptionFactors = synth.tfs,
                .permutationPairs = 500,
                .maxNeighbors = 10,
                .randomSeed = 1234567,
                .priorNetwork = {New PriorEdge(CHAIN_TF, CHAIN_Y, item.Key, 0)},
                .priorMinConfidence = 0.4R
            })
            Dim kept As Boolean = net.Edges.Any(Function(e) e.regulator = CHAIN_TF AndAlso e.target = CHAIN_Y)

            Console.WriteLine($"     >> confidence {item.Key:F2}: rescued = {kept} " &
                              $"(grade statistics: {GradeText(net.Statistics.priorRescuedByConfidence)})")

            Call DebugAudit.Test(kept = item.Value,
                $"the confidence {item.Key:F2} prior edge is rescued = {item.Value} (actual {kept})")
            Call DebugAudit.AreEqual(PriorProtectionMode.Graded, net.Statistics.priorProtection, "the graded protection mode is reported")
        Next

        ' ---- 3. 阈值放宽同样是分级的：High 的 relax 更强 ----
        Dim relaxHigh As AracneNetwork = InferWithFixedThreshold(synth, New PriorEdge(CHAIN_TF, "G_3", 0.95R, 0))
        Dim relaxLow As AracneNetwork = InferWithFixedThreshold(synth, New PriorEdge(CHAIN_TF, "G_3", 0.45R, 0))
        Dim highKeeps As Boolean = relaxHigh.Edges.Any(Function(e) e.regulator = CHAIN_TF AndAlso e.target = "G_3")
        Dim lowKeeps As Boolean = relaxLow.Edges.Any(Function(e) e.regulator = CHAIN_TF AndAlso e.target = "G_3")

        Console.WriteLine($"     >> graded threshold: high keeps G_3 = {highKeeps}, low keeps G_3 = {lowKeeps}")

        Call DebugAudit.Test(highKeeps, "the high-confidence prior relaxes the threshold enough to keep TF_A -> G_3")
        Call DebugAudit.Test(Not lowKeeps, "the low-confidence prior does not relax the threshold that far")

        ' ---- 4. 概率保护模式：同一随机种子下可复现 ----
        Dim sampledFirst As AracneNetwork = InferSampled(synth, 0.95R, seed:=2026)
        Dim sampledSecond As AracneNetwork = InferSampled(synth, 0.95R, seed:=2026)

        Call DebugAudit.Test(sampledFirst.Statistics.priorProtection = PriorProtectionMode.Sampled,
            "the sampled protection mode is reported")
        Call DebugAudit.Test(
            sampledFirst.Edges.Select(Function(e) e.UID).OrderBy(Function(s) s).SequenceEqual(
                sampledSecond.Edges.Select(Function(e) e.UID).OrderBy(Function(s) s)),
            "the sampled protection is reproducible for a fixed random seed")
    End Sub

    ''' <summary>
    ''' 构造一个三元组：最弱边 (A, B) 的互信息为 <paramref name="weakest"/>，
    ''' 另外两条边都是 <paramref name="other"/>。
    ''' </summary>
    Private Function MakeTriangle(weakest As Double, other As Double) As MiMatrix
        Dim mi As New MiMatrix({"A", "B", "C"})

        Call mi.SetValue(0, 1, weakest)
        Call mi.SetValue(0, 2, other)
        Call mi.SetValue(1, 2, other)

        Return mi
    End Function

    ''' <summary>
    ''' 用固定阈值推断，用于检查“阈值放宽”是否按置信度分级
    ''' </summary>
    Private Function InferWithFixedThreshold(synth As SyntheticNetwork, prior As PriorEdge) As AracneNetwork
        Return synth.matrix.InferAracneNetwork(New AracneOptions With {
            .silent = True,
            .transcriptionFactors = synth.tfs,
            .thresholdMode = MiThresholdMode.Fixed,
            .miThreshold = 0.95R,
            .priorNetwork = {prior},
            .priorMinConfidence = 0.4R,
            .maxNeighbors = 10
        })
    End Function

    ''' <summary>
    ''' 用概率保护模式推断
    ''' </summary>
    Private Function InferSampled(synth As SyntheticNetwork, confidence As Double, seed As Integer) As AracneNetwork
        Return synth.matrix.InferAracneNetwork(New AracneOptions With {
            .silent = True,
            .transcriptionFactors = synth.tfs,
            .permutationPairs = 500,
            .maxNeighbors = 10,
            .randomSeed = seed,
            .priorProtectionMode = PriorProtectionMode.Sampled,
            .priorNetwork = {New PriorEdge(CHAIN_TF, CHAIN_Y, confidence, 0)},
            .priorMinConfidence = 0.4R
        })
    End Function

    ''' <summary>
    ''' 把分级统计渲染成一行文本
    ''' </summary>
    Private Function GradeText(counts As Dictionary(Of String, Integer)) As String
        If counts Is Nothing OrElse counts.Count = 0 Then
            Return "(none)"
        End If

        Return String.Join(", ", counts.Select(Function(kv) $"{kv.Key}={kv.Value}"))
    End Function

    ''' <summary>
    ''' 渲染“每一档置信度的先验边之中有多少比例被 DPI 保护下来”
    ''' </summary>
    Private Function RescueRateText(stat As AracneStatistics) As String
        If stat Is Nothing OrElse stat.priorPairsByConfidence Is Nothing Then
            Return "(none)"
        End If

        Dim parts As New List(Of String)

        For Each grade As String In {"high", "medium", "low"}
            Dim total As Integer
            Dim rescued As Integer

            If Not stat.priorPairsByConfidence.TryGetValue(grade, total) Then
                total = 0
            End If
            If Not stat.priorRescuedByConfidence.TryGetValue(grade, rescued) Then
                rescued = 0
            End If

            If total > 0 Then
                Call parts.Add($"{grade} {rescued}/{total} ({rescued / CDbl(total):P1})")
            End If
        Next

        Return If(parts.Count = 0, "(none)", String.Join(", ", parts))
    End Function

#Region "synthetic data"

    Private Const CHAIN_TF As String = "TF_A"
    Private Const CHAIN_TF2 As String = "TF_B"
    Private Const CHAIN_MED As String = "CHAIN_MED"
    Private Const CHAIN_Y As String = "CHAIN_Y"
    Private Const RND_1 As String = "RND_1"
    Private Const RND_2 As String = "RND_2"
    Private Const RND_3 As String = "RND_3"
    Private Const TF_PAIR_A As String = "TF_PAIR_A"
    Private Const TF_PAIR_B As String = "TF_PAIR_B"
    Private Const TF_PAIR_NOISE As String = "TF_PAIR_NOISE"

    ''' <summary>
    ''' 构造一个合成的 TF-靶基因网络：
    ''' 
    ''' ```
    ''' TF_A -> CHAIN_MED -> CHAIN_Y       (链式关系，TF_A -> CHAIN_Y 是间接边)
    ''' TF_A -> G_1 .. G_5
    ''' TF_B -> G_6 .. G_10
    ''' RND_1 .. RND_3                     (纯噪声基因)
    ''' ```
    ''' 
    ''' 注意 CHAIN_MED 也被登记为转录因子：在 TF 引导的计算空间之中，只有
    ''' 涉及 TF 的基因对才会被计算互信息，因此链式关系必须落在 TF 之上
    ''' (``TF_A -> CHAIN_MED -> CHAIN_Y``)，DPI 才有可能观察到
    ''' ``CHAIN_MED - CHAIN_Y`` 这条边并把 ``TF_A - CHAIN_Y`` 判定为间接边。
    ''' </summary>
    ''' <param name="nSample">样本数量</param>
    ''' <param name="seed">随机种子</param>
    ''' <returns></returns>
    Private Function BuildSyntheticNetwork(nSample As Integer, seed As Integer) As SyntheticNetwork
        Dim rng As New Random(seed)
        Dim tfA As Double() = Sample(rng, nSample, 1.0R)
        Dim tfB As Double() = Sample(rng, nSample, 1.0R)
        Dim names As New List(Of String) From {CHAIN_TF, CHAIN_TF2}
        Dim data As New List(Of Double()) From {tfA, tfB}
        Dim truth As New List(Of (regulator As String, target As String))

        ' 链式关系：TF_A -> CHAIN_MED -> CHAIN_Y
        Dim med As Double() = Sample(rng, nSample, 0.6R)

        For i As Integer = 0 To nSample - 1
            med(i) += tfA(i)
        Next

        Dim y As Double() = Sample(rng, nSample, 0.6R)

        For i As Integer = 0 To nSample - 1
            y(i) += med(i)
        Next

        Call names.Add(CHAIN_MED)
        Call data.Add(med)
        Call truth.Add((CHAIN_TF, CHAIN_MED))

        Call names.Add(CHAIN_Y)
        Call data.Add(y)
        Call truth.Add((CHAIN_MED, CHAIN_Y))

        ' 靶基因：前半部分由 TF_A 驱动，后半部分由 TF_B 驱动
        For k As Integer = 1 To 10
            Dim regulator As String = If(k <= 5, CHAIN_TF, CHAIN_TF2)
            Dim driver As Double() = If(k <= 5, tfA, tfB)
            Dim values As Double() = Sample(rng, nSample, 0.8R)

            For i As Integer = 0 To nSample - 1
                values(i) += driver(i)
            Next

            Call names.Add($"G_{k}")
            Call data.Add(values)
            Call truth.Add((regulator, $"G_{k}"))
        Next

        ' 纯噪声基因
        For k As Integer = 1 To 3
            Call names.Add($"RND_{k}")
            Call data.Add(Sample(rng, nSample, 1.0R))
        Next

        Return New SyntheticNetwork With {
            .matrix = MakeMatrix(names.ToArray, data.ToArray),
            .tfs = {CHAIN_TF, CHAIN_TF2, CHAIN_MED},
            .truth = truth.ToArray
        }
    End Function

    ''' <summary>
    ''' 构造一个两个转录因子高度相关的矩阵，用于检查 TF-TF 之间的双向边
    ''' </summary>
    ''' <param name="nSample"></param>
    ''' <param name="seed"></param>
    ''' <returns></returns>
    Private Function BuildTfPairMatrix(nSample As Integer, seed As Integer) As Matrix
        Dim rng As New Random(seed)
        Dim a As Double() = Sample(rng, nSample, 1.0R)
        Dim b As Double() = Sample(rng, nSample, 0.5R)

        For i As Integer = 0 To nSample - 1
            b(i) += a(i)
        Next

        Return MakeMatrix(
            {TF_PAIR_A, TF_PAIR_B, TF_PAIR_NOISE},
            {a, b, Sample(rng, nSample, 1.0R)}
        )
    End Function

    ''' <summary>
    ''' 生成一个给定噪声水平的正态随机向量
    ''' </summary>
    ''' <param name="rng"></param>
    ''' <param name="n"></param>
    ''' <param name="sd"></param>
    ''' <returns></returns>
    Private Function Sample(rng As Random, n As Integer, sd As Double) As Double()
        Dim values As Double() = New Double(n - 1) {}

        For i As Integer = 0 To n - 1
            values(i) = sd * Gauss(rng)
        Next

        Return values
    End Function

    ''' <summary>
    ''' 标准正态随机数(Box-Muller 变换)
    ''' </summary>
    ''' <param name="rng"></param>
    ''' <returns></returns>
    Private Function Gauss(rng As Random) As Double
        Dim u1 As Double = 1.0R - rng.NextDouble()
        Dim u2 As Double = rng.NextDouble()

        Return std.Sqrt(-2.0R * std.Log(u1)) * std.Cos(2.0R * std.PI * u2)
    End Function

    ''' <summary>
    ''' 用给定的基因编号与表达向量构造一个表达矩阵
    ''' </summary>
    ''' <param name="names"></param>
    ''' <param name="data"></param>
    ''' <returns></returns>
    Private Function MakeMatrix(names As String(), data As Double()()) As Matrix
        Dim nSample As Integer = data(0).Length
        Dim samples As String() = Enumerable.Range(1, nSample).Select(Function(i) $"S{i}").ToArray
        Dim rows As DataFrameRow() = names _
            .Select(Function(name, i)
                        Return New DataFrameRow With {
                            .geneID = name,
                            .experiments = data(i)
                        }
                    End Function) _
            .ToArray

        Return New Matrix With {
            .tag = "aracne-synthetic",
            .sampleID = samples,
            .expression = rows
        }
    End Function

    ''' <summary>
    ''' 使用默认的 demo 参数执行一次 ARACNE 推断
    ''' </summary>
    ''' <param name="synth"></param>
    ''' <returns></returns>
    Private Function Infer(synth As SyntheticNetwork) As AracneNetwork
        Return synth.matrix.InferAracneNetwork(New AracneOptions With {
            .silent = True,
            .transcriptionFactors = synth.tfs,
            .permutationPairs = 500,
            .maxNeighbors = 10,
            .randomSeed = 1234567
        })
    End Function

#End Region

#Region "helpers"

    ''' <summary>
    ''' 执行一个用例，并且记录它的耗时
    ''' </summary>
    ''' <param name="title"></param>
    ''' <param name="body"></param>
    Private Sub RunCase(title As String, body As Action)
        Console.WriteLine($"--- {title}")

        Dim before As Integer = DebugAudit.FailureCount

        Using scope As DebugAuditScope = DebugAudit.Watch(title, logOnExit:=False)
            Try
                Call body()
            Catch ex As Exception
                Console.WriteLine("  [FAIL] unhandled exception: " & ex.ToString)

                Call DebugAudit.Test(False, $"unhandled exception in '{title}': {ex.Message}")
            End Try
        End Using

        Dim failed As Integer = DebugAudit.FailureCount - before
        Dim status As String = If(failed = 0, "PASS", $"FAIL({failed} assertion(s))")

        ' 把每一个用例的结果写入审计报告的变量快照，便于事后追溯
        Call DebugAudit.Mark(title, status)
        Console.WriteLine($"  [{status}] {title}")
        Console.WriteLine()
    End Sub

    ''' <summary>
    ''' 读取命令行参数的值(形如 ``--matrix path``)
    ''' </summary>
    Private Function ArgValue(args As String(), name As String, Optional fallback As String = Nothing) As String
        If args Is Nothing Then
            Return fallback
        End If

        For i As Integer = 0 To args.Length - 1
            If String.Equals(args(i), name, StringComparison.OrdinalIgnoreCase) Then
                If i + 1 < args.Length Then
                    Return args(i + 1)
                End If
            End If
        Next

        Return fallback
    End Function

    ''' <summary>
    ''' 读取整数类型的命令行参数
    ''' </summary>
    Private Function ArgInt(args As String(), name As String, fallback As Integer) As Integer
        Dim text As String = ArgValue(args, name)
        Dim value As Integer

        If text IsNot Nothing AndAlso Integer.TryParse(text, value) Then
            Return value
        Else
            Return fallback
        End If
    End Function

    ''' <summary>
    ''' 读取浮点类型的命令行参数
    ''' </summary>
    Private Function ArgDouble(args As String(), name As String, fallback As Double) As Double
        Dim text As String = ArgValue(args, name)
        Dim value As Double

        If text IsNot Nothing AndAlso Double.TryParse(text, Globalization.NumberStyles.Float, Globalization.CultureInfo.InvariantCulture, value) Then
            Return value
        Else
            Return fallback
        End If
    End Function

#End Region

End Module
