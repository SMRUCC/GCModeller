---
name: WGCNA性能优化与分块计算
overview: 对 GCModeller 的 WGCNA 项目做性能优化：用 TensorFlow 的 Tensor 算子（可透明切换 CUDA）重写相关矩阵/邻接矩阵/TOM 的 O(n²)/O(n³) 核心计算，引入并行化，并实现 GNU R WGCNA blockwiseModules 的分块计算模式；随后用 R 4.5.0 的 WGCNA 包在 ath_norm.csv 上做两步对照验证，最后在 60273×1888 的大型表达矩阵上做完整性能测试。
todos:
  - id: tensor-ops-layer
    content: 用 [subagent:code-explorer] 核实 Tensor/hctree/CorrelationMatrix 签名，新建 TensorOps.vb 与 Gpu/TensorBackend.vb（含 EnableGpu/DisableGpu）
    status: completed
  - id: tensor-core-kernels
    content: 新增 TensorCorrelation.vb，并改造 WeightedNetwork.vb、TOM.vb、BetaTest.vb 为 Tensor/GEMM 内核
    status: completed
    dependencies:
      - tensor-ops-layer
  - id: tree-cut
    content: 实现 DynamicTreeCut.vb（hybrid 自适应剪枝）与 StaticCut.vb，改造 TOM.vb 的模块剪切分派
    status: completed
    dependencies:
      - tensor-ops-layer
  - id: blockwise
    content: 实现 GeneFilter/ProjectiveKMeans/BlockwiseModules/MergeCloseModules，新增 WGCNAConfig 与 Analysis.RunBlockwise 并扩展 Result
    status: completed
    dependencies:
      - tensor-core-kernels
      - tree-cut
  - id: bench-project
    content: 新建 benchmark 控制台项目（x64+ServerGC），用 [skill:lsp-code-analysis] 校验向后兼容后编译并在小数据冒烟
    status: completed
    dependencies:
      - blockwise
  - id: r-validation
    content: 编写 run_test/wgcna_reference.R 与 compare.R，用 R WGCNA 在 ath_norm.csv 上完成单块数值对照与整表 blockwise 对照
    status: completed
    dependencies:
      - bench-project
  - id: hsa-benchmark
    content: 用 K:\hsa 大矩阵跑全量与 top-N 过滤两档性能测试（CPU/GPU 对比），输出分段耗时与峰值内存报告
    status: completed
    dependencies:
      - r-validation
---

## 产品概述

对 GCModeller 的 `WGCNA` 项目（`G:\GCModeller\src\GCModeller\annotations\WGCNA\WGCNA\WGCNA.vbproj`）进行系统性性能优化，使其能够处理大型表达矩阵的共表达网络计算。优化不改变对外 API 语义，但将核心计算从"逐元素循环 + PLINQ"改写为"Tensor 算子 / GEMM + 分块"，并引入与 GNU R WGCNA 一致的 blockwise（分块）计算模式。

## 核心功能

### 一、Tensor 化核心计算

- 用 `Microsoft.VisualBasic.MachineLearning.TensorFlow.Tensor`（行优先 1D `Double()`）重写相关矩阵、软阈值邻接矩阵、连接度向量、TOM 矩阵、beta 扫描的计算内核
- 相关矩阵由"逐行 PLINQ Pearson"改为"行标准化 + `MatMul`（`Z * Zᵀ`）"的 GEMM 实现
- TOM 中间矩阵由三重循环 O(n³) 串行改为 `A * A` 矩阵乘
- beta 扫描不再为每个候选 beta 分配完整 n² 临时矩阵，改为流式行和（利用 `|cor|^β < 阈值 ⟺ |cor| < 阈值^(1/β)` 提前剪枝）

### 二、CUDA 加速接入（显式开关）

- 提供 `Analysis.EnableGpu()` / `DisableGpu()`，内部调用 `Microsoft.VisualBasic.Computing.ILCuda.GPUTensor.CudaTensor.Register()` / `Unregister()`
- 默认仍走 SIMD CPU，不改动任何现有调用方行为；注册失败时读取 `CudaTensor.LastError` 并静默回退 CPU

### 三、并行计算

- 块之间并行（可配置并发块数，控制 n² 内存峰值）
- 块内按行条带并行（相关矩阵、阈值化、TOM 组合、beta 扫描）
- 基因过滤、模块特征基因/基因显著性/模块成员计算并行化

### 四、R WGCNA 分块（blockwise）计算模式

- 基因预过滤（可选：低表达/低方差过滤、top-N 方差基因）
- `projectiveKMeans` 式预聚类：抽样 k-means 得质心 → 全基因按最近质心分配 → 过大簇递归拆分
- 逐块执行：相关矩阵 → 邻接 → TOM → 不相似度 → 层次聚类 → 模块剪切
- 跨块模块合并：`mergeCloseModules`（模块特征基因相关性高于阈值即合并）
- 全局 beta 值在抽样基因上统一估计，避免逐块重复

### 五、双模式树剪切

- 新增 `dynamicTreeCut`（hybrid 自适应剪枝）作为默认，参数 `minModuleSize` / `deepSplit` / `cutHeight`
- 保留原静态剪切（按树总距离百分比 `distCut`）作为可选项

### 六、对照与性能测试

- 用本地 R 4.5.0（已装 WGCNA / dynamicTreeCut / fastcluster / igraph）对 `ath_norm.csv`（34262 基因 × 6 样本）做两步对照
- 用 `K:\hsa\Homo_sapiens_expr_advanced_all_conditions.csv`（60273 基因 × 1888 样本，822 MB）做全量与过滤两档性能测试
- 输出分段耗时、峰值内存、模块统计与一致性指标（矩阵最大绝对误差、ARI）

## 技术栈选型

沿用项目现有技术栈（不引入新依赖）：

| 层次 | 选型 | 说明 |
| --- | --- | --- |
| 语言/框架 | VB.NET，net10.0，x64 | 与 `WGCNA.vbproj` 现有配置一致 |
| 数值内核 | `Microsoft.VisualBasic.MachineLearning.TensorFlow.Tensor` | 项目已引用 `TensorFlow.vbproj`；默认后端 `Compute.SIMDTensor.Default` |
| GPU 后端 | `Microsoft.VisualBasic.Computing.ILCuda.GPUTensor.CudaTensor` | 项目已引用 `ILCudaTensor.vbproj` + `ILCuda.vbproj` |
| 层次聚类 | `Microsoft.VisualBasic.DataMining.HierarchicalClustering` | 块内改用 `PDistClusteringAlgorithm`（pdist 压缩输入）省内存 |
| 表达矩阵 | `SMRUCC.genomics.Analysis.HTS.DataFrame.Matrix.LoadData` | 用户指定入口 |
| 对照环境 | `C:\Program Files\R\R-4.5.0\bin\Rscript.exe` | 已安装 WGCNA / dynamicTreeCut / fastcluster / igraph |


**无需新增任何 ProjectReference**（`WGCNA.vbproj` 已引用 TensorFlow、ILCudaTensor、ILCuda、hctree、DataMining、HTS_matrix 等全部所需项目）。

## 实现思路

### 总体策略

把 WGCNA 主流程拆成"**可 GEMM 化的稠密线性代数**"与"**逐块流式处理**"两层：

1. **算子层**：所有 n² 级运算统一走 `Tensor`（1D 行优先 `Double()`），不再使用 jagged `Double()()` 做多重循环。切换 GPU 只需改 `Tensor.computeKernel`，业务代码零改动。
2. **分块层**：当基因数 > `maxBlockSize` 时，预聚类切块，每块独立完成 相关 → 邻接 → TOM → 聚类 → 剪切，最后跨块合并模块。这样把 TOM 的 O(n³) 降为 O(n·B²)（B 为块大小）。

### 关键技术决策与权衡

**决策 1：相关矩阵用 GEMM 而非逐对 Pearson**

- 现状：`Builder.Correlation` → `GetCorrelations(Mantel.Pearson)`，PLINQ 逐行对全表调用 `Correlations.GetPearson`，每行分配两个 `List(Of Double)` 反复扩容，常数开销极大。
- 改为：行中心化 → 行 L2 归一化 → `C = Z * Zᵀ`。数学等价（Pearson = 余弦相似度 on 中心化向量）。
- 复杂度同为 O(n²·m)，但 GEMM 走 SIMD/多线程，常数因子可降低 1~2 个数量级。
- 代价：仍需 O(n²) 内存 —— 这正是必须分块的原因。
- p 值：由 r 与样本数用 t 分布换算（`CreateGraph` 用到 `cor.pvalue(i,j)`），默认**不物化** p 值矩阵（`computePvalue=False`），只在需要时按 `(i,j)` 现算，省一半 n² 内存。

**决策 2：TOM 的 `Intermediate` 改为 `A * A`**

- 现状三重循环 `sum += alpha(i)(u) * alpha(j)(u)` 就是 `A·Aᵀ`，A 对称故等于 `A·A`；串行 O(n³)，n=5000 时约 1.25×10¹¹ 次浮点 + 同等次数的 jagged 间接寻址，实测会跑到小时级。
- 改为 `A.MatMul(A)`（或 `Math.matmul`），BLAS 风格 GEMM；GPU 下走 fp32 GEMM 收益最大。
- TOM 组合 `(I + A) / (min(k_i,k_j) + 1 - A)` 直接在 1D `Data` 数组上做一次融合遍历（避免多次 Tensor 分配），`k` 由 `A.Sum(1)` 得。

**决策 3：beta 扫描流式化**

- 现状每个 beta 都 `S ^ betaPow`（新分配 n²）+ `Adjacency` 的 `cor.Copy`（又一次 n² 分配）+ `RowApply`，20 个 beta ≈ 40+ 次 n² 分配与 GC 压力。
- 改为：预计算 `|cor|` 一次；对每个 beta，`cut = threshold^(1/beta)`，一次 O(n²) 遍历累加行和（`< cut` 直接跳过 pow），**不分配任何 n² 临时矩阵**；beta 之间 PLINQ 并行。
- 内存从 O(B·n²) 降到 O(n²)。

**决策 4：块内层次聚类用 pdist 压缩格式**

- `DefaultClusteringAlgorithm` 吃 jagged `Double()()`（n² 引用数组 + 每行一个对象）；`PDistClusteringAlgorithm` 吃 pdist 压缩向量（n(n-1)/2），且 n≥100 时内部 PLINQ 展开。
- 对 B=5000：jagged 约 200 MB + 5000 个数组对象；pdist 约 100 MB。
- 风险：`HierarchyBuilder` / `DistanceMap` 会构造 n(n-1)/2 个 `HierarchyTreeNode`（B=5000 时 1250 万个对象），可能数百 MB 且建堆较慢。实施时**先实测 B=4000/5000 的耗时内存**，若成为瓶颈则把 `maxBlockSize` 降到 3000~4000，或补一个紧凑数组版 average-linkage（merge/height 双数组 + 最近邻链，O(n²) 内存、O(n² log n) 时间）。

**决策 5：CUDA 显式开关，默认 CPU**

- `CudaTensor.Register()` 成功才切换后端，失败写 `LastError` 且不改变当前后端 —— 天然安全。
- 但默认自动注册会改变现有调用方（R# `cor_network`、CLI）的行为与显存占用，故默认关闭，由 benchmark 显式开启做 CPU/GPU 对比。
- GPU 阈值：`MinGpuElements=4096`、`MinGemmElements=65536`；块内 TOM 的 GEMM（5000³ 级）远超阈值，是 GPU 收益最大的环节。
- 注意：`Tensor` 无 GPU 内核的算子自动回退 CPU，无需业务侧分支。

**决策 6：大矩阵下网络图构建必须受限**

- 现状 `createGraph` 对所有 `mat(i,j) <> 0` 调 `g.AddEdge`，n² 条边必然 OOM。
- 改为：`buildGraph`（分块模式默认 False）+ `maxEdges`（0=不限）+ 按 TOM/邻接阈值稀疏化；分块模式下只导模块分配表与模块特征基因，不建全图。

### 性能与复杂度

设 n=基因数、m=样本数、B=块大小、K=n/B 块数：

| 阶段 | 现状 | 优化后（单块 B） | 分块总计 |
| --- | --- | --- | --- |
| 相关矩阵 | O(n²·m)，常数极大 | O(B²·m) GEMM | O(n·B·m) |
| beta 扫描 | O(20·n²) 时间 + O(20·n²) 分配 | O(20·B²) 时间，O(B²) 分配 | O(20·n·B) |
| 邻接/连接度 | O(n²) × 多次拷贝 | O(B²) 原地 | O(n·B) |
| TOM | **O(n³) 串行三重循环** | O(B³) GEMM | **O(n·B²)** |
| 层次聚类 | O(n²) 内存（jagged） | O(B²/2) pdist | O(n·B/2) |
| 建图 | O(n²) 边（必 OOM） | 稀疏化/可选 | 可控 |


内存峰值（B=5000，Double）：cor 200 MB + TOM 200 MB + pdist 100 MB ≈ 500 MB/块；并行块数由 `maxConcurrentBlocks`（默认 1，可 2）限制。
表达矩阵本体：60273 × 1888 × 8 B ≈ 910 MB，加 `DataFrameRow` 对象开销，benchmark 项目须 `x64` + `<ServerGarbageCollection>true</ServerGarbageCollection>`。

### 避免技术债

- 不改动 `Analysis.Run(samples, adjacency, pcaLayout)` 签名与 `Result` 的既有字段，新能力走 `RunBlockwise(samples, config)` + `WGCNAConfig`
- 不新增 `ProjectReference`、不新增 NuGet 包
- Tensor 与 `NumericMatrix`/`GeneralMatrix` 之间**无 CType 转换**（已核实），故统一在 `TensorOps` 中提供展平/还原辅助，避免各处重复手写
- 不修改 sciBASIC# 基础库；若块内聚类确需紧凑实现，新增在 WGCNA 项目内

## 系统架构

```mermaid
flowchart TD
    A[Matrix.LoadData 表达矩阵] --> B[GeneFilter 可选预过滤]
    B --> C{n > maxBlockSize ?}
    C -->|否| D[单块路径]
    C -->|是| E[ProjectiveKMeans 预聚类分块]
    E --> F[Block 1..K 并行]
    D --> G[BlockPipeline]
    F --> G
    G --> G1[TensorCorrelation: 标准化 + MatMul]
    G1 --> G2[BetaTest: 流式行和扫描]
    G2 --> G3[WeightedNetwork: 幂 + 阈值 + 连接度]
    G3 --> G4[TOM: A*A MatMul + 融合组合]
    G4 --> G5[PDistClusteringAlgorithm 层次聚类]
    G5 --> G6[DynamicTreeCut / StaticCut]
    G6 --> H[MergeCloseModules 跨块合并]
    H --> I[ModulePhenotype: 特征基因/GS/MM]
    I --> J[Result + 导出 CSV]
    K[Gpu/TensorBackend EnableGpu] -.设置 Tensor.computeKernel.-> G1
    K -.-> G4
```

## 执行要点（防回归）

**编译/兼容性**

- `Analysis.Run` 仍返回含 `beta/hclust/K/network/TOM/modules/softBeta` 的 `Result`；分块模式下 `TOM`/`hclust` 为最后一块或 Nothing，`modules` 为跨块合并后的完整结果，在 XML 注释中说明
- `WGCNA.vbproj` 开启了 `GenerateDocumentationFile=True`，所有新增 Public 成员必须有 XML 注释，否则产生警告
- `test` 项目 `StartupObject` 为 `test.networkTest`，不要误改；benchmark 用独立新项目

**Tensor 使用陷阱（已核实）**

- `New Tensor(data As Double(), ParamArray shape)` 会校验长度并 **Clone**，构造后直接写 `t.Data(i)` 须调 `t.MarkHostModified()`
- `*` 对两个 Tensor 是**矩阵乘**，逐元素乘用 `ElementwiseMultiply`
- 无 `^` 运算符，用 `Math.pow(t, p)`；标量重载参数多为 `Single`，注意 `2.0F`
- `Sum(axis)`：axis=0 列和、axis=1 行和，keepdims=True
- 无 `Double()()` 构造，须先按行优先展平

**内存**

- 块处理完立即释放该块的 cor/TOM/pdist（置 Nothing + `GC.Collect` 在块边界可控调用）
- 并行块数默认 1（`maxConcurrentBlocks`），避免 K 份 n² 同时驻留
- 直接改 `Tensor.Data` 数组做融合遍历比多次 `Math.*` 生成中间 Tensor 更省内存，优先用前者

**日志与可观测**

- 沿用现有 `VBDebugger.EchoLine` 风格输出阶段提示
- benchmark 用 `Stopwatch` 分段计时（load / filter / precluster / cor / beta / adjacency / tom / hclust / cut / merge / phenotype），并采样 `GC.GetTotalMemory` / `Process.WorkingSet64` 记录峰值

**R 对照注意事项**

- PowerShell 调用必须用 `& "C:\Program Files\R\R-4.5.0\bin\Rscript.exe" -e "..."` 形式（直接带引号会被解析失败）
- 对照要固定随机种子（`set.seed`）与超参（power、minModuleSize、deepSplit、maxBlockSize），否则不可比
- 34262² 稠密矩阵约 9.4 GB，R 侧也必须用 `blockwiseModules`；单块严格对照只在 top 3000~5000 方差基因子集上做

## 目录结构

```
G:\GCModeller\src\GCModeller\annotations\WGCNA\
├── WGCNA/                                        # 主库项目（SMRUCC.genomics.Analysis.HTS.WGCNA）
│   ├── Analysis.vb                               # [MODIFY] 保留 Run/RunWithPhenotype 签名；新增 RunBlockwise、EnableGpu/DisableGpu；createGraph 增加 maxEdges/稀疏化与 buildGraph 开关
│   ├── WGCNAConfig.vb                            # [NEW] 分析配置对象：maxBlockSize、adjacency、power/betaSeq、minModuleSize、deepSplit、cutHeight、mergeCutHeight、treeCut、useGpu、maxConcurrentBlocks、computePvalue、buildGraph、maxEdges、filter、randomSeed
│   ├── Result.vb                                 # [MODIFY] 新增 blocks（块划分）、blockModules（块内结果）、timing（分段耗时）、mergedFrom 等字段；既有字段保持
│   ├── CorrelationNetwork.vb                     # [MODIFY] 导出/加载辅助适配新的 gene→module 结果（保持 LoadAdjacencyMatrix/ExportGraph 签名）
│   ├── Gpu/
│   │   └── TensorBackend.vb                      # [NEW] EnableGpu/DisableGpu/IsGpuEnabled/LastGpuError；封装 CudaTensor.Register/Unregister 与 SIMDTensor.Register 回退
│   └── Algorithm/
│       ├── TensorOps.vb                          # [NEW] Tensor 互操作：ToTensor(Double()()) 行优先展平、ToJagged、RowSums/ColumnSums、InPlaceThreshold、InPlacePow、FusedTomCombine 等融合原语
│       ├── TensorCorrelation.vb                  # [NEW] 基于 GEMM 的 Pearson 相关矩阵：行中心化 → L2 归一化 → Z*Zᵀ；提供轻量 (i,j)/pvalue(i,j) 访问与可选物化 p 值
│       ├── WeightedNetwork.vb                    # [MODIFY] Adjacency 改 Tensor 原地阈值化；WeightedCorrelation 去重复拷贝；Connectivity 用 Sum(1) 减对角
│       ├── BetaTest.vb                           # [MODIFY] BetaTable 重载接受 Tensor；流式行和扫描（阈值^(1/β) 提前剪枝），零 n² 临时分配；beta 间 PLINQ 并行；保留原 API
│       ├── TOM.vb                                # [MODIFY] Intermediate 改 A*A MatMul（保留 jagged 回退）；Matrix 走 Tensor 融合组合；CreateModules 迁出到 StaticCut
│       ├── StaticCut.vb                          # [NEW] 原静态剪切（按树总距离百分比 distCut），作为 TreeCutMethod.Static 的实现
│       ├── DynamicTreeCut.vb                     # [NEW] R cutreeHybrid 的 VB 实现：Cluster→(merge,height) 转换、自顶向下自适应剪枝、minModuleSize/deepSplit/cutHeight、离群基因后处理
│       ├── GeneFilter.vb                         # [NEW] 基因预过滤：低表达/近零方差剔除、top-N 方差基因；输出过滤后 Matrix 与保留索引
│       ├── ProjectiveKMeans.vb                   # [NEW] R projectiveKMeans 式预聚类：z-score → 抽样 k-means（k=ceil(n/preferredSize)）→ 全基因最近质心分配 → 过大簇递归拆分；输出 blocks
│       ├── BlockwiseModules.vb                   # [NEW] 分块主流程：全局 beta 估计 → 逐块 BlockPipeline → 汇总；块间并行受 maxConcurrentBlocks 限制
│       ├── MergeCloseModules.vb                  # [NEW] R mergeCloseModules：全基因上算模块特征基因 → 相关性矩阵 → 高于 1-mergeCutHeight 的合并 → 重新编号
│       └── ModulePhenotype.vb                    # [MODIFY] CalculateModuleEigengene 去掉重复的 NamedCollection 构建（直接 PCA 输入）；批量函数 PLINQ 并行；复用已算特征基因避免重复 PCA
├── benchmark/                                    # [NEW] 独立控制台基准项目
│   ├── WGCNA_benchmark.vbproj                    # [NEW] net10.0，x64，Exe，ServerGarbageCollection=True；引用 ..\WGCNA\WGCNA.vbproj 与 HTS_matrix
│   └── Program.vb                                # [NEW] CLI：--file --mode(full|blockwise) --block --gpu --filter --topN --out；分段计时 + 峰值内存 + 导出 gene→module CSV、模块统计、timing JSON
└── run_test/
    ├── wgcna_reference.R                         # [NEW] R 侧对照脚本：① top 3000~5000 方差基因子集导出 cor/adjacency/TOM/labels（cutreeDynamic 与固定高度 cutree 两版）；② 全表 34262 用 blockwiseModules 导出 labels/MEs；记录各阶段耗时
    └── compare.R                                 # [NEW] 比对脚本：矩阵最大绝对误差/相关系数、模块数、igraph::compare 的 adjusted Rand index、eigengene 相关性、耗时对比，输出 Markdown 报告
```

## 关键代码结构

```
''' <summary>
''' WGCNA 分析配置（新增 API 的统一入口，既有 Analysis.Run 重载内部使用默认实例）
''' </summary>
Public Class WGCNAConfig
    Public Property maxBlockSize As Integer = 5000
    Public Property adjacency As Double = 0.6
    Public Property power As Double = Double.NaN        ' NaN 时自动做 beta 扫描
    Public Property betaSeq As Double() = Nothing       ' Nothing 时用 seq(1,10) + seq(11,30,by:=2)
    Public Property treeCut As TreeCutMethod = TreeCutMethod.Dynamic
    Public Property minModuleSize As Integer = 20
    Public Property deepSplit As Integer = 2
    Public Property cutHeight As Double = 0.99
    Public Property distCut As Double = 0.6             ' 仅 Static 模式
    Public Property mergeCutHeight As Double = 0.15
    Public Property useGpu As Boolean = False
    Public Property maxConcurrentBlocks As Integer = 1
    Public Property computePvalue As Boolean = False
    Public Property buildGraph As Boolean = False
    Public Property maxEdges As Integer = 0
    Public Property filterTopN As Integer = 0           ' 0 = 不过滤
    Public Property minVariance As Double = 0
    Public Property randomSeed As Integer = 12345
End Class

Public Enum TreeCutMethod
    Dynamic   ' dynamicTreeCut hybrid
    Static    ' 原有按树总距离百分比剪切
End Enum
```

```
' BlockwiseModules.Run 与 GPU 开关的对外契约（实现时 signatures 以此为准）
Public Function Run(samples As Matrix, Optional config As WGCNAConfig = Nothing) As Result
Public Function EnableGpu(Optional cacheBytes As Long = 0, Optional useFp32Gemm As Boolean = True) As Boolean
Public Sub DisableGpu()
```

## Agent Extensions

### SubAgent

- **code-explorer**
- 用途：在实施前核实 `Tensor` / `Math` / `NumPy` 的精确方法签名（尤其 `Sum(axis)`、`Math.pow`、`ElementwiseMultiply` 的参数类型与返回形状）、`PDistClusteringAlgorithm` 的 pdist 输入约定、`Cluster` 的 `TotalDistance/isLeaf/Children` 成员、`CorrelationMatrix` 构造函数重载，以及 `Analysis.Run` 的全部调用点
- 预期结果：拿到可直接编码的准确 API 契约与受影响调用点清单，避免因签名猜测导致反复编译失败

### Skill

- **lsp-code-analysis**
- 用途：对 `Analysis.Run`、`TOM.Matrix`、`WeightedNetwork.Adjacency`、`BetaTest.BetaTable`、`TOM.CreateModules` 做引用点与调用层次分析，评估重构影响面（含 `G:\GCModeller\src\workbench\R#\phenotype_kit\WGCNA.vb`、`TRNtoolkit\WGCNA.vb`、CLI 工具）
- 预期结果：确认向后兼容边界，确保既有 R# `cor_network` 与 CLI 调用链在优化后行为不变