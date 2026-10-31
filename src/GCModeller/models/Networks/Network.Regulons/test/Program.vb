Imports System.IO
Imports System.IO.Compression
Imports System.Text
Imports DebugAudit = Microsoft.VisualBasic.ApplicationServices.Debugging.DebugAudit
Imports Microsoft.VisualBasic.Data.Bootstrapping.ElasticNet
Imports Microsoft.VisualBasic.Data.Bootstrapping.LASSO
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.Model.Network.Regulons
Imports SMRUCC.genomics.Model.Network.Regulons.LinearRegression
Imports std = System.Math

''' <summary>
''' GeneRegulatoryNetwork 线性拟合模块的 demo 与自检程序
''' 
''' ```
''' dotnet run --project test.vbproj
''' ```
''' </summary>
Module Program

    Private failures As Integer = 0

    Sub Main(args As String())
        ' ARACNE 模块的自检与 demo 有一套独立的入口，通过 --aracne 开关触发
        If args IsNot Nothing AndAlso args.Any(Function(a) String.Equals(a, "--aracne", StringComparison.OrdinalIgnoreCase)) Then
            Call AracneDemo.RunAracneDemo(args)

            Environment.ExitCode = If(DebugAudit.FailureCount = 0, 0, 1)

            Return
        End If

        Console.WriteLine("== GeneRegulatoryNetwork linear model fitting demo ==")
        Console.WriteLine()

        Dim matrixFile As String = ArgValue(args, "--matrix")

        If Not String.IsNullOrEmpty(matrixFile) Then
            ' 真实表达矩阵之上的分析(耗时较长)，通过命令行显式触发
            Run($"case 12: elastic net network on '{System.IO.Path.GetFileName(matrixFile)}'",
                Sub() CaseRealMatrix(matrixFile, args))
        Else
            Run("case 1: recover the coefficients of a hand-made linear model", AddressOf CaseExactRecovery)
            Run("case 2: rebuild a synthetic TF-target network by OLS", AddressOf CaseOlsNetwork)
            Run("case 3: rebuild the same network by the sparse LASSO model", AddressOf CaseLassoNetwork)
            Run("case 4: restrict the regulator pool to the transcription factors", AddressOf CaseRegulatorPool)
            Run("case 5: export the coefficient matrix and the edge table", AddressOf CaseExport)
            Run("case 6: input validation", AddressOf CaseValidation)
            Run("case 7: lasso solver regression tests", AddressOf CaseLassoSolver)
            Run("case 8: fit the network when there are more genes than samples", AddressOf CaseWideMatrix)
            Run("case 9: the grouping effect on collinear regulators", AddressOf CaseCollinearRegulators)
            Run("case 10: compare OLS / LASSO / elastic net on the same network", AddressOf CaseCompareMethods)
            Run("case 11: elastic net path consistency and export", AddressOf CaseElasticNetPath)
            Run("case 13: save the fitted network into a zip archive and load it back", AddressOf CasePersistence)
        End If

        Console.WriteLine()
        Console.WriteLine(If(failures = 0, "PASS", "FAIL") & $": {failures} failure(s)")

        Environment.ExitCode = If(failures = 0, 0, 1)
    End Sub

    ''' <summary>
    ''' 用例 1：手工构造一个没有噪声的线性模型，检查回归系数是否可以被精确还原
    ''' 
    ''' ```
    ''' gene3 = 2.0 + 1.5 * gene1 - 0.8 * gene2
    ''' ```
    ''' </summary>
    Private Sub CaseExactRecovery()
        Dim rng As New Random(11)
        Dim nS As Integer = 30
        Dim gene1 As Double() = New Double(nS - 1) {}
        Dim gene2 As Double() = New Double(nS - 1) {}
        Dim gene3 As Double() = New Double(nS - 1) {}
        Dim samples As String() = Enumerable.Range(1, nS).Select(Function(i) $"S{i}").ToArray

        For s As Integer = 0 To nS - 1
            gene1(s) = Gauss(rng)
            gene2(s) = Gauss(rng)
            gene3(s) = 2.0 + 1.5 * gene1(s) - 0.8 * gene2(s)
        Next

        Dim expr As Matrix = MakeMatrix(
            names:={"gene1", "gene2", "gene3"},
            data:={gene1, gene2, gene3},
            samples:=samples
        )
        Dim options As New GrnFittingOptions With {
            .silent = True,
            .computeStatistics = True
        }
        Dim grn As GeneRegulatoryNetwork = expr.InferRegulatoryNetwork(options)
        Dim model As GeneRegressionModel = grn.GetModel("gene3")

        Check(model IsNot Nothing, "the linear model of gene3 is fitted")
        Check(model.r2 > 0.9999, $"R2 = {model.r2.ToString("F6")} ~ 1")
        Check(std.Abs(model.intercept - 2.0) < 0.001, $"const = {model.intercept.ToString("F6")} ~ 2.0")
        Check(std.Abs(model.GetWeight("gene1") - 1.5) < 0.001, $"beta(gene1) = {model.GetWeight("gene1").ToString("F6")} ~ 1.5")
        Check(std.Abs(model.GetWeight("gene2") + 0.8) < 0.001, $"beta(gene2) = {model.GetWeight("gene2").ToString("F6")} ~ -0.8")
        Check(model.GetPvalue("gene1") < 0.001, $"pvalue(gene1) = {model.GetPvalue("gene1").ToString("G3")} < 0.001")
        Check(model.GetPvalue("gene2") < 0.001, $"pvalue(gene2) = {model.GetPvalue("gene2").ToString("G3")} < 0.001")

        Console.WriteLine($"     >> {model.ToString}")

        ' 标准化之后回归系数可以被换算到同一个量纲之上进行比较，但是符号不会改变
        Dim scaled As GeneRegulatoryNetwork = expr.InferRegulatoryNetwork(
            New GrnFittingOptions With {
                .silent = True,
                .standardize = True
            }
        )
        Dim zmodel As GeneRegressionModel = scaled.GetModel("gene3")

        Check(zmodel.nonZeroWeights = 2, "the standardized model also keeps the two regulators")
        Check(std.Sign(zmodel.GetWeight("gene1")) <> std.Sign(zmodel.GetWeight("gene2")),
            "the standardized model gives the same regulation directions")
        Check(std.Abs(zmodel.r2 - model.r2) < 0.001, $"R2 is not affected by the standardization: {zmodel.r2.ToString("F6")}")
    End Sub

    ''' <summary>
    ''' 用例 2：从一个合成的 TF-target 网络之中，(OLS)重建调控关系
    ''' </summary>
    Private Sub CaseOlsNetwork()
        Dim synth As (matrix As Matrix, truth As TruthEdge()) = BuildNetwork(
            nTf:=4,
            nTarget:=16,
            nSample:=60,
            noise:=0.15,
            seed:=7
        )
        Dim options As New GrnFittingOptions With {
            .silent = True,
            .maxCandidates = 10
        }
        Dim watch As Diagnostics.Stopwatch = Diagnostics.Stopwatch.StartNew()
        Dim grn As GeneRegulatoryNetwork = synth.matrix.InferRegulatoryNetwork(options)
        Dim seconds As Double = watch.Elapsed.TotalSeconds
        Dim edges As RegulationEdge() = grn.Regulations().ToArray
        Dim score = Evaluate(synth.truth, edges)
        Dim sign As Double = SignAccuracy(synth.truth, edges)

        Dim significant As RegulationEdge() = grn.Regulations(pvalueCutoff:=0.01).ToArray

        Console.WriteLine($"     >> {synth.matrix.size} genes x {synth.matrix.sample_count} samples, " &
                          $"{synth.truth.Length} true edges, {edges.Length} predicted edges, cost {seconds.ToString("F2")}s")

        Check(significant.Length < edges.Length AndAlso significant.Length > 0,
            $"{significant.Length} of {edges.Length} edges are significant at pvalue < 0.01")

        Check(edges.Length > 0, $"{edges.Length} regulation edges inferred")
        Check(score.recall > 0.7, $"top-{synth.truth.Length} recall = {score.recall.ToString("F3")}")
        Check(score.precision > 0.5, $"top-{synth.truth.Length} precision = {score.precision.ToString("F3")}")
        Check(sign > 0.9, $"sign accuracy = {sign.ToString("F3")}")

        Dim sizes = edges.RegulonSizes().ToArray

        Console.WriteLine("     >> top regulons: " & String.Join(", ", sizes.Take(4).Select(Function(r) $"{r.regulator}({r.size})")))

        ' 转录因子应当拥有明显多于随机基因的靶基因数量
        Dim tfSize As Integer = sizes _
            .Where(Function(r) r.regulator.StartsWith("TF_")) _
            .Sum(Function(r) r.size)

        Check(tfSize > 0, $"the transcription factors regulate {tfSize} target genes")

        Dim top As RegulationEdge() = edges.TopEdges(6)

        For Each edge As RegulationEdge In top
            Console.WriteLine($"        {edge.ToString}")
        Next
    End Sub

    ''' <summary>
    ''' 用例 3：使用 LASSO 稀疏重建同一个网络
    ''' </summary>
    Private Sub CaseLassoNetwork()
        Dim synth As (matrix As Matrix, truth As TruthEdge()) = BuildNetwork(
            nTf:=4,
            nTarget:=16,
            nSample:=60,
            noise:=0.15,
            seed:=7
        )

        Dim options As New GrnFittingOptions With {
            .silent = True,
            .method = FittingMethod.Lasso,
            .maxCandidates = 18,
            .maxRegulatorsPerGene = 8
        }
        Dim grn As GeneRegulatoryNetwork = synth.matrix.InferRegulatoryNetwork(options)
        Dim edges As RegulationEdge() = grn.Regulations().ToArray
        Dim score = Evaluate(synth.truth, edges)
        Dim maxNonZero As Integer = grn.Models.Max(Function(m) m.nonZeroWeights)

        Console.WriteLine($"     >> {edges.Length} edges (each gene keeps at most {maxNonZero} regulators), " &
                          $"recall = {score.recall.ToString("F3")}, precision = {score.precision.ToString("F3")}")

        Check(edges.Length > 0, $"{edges.Length} regulation edges inferred by lasso")
        Check(maxNonZero <= options.maxRegulatorsPerGene, $"each gene keeps <= {options.maxRegulatorsPerGene} regulators")
        Check(score.recall > 0.6, $"top-{synth.truth.Length} recall = {score.recall.ToString("F3")}")
        Check(edges.All(Function(e) e.pvalue = 1), "the lasso model reports no pvalue")

        Dim model As GeneRegressionModel = grn.GetModel("G_1")

        Check(model IsNot Nothing AndAlso model.lambda > 0, $"the lasso step of G_1 is picked at lambda = {model.lambda.ToString("G4")}")
    End Sub

    ''' <summary>
    ''' 用例 4：只允许转录因子作为调控因子参与拟合
    ''' </summary>
    Private Sub CaseRegulatorPool()
        Dim synth As (matrix As Matrix, truth As TruthEdge()) = BuildNetwork(
            nTf:=4,
            nTarget:=16,
            nSample:=60,
            noise:=0.15,
            seed:=7
        )
        Dim tfs As String() = synth.matrix.rownames _
            .Where(Function(id) id.StartsWith("TF_")) _
            .ToArray
        Dim options As New GrnFittingOptions With {
            .silent = True,
            .regulators = tfs,
            .maxCandidates = 4,
            .minAbsCorrelation = 0.2
        }
        Dim grn As GeneRegulatoryNetwork = synth.matrix.InferRegulatoryNetwork(options)
        Dim edges As RegulationEdge() = grn.Regulations().ToArray

        Check(edges.Length > 0, $"{edges.Length} regulation edges inferred from the TF pool only")
        Check(edges.All(Function(e) e.regulator.StartsWith("TF_")), "all of the regulators come from the TF pool")

        Dim score = Evaluate(synth.truth, edges)

        Console.WriteLine($"     >> restricted pool: recall = {score.recall.ToString("F3")}, precision = {score.precision.ToString("F3")}")

        Check(score.precision > 0.5, $"top-{synth.truth.Length} precision = {score.precision.ToString("F3")}")
    End Sub

    ''' <summary>
    ''' 用例 5：系数矩阵与边表导出的冒烟测试
    ''' </summary>
    Private Sub CaseExport()
        Dim synth As (matrix As Matrix, truth As TruthEdge()) = BuildNetwork(
            nTf:=3, nTarget:=9, nSample:=40, noise:=0.1, seed:=13
        )
        Dim grn As GeneRegulatoryNetwork = synth.matrix.InferRegulatoryNetwork(
            New GrnFittingOptions With {.silent = True, .maxCandidates = 8}
        )
        Dim dir As String = Path.Combine(Path.GetTempPath(), "grn-demo")
        Dim csv As String = Path.Combine(dir, "regulation-edges.csv")

        Call grn.WriteEdges(csv)

        Dim lines As String() = File.ReadAllLines(csv)
        Dim edges As RegulationEdge() = grn.Regulations().ToArray
        Dim coef As Matrix = grn.CoefMatrix()

        Check(File.Exists(csv), $"edge table saved to {csv}")
        Check(lines.Length = edges.Length + 1, $"{lines.Length - 1} rows written into the edge table")
        Check(lines(0) = "regulator,target,weight,abs_weight,intercept,r2,pvalue,alpha,l1_ratio,interaction", "the csv header is correct")

        Check(coef.expression.Length = grn.Size, $"coefficient matrix rows = {coef.expression.Length}")
        Check(coef.sampleID.Length = grn.Size, $"coefficient matrix columns = {coef.sampleID.Length}")

        ' 互为因果的调控关系会明显少于全部的调控关系
        Dim mutual As New GeneRegulatoryNetwork(
            synth.matrix,
            New GrnFittingOptions With {
                .silent = True,
                .maxCandidates = 8,
                .mutualOnly = True
            }
        )
        Dim pairs As RegulationEdge() = mutual.Fit().Regulations().ToArray

        Check(pairs.Length <= edges.Length, $"the mutual-only filter keeps {pairs.Length} of {edges.Length} edges")
        Check(pairs.All(Function(e) e.interaction = "mutual"), "the mutual edges are tagged with 'mutual'")

        Dim connections As Connection() = edges.ToConnections().ToArray

        Check(connections.Length = edges.Length, "the regulation edges could be converted into network connections")
    End Sub

    ''' <summary>
    ''' 用例 6：输入数据合法性检查
    ''' </summary>
    Private Sub CaseValidation()
        Dim rng As New Random(3)
        Dim vector As Double() = Enumerable.Range(0, 10).Select(Function(i) Gauss(rng)).ToArray

        Check(Throws(Sub()
                         Dim expr As Matrix = MakeMatrix({"a", "a"}, {vector, vector}, 10)
                         Dim probe As GeneRegulatoryNetwork = New GeneRegulatoryNetwork(expr)

                         Check2(probe)
                     End Sub),
            "duplicated gene id is rejected")

        Check(Throws(Sub()
                         Dim expr As Matrix = MakeMatrix({"a"}, {vector}, 10)
                         Dim probe As GeneRegulatoryNetwork = New GeneRegulatoryNetwork(expr)

                         Check2(probe)
                     End Sub),
            "a single gene matrix is rejected")

        Check(Throws(Sub()
                         Dim expr As Matrix = MakeMatrix({"a", "b"}, {vector, vector}, 10)
                         Dim probe As GeneRegulatoryNetwork = New GeneRegulatoryNetwork(
                             expr,
                             New GrnFittingOptions With {.regulators = {"unknown_gene"}}
                         )

                         Check2(probe)
                     End Sub),
            "an empty regulator pool is rejected")

        Check(Throws(Sub()
                         Dim expr As Matrix = MakeMatrix({"a", "b"}, {vector, vector}, 3)
                         Dim probe As GeneRegulatoryNetwork = New GeneRegulatoryNetwork(expr)

                         Check2(probe)
                     End Sub),
            "too few samples is rejected")
    End Sub

    ''' <summary>
    ''' 用例 7：LASSO 求解器的回归测试
    ''' 
    ''' 覆盖两个已经被修复的缺陷：
    ''' 
    ''' 1. ``LassoFitGenerator.init`` 按照观测数量分配特征数组，自变量个数大于样本数量的时候会越界；
    ''' 2. ``maxAllowedFeaturesPerModel`` 受限的时候，``denseActiveSet`` 与协方差行向量的长度不匹配，
    '''    内部点积会抛出 ``the length of the two vectors not agree``。
    ''' </summary>
    Private Sub CaseLassoSolver()
        ' ---------------------------------------------------------------
        ' 缺陷 1：特征数量 > 样本数量
        ' ---------------------------------------------------------------
        Dim rng As New Random(23)
        Dim nFeature As Integer = 19
        Dim nSample As Integer = 10
        Dim wide As Double()() = RectangularArray(nSample, nFeature, rng)
        Dim wideY As Double() = Enumerable.Range(0, nSample).Select(Function(i) Gauss(rng)).ToArray
        Dim widePath As LassoFit = Nothing
        Dim wideThrow As Boolean = Throws(Sub()
                                              widePath = LassoPath(wide, wideY, maxFeatures:=-1)
                                          End Sub)

        Check(Not wideThrow, $"the lasso solver accepts {nFeature} features with only {nSample} samples")
        Check(widePath IsNot Nothing AndAlso widePath.numberOfLambdas > 0,
            $"the lasso path contains {If(widePath Is Nothing, 0, widePath.numberOfLambdas)} lambda steps")
        Check(widePath IsNot Nothing AndAlso widePath.numFeatures = nFeature,
            $"the lasso solver keeps all of the {nFeature} features")

        ' ---------------------------------------------------------------
        ' 缺陷 2：maxAllowedFeaturesPerModel 受限
        ' ---------------------------------------------------------------
        Dim synth As (matrix As Matrix, truth As TruthEdge()) = BuildNetwork(
            nTf:=4, nTarget:=16, nSample:=60, noise:=0.15, seed:=7
        )
        Dim target As String = "G_1"
        Dim features As String() = synth.matrix.rownames.Where(Function(id) id <> target).ToArray
        Dim X As Double()() = New Double(synth.matrix.sample_count - 1)() {}

        For s As Integer = 0 To X.Length - 1
            Dim sample As Integer = s

            X(s) = features.Select(Function(id) synth.matrix(id).experiments(sample)).ToArray
        Next

        Dim y As Double() = synth.matrix(target).experiments
        Dim fullPath As LassoFit = LassoPath(X, y, maxFeatures:=-1)
        Dim limitedPath As LassoFit = Nothing
        Dim limitedThrow As Boolean = Throws(Sub()
                                                 limitedPath = LassoPath(X, y, maxFeatures:=4)
                                             End Sub)

        Check(Not limitedThrow, "the lasso solver accepts a limited maxAllowedFeaturesPerModel")
        Check(limitedPath IsNot Nothing AndAlso limitedPath.numberOfLambdas > 0,
            $"the limited lasso path contains {If(limitedPath Is Nothing, 0, limitedPath.numberOfLambdas)} lambda steps")

        If limitedPath Is Nothing Then
            Return
        End If

        Dim capacity As Integer = CInt(4 * 1.2)
        Dim maxNonZero As Integer = limitedPath _
            .nonZeroWeights _
            .Take(limitedPath.numberOfLambdas) _
            .Max()

        Console.WriteLine($"     >> full path: {fullPath.numberOfLambdas} steps, " &
                          $"max non-zero = {fullPath.nonZeroWeights.Take(fullPath.numberOfLambdas).Max()}, " &
                          $"best R2 = {fullPath.rsquared.Take(fullPath.numberOfLambdas).Max().ToString("F4")}")
        Console.WriteLine($"     >> limited path: {limitedPath.numberOfLambdas} steps (capacity {capacity}), " &
                          $"max non-zero = {maxNonZero}, " &
                          $"best R2 = {limitedPath.rsquared.Take(limitedPath.numberOfLambdas).Max().ToString("F4")}")

        Check(maxNonZero <= capacity, $"the limited path never exceeds {capacity} non-zero weights")
        Check(limitedPath.numberOfLambdas <= fullPath.numberOfLambdas,
            "the limited path is not longer than the full path")

        ' 受限路径的最后一步应当给出不超过 4 个有效的调控因子，并且解释度接近完整路径
        Dim lastStep As Integer = limitedPath.numberOfLambdas - 1
        Dim weights As Double() = limitedPath.getWeights(lastStep)
        Dim nonZero As Integer = weights.Count(Function(w) w <> 0.0R)
        Dim fullBest As Double = fullPath _
            .rsquared _
            .Take(fullPath.numberOfLambdas) _
            .Max()

        Check(nonZero > 0 AndAlso nonZero <= 4, $"{nonZero} non-zero weights are reported at the last step")
        Check(limitedPath.rsquared(lastStep) > 0.5,
            $"the limited path still explains the target gene: R2 = {limitedPath.rsquared(lastStep).ToString("F4")}")
        Check(limitedPath.rsquared(lastStep) <= fullBest + 0.0001,
            "the limited path could not explain more than the full path")

        ' 修复之后不能改变原有(不受限)行为：完整路径的非零权重数量应当达到全部特征
        Check(fullPath.nonZeroWeights.Take(fullPath.numberOfLambdas).Max() >= 5,
            $"the full path still grows to many non-zero weights: {fullPath.nonZeroWeights.Take(fullPath.numberOfLambdas).Max()}")
    End Sub

    ''' <summary>
    ''' 用例 8：基因数量远大于样本数量(p > n)的场景
    ''' 
    ''' 普通最小二乘与 LASSO 求解器都要求自变量个数小于样本数量，因此候选调控因子
    ''' 会被强行截断；弹性网络没有这个限制，可以把全部基因都作为候选。
    ''' </summary>
    Private Sub CaseWideMatrix()
        Dim synth As (matrix As Matrix, truth As TruthEdge()) = BuildNetwork(
            nTf:=10, nTarget:=50, nSample:=20, noise:=0.2, seed:=31
        )
        Dim nS As Integer = synth.matrix.sample_count

        Console.WriteLine($"     >> {synth.matrix.size} genes x {nS} samples, {synth.truth.Length} true edges")

        Dim ols As GeneRegulatoryNetwork = synth.matrix.InferRegulatoryNetwork(
            New GrnFittingOptions With {
                .silent = True,
                .maxCandidates = 18
            }
        )
        Dim wide As GeneRegulatoryNetwork = synth.matrix.InferRegulatoryNetwork(
            New GrnFittingOptions With {
                .silent = True,
                .method = FittingMethod.ElasticNet,
                .maxRegulatorsPerGene = 10,
                .elasticNet = New ElasticNetOptions With {
                    .l1Ratio = 0.8,
                    .numberOfAlphas = 40
                }
            }
        )
        Dim olsMax As Integer = ols.Models.Max(Function(m) If(m.regulators Is Nothing, 0, m.regulators.Length))
        Dim enMax As Integer = wide.Models.Max(Function(m) If(m.regulators Is Nothing, 0, m.regulators.Length))

        Check(olsMax <= nS - 2, $"the ols candidates are truncated to {olsMax} (sample size - 2)")
        Check(enMax > nS - 2, $"the elastic net keeps all of the {enMax} candidate regulators")

        Dim edges As RegulationEdge() = wide.Regulations().ToArray
        Dim score = Evaluate(synth.truth, edges)

        Console.WriteLine($"     >> elastic net: {edges.Length} edges, " &
                          $"recall = {score.recall:F3}, precision = {score.precision:F3}, f1 = {score.f1:F3}")

        Check(edges.Length > 0, $"{edges.Length} regulation edges inferred under the p > n condition")
        Check(wide.Models.Where(Function(m) Not m.IsValid).Count() = 0, "all of the gene models are fitted by the elastic net")
        Check(score.f1 > 0.3, $"top-{synth.truth.Length} f1 = {score.f1:F3}")
        Check(edges.All(Function(e) e.alpha > 0), "each edge reports the alpha of its elastic net model")

        Dim maxRegulators As Integer = wide.Models.Max(Function(m) m.nonZeroWeights)

        Check(maxRegulators <= 10, $"each gene keeps at most 10 regulators: {maxRegulators}")
    End Sub

    ''' <summary>
    ''' 用例 9：高度共线基因的分组效应
    ''' 
    ''' 当两个调控因子几乎完全共线的时候，弹性网络的 L2 惩罚项会让它们的系数
    ''' 平均分配(``beta_1 = beta_2 = beta / 2``)，而纯 L1 的 LASSO 只会保留其中一个。
    ''' </summary>
    Private Sub CaseCollinearRegulators()
        Dim synth As (matrix As Matrix, truth As TruthEdge()) = BuildCollinearNetwork(
            nSample:=40, noise:=0.05, collinearNoise:=0.0, seed:=91
        )
        Dim options As New GrnFittingOptions With {
            .silent = True,
            .method = FittingMethod.ElasticNet,
            .maxRegulatorsPerGene = 5,
            .elasticNet = New ElasticNetOptions With {
                .l1Ratio = 0.7,
                .numberOfAlphas = 100,
                .tolerance = 0.0000000001
            }
        }
        Dim enGrn As GeneRegulatoryNetwork = synth.matrix.InferRegulatoryNetwork(options)
        Dim model As GeneRegressionModel = enGrn.GetModel("G_1")
        Dim betaA As Double = model.GetWeight("TF_A")
        Dim betaB As Double = model.GetWeight("TF_B")

        Console.WriteLine($"     >> elastic net: beta(TF_A) = {betaA:F4}, beta(TF_B) = {betaB:F4}")

        Check(betaA <> 0 AndAlso betaB <> 0, "the elastic net keeps both of the collinear regulators")
        Check(std.Sign(betaA) = std.Sign(betaB), "the collinear regulators get the same regulation direction")

        Dim ratio As Double = std.Min(std.Abs(betaA), std.Abs(betaB)) / std.Max(std.Abs(betaA), std.Abs(betaB))

        Check(ratio > 0.6, $"the collinear regulators share the weight: ratio = {ratio:F3}")

        ' 纯 L1 的 LASSO 只会从这组共线基因之中保留一个
        Dim l1Grn As GeneRegulatoryNetwork = synth.matrix.InferRegulatoryNetwork(
            New GrnFittingOptions With {
                .silent = True,
                .method = FittingMethod.Lasso,
                .maxRegulatorsPerGene = 5
            }
        )
        Dim l1Model As GeneRegressionModel = l1Grn.GetModel("G_1")
        Dim lassoA As Double = l1Model.GetWeight("TF_A")
        Dim lassoB As Double = l1Model.GetWeight("TF_B")
        Dim lassoRatio As Double

        If lassoA = 0.0R OrElse lassoB = 0.0R Then
            lassoRatio = 0.0R
        Else
            lassoRatio = std.Min(std.Abs(lassoA), std.Abs(lassoB)) / std.Max(std.Abs(lassoA), std.Abs(lassoB))
        End If

        Console.WriteLine($"     >> lasso: beta(TF_A) = {lassoA:F4}, beta(TF_B) = {lassoB:F4}, " &
                          $"non-zero = {l1Model.nonZeroWeights}, balance = {lassoRatio:F3}")

        Check(lassoRatio < ratio, $"the lasso is less balanced than the elastic net: {lassoRatio:F3} < {ratio:F3}")

        ' 另外两档超参数确定方式(全局交叉验证 / 每基因交叉验证)的冒烟测试，
        ' 这组数据只有 3 个基因，交叉验证的成本很低
        Dim globalCV As GeneRegulatoryNetwork = synth.matrix.InferRegulatoryNetwork(
            New GrnFittingOptions With {
                .silent = True,
                .method = FittingMethod.ElasticNet,
                .maxRegulatorsPerGene = 5,
                .elasticNet = New ElasticNetOptions With {
                    .mode = ElasticNetMode.GlobalCrossValidation,
                    .globalCVReplicates = 3,
                    .cvAlphaSteps = 10
                }
            }
        )
        Dim globalModel As GeneRegressionModel = globalCV.GetModel("G_1")

        Check(globalModel.IsValid AndAlso globalModel.alpha > 0,
            $"the global cv mode works: alpha = {globalModel.alpha:G4}, l1_ratio = {globalModel.l1Ratio:G4}")
        Check(globalModel.GetWeight("TF_A") <> 0 AndAlso globalModel.GetWeight("TF_B") <> 0,
            "the global cv mode keeps both of the collinear regulators")

        Dim perGene As GeneRegulatoryNetwork = synth.matrix.InferRegulatoryNetwork(
            New GrnFittingOptions With {
                .silent = True,
                .method = FittingMethod.ElasticNet,
                .maxRegulatorsPerGene = 5,
                .elasticNet = New ElasticNetOptions With {
                    .mode = ElasticNetMode.PerGeneCrossValidation,
                    .cvAlphaSteps = 10
                }
            }
        )
        Dim perModel As GeneRegressionModel = perGene.GetModel("G_1")

        Check(perModel.IsValid AndAlso perModel.alpha > 0,
            $"the per-gene cv mode works: alpha = {perModel.alpha:G4}, l1_ratio = {perModel.l1Ratio:G4}")
        Check(perModel.GetWeight("TF_A") <> 0 AndAlso perModel.GetWeight("TF_B") <> 0,
            "the per-gene cv mode keeps both of the collinear regulators")
    End Sub

    ''' <summary>
    ''' 用例 10：同一个合成网络之上横向比较 OLS / LASSO / Elastic Net
    ''' </summary>
    Private Sub CaseCompareMethods()
        Dim synth As (matrix As Matrix, truth As TruthEdge()) = BuildNetwork(4, 16, 60, 0.15, 7)
        Dim watch As Diagnostics.Stopwatch = Diagnostics.Stopwatch.StartNew()
        Dim olsGrn As GeneRegulatoryNetwork = synth.matrix.InferRegulatoryNetwork(
            New GrnFittingOptions With {.silent = True, .maxCandidates = 10}
        )
        Dim timeOls As Double = watch.Elapsed.TotalSeconds

        watch.Restart()

        Dim l1Grn As GeneRegulatoryNetwork = synth.matrix.InferRegulatoryNetwork(
            New GrnFittingOptions With {
                .silent = True,
                .method = FittingMethod.Lasso,
                .maxCandidates = 18,
                .maxRegulatorsPerGene = 8
            }
        )
        Dim timeLasso As Double = watch.Elapsed.TotalSeconds

        watch.Restart()

        Dim enGrn As GeneRegulatoryNetwork = synth.matrix.InferRegulatoryNetwork(
            New GrnFittingOptions With {
                .silent = True,
                .method = FittingMethod.ElasticNet,
                .maxCandidates = 18,
                .maxRegulatorsPerGene = 8,
                .elasticNet = New ElasticNetOptions With {
                    .l1Ratio = 0.7,
                    .numberOfAlphas = 50
                }
            }
        )
        Dim timeEn As Double = watch.Elapsed.TotalSeconds

        Dim olsScore = Evaluate(synth.truth, olsGrn.Regulations().ToArray)
        Dim l1Score = Evaluate(synth.truth, l1Grn.Regulations().ToArray)
        Dim enScore = Evaluate(synth.truth, enGrn.Regulations().ToArray)

        Console.WriteLine($"     >> OLS         : f1={olsScore.f1:F3}, recall={olsScore.recall:F3}, " &
                          $"precision={olsScore.precision:F3}, {timeOls:F2}s")
        Console.WriteLine($"     >> LASSO       : f1={l1Score.f1:F3}, recall={l1Score.recall:F3}, " &
                          $"precision={l1Score.precision:F3}, {timeLasso:F2}s")
        Console.WriteLine($"     >> ElasticNet  : f1={enScore.f1:F3}, recall={enScore.recall:F3}, " &
                          $"precision={enScore.precision:F3}, {timeEn:F2}s")

        Check(enScore.f1 > 0.6, $"the elastic net f1 = {enScore.f1:F3}")
        Check(enScore.f1 >= l1Score.f1 - 0.1, "the elastic net is comparable to the lasso on this data")
        Check(enGrn.Regulations().ToArray().All(Function(e) e.l1Ratio = 0.7), "each edge reports the l1_ratio of its model")

        ' 调参提示：这个合成网络的每个靶基因只有 1~3 个真实的转录因子，
        ' 提高 l1_ratio(更接近纯 L1)并收紧每基因的调控因子上限会更贴近真实的稀疏结构
        Dim tuned As GeneRegulatoryNetwork = synth.matrix.InferRegulatoryNetwork(
            New GrnFittingOptions With {
                .silent = True,
                .method = FittingMethod.ElasticNet,
                .maxCandidates = 18,
                .maxRegulatorsPerGene = 5,
                .elasticNet = New ElasticNetOptions With {
                    .l1Ratio = 0.9,
                    .numberOfAlphas = 50
                }
            }
        )
        Dim tunedScore = Evaluate(synth.truth, tuned.Regulations().ToArray)

        Console.WriteLine($"     >> ElasticNet(l1=0.9, k=5): f1={tunedScore.f1:F3}, " &
                          $"recall={tunedScore.recall:F3}, precision={tunedScore.precision:F3}")
    End Sub

    ''' <summary>
    ''' 用例 11：弹性网络正则化路径的单调性、与单点拟合的一致性以及导出
    ''' </summary>
    Private Sub CaseElasticNetPath()
        Dim synth As (matrix As Matrix, truth As TruthEdge()) = BuildNetwork(4, 16, 60, 0.15, 7)
        Dim targetName As String = "G_1"
        Dim features As String() = synth.matrix.rownames.Where(Function(id) id <> targetName).ToArray
        Dim nS As Integer = synth.matrix.sample_count
        Dim X As Double()() = New Double(nS - 1)() {}

        For s As Integer = 0 To nS - 1
            Dim sample As Integer = s

            X(s) = features.Select(Function(id) synth.matrix(id).experiments(sample)).ToArray
        Next

        Dim y As Double() = synth.matrix(targetName).experiments
        Dim ratio As Double = 0.5
        Dim path As ElasticNetPath = ElasticNetFitting.RegularizationPath(
            x:=X, y:=y, l1Ratio:=ratio, numberOfAlphas:=20, standardize:=True, tolerance:=0.0000000001)
        Dim lastStep As Integer = path.Count - 1
        Dim fit As ElasticNetFit = ElasticNetFitting.Fit(
            x:=X, y:=y, alpha:=path.Alphas(lastStep), l1Ratio:=ratio, standardize:=True, tolerance:=0.0000000001)

        Check(path.Count = 20, $"the elastic net path has {path.Count} step(s)")
        Check(path.Alphas(0) > path.Alphas(lastStep), "the alphas decrease along the path")

        Dim monotoneNonZero As Boolean = True
        Dim monotoneR2 As Boolean = True

        For k As Integer = 1 To lastStep
            If path.NonZeroCounts(k) < path.NonZeroCounts(k - 1) Then
                monotoneNonZero = False
            End If
            If path.RSquared(k) < path.RSquared(k - 1) - 0.000001 Then
                monotoneR2 = False
            End If
        Next

        Check(monotoneNonZero, "the number of the non-zero coefficients is monotone along the path")
        Check(monotoneR2, "the R2 is monotone along the path")

        Dim gap As Double = 0.0R
        Dim lastCoef As Double() = path.Coefficients(lastStep)

        For j As Integer = 0 To fit.Coefficients.Length - 1
            gap = std.Max(gap, std.Abs(fit.Coefficients(j) - lastCoef(j)))
        Next

        Console.WriteLine($"     >> last path step: alpha = {path.Alphas(lastStep):G4}, " &
                          $"nonzero = {path.NonZeroCounts(lastStep)}, R2 = {path.RSquared(lastStep):F4}")

        Check(gap < 0.000001, $"the last path step matches a standalone fit, max |diff| = {gap:E2}")

        ' 导出：边表需要携带 alpha / l1_ratio 两列，系数矩阵的 tag 需要标明算法与超参数
        Dim enGrn As GeneRegulatoryNetwork = synth.matrix.InferRegulatoryNetwork(
            New GrnFittingOptions With {
                .silent = True,
                .method = FittingMethod.ElasticNet,
                .maxCandidates = 18,
                .maxRegulatorsPerGene = 8,
                .elasticNet = New ElasticNetOptions With {
                    .l1Ratio = 0.7,
                    .numberOfAlphas = 40
                }
            }
        )
        Dim dir As String = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "grn-en-demo")
        Dim csv As String = System.IO.Path.Combine(dir, "elastic-net-edges.csv")

        Call enGrn.WriteEdges(csv)

        Dim header As String = File.ReadAllLines(csv)(0)

        Check(header = "regulator,target,weight,abs_weight,intercept,r2,pvalue,alpha,l1_ratio,interaction",
            "the edge table carries the alpha and l1_ratio columns")
        Check(enGrn.CoefMatrix().tag.Contains("ElasticNet"), $"the coefficient matrix tag = {enGrn.CoefMatrix().tag}")
        Check(enGrn.Regulations().ToArray().Any(), "the elastic net network exports at least one edge")
    End Sub

    ''' <summary>
    ''' 用例 13：模型文件的持久化往返
    ''' 
    ''' 拟合好的网络被保存成 ``zip`` 压缩包之后重新加载，检查表达矩阵、
    ''' 拟合参数、逐基因的模型以及全局交叉验证的超参数是否被完整地还原。
    ''' </summary>
    Private Sub CasePersistence()
        Dim synth As (matrix As Matrix, truth As TruthEdge()) = BuildNetwork(
            nTf:=4, nTarget:=12, nSample:=40, noise:=0.2, seed:=41
        )
        Dim options As New GrnFittingOptions With {
            .silent = True,
            .maxCandidates = 10,
            .minAbsWeight = 0.05,
            .computeStatistics = True
        }
        Dim grn As GeneRegulatoryNetwork = synth.matrix.InferRegulatoryNetwork(options)
        Dim dir As String = Path.Combine(Path.GetTempPath(), "grn-persistence-demo")
        Dim zipFile As String = Path.Combine(dir, "ols-network.grn.zip")

        Call grn.Save(zipFile)

        Check(File.Exists(zipFile), $"the fitted network is saved into {zipFile}")
        Check(New FileInfo(zipFile).Length > 0, $"the model file takes {New FileInfo(zipFile).Length} bytes")

        Dim entries As String() = ZipEntries(zipFile)

        Check(entries.OrderBy(Function(e) e).SequenceEqual(
                  {"expression.tsv", "meta.json", "models.json", "options.json"}),
            "the archive contains the entries: " & String.Join(", ", entries))

        Dim loaded As GeneRegulatoryNetwork = GRNFile.Load(zipFile)

        Check(loaded.Size = grn.Size, $"the gene size is restored: {loaded.Size}")
        Check(loaded.Genes.SequenceEqual(grn.Genes), "the gene ids are restored in the original order")
        Check(loaded.Expression.sampleID.SequenceEqual(grn.Expression.sampleID), "the sample ids are restored")
        Check(loaded.Expression.tag = grn.Expression.tag, $"the expression matrix tag is restored: {loaded.Expression.tag}")
        Check(loaded.Options.method = grn.Options.method, $"the fitting method is restored: {loaded.Options.method}")
        Check(loaded.Options.maxCandidates = grn.Options.maxCandidates, "the fitting options are restored")

        Check(SameMatrix(grn.Expression, loaded.Expression), "the raw expression matrix is restored without any precision loss")
        Check(SameVectors(grn.Vectors, loaded.Vectors), "the standardized expression vectors are recomputed identically")
        Check(SameVector(grn.Baseline, loaded.Baseline), "the baseline expression is recomputed identically")
        Check(SameVector(grn.BaselineSD, loaded.BaselineSD), "the baseline deviation is recomputed identically")

        Dim before As GeneRegressionModel() = grn.Models.OrderBy(Function(m) m.target).ToArray
        Dim after As GeneRegressionModel() = loaded.Models.OrderBy(Function(m) m.target).ToArray

        Check(before.Length = after.Length, $"{after.Length} gene model(s) are restored")
        Check(SameModels(before, after), "each gene model is restored without any precision loss")

        Dim edgesBefore As RegulationEdge() = grn.Regulations().ToArray
        Dim edgesAfter As RegulationEdge() = loaded.Regulations().ToArray

        Check(edgesBefore.Length > 0 AndAlso edgesBefore.Length = edgesAfter.Length,
            $"{edgesAfter.Length} regulation edge(s) are restored")
        Check(EdgeKeys(edgesBefore).SequenceEqual(EdgeKeys(edgesAfter)), "the regulation edges are identical")

        Dim coefBefore As Matrix = grn.CoefMatrix()
        Dim coefAfter As Matrix = loaded.CoefMatrix()

        Check(coefAfter.tag = coefBefore.tag, $"the coefficient matrix is rebuilt: {coefAfter.tag}")
        Check(SameMatrix(coefBefore, coefAfter), "the coefficient matrix values are identical")

        ' 流式接口：写出去的流在方法返回之后仍然可用，直接回绕就可以读取
        Dim buffer As New MemoryStream()

        Call grn.Save(buffer)

        Check(buffer.Length > 0, $"the network could be saved into a stream ({buffer.Length} bytes)")

        buffer.Position = 0

        Dim fromStream As GeneRegulatoryNetwork = GRNFile.Load(buffer)

        Check(SameModels(before, fromStream.Models.OrderBy(Function(m) m.target).ToArray),
            "the network could be restored from a stream")

        ' 全局交叉验证解析出来的超参数也需要一起保存，否则系数矩阵的标签会退化
        Dim cvGrn As GeneRegulatoryNetwork = synth.matrix.InferRegulatoryNetwork(
            New GrnFittingOptions With {
                .silent = True,
                .method = FittingMethod.ElasticNet,
                .maxRegulatorsPerGene = 5,
                .elasticNet = New ElasticNetOptions With {
                    .mode = ElasticNetMode.GlobalCrossValidation,
                    .globalCVReplicates = 3,
                    .cvAlphaSteps = 10,
                    .numberOfAlphas = 20
                }
            }
        )
        Dim cvFile As String = Path.Combine(dir, "en-network.grn.zip")

        Call cvGrn.Save(cvFile)

        Dim cvLoaded As GeneRegulatoryNetwork = GRNFile.Load(cvFile)
        Dim cvBefore As GeneRegressionModel() = cvGrn.Models.OrderBy(Function(m) m.target).ToArray
        Dim cvAfter As GeneRegressionModel() = cvLoaded.Models.OrderBy(Function(m) m.target).ToArray

        Check(SameModels(cvBefore, cvAfter), "the elastic net models are restored")
        Check(cvLoaded.CoefMatrix().tag = cvGrn.CoefMatrix().tag,
            $"the cv hyperparameters are restored: {cvLoaded.CoefMatrix().tag}")
        Check(cvLoaded.Options.elasticNet.mode = ElasticNetMode.GlobalCrossValidation,
            "the elastic net options are restored")

        ' 还没有拟合的网络(空的模型集合)也必须能够往返，便于保存待分析的输入数据
        Dim pending As New GeneRegulatoryNetwork(synth.matrix, options)
        Dim pendingFile As String = Path.Combine(dir, "pending-network.grn.zip")

        Call pending.Save(pendingFile)

        Dim pendingLoaded As GeneRegulatoryNetwork = GRNFile.Load(pendingFile)

        Check(pendingLoaded.Models.Count() = 0, "a network without any fitted model is saved and restored")
        Check(pendingLoaded.Regulations().Count() = 0, "the restored pending network reports no regulation edge")
        Check(pendingLoaded.Size = synth.matrix.size, "the expression matrix of the pending network is restored")

        ' 损坏的文件必须给出明确的异常，而不是返回一个半成品对象
        Dim garbage As New MemoryStream(Encoding.UTF8.GetBytes("this is not a grn model file"))

        Check(Throws(Sub()
                         Dim broken As GeneRegulatoryNetwork = GRNFile.Load(garbage)

                         Check2(broken)
                     End Sub),
            "an invalid stream is rejected")
    End Sub

    ''' <summary>
    ''' 用例 12：在真实的基因表达矩阵之上用弹性网络建立调控网络
    ''' 
    ''' ```
    ''' dotnet test.dll --matrix "K:\hsa_grn\20261020-1\hsa_sub_expression.csv" [--top 150] [--candidates 100] [--maxreg 10] [--l1 0.7] [--alphas 40] [--out dir]
    ''' ```
    ''' 
    ''' 真实的表达矩阵包含上万个基因，全部纳入推断既无必要也不可行，
    ''' 因此会先过滤零表达基因、做 ``log2(x + 1)`` 变换，然后选出表达变化最剧烈的
    ''' 若干个基因作为节点。
    ''' </summary>
    ''' <param name="matrixFile">表达矩阵 CSV 文件的路径</param>
    ''' <param name="args">命令行参数</param>
    Private Sub CaseRealMatrix(matrixFile As String, args As String())
        Dim topGenes As Integer = ArgInt(args, "--top", 150)
        Dim candidates As Integer = ArgInt(args, "--candidates", 100)
        Dim maxRegulators As Integer = ArgInt(args, "--maxreg", 10)
        Dim l1Ratio As Double = ArgDouble(args, "--l1", 0.7)
        Dim alphas As Integer = ArgInt(args, "--alphas", 40)
        Dim mutualOnly As Boolean = args.Any(Function(a) String.Equals(a, "--mutual", StringComparison.OrdinalIgnoreCase))
        Dim watch As Diagnostics.Stopwatch = Diagnostics.Stopwatch.StartNew()
        Dim loaded = MatrixCsvLoader.Load(matrixFile, New MatrixCsvLoader.LoadOptions With {
            .topGenes = topGenes,
            .minNonZeroRatio = 0.5,
            .logTransform = True
        })

        Console.WriteLine($"     >> loaded {loaded.retained} of {loaded.scanned} genes, " &
                          $"{loaded.matrix.sample_count} samples, cost {watch.Elapsed.TotalSeconds:F1}s")

        Check(loaded.matrix.size > 10, $"{loaded.matrix.size} genes are kept from the expression matrix")
        Check(loaded.matrix.sample_count > 4, $"{loaded.matrix.sample_count} samples are loaded")

        Dim options As New GrnFittingOptions With {
            .silent = True,
            .method = FittingMethod.ElasticNet,
            .maxCandidates = candidates,
            .maxRegulatorsPerGene = maxRegulators,
            .mutualOnly = mutualOnly,
            .elasticNet = New ElasticNetOptions With {
                .l1Ratio = l1Ratio,
                .numberOfAlphas = alphas
            }
        }

        If mutualOnly Then
            Console.WriteLine("     >> only the mutual (reversible) regulation pairs are kept")
        End If

        watch.Restart()

        Dim grn As GeneRegulatoryNetwork = loaded.matrix.InferRegulatoryNetwork(options)
        Dim edges As RegulationEdge() = grn.Regulations().ToArray
        Dim failed As Integer = grn.Models.Where(Function(m) Not m.IsValid).Count()

        Console.WriteLine($"     >> fitting cost {watch.Elapsed.TotalSeconds:F1}s, " &
                          $"{edges.Length} regulation edges, {failed} failed model(s)")

        Check(edges.Length > 0, $"{edges.Length} regulation edges are inferred from the real expression data")
        Check(failed = 0, $"all of the {grn.Size} gene models are fitted")

        Console.WriteLine("     >> top regulons (by the number of target genes):")

        For Each item In edges.RegulonSizes().Take(8)
            Console.WriteLine($"        {item.regulator} ({item.size} targets)")
        Next

        Console.WriteLine("     >> top regulation edges (by |beta|):")

        For Each edge As RegulationEdge In edges.TopEdges(10)
            Console.WriteLine($"        {edge.interaction}: {edge.regulator} -> {edge.target}, " &
                              $"beta={edge.weight:G4}, alpha={edge.alpha:G4}, R2={edge.r2:F3}")
        Next

        Dim dir As String = ArgValue(args, "--out", System.IO.Path.Combine(System.IO.Path.GetTempPath(), "grn-hsa-demo"))
        Dim csv As String = System.IO.Path.Combine(dir, "hsa-elastic-net-edges.csv")

        Call grn.WriteEdges(csv)

        Check(File.Exists(csv), $"the inferred network is exported into {csv}")

        Dim coef As Matrix = grn.CoefMatrix()

        Check(coef.expression.Length = grn.Size,
            $"the coefficient matrix is [{coef.expression.Length} x {coef.sampleID.Length}], tag = {coef.tag}")

        Dim significant As Integer = edges.Count(Function(e) e.r2 > 0.9)

        Console.WriteLine($"     >> {significant} edges come from the gene models with R2 > 0.9")
    End Sub

    ''' <summary>
    ''' 直接驱动 LASSO 求解器跑出一条正则化路径
    ''' </summary>
    ''' <param name="X">行主序的设计矩阵</param>
    ''' <param name="y">观测值向量</param>
    ''' <param name="maxFeatures">每一个模型允许的最大非零特征数量，负数表示不做限制</param>
    ''' <returns></returns>
    Private Function LassoPath(X As Double()(), y As Double(), maxFeatures As Integer) As LassoFit
        Dim nS As Integer = y.Length
        Dim p As Integer = X(0).Length
        Dim names As String() = Enumerable.Range(1, p).Select(Function(i) $"x{i}").ToArray
        Dim generator As New LassoFitGenerator()

        LassoFitGenerator.tqdm_verbose = False
        Call generator.init(names, nS)

        For j As Integer = 0 To p - 1
            Dim column As Double() = New Double(nS - 1) {}

            For i As Integer = 0 To nS - 1
                column(i) = X(i)(j)
            Next

            Call generator.setFeatureValues(j, column)
        Next
        For i As Integer = 0 To nS - 1
            Call generator.setTarget(i, y(i))
        Next

        Return generator.fit(maxFeatures)
    End Function

    ''' <summary>
    ''' 生成一个指定行列数量、元素为随机数的矩阵
    ''' </summary>
    Private Function RectangularArray(rows As Integer, cols As Integer, rng As Random) As Double()()
        Dim mat As Double()() = New Double(rows - 1)() {}

        For i As Integer = 0 To rows - 1
            Dim row As Double() = New Double(cols - 1) {}

            For j As Integer = 0 To cols - 1
                row(j) = Gauss(rng)
            Next

            mat(i) = row
        Next

        Return mat
    End Function

#Region "helpers"

    ''' <summary>
    ''' 读取命令行参数的值(形如 ``--matrix path``)
    ''' </summary>
    ''' <param name="args">命令行参数</param>
    ''' <param name="name">参数名称</param>
    ''' <param name="fallback">参数缺失的时候所使用的默认值</param>
    ''' <returns></returns>
    Private Function ArgValue(args As String(), name As String, Optional fallback As String = Nothing) As String
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

    Private Sub Run(title As String, body As Action)
        Console.WriteLine($"--- {title}")

        Try
            Call body()
        Catch ex As Exception
            failures += 1

            Console.WriteLine("  [FAIL] unhandled exception: " & ex.Message)
        End Try

        Console.WriteLine()
    End Sub

    Private Sub Check(condition As Boolean, message As String)
        If condition Then
            Console.WriteLine("  [ OK ] " & message)
        Else
            failures += 1

            Console.WriteLine("  [FAIL] " & message)
        End If
    End Sub

    ''' <summary>
    ''' 使得临时构造出来的网络对象被引用一次，避免编译器把对象构造优化掉
    ''' </summary>
    ''' <param name="probe"></param>
    Private Sub Check2(probe As GeneRegulatoryNetwork)
        probe.ToString()
    End Sub

    Private Function Throws(body As Action) As Boolean
        Try
            Call body()

            Return False
        Catch ex As Exception
            Return True
        End Try
    End Function

    Private Function MakeMatrix(names As String(), data As Double()(), nSample As Integer) As Matrix
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
            .tag = "demo",
            .sampleID = samples,
            .expression = rows
        }
    End Function

    Private Function MakeMatrix(names As String(), data As Double()(), samples As String()) As Matrix
        Dim rows As DataFrameRow() = names _
            .Select(Function(name, i)
                        Return New DataFrameRow With {
                            .geneID = name,
                            .experiments = data(i)
                        }
                    End Function) _
            .ToArray

        Return New Matrix With {
            .tag = "demo",
            .sampleID = samples,
            .expression = rows
        }
    End Function

    ''' <summary>
    ''' 枚举压缩包之中的条目名称
    ''' </summary>
    Private Function ZipEntries(zipFile As String) As String()
        Using stream As New FileStream(zipFile, FileMode.Open, FileAccess.Read)
            Using archive As New ZipArchive(stream, ZipArchiveMode.Read)
                Return archive.Entries.Select(Function(e) e.FullName).ToArray
            End Using
        End Using
    End Function

    ''' <summary>
    ''' 逐个元素(逐位)比较两个矩阵
    ''' </summary>
    Private Function SameMatrix(expected As Matrix, actual As Matrix) As Boolean
        If expected.sample_count <> actual.sample_count Then
            Return False
        End If
        If Not expected.sampleID.SequenceEqual(actual.sampleID) Then
            Return False
        End If
        If expected.expression.Length <> actual.expression.Length Then
            Return False
        End If

        For i As Integer = 0 To expected.expression.Length - 1
            If expected.expression(i).geneID <> actual.expression(i).geneID Then
                Return False
            End If
            If Not SameVector(expected.expression(i).experiments, actual.expression(i).experiments) Then
                Return False
            End If
        Next

        Return True
    End Function

    ''' <summary>
    ''' 逐位比较两个向量
    ''' </summary>
    Private Function SameVector(expected As Double(), actual As Double()) As Boolean
        If expected Is Nothing OrElse actual Is Nothing Then
            Return expected Is actual
        End If
        If expected.Length <> actual.Length Then
            Return False
        End If

        For i As Integer = 0 To expected.Length - 1
            If Not SameValue(expected(i), actual(i)) Then
                Return False
            End If
        Next

        Return True
    End Function

    ''' <summary>
    ''' 逐个向量比较两个锯齿数组
    ''' </summary>
    Private Function SameVectors(expected As Double()(), actual As Double()()) As Boolean
        If expected Is Nothing OrElse actual Is Nothing Then
            Return expected Is actual
        End If
        If expected.Length <> actual.Length Then
            Return False
        End If

        For i As Integer = 0 To expected.Length - 1
            If Not SameVector(expected(i), actual(i)) Then
                Return False
            End If
        Next

        Return True
    End Function

    Private Function SameStrings(expected As String(), actual As String()) As Boolean
        If expected Is Nothing OrElse actual Is Nothing Then
            Return expected Is actual
        End If

        Return expected.SequenceEqual(actual)
    End Function

    ''' <summary>
    ''' 逐位比较两个数值，``NaN`` 与 ``NaN`` 也视为相等
    ''' </summary>
    Private Function SameValue(expected As Double, actual As Double) As Boolean
        Return expected.Equals(actual)
    End Function

    ''' <summary>
    ''' 逐个比较两组(已经按目标基因排序的)模型
    ''' </summary>
    Private Function SameModels(expected As GeneRegressionModel(), actual As GeneRegressionModel()) As Boolean
        If expected.Length <> actual.Length Then
            Return False
        End If

        For i As Integer = 0 To expected.Length - 1
            If Not SameModel(expected(i), actual(i)) Then
                Console.WriteLine($"     >> the model of '{expected(i).target}' is changed by the round trip")
                Return False
            End If
        Next

        Return True
    End Function

    ''' <summary>
    ''' 比较两个基因模型的所有数据成员
    ''' </summary>
    Private Function SameModel(expected As GeneRegressionModel, actual As GeneRegressionModel) As Boolean
        If expected.target <> actual.target OrElse expected.method <> actual.method Then
            Return False
        End If
        If expected.N <> actual.N OrElse expected.df <> actual.df Then
            Return False
        End If
        If String.CompareOrdinal(expected.errorMessage, actual.errorMessage) <> 0 Then
            Return False
        End If

        Return SameValue(expected.intercept, actual.intercept) AndAlso
               SameValue(expected.interceptT, actual.interceptT) AndAlso
               SameValue(expected.interceptPvalue, actual.interceptPvalue) AndAlso
               SameValue(expected.r2, actual.r2) AndAlso
               SameValue(expected.sse, actual.sse) AndAlso
               SameValue(expected.sst, actual.sst) AndAlso
               SameValue(expected.lambda, actual.lambda) AndAlso
               SameValue(expected.alpha, actual.alpha) AndAlso
               SameValue(expected.l1Ratio, actual.l1Ratio) AndAlso
               SameStrings(expected.regulators, actual.regulators) AndAlso
               SameVector(expected.weights, actual.weights) AndAlso
               SameVector(expected.tvalues, actual.tvalues) AndAlso
               SameVector(expected.pvalues, actual.pvalues)
    End Function

    ''' <summary>
    ''' 把调控边转换成可以逐字比较的文本指纹
    ''' </summary>
    Private Function EdgeKeys(edges As RegulationEdge()) As String()
        Return edges _
            .Select(Function(e)
                        Return $"{e.UID}|p={e.pvalue.ToString("R", Globalization.CultureInfo.InvariantCulture)}|" &
                               $"r2={e.r2.ToString("R", Globalization.CultureInfo.InvariantCulture)}|" &
                               $"b={e.weight.ToString("R", Globalization.CultureInfo.InvariantCulture)}"
                    End Function) _
            .OrderBy(Function(s) s) _
            .ToArray
    End Function

#End Region

End Module
