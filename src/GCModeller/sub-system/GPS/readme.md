# GPS：GNN + Transformer 基因表达调控网络模型

基于 **先验调控网络（TF + motif）** 与 **时序基因表达矩阵** 构建动态 GRN 模型，
预测下一时刻全基因组表达，并支持敲除 / 敲低 / 过表达、组合扰动与多步级联的
**虚拟扰动实验**。实现方法见同目录的 `method.md`（GraphGPS + Graphormer + GEARS 范式）。

## 架构

```
               时序表达矩阵 X[N×T]        先验调控网络 G_prior
                        │                        │
                        ▼                        ▼
         预处理(CPM/log/z-score/高变异过滤)   基因调控图 + 图编码
                        │                 (SPD 桶 / LapPE / 入出度)
                        └──────────┬─────────────┘
                                   ▼
        初始嵌入 h⁰ = x·W_expr + e_gene + PE·W_pe + z_degIn + z_degOut
                                   ▼
        ┌──────────── GPS 融合层 × L（参数跨时间步共享）────────────┐
        │  局部消息传递 MPNN（边类型感知稀疏聚合）                  │
        │        ∥ 相加融合                                        │
        │  全局自注意力（多头 + 可学习 SPD 空间偏置）                │
        │        → FFN → 残差 / LayerNorm                          │
        └──────────────────────────────────────────────────────────┘
                                   ▼
                  批量 GRU（batch = 基因数，沿时间编码）
                                   ▼
                  基因特异性线性解码（每基因独立读出）
                                   ▼
                  下一时刻表达 + 可选不确定性 σ
```

| 组件 | 实现 | 说明 |
|---|---|---|
| 局部消息传递 | `GEARS.Layers.GEARSConvLayer` | 复用既有边类型感知稀疏图卷积（含手写反向），层内激活关闭、由本层在双精度域应用 tanh |
| 全局注意力 | `Model.BiasedMultiHeadAttention` | 新写：`score = QKᵀ/√d + biasTable[SPD(i,j), head]`，含偏置表 scatter-add 反向 |
| 融合层 | `Model.GPSLayer` | MPNN ∥ Attention 并行相加 → FFN → AddNorm（不复用串联结构的 `EncoderLayer`） |
| 时序编码 | `Model.BatchGRU` | 新写双精度 GRU（`GNN.GRULayer` 的门控用 `CSng` 会降到 float32） |
| 解码 | `Model.PerGeneDecoder` | 每基因独立权重 `[N, d]`，避免共享解码器抹平异质性 |
| 参数与优化 | `Model.ParameterStore` | 全模型统一 Adam + 全局梯度范数裁剪；支持外部梯度张量（复用图卷积层） |
| 两阶段训练 | `Training.GPSTrainer` | 掩码重建预训练 → 下一时刻微调；按时间切分早停与最优参数回滚 |
| 评估 | `Training.Evaluator` | MSE / R² / Pearson / top-K 困难基因 MSE / 方向准确率 / **图外基因子集** R²；含历史持续性基线 |
| 虚拟扰动 | `Perturbation.VirtualPerturbationEngine` | 单基因 / 组合（Bliss 加性对照 + 协同效应）/ 多步级联 |
| 门面 | `GPSModel` | 实现 `BNLearn.InsilicoPerturbationExperiment`，与贝叶斯网络、GEARS 实现可互换 |
| 持久化 | `GPSFile` | 保存/加载 `*.gps.zip`：配置 + 表达矩阵 + 先验网络 + 参数 + 训练曲线 |

## 快速开始

```vb
Imports SMRUCC.genomics.Analysis.BNLearn.Core
Imports SMRUCC.genomics.Analysis.GPS
Imports SMRUCC.genomics.Analysis.GPS.Config

Dim prior As PriorNetwork = PriorNetwork.ParseJson(File.ReadAllText("prior.json"))
Dim expression As GeneExpressionData = ...   ' 行=基因，列=样本，需带 TimePoints

Dim config As New GPSConfig With {
    .MaxGenes = 1500,        ' 第一期建议 ≤ 2000（全局注意力为 O(N²)）
    .Window = 5,
    .DModel = 128,
    .NLayers = 3,
    .NHeads = 8,
    .FfnDim = 256,
    .GruHidden = 128
}

Dim gps As New GPSModel(expression, prior, config)
Dim training = gps.Train()                    ' 预训练 + 微调
Console.WriteLine(gps.Evaluate().ToString())  ' 严格按时间切分的测试集指标
Console.WriteLine(gps.EvaluateBaseline().ToString())  ' 历史持续性基线

' ---- 虚拟扰动（InsilicoPerturbationExperiment） ----
Dim ko = gps.KnockoutGene("TF1")
For Each item In ko.GetTopChangedGenes(20)
    Console.WriteLine($"{item.GeneName}  Δ={item.FoldChange:F4}")
Next

Dim combo = gps.PredictCombination({"TF1", "TF2"})
Console.WriteLine($"协同分数 = {combo.SynergyScore:F4}")

Dim cascade = gps.SimulateCascade("TF1", steps:=5)
' cascade.DynamicTrajectory 为 [时间步, 基因]
```

## 混合精度

- **参数、LayerNorm 统计量、GRU 门控、损失、优化器状态**：双精度。
- **GEMM 热点**（QKV 投影、FFN、GRU 门控、解码）：经 `TensorOps` / `MixedPrecisionOps`，
  规模达到 `MixedPrecisionPolicy.GemmOpsThreshold`（默认 262144 = m·k·n）时
  临时降单精度计算再升回双精度。
- 整体回退：`MixedPrecisionPolicy.Enabled = False`。
- 可观测：`MixedPrecisionDiagnostics.Report()` 给出每个算子的加速/回退计数与命中率。

阈值是经验起点，建议在目标机器上用真实模型形状校准后再固定。

## GPU 加速

本工程**不引用** `ILCudaTensor`：GPU 后端由宿主（或测试工程）注册，两条精度栈共享同一个 CUDA 引擎：

```vb
Imports Microsoft.VisualBasic.Computing.ILCuda.Runtime
Imports ILCudaTensor.GPUTensor

Dim engine = CudaEngine.TryCreate()
CudaTensorF.Register(engine)     ' 单精度栈：TensorF.computeKernelF（承载混合精度热点）
CudaTensor.Register(engine)      ' 双精度栈：Tensor.computeKernel
```

注意：
- 未做 `PinDevice` 时每个算子输出都会 D2H 回读，单精度算子链可能反而慢于双精度 SIMD；
  常驻权重应显式钉驻显存。
- 消费级显卡 FP64 吞吐约为 FP32 的 1/64，热点走单精度栈收益最明显。
- 小规模输入（低于后端阈值，如 `MinGemmElementsF = 65536`）会自动回退 CPU。

## 规模约束与后续工作

| 事项 | 当前（第一期） | 二期 |
|---|---|---|
| 基因数 | ≤ 2000（高变异过滤，保留先验 TF） | 稀疏/线性注意力（Exphormer / Performer）支持全基因组 |
| SPD 偏置内存 | `[heads, N, N]` 物化，N=2000、8 头约 128 MB | 分块注意力 / 稀疏偏置 |
| 可学习邻接矩阵 | 固定 `A_prior`（method.md §5 建议先固定） | 学习式图结构 + L1/先验距离正则 |
| LapPE | 稠密特征分解，N > 1500 自动置零 | 随机游走 / 稀疏近似位置编码 |
| 不确定性 | 已实现 softplus 方差头与高斯 NLL | 校准与置信区间报告 |
| 梯度检查点 | 反向按时间步重算前向（多一次前向） | 显式 stateful 层 API 以缓存全部时间步 |

## 模型持久化

```vb
' 保存（训练完成后）
Call GPSFile.Save(model, "model.gps.zip")

' 加载（重建预处理、调控图、图编码、滑窗数据集与全部参数）
Dim loaded As GPSModel = GPSFile.Load("model.gps.zip")

' 只读清单（不加载参数）
Dim manifest As GPSPackageManifest = GPSFile.ReadManifest("model.gps.zip")
```

包内条目与设计要点：

| 条目 | 内容 |
|---|---|
| `manifest.json` | 格式/版本/规模/训练概要 |
| `config.json` | 全部超参数（按属性名反射） |
| `expression.json` | 基因名、样本名、时间点、表达矩阵 |
| `prior.json` | 先验调控网络 |
| `parameters.tsv` | 参数名 + 形状 + 数值（`G17`，double 往返无损） |
| `training.json` | 训练损失曲线与最优 epoch |

只保存**输入与参数**，派生结构（预处理、调控图、SPD/LapPE 编码、滑窗切分）在加载端用同一套代码
确定性重算，因此不会出现"包里的图编码与训练时不一致"的隐患；加载时会逐个校验参数名与长度，
不匹配立即报错。保存采用"临时文件 + 原子替换"，避免写坏已有包。

## DEMO（TestData1 真实数据）

`test` 工程内置了基于 `sub-system/demo/TestData1` 的两阶段 DEMO：

```text
cd test
dotnet run --project GPS.Test.vbproj -c Debug -p:Platform=x64 -- --demo               ' 训练+保存，然后加载+分析
dotnet run --project GPS.Test.vbproj -c Debug -p:Platform=x64 -- --demo --train-only   ' 只训练并保存
dotnet run --project GPS.Test.vbproj -c Debug -p:Platform=x64 -- --demo --analyze-only ' 只加载并分析
```

- 训练阶段：加载数据 → 训练 → 评估 → `GPSFile.Save` → **往返校验**（重新加载并比对敲除预测，最大差异 0）。
- 分析阶段：`GPSFile.Load` → 虚拟敲除 → Limma 差异分析 → Fisher 通路富集。

可用参数：`--epochs`、`--pretrain`、`--tf`、`--samples`、`--noise`、`--topn`、`--lr`、
`--logfc`、`--fdr`、`--seed`、`--data`、`--out`、`--model`、`--train-only`、`--analyze-only`
（`--help` 查看完整说明）。

结果写入 `test/demo_output/`：`gps_model.gps.zip`、`train_report.txt`、`analysis_report.txt`、
`limma_all/up/down.tsv`、`enrichment_all/up/down/significant.tsv`。
注意：通路注释（`pathway_info.json`）不属于模型状态，`--analyze-only` 仍需 `--data` 指向数据目录。

说明：虚拟扰动本身是确定性点估计，DEMO 为满足 Limma 的方差估计需要，
用**实测同时间点重复间 SD** 作为噪声尺度构造 in-silico 技术重复；
logFC 处于模型处理后的 log2 表达尺度。DEMO 默认训练轮数较少（约 260 步），
主要用于验证分析链路，不代表模型在真实数据上的最终精度。

## 自测

```text
cd test
dotnet run --project GPS.Test.vbproj -c Debug -p:Platform=x64
```

覆盖：
1. 预处理（CPM/log/z-score、高变异过滤、反标准化）
2. 滑窗数据集（重复样本聚合、严格时间切分、窗口形状）
3. 图编码（SPD 对称性与桶划分、度编码、LapPE 单位范数与可复现性）
4. **有限差分梯度校验**：偏置注意力、MPNN、GPS 融合层、批量 GRU、完整 GRN 模型（参数 + 输入）
5. 端到端训练冒烟（损失下降、参数更新、指标与基线）
6. `InsilicoPerturbationExperiment` 三方法、组合扰动、级联轨迹、未知基因降级
7. 混合精度对拍（单精度热点命中计数 > 0，输出相对 L2 差 ≤ 1e-4）
