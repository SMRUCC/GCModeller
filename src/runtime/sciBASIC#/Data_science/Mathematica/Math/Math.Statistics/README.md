# 统计工具箱：分布、假设检验与更多

## 引言

本包是 `sciBASIC#` 的**统计中枢**：分布建模、假设检验、描述统计、序统计与重采样，以及几类高级方法（ICA、RANSAC、Shapley 值）都集中在这里，供数据挖掘、机器学习与绘图包共同调用。

## 核心能力

### 概率分布（`Distributions`）

- 常见概率分布模型与密度函数；
- **参数估计**：矩估计（`MethodOfMoments`）与 L-矩估计（`LinearMoments`）。

### 正态分布数值函数（根命名空间 `NormalDistribution`）

- `Erf` / `Erfc`：幂级数（`|x| ≤ 2`）与连分式（`|x| > 2`）分段，尾部保持相对精度；
- `CDF` / `CCDF` / `PDF` / `LogPDF`：上尾概率直接由 `erfc` 给出，避免 `1 - Φ(z)` 的相消；
- `InvCDF` / `InvCCDF`：Acklam 逼近 + Halley 精化；`InvCCDF` 走对称性，**不构造 `1 - q`**，
  因此 `q = 1e-16` 这类极端 P 值仍可精确反解；
- `TwoSidedPValue` / `SignedScore`：P 值与带符号正态分数互转。

> `Distributions.MethodOfMoments.Normal` 的 `GetCDF` / `GetPDF` / `GetInvCDF` 已改为委托到这里：
> 旧实现用梯形数值积分，且 `1 \ 250`、`(1 \ 2)` 是 VB 整数除法（结果为 0），PDF 的归一化常数也写错了一位。

### 假设检验（`Hypothesis`）

- 常用参数检验（t 检验、F 检验、卡方检验等）；
- **Fisher 精确检验**（`FishersExact`）：小样本列联表；
- **Mantel 检验**（`Mantel`）：两个距离矩阵之间的相关性检验；
- **Wilcoxon 秩和检验**（`Wilcoxon`，等价于 Mann-Whitney U）：
  - 两样本一次性入口 `Wilcoxon.Test(x, y)`，小样本无并列时自动走**精确法**（枚举 U 的精确分布）；
  - 逐基因批量入口 `Wilcoxon.TestTable(control, sampleCpm)`：以 `ControlOrderStats`
    缓存的对照序统计为基准，每基因 `O(n log m)`，并自动做 BH 多重校正；
  - 并列校正 `tc = 1 - Σ(t³-t)/(N³-N)`，全并列时退化为 `p = 1` 并给出诊断。

### 序统计与分位数映射（`OrderStatistics`）

- `OrderStatisticsTable`：逐基因升序序统计表（可缓存），提供
  `LowerBound` / `UpperBound` / `CountEqual` / `ShiftedQuantile` / `Type7Quantile`；
- `QuantileProfile`：分层分位剖面，支持 `RescaleTo`（总和守恒）、`Spread`（幂次调整离散度）、
  `AsIntegers`（最大余数法转整数）；
- `Quantiles`：无状态工具（`Stratified`、`Type7`、`Interpolate`、`Median`、`ShiftedIndex`）。

### 单细胞计数（`SingleCell`）

- `SingleCellCounts`：CSR 计数矩阵包装，构造时校验"非负整数且小于 2^24"；
- `CountStatistics`：分块 CPM 均值、检出率、深度分位、库大小剖面、delete-one **Jackknife 矩**
  （流式 `Σv² - (Σv)²/n`，不需要 `n × G` 的浮点矩阵）。

> 底层的 `CsrMatrix` / `CscMatrix` 位于 `Microsoft.VisualBasic.Math.Core` 的
> `LinearAlgebra.Matrix.Compressed` 命名空间，任何项目都可以直接复用。

### 元分析（`MetaAnalysis`）

- `RandomEffects.Pool` / `PoolMatrix`：DerSimonian-Laird 的 τ²、逆方差权重、`1/(v + τ²)` 加权合并；
- `Heterogeneity`：Cochran Q、I²、τ²；
- `EvidenceCombination`：加权均值（两种缺失语义）、**Stouffer / Liptak** 合并、有效研究数、
  最大权重占比；
- 缺失方差可按同组中位数回填；方差有绝对地板与相对地板；权重集中度与有效研究数作为诊断量返回。

### 整数分摊（`Apportionment`）

- `Apportion.LargestRemainder`：最大余数法，总和精确、每项偏差小于 1、并列按下标稳定；
- `Apportion.ConserveMass`：把现有整数调整到目标总和（补足按优先顺序均摊，
  削减用**水位法**从最大值削平，`O(n log n)` 且不会产生负值）。

### Copula（`Copula`）

- `GaussianCopula.Mix`：`e = r·z + √(1-r²)·e0`（Iman-Conover 类方法的连续版本），
  在**不改变边缘分布**的前提下注入指定秩相关；
- `GaussianCopula.RanksToQuantiles`：名次 → `(i+0.5)/n` 分位锚点，同时给出降序名次；
- `GaussianCopula.FindCorrelation`：把任意目标函数交给一维求根器，不收敛时返回 `Capped` 而不是抛异常；
- `GaussianCopula.Spearman`：平均秩的秩相关，用于诊断注入效果。

### 描述统计（`MomentFunctions`）

均值、方差、偏度、峰度等矩统计量。

### 高级方法

| 命名空间 | 方法 | 用途 |
|---|---|---|
| `ShapleyValue` | Shapley 值归因 | 公平分配特征贡献 |
| `ShapleyValue.TreeShap` | TreeSHAP | 树模型预测的精确归因 |
| `FastICA` | 独立成分分析 | 盲源分离（如去除生理噪声） |
| `RANSAC` | 随机采样一致性 | 含离群点时的稳健模型拟合 |

## 命名空间地图

| 命名空间 | 职责 |
|---|---|
| `Microsoft.VisualBasic.Math.Statistics`（根） | `NormalDistribution` 与统计入口 |
| `....Statistics.Distributions`（+ `LinearMoments` / `MethodOfMoments`） | 分布与参数估计 |
| `....Statistics.Hypothesis`（+ `FishersExact` / `MantelTest` / `Wilcoxon`） | 假设检验与秩和检验 |
| `....Statistics.OrderStatistics` | 序统计表、分位剖面与分位数工具 |
| `....Statistics.SingleCell` | 单细胞计数包装与计数算子 |
| `....Statistics.MetaAnalysis` | 随机效应元分析、异质性与证据合并 |
| `....Statistics.Apportionment` | 整数分摊与质量守恒 |
| `....Statistics.Copula` | Gaussian copula 与秩相关反演 |
| `....Statistics.MomentFunctions` | 矩统计量 |
| `....Statistics.ShapleyValue`（+ `TreeShap`） | Shapley 值归因 |
| `....Statistics.RANSAC` | 稳健拟合 |
| `Microsoft.VisualBasic.Math.Numerics.RootFinding`（Math.Core） | 一维求根（二分/割线/不动点） |
| `Microsoft.VisualBasic.Math.LinearAlgebra.Matrix.Compressed`（Math.Core） | CSR / CSC 压缩稀疏矩阵 |
| `Microsoft.VisualBasic.Math.RandomSources`（Randomizer） | `IRandomSource` 与 MT19937 实现 |

## 快速上手

```vbnet
Imports Microsoft.VisualBasic.Math.Statistics
Imports Microsoft.VisualBasic.Math.Statistics.Hypothesis
Imports Microsoft.VisualBasic.Math.Statistics.MetaAnalysis

' 1. 描述统计
Dim moments = MomentFunctions.Compute(values)

' 2. 正态分位（尾部精确）
Dim z = NormalDistribution.InvCCDF(0.025)      ' ≈ 1.959963984540054

' 3. 两样本秩和检验（小样本自动精确法）
Dim rankTest = Wilcoxon.Test(groupA, groupB)

' 4. 随机效应元分析
Dim pooled = RandomEffects.Pool({
    StudyEffect.Create(0.8, 0.04),
    StudyEffect.Create(1.1, 0.09),
    StudyEffect.Create(0.6, 0.16)
})

' 5. 整数分摊（总和精确守恒）
Dim counts = Apportionment.Apportion.LargestRemainder(fractions, 400000L)

' 6. Shapley 值归因（TreeSHAP 面向树模型）
Dim shap = ShapleyValue.TreeShap.Explain(treeModel, instance)
```

## 实现要点

- **为什么需要多种参数估计方法**：矩估计简单但受离群点影响大；L-矩（线性矩）基于有序统计量，对小样本与重尾分布更稳健——这正是水文学、极值分析等领域偏爱它的原因。
- **参数检验 vs 精确检验**：卡方检验依赖大样本近似；当期望频数过小时必须改用 Fisher 精确检验。秩和检验同理：样本量小且无并列时，精确法的 P 值会比正态近似更保守、更可信。
- **"未测"与"测得 0"必须分开**：元分析的 `IsMeasured`、加权均值的 `missingAsZero`、序统计表的缺失语义都是显式选项，避免缺失值被静默当成 0 参与平均。
- **不收敛不抛异常**：一维求根、相关系数搜索、元分析合并都用"结果对象 + 标志位"表达退化，
  由调用方决定是否接受。
- **Shapley 值的代价**：原始定义需要枚举全部特征子集（指数级），因此实践中使用 TreeSHAP 这类针对树模型的多项式算法。
- **FastICA 与 PCA 的分工**：PCA 找的是**不相关**的成分（二阶统计量），FastICA 找的是**统计独立**的成分（高阶统计量）；后者更适合「多个独立信号线性混合」的场景。

## 包信息

- Assembly：`Microsoft.VisualBasic.Math.Statistics`
- TargetFramework：`net10.0`
- Tags：`scibasic;statistics;hypothesis-testing;distribution;moment-functions;wilcoxon;meta-analysis;order-statistics;copula;sparse;single-cell;fastica;ransac;shapley-value;treeshap`
- 许可：GPL-3.0-or-later
