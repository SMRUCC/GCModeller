# 生成式模型：高斯混合、变分自编码器与图/图像/单细胞生成

## 引言

**自编码器**压缩再重建，但它的潜在空间是「散乱」的——随机采一个点往往解码不出合理结果。
**变分自编码器（VAE）**通过让编码器输出**分布**（均值与方差）而非单点，并对潜在空间施加先验约束，
使潜在空间连续、可采样。

本包提供五个模型，外加一层共用的数值与工程基础设施。

## 模型一览

| 类型 | 结构 | 适用场景 |
|---|---|---|
| **`GaussianMixtureModel`** | EM 训练的混合高斯 | 软聚类、密度估计；无神经网络，数值最稳 |
| **`GMVAE`** | 高斯编码器 + **潜空间高斯混合先验** | 多峰数据的联合聚类 + 生成 |
| **`GraphVAE`** | GCN 编码器 + 内积解码器 | 图结构生成（可变节点数、稀疏邻接） |
| **`ConditionalVAE`** | 条件编码器 + 计数似然 | 虚拟细胞 / 虚拟扰动（含反事实预测） |
| **`ConvVAE`** | 卷积编解码 + 伯努利似然 | 图像生成 |

> 注意类名：高斯混合模型是 `GaussianMixtureModel`，高斯混合 VAE 是 `GMVAE`。
> `GraphVAE` 的构造函数需要一个 `GraphVAEConfig`。

## 快速上手

### 1. 高斯混合模型

```vbnet
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports Microsoft.VisualBasic.MachineLearning.VariationalAutoencoder

' 旧签名完全保留
Dim gmm As New GaussianMixtureModel(nComponents:=3, maxIter:=200,
                                    tol:=0.000001, regCovar:=0.000001,
                                    diagCovariance:=False,
                                    initMethod:=GaussianMixtureModel.InitMethod.KMeansPP,
                                    seed:=42)
gmm.Fit(X)

Dim labels = gmm.Predict(X)         ' 硬聚类
Dim resp = gmm.PredictProba(X)      ' 责任度 [N, K]
Dim samples = gmm.Sample(100, seed:=7)

' 新增：协方差结构 + 模型选择
Dim tied As New GaussianMixtureModel(3, GmmCovarianceType.Tied, maxIter:=200, seed:=42)
tied.Fit(X)
Console.WriteLine($"BIC={tied.BIC:F2} 参数={tied.NParameters}")
```

`GmmCovarianceType` 提供 `Full / Diagonal / Tied / Spherical` 四种结构。
`BIC` 越小越好；`Core.ModelSelection.GridSearch` 可以按 K 网格自动选成分数。

### 2. 高斯混合 VAE

```vbnet
Dim gmvae As New GMVAE(inputDim:=4, latentDim:=4, nComponents:=3,
                       hiddenDim:=32, seed:=42)
gmvae.Fit(X, epochs:=200, batchSize:=32, learningRate:=0.005,
          beta:=1.0, gamma:=0.1, verbose:=True)

Dim clusters = gmvae.PredictClusters(X)     ' 由潜空间混合先验的责任度给出
Dim yProba, zMean, zLogVar As Tensor
gmvae.Encode(X, yProba, zMean, zLogVar)
Dim generated = gmvae.Generate(20, componentIdx:=1, seed:=3)
```

**`gamma` 的取值很关键**：它是不随数据维度缩放的 KL 权重，取 1.0 时容易在高维 / 强可分数据上
压垮重建项（后验坍缩）。经验区间 0.1 ~ 0.5。

### 3. 图 VAE

```vbnet
Dim config As New GraphVAEConfig With {
    .NumNodes = 6, .InputDim = 6,
    .Hidden1 = 16, .Hidden2 = 8,
    .LatentDim = 4, .DecHidden = 8
}
Dim gvae As New GraphVAE(config, seed:=42)

Dim graphs As New List(Of Tuple(Of Tensor, Tensor))
graphs.Add(Tuple.Create(adjacency, features))

' 旧签名：lr 是必填参数
Dim losses = gvae.Train(graphs, epochs:=500, lr:=0.01F, beta:=1.0F, verbose:=True)

Dim pred = gvae.Predict(adjacency, features)   ' (重构邻接, 重构特征)
Dim eval = gvae.Evaluate(adjacency, features)  ' 确定性，可复现
```

变长图使用掩码：

```vbnet
Dim options As New TrainingOptions With {.Epochs = 200, .LearningRate = 0.01, .KlWarmupEpochs = 20}
gvae.TrainMasked(New List(Of Tuple(Of Tensor, Tensor, Integer)) From {
                     Tuple.Create(paddedAdj, paddedFeatures, validNodeCount)},
                 options)
```

大图可用 `SparseAdjacency`（CSR）承载邻接矩阵：

```vbnet
Dim sparse = SparseAdjacency.FromDense(adjacency)
Dim normalized = sparse.NormalizedWithSelfLoops()   ' D^-1/2 (A+I) D^-1/2
Dim propagated = normalized.Multiply(features)      ' 稀疏 Â @ X
```

### 4. 虚拟细胞 / 虚拟扰动

```vbnet
Imports Microsoft.VisualBasic.MachineLearning.VariationalAutoencoder.Cells

Dim dataset = PerturbationDataset.FromCounts(counts, conditions,
                                             New String() {"control", "drug"})

Dim model As New ConditionalVAE(geneCount:=dataset.GeneCount,
                                conditionCount:=dataset.ConditionCount,
                                latentDim:=16, hiddenDim:=64, seed:=42)
model.Likelihood.Kind = CountLikelihoodKind.NegativeBinomial
model.Likelihood.Dispersion = 1.5
model.Fit(dataset, epochs:=200, batchSize:=64, learningRate:=0.001, beta:=1.0)

' 反事实：同一细胞状态，只替换条件
Dim pair = model.PredictCounterfactual(counts, conditionFrom:=0, conditionTo:=1)
Dim log2fc = model.LogFoldChange(counts, 0, 1)   ' 逐基因的对数倍数变化
```

### 5. 图像生成

```vbnet
Imports Microsoft.VisualBasic.MachineLearning.VariationalAutoencoder.Imaging

' 输入为 channel-last：[N, H, W, C]；H/W 必须是 4 的倍数
Dim vae As New ConvVAE(28, 28, channels:=1, latentDim:=32, filter1:=16, filter2:=32, seed:=42)
vae.Fit(images, epochs:=50, batchSize:=16, learningRate:=0.001, beta:=0.1)

Dim reconstructed = vae.Reconstruct(images)
Dim generated = vae.Generate(16, seed:=7)
Dim metrics = vae.Evaluate(images)   ' (每像素 BCE, 每像素 MSE, KL 均值)
```

## 共用基础设施（`Core`）

| 能力 | 类型 | 说明 |
|---|---|---|
| 统一契约 | `IGenerativeModel`、`IPriorSampler`、`IProbabilisticClusterer`、`IModelScore`、`IModelPersistable` | 既有模型以显式接口实现挂载，公开签名不变 |
| 训练超参 | `TrainingOptions` | epochs / batch / lr / beta / gamma / **KL 退火** / 梯度裁剪 / 早停 |
| 训练观察者 | `ITrainingObserver`、`ConsoleTrainingObserver`、`NullTrainingObserver` | 控制台文案与旧版一致 |
| 模型持久化 | `ModelParameters` | JSON 清单之外的自描述二进制容器；**不使用** `BinaryFormatter` |
| 模型选择 | `ModelSelection` | BIC / AIC / K 网格搜索 |
| 稳定数值 | `Core.Numerics.LogSumExpOps` | log-sum-exp、log-softmax、logVar 夹取 |
| 随机数 | `Core.Numerics.RandomGaussian`、`BoxMuller` | 可复现、成对采样（一次生成两个独立样本） |
| 线性代数 | `Core.Numerics.LinearAlgebraOps` | 唯一数值边界，委托 `VariationalBayes.MatrixBridge` → JAMA |
| 网络层 | `Core.Layers.Dense`、`Core.Layers.Adam` | 前向走 `Tensor.MatMul`（命中 SIMD 内核） |

## 实现要点

- **重参数化为什么关键**：采样本身随机、不可微；写成 `z = μ + σ·ε` 之后梯度可以穿过 μ 与 σ。
  相应地，`∂L/∂logσ² = ∂L/∂z · 0.5 · ε`——**必须用 ε**，用 σ 会退化成恒正偏置。
- **KL 项的双重作用**：既约束后验接近先验（保证可采样），也起正则化作用。
- **为什么 `GMVAE` 把类别变量放到潜空间**：若用一个独立的分类器 `q(y|x)`，它的唯一梯度来自
  `KL(q(y)||p(y))`，而 `p(y)` 又会被学成 `mean(q(y|x))`——KL 无需判别性即可消掉，聚类必然退化。
  若改用「每成分独立解码器」，一旦某个成分略优，责任度就趋于 one-hot，而 one-hot 处 softmax
  的 Jacobian 恒为零，梯度永久消失。把类别变量交给潜空间的高斯混合，责任度由几何关系解析给出，
  两条死路都避开了。
- **图 VAE 的规模边界**：内积解码器 `sigmoid(E·Eᵀ)` 必然输出稠密 N×N，这是该模型族固有的
  O(N²) 成本，用 `GraphVAEConfig.NumNodes` 约束节点数上限；编码器侧的 `Â @ X` 可以用
  `SparseAdjacency` 降到 O(E)。
- **卷积 VAE 的转置卷积**：最近邻上采样 ×2 等价于「2×2 单位核 + 步长 2」的转置卷积，而转置卷积
  正是 `Conv2D` 的伴随算子，直接复用仓库已有的 `Conv2DBackwardInput`，无需新内核。
- **计数数据**：单细胞表达量是过度离散的非负整数，高斯 + MSE 会把稀有基因平滑掉。
  `CountLikelihood` 提供 Gaussian / Poisson / NegativeBinomial / ZeroInflatedNegativeBinomial。

## 质量审计

审计使用 `Microsoft.VisualBasic.Core` 的 `DebugAudit`，覆盖数值闭式校验、有限差分梯度校验、
持久化往返、确定性、边界条件与性能基线。

```powershell
cd test
dotnet run -c Debug -- --audit
```

运行结束后在 `bin/Debug/net10.0/` 生成：

- `vae-audit-report.json` —— 结构化审计报告
- `vae-audit-dump.txt` —— 事件明细与变量快照

进程退出码为失败项数量（0 表示全部通过），可直接用于 CI。

`sciBASIC#\Data_science\MachineLearning\VAE\test\Audit\`

| 分组 | 覆盖内容 |
|---|---|
| `numerics` | log-sum-exp 极值、Box-Muller 矩与独立性、Cholesky 还原协方差、采样保留相关性、抖动回退、持久化往返、Adam 收敛、BIC 公式 |
| `gmm` | 四种协方差结构、BIC 选 K、持久化、全协方差采样相关性、空簇、输入校验 |
| `gmvae` | 纯自编码器重构能力、同种子可复现、持久化、条件生成、伯努利有界性 |
| `graphvae` | 持久化、变长图掩码、稀疏/稠密等价、输入校验、KL 退火与早停 |
| `cells` | NB/ZINB 闭式似然、似然梯度与差分一致、文库大小归一化、反事实方向性、持久化 |
| `convvae` | 上采样与转置卷积算子等价、重构误差下降、输出有界、持久化 |
| `compatibility` | **只用重构前就存在的签名**逐个调用三个模型，验证公开 API 零破坏 |
| `regression` | 已确认缺陷的回归用例（GMM 权重初始化、LogDeterminant 符号、GMVAE 类别信号、GraphVAE 评估确定性） |

## 包信息

- Assembly：`Microsoft.VisualBasic.MachineLearning.VAE`
- TargetFramework：`net10.0`
- Tags：`scibasic;variational-autoencoder;graph-vae;gaussian-mixture;gmm;generative-model;latent-space;clustering`
- 许可：GPL-3.0-or-later
