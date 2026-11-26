# 核心数学库：线性代数、数值方法与更多

## 引言

`Math` 是 `sciBASIC#` 数学栈的基座：上层的数据分析、机器学习与绘图都建立在这里提供的数据结构（矩阵 / 分布 / 插值 / 采样）与求解器（线性系统 / 优化 / 线性规划）之上。

它覆盖的领域相当广，可以按「你在做什么」来定位：

| 你要做的事 | 去哪里找 |
|---|---|
| 解线性方程组 / 求 SVD | `LinearAlgebra`、`LinearAlgebra.Solvers` |
| 高维矩阵只要前 k 个奇异三元组 | `LinearAlgebra.Matrix.RandomizedSVD`（随机化 + 幂迭代） |
| 做主成分分析（载荷 + 解释方差） | `LinearAlgebra.Matrix.TruncatedPCA` |
| 做线性规划 / 整数规划 | `LinearAlgebra.LinearProgramming`（含 `MILP`、`IPMCrossover`） |
| 求最优解（含边界约束） | `Numerics.Framework.Optimization.LBFGSB` |
| 描述随机变量 / 分箱统计 | `Distributions`、`Distributions.BinBox` |
| 对时间序列降采样 | `DownSampling`（LTTB / MinMax / Mixed / TimeGap） |
| 插值 / 求分位数 | `Interpolation`（样条）、`Quantile` |
| 解析并执行数学表达式 | `Scripting`（含 `MathExpression`、BasicR 桥） |
| 近似集合相似度 | `HashMaps.MinHash` |
| 模糊推理 | `Logical.FuzzyLogic` |
| 高精度十进制运算 | BigDecimal 算术支持 |

## 命名空间地图

| 命名空间 | 职责 |
|---|---|
| `Microsoft.VisualBasic.Math`（根） | 数值辅助与共享类型 |
| `....Math.Algebra`（+ `LP` / `LP.IPMCrossover` / `Matrix.NET` / `Matrix.NET.MDS` / `Matrix.NET.Decomposition` / `MILP` / `Solvers`） | 线性代数、线性规划、矩阵（含随机化 SVD / 截断 PCA）与求解器 |
| `....Math.Numerics`（+ `Framework` / `Optimization` / `Optimization.LBFGSB`） | 数值方法与优化框架 |
| `....Math.Distributions`（+ `BinBox` / `Summary`） | 概率分布与分箱 |
| `....Math.DownSampling`（+ `LargestTriangleBucket` / `MaxMin` / `Mixed` / `TimeGap`） | 时间序列降采样 |
| `....Math.Quantile` / `....Math.Spline` | 分位数与样条插值 |
| `....Math.Scripting`（+ `Expression` / `R`） | 数学表达式脚本 |
| `....Math.HashMaps` / `....Math.Logical.FuzzyLogic` | MinHash 与模糊逻辑 |

## 快速上手

```vbnet
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.Numerics.Framework.Optimization.LBFGSB

' 1. 线性代数：求解 Ax = b
Dim x = Matrix.Solve(A, b)

' 2. 有界优化：L-BFGS-B
Dim result = LBFGSB.Minimize(objective, lower, upper, x0)

' 3. 降采样：把曲线压到 500 个点（保留形状特征）
Dim reduced = DownSampling.LargestTriangleBucket(source, points:=500)

' 4. 随机化截断 SVD：只要前 160 个奇异三元组，不把矩阵稠密化
Dim svd As New LinearAlgebra.Matrix.RandomizedSVD(A, 160,
        oversampling:=8, powerIterations:=2, seed:=0)
Console.WriteLine($"rank={svd.Rank}, retained={svd.RetainedRank}")

' 5. 截断 PCA：样本 × 特征，取特征空间前 k 个载荷与解释方差
Dim pca = LinearAlgebra.Matrix.TruncatedPCA.Fit(cellsByGenes, 100,
        center:=True, powerIterations:=2, seed:=0)
Console.WriteLine($"explained = {pca.ExplainedVarianceRatio.Sum():P2}")
```

### 随机化 SVD 与截断 PCA

| 类型 | 输入 | 输出 | 说明 |
|---|---|---|---|
| `RandomizedSVD` | 稠密数组 / `SparseMatrix` / `CsrMatrix` | `U`、`SingularValues`、`V`、`Components`、`ReducedMatrix` | Halko–Martinsson–Tropp 算法；`ℓ = k + oversampling`，幂迭代每轮两次 QR 重正交，按 `eps·max(m,n)·max(σ₁,1)` 截断有效秩 |
| `TruncatedPCA` | 稠密数组 / `CsrMatrix`（样本 × 特征） | `Components`（载荷 k×p）、`Scores`、`ExplainedVariance`（σ²）、`Mean`/`Scale` | 把列中心化/标准化表达成算子，不物化中心化矩阵；`Project`/`InverseTransform` 构成闭环 |
| `TruncatedSVD` | `SparseMatrix` | 同上（固定 k 列） | 兼容入口：内部委托 `RandomizedSVD`，保留历史默认参数与尺寸契约 |

- **为什么要随机化**：全量 SVD 的代价是 `O(m·n²)` 且需要把稀疏矩阵稠密化；随机化版本只依赖 `A·x` / `Aᵀ·y`，复杂度降到 `O(nnz·ℓ·(2q+3) + (m+n)·ℓ² + ℓ³)`，在 `m=18000, n=1024, k=160` 这个量级上是"能跑"与"跑不动"的区别。
- **为什么要两次 QR 重正交**：幂迭代把 `(AAᵀ)^q` 作用在随机子空间上，条件数被幂次放大；每次迭代对中间结果重新正交化可以把舍入误差压回机器精度量级。
- **为什么默认对 `QᵀA` 做 SVD（而不是对 Gram 矩阵做特征分解）**：Gram 矩阵把条件数平方，小奇异值的相对精度会明显变差；`GramEigen` 路径保留下来仅供与早期实现做对拍。
- **可复现性**：随机测试矩阵由 `New Random(seed)` 加本地 Box–Muller 生成，同一 seed 逐位一致；不使用全局共享的随机源。

## 实现要点

- **稠密与稀疏并存**：小规模稠密问题用稠密矩阵（缓存友好），大规模稀疏问题用稀疏表示（避免零元素存储与运算）。
- **L-BFGS-B 的适用面**：多数实际优化问题都有变量边界（浓度 ≥ 0、比例 ∈ [0,1]）；L-BFGS-B 在保持拟牛顿法收敛速度的同时处理边界，因此是默认选择。
- **降采样方法为何有多种**：LTTB 保留整体形状（适合展示趋势），MinMax 保留极值（适合展示波动），TimeGap 按时间间隔抽稀（适合不均匀采样）。选择取决于"你想让读者看到什么"。
- **表达式脚本的意义**：把公式写成文本而非编译进代码，意味着公式可以由用户在运行时提供——这是报表与可配置分析的基础。

## 包信息

- Assembly：`Microsoft.VisualBasic.Math`
- TargetFramework：`net10.0`
- Tags：`scibasic;mathematics;linear-algebra;sparse-matrix;svd;numerics;linear-programming;bigdecimal;spline;quantile;expression-scripting`
- 许可：GPL-3.0-or-later
