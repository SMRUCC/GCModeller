Imports System.Diagnostics
Imports System.IO
Imports Microsoft.VisualBasic.DeepLearning.LiquidNeuralNetwork
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.Analysis.Metaboliq
Imports std = System.Math

''' <summary>
''' Metaboliq —— 基于液态神经网络（LNN / LTC / CfC）的代谢网络动力学模拟：全链路演示
''' </summary>
''' <remarks>
''' 演示流程（与 readme.md 第六节的 pipeline 一一对应）：
''' <list type="number">
''' <item><description>生成 / 加载 demo 模拟数据（网络 JSON + 时序 CSV）</description></item>
''' <item><description>由 <see cref="MetabolicReaction"/> 构建 <see cref="MetabolicNetworkGraph"/></description></item>
''' <item><description>用 <c>Matrix.LoadData</c> 载入时序矩阵并做 log1p + z-score 归一化</description></item>
''' <item><description>构建结构化 LTC 代谢模型（含拓扑掩码与通量读取头）</description></item>
''' <item><description>可选：解析梯度 vs 数值梯度的一致性自检</description></item>
''' <item><description>PINN 风格多目标训练（数据 / 质量守恒 / 热力学 / 通量监督）</description></item>
''' <item><description>模拟输出浓度轨迹、通量分布与液态时间常数 τ^sys</description></item>
''' <item><description>敲除呼吸链反应，外推"有氧 → 无氧"代谢重编程</description></item>
''' <item><description>评估（RMSE / R² / ‖S·v‖）并导出结果 CSV</description></item>
''' </list>
''' </remarks>
Module Program

    Sub Main(args As String())
        Console.OutputEncoding = System.Text.Encoding.UTF8

        ' ------------------------------------------------------------------
        ' 路径准备：从 bin 目录向上回到 test 目录，data/ 与 result/ 都放在它下面
        ' ------------------------------------------------------------------
        Dim testDir = FindDirectoryUp(AppContext.BaseDirectory, "test")
        Dim dataDir = Path.Combine(testDir, "data")
        Dim resultDir = Path.Combine(testDir, "result")

        Call Banner("Metaboliq: 基于液态神经网络的代谢网络动力学模拟")

        ' ==================================================================
        ' 阶段 1：生成 / 加载 demo 模拟数据
        ' ==================================================================
        Call Banner("阶段 1 / 8  生成 demo 模拟数据")

        Dim files = DemoData.Generate(dataDir)
        Dim networkPath = files(0)
        Dim metaboliteCsv = files(1)
        Dim enzymeCsv = files(2)
        Dim fluxCsv = files(3)

        For Each f In files
            Console.WriteLine($"  [数据] {Path.GetFileName(f)}  ({New FileInfo(f).Length / 1024.0:F1} KB)")
        Next

        ' ==================================================================
        ' 阶段 2：由 MetabolicReaction 构建代谢网络拓扑
        ' ==================================================================
        Call Banner("阶段 2 / 8  构建代谢网络拓扑")

        Dim graph = MetabolicNetworkGraph.LoadJson(networkPath, DemoData.BoundaryIds)

        Console.WriteLine($"  网络规模        : {graph}")
        Console.WriteLine($"  化学计量矩阵 S  : {graph.Stoichiometry.Shape(0)} 代谢物 × {graph.Stoichiometry.Shape(1)} 反应")
        Console.WriteLine($"  内部代谢物({graph.MetaboliteCount}) : {String.Join(", ", graph.InternalIds)}")
        Console.WriteLine($"  边界代谢物({graph.BoundaryCount}) : {String.Join(", ", graph.BoundaryIds)}")
        Console.WriteLine($"  反应({graph.ReactionCount})      : {String.Join(", ", graph.ReactionIds)}")
        Console.WriteLine($"  网络输入维度    : {graph.InputSize}（{graph.ReactionCount} 个酶通道 + {graph.BoundaryCount} 个边界通道）")

        ' ==================================================================
        ' 阶段 3：载入时序矩阵并归一化
        ' ==================================================================
        Call Banner("阶段 3 / 8  载入时序表达矩阵（Matrix.LoadData）")

        ' 代谢物矩阵：行=代谢物，列=样本（时间点）
        Dim metabolomeRaw = MetabolicDataIO.LoadCsv(metaboliteCsv)
        Dim metabolome = MetabolicDataIO.LogZScoreNormalize(metabolomeRaw)

        Console.WriteLine($"  代谢物矩阵      : {metabolome.FeatureCount} 行 × {metabolome.SampleCount} 列，归一化的={metabolome.Normalization}")
        Console.WriteLine($"  时间轴（不规则）: {String.Join(", ", metabolome.Times.Select(Function(x) x.ToString("0.#")))}")

        ' 监督目标：内部代谢物浓度 (T × m)
        Dim observed = metabolome.Reorder(graph.InternalIds)
        ' 外部驱动：边界代谢物浓度 (T × nB)
        Dim boundarySeries = metabolome.Reorder(graph.BoundaryIds)

        ' 酶表达矩阵：min-max 到 [0,1]，因为通量读取头 v = e ⊙ σ(·) 直接把 e 当作容量上限
        Dim enzymeRaw = MetabolicDataIO.LoadCsv(enzymeCsv)
        Dim enzymeSeries = MetabolicDataIO.MinMaxNormalize(enzymeRaw).Reorder(graph.ReactionIds)

        Console.WriteLine($"  酶表达矩阵      : {enzymeRaw.FeatureCount} 行 × {enzymeRaw.SampleCount} 列，归一化的=minmax")

        ' 真值通量（可选，用于 λ3 通量监督与最终验证）
        Dim fluxTruthRaw = MetabolicDataIO.LoadCsv(fluxCsv)
        Dim fluxTruth = fluxTruthRaw.Reorder(graph.ReactionIds)

        Dim times = metabolome.Times
        Dim T = times.Length
        Dim h0 = Row(observed, 0)

        ' ==================================================================
        ' 阶段 4：构建结构化 LTC 代谢模型
        ' ==================================================================
        Call Banner("阶段 4 / 8  构建结构化 LTC 代谢模型")

        Dim model As New MetabolicLiquidNetwork(graph, LiquidMode.LTC, "rk4", seed:=42)
        ' 代谢系统是 stiff 系统：不同反应的时间尺度跨越多个数量级，放宽 τ 的取值范围
        model.SetTauBounds(0.5, 30.0)

        Console.WriteLine($"  {model}")
        Console.WriteLine($"  结构化掩码      : 循环权重被掩码掉 {model.MaskedRatio() * 100.0:F1}% 的连接（无生化关联即不可连接）")
        Console.WriteLine($"  隐藏状态 h      : {model.MetaboliteCount} 维 = 代谢物浓度")
        Console.WriteLine($"  外部输入 u      : {model.InputSize} 维 = 酶表达 + 边界底物")
        Console.WriteLine($"  通量读取头      : v = e ⊙ σ(Wv·[h;u] + bv)")

        ' ==================================================================
        ' 阶段 5：梯度自检（解析梯度 vs 中心差分）
        ' ==================================================================
        Call Banner("阶段 5 / 8  梯度自检（解析 BPTT vs 数值差分）")

        Call GradientSelfCheck(graph, times, observed, enzymeSeries, boundarySeries, fluxTruth)

        ' ==================================================================
        ' 阶段 6：PINN 风格多目标训练
        ' ==================================================================
        Call Banner("阶段 6 / 8  PINN 风格多目标训练")

        Dim config As New MetabolicTrainerConfig With {
            .LambdaData = 1.0,
            .LambdaMass = 1.0,      ' ‖S·v̂‖²  质量守恒
            .LambdaThermo = 0.5,    ' 不可逆反应通量非负
            .LambdaFlux = 0.2,      ' 通量监督（有 13C-MFA 真值时启用）
            .LearningRate = 0.01,
            .Epochs = 200,
            .WarmupEpochs = 20,
            .GradientClip = 5.0,
            .TeacherForcingStart = 0.9,
            .TeacherForcingEnd = 0.0,
            .LogEvery = 20,
            .Verbose = True,
            .Seed = 123
        }

        Dim trainer As New MetabolicTrainer(model, config)
        Dim before = trainer.Evaluate(times, observed, enzymeSeries, boundarySeries, fluxTruth)

        Console.WriteLine($"  训练前 loss     : {before}")
        Console.WriteLine()

        Dim sw = Stopwatch.StartNew()
        Dim history = trainer.Fit(times, observed, enzymeSeries, boundarySeries, fluxTruth)
        sw.Stop()

        Console.WriteLine()
        Console.WriteLine($"  训练后 loss     : {history(history.Count - 1)}")
        Console.WriteLine($"  训练耗时        : {sw.ElapsedMilliseconds} ms / {config.Epochs} epochs")

        ' ==================================================================
        ' 阶段 7：模拟输出（浓度 / 通量 / 液态时间常数）
        ' ==================================================================
        Call Banner("阶段 7 / 8  模拟输出：浓度轨迹 / 通量分布 / τ^sys")

        Dim traj = trainer.Predict(h0, times, enzymeSeries, boundarySeries)

        Console.WriteLine("  代谢物浓度轨迹（归一化空间，抽样 6 个时间点）：")
        Call PrintTrajectory(traj, {"g6p", "pyr", "accoa", "cit", "atp", "nadh", "lac_c", "etoh_c"}, 6)

        Console.WriteLine()
        Console.WriteLine("  反应通量（末时刻，按大小排序前 12 条）：")
        Call PrintFluxes(traj, T - 1, 12)

        Console.WriteLine()
        Console.WriteLine("  液态时间常数 τ^sys（可解释性输出，末时刻，最小的 8 个 = 响应最快的代谢物）：")
        Call PrintTau(traj, T - 1, 8)

        ' ==================================================================
        ' 阶段 8：敲除呼吸链 → 有氧/无氧代谢重编程外推
        ' ==================================================================
        Call Banner("阶段 8 / 8  扰动外推：敲除呼吸链反应")

        Dim last = T - 1

        ' 野生型基线
        model.ResetPerturbation()
        Dim wildType = model.Simulate(h0, enzymeSeries, boundarySeries, times)

        ' 敲除细胞色素氧化酶（有氧呼吸链终端氧化酶）
        model.KnockOut("CYTBO3")
        Dim koCytbo3 = model.Simulate(h0, enzymeSeries, boundarySeries, times)

        ' 敲除 ATP 合成酶
        model.ResetPerturbation()
        model.KnockOut("ATPS4r")
        Dim koAtps = model.Simulate(h0, enzymeSeries, boundarySeries, times)

        model.ResetPerturbation()

        Console.WriteLine("  末时刻关键表型（通量，归一化单位）：")
        Console.WriteLine($"  {"反应",-10}{"野生型",12}{"KO-CYTBO3",12}{"KO-ATPS4r",12}")
        For Each r In {"CYTBO3", "NDH1", "ATPS4r", "LDH_L", "ADH", "PDH", "CS", "ICDH"}
            Console.WriteLine($"  {r,-10}{wildType.Fluxes(last, graph.IndexOfReaction(r)),12:F4}" &
                              $"{koCytbo3.Fluxes(last, graph.IndexOfReaction(r)),12:F4}" &
                              $"{koAtps.Fluxes(last, graph.IndexOfReaction(r)),12:F4}")
        Next

        Console.WriteLine()
        Console.WriteLine("  末时刻关键代谢物（归一化浓度）：")
        Console.WriteLine($"  {"代谢物",-10}{"野生型",12}{"KO-CYTBO3",12}{"KO-ATPS4r",12}")
        For Each id In {"atp", "nadh", "lac_c", "etoh_c", "pyr", "cit"}
            Dim k = graph.IndexOfInternal(id)
            Console.WriteLine($"  {id,-10}{wildType.Concentrations(last, k),12:F4}" &
                              $"{koCytbo3.Concentrations(last, k),12:F4}" &
                              $"{koAtps.Concentrations(last, k),12:F4}")
        Next

        Console.WriteLine()
        Console.WriteLine("  解读：敲除终端氧化酶 CYTBO3 后电子传递链中断，" &
                          "模型应当自发把碳流从 TCA 转向乳酸/乙醇发酵（有氧 → 无氧重编程）。")

        ' ==================================================================
        ' 评估与导出
        ' ==================================================================
        Call Banner("评估与结果导出")

        Console.WriteLine($"  浓度拟合        : RMSE={traj.RMSE(observed):F4}  MAE={traj.MAE(observed):F4}  R²={traj.R2(observed):F4}")
        Console.WriteLine($"  通量重建        : RMSE={traj.RMSE(fluxTruth):F4}（注意列数需一致）")
        Console.WriteLine($"  稳态违反度      : mean‖S·v̂‖ = {traj.SteadyStateViolation(graph):F6}（越接近 0 越满足质量守恒）")

        ' 与真值通量的对比（形状一致时才计算）
        If traj.Fluxes.Shape.SequenceEqual(fluxTruth.Shape) Then
            Dim fluxRmse = RMSE(traj.Fluxes, fluxTruth)
            Console.WriteLine($"  通量 RMSE       : {fluxRmse:F4}")
        End If

        Call traj.SaveCsv(resultDir, "ltc_simulation")

        Console.WriteLine()
        Console.WriteLine($"  结果已导出到：{resultDir}")
        For Each f In Directory.GetFiles(resultDir)
            Console.WriteLine($"    {Path.GetFileName(f)}")
        Next

        ' ==================================================================
        ' 附加：CfC 闭式解变体的速度对比
        ' ==================================================================
        Call Banner("附加对比：CfC 闭式解变体")

        Dim cfcModel As New MetabolicLiquidNetwork(graph, LiquidMode.CFC, "cfc", seed:=42)
        cfcModel.SetTauBounds(0.5, 30.0)

        Dim cfcConfig As New MetabolicTrainerConfig With {
            .LambdaData = 1.0, .LambdaMass = 1.0, .LambdaThermo = 0.5, .LambdaFlux = 0.2,
            .LearningRate = 0.01, .Epochs = 200, .WarmupEpochs = 20,
            .GradientClip = 5.0, .LogEvery = 50, .Verbose = False, .Seed = 123
        }
        Dim cfcTrainer As New MetabolicTrainer(cfcModel, cfcConfig)

        sw.Restart()
        Dim cfcHistory = cfcTrainer.Fit(times, observed, enzymeSeries, boundarySeries, fluxTruth)
        sw.Stop()

        Dim cfcTraj = cfcTrainer.Predict(h0, times, enzymeSeries, boundarySeries)

        Console.WriteLine($"  LTC (RK4) : total loss={history(history.Count - 1).Total:F6}, RMSE={history(history.Count - 1).Total:F6}, 浓度 R²={traj.R2(observed):F4}")
        Console.WriteLine($"  CfC (闭式): total loss={cfcHistory(cfcHistory.Count - 1).Total:F6}, 浓度 R²={cfcTraj.R2(observed):F4}, 训练耗时={sw.ElapsedMilliseconds} ms")
        Console.WriteLine()
        Console.WriteLine("  readme 建议：追求动力学可解释性用 LTC，追求推理速度用 CfC。")
        Console.WriteLine($"  结构化掩码下 CfC 的参数量与 LTC 相同（{cfcModel.GetParameterCount()}），但每步只需 1 次前向求值（RK4 需要 4 次）。")

        Console.WriteLine()
        Console.WriteLine("演示结束。")
    End Sub

#Region "演示辅助"

    Private Sub Banner(title As String)
        Console.WriteLine()
        Console.WriteLine(New String("="c, 78))
        Console.WriteLine($"  {title}")
        Console.WriteLine(New String("="c, 78))
    End Sub

    ''' <summary>从 bin 目录逐级向上寻找指定名字的目录</summary>
    Private Function FindDirectoryUp(startDir As String, name As String) As String
        Dim dir As New DirectoryInfo(startDir)

        While dir IsNot Nothing
            If String.Equals(dir.Name, name, StringComparison.OrdinalIgnoreCase) Then
                Return dir.FullName
            End If
            dir = dir.Parent
        End While

        ' 退回：直接使用启动目录下的 data 子目录
        Return startDir
    End Function

    Private Function Row(mat As Tensor, rowIndex As Integer) As Tensor
        Dim width = mat.Shape(1)
        Dim v = New Tensor(width)

        For j = 0 To width - 1
            v(j) = mat(rowIndex, j)
        Next

        Return v
    End Function

    Private Function RMSE(pred As Tensor, truth As Tensor) As Double
        Dim sq As Double = 0.0
        Dim n As Integer = 0

        For i = 0 To pred.Shape(0) - 1
            For j = 0 To pred.Shape(1) - 1
                Dim d = pred(i, j) - truth(i, j)
                sq += d * d
                n += 1
            Next
        Next

        Return std.Sqrt(sq / std.Max(1, n))
    End Function

    ''' <summary>打印若干代谢物的浓度轨迹（抽取等间隔的时间点）</summary>
    Private Sub PrintTrajectory(traj As MetabolicTrajectory, ids As String(), nShow As Integer)
        Dim T = traj.Steps
        Dim stride = std.Max(1, T \ nShow)

        Console.Write($"  {"代谢物",-10}")
        For k = 0 To T - 1 Step stride
            Console.Write($"t={traj.Times(k),7:F1}")
        Next
        Console.WriteLine()

        For Each id In ids
            Dim series = traj.ConcentrationOf(id)

            Console.Write($"  {id,-10}")
            For k = 0 To T - 1 Step stride
                Console.Write($"{series(k),10:F3}")
            Next
            Console.WriteLine()
        Next
    End Sub

    ''' <summary>打印某个时刻通量最大的若干条反应</summary>
    Private Sub PrintFluxes(traj As MetabolicTrajectory, t As Integer, topN As Integer)
        Dim items As New List(Of (id As String, v As Double))()

        For j = 0 To traj.ReactionCount - 1
            items.Add((traj.ReactionIds(j), traj.Fluxes(t, j)))
        Next

        items.Sort(Function(x, y) y.v.CompareTo(x.v))

        For k = 0 To std.Min(topN, items.Count) - 1
            Console.WriteLine($"    {items(k).id,-10} v = {items(k).v:F4}")
        Next
    End Sub

    ''' <summary>打印 τ^sys 最小的若干代谢物（响应最快）</summary>
    Private Sub PrintTau(traj As MetabolicTrajectory, t As Integer, topN As Integer)
        Dim items As New List(Of (id As String, tau As Double))()

        For i = 0 To traj.MetaboliteCount - 1
            items.Add((traj.MetaboliteIds(i), traj.Tau(t, i)))
        Next

        items.Sort(Function(x, y) x.tau.CompareTo(y.tau))

        For k = 0 To std.Min(topN, items.Count) - 1
            Console.WriteLine($"    {items(k).id,-10} τ^sys = {items(k).tau:F4}")
        Next
    End Sub

#End Region

#Region "梯度自检"

    ''' <summary>
    ''' 用中心差分校验整个代谢模型（LTC 内核 + 通量读取头）的解析梯度是否正确
    ''' </summary>
    Private Sub GradientSelfCheck(graph As MetabolicNetworkGraph, times As Double(), observed As Tensor,
                                  enzymeSeries As Tensor, boundarySeries As Tensor, fluxTruth As Tensor)
        ' 只取前 6 个时间点，缩短校验耗时
        Dim shortT = std.Min(6, times.Length)
        Dim tShort = times.Take(shortT).ToArray()
        Dim obsShort = Slice(observed, shortT)
        Dim enzShort = Slice(enzymeSeries, shortT)
        Dim bndShort = Slice(boundarySeries, shortT)
        Dim fluxShort = Slice(fluxTruth, shortT)
        Dim h0Short = Row(obsShort, 0)

        Dim probe As New MetabolicLiquidNetwork(graph, LiquidMode.LTC, "rk4", seed:=7)
        Dim cfg As New MetabolicTrainerConfig With {
            .LambdaData = 1.0, .LambdaMass = 1.0, .LambdaThermo = 0.5, .LambdaFlux = 0.2,
            .Epochs = 1, .Verbose = False, .TeacherForcingStart = 0.0, .TeacherForcingEnd = 0.0
        }
        Dim probeTrainer As New MetabolicTrainer(probe, cfg)

        ' ---------- 解析梯度 ----------
        Call probeTrainer.TrainEpoch(tShort, obsShort, enzShort, bndShort, fluxShort, h0Short, 1, keepGradients:=True)

        Dim pairs = probeTrainer.GetAllParameterPairs()
        Dim analytic(pairs.Count - 1)() As Double

        For p = 0 To pairs.Count - 1
            Dim g = pairs(p).Gradient
            analytic(p) = New Double(g.Length - 1) {}
            For i = 0 To g.Length - 1
                analytic(p)(i) = g(i)
            Next
        Next

        ' 校验完成后不需要这些梯度
        Call probe.Liquid.ZeroGradients()
        Call probe.ZeroFluxGradients()

        ' ---------- 数值梯度 ----------
        Const eps As Double = 0.00001
        Dim worstRel As Double = 0.0
        Dim worstName As String = ""
        Dim totalRel As Double = 0.0
        Dim totalCount As Integer = 0

        For p = 0 To pairs.Count - 1
            Dim param = pairs(p).Value
            ' 每个参数组最多抽查 12 个元素，避免校验过慢
            Dim stride = std.Max(1, param.Length \ 12)
            Dim groupWorst As Double = 0.0

            For i = 0 To param.Length - 1 Step stride
                Dim old = param(i)

                param(i) = old + eps
                Dim lp = probeTrainer.Evaluate(tShort, obsShort, enzShort, bndShort, fluxShort).Total
                param(i) = old - eps
                Dim lm = probeTrainer.Evaluate(tShort, obsShort, enzShort, bndShort, fluxShort).Total
                param(i) = old

                Dim fd = (lp - lm) / (2 * eps)
                Dim an = analytic(p)(i)
                Dim denom = std.Max(1.0, std.Abs(fd) + std.Abs(an))
                Dim rel = std.Abs(fd - an) / denom

                groupWorst = std.Max(groupWorst, rel)
                totalRel += rel
                totalCount += 1
            Next

            If groupWorst > worstRel Then
                worstRel = groupWorst
                worstName = pairs(p).Name
            End If
        Next

        Console.WriteLine($"  校验参数组      : {pairs.Count} 组（抽查 {totalCount} 个元素）")
        Console.WriteLine($"  最差相对误差    : {worstRel:E3}  ({worstName})")
        Console.WriteLine($"  平均相对误差    : {totalRel / std.Max(1, totalCount):E3}")

        If worstRel < 0.01 Then
            Console.WriteLine("  结论            : 解析梯度与数值梯度一致，BPTT 实现正确 ✓")
        Else
            Console.WriteLine("  结论            : 解析梯度与数值梯度存在偏差，请检查 ⚠")
        End If
    End Sub

    Private Function Slice(mat As Tensor, rows As Integer) As Tensor
        Dim cols = mat.Shape(1)
        Dim out = New Tensor(rows, cols)

        For i = 0 To rows - 1
            For j = 0 To cols - 1
                out(i, j) = mat(i, j)
            Next
        Next

        Return out
    End Function

#End Region

End Module
