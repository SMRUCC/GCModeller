# CellularAI

在**未知细胞类型**中预测转录组对基因扰动的响应，并为每个扰动合成非负整数计数的单细胞。

本程序集是 CellularAI 算法的**纯 CPU、纯内存**实现：输入是上游装配好的内存对象，输出是预测计数矩阵
（`CsrMatrix`）与一份诊断报告（`PredictionReport`）。**不做任何文件 IO**——parquet / `.h5ad` / `.vcc`
的读写全部留给调用方。

---

## 1. 定位与边界

| 项 | 约定 |
|---|---|
| 输入 | 控制细胞矩阵（`CsrMatrix`，细胞 × 基因）+ 训练摘要（`SummaryBundle`）+ 模型参数（`CellularAiModel`） |
| 输出 | `CsrMatrix`（每扰动一个块，块内 细胞 × 基因，非负整数且 < 2²⁴）+ `PredictionReport` |
| 文件 IO | **不做**。不解析 YAML、不读 parquet、不写 `.h5ad`、不打包 `.vcc` |
| 配置 | `configs/example.yaml` 的已发布超参数成为 POCO 的默认值 |
| 计算 | CPU 串行为正确性基线；并行只做"每扰动只写自己槽位"，结果与并行度无关；GPU（`ILCudaTensor`）留作后期可选后端 |

## 2. 七阶段流程

| 阶段 | 命名空间 | 做什么 | 关键输出 |
|---|---|---|---|
| 1 摘要池化 | `CellularAI.Summary`、`CellularAI.Evidence` | 轴对齐、带符号 z（Liptak）、三维加权（记录 × 基因 × 扰动）的随机效应合并 | `ContextEvidence`：`L2`/`Z`/`L2Tot`/`ZTot` + 护栏快照 + 未测靶列表 |
| 2 响应计划 | `CellularAI.Planning` | 相似度权重（softmax + 上限水位）、计划显著性、曲线尺寸 / 功效重排 / 相近度抬升 | 每扰动的调用基因、方向、优先级与非计划基因的微调方向 |
| 3 残差校正 | `CellularAI.Residual` | 低秩响应基（随机化 SVD）+ 逐基因多响应岭回归（行掩码）+ 形状约束 | 与上下文相关的剖面修正 |
| 4 总量设计 | `CellularAI.Design` | 剖面离散度、边距/幅度求解、控制程序平滑与加权、最大余数分摊、信用固定点 | 每扰动逐基因的整数总量 |
| 5 计数合成 | `CellularAI.Synthesis`、`CellularAI.Placement` | 分位取值 + 质量守恒、精确秩和 z、平移二分、copula 相关系数搜索、突发散布 | 每扰动的细胞计数块 |
| 6 评分 | `CellularAI.Scoring` | DE 表（精确秩和 + BH）与 VCC 六项指标 | `pds`/`expr_mse`/`fid`/`reach`/`jac`/`nmae` + 锚点总分 |
| 7 门控 | `CellularAI` | 结构、守恒、收敛、信用区间、护栏、评分逐项校验 | `PredictionReport.Accepted` 与逐项 `Gates` |

> 设计层与评分层共用 `Scoring.Metrics.TS = 5e4` 与同一份"剖面离散度"实现——评分函数是模型的一部分，
> 两份实现一旦漂移，边距求解就会收敛到评分层不认可的点上。

## 3. 快速开始

```vb
Imports CellularAI
Imports CellularAI.Model
Imports CellularAI.Summary
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix.Compressed

' 1) 参数：全部默认值对齐参考实现，可按需覆盖
Dim model As CellularAiModel = CellularAiModel.Default()

' 2) 输入（由上游装配，本库不读文件）
Dim controls As CsrMatrix = ...        ' 细胞 × 基因
Dim bundle As SummaryBundle = ...      ' 训练摘要栈
Dim genes As String() = ...
Dim perturbations As String() = ...

' 3) 端到端预测
Dim result As ContextPipeline.PredictionResult = ContextPipeline.Predict(
    controls, genes, perturbations, bundle, model,
    medianDegs:=50.0,          ' median_degs
    callDepth:=20000.0,        ' calls_depth
    context:="A549",
    nCells:=400,
    seed:=20261126)

Dim blocks As CsrMatrix() = result.Blocks        ' 每扰动一块
Dim matrix As CsrMatrix = result.Matrix          ' 全部扰动按序叠放
Dim report As PredictionReport = result.Report

Console.WriteLine($"accepted={report.Accepted}, margin={report.Margin:G4}, credit={report.CreditShare:G4}")

For Each gate As GateResult In report.Gates
    Console.WriteLine($"  {gate.Name,-14} {gate.Passed}  {gate.Detail}")
Next
```

需要 VCC 指标时，把参考侧统计一起传进去：

```vb
Dim scored = ContextPipeline.Predict(controls, genes, perturbations, bundle, model,
                                     medianDegs:=50.0, callDepth:=20000.0,
                                     reference:=referenceSide)

Dim six = scored.Report.Scores        ' pds / expr_mse / fid / reach / jac / nmae
Dim total = scored.Report.Total       ' 0~100 的锚点总分
```

单独计算指标（不跑整条流水线）：

```vb
Imports CellularAI.Scoring

Dim predicted As SideStats = Metrics.BuildSide(blocks, perturbations, genes,
                                               Function(g) engine.SortedCpm(g),
                                               Function(g) engine.TieSum(g))
Dim scores = Metrics.Score(predicted, referenceSide, panelTargets:=perturbations)
Dim total As Double = Metrics.AnchorScore(scores)
```

## 4. 公开 API

| 命名空间 | 主要类型 | 说明 |
|---|---|---|
| `CellularAI` | `ContextPipeline`、`PredictionReport`、`GateResult`、`Guards` | 端到端入口、报告契约、护栏计数 |
| `CellularAI.Model` | `CellularAiModel` + 7 个 Options | 超参数契约（默认值 = 参考配置），全部支持 `Clone` |
| `CellularAI.Summary` | `SummaryBundle`、`SummaryCatalog`、`SummaryRow`、`OffsetTable` | 内存版摘要栈；`AlignPanel` / `SignedZ` / `ApplyOffsets` / `StackEffects` / `TrainingRows` |
| `CellularAI.Evidence` | `MetaPooling`、`PoolingFactors`、`FactorExpression`、`ContextEvidence` | 三维加权池化、因子表达式求值 |
| `CellularAI.Planning` | `SimilarityWeights`、`CallSizing`、`CallPlan`、`CallEntry` | 相似度权重、尺寸三步、成员选择 |
| `CellularAI.Residual` | `ResponseBasis`、`ControlResidual`、`EffectShaping` | 随机化 SVD 基、逐基因岭回归、比值限幅 / 符号保持 / 一致性融合 |
| `CellularAI.Design` | `ProfileSpread`、`MarginSolver`、`ControlPcs`、`ShapedDesign`、`TotalsDesign` | 离散度、边距与幅度、控制主成分、整数总量与固定点 |
| `CellularAI.Placement` | `SynthesisJob` | 单扰动合成作业（总量、文库、放置目标、方向、种子） |
| `CellularAI.Synthesis` | `SynthesisEngine`、`WilcoxonKernel`、`QuantileBuilder`、`BlockSolver`、`BlockPlacement`、`CellSynthesizer` | 控制侧状态、精确 z、分位取值、平移二分、三段式放置求解、逐扰动合成 |
| `CellularAI.Scoring` | `Metrics`、`SideStats`、`DeTable`、`DeRow` | 六项指标、DE 表、锚点缩放 |
| `CellularAI.Randomizer` | `Pcg64`、`PerturbationSeed` | 可播种随机源；扰动种子派生 |
| `CellularAI.SingleCell` | `SingleCellCounts`、`CountStatistics` | 计数矩阵校验、chunked CPM、jackknife 矩（本库自带，保持稳定） |

## 5. 参数契约（节选默认值）

| 分组 | 参数 | 默认值 |
|---|---|---|
| `Pooling` | `L2Row` / `L2Tot` / `Z` 因子表达式 | `inv:mad:dl` / 同类 / `sef*kd` |
| `Calls` | `EligibleCpm` / `MinLfc` / `NMin` / `NMax` | 5 / 0.2 / 50 / 2000 |
| `Plan` | `SimNtop` / `SimCap` / `PlanZUniform` | 3000 / 0.25 / 0 |
| `Totals` | `ResidualRank` / `ResidualRidge` / `ResidualMinMeasured` / `PwK` | 160 / 10.0 / 32 / 100 |
| `Totals` | `AmpShare` / `Mix` / `ShapeGamma` / `ShapeKz` / `GateZ` | 0.9 / 0.5 / 0.8 / 16 / 5 |
| `Totals` | `CreditDraws` / `CreditSeed` / `FixedSteps` / `FixedRtol` | 64 / 7 / 3 / 1e-3 |
| `Synthesis` | `ZPlan` / `ZNudge` / `RMax` / `NBis` / `NeBurst` | (5,12) / 2 / 0.99 / 5 / 4 |
| `Synthesis` | `ShiftBand` / `SolveTol` / `SolveIters` / `SparsifyStep` | (−0.6, 1.6) / 0.05 / 40 / 0.025 |
| `Guards` | `L2AbsMax` / `ZAbsMax` / `Log2Max` / `VFloorRel` | 6.25 / 187.5 / 12.5 / 0.0024 |
| `Guards` | `CreditBandLo` / `CreditKink` / `MaxUnmeasured` | 0.975 / 1.001 / 0 |

## 6. 确定性

- 随机源：`Pcg64`（热路径 `xoshiro256+`，种子用 SplitMix64 扩散），**同一 seed 逐位一致**。
- 扰动种子：`PerturbationSeed.Combine(taskSeed, 扰动名)`（CRC32 派生），与扰动顺序、并行度无关。
- 合成验收：同 seed 两次运行**计数矩阵逐元素相等**；每基因总量精确等于设计总量；全部计数为非负整数且 < 2²⁴。
- 跨行归约一律单线程（浮点加法不可交换）；并行只用于"每扰动 / 每基因只写自己槽位"。

## 7. 门控项

`ContextPipeline` 在返回前逐项校验，任一失败即 `Accepted = False` 并列出原因：

`axes`（块数 / 行数 / 列数）、`integral`（非负整数）、`count_limit`（< 2²⁴）、`credit_band`
（`realized/target ∈ [0.975, 1.001]`）、`converged`（信用固定点）、`margin`（边距命中）、
`unmeasured`（未测靶数）、`guards`（护栏命中率上限）、`fid`（提供参考侧时）。

## 8. 测试与审计

审计使用 `Microsoft.VisualBasic.ApplicationServices.Debugging.DebugAudit`，输出 JSON 报告。
测试工程位于**兄弟目录** `../CellularAI.test/`（库工程未排除 `test\**`，放项目内会被编进程序集）。

```bash
dotnet build CellularAI.test/CellularAI.test.vbproj -c Debug
dotnet run --project CellularAI.test/CellularAI.test.vbproj -- m0   # 参数契约 / 护栏 / 轴对齐
dotnet run --project CellularAI.test/CellularAI.test.vbproj -- m1   # 池化：手算对拍 + tau2 交叉验证
dotnet run --project CellularAI.test/CellularAI.test.vbproj -- m2   # 计划：权重 / 尺寸 / 成员
dotnet run --project CellularAI.test/CellularAI.test.vbproj -- m3   # 残差：基正交 / 限幅 / 符号
dotnet run --project CellularAI.test/CellularAI.test.vbproj -- m4   # 设计：边距 / 总量精确 / 信用
dotnet run --project CellularAI.test/CellularAI.test.vbproj -- m5   # 合成：守恒 / 同 seed 一致 / 精确 z
dotnet run --project CellularAI.test/CellularAI.test.vbproj -- m6   # 评分：完美=1 / 噪声退化
dotnet run --project CellularAI.test/CellularAI.test.vbproj -- m7   # 端到端：门控与可复现
```

报告落在 `<BaseDirectory>/cellularai-m0-report.json` … `cellularai-m7-report.json`；退出码 0 表示全部断言通过。

## 9. 性能与内存（G=18000，P=1024，T≈10，n_cells=400）

| 阶段 | 估计代价 | 策略 |
|---|---|---|
| 池化 | ~2×10⁸ 次乘加 | 逐基因外循环、逐扰动内循环，因子常驻 |
| 残差 SVD | ~2.5×10¹⁰ flops | 走 `RandomizedSVD`（ℓ=k+8，2 次幂迭代，不物化中间矩阵）；残差栈按扰动分块 |
| 控制 PCA | ~1.3×10¹⁰ flops | 走 `TruncatedPCA`（中心化表达成算子，不复制 6000×18000） |
| 计数合成 | 主导项 | 按扰动 `Parallel.For`；控制侧序统计惰性缓存并只读共享 |

> 小规模数据上边距求解的离散度台阶较粗，可能落不进信用带而让 `credit_band` 判失败——这是门控的
> **预期行为**（宁可拒绝，也不交出不可信的结果），不是缺陷。

## 10. 依赖的 sciBASIC# 组件

| 需求 | 组件 |
|---|---|
| 随机化 SVD / 截断 PCA | `LinearAlgebra.Matrix.RandomizedSVD` / `TruncatedPCA` |
| 逐基因多响应岭回归（行掩码） | `Bootstrapping.Ridge.MultiResponseRidge` |
| DL τ² / 逆方差权重 / 加权均值 / Stouffer | `MetaAnalysis.RandomEffects` / `EvidenceCombination` |
| 双侧 P ↔ 带符号 z | `Statistics.NormalDistribution` |
| BH 校正 | `Math.FalseDiscoveryRate` |
| CSR / 计数校验 | `LinearAlgebra.Matrix.Compressed.*` |
| jackknife 矩 / CPM 均值 | `CellularAI.SingleCell.CountStatistics` |

## 11. 与参考实现的差异

| 项 | 说明 |
|---|---|
| 随机源（Level A） | 热路径用 `xoshiro256+`、种子用 SplitMix64（规避 VB 的整数溢出检查），类型名仍为 `Pcg64`；**不追求**与 numpy `default_rng` 的位流逐位一致，只保证统计等价 + 同 seed 自一致 |
| 残差岭回归入口 | 逐基因调用 `MultiResponseRidge.Fit`（内存 `O(T×k)`），而非一次性 `BatchFit` 的 `基因×记录×秩` 三维数组（18000×10×160 ≈ 230 MB） |
| 池化结果类型名 | `ContextEvidence`（避免与 `MetaAnalysis.PooledEvidence` 冲突） |
| 精确秩和 | 合成内层走 `WilcoxonKernel`（float32 栅格 + 游程），而非逐基因建字典的 `Wilcoxon.TestTable` |
| 项目引用 | 新增对 `linear-netcore5.vbproj`（多响应岭回归）的引用 |

## 12. 后续

- **M8（可选）GPU 异构**：抽象 `IComputeBackend`，复用 `ILCudaTensor` 的 `CellPooling.Pool` 与
  `CellSynthesis.{CopulaQuantiles, QuantileMass, ShiftBisect, Burst}`；总量设计 / 固定点循环 / 外层
  `r` 搜索留在主机侧；`n_cells > 1024` 回退 CPU。CPU 实现始终保留为参考与回退路径。
