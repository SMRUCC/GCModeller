---
name: aracne-network-inference
overview: 在 Network.Regulons 项目的 ARACNE 文件夹内实现完整的 ARACNE 基因调控网络推断算法（互信息估计 + 置换显著性阈值 + DPI 去间接边 + TF 定向），并在 test 项目中以 DebugAudit 编写合成数据自检用例与真实矩阵 demo。
todos:
  - id: mi-estimators
    content: 实现 MutualInformation.vb：高斯 copula（SIMD 加速）、自适应分箱、等频直方图、核密度四种 MI 估计器及秩变换/熵/置换工具
    status: completed
  - id: mi-matrix-dpi
    content: 实现 MiMatrix.vb 稀疏互信息矩阵（top-K 剪枝、导出 Matrix）与 DpiFilter.vb 共同邻居 DPI 过滤（含 TF 导向容差）
    status: completed
    dependencies:
      - mi-estimators
  - id: permutation-threshold
    content: 实现 PermutationThreshold.vb：逐基因打乱样本生成零分布并估计分位数互信息显著性阈值 tau
    status: completed
    dependencies:
      - mi-estimators
  - id: aracne-network
    content: 实现 AracneOptions.vb、AracneEdge.vb、AracneNetwork.vb、AracneExtensions.vb：预处理、TF 计算空间并行 MI、阈值、DPI、定向与 CSV/Matrix/Connection 导出
    status: completed
    dependencies:
      - mi-matrix-dpi
      - permutation-threshold
  - id: test-loader
    content: 在 test 项目新增 AracneMatrixCsvLoader.vb：4.6GB 表达矩阵流式列抽样读取与 Homo_sapiens_TF.txt 转录因子列表解析
    status: completed
  - id: test-demo
    content: 编写 AracneDemo.vb 六个合成数据用例（DebugAudit 断言/计时/JSON 报告），并最小改动 Program.vb 增加 --aracne 分支
    status: completed
    dependencies:
      - aracne-network
      - test-loader
  - id: real-matrix-run
    content: 运行真实 18533 基因 × 30409 细胞矩阵的 ARACNE demo，校验分钟级耗时、导出边表与测试报告
    status: completed
    dependencies:
      - test-demo
---

## 用户需求

在 `Network.Regulons` 项目（`G:\GCModeller\src\GCModeller\models\Networks\Network.Regulons\Network.Regulons.vbproj`）的 `ARACNE` 文件夹中，依据该文件夹内的论文（1471-2105-7-S1-S7.pdf，Margolin et al. 的 ARACNE 原始方法）与 `readme.md`（算法原理、伪代码、TF 先验增强流程）实现一套完整的 **ARACNE 基因表达调控网络推断算法**，并在 `test\test.vbproj` 中编写 demo 测试程序。

## 功能内容

1. **表达矩阵预处理**：可选 `log2(x+1)` 变换、零表达/零方差基因过滤、标准化或秩转换，直接消费既有 `Matrix`（行=基因、列=样本）对象。
2. **互信息估计**：按 `MI(X,Y)=ΣΣ p(x,y)·log2[p(x,y)/(p(x)p(y))]` 计算基因对互信息，支持多种估计器：

- 高斯 copula（`MI = -0.5·log2(1-r²)`，快速路径，推荐用于大样本）；
- 等质量自适应分箱（ARACNE 原论文的分区估计器）；
- 分位数等频分箱（含 Miller–Madow 偏差校正）；
- 高斯核密度估计（小样本精确路径）。

3. **互信息矩阵**：稀疏存储（避免 N×N 稠密内存爆炸），支持阈值剪枝与每个调控因子 top-K 剪枝。
4. **显著性阈值 tau**：支持固定阈值与置换零分布自动阈值（打乱样本后重算 MI，取 95%/99% 分位数）。
5. **DPI 去间接边**：三元组 (i,j,k) 中互信息最弱的边若显著低于另外两条边（相对容差 ε，默认 0.1）则剔除；提供 TF 导向模式：两端均非 TF 的边使用 ε=0 坚决剔除，涉及 TF 的边按容差决定。
6. **计算空间缩减（TF 先验）**：提供 TF 列表时只计算 `TF×All` 与 `TF-TF`，将 O(N²) 降为 O(T·N)，并行计算。
7. **方向赋予**：`TF→非TF` 定向、`TF↔TF` 双向、非 TF-非 TF 保留为无向候选边。
8. **结果输出**：调控边表（`regulator,target,mi,correlation,interaction,is_tf_source,is_tf_target`）CSV 导出、MI 系数矩阵（复用 `Matrix` 类型）、转换为工程既有 `Connection` 网络边对象、regulon 规模统计。

## 视觉/交互效果

无 UI 需求。输出为控制台运行日志 + CSV 边表 + MI 矩阵；测试程序使用项目既定的 `DebugAudit` 模块输出带耗时与断言结果的结构化审计日志，并生成 JSON 测试报告。

## 测试与验证

- 合成数据自检用例：MI 估计器正确性与对称性、链式调控伪边被 DPI 剔除、置换阈值落在零分布与真实边之间、方向赋予与导出格式、DPI 容差行为、非法输入校验。
- 真实数据 demo：以 `K:\hsa_grn\20261025\GRN_expression_matrix_metacell30k_18533genes_x_30409cells.csv`（4.94 GB、18533 基因 × 30409 细胞）为输入，采用流式读取 + 细胞列抽样 + 高变基因筛选 + `Homo_sapiens_TF.txt` 转录因子列表，在分钟级完成网络推断并导出结果。

## 技术选型

- 语言/框架：VB.NET（`net10.0`），沿用 `Network.Regulons.vbproj` 现有依赖，不新增项目引用。
- 表达矩阵：`SMRUCC.genomics.Analysis.HTS.DataFrame.Matrix` / `DataFrameRow`（HTS_matrix 项目）。
- 数值计算：`Microsoft.VisualBasic.Math.SIMD`（`SimdReduce.Dot/Sum/Mean/L2Norm`、`SimdSelect.TopKIndices`、`SimdMatrix` 分块运算）。
- 稀疏矩阵互操作：`Microsoft.VisualBasic.Math.Matrix.NamedSparseMatrix`（dataframeUtils 项目）。
- 图对象：`Microsoft.VisualBasic.Data.GraphTheory.Network.IInteraction` / `INetworkEdge`（与 `Connection.vb`、`RegulationEdge.vb` 同一写法）。
- 测试：console 程序 + `Microsoft.VisualBasic.ApplicationServices.Debugging.DebugAudit`（用户既定规则）。

## 实现策略

整体采用「预处理 → MI 计算 → 阈值 → DPI → 定向 → 导出」的线性流水线，各阶段职责由独立类型承担，网络主体仅做编排：

1. **MI 估计**统一输出比特（log2）单位，4 种估计器实现同一个委托签名，便于置换阈值、DPI 与测试复用；高斯 copula 路径把每个基因的表达式一次性中心化 + L2 归一化，任意基因对的 MI 退化为一次 `SimdReduce.Dot`（O(n)），是 30409 细胞规模下的主路径。
2. **自适应分箱**实现论文 Algorithm 1 的等质量递归二分：始终切分“点数最多”的 bin、沿极差更大的轴、在中位数处分裂，直到 bin 数达到上限或 bin 内点数低于下限，MI 用联合/边缘计数求和。分箱带来的偏差在置换零分布中被同样的偏差抵消，因此阈值仍然可比。
3. **稀疏 MI 存储**：TF 导向模式下 `T×N` 直接使用一维 `Double()` 缓冲（1600×2000×8 ≈ 25 MB，可按行并行填充）；全量模式使用每行 `Dictionary(Of Integer, Double)` 稀疏结构。与 `AracneDpi`（`Bio.InteractionModel` 中的稠密 O(n³) 朴素实现）保持语义一致但**不复用其代码、不新增项目引用**。
4. **DPI** 只枚举“共同邻居”三元组：对边 (i,j) 取 `Neighbors(i) ∩ Neighbors(j)`，按 MI 升序遍历弱边优先删除，多轮迭代至稳定（设最大轮数）。复杂度由 O(N³) 降为 O(Σ|Γ(i)∩Γ(j)|)，配合 top-K 剪枝后完全可控。
5. **阈值**用置换零分布：每个基因独立打乱样本顺序（保持边缘分布），抽取固定数量的基因对重算 MI，取分位数；同一 `MiEstimator` 保证偏置一致。
6. **并行**：`Parallel.For` + 行级独立写入（无锁），`MaxDegreeOfParallelism` 可配；进度与统计通过 `silent` 开关控制，避免刷屏日志。
7. **兼容性**：新增代码位于独立命名空间 `SMRUCC.genomics.Model.Network.Regulons.ARACNE`，不触碰 `LinearRegression`/`ElasticNet`/`TRN.vb`/`Connection.vb` 语义；`ToConnections()` 复用既有 `Connection` 类型接入现有网络导出流程。

性能预期：30409 细胞抽样至 800 列、18533 基因筛至约 2000 个节点 + 约 1600 个 TF 时，`3.2×10⁶` 个基因对的 copula MI（SIMD + 多核）在秒级到十秒级完成，DPI 与导出在秒级完成；CSV 流式读取为主要耗时（分钟级）。瓶颈与对策：全量 N×N 场景通过 top-K 剪枝 + 稀疏结构控制内存；核密度估计对样本数设上限并自动退回 copula 并给出 warning。

## 架构设计

```mermaid
flowchart LR
    A[Matrix 表达矩阵] --> B[AracneNetwork 预处理]
    B --> C[Pair 枚举: TF x All 或 N x N 上三角]
    C --> D[MiMatrix 稀疏互信息]
    D --> E[PermutationThreshold 零分布 tau]
    E --> F[DpiFilter 三元组去间接边]
    F --> G[方向赋予 TF 先验]
    G --> H[AracneEdge: CSV / Matrix / Connection]
```

模块划分：

- **参数层**：`AracneOptions`（枚举 + 全部可调参数 + `ToString()` 摘要）。
- **数值层**：`MutualInformation`（估计器 + 秩变换/熵/置换工具）、`PermutationThreshold`。
- **数据结构层**：`MiMatrix`（稀疏容器、剪枝、导出）、`AracneEdge`。
- **算法层**：`DpiFilter`、`AracneNetwork`（编排）。
- **门面层**：`AracneExtensions`（`InferAracneNetwork` 扩展方法 + 导出/统计扩展）。

## 关键实现约定

- MI 一律使用 **log2**（比特），与 readme 公式一致；copula 使用 `-0.5*log2(1-r²)`。
- `SimdReduce.Dot` 会校验长度，子向量运算前先在局部缓冲中拷贝，避免长度不一致异常。
- 所有新增 Public 成员必须有 XML 文档注释（项目开启 `GenerateDocumentationFile`），注释为简体中文并引用 readme 的算法步骤与公式。
- `AracneNetwork` 构造即校验：重复 geneID、基因数 < 2、样本数 < 20、TF 列表为空或全部不在矩阵中均抛异常并给出可读消息。
- 具体参数取值：`miEstimator=GaussianCopula`；`thresholdMode=Permutation`，`permutationQuantile=0.95`；`tolerance=0.1`；`dpiMode` 在提供 TF 列表时默认 `TfGuided`；`directionMode=TfDirected`；`randomSeed` 固定保证可复现。

## 目录结构

```
Network.Regulons/
├── ARACNE/
│   ├── 1471-2105-7-S1-S7.pdf        # 既有，算法依据，不改动
│   ├── readme.md                    # 既有，方法说明，不改动
│   ├── AracneOptions.vb             # [NEW] 参数层。定义 MiEstimator / MiThresholdMode / DpiMode / DirectionMode 四个枚举与 AracneOptions 参数类（估计器、bin 参数、核带宽、阈值模式与分位数、置换数、DPI 容差与模式、TF 列表、预处理开关、top-K 剪枝、并行度、silent、随机种子），附 ToString() 摘要与 DefaultOptions()。
│   ├── MutualInformation.vb         # [NEW] 数值层。Module MutualInformation：GaussianCopulaMi、RankTransform、AdaptivePartitioning（等质量递归二分）、EqualFrequencyHistogram（Miller-Madow）、KernelDensity（Silverman 带宽 + 样本上限保护）、Entropy、QuantileBins、可复现 Shuffle；统一 MiEstimatorFn 委托签名。
│   ├── PermutationThreshold.vb      # [NEW] 置换零分布阈值估计。抽基因对、逐基因打乱样本、复用同一估计器、返回 tau 与零分布统计（均值/标准差/分位数/样本量）。
│   ├── MiMatrix.vb                  # [NEW] 稀疏互信息容器。names、GetItem/SetItem、Neighbors、Edges 迭代器、Prune(threshold)、PruneTopK(k)、ToDense、ToMatrix（复用 Matrix 类型，行=调控因子）、ToNamedSparseMatrix、Statistics。
│   ├── DpiFilter.vb                 # [NEW] DPI 三元组过滤。共同邻居枚举、弱边优先、mi_ij < (1-ε)*min(mi_ik,mi_jk) 判定、TfGuided 模式对非 TF 边 ε=0、多轮迭代至稳定、返回 DpiResult（移除边、TrianglesExamined/EdgesBefore/EdgesAfter）。
│   ├── AracneEdge.vb                # [NEW] 边对象。实现 IInteraction / INetworkEdge（source=调控因子、target=靶基因、value=MI），附加 mi、correlation、isTfSource/isTfTarget、interaction（directed/undirected/mutual）、UID、ToString。
│   ├── AracneNetwork.vb             # [NEW] 算法主体。输入校验、预处理、计算空间选择、并行 MI 填充、阈值过滤、DPI、定向、AracneStatistics、Edges/Genes/TFs/MiMatrix/Threshold 属性、WriteEdges(csv)、CoefMatrix()、Regulations()、ToString。
│   └── AracneExtensions.vb          # [NEW] 门面层。<Extension> InferAracneNetwork(Matrix, AracneOptions)、ToConnections、TopEdges、RegulonSizes；风格对齐 LinearRegression\GrnExtensions.vb。
└── test/
    ├── test.vbproj                  # [MODIFY] 默认通配编译，如有必要仅补充引用（预期无需修改）
    ├── Program.vb                   # [MODIFY] Main 最小侵入新增分支：含 --aracne 时调用 RunAracneDemo(args)，否则保持既有 case 1~13 逻辑与 --matrix 用法；退出码纳入 DebugAudit.FailureCount。
    ├── AracneDemo.vb                # [NEW] RunAracneDemo：合成数据用例 1~6（MI 估计器正确性、合成网络重建与伪边 DPI 剔除、置换阈值、定向与导出、DPI 容差、输入校验），全部使用 DebugAudit 断言/计时/打点，结束时 SaveReport 生成 JSON 报告。
    ├── AracneMatrixCsvLoader.vb     # [NEW] 真实矩阵流式加载器。单次解析表头、按等距/固定种子随机选列（--cells）、逐行 split 只解析选中列、可选 log2(x+1)、minNonZeroRatio 与方差过滤、保留 TF 基因 ∪ top 方差基因；LoadTfList(path) 解析 Homo_sapiens_TF.txt 的 Symbol 列（兼容纯文本每行一个基因）。
    └── MatrixCsvLoader.vb           # 既有，不改动，仅供 LinearRegression 用例使用
```

## 关键代码结构

```
Namespace ARACNE
    ''' <summary>MI 估计器实现：输入两个等长的表达向量，输出以比特为单位的互信息</summary>
    Public Delegate Function MiEstimatorFn(x As Double(), y As Double()) As Double

    Public Class AracneNetwork
        Public Sub New(expr As Matrix, Optional options As AracneOptions = Nothing)
        Public ReadOnly Property Genes As String()
        Public ReadOnly Property TFs As String()
        Public ReadOnly Property Threshold As Double
        Public ReadOnly Property MiMatrix As MiMatrix
        Public ReadOnly Property Statistics As AracneStatistics
        Public Function Fit() As AracneNetwork
        Public Iterator Function Regulations() As IEnumerable(Of AracneEdge)
        Public Sub WriteEdges(csv As String)
        Public Function CoefMatrix() As Matrix
    End Class
End Namespace
```

## 执行注意事项

- 不新增对 `Bio.InteractionModel` 的引用，不修改 `Network.Regulons.vbproj` 的 `Compile Remove` 规则。
- 新增文件位于 `ARACNE\` 与 `test\`，属于 SDK 通配编译范围，无需逐文件登记。
- `Matrix.ArrayPack()` 可一次性取得 `Double()()` 行主序数据，供 SIMD 与并行填充使用，避免重复装箱。
- 真实矩阵测试默认不执行（受 `--aracne --matrix` 双开关控制），避免普通自检被 4.6 GB 数据拖慢。
- 日志只输出阶段统计（基因数、样本数、候选对数、边数、DPI 移除数、耗时），不打印矩阵内容。