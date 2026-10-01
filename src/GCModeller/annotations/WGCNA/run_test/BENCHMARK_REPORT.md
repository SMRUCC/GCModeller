# WGCNA 性能优化 / GNU R 对照 / 大矩阵性能测试报告

生成时间：2026-10-02
测试对象：`SMRUCC.genomics.Analysis.HTS.WGCNA`（net10.0 / x64 / Server GC）
对照环境：GNU R 4.5.0 + `WGCNA` + `dynamicTreeCut` + `igraph`

---

## 一、做了哪些优化

| # | 环节 | 优化前 | 优化后 |
|---|---|---|---|
| 1 | 相关矩阵 | `Builder.Correlation` → `GetCorrelations(Mantel.Pearson)`，PLINQ 对每一对基因调一次 Pearson，每行分配两个 `List(Of Double)` 反复扩容 | 行去均值 + L2 归一化后一次 GEMM `Z·Zᵀ`（`Tensor.MatMul`），数学上等价于逐对 Pearson |
| 2 | TOM 中间矩阵 | `TOM.Intermediate` 三重循环 `sum_u a(i,u)*a(u,j)`，串行 O(n³) | 一次 GEMM `A·A`（<c>Tensor.MatMul</c>），GPU 下走 fp32 GEMM |
| 3 | TOM 组合 | 两次 n² 中间矩阵分配 | 融合成一次 O(n²) 遍历，就地写回 GEMM 结果缓冲区 |
| 4 | beta 扫描 | 每个候选 beta 都分配一份 n² 邻接矩阵（约 20 个候选 ⇒ 40+ 次 n² 分配） | 复用同一份 `\|cor\|`，用 `cut = τ^(1/β)` 提前剪枝的**流式行和**，零 n² 临时分配；beta 之间 PLINQ 并行 |
| 5 | 邻接矩阵 | `cor.Copy` + 逐元素幂 + 逐元素阈值，多份 n² 拷贝 | 单次遍历就地完成；并按 R `adjacency()` 的约定把**对角线置零**（否则 TOM 会系统性偏离 R） |
| 6 | 距离矩阵 | jagged `Double()()`（n 个数组对象） | `AverageLinkage` 内部的 pdist 压缩工作区（n(n-1)/2） |
| 7 | **层次聚类** | 基础库 `AverageLinkageStrategy` 实际算的是 `(d1+d2)/2`（McQuitty / WPGMA），**不是** UPGMA，实测 400 个基因上 399 次合并全部与 R 不同 | 自建 `AverageLinkage`（Lance-Williams 加权更新 + 最近邻表），与 R `hclust(method="average")` **合并高度逐位一致** |
| 8 | 模块剪切 | 按树总距离百分比一刀切 | 新增 `DynamicTreeCut`（`cutreeHybrid`，hybrid 自适应剪枝）作为默认；原静态剪切保留为 `TreeCutMethod.StaticCut` |
| 9 | 大规模支持 | 无 | 新增 blockwise：`GeneFilter` → `ProjectiveKMeans` 预聚类 → 逐块建网切模块 → `MergeCloseModules` 跨块合并 |
| 10 | 并行 | 仅 beta 扫描用 PLINQ | 块内所有 n² 级遍历（标准化、幂、阈值、TOM 组合、不相似度、行和）全部 `Parallel.For`；块间可配置并发（`maxConcurrentBlocks`） |
| 11 | GPU | 无 | `Analysis.EnableGpu()` 注册 `ILCuda.GPUTensor.CudaTensor`，`Tensor` 算子透明落到 CUDA；默认仍走 SIMD CPU |
| 12 | 网络图 | 无条件建 n² 条边（n 稍大即 OOM） | 增加 `buildGraph` 开关与 `maxEdges` 上限；分块模式默认不建图 |

---

## 二、与 GNU R 的对照（ath_norm.csv，34262 基因 × 6 样本）

### 2.1 单块严格数值对照（按方差取 top 3000 基因，power = 6 固定）

脚本：`run_test/wgcna_reference.R`（阶段 A） + `run_test/compare.R`

| 矩阵 | 共同基因数 | 最大绝对误差 | 平均绝对误差 | RMSE | 元素级 Pearson |
|---|---|---|---|---|---|
| 相关矩阵 cor | 3000 | **2.887e-15** | 2.828e-16 | 3.521e-16 | **1.000000000000** |
| 邻接矩阵 adj | 3000 | 1.000e+00 ※ | 3.333e-04 | 1.826e-02 | 0.997262369282 |
| TOM | 3000 | **5.218e-15** | 3.645e-16 | 5.605e-16 | **1.000000000000** |

※ adj 的 1.0 误差**只出现在 3000 个对角线元素上**：R 的 `TOMsimilarity()` 会**就地修改入参**并把邻接矩阵对角线改回 1，
而 WGCNA 的 `adjacency()` 约定对角线为 0。去掉对角线后二者完全一致（平均绝对误差 3.3e-4 也是由这 3000 个元素拉高的，
非对角元素的误差在 1e-15 量级）。这一点已写进 `wgcna_reference.R` 的注释里。

### 2.2 树剪切实现对照（喂给 R 同一棵 hclust 树）

| 指标 | GCModeller | GNU R `cutreeHybrid` |
|---|---|---|
| 模块数 | 19 | 19 |
| 已标注基因 | 3000 | 3000 |
| **adjusted Rand index** | — | **1.000000** |

→ `DynamicTreeCut` 与 R 的 `cutreeHybrid` **行为完全一致**。

### 2.3 单块端到端对照

| 指标 | GCModeller | GNU R |
|---|---|---|
| 模块数 | 19 | 19 |
| **ARI** | — | **1.000000** |

### 2.4 整表分块（blockwise）对照

| 指标 | GCModeller | GNU R `blockwiseModules` |
|---|---|---|
| 基因数 | 34262 | 34262 |
| 模块数 | 81 | 59 |
| 已标注基因 | 26382 | 34262 |
| 耗时 | **187.5 s** | **311.3 s** |
| **ARI** | — | **0.188951** |

**ARI 偏低的原因（不是数值问题 —— 单块对照已经做到 ARI = 1.0）**：

1. **预聚类策略不同**。R 的 `blockwiseModules` 用 `nPreclusteringCenters = min(n/20, 100·n/maxBlockSize)` 个中心
   做投影 K-means，再把小簇按 eigengene 层次聚合成 ≤ maxBlockSize 的块；我们实现的是
   `projectiveKMeans`（抽样 K-means + 最近质心分配 + 递归拆分）。两者得到的**块划分本身就不一样**，
   跨块的基因自然会被分到不同的模块里。
2. R 在切完模块后还做了 `minCoreKME / minKMEtoStay` 的 KME 重分配（把所有基因都归入某个模块），
   我们还未实现这一步，因此有 7880 个基因标签为 0（灰色）。

---

## 三、大矩阵性能测试

数据：`K:\hsa\Homo_sapiens_expr_advanced_all_conditions.csv`
60273 基因 × 1888 样本，822 MB CSV。参数：`maxBlockSize=5000`、`power=6`、`adjacency=0`、`cutHeight=0.995`、`seed=54321`。

| 场景 | 后端 | 加载 | 分析 | 合计 | 峰值 RSS | 模块数 | 已标注基因 |
|---|---|---|---|---|---|---|---|
| 全量 60273 基因（21 块） | SIMD CPU | 17.7 s | 431.4 s | **449.3 s** | 3.66 GB | 85 | 16684 |
| 全量 60273 基因（21 块） | CUDA GPU | 17.9 s | 149.3 s | **167.1 s** | 7.02 GB | 85 | 16684 |
| top 20000 方差基因（6 块） | SIMD CPU | 17.7 s | 144.5 s | **162.2 s** | 3.43 GB | 57 | 6610 |
| top 20000 方差基因（6 块） | CUDA GPU | 17.9 s | 34.7 s | **52.6 s** | 3.91 GB | 57 | 6610 |

**CPU 与 GPU 的模块划分逐行完全一致**（`diff` = 0 行差异），说明 CUDA 后端接入是"对使用者透明"的。

### GPU 加速收益（按阶段，top20000 场景）

| 阶段 | CPU | GPU | 加速比 |
|---|---|---|---|
| 相关矩阵 GEMM（block 1，3786 基因） | 4719 ms | 186 ms | **25×** |
| TOM = A·A（block 1，3786 基因） | 12335 ms | 159 ms | **78×** |
| 整条流水线 | 144.5 s | 34.7 s | **4.2×** |

TOM 的 78× 是收益最大的环节 —— n³ 级 GEMM 正是 GPU 最擅长的负载。
切换到 GPU 之后，剩下的主要开销是**块内的 UPGMA 层次聚类**（CPU 串行），
top20000 场景里它占了 34.7 s 中的约 28 s，是下一步最值得优化的点。

---

## 四、怎么用

```vbnet
' 1) 小型数据：单块（签名与原来一致）
Dim result = Analysis.Run(samples, adjacency:=0.6, pcaLayout:=True)

' 2) 大型数据：分块（对应 R 的 blockwiseModules）
Dim config As New WGCNAConfig With {
    .maxBlockSize = 5000,
    .power = 6,                       ' 留空(NaN)则在抽样基因上自动估计
    .treeCut = TreeCutMethod.Dynamic, ' 或 StaticCut
    .deepSplit = 2,
    .minModuleSize = 20,
    .mergeCutHeight = 0.15,
    .filterTopN = 20000,              ' 0 = 不过滤
    .buildGraph = False,              ' 大规模下不要建图
    .useGpu = True
}
Dim result = Analysis.RunBlockwise(samples, config)

' 3) 显式开关 CUDA（默认 SIMD CPU）
If Analysis.EnableGpu() Then ...
Analysis.DisableGpu()
```

命令行基准：

```
WGCNA_benchmark --file <expr.csv> --mode blockwise|full|validate|hclust
                [--block 5000] [--gpu] [--topN 0] [--power 6] [--adjacency 0]
                [--cutheight 0.995] [--cut dynamic|static] [--seed 54321] [--dump] [--out <dir>]
```

输出：`gene_module.csv`、`modules.csv`、`blocks.csv`、`timing.csv`、`summary.txt`，
以及（`--dump` 时）`cor.csv / adj.csv / tom.csv / dendro.csv`。

对照脚本：

```
Rscript run_test/wgcna_reference.R <expr.csv> <outdir> [topN] [power] [blockSize] [both|a|b]
Rscript run_test/compare.R <outdir>
```

---

## 五、已知局限 / 后续可做

1. **块内 UPGMA 仍是 CPU 串行**，且距离矩阵并列（tie）很多时最近邻重扫会退化
   （`hsa` 里个别 4000+ 基因的块会出现 10~20 s 的尖峰）。可以改成 NN-chain 或并行化。
2. **跨块合并后未做 KME 重分配**，因此有部分基因标签为 0（灰色），这也是整表对照 ARI 偏低的次要原因。
3. `ProjectiveKMeans` 与 R 的预聚类策略不同，若需要更高的块级别一致性，可对齐 R 的
   `nPreclusteringCenters` + eigengene 层次聚合。
4. `Result.TOM` 在分块模式下为 Nothing（只保留最后一块的聚类树），避免 n² 矩阵常驻。
