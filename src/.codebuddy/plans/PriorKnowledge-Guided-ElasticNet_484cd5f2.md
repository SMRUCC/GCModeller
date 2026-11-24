---
name: PriorKnowledge-Guided-ElasticNet
overview: 把 45,001 条 curated TF→target 先验接入 ElasticNet 拟合：(A) 为每个目标基因的候选集注入其 High 置信度先验 TF（按 target 精确匹配，不受 top-k 相关筛选截断）；(B) 对先验支持的边把 L1 惩罚乘以 0.3（自适应惩罚，逐特征权重）。仅 High 置信度生效，未覆盖先验的基因行为与现在完全一致。
todos:
  - id: prior-struct-and-options
    content: 新增 PriorEdge 结构并在 GrnFittingOptions 添加 priorNetwork、priorMinConfidence、priorPenalty 三个字段
    status: completed
  - id: prior-injection
    content: 在 GeneRegulatoryNetwork 构造期建立 target 到先验 TF 的只读映射，InjectPriorityRegulators 优先注入先验 TF（方案 A）
    status: completed
    dependencies:
      - prior-struct-and-options
  - id: adaptive-penalty
    content: 打通逐特征 L1 惩罚：ElasticNetFitting 三处签名加 Optional penalty，CoordinateDescent 按坐标加权，ElasticNetRegressor 与 FitElasticNet 对齐透传（方案 B）
    status: completed
    dependencies:
      - prior-struct-and-options
  - id: rsharp-wiring
    content: TRNtoolkit 把 priorKnowledge 过滤后写入 options 并新增 prior_min_confidence、prior_penalty，elastic_fit.R 增加对应参数
    status: completed
    dependencies:
      - prior-injection
      - adaptive-penalty
  - id: audit-and-verify
    content: 用 [subagent:code-explorer] 审计签名变更的全部调用点，再验证先验为空时逐位一致、先验生效、五个工程 Debug/Release 构建通过
    status: completed
    dependencies:
      - rsharp-wiring
---

## 需求概述

在现有 ElasticNet 基因调控网络拟合流程中接入一份 curated TF→target 先验（45,001 条，其中 High 置信度 15,921 条），通过两种机制提升模型质量：

- **方案 A（候选注入）**：对每个目标基因，把它在先验中的调控 TF 追加进候选集，使其不受 top-k 相关性截断影响。
- **方案 B（自适应惩罚）**：对先验支持的边，把 L1 惩罚乘以 0.3（L2 不变），让这些边更容易在稀疏化中存活。

## 已确认的约束

- 仅使用 **High 置信度**（Confidence ≥ 0.9）的先验边；Medium / Low 不参与任何注入与惩罚。
- 方案 B 的惩罚系数固定为 **0.3**。
- 先验未覆盖的基因（约占 64%）**保持现状**，行为与当前版本逐位一致。
- 不改变扰动预测侧（delta 传播、clamp、library-size）的任何逻辑。
- 不引入 `Network.Regulons` 对 `BNLearn` 工程的依赖，先验以轻量结构传入。

## 预期效果

- 6,613 个有先验调控因子的基因，其模型可以用上已知 TF，而不是仅凭相关性排名；
- 45 个作为 TF 的扰动靶基因（E2F1、EZH2、GLI3、BCL6 等）的下游边更贴近已知调控关系；
- 当 `prior` 为空时，拟合结果与当前版本完全一致，可安全回退。

## 技术栈

沿用现有技术栈，不新增依赖：VB.NET（.NET 10）、`Microsoft.VisualBasic.Math.SIMD` 算子、R# 导出层（TRNtoolkit）、R# 脚本（elastic_fit.R）。

## 现状与改动定位（已核实）

- `TRNtoolkit/Linear.vb`：`elastic_net(...)` 已解析出 `priorKnowledge As RegulatoryEdge()`，但该变量尚未被使用，需要过滤并转换为内部结构后写入拟合参数。
- `Network.Regulons/LinearRegression/GrnOptions.vb`：已有 `regulators`、`priorityRegulators`、`priorityBudget` 等，缺先验相关字段。
- `GeneRegulatoryNetwork.vb`：构造函数把 `priorityRegulators` 解析成只读的 `priorityPool`；`InjectPriorityRegulators(target, candidates)` 负责追加；`FitElasticNet` 在第 1053 行通过 `BuildDesign` 生成列主序 `columns`，随后调用 `ElasticNetRegressor.FitByPathColumns`。
- `ElasticNetRegressor.FitByPathColumnsWithRatio`（第 157 行）调用 `ElasticNetFitting.RegularizationPathColumns`。
- `ElasticNetFitting.vb`：`RegularizationPathColumns`（第 236 行）→ `RunPath`（第 645 行）→ `CoordinateDescent`（第 744 行），内部为标量 `l1 = alpha * l1Ratio`。

## 实现方案

### 1. 轻量先验结构（避免跨工程依赖）

新增 `Network.Regulons/LinearRegression/PriorEdge.vb`：

- `Public Structure PriorEdge`，字段 `TF As String`、`TargetGene As String`、`Confidence As Double`。
- 不引入 `RegulatoryEdge`，由 R# 层完成投影；库侧只负责按 `priorMinConfidence` 过滤。

`GrnFittingOptions` 新增三个字段：

- `priorNetwork As PriorEdge()`（默认 Nothing）
- `priorMinConfidence As Double = 0.9`（High 阈值）
- `priorPenalty As Double = 1.0`（L1 乘子，1.0 表示不启用；调用方传 0.3）

### 2. 方案 A：按 target 精准注入

在 `GeneRegulatoryNetwork` 构造阶段（紧随现有 `priorityPool` 构建之后）建立只读映射：

- `priorAdjacency As Dictionary(Of Integer, HashSet(Of Integer))`：目标基因行号 → 先验 TF 行号集合。
- 构建规则：遍历 `opt.priorNetwork`，`Confidence` 小于 `priorMinConfidence` 的跳过；`TF` 与 `TargetGene` 都要能在 `geneNames` 中大小写不敏感命中的才保留，缺失静默忽略；`TF` 等于 `TargetGene` 的跳过。
- 因为 `Fit()` 用 `Parallel.For` 并发调用 `FitGene`，映射必须在构造期一次性建好、之后只读。

`InjectPriorityRegulators(target, candidates)` 的合并顺序改为：先追加 `priorAdjacency(target)`，再追加全局 `priorityPool`；统一去重、排除目标自身、遵守 `priorityBudget`；日志分别打印两类注入数量（先验 N 个、全局 M 个）。这样先验 TF 优先占用预算。

### 3. 方案 B：逐特征 L1 惩罚

- 在 `FitElasticNet` 中，`candidates` 定稿后构造与列顺序严格对齐的 `penalty As Double()`：`candidates(j)` 属于 `priorAdjacency(target)` 时取 `opt.priorPenalty`，否则 1.0；当 `opt.priorPenalty` 为 1.0 或先验为空时传 Nothing。
- `ElasticNetRegressor.FitByPathColumns` 与 `FitByPathColumnsWithRatio` 增加 `Optional penalty As Double() = Nothing` 并透传。
- `ElasticNetFitting.RegularizationPathColumns`、`RunPath`、`CoordinateDescent` 增加同名可选参数；`CoordinateDescent` 内把 `l1` 改为按坐标取 `l1 * penalty(j)`，`l2` 保持标量。
- `penalty` 为 Nothing 时保留原有标量分支，热循环不增加数组访问，确保既有结果逐位不变。
- `AlphaGrid` 保持现状（用未加权起点）；在注释中说明加权后第一步可能出现少量非零系数，属于预期行为。

### 4. R# 与脚本接线

- `elastic_net` 新增 `prior_min_confidence As Double = 0.9`、`prior_penalty As Double = 0.3`；把 `priorKnowledge` 过滤后投影为 `PriorEdge()` 写入 `options.priorNetwork`，并打印有效边数与覆盖的靶基因数；为空时打印提示且不改变行为。
- `elastic_fit.R` 增加 `--prior-penalty`、`--prior-min-confidence`，默认 0.3 / 0.9，并透传给 `elastic_net`。

## 数据流

先验 CSV → R# `bnlearn::prior_network` → `RegulatoryEdge()` → 按置信度过滤 → `PriorEdge()` → `GrnFittingOptions` → 构造阶段解析为 target→TF 行号映射 → `FitGene` 注入候选并生成 penalty 向量 → 求解器按坐标加权 L1 → 稀疏解。

## 性能与兼容性

- 方案 A 的开销仅为每个基因一次集合查表；High 边 15,921 条，平均每靶基因不足 1 条，内存与时间可忽略。
- 方案 B 在热循环内每坐标多一次数组访问；相对 14,891 维点积可忽略。`penalty` 为空时零开销。
- 向后兼容：`prior` 为 Nothing 或空数组时，`priorAdjacency` 为空、`penalty` 为 Nothing，候选集与拟合结果必须与当前版本逐位一致。
- 与既有机制的关系：先验注入发生在相关性筛选之后，仍受 `regulators` 限定池约束；`maxRowSum` 压缩预算在拟合后照常生效。

## 风险与规避

- 先验 TF 不在限定池内：注入前按池成员过滤，避免"限定池被悄悄放开"。
- 惩罚数组与列顺序错位：`penalty` 必须由同一份 `candidates` 数组生成，并在 `FitElasticNet` 内构建、不跨层缓存。
- 求解器签名变更影响其它调用方：全部以 `Optional` 参数追加，旧调用点无需修改；仍需审计确认。
- VB 陷阱：`target` 已是既有参数名，新增变量避免与其冲突；局部变量不得与函数名仅大小写不同。

## Agent Extensions

### SubAgent

- **code-explorer**
- Purpose: 审计 `RegularizationPathColumns`、`FitByPathColumns`、`FitByPathColumnsWithRatio` 三个方法签名变更后的全部调用点（含 Moira、测试工程、R# 层），确认新增可选参数不会破坏其它消费者。
- Expected outcome: 输出"文件绝对路径 + 行号 + 是否仅依赖签名"的清单，并指出必须同步修改的调用点；若发现除本计划外的调用方，在实施前纳入改动范围。