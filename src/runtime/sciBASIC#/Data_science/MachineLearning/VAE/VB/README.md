# VariationalBayes —— 变分贝叶斯模型族

本目录是 `VAE.vbproj` 内的均值场变分贝叶斯（mean-field Variational Bayes）实现，
复用项目已有的张量运行时与线性代数库，提供统一的坐标上升（coordinate ascent）训练框架
以及三个具体模型。

```
VAE/VB/
├── NamespaceDoc.vb                     命名空间文档
├── VBMath.vb                           digamma / trigamma / logSumExp / 熵与 Wishart 归一化
├── MatrixBridge.vb                     Tensor <-> NumericMatrix 桥接、log|A|、稳健求逆
├── VariationalInference.vb             坐标上升模板基类（收敛监控 + 下界历史 + 迭代事件）
├── VBGMM/VariationalGaussianMixture.vb 变分贝叶斯高斯混合（Dirichlet + Normal-Wishart）
├── VBLinearRegression/VariationalLinearRegression.vb
└── VBPCA/VariationalPCA.vb             变分贝叶斯 PCA（ARD 自动降维，支持缺失值）
```

## 设计约定

- **公共数据接口统一用 `Tensor`**（`Microsoft.VisualBasic.MachineLearning.TensorFlow`），
  与同项目的 `GMM` / `GMVAE` / `GraphVAE` 保持一致。
- **矩阵分解下沉到 `NumericMatrix`**（`Microsoft.VisualBasic.Math.LinearAlgebra.Matrix`）：
  `Inverse` / `LUD` / `CholeskyDecomposition` 用于 `ln|Σ|`、`Σ⁻¹` 与正定性检查。
  转换只发生在参数更新的边界（每次迭代 `O(K·D²)` 次），不在逐样本循环内。
- **特殊函数复用** `Microsoft.VisualBasic.Math.Statistics.SpecialFunctions.gammaln`；
  项目原本缺失的 `digamma` / `trigamma` 在 `VBMath.vb` 中补齐（递推 + 渐近展开）。
- **对数域计算**：责任度用 `ln ρ_nk` 计算，按行做 `logSumExp` 归一化，避免下溢。
- 所有模型对外暴露 `ELBO`、`ELBOHistory`、`Converged`、`NIter`，并通过
  `IterationCompleted` 事件支持训练监控。

## 迭代骨架

`VariationalInference.Fit` 固定执行：

```
InitializeModel  →  [ Expectation → Maximization → ComputeELBO ] × MaxIter  →  OnConverged
```

子类只实现四个钩子方法。收敛判定使用**相对变化量**
`|ΔELBO| ≤ max(Tolerance, Tolerance·|ELBO|)`，因此不同数据尺度下阈值可以复用。

## 1. VB-GMM（`VariationalGaussianMixture`）

```
π  ~ Dirichlet(α0)
μk ~ N(m0, (β0 Λk)^-1),  Λk ~ Wishart(W0, ν0)
zn ~ Cat(π),  xn | znk=1 ~ N(μk, Λk^-1)
q(Z)=Π r_nk,  q(π)=Dirichlet(α),  q(μ,Λ)=Π N·Wishart
```

- E 步：`ln ρ_nk = E[ln πk] + ½E[ln|Λk|] − D/2·ln2π − ½(D/βk + νk·(xn−mk)'Wk(xn−mk))`
- M 步：`αk=α0+Nk`、`βk=β0+Nk`、`νk=ν0+Nk`、
  `mk=(β0m0+Nkx̄k)/βk`、
  `Wk⁻¹ = W0⁻¹ + Nk·Sk + (β0Nk/βk)(x̄k−m0)(x̄k−m0)'`
- ELBO：Bishop 10.70–10.77 的七项分解（含 Dirichlet / Wishart 熵）

关键属性：`Alpha`、`Weights`、`Means`、`Covariances`、`Responsibilities`、
`EffectiveComponents`（`αk − α0 > PruneThreshold` 的成分数）、`Predict` / `PredictProba` / `Score`。

```vb
Dim model As New VariationalGaussianMixture(nComponents:=8, maxIter:=300, seed:=42)
model.DirichletAlpha0 = 0.001   ' 小值驱动成分剪枝
model.Fit(X)                    ' X: Tensor [N, D]
Dim labels = model.Predict(X)
```

> 变分混合模型对初值敏感，建议像演示程序那样做多次随机重启、保留下界最大的模型。

## 2. VB 线性回归（`VariationalLinearRegression`）

```
w|α ~ N(0, α⁻¹I),  α ~ Gamma(a0,b0)
t|w,β ~ N(Φw, β⁻¹I),  β ~ Gamma(c0,d0)
q(w)=N(m,S), q(α)=Gamma(a,b), q(β)=Gamma(c,d)
```

更新：

```
S⁻¹ = E[α]I + E[β]Φ'Φ,        m = E[β]SΦ't
b = b0 + ½(m'm + tr S),        a = a0 + M/2
d = d0 + ½(||t−Φm||² + tr(SΦ'Φ)),  c = c0 + N/2
```

```vb
Dim design = VariationalLinearRegression.DesignMatrix(X, addIntercept:=True)
Dim model As New VariationalLinearRegression()
model.Fit(design, y)            ' y: Tensor [N] 或 [N,1]
Dim mu = model.Predict(design)
Dim sigma2 = model.PredictiveVariance(design)   ' 1/E[β] + φ'Sφ
```

## 3. VB-PCA（`VariationalPCA`）

```
x_n = W z_n + μ + ε,  ε ~ N(0, β⁻¹I),  z_n ~ N(0, I)
w_d ~ N(0, diag(α)⁻¹)      ' ARD 先验
q(Z)=Π N(m_n, Σz_n), q(W)=Π N(m_wd, Σw), q(α), q(β)
```

- `Σz = (I + E[β]E[W'W])⁻¹`，`m_n = E[β]ΣzE[W]'(x_n−μ)`
- `Σw = (diag(E[α]) + E[β]E[Z'Z])⁻¹`（与维度 `d` 无关，每次迭代只需一次 `L×L` 求逆）
- μ 使用观测数据的列均值（经典 PCA 中心化）；载荷默认用协方差矩阵的前 L 个主方向初始化
- `NaN` 视为缺失值，只使用观测维度参与更新与下界计算

```vb
Dim model As New VariationalPCA(latentDim:=6, maxIter:=1000, tol:=1e-4)
model.Fit(X)
Dim w = model.Components          ' [D, L]
Dim z = model.Scores              ' [N, L]
Dim dims = model.EffectiveDimension
Dim rmse = model.ReconstructionError(X)
```

`EffectiveDimension` 使用载荷能量 `E[||w_i||²]` 的相对阈值判定，
对应 ARD 中 `α_i → ∞` 的剪枝行为。

## 复杂度

| 模型 | 每次迭代 |
| --- | --- |
| VB-GMM | `O(N·K·D²)` + `K` 次 `D×D` 求逆 |
| VB 回归 | `O(N·M² + M³)` |
| VB-PCA | `O(N·D·L² + N·L³ + L³)` |

## 演示与审计

`test/VBDemo.vb` 覆盖三个模型的合成数据审计（下界单调性、责任度归一化、聚类精度、
系数恢复、ARD 有效维度、重构误差、缺失值路径），断言经由 `DebugAudit` 记录，
结束时在 `bin/<Configuration>/<TFM>/vb-audit-report.json` 生成 JSON 报告。

```powershell
# 只运行 VB 审计（跳过 GMM / GraphVAE 演示）
dotnet run --project test/test.vbproj -c Debug -- --vb-only
```

> `DebugAudit.Snapshot` / `DebugAudit.Check` 带有 `<Conditional("DEBUG")>`，
> 因此需要以 Debug 配置运行才能得到完整的变量快照。
