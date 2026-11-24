---
name: GeneTree_ResidualEdgeDiscovery
overview: 为 GeneTree 增加"残差二阶段"结构发现开关：先用 ARACNE 边拟合，再在残差上对用户指定的候选基因集合做相关筛选，候选逐个增量重拟合并以 ΔR² 决定是否接受为新调控边；ARACNE 边始终保留，新增边标记为 discovered 并输出 discovery.tsv，同时提供 R# API（discovery / discovered_edges）与存档持久化。
todos:
  - id: config-discovery-switch
    content: 扩展 GeneTreeConfig：新增 EdgeDiscoveryMode、8 个发现参数、校验与 ToString、SetDiscoveryCandidates
    status: completed
  - id: residual-screening
    content: 新增 EdgeDiscovery.vb：候选向量标准化、残差 Pearson 筛选、DiscoveryEdge/DiscoveryReport 与 discovery.tsv 写出
    status: completed
    dependencies:
      - config-discovery-switch
  - id: trainer-two-stage
    content: 在 GeneTreeTrainer.TrainGene 实现残差二阶段增量重拟合与 ΔR² 接受，RegulatorImportance 增 source/deltaR2
    status: completed
    dependencies:
      - residual-screening
  - id: report-storage
    content: TrainingReport 与 GeneTreeStorage 增发现报告、importance 新列、manifest 键与 candidates/discovery 条目（旧档兼容）
    status: completed
    dependencies:
      - trainer-two-stage
  - id: facade-demo
    content: XGBoostGRN 暴露 Discovery/DiscoveredEdges，GeneTreeDemo 增加 --discovery-candidates 参数
    status: completed
    dependencies:
      - report-storage
  - id: rsharp-api
    content: R# geneTreeTools 新增 discovery()/discovered_edges()，training_stats 增发现计数并更新包文档
    status: completed
    dependencies:
      - facade-demo
  - id: build-verify
    content: 按 Rsharp_app_release|x64 编译 GCModeller.slnx，并做 max_genes 小规模冒烟核验 ΔR²、splits 与 discovery.tsv
    status: completed
    dependencies:
      - rsharp-api
---

## 产品概述

为 GeneTree（基于 XGBoost 的非线性基因调控网络模拟器）增加“从表达数据中发现新调控边”的能力。当前模型只能使用 ARACNE 提供的固定拓扑：每个靶基因的特征集合完全由 `edges.csv` 经 MI top-K 裁剪得到，XGBoost 的分裂次数与灵敏度仅能对已有边排序/剪枝，无法新增边。

## 核心功能

- **开关式结构发现**：新增 `EdgeDiscovery` 模式（默认 `AracneOnly` 保持现状，开启为 `ResidualTwoStage`），只有显式开启时才执行发现流程。
- **残差二阶段流程**：第一阶段仅用 ARACNE 边拟合；第二阶段在残差上对用户给定的候选基因集合做相关搜索，把候选逐个增量加入并重拟合，以验证集 R² 的增量（ΔR²）决定是否接受为新调控边。
- **候选集由调用方指定**：新增 R# 函数，接受一个基因 id 字符向量作为候选调控者，不自动猜测候选范围。
- **只增不减**：ARACNE 原有边始终保留，发现边按接受顺序追加在特征末尾，保证与旧模型可比。
- **结果可见**：新增 `discovery.tsv`（gene、regulator、source、correlation、delta_r2、splits、sensitivity 等），`importance.tsv` 增加 source/delta_r2 列，manifest 记录发现模式与候选集；扰动网络自动包含发现边（被发现的调控者可获得出边，从而让此前无下游的靶基因具备传播能力）。
- **默认零开销**：未开启时不增加任何计算与文件输出，旧存档加载行为不变。

## 技术栈

- 语言/框架：VB.NET（net10.0），项目 `G:\GCModeller\src\GCModeller\sub-system\GeneTree\GeneTree.vbproj`，`RootNamespace = SMRUCC.genomics.Analysis.GeneTree`（源文件内不写 Namespace 块）。
- 学习器：现有 TGBoost `train.GBM`（`squareloss` + `Metrics.mse` + `maximize:=False`），经 `XGBoostDataSet.Conversion.ToTrainingSet/ToValidateSet` 构造训练数据；本方案**不修改 xgboost 项目**。
- 数据：`SMRUCC.genomics.Analysis.HTS.DataFrame.Matrix`（`Matrix.LoadData` 读 CSV，行=基因、列=细胞）。
- R# 前端：`G:\GCModeller\src\workbench\R#\biosystem\geneTreeTools.vb`（`ExportAPI` + `RApiReturn` + `RInternal.debug.stop` + `CLRVector.asCharacter`）。

## 实施方案

### 1) 配置开关（`GeneTreeConfig.vb`）

新增枚举 `EdgeDiscoveryMode { AracneOnly = 0, ResidualTwoStage = 1 }` 与配置项：

| 参数 | 默认 | 含义 |
| --- | --- | --- |
| `EdgeDiscovery` | `AracneOnly` | 总开关，默认关闭，行为与现状逐位一致 |
| `DiscoveryCandidates As String()` | 空 | 用户给定的候选调控者 id 集合 |
| `DiscoveryMaxPerGene` | 2 | 每个靶基因最多接受几条新边 |
| `DiscoveryMaxCandidates` | 5 | 每个靶基因最多尝试几个候选（控制重拟合次数） |
| `DiscoveryMinCorrelation` | 0.10 | 残差与候选的相关性绝对值下限 |
| `DiscoveryMinDeltaR2` | 0.002 | 接受一条新边所需的最小验证集 R² 增益 |
| `DiscoverySampleSize` | 1000 | 计算残差相关时采样的细胞数（0 表示用全部训练细胞） |
| `DiscoveryMaxGenes` | 0 | 最多对多少个靶基因做发现（0 = 全部，用于试跑限流） |


同步扩展 `GetValidationErrors`（模式合法、开启时候选集非空、阈值区间、`MaxPerGene>=1`、`MaxCandidates>=MaxPerGene`、`SampleSize>=50 或 0`）、`ToString()`，并新增 `SetDiscoveryCandidates(IEnumerable(Of String))` 便捷方法。

### 2) 残差筛选（新文件 `EdgeDiscovery.vb`）

- `ResidualScreening`：把候选基因在采样细胞上的表达向量**一次性标准化**（mean/sd，sd≈0 的基因直接剔除），随后对每个靶基因计算 `Pearson(residual, candidate)`；残差的均值=0，因此只需点积，复杂度 `O(|C| × nCells)`，按靶基因并行。
- 残差定义：`residual = label(trainPos) − model.PredictBatch(features(trainPos))`；常数模型的残差为 `label − mean`，因此**无调控者的源节点/训练失败的基因也能通过发现获得真实模型**。
- 相关阈值过滤后按 `|r|` 降序取前 `DiscoveryMaxCandidates` 个候选；排除自身与已存在的调控者。

### 3) 增量接受（`GeneTreeTrainer.TrainGene`）

第一阶段：现有逻辑拟合 ARACNE 边模型，得到 `r2_base`。
第二阶段：对候选逐个（降序 |r|，最多 `DiscoveryMaxCandidates` 次）用 **同一 `trainPos/validatePos` 划分**重拟合 `regs ∪ 已接受 ∪ 当前候选`，计算验证集 R²：

- 若 `ΔR² ≥ DiscoveryMinDeltaR2` 且该新特征的 `splits > 0` → 接受，`r2_current = r2_new`，继续下一个候选；
- 否则丢弃，继续下一个候选；
- 达到 `DiscoveryMaxPerGene` 或候选耗尽即停止。
最终 `GeneModel.Regulators = ARACNE 边（原顺序） ++ 已接受发现边（接受顺序）`，`RegulatorIndex` 同步映射到表达矩阵行号；特征顺序在训练/预测/存档三处保持一致。

### 4) 报告与持久化

- `RegulatorImportance` 增 `source`（`aracne`/`discovered`）与 `deltaR2`。
- `TrainingReport` 增 `Discovery As DiscoveryReport`（由 `FromModels` 单线程聚合，避免并行体内共享状态）；`Write(folder)` 额外输出 `discovery.tsv`；`importance.tsv` 末尾追加 `source,delta_r2`（旧列顺序不变）。
- `GeneTreeStorage`：manifest 增 `opt.edgeDiscovery`、`opt.discoveryMaxPerGene`、`opt.discoveryMaxCandidates`、`opt.discoveryMinCorrelation`、`opt.discoveryMinDeltaR2`、`opt.discoverySampleSize`、`report.discoveredEdges`；新增条目 `discovery/candidates.txt` 与 `discovery.tsv`；`readConfig` 对缺失键回退默认值，`Load` 尽力恢复 `DiscoveryReport`（缺条目不报错），`FORMAT_VERSION` 不变。
- 由于存档已按“每模型调控者列表”保存、Load 时用 `GeneRegulationGraph.FromRegulations` 重建拓扑，发现边天然随模型持久化并被扰动求解使用。

### 5) R# API（`geneTreeTools.vb`）

- `geneTree::discovery(config, candidates, max_per_gene = 2, max_candidates = 5, min_correlation = 0.1, min_delta_r2 = 0.002, sample_size = 1000, max_genes = 0)` → 返回同一个 `GeneTreeConfig`（可管道接在 `make_profile()` 之后）；候选为空时返回 `RInternal.debug.stop` 错误对象。
- `geneTree::discovered_edges(tree, top_n = 50)` → `dataframe`（gene、regulator、correlation、delta_r2、splits、sensitivity，按 delta_r2 降序）。
- `geneTree::training_stats(tree)` 增 `discovery_mode`、`discovered_edges`、`discovery_targets`。
- `geneTree::report_table` / `write_report` 自动带上发现信息；包 `<remarks>` 与示例补充“候选集给谁”的说明：**若希望非 TF 扰动产生下游效应，需把这些基因放进候选集**。

## 架构设计

```mermaid
flowchart TD
  A[aracne.zip: edges.csv] --> B[GeneRegulationGraph: top-K ARACNE 边]
  C[表达矩阵 Matrix] --> D[GeneTreeTrainer 第一阶段拟合]
  B --> D
  D --> E[每基因模型 + 训练集残差]
  F[R# discovery: 候选基因 id 集合] --> G[候选向量标准化]
  G --> H[残差 vs 候选 Pearson 筛选]
  E --> H
  H --> I[逐候选增量重拟合 + ΔR² 校验]
  I --> J{ΔR² ≥ 阈值 且 splits > 0}
  J -- 是 --> K[接受: 追加到 Regulators/RegulatorIndex]
  J -- 否 --> L[丢弃并记录]
  K --> M[最终 GeneModel / TrainingReport.Discovery]
  L --> M
  M --> N[discovery.tsv + importance.tsv(source) + zip manifest]
  M --> O[PerturbationSolver: 发现边参与虚拟扰动]
```

## 性能与可靠性

- 相关性筛选：`|C| × nCells` 每靶基因，候选向量只标准化一次；`O(G × |C| × n)`，并行于靶基因维度。
- 重拟合：仅对“至少有一个候选通过相关阈值”的靶基因执行，最多 `DiscoveryMaxCandidates` 次/基因，单次成本与基线拟合同阶 → 整体约 1.5~2×；可用 `DiscoveryMaxGenes` / `MaxGenes` 限流。
- 确定性：复用 `SampleCells` 与 `SplitPositions` 的既有抽样与划分，随机性只来自 TGBoost 内部行/列抽样（与现有行为一致）。
- 兼容：`AracneOnly` 路径不新增任何计算/文件；旧 zip 缺键全部走默认值。

## 目录结构

```
sub-system/GeneTree/
├── GeneTreeConfig.vb        [MODIFY] 新增 EdgeDiscoveryMode 枚举与 8 个发现相关配置项、
│                                     校验规则、ToString、SetDiscoveryCandidates
├── EdgeDiscovery.vb         [NEW]    DiscoveryEdge / DiscoveryReport 数据结构、
│                                     候选向量标准化与残差 Pearson 筛选、discovery.tsv 写出
├── GeneModel.vb             [MODIFY] RegulatorImportance 增 source / deltaR2
├── GeneTreeTrainer.vb       [MODIFY] TrainGene 拆分为 fitModel + 发现阶段（残差、筛选、
│                                     逐候选增量重拟合、ΔR² 接受、特征追加与 RegulatorIndex 同步），
│                                     日志限流 logLimited
├── TrainingReport.vb        [MODIFY] 增 Discovery 属性；FromModels 聚合发现统计；
│                                     Write 输出 discovery.tsv；importance.tsv 追加 source/delta_r2
├── GeneTreeStorage.vb       [MODIFY] manifest 增 opt.discovery*/report.discoveredEdges；
│                                     新增 discovery/candidates.txt、discovery.tsv 读写；旧档兼容
├── XGBoostGRN.vb            [MODIFY] 暴露 Discovery / DiscoveredEdges 只读属性
└── test/GeneTreeDemo.vb     [MODIFY] 新增 --discovery-candidates <file> 参数与发现摘要输出
workbench/R#/biosystem/
└── geneTreeTools.vb         [MODIFY] 新增 discovery() 与 discovered_edges()，training_stats 增字段，
                                      更新包文档与示例
```

## 关键代码结构

```
' GeneTreeConfig.vb
Public Enum EdgeDiscoveryMode
    AracneOnly = 0
    ResidualTwoStage = 1
End Enum

' EdgeDiscovery.vb
Public Class DiscoveryEdge
    Public Property target As String
    Public Property regulator As String
    Public Property correlation As Double     ' 残差与候选的相关
    Public Property deltaR2 As Double         ' 接受该边带来的验证集 R² 增益
    Public Property splits As Integer
    Public Property sensitivity As Double
End Class

Public Class DiscoveryReport
    Public Property Mode As EdgeDiscoveryMode
    Public Property Candidates As String()
    Public Property TargetsExamined As Integer
    Public Property CandidatesScreened As Integer
    Public Property Edges As DiscoveryEdge()
    Public ReadOnly Property Accepted As Integer
    Public Sub WriteTable(file As String)
End Class
```

## Agent Extensions

### SubAgent

- **code-explorer**
- Purpose: 实施前后复核 `GeneTreeTrainer.TrainGene`、`TrainingReport`、`GeneTreeStorage`、`geneTreeTools.vb` 的现有签名与调用点，确认新增字段/参数不破坏既有调用（尤其 `TrainingReport.FromModels`、manifest 读写、R# `ExportAPI` 绑定）。
- Expected outcome: 给出确切的修改位置清单与既有调用点核对结果，避免特征顺序错位或存档键冲突。