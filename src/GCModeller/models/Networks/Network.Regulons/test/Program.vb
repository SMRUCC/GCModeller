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

        Console.WriteLine($"     >> {synth.matrix.size} genes x {synth.matrix.sample_count} samples, " &
                          $"{synth.truth.Length} true edges, {edges.Length} predicted edges, cost {seconds.ToString("F2")}s")

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
    Private Sub DebugLasso(expr As Matrix)
        Dim target As String = "G_1"
        Dim names As New List(Of String)()
        Dim cols As New List(Of Double())()
        Dim nS As Integer = expr.sample_count

        For Each id As String In expr.rownames
            If id <> target Then
                names.Add(id)
                cols.Add(expr(id).experiments)
            End If
        Next

        Dim generator As New LassoFitGenerator()

        LassoFitGenerator.tqdm_verbose = False
        generator.init(names.ToArray, nS)

        For j As Integer = 0 To names.Count - 1
            Call generator.setFeatureValues(j, cols(j))
        Next

        Dim y As Double() = expr(target).experiments

        For i As Integer = 0 To nS - 1
            Call generator.setTarget(i, y(i))
        Next

        Dim fit As LassoFit = generator.fit(-1)

        Console.WriteLine($"     DEBUG numberOfLambdas={fit.numberOfLambdas}, numFeatures={fit.numFeatures}, passes={fit.numberOfPasses}")
        Console.WriteLine($"     DEBUG nonZero=[{String.Join(", ", fit.nonZeroWeights.Take(8))}]")
        Console.WriteLine($"     DEBUG rsquared=[{String.Join(", ", fit.rsquared.Take(8).Select(Function(x) x.ToString("F4")))}]")
        Console.WriteLine($"     DEBUG lambdas=[{String.Join(", ", fit.lambdas.Take(8).Select(Function(x) x.ToString("F4")))}]")
        Console.WriteLine($"     DEBUG last weights=[{String.Join(", ", fit.getWeights(std.Max(fit.numberOfLambdas - 1, 0)).Select(Function(x) x.ToString("F3")))}]")
    End Sub

    Private Sub CaseLassoNetwork()
        Dim synth As (matrix As Matrix, truth As TruthEdge()) = BuildNetwork(
            nTf:=4,
            nTarget:=16,
            nSample:=60,
            noise:=0.15,
            seed:=7
        )

        Call DebugLasso(synth.matrix)
        Dim options As New GrnFittingOptions With {
            .silent = True,
            .method = FittingMethod.Lasso,
            .maxCandidates = 18,
            .maxRegulatorsPerGene = 4
        }
        Dim grn As GeneRegulatoryNetwork = synth.matrix.InferRegulatoryNetwork(options)

        For Each failed As GeneRegressionModel In grn.Models.Where(Function(m) Not m.IsValid).Take(3)
            Console.WriteLine($"     ERR {failed.target}: {failed.errorMessage}")
        Next

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
