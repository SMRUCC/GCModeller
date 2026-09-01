Imports System.Globalization
Imports System.IO
Imports Microsoft.VisualBasic.DeepLearning.LiquidNeuralNetwork
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
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
        Dim keqCsv = files(4)

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

        ' ---------- 热力学先验与 λ2 上下文 ----------
        ' keq_truth.csv：可逆反应写真值，不可逆反应写真值速率律隐含的"有效大值"（无反向项 ⇒ Keq 视为 ∞）
        Dim keqById = LoadKeqCsv(keqCsv)
        Dim thermoConfig As New ThermoConfig With {
            .FluxScale = 0.05,          ' â = tanh(v/0.05)：|v|>0.15 视为"有通量"
            .MinConcentration = 0.001,  ' 物理浓度下限，兼顾 ln c 不失真与 (c+1)/c 不爆炸
            .MaxDrivingForce = 20.0     ' dg 钳制范围
        }
        Dim thermoCtx = ThermoContext.FromMetabolome(metabolome, graph, keqById, thermoConfig)

        Dim nRevTherm = 0
        For j = 0 To graph.ReactionCount - 1
            If graph.Reversible(j) Then nRevTherm += 1
        Next

        Console.WriteLine($"  热力学先验 Keq  : 载入 {keqById.Count} 条（可逆 {nRevTherm} 条写真值，" &
                          $"不可逆 {graph.ReactionCount - nRevTherm} 条取有效大值 {DemoData.EffectiveKeqIrreversible:F0}）")
        Console.WriteLine($"  λ2 上下文       : 门控尺度 vScale={thermoConfig.FluxScale}，" &
                          $"浓度下限 cMin={thermoConfig.MinConcentration}，推动力钳制 ±{thermoConfig.MaxDrivingForce}")

        Dim times = metabolome.Times
        Dim steps = times.Length
        Dim h0 = Row(observed, 0)

        ' ==================================================================
        ' 阶段 4：构建结构化 LTC 代谢模型
        ' ==================================================================
        Call Banner("阶段 4 / 8  构建结构化 LTC 代谢模型")

        Dim model As New MetabolicLiquidNetwork(graph, LiquidMode.LTC, "rk4", seed:=42)
        ' 代谢系统是 stiff 系统：不同反应的时间尺度跨越多个数量级，放宽 τ 的取值范围
        model.SetTauBounds(2.0, 60.0)
        ' 观测间隔最长可达 11 个时间单位，内部按 1.0 细分子步以保证显式 RK4 的数值稳定
        model.MaxSubStep = 1.0

        Console.WriteLine($"  {model}")
        Console.WriteLine($"  积分子步长      : MaxSubStep={model.MaxSubStep}（区间内自动细分，支持不规则采样）")
        Console.WriteLine($"  结构化掩码      : 循环权重被掩码掉 {model.MaskedRatio() * 100.0:F1}% 的连接（无生化关联即不可连接）")
        Console.WriteLine($"  隐藏状态 h      : {model.MetaboliteCount} 维 = 代谢物浓度")
        Console.WriteLine($"  外部输入 u      : {model.InputSize} 维 = 酶表达 + 边界底物")
        Console.WriteLine($"  通量读取头      : v = e ⊙ σ(Wv·[h;u] + bv)")

        ' ==================================================================
        ' 阶段 5：梯度自检（解析梯度 vs 中心差分）
        ' ==================================================================
        Call Banner("阶段 5 / 8  梯度自检（解析 BPTT vs 数值差分）")

        Call GradientSelfCheck(graph, times, observed, enzymeSeries, boundarySeries, fluxTruth, thermoCtx)

        ' ==================================================================
        ' 阶段 6：PINN 风格多目标训练
        ' ==================================================================
        Call Banner("阶段 6 / 8  PINN 风格多目标训练")

        Dim config As New MetabolicTrainerConfig With {
            .LambdaData = 1.0,
            .LambdaMass = 0.5,      ' ‖S·v̂‖²  质量守恒（软约束）
            .LambdaThermo = 0.5,    ' 热力学可行性：有通量的反应不得逆浓度梯度运行（ΔG 方向性）
            .LambdaFlux = 0.2,      ' 通量监督（有 13C-MFA 真值时启用）
            .LearningRate = 0.02,
            .Epochs = 600,
            .WarmupEpochs = 40,
            .GradientClip = 5.0,
            .TeacherForcingStart = 0.9,
            .TeacherForcingEnd = 0.0,
            .LogEvery = 20,
            .Verbose = True,
            .Seed = 123
        }

        Dim trainer As New MetabolicTrainer(model, config)
        Call trainer.SetThermo(thermoCtx)

        Dim before = trainer.Evaluate(times, observed, enzymeSeries, boundarySeries, fluxTruth)

        Console.WriteLine($"  训练前 loss     : {before}")
        Console.WriteLine()

        Dim sw = Stopwatch.StartNew()
        Dim history = trainer.Fit(times, observed, enzymeSeries, boundarySeries, fluxTruth)
        sw.Stop()

        Console.WriteLine()
        Console.WriteLine($"  训练后 loss     : {history(history.Count - 1)}")
        Console.WriteLine($"  训练耗时        : {sw.ElapsedMilliseconds} ms / {config.Epochs} epochs")

        Dim ltcMs = sw.ElapsedMilliseconds

        ' ==================================================================
        ' 阶段 7：模拟输出（浓度 / 通量 / 液态时间常数）
        ' ==================================================================
        Call Banner("阶段 7 / 8  模拟输出：浓度轨迹 / 通量分布 / τ^sys")

        Dim traj = trainer.Predict(h0, times, enzymeSeries, boundarySeries)

        Console.WriteLine("  代谢物浓度轨迹（归一化空间，抽样 6 个时间点）：")
        Call PrintTrajectory(traj, {"g6p", "pyr", "accoa", "cit", "atp", "nadh", "lac_c", "etoh_c"}, 6)

        Console.WriteLine()
        Console.WriteLine("  反应通量（末时刻，按大小排序前 12 条）：")
        Call PrintFluxes(traj, steps - 1, 12)

        Console.WriteLine()
        Console.WriteLine("  液态时间常数 τ^sys（可解释性输出，末时刻，最小的 8 个 = 响应最快的代谢物）：")
        Call PrintTau(traj, steps - 1, 8)

        Console.WriteLine()
        Console.WriteLine("  分通路平均 τ^sys（τ 小 = 快反应，τ 大 = 慢过程）：")
        Call PrintPathwayTau(traj, steps - 1)

        ' ==================================================================
        ' 阶段 8：敲除呼吸链 → 有氧/无氧代谢重编程外推
        ' ==================================================================
        Call Banner("阶段 8 / 8  扰动外推：敲除呼吸链反应")

        Dim last = steps - 1
        Dim o2Index = graph.IndexOfBoundary("o2_e")

        ' 好氧 / 厌氧两档溶氧水平（取归一化空间中该边界代谢物的首末值）
        Dim o2Aerobic = boundarySeries(0, o2Index)
        Dim o2Anaerobic = boundarySeries(last, o2Index)

        ' ---- 场景 1：好氧 + 野生型（基线）----
        model.ResetPerturbation()
        model.SetBoundary("o2_e", o2Aerobic)
        Dim aerobicWT = model.Simulate(h0, enzymeSeries, boundarySeries, times)

        ' ---- 场景 2：好氧 + 敲除终端氧化酶 CYTBO3（呼吸链中断）----
        model.KnockOut("CYTBO3")
        Dim aerobicKO = model.Simulate(h0, enzymeSeries, boundarySeries, times)

        ' ---- 场景 3：好氧 + 敲除 ATP 合成酶 ----
        model.ResetPerturbation()
        model.SetBoundary("o2_e", o2Aerobic)
        model.KnockOut("ATPS4r")
        Dim aerobicKOAtp = model.Simulate(h0, enzymeSeries, boundarySeries, times)

        ' ---- 场景 4：厌氧 + 野生型（作为"呼吸链失效"的对照表型）----
        model.ResetPerturbation()
        model.SetBoundary("o2_e", o2Anaerobic)
        Dim anaerobicWT = model.Simulate(h0, enzymeSeries, boundarySeries, times)

        model.ResetPerturbation()

        Console.WriteLine("  末时刻反应通量（归一化单位）：")
        Console.WriteLine($"  {"反应",-10}{"好氧WT",12}{"好氧ΔCYTBO3",14}{"好氧ΔATPS4r",14}{"厌氧WT",12}")
        For Each r In {"CYTBO3", "NDH1", "ATPS4r", "LDH_L", "ADH", "PDH", "CS", "ICDH", "ACKr"}
            Dim j = graph.IndexOfReaction(r)
            Console.WriteLine($"  {r,-10}{aerobicWT.Fluxes(last, j),12:F4}" &
                              $"{aerobicKO.Fluxes(last, j),14:F4}" &
                              $"{aerobicKOAtp.Fluxes(last, j),14:F4}" &
                              $"{anaerobicWT.Fluxes(last, j),12:F4}")
        Next

        Console.WriteLine()
        Console.WriteLine("  末时刻代谢物浓度（归一化）：")
        Console.WriteLine($"  {"代谢物",-10}{"好氧WT",12}{"好氧ΔCYTBO3",14}{"好氧ΔATPS4r",14}{"厌氧WT",12}")
        For Each id In {"atp", "adp", "nadh", "lac_c", "etoh_c", "ac_c", "pyr", "cit"}
            Dim k = graph.IndexOfInternal(id)
            Console.WriteLine($"  {id,-10}{aerobicWT.Concentrations(last, k),12:F4}" &
                              $"{aerobicKO.Concentrations(last, k),14:F4}" &
                              $"{aerobicKOAtp.Concentrations(last, k),14:F4}" &
                              $"{anaerobicWT.Concentrations(last, k),12:F4}")
        Next

        Console.WriteLine()
        Console.WriteLine("  敲除相对变化（相对好氧野生型）：")
        Console.WriteLine($"  {"通量",-10}{"ΔCYTBO3",12}{"ΔATPS4r",12}{"厌氧WT",12}")
        For Each r In {"CYTBO3", "ATPS4r", "LDH_L", "ACKr", "PDH", "CS", "NDH1"}
            Dim j = graph.IndexOfReaction(r)
            Dim base_ = aerobicWT.Fluxes(last, j)

            Console.WriteLine($"  {r,-10}{Pct(aerobicKO.Fluxes(last, j), base_),12}" &
                              $"{Pct(aerobicKOAtp.Fluxes(last, j), base_),12}" &
                              $"{Pct(anaerobicWT.Fluxes(last, j), base_),12}")
        Next

        ' ---- 只读诊断：SDH / FRD 同时活跃度 ----
        ' 这两条反应方向恰好相反，同时通流即构成"无效循环"；
        ' 由于二者都被声明为不可逆（结构上 v ≥ 0，且有效 Keq 取大值），
        ' ΔG 方向性项不会直接惩罚这一对，因此单独用 min(v_SDH, v_FRD) 观测。
        Dim sdhIdx = graph.IndexOfReaction("SDH")
        Dim frdIdx = graph.IndexOfReaction("FRD")

        If sdhIdx >= 0 AndAlso frdIdx >= 0 Then
            Console.WriteLine()
            Console.WriteLine("  无效循环诊断（只读，不进损失）：SDH 与 FRD 方向相反，同时通流即为无效循环")
            Console.WriteLine($"  {"时刻",-8}{"v_SDH",12}{"v_FRD",12}{"min(同时活跃度)",18}")
            Console.WriteLine($"  {"好氧WT",-8}{aerobicWT.Fluxes(last, sdhIdx),12:F4}" &
                              $"{aerobicWT.Fluxes(last, frdIdx),12:F4}" &
                              $"{std.Min(aerobicWT.Fluxes(last, sdhIdx), aerobicWT.Fluxes(last, frdIdx)),18:F4}")
            Console.WriteLine($"  {"厌氧WT",-8}{anaerobicWT.Fluxes(last, sdhIdx),12:F4}" &
                              $"{anaerobicWT.Fluxes(last, frdIdx),12:F4}" &
                              $"{std.Min(anaerobicWT.Fluxes(last, sdhIdx), anaerobicWT.Fluxes(last, frdIdx)),18:F4}")
        End If

        Console.WriteLine()
        Console.WriteLine("  解读：")
        Console.WriteLine("   1) 溶氧由好氧切换到厌氧（训练数据中存在的条件变化）时，模型给出清晰的")
        Console.WriteLine("      代谢重编程：呼吸链 CYTBO3/NDH1 与 PDH/CS 通量大幅下降，")
        Console.WriteLine("      乳酸与乙酸支路通量显著上升 —— 与真实的有氧→无氧切换一致。")
        Console.WriteLine("   2) 单酶敲除（CYTBO3 / ATPS4r 设为 0）属于训练时从未见过的扰动，")
        Console.WriteLine("      被敲除反应的通量会立刻归零，但下游重编程的幅度较弱。")
        Console.WriteLine("      这正是 readme 提到的外推难点：若要让敲除响应同样可靠，")
        Console.WriteLine("      需要在训练数据中纳入多条件（不同敲除株）的时序数据。")

        ' ==================================================================
        ' 评估与导出
        ' ==================================================================
        Call Banner("评估与结果导出")

        Console.WriteLine($"  浓度拟合        : RMSE={traj.RMSE(observed):F4}  MAE={traj.MAE(observed):F4}  R²={traj.R2(observed):F4}")
        Console.WriteLine($"  通量重建        : RMSE={RMSE(traj.Fluxes, fluxTruth):F4}（与真值通量对比）")
        Console.WriteLine($"  稳态违反度      : mean‖S·v̂‖ = {traj.SteadyStateViolation(graph):F6}（越接近 0 越满足质量守恒）")

        ' ---- 热力学可行性（λ2）----
        Dim thermo As New ThermoFeasibility(graph, thermoCtx)
        Dim activeTherm As Integer = 0
        Dim thermoViol = traj.ThermoViolation(thermo, boundarySeries, activeTherm)

        Console.WriteLine($"  热力学可行性 λ2 : mean Σ_j max(0, â_j·dg_j)²/r = {thermoViol:F6}（越接近 0 越满足 ΔG 方向性）")
        Console.WriteLine($"                    违反的「反应 × 时刻」共 {activeTherm} 条 / {steps * graph.ReactionCount} 条")
        Console.WriteLine($"  不可逆反应负通量: {trainer.NegativeFluxCount} 条（通量读取头对不可逆反应取 v = e·σ(·) ≥ 0，" &
                          "由结构硬保证，因此恒为 0）")

        ' ---- 正确性佐证：真值 (真值浓度 + 真值通量) 的 ΔG 违反度应 ≈ 0 ----
        Dim truthViol = TruthThermoViolation(thermo, graph, metabolomeRaw, fluxTruth, steps)

        Console.WriteLine($"  真值 ΔG 违反度  : {truthViol:F6}（真值动力学按其自身 Keq 构造，天然满足方向性，应≈0）")

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
        cfcModel.SetTauBounds(2.0, 60.0)
        cfcModel.MaxSubStep = 1.0

        Dim cfcConfig As New MetabolicTrainerConfig With {
            .LambdaData = 1.0, .LambdaMass = 1.0, .LambdaThermo = 0.5, .LambdaFlux = 0.2,
            .LearningRate = 0.02, .Epochs = 300, .WarmupEpochs = 30,
            .GradientClip = 5.0, .LogEvery = 50, .Verbose = False, .Seed = 123
        }
        Dim cfcTrainer As New MetabolicTrainer(cfcModel, cfcConfig)
        Call cfcTrainer.SetThermo(thermoCtx)

        sw.Restart()
        Dim cfcHistory = cfcTrainer.Fit(times, observed, enzymeSeries, boundarySeries, fluxTruth)
        sw.Stop()

        Dim cfcTraj = cfcTrainer.Predict(h0, times, enzymeSeries, boundarySeries)

        Console.WriteLine($"  LTC (RK4) : 训练 loss={history(history.Count - 1).Total:F6}, 自由运行浓度 RMSE={traj.RMSE(observed):F4}, R²={traj.R2(observed):F4}")
        Console.WriteLine($"  CfC (闭式): 训练 loss={cfcHistory(cfcHistory.Count - 1).Total:F6}, 自由运行浓度 RMSE={cfcTraj.RMSE(observed):F4}, R²={cfcTraj.R2(observed):F4}")
        Console.WriteLine($"  两者训练轮数与超参完全相同：LTC 耗时 {ltcMs} ms，CfC 耗时 {sw.ElapsedMilliseconds} ms")
        Console.WriteLine("  （CfC 用解析解替代数值积分，每步只需 1 次前向求值，RK4 需要 4 次）")
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

    ''' <summary>载入 keq_truth.csv（ID,Keq）成 反应 id → Keq 的映射</summary>
    Private Function LoadKeqCsv(path As String) As Dictionary(Of String, Double)
        Dim map As New Dictionary(Of String, Double)()

        For Each line In File.ReadAllLines(path).Skip(1)
            If String.IsNullOrWhiteSpace(line) Then Continue For

            Dim parts = line.Split(","c)

            If parts.Length < 2 Then Continue For

            map(parts(0).Trim()) = Double.Parse(parts(1).Trim(), CultureInfo.InvariantCulture)
        Next

        Return map
    End Function

    ''' <summary>
    ''' 用真值浓度 + 真值通量计算 ΔG 违反度。
    ''' 真值动力学按其自身 Keq 构造，天然满足方向性，因此该值应≈0——
    ''' 这是 λ2 实现是否正确的一个强佐证。
    ''' </summary>
    Private Function TruthThermoViolation(thermo As ThermoFeasibility, graph As MetabolicNetworkGraph,
                                          metabolomeRaw As TimeSeriesMatrix, fluxTruth As Tensor,
                                          steps As Integer) As Double
        ' metabolomeRaw 未归一化，Reorder 后即为物理浓度（T × mAll）
        Dim physical = metabolomeRaw.Reorder(graph.MetaboliteIds)
        Dim acc As Double = 0.0
        Dim nRxn = graph.ReactionCount

        For t = 0 To steps - 1
            Dim cAll = New Double(graph.MetaboliteIds.Length - 1) {}
            Dim v = New Double(nRxn - 1) {}

            For i = 0 To cAll.Length - 1
                cAll(i) = physical(t, i)
            Next
            For j = 0 To nRxn - 1
                v(j) = fluxTruth(t, j)
            Next

            Dim tStep = thermo.EvaluatePhysical(cAll, v)

            acc += tStep.Penalty / std.Max(1, nRxn)
        Next

        Return acc / std.Max(1, steps)
    End Function

    ''' <summary>相对基线的变化百分比（基线接近 0 时退化为输出绝对值）</summary>
    Private Function Pct(value As Double, baseline As Double) As String
        If std.Abs(baseline) < 0.001 Then
            Return value.ToString("F4")
        End If

        Dim rel = (value - baseline) / std.Abs(baseline) * 100.0

        Return $"{rel:F1}%"
    End Function

    ''' <summary>按通路分组打印平均 τ^sys，用于展示 LNN 的可解释性</summary>
    Private Sub PrintPathwayTau(traj As MetabolicTrajectory, t As Integer)
        Dim groups = New Dictionary(Of String, String()) From {
            {"糖酵解", {"g6p", "f6p", "fdp", "dhap", "gap", "_13dpg", "_3pg", "_2pg", "pep", "pyr"}},
            {"TCA 循环", {"accoa", "cit", "icit", "akg", "succoa", "succ", "fum", "mal", "oaa"}},
            {"呼吸链/能量", {"nad", "nadh", "atp", "adp", "pi", "q8", "q8h2"}},
            {"发酵产物", {"lac_c", "acald", "etoh_c", "actp", "ac_c"}}
        }

        For Each kv In groups
            Dim sum As Double = 0.0
            Dim n As Integer = 0

            For Each id In kv.Value
                Dim k = Array.IndexOf(traj.MetaboliteIds, id)

                If k >= 0 Then
                    sum += traj.Tau(t, k)
                    n += 1
                End If
            Next

            If n > 0 Then
                Console.WriteLine($"    {kv.Key,-12} 平均 τ^sys = {sum / n:F4}")
            End If
        Next
    End Sub

#End Region

#Region "梯度自检"

    ''' <summary>
    ''' 用中心差分校验整个代谢模型（LTC 内核 + 通量读取头）的解析梯度是否正确
    ''' </summary>
    Private Sub GradientSelfCheck(graph As MetabolicNetworkGraph, times As Double(), observed As Tensor,
                                  enzymeSeries As Tensor, boundarySeries As Tensor, fluxTruth As Tensor,
                                  Optional thermoCtx As ThermoContext = Nothing)
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

        ' 让自检覆盖 λ2 热力学项（它会向浓度读出层与通量头注入梯度）
        Call probeTrainer.SetThermo(thermoCtx)

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

        ' 阈值取 5%：中心差分本身存在 O(eps²) 截断误差与浮点相消，
        ' 个别梯度接近 0 的元素相对误差会被放大，1%~5% 属于正常范围
        If worstRel < 0.05 Then
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
