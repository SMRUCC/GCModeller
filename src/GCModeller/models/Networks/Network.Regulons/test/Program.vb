Imports System.IO
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
        Console.WriteLine("== GeneRegulatoryNetwork linear model fitting demo ==")
        Console.WriteLine()

        Run("case 1: recover the coefficients of a hand-made linear model", AddressOf CaseExactRecovery)
        Run("case 2: rebuild a synthetic TF-target network by OLS", AddressOf CaseOlsNetwork)
        Run("case 3: rebuild the same network by the sparse LASSO model", AddressOf CaseLassoNetwork)
        Run("case 4: restrict the regulator pool to the transcription factors", AddressOf CaseRegulatorPool)
        Run("case 5: export the coefficient matrix and the edge table", AddressOf CaseExport)
        Run("case 6: input validation", AddressOf CaseValidation)
        Run("case 7: lasso solver regression tests", AddressOf CaseLassoSolver)

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
        Check(lines(0) = "regulator,target,weight,abs_weight,intercept,r2,pvalue,interaction", "the csv header is correct")

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

#End Region

End Module
