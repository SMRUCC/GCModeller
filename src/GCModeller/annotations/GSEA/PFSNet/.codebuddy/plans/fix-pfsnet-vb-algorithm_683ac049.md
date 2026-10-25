---
name: fix-pfsnet-vb-algorithm
overview: 以 R 版 PFSNet 的置换与双侧显著性语义为规范，对照 Python 版修正 VB.NET 算法、输入解析和图处理错误，并在现有控制台测试项目中用三份指定数据完成可重复的全流程 demo 与关键回归断言。
todos:
  - id: fix-weight-ranking
    content: 修正 computew1 的平均秩与精确分位数，及 masked_ggi 节点均值权重
    status: completed
  - id: fix-graph-input
    content: 修正 GraphEdge.LoadData 表头解析与 Graph.simplify 自环/重复边去重
    status: completed
  - id: fix-statistics
    content: 新增配对 t 检验助手并修正 subnet_scores、ccs_statistics、estimatetdist
    status: completed
  - id: fix-align-result
    content: 新增基因对齐模块，修复 R.vb 的 cbind 并在 PFSNet.vb 串联与填充结果元数据
    status: completed
  - id: write-demo-test
    content: 用 [subagent:code-explorer] 核对图分解语义后重写 test/Program.vb 的三数据 demo 与断言
    status: completed
    dependencies:
      - fix-weight-ranking
      - fix-graph-input
      - fix-statistics
      - fix-align-result
  - id: build-and-run
    content: 构建两个工程并运行 demo，修复编译与运行时问题直到 0 警告 0 错误、退出码 0
    status: completed
    dependencies:
      - write-demo-test
---

## 用户需求（原文要点）

参考 `PFSNet/python/pfsnet.py` 与 `PFSNet/R/R` 下的 PFSNet 算法实现，修正 VB.NET 项目 `PFSNet/PFSNet.NET5.vbproj` 中的算法逻辑错误；修复完成后，在测试项目 `PFSNet/PFSNet/test/test.vbproj` 中使用 `data/pathways.txt`、`data/sample_control.txt`、`data/sample_test.txt` 三份文件作为测试数据集做 demo 测试。

## 需求澄清结论

- 统计语义**以 R 版为准**：每次置换重新筛选基因、重新生成子网，汇总双侧（`abs(t)`）pooled null；Python 版仅用于校验共享数学不变量（并列排名、组均值、样本重排）。
- Python 侧自身的置换抽样缺陷（`choice(num_c+num_t-1, ...)` 漏掉最后一列）与 R 侧未定义变量 `obj` 不得移植。

## 产品概述

对 VB.NET 版 PFSNet 做一次“对齐 R 参考实现”的算法纠错，并用仓库自带的三份真实数据（12 个 Normal 对照样本、42 个 Hyperdip>50 测试样本、通路基因互作边表）跑通完整流程，产出可复核的控制台 demo 与结果表格。

## 核心功能

- 模糊权重计算：并列表达值获得相同秩（R `rank()` 平均秩语义），分位数改用精确线性插值（R type 7），权重落在 `[0,1]`。
- 子网节点权重：改为“该基因在本组样本上的非缺失模糊权重均值”，不再依赖子网首节点，不再出现 >1 或 NaN/Infinity。
- 通路解析与图简化：跳过表头与非法行，删除自环并按 `(pathway, min(g1,g2), max(g1,g2))` 去重无向重复边。
- 统计检验：显式配对 t 检验（逐样本差值），缺失权重置零，非有限统计量按 R 的 `try()` 语义跳过，p 值按 `sum(abs(null)>abs(stat))/length(null)` 计算。
- 置换检验：置换分组大小严格等于两组样本数（12/42），54 列不重不漏，支持固定随机种子以复现。
- 基因对齐：进入算法前显式对齐两组基因集合与顺序，避免静默补零/丢基因。
- Demo 测试：读取三份指定数据，输出样本数/共同基因数/边数/显著子网数，落盘 CSV，并执行一组确定性断言（并列秩、权重范围、配对 t 已知答案、图不变量、置换覆盖性、结果有限性），失败以非零退出码结束。

## 技术栈

- VB.NET（`net10.0`，工程名仍为 `PFSNet.NET5.vbproj`），沿用现有 sciBASIC 依赖：`HTS_matrix`（`Matrix`/`DataFrameRow`）、`Core`（`Ranking`/`Quantile`/`Index(Of String)`/`List(Of T)`）、`Math.Statistics`（`t.Test`）、`network_graph-netcore5`（`NetworkGraph.DecomposeGraph`）。
- 测试工程为控制台 `Exe`（无断言框架），采用自写 `Assert` 计数 + 退出码的自检模式，与仓库其它 `_smoketest` 工程风格一致。

## 实现方案

以 **R 版 `pfsnet.R` + `estimatetdist.R` + `compute_npscore.R` 为唯一规范**，逐点修正 VB 实现；Python 版用于交叉验证共享不变量。修完之后用确定性小数据断言锁定每处修复，再用三份真实数据跑端到端 demo。

### 已确认的 VB 逻辑错误与修法（逐条对应参考实现）

1. **并列秩** `Algorithm/computew1.vb:88` 用 `Strategies.OrdinalRanking`（并列给 1,2,3…），而 R 用 `rank(x)`（并列取平均）。数据里大量 `0.0` 并列，导致结果依赖基因行顺序。改为 `data.Ranking(Strategies.FractionalRanking)`（`Microsoft.VisualBasic.Core\src\Math\Correlations\Ranking.vb` 中 `FractionalRanking` 即 “1, 2.5, 2.5, 4” 平均秩）。
2. **分位数近似** 同文件用 `Quantile.Threshold`（GK 近似，epsilon=0.001），R 用精确 `quantile(type 7)`。改为 `x.ToArray.LinearQuantile(theta)`（`Math\Quantile\Extensions.vb:111`，即 `pos=q*(n-1)` 线性插值，与 R type 7 一致），并按 R 的 `na.rm=TRUE` 过滤非有限值。
3. **节点权重公式** `Algorithm/masked_ggi.vb:167-170` 分母写成“首节点权重之和”：
`weight = sum(当前基因)/sum(首节点)`。R（`pfsnet.R:48-49`、`estimatetdist.R:36-37`）语义是 `sum(当前基因)/sum(!is.na(当前基因))` = 该基因在本组样本上的均值。改为按基因自身样本数求均值；`w1matrix1.Select(name)` 返回 `Nothing`（基因不在矩阵中）时置 0，避免 NRE。
4. **通路表头** `Models/GraphEdge/GraphEdge.vb:96-105` 把 `pathway gene1 gene2` 当边读入。改为跳过表头（首行首列为 `pathway`）/空行/字段数不足 3 的行，并做 BOM、CR、空白裁剪。
5. **simplify 不去重** `Models/Graph.vb:109-119` 只过滤自环。按 igraph `simplify`（`remove.loops=TRUE, remove.multiple=TRUE`）补全：自环 + 无向重复边（同向与反向）按规范化键去重；同时给 `graph.edges.First.pathwayID` 加空集合保护（当前空边表会抛异常）。
6. **缺失值** `Algorithm/subnet_scores.vb:98/121` 直接相乘。按 R `compute_npscore.R:11` 的 `ws[is.na(ws)]<-0` 把权重与表达中的非有限值置 0，并对缺失节点做保护。
7. **未配对 t 检验** `Algorithm/ccs_statistics.vb:86` 与 `estimatetdist.vb:118` 调用 `t.Test(x, y)`；该函数（`Math.Statistics\HypothesisTesting\T-test\Ttest.vb:150`）是**两样本（默认 Welch）**检验，而 R 是 `paired=TRUE`。新增 `Algorithm/ttest.vb` 的 `PairedTTest(x,y)`：先取逐样本差值 `d=x-y`，再走单样本 `t.Test(d, mu:=0).TestValue`；用 `x={2,4,6}, y={1,2,3}` → `t≈3.464101615` 锁定（与 R `t.test(paired=TRUE)$statistic` 一致）。
8. **置换分组大小错误** `Algorithm/estimatetdist.vb:91` 写 `Sample(total.First.samples - 1, d1.First.samples - 1)`，即 `Sample(53, 11)`：既丢掉最后一列（与 Python 同款 bug），又把分组变成 11/43。R 是 `sample(ncol(total), ncol(d1))` = `Sample(54, 12)`。
9. **null 分布未去 NA** R `estimatetdist.R:52` 用 `na.omit(statistics)`；VB 用 `rep(False, ccs.Length)` 原样保留（含 0）。改为只收有限统计量；保留 `length>1000` 提前终止（与 R 一致），但改为显式 `Count` 比较并可通过参数调节。
10. **基因集未对齐 / cbind 缺陷** `Models/R.vb:90-112` 的 `cbind` 缺失基因时 `vec = 0`（长度 1，产生锯齿行，后续必然越界/静默错位），且只遍历 `d1`、丢弃 `d2` 独有基因。新增 `Algorithm/geneAlignment.vb`：进入算法前求两组共同基因并保持 `d1` 顺序（报告剔除数量），`cbind` 的零填充改为 `New Double(n-1) {}`。
11. **结果元数据缺失** `PFSNet.vb:203-206` 只赋 `phenotype1/phenotype2`；在文件路径重载中补充 `tag/class1/class2`（两组文件名），便于 demo 输出。
12. **可复现性** `R.Sample` 走 `Shuffles`（全局随机、不可播种子）。为 `Sample` 增加可选 `rng As Random`（Fisher-Yates）重载，`estimatetdist`/`pfsnet` 增加 `Optional seed As Integer?`，默认 `Nothing` 时保持现有行为，避免破坏既有调用。

### 性能与可靠性

- 热路径：`computew1` 每置换调用一次（54 列 × ~1000 基因），改用 `LinearQuantile` 需先排序；对每列复用一次排序结果（一次 `OrderBy` 同时取 `theta1/theta2` 两个分位数），避免 GK 构造开销且更精确。
- 置换上限：单次置换可能一次追加数百条统计量，`>1000` 即终止（与 R 相同），避免 `n=1000` 次全量重算；demo 默认 `n=100` 并可用 `--n` 覆盖。
- 日志：`cat(".")` 每置换打印会刷屏，改为每 10 次打印一次进度。
- 兼容性：所有新增参数均为 `Optional`，不改既有 `Public` 签名顺序，保证 R# `ExportAPI("PfsNet.Evaluate")` 调用方不受影响。

## 执行要点（防止回归）

- VB 命名陷阱：局部变量不可与所在模块/属性同名（`masked_ggi` 模块内已有同名局部变量，保持原样即可但新代码避免）；`error/next/call/set` 为保留字；`List(Of T)` 在此工程解析为 `Microsoft.VisualBasic.Language.List(Of T)`（带 `Operator >` 比较元素个数），换成 `System.Collections.Generic.List` 会改变语义，`estimatetdist` 中保持原类型、改用显式 `.Count` 判断。
- `Vector.Abs > x` 的布尔求和沿用现有写法但改为显式 `Where(...).Count`，避免依赖向量比较运算符的返回类型。
- 不要照搬 Python 的单侧显著性/固定子网流程，也不要移植其 `num_c+num_t-1` 抽样上界；不要照搬 R 入口未定义的 `obj`。
- 旧 `data/demo.csv` 含重复记录、超范围权重与 `-1.5e14` 级统计量，是修复前的反例，**不得**作为 golden 快照。

## 架构设计

流程保持现有分层不变，仅修正各环节语义：

```mermaid
flowchart TD
    A[LoadData: 表达矩阵 x2 + 通路边表] --> B[geneAlignment: 共同基因对齐]
    B --> C[computew1: 平均秩 + 精确分位数 → 模糊权重]
    C --> D[computegenelist: beta 筛选]
    D --> E[ggi_mask/getMasked_ggi: 边过滤 + 去自环]
    E --> F[ccs: 节点均值权重 + simplify去重 + 连通分量>=5]
    F --> G[subnet_scores: NA置零 → pscore/nscore]
    G --> H[PairedTTest: 逐样本差值 t 统计量]
    B --> I[estimatetdist: 置换 n 次重建子网 → pooled null]
    I --> J[doStatics: p = sum|null|>|stat| / N, 阈值 0.05]
    J --> K[PFSNetResultOut: phenotype1/2 + tag/class1/class2]
```

## 目录结构

```
G:\GCModeller\src\GCModeller\annotations\GSEA\PFSNet\PFSNet\
├── Algorithm\
│   ├── computew1.vb            # [MODIFY] 排名策略改为平均秩(FractionalRanking)；分位数改为精确 LinearQuantile(R type 7)；过滤非有限值；保留 Public 签名
│   ├── masked_ggi.vb           # [MODIFY] InternalVg 节点 weight/weight2 改为“本基因在本组样本上的非缺失均值”；Select 返回 Nothing 时置 0
│   ├── subnet_scores.vb        # [MODIFY] pscoring/nscoring 按 R 把非有限 ws 置 0；缺失节点保护
│   ├── estimatetdist.vb        # [MODIFY] 置换抽样改为 Sample(总样本数, d1样本数)；可选 seeded RNG；只收集有限统计量；Count 上限；改用 PairedTTest；降低日志频率
│   ├── ccs_statistics.vb       # [MODIFY] 改用 PairedTTest；非有限统计量 → pvalue=1、masked=False；p 值用显式计数
│   ├── ttest.vb                # [NEW] 配对 t 检验助手：PairedTTest(x,y)（差值 → t.Test(d, mu:=0).TestValue）+ IsFinite 判定
│   └── geneAlignment.vb        # [NEW] 两组表达矩阵共同基因对齐（保持 d1 顺序）与差异报告
├── Models\
│   ├── Graph.vb                # [MODIFY] simplify：自环 + 无向重复边去重（规范化键）；空边表保护
│   ├── GraphEdge\GraphEdge.vb  # [MODIFY] LoadData 跳过表头/空行/字段不足行，裁剪 BOM/CR/空白；新增无向规范化键辅助
│   ├── R.vb                    # [MODIFY] cbind 零填充改为定长数组；Sample 增加可选 Random 重载
│   └── PFSNetResultOut.vb      # [不改动结构] 由 PFSNet.vb 填充 tag/class1/class2
├── PFSNet.vb                   # [MODIFY] 串联 geneAlignment；透传 seed；填充结果元数据
└── test\
    ├── Program.vb              # [MODIFY] 真实数据 demo + 确定性断言 + CSV 落盘 + 退出码
    └── test.vbproj             # [可选] 如需把 data 复制到输出目录再添加；默认用向上查找路径，无需改动
```

## 关键代码结构

```
' Algorithm/ttest.vb —— 与 R 的 t.test(..., paired=TRUE) 对齐
Public Function PairedTTest(x As Double(), y As Double()) As Double
'   d(i) = x(i) - y(i)；返回 t.Test(d, mu:=0).TestValue
'   已知答案：x={2,4,6}, y={1,2,3} → 3.4641016151377544

' Algorithm/masked_ggi.vb —— 节点权重（对应 R: sum(w[g,])/sum(!is.na(w[g,]))）
Private Function NodeWeight(w As DataFrameRow(), gene As String) As Double
'   取该基因在本组样本上的模糊权重，按非缺失样本数求均值；缺失基因返回 0

' Models/Graph.vb —— simplify 的无向边规范化键
Private Function EdgeKey(pathwayID As String, g1 As String, g2 As String) As String
'   pathwayID & "|" & If(g1 < g2, g1 & "|" & g2, g2 & "|" & g1)
```

## Agent Extensions

### SubAgent

- **code-explorer**
- Purpose：在实现阶段精确核对 `NetworkGraph.DecomposeGraph(minVertices:=)` 的返回语义（是否按顶点数过滤连通分量）、`Apply(axis:=ApplyOnAxis.Row/Column)` 的维度方向，以及 `Vector`/`Index(Of String)` 的既有用法。
- Expected outcome：确认图分解与矩阵 Apply 方向无需改动（或给出需要调整的确切位置），避免引入新的方向性错误。