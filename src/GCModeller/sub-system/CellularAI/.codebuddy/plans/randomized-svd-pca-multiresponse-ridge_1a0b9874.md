---
name: randomized-svd-pca-multiresponse-ridge
overview: 在 sciBASIC# 的两个项目中补齐 CellularAI 复现所需的三个数值组件：Math.NET5.vbproj 中的随机化 SVD（含幂迭代与 QR 重正交）与截断 PCA（基因空间 top-k 载荷 + 解释方差）；linear-netcore5.vbproj 中的多响应岭回归（逐基因、带行掩码）。并配套基于 DebugAudit 的自检报告。
todos:
  - id: impl-randomized-svd
    content: 新增 RandomizedSVD.vb：种子 Omega、幂迭代双 QR 重正交、SmallSVD/GramEigen 双路径、容差秩、稠密/SparseMatrix/CsrMatrix 三入口
    status: completed
  - id: refactor-truncated-svd
    content: 用 [skill:lsp-code-analysis] 查全部 TruncatedSVD 引用后改为委托引擎，保留原 API、尾部追加 seed/solver
    status: completed
    dependencies:
      - impl-randomized-svd
  - id: impl-truncated-pca
    content: 新增 TruncatedPCA.vb：中心化/标准化算子包装、基因空间载荷、σ²解释方差、得分/投影/逆变换
    status: completed
    dependencies:
      - impl-randomized-svd
  - id: math-audit
    content: 新增 RandomizedSvdAudit/TruncatedPcaAudit 并在 ProgramMilp 注册 rsvd、pca 子命令，用 DebugAudit 导出 JSON 报告
    status: completed
    dependencies:
      - refactor-truncated-svd
      - impl-truncated-pca
  - id: impl-multi-response-ridge
    content: 新增 Ridge/MultiResponseRidge.vb：Fit/Predict/BatchFit、行掩码、特征中心、Cholesky 与 QR 回退、逐基因并行
    status: completed
  - id: ridge-audit-and-docs
    content: 新增 MultiResponseRidgeAudit 并接入 _smoketest/Program.vb，用 DebugAudit 出报告；同步 NamespaceDoc、README、两个 vbproj 发布说明
    status: completed
    dependencies:
      - impl-multi-response-ridge
---

## 产品概述

在 sciBASIC# 的两个数学项目中补齐一套用于复现 CellularAI（虚拟细胞扰动响应预测）的数值算法组件：随机化 SVD、截断 PCA、多响应岭回归，并配套基于 DebugAudit 的自检报告。属于纯计算库能力补全，无界面。

## 核心功能

### 一、随机化 SVD（Math.NET5.vbproj）

- 以 Halko–Martinsson–Tropp 随机化算法计算矩阵前 k 个奇异三元组（U、Σ、V），支持子空间幂迭代与 QR 重正交。
- 随机测试矩阵 Ω 由可指定 seed 的局部随机源生成；同一 seed 重复调用结果逐位一致。
- 支持三种输入形态：稠密锯齿数组、SparseMatrix、CsrMatrix，三者在同一 seed 下结果一致。
- 提供"按容差截断有效秩"的语义：以 `eps·max(m,n)·max(σ₁,1)` 为阈值给出实际秩与截断后的分量。
- 输出左奇异向量、奇异值、右奇异向量、分量矩阵（Vᵀ）、降维矩阵（U·Σ），并给出请求秩、实际秩、谱容差等诊断量。
- 保留既有 TruncatedSVD 的公开 API 与数值语义（新参数以尾部可选参数形式追加），使其成为新引擎的薄封装。

### 二、截断 PCA（Math.NET5.vbproj）

- 以"样本 × 特征"约定拟合主成分，输出特征空间（基因空间）前 k 个载荷、奇异值、解释方差与解释方差比。
- 支持中心化（默认开启）与标准化（默认关闭），中心化/尺度参数可持久化，供后续对新样本做同一变换。
- 提供得分矩阵（=U·Σ）、投影（Project）与逆变换（InverseTransform），使"训练—投影—还原"闭环可用。
- 解释方差明确采用未除以样本数的形式，与上游 CellularAI 的口径一致；同时给出归一化的解释方差比。
- 支持稠密与稀疏输入，k 越界或非法参数给出明确异常，空输入返回空结果。

### 三、多响应岭回归（linear-netcore5.vbproj）

- 一次求解多个响应列（f×k 系数矩阵）的闭式岭回归：`(XᵀX + λI)·B = XᵀY`。
- 支持行掩码：只有被掩码选中的样本行参与拟合，用于"逐基因只在有效记录上回归"的场景。
- 特征中心化在参与行上完成，并保存中心向量，预测时按同一中心做减除。
- 提供逐基因批量求解入口：对每个基因独立拟合（各自一套小法方程），支持最小样本数门限（不足则整体跳过并标记未支持），并按基因并行且结果与并行度无关。
- 数值路径以 Cholesky 为主，非正定时回退到 QR/LU，保证秩亏数据下不产生 NaN/Inf。
- 暴露系数、特征中心、支持标记、实际秩、残差平方和、每响应 R²、参与样本数等诊断量，并提供预测接口。

### 四、自检与报告

- 两个项目各自新增审计模块，全部断言走 DebugAudit，结束时导出 JSON 测试报告，失败数为 0 时退出码为 0。
- 审计覆盖：奇异值精度、正交性、重构误差、seed 可复现性、三种输入形态一致性、容差秩判定、PCA 方差分解与投影往返、岭回归退化到最小二乘、掩码等价性、收缩单调性、批量与逐基因一致、异常输入防护。

## 技术栈

- 语言/运行时：VB.NET（`net10.0`），沿用两个项目现有 SDK 式工程（隐式编译，新增源码放入项目目录即自动纳入，不修改 ItemGroup 的编译项）。
- 数学基础：复用同解决方案已有的 `NumericMatrix`（LU/QR/Cholesky/SVD/Solve）、`SparseMatrix`、`CsrMatrix`、`QRDecomposition`、`SingularValueDecomposition`、`CholeskyDecomposition`、`EigenvalueDecomposition`、`RectangularArray`、`Vector`。
- 测试框架：`Microsoft.VisualBasic.Core` 的 DebugAudit（用户强制要求），不使用外部测试框架、不引入 NuGet 依赖。

## 关键事实核对（均已验证）

- `Math.NET5.vbproj`：`RootNamespace=Microsoft.VisualBasic.Math`，`AssemblyName=Microsoft.VisualBasic.Math.Core`，`GenerateDocumentationFile=True`；显式排除 `test\**`、`Numerics\Optimization\LBFGSB\examples\**`、`packages\**`。
- `linear-netcore5.vbproj`：`RootNamespace=Microsoft.VisualBasic.Data.Bootstrapping`，`AssemblyName=Microsoft.VisualBasic.Data.Bootstrapping.Fittings`；引用 `Core.vbproj` 与 `..\Math\Math.NET5.vbproj`；显式排除 `Test\**`、`_smoketest\**`。
- 已存在 `Algebra\Matrix.NET\Decomposition\TruncatedSVD.vb`（`Namespace LinearAlgebra.Matrix`）：仅接受 `SparseMatrix`，Ω 取自全局 `RandomExtensions.NextGaussian()`（不可播种），最终用 `EigenvalueDecomposition` 对 `QᵀAAᵀQ` 求特征值（Gram 路径，条件数平方），无容差秩语义。
- `QRDecomposition.Q` 为 **economy 尺寸 m×n**（内部 `New NumericMatrix(m, n)`），对 m=18000、n=170 的场景安全，不会构造 m×m。
- `SingularValueDecomposition.SingularValues` 返回 `Vector`（非 `Double()`），取值需 `.ToArray()`；该类无 `Solve`，矩形最小二乘回退应走 `NumericMatrix.Solve`（方阵走 LU，非方阵走 QR）。
- `CholeskyDecomposition` 具 `SPD` 属性与 `Solve(GeneralMatrix)`；`NumericMatrix` 有 `New(A As Double()(), m, n)`、`New(A As Double()())`、`Identity(n)`、`ArrayPack()`、索引器 `A(i, j)`、`RowDimension/ColumnDimension`。
- 稀疏乘法可用：`SparseMatrix.Multiply/MultiplyTranspose`、`CsrMatrix.Multiply/MultiplyTranspose`，均为 O(nnz)。
- 参考样板：`Ridge\RidgeFitting.vb`（列中心化/标准化 + `chol()` 求解 `(R+λI)β=xty` + 系数还原的写法与中文文档风格）。

## 实现方案

### 1. 随机化 SVD 引擎（新增 `Algebra\Matrix.NET\Decomposition\RandomizedSVD.vb`）

命名空间与 `TruncatedSVD` 保持一致（`LinearAlgebra.Matrix`），类名 `RandomizedSVD`。

算法步骤（严格对齐 Halko 等人 Algorithm 4.4 与上游 `response_basis` 的数值路径）：

1. `ℓ = min(k + oversampling, min(m, n))`，高斯随机矩阵 Ω（n×ℓ）由**局部种子随机源**生成（`New Random(seed)` + 本地 Box–Muller，不触碰全局 Random）。
2. 范围查找：`Y = A·Ω` → `Q = qrEconomy(Y)`。
3. 子空间幂迭代 q 次，每轮**两次 QR 重正交**：`Z = qr(Aᵀ·Q)`，`Q = qr(A·Q)`。
4. 小矩阵分解：`B = Qᵀ·A`（ℓ×n），对 B 求 SVD 得 `Ub, sv, Vt`，则 `U = Q·Ub`、`Σ = sv`、`V = Vtᵀ`。此路径避免 Gram 矩阵平方条件数。
5. 容差截断：`tol = eps·max(m,n)·max(sv₀,1)`，实际秩 `r = #{sv > tol}`，输出时取前 `min(k, r)` 个分量。
6. 降维矩阵 `ReducedMatrix = U·Σ`（m×k）。

设计要点：

- 内部抽象出 `apply(v)` / `applyTranspose(v)` 两个委托，三份公开构造重载（`Double()()`, `SparseMatrix`, `CsrMatrix`）只负责绑定委托，主流程只有一份，保证三条入口数值完全一致。
- 提供 `Solver` 选项枚举：`SmallSVD`（默认）与 `GramEigen`（保留旧路径，供向后兼容与对拍）。Gram 路径复用 `EigenvalueDecomposition`，并在文档中标注其精度代价。
- 空矩阵与 `k=0` 返回空结果而不抛异常（沿用 `TruncatedSVD` 现有边界约定）；`k<1`、`k>min(m,n)`、`oversampling<0`、`powerIterations<0` 抛带参数名的 `ArgumentException`。
- 复杂度写在文档中：`O(nnz·ℓ·(2q+3) + (m+n)·ℓ² + ℓ³)`，空间 `O(nnz + (m+n)·ℓ)`。
- 诊断量：`RequestedRank`、`Rank`、`SpectrumTolerance`、`Oversampling`、`PowerIterations`、`Solver`、`OperatorApplications`（乘法次数，便于核对性能与回归）。

### 2. 重构 TruncatedSVD（修改既有文件）

- 将主流程委托给 `RandomizedSVD`，公开成员 `U / SingularValues / V / Components / ReducedMatrix` 与共享入口 `Reduce(...)` 全部保留，签名只允许追加尾部可选参数（`Optional seed As Integer = 0`、`Optional solver As ... = SmallSVD`）。
- 保留 `oversampling:=10`、`powerIterations:=1` 的旧默认值，避免既有调用行为漂移；内部把 `ℓ` 的截断交给引擎处理。
- 既有 `test\TruncatedSVDTest.vb` 只断言容差、不要求逐位一致，因此数值路径替换安全；重构后必须回归该测试。

### 3. 截断 PCA（新增 `Algebra\Matrix.NET\Decomposition\TruncatedPCA.vb`）

命名空间 `LinearAlgebra.Matrix`，类名 `TruncatedPCA`，采用 sklearn 的 **样本 × 特征**约定（与上游 `control_pcs` 的 6000×18000 形状一致）。

- 拟合入口：`Fit(data, k, Optional center:=True, Optional scale:=False, Optional oversampling:=8, Optional powerIterations:=2, Optional seed:=0, Optional tolerance:=Nothing)`。
- 中心化（可选标准化）不物化中间矩阵：以算子形式传给 `RandomizedSVD` 的 `apply/applyTranspose`（`A(v) = X·v - mean·(1ᵀv)`，转置同理），避免 6000×18000 的额外稠密拷贝。
- 输出：`Components`（k×features，即特征空间载荷 Vᵀ）、`SingularValues`、`ExplainedVariance`（=σ²，**不除以 n−1**）、`ExplainedVarianceRatio`（σ²/Σσ²）、`Scores`（=U·Σ，samples×k）、`Mean`、`Scale`、`Rank`、`RequestedRank`。
- 方法：`Project(data)`（samples×k 得分）、`InverseTransform(scores)`（近似还原）、`Transform` 别名按现有命名习惯决定；提供 `Shared Fit(...)` 与直接构造两种入口。
- 文档中必须写清与上游的对应关系：`Components` 对应 `Vt[:k]`，`ExplainedVariance` 对应 `sv**2`，避免使用者误除以自由度。

### 4. 多响应岭回归（新增 `Ridge\MultiResponseRidge.vb`）

命名空间 `Microsoft.VisualBasic.Data.Bootstrapping.Ridge`（与 `RidgeFitting` 同目录、同风格）。

- 结果类 `MultiResponseRidgeFit`：`Coefficients`（f×k）、`FeatureCenter`（f）、`Lambda`、`SampleCount`、`Rank`、`RSS`（每个响应）、`RSquared`（每个响应）、`Supported`。
- 单次拟合：`Fit(x As Double()(), y As Double()(), Optional mask As Boolean() = Nothing, Optional lambda As Double = 10.0, Optional center As Boolean = True) As MultiResponseRidgeFit`。
- 掩码语义：`mask(i)=False` 的样本行既不计入中心化均值，也不进入法方程。
- 中心化：在参与行上按列求均值，`Xc = X[mask] − center`；惩罚 `λ·I` 加在全部 f 列上（不单独豁免常数项列），与上游一致。
- 求解：`XcᵀXc + λI` 用 `NumericMatrix.chol()`；`SPD=False` 时回退 `NumericMatrix.Solve`（方阵 LU）。λ 必须为正，否则抛 `ArgumentException`。
- `Rank` 由 Cholesky 成功分解的自由度或 `Rank` 语义给出，用于诊断 H（帽子矩阵）有效自由度。
- 批量入口（对应上游逐基因拟合）：`BatchFit(features As Double()()(), responses As Double()()(), support As Boolean()(,), lambda As Double, Optional minimumRows As Integer = 32, Optional degreeOfParallelism As Integer = 0) As MultiResponseRidgeFit()`。
- 每个基因独立：按 `support(:, g)` 取行、求中心、解 `(f×f)` 方程；支持行数 `< minimumRows` 或对方程非正定且回退失败时，该基因 `Supported=False`、系数全 0（与上游 `coefficients` 初值为 0 一致）。
- 并行：`System.Threading.Tasks.Parallel.For`（必须全限定，`Microsoft.VisualBasic.Parallel` 会遮蔽），每个基因只写自己的槽位，无锁、无归约，结果与并行度无关；基因间工作量 < 2e6 元素时串行，参照 `HVG.RunGenes` 的阈值策略。
- 预校验：三维数组维度一致、`support` 形状为 (records, genes)、`responses` 形状为 (records, genes, k)。

### 5. 审计与报告（DebugAudit）

Math 项目：

- 新增 `test\RandomizedSvdAudit.vb`（`Module RandomizedSvdAudit`，`Public Function RunAll() As Integer`）与 `test\TruncatedPcaAudit.vb`。
- 在 `test\milp\ProgramMilp.vb` 的 `Select Case` 中追加 `"rsvd"` 与 `"pca"` 分支（不改 `StartupObject`）。
- 报告路径 `Path.Combine(AppContext.BaseDirectory, "randomized-svd-test-report.json")` / `"truncated-pca-test-report.json"`，经 `DebugAudit.SaveReport` 落盘。

DataFittings 项目：

- 新增 `_smoketest\MultiResponseRidgeAudit.vb`（`Public Function Run() As Integer`），在 `_smoketest\Program.vb` 中以 `failures += MultiResponseRidgeAudit.Run()` 接入（与 `MGaussianAudit.Run()` 同位置风格）。
- 报告 `"multi-response-ridge-test-report.json"`。

断言一律使用 `DebugAudit.Test / AreEqual / Require / CheckThrows / Mark / Watch`；**禁止**在断言中使用带 `<Conditional("DEBUG")>` 的 `Check / Snapshot / Capture`（Release 下会被消除，导致"假通过"）。每模块采用 `Reset()` → `MaxEvents` → `Mark("... started")` → `Try/Catch`（catch 内 `Mark("...crash", ex.ToString())` + `Test(False, ...)`）→ `SaveReport` → `Return FailureCount` 的既有骨架。

断言清单（关键）：

- 低秩构造矩阵（秩 5）上，`k=5`、`powerIterations=2` 时奇异值相对误差 < 1e-8，子空间误差（`‖Uᵀ U_true⊥‖₂`）< 1e-6。
- 同一 seed 两次运行，`U`、`SingularValues` **逐位相等**；不同 seed 时奇异值仍在容差内一致（算法稳定性）。
- `‖UᵀU − I‖_F < 1e-10`、`‖VᵀV − I‖_F < 1e-10`；`ReducedMatrix ≈ A·V`。
- 稠密 / `SparseMatrix` / `CsrMatrix` 三种入口在同一 seed、同一 `k` 下结果差异 < 1e-10。
- 容差秩：构造奇异值为 `1, 1e-3, 1e-12` 的谱，实际秩应为 2。
- 退化输入：`k=0` 返回空、`k>min(m,n)` 与 `oversampling<0` 抛异常（`CheckThrows`）。
- PCA：`Σσ²/Σσ²_all` 与 `ExplainedVarianceRatio` 自洽；`Mean` 等于列均值；`Scores = (X−mean)·Componentsᵀ`；`Project` 与 `Scores` 一致；`InverseTransform(Project(X))` 在 `k=rank` 时相对误差 < 1e-10。
- 岭回归：无掩码且 λ→1e-8 时系数与 `Multivariate.MLRFit`/OLS 差 < 1e-8；带掩码结果与"先切片再拟合"差 < 1e-12；λ 增大时 `‖B‖_F` 单调不增；共线特征 + λ>0 时系数有限且 `Rank < f`；`minimumRows` 未达标基因 `Supported=False` 且系数为 0；`BatchFit` 与逐基因循环逐位一致；并行度 1 与 0（全部核心）结果逐位一致；`CheckThrows` 覆盖掩码长度不符、λ≤0、维度不一致。

### 6. 文档与元数据同步

- 新增/更新 `NamespaceDoc.vb`（`Ridge\NamespaceDoc.vb` 增补多响应岭回归条目；Math 根 `NamespaceDoc.vb` 增补分解类条目）。
- 更新两个项目 `README.md` 的命名空间地图与能力清单。
- 更新两个 vbproj 的 `<PackageReleaseNotes>`（本项目已有"每轮追加一句"的惯例）；按需补充 `<Description>`/`<PackageTags>` 中的 `randomized-svd`、`pca`、`multi-response-ridge` 关键词。

## 架构设计

数据流（单次拟合）：

```mermaid
graph LR
  A[输入矩阵/数组] --> B[算子绑定 apply/applyTranspose]
  B --> C[局部种子随机源生成 Omega]
  C --> D[范围查找 Y=A*Omega + economy QR]
  D --> E[幂迭代 q 次: QR(A^T Q) 再 QR(A Q)]
  E --> F[小矩阵 B=Q^T A 的 SVD]
  F --> G[容差截断有效秩]
  G --> H[U/S/V/ReducedMatrix 输出]
  H --> I[TruncatedSVD 兼容封装]
  H --> J[TruncatedPCA 载荷与解释方差]
```

职责边界：

- `RandomizedSVD` 只做"任意线性算子的低秩 SVD"，不认识中心化、基因、细胞等业务概念。
- `TruncatedPCA` 负责"中心化/标准化的算子包装 + 解释方差口径"，不重复实现迭代逻辑。
- `MultiResponseRidge` 负责"法方程组装 + 掩码 + 逐基因批量调度"，数值求解复用 `NumericMatrix.chol()`。

## 目录结构

```
Math/                                              # Math.NET5.vbproj
├── Algebra/Matrix.NET/Decomposition/
│   ├── RandomizedSVD.vb        # [NEW] 随机化 SVD 引擎：算子绑定、种子 Omega、幂迭代双 QR 重正交、SmallSVD/GramEigen 双路径、容差秩
│   ├── TruncatedPCA.vb         # [NEW] 截断 PCA：中心化算子包装、基因空间载荷、解释方差(σ²)、得分/投影/逆变换
│   └── TruncatedSVD.vb         # [MODIFY] 改为委托引擎，保留 U/S/V/Components/ReducedMatrix/Reduce，尾部追加 seed/solver
├── NamespaceDoc.vb             # [MODIFY] 增补 Decomposition 下新增类型的命名空间说明
├── README.md                   # [MODIFY] 能力清单与复杂度说明
├── Math.NET5.vbproj            # [MODIFY] PackageReleaseNotes/Description/Tags
└── test/
    ├── RandomizedSvdAudit.vb   # [NEW] DebugAudit 审计：精度/正交性/seed 复现/三入口一致/容差秩/异常
    ├── TruncatedPcaAudit.vb    # [NEW] DebugAudit 审计：方差分解/均值/得分/投影往返/k 边界
    └── milp/ProgramMilp.vb     # [MODIFY] 追加 "rsvd"、"pca" 子命令分支

DataFittings/                                      # linear-netcore5.vbproj
├── Ridge/
│   ├── MultiResponseRidge.vb   # [NEW] 多响应岭回归：Fit/Predict/BatchFit、行掩码、特征中心、Cholesky+回退、逐基因并行
│   └── NamespaceDoc.vb         # [MODIFY] 增补多响应岭回归条目
├── README.md                   # [MODIFY] 能力清单
├── linear-netcore5.vbproj      # [MODIFY] PackageReleaseNotes
└── _smoketest/
    ├── MultiResponseRidgeAudit.vb  # [NEW] DebugAudit 审计：OLS 退化/掩码等价/收缩单调/秩亏稳健/批量一致/异常
    └── Program.vb                  # [MODIFY] 追加 failures += MultiResponseRidgeAudit.Run()
```

## 关键代码结构

```
' Math: Microsoft.VisualBasic.Math.LinearAlgebra.Matrix
Public Class RandomizedSVD
    Public Sub New(A As Double()(), k As Integer,
                   Optional oversampling As Integer = 8,
                   Optional powerIterations As Integer = 2,
                   Optional seed As Integer = 0,
                   Optional solver As RandomizedSvdSolver = RandomizedSvdSolver.SmallSVD)
    Public Sub New(A As SparseMatrix, k As Integer, Optional oversampling As Integer = 8,
                   Optional powerIterations As Integer = 2, Optional seed As Integer = 0,
                   Optional solver As RandomizedSvdSolver = RandomizedSvdSolver.SmallSVD)
    Public Sub New(A As CsrMatrix, k As Integer, Optional oversampling As Integer = 8,
                   Optional powerIterations As Integer = 2, Optional seed As Integer = 0,
                   Optional solver As RandomizedSvdSolver = RandomizedSvdSolver.SmallSVD)

    Public ReadOnly Property U As Double()()            ' m x k
    Public ReadOnly Property SingularValues As Double() ' k, 降序
    Public ReadOnly Property V As Double()()            ' n x k
    Public ReadOnly Property ReducedMatrix As Double()()' m x k = U * Sigma
    Public ReadOnly Property Rank As Integer            ' 容差截断后的有效秩
    Public ReadOnly Property RequestedRank As Integer
    Public ReadOnly Property SpectrumTolerance As Double
End Class

' DataFittings: Microsoft.VisualBasic.Data.Bootstrapping.Ridge
Public Module MultiResponseRidge
    Public Function Fit(x As Double()(), y As Double()(),
                        Optional mask As Boolean() = Nothing,
                        Optional lambda As Double = 10.0,
                        Optional center As Boolean = True) As MultiResponseRidgeFit
    Public Function BatchFit(features As Double()()(), responses As Double()()(),
                             support As Boolean()(,), lambda As Double,
                             Optional minimumRows As Integer = 32,
                             Optional degreeOfParallelism As Integer = 0) As MultiResponseRidgeFit()
End Module
```

## 实现注意事项

- **性能**：`QRDecomposition.Q` 每次重建 Q 的代价为 O(m·n²)；对 m=18000、ℓ=178 的残差基，5 次 QR 约 1e9 级浮点运算，属可接受量级；实现时禁止在循环内重复构造 `NumericMatrix` 拷贝，Ω 与 Y 复用缓冲。
- **内存**：不允许把输入稠密化后再分解；`Double()()()` 批量入口的总内存为 records×genes×f×8 字节，文档中标注 18k 基因 × 512 记录 × 6 特征约 440 MB，调用方需自行分块。
- **确定性**：随机源必须局部化（`New Random(seed)` + 本地 Box–Muller）；`Random(seed)` 的位流在同一运行时版本内稳定，跨 .NET 大版本不保证，文档中如实说明；不使用 `RandomExtensions.NextGaussian()`（全局共享状态）。
- **并行安全**：`Parallel.For` 全限定；每个基因只写自己的槽位；禁止对共享累加器做归约。
- **向后兼容**：`TruncatedSVD` 的公开成员与共享入口签名只增不改；默认参数保持旧值，避免既有测试与调用方行为漂移；`TruncatedSVDTest` 必须回归通过。
- **错误处理**：所有入口做维度与取值预校验并抛 `ArgumentException`/`ArgumentNullException`（带参数名）；数值退化（`SPD=False`、`sv₀=0`）走明确回退而不是静默返回错值。
- **日志**：库代码不打日志；诊断信息通过结果对象的属性暴露，测试侧由 DebugAudit 记录。
- **影响面控制**：不修改 `NumericMatrix`/`SparseMatrix`/`CsrMatrix`/各 Decomposition 的既有实现；不改 vbproj 的 ItemGroup；不动 `StartupObject`；P2 的 `NumericTable` 扩展方法本次可不做。

## Agent Extensions

### Skill

- **lsp-code-analysis**
- Purpose: 在重构 `TruncatedSVD` 为引擎薄封装之前，用语义引用查询确认全部调用点（`TruncatedSVDTest`、`ProgramMilp` 的 `"svd"` 分支及解决方案内其他引用），避免破坏公开 API。
- Expected outcome: 得到一份完整的调用点清单与受影响成员列表，重构后逐个核对，保证既有测试与调用方零回归。