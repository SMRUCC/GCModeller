---
name: Traitar-SVM-LibSVM-重构
overview: 将 MetaFunction/metaTraits/Traitar 中手写的坐标下降 SVM 实现彻底删除，改为基于 sciBASIC# SVM(LibSVM) 算法库重构：新增 Pfam one-hot 嵌入、基于 phenotype_traits_and_types.csv 的表型→SVM 模型实例映射与训练管线（boolean/categorical 用 C_SVC，numeric 用 EPSILON_SVR）、每表型一个 JSON 的模型持久化，并在 test/metaTraitsTest.vb 中补全 trainingTest 与 predicttest。
todos:
  - id: metadata-embedding
    content: 新增 PhenotypeTraitType.vb 表型元数据层与 PfamOneHotEmbedding.vb 的 Pfam one-hot 嵌入层
    status: completed
  - id: problem-builder
    content: 实现 TraitProblemBuilder 将 TraitAnnotation 标签与 one-hot 特征装配为 ProblemTable
    status: completed
    dependencies:
      - metadata-embedding
  - id: svm-trainer
    content: 实现 PhenotypeSVMTrainer 训练引擎（C_SVC 与 EPSILON_SVR 双路径）并删除 Modules\SVMClassifier.vb
    status: completed
    dependencies:
      - problem-builder
  - id: model-store
    content: 重写 PhenotypeModel 与 ModelLoader，实现每表型一个 JSON 加 index.json 与 pfam_vocabulary.json 的存取
    status: completed
    dependencies:
      - svm-trainer
  - id: eval-predict
    content: 重写 CrossValidation/ModelEvaluation/FeatureSelection，并用 PhenotypePredictor 替换 EnsembleVoting.vb
    status: completed
    dependencies:
      - model-store
  - id: report-cleanup
    content: 更新 Export/ReportJSON/PredictionResult 并清理 Utils\FileParser.vb 中的旧 txt 模型解析函数
    status: completed
    dependencies:
      - eval-predict
  - id: test-pipeline
    content: 用 [subagent:code-explorer] 核查残留引用，清理 test/TraitarTest.vb 与 Program.vb，实现 metaTraitsTest 的 trainingTest 与 predicttest
    status: completed
    dependencies:
      - report-cleanup
  - id: build-verify
    content: 编译 MetaFunction 与 test 项目并冒烟跑通训练与预测全流程
    status: completed
    dependencies:
      - test-pipeline
---

## 产品概述

对 `MetaFunction\metaTraits\Traitar` 模块中的表型 SVM 预测模型代码做完全重构：删除现有手写的坐标下降线性 SVM 实现，改造为基于 sciBASIC# 底层 LibSVM 算法库（`Microsoft.VisualBasic.MachineLearning.SVM`）的生物表型多模型训练与预测体系。

## 核心功能

1. **表型元数据驱动的模型实例映射**：解析 `phenotype_traits_and_types.csv`（2652 条表型，含 `boolean` / `categorical` / `numeric (continuous)` 三种 data_type），每一条表型按其数据类型对应一个独立的 SVM 模型实例（boolean、categorical 用 C_SVC 分类，numeric 用 EPSILON_SVR 回归）。
2. **Pfam 归一化丰度嵌入**：读取各微生物基因组预测的蛋白质组 Pfam 结构域组成（`Pfam.csv` → `PfamString`），构建全局 Pfam 词表，并按"结构域命中总次数 / 该基因组内最大命中次数"做 per-genome 归一化，得到 `[0,1]` 区间的丰度向量（而非单纯 0/1 one-hot），以捕捉同一 Pfam 结构域在基因组内的重复/拷贝数信息。

- **计数粒度**：遍历该基因组所有蛋白的 `Pfam-string` 数组的每一个条目累加；同一蛋白内同一 PF 出现在不同位置时重复计数（如 `PF00044` 出现 3 次就计 3）。
- **归一化**：`value(pfam_i) = count(pfam_i) / max(count(*) over all pfam in that genome)`；分母为该基因组内所有 PF 的最大命中次数；`max = 0`（无注释）时全向量置 0 并跳过该样本。
- **编码模式可切换**：`PfamEncoding` 枚举 —— `NormalizedCount`（默认，上述 per-genome 归一化）与 `Binary`（旧 0/1 one-hot 行为），便于后续做对比实验。
- **词表剪枝可配置**：`minGenomes` 参数（PF 至少在 N 个基因组中出现才入词表），**默认 1 = 不剪枝**。

3. **基于 LibSVM 的全量训练**：以 `TraitAnnotation` 数据集（GTDB + NCBI species summary tsv）的 `consensus_value`（boolean/categorical）或 `mean`/`median`（numeric）作为标签，对全部表型逐一训练 SVM 模型；样本不足或单类别的表型容错跳过并记录失败原因，不中断整体流程。
4. **模型持久化**：每个表型一个 JSON（`SvmModelJSON` 序列化）+ 目录级 `index.json` 索引 + `pfam_vocabulary.json` Pfam 词表（词表必须与模型一并落盘，否则预测端无法复现同一嵌入）。
5. **表型预测**：加载模型目录（含词表与编码模式），对新的基因组 Pfam 组成做**完全相同的**嵌入，逐表型调用对应 SVM 模型实例预测，输出分类标签/回归值、置信度与关键 Pfam 特征。
6. **测试闭环**：在 `MetaFunction\test\metaTraitsTest.vb` 中补全 `trainingTest()`（训练并落盘模型）与 `predicttest()`（加载模型目录并对样本基因组做表型预测，输出报告）。

## 技术栈

- 语言/框架：VB.NET，`net10.0`，`RootNamespace = SMRUCC.genomics.Analysis.Metagenome.MetaFunction`
- 机器学习：`Microsoft.VisualBasic.MachineLearning.SVM`（LibSVM 移植版）+ `.StorageProcedure`（`MetaFunction.vbproj` 已引用 `machine_learning-netcore5.vbproj`，无需新增引用）
- 数据 IO：`Microsoft.VisualBasic.Data.Framework`（`LoadCsv(Of T)(mute:=True, tsv:=True)`）、`SMRUCC.genomics.Data.Xfam.Pfam.PfamString.PfamString`
- 序列化：`Microsoft.VisualBasic.Serialization.JSON`（`GetJson` / `LoadJSON`）

## 实现方案

### 总体策略

以 `ProblemTable`（来自 LibSVM `StorageProcedure`）作为"一个基因组一行样本、一个表型一个 topic"的通用数据容器，把重构拆成四层：**元数据层 → 嵌入层 → 训练层 → 预测层**。所有 SVM 训练/预测/评估一律委托给底层算法库（`Training.Train`、`LibSVM.getSvmModel`、`Training.PerformCrossValidation`、`Prediction.Predict`），本模块不再自带任何优化器代码。

### 关键技术决策

1. **分类与回归走两条不同的训练路径**（这是最重要的决策）

- `boolean` / `categorical`：`Problem.Y` 由字符串标签经 `ClassEncoder` 编码，直接调用 `LibSVM.getSvmModel(problem, param)`，一次拿到 `SVMModel`（自带 `RangeTransform` + `factors`）。
- `numeric (continuous)`：**不能**走 `LibSVM.getSvmModel`，因为其内部 `New ClassEncoder(problem.Y)` 会把数值标签重编码为 0/1/2…。改为：`RangeTransform.Compute(problem)` → `transform.Scale(problem)` → `Training.Train(scaled, param)`（param.svmType = EPSILON_SVR），再手动组装 `SVMModel`，其中 `Y` 用 `New ColorClass With {.factor = CDbl(value), .name = value}` 直接承载真实数值，`transform` 必须是 `RangeTransform` 实例。

2. **Pfam 嵌入 = per-genome 归一化丰度向量（默认模式）**

```
' 单基因组原始计数
counts(pfamId) += 1   ' 遍历每个蛋白的 Pfam-string 数组的每个条目
                      ' 形如 "PF00044:Gp_dh_N(3|103)"，取 ':' 前的 PF 号
                      ' 同一蛋白内重复位置重复计数

' per-genome 归一化
maxCount = counts.Values.Max()
value(pfamId) = If(maxCount = 0, 0.0, counts(pfamId) / maxCount)
```

- 词表 = 所有训练基因组中出现的 PF 号并集（按 `minGenomes` 过滤后）排序，保证维度固定。
- `Binary` 模式：`value = If(counts > 0, 1.0, 0.0)`，用于对照实验。
- **嵌入配置必须持久化**：`encoding`（模式）+ `minGenomes` + `dimensionNames`（词表）写入 `pfam_vocabulary.json`，预测端从该文件还原，保证训练/预测两端嵌入严格一致。
- 特征向量取值 `[0,1]`，仍然**必须稠密 + 1-based** 写成 `New Node(i + 1, value)`（`RangeTransform` 会把最小值映射为 -1，稀疏省略 0 会被当作 0 而算错）。

3. **全量训练 + 容错**：外层对 2652 个表型循环，每个表型包在 `Try/Catch` 内；先把"有效标签样本数 < 2"或"分类标签只有 1 个类别"的表型标记为 `skipped` 并写入原因，避免 `svm_check_parameter` / `svm_train` 抛异常中断整体流程。
4. **参数默认值**：`C_SVC` / `EPSILON_SVR` + `KernelType.RBF`，`gamma = 1 / 词表维度`（必须为 0 以外的正数），`c = 1`，`EPS = 0.001`，`P = 0.1`，`cacheSize = 40`，`shrinking = True`；全部可通过 `Parameter` 注入覆盖。
5. **模型落盘用 `SvmModelJSON`**：`SvmModelJSON.CreateJSONModel(svm)` 序列化、`CreateSVMModel()` 还原；文件名按表型名做安全化（非法文件名字符替换）以避免路径问题。

### 性能与复杂度

- 训练规模：2652 表型 × 约 33 样本 × D 维 Pfam 特征。单表型训练为 O(n²·D) 量级且 n 极小，瓶颈在于**稠密 Problem 的构造**（2652 × 33 × D 个 `Node`）。
- 优化措施：`ProblemTable` 只构建一次并复用（所有 topic 共享 `X`）；逐表型 `GetProblem(topic)` 后立即训练并释放引用；`SupportVector.Properties` 一次性按词表填充。
- 词表剪枝：`minGenomes` 默认 1（不剪枝）；样本规模扩大后可调大以控制维度膨胀。

## 架构设计

```mermaid
flowchart TD
    A[phenotype_traits_and_types.csv] -->|PhenotypeTraits.LoadTable| B[PhenotypeTrait 元数据<br/>data_type -> SvmType/Parameter]
    C[pfam 目录<br/>Pfam.csv] -->|PfamString 解析 PF 号| D[PfamEmbedding<br/>命中计数 + per-genome max 归一化]
    E[TraitAnnotation tsv<br/>GTDB + NCBI] -->|taxon_name 规范化匹配| F[TraitProblemBuilder]
    D --> F
    B --> F
    F -->|ProblemTable<br/>行=基因组 topic=表型| G[PhenotypeSVMTrainer]
    B -->|SvmType/Parameter| G
    G -->|Per-Phenotype Try/Catch| H[Training.Train / LibSVM.getSvmModel]
    H --> I[ModelStore 落盘<br/>{trait}.json + index.json + pfam_vocabulary.json]
    I -->|ModelLoader.LoadDirectory| J[PhenotypePredictor]
    K[待预测基因组 Pfam] --> D
    I -->|词表 + encoding 还原| D
    D --> J
    J -->|Prediction.Predict| L[ReportJSON 表型预测报告]
```

### 模块职责

- **元数据层** `PhenotypeTraitType.vb`：表型数据类型 → `SvmType` / `Parameter` / 标签提取规则。
- **嵌入层** `PfamEmbedding.vb`：`PfamEncoding` 枚举（NormalizedCount / Binary）、Pfam 词表构建（`minGenomes` 剪枝）、per-genome 命中计数、`count / max` 归一化、基因组 → `Node()`（1-based 稠密）、物种名规范化匹配、词表与编码配置的 `Save`/`Load`。
- **数据装配层** `Modules\TraitProblemBuilder.vb`：组装 `ProblemTable`（`SupportVector.Properties` = Pfam 归一化丰度向量，`labels` = 表型名 → 标签值）。
- **训练层** `Modules\PhenotypeSVMTrainer.vb`：逐表型训练、交叉验证、模型集落盘。（替代 `SVMClassifier.vb`）
- **模型层** `Models\PhenotypeModel.vb` + `ModelLoader.vb`：单表型模型封装（持有一个 `SVMModel`）与目录级 JSON 存取。
- **评估层** `Modules\CrossValidation.vb` / `ModelEvaluation.vb`：包装 `Training.PerformCrossValidation` 与 `Prediction.Predict`。
- **解释层** `Modules\FeatureSelection.vb`：基于 Pfam 特征与表型的显著性/排列重要性给出关键 Pfam（不再依赖线性权重字典）。
- **预测层** `Modules\PhenotypePredictor.vb`：替代 `EnsembleVoting.vb`（一表型一模型，无需投票委员会）。

## 目录结构

```
MetaFunction/metaTraits/
├── phenotype_traits_and_types.csv                       # [不变] 表型-数据类型定义表
├── TraitAnnotation.vb / TraitData.vb / metaTraitData.vb  # [不变] 训练集解析模型
└── Traitar/
    ├── PhenotypeTraitType.vb                             # [NEW] 表型元数据层：PhenotypeTrait 实体（trait_name/data_type/unit/group_1/group_2）+ PhenotypeTraits 模块（LoadTable 解析 csv、ToSvmType 数据类型映射、CreateParameter 生成 LibSVM Parameter、GetLabel 从 TraitAnnotation 提取标签值）。
    ├── PfamEmbedding.vb                                  # [NEW] Pfam 嵌入层：PfamEncoding 枚举（NormalizedCount 默认 / Binary）；CountDomains(pfamSet) 按命中总次数计数（遍历 PfamString.PfamString 各条目的 "PFxxxxx:Name(a|b)" 前缀，+ 分隔，同蛋白内重复位置重复计数）；BuildVocabulary(pfamSets, minGenomes=1) 构建全局 PF 词表；Embed(counts, encoding) 做 count / per-genome max 归一化（max=0 保护）；ToNodes 生成 1-based 稠密 Node()；NormalizeSpeciesName 做 "Carnobacterium_divergens" ↔ "Carnobacterium divergens" 匹配；Save/Load 词表与编码配置（pfam_vocabulary.json）。
    ├── ModelLoader.vb                                    # [MODIFY] 重写为 JSON 模型仓库：SaveDirectory(models, dir) 逐表型写 {safeTraitName}.json（SvmModelJSON）、写 index.json（表型名/类型/样本数/指标/失败原因/编码模式）、写 pfam_vocabulary.json；LoadDirectory(dir) 反向还原 Dictionary(Of String, PhenotypeModel) 与 PfamEmbedding 配置。
    ├── Models/
    │   └── PhenotypeModel.vb                             # [MODIFY] 重写：删除 SVMSubModel 投票体系，改为单模型封装（Trait As PhenotypeTrait、Model As SVMModel、SampleCount、CVScore、Status、ErrorMessage、KeyFeatures）；提供 Predict(nodes) / PredictLabel / PredictValue。
    ├── Modules/
    │   ├── SVMClassifier.vb                              # [DELETE] 删除手写坐标下降 SVM 全部代码。
    │   ├── TraitProblemBuilder.vb                        # [NEW] 数据装配：Build(annotations, pfamSets, traits, embedding) → ProblemTable（行=基因组 SupportVector，Properties = 归一化 Pfam 丰度，topic=表型名，labels 来自 TraitAnnotation.consensus_value / mean）。
    │   ├── PhenotypeSVMTrainer.vb                        # [NEW] 训练引擎：TrainAll(problemTable, traits, paramFactory) 逐表型训练（C_SVC 走 LibSVM.getSvmModel；EPSILON_SVR 走 RangeTransform+Training.Train 手动组装），逐表型 Try/Catch 记录失败原因，返回 PhenotypeModel 集合。
    │   ├── PhenotypePredictor.vb                         # [NEW] 预测引擎（替代 EnsembleVoting.vb）：PredictGenome(models, pfamVector) → IEnumerable(Of PhenotypePrediction)（表型名/预测值/置信度/关键特征）。
    │   ├── CrossValidation.vb                            # [MODIFY] 改为包装 Training.PerformCrossValidation(problem, parameter, nrfold)。
    │   ├── ModelEvaluation.vb                            # [MODIFY] 改为基于 Prediction.Predict / PredictLabels 计算准确率、混淆矩阵、SVR 相关指标。
    │   ├── FeatureSelection.vb                           # [MODIFY] 重写 KeyFeature 提取：改为基于 Pfam 特征与标签的相关性/排列重要性（不再依赖 SVMClassifier.SVMModel 权重字典）。
    │   └── EnsembleVoting.vb                             # [DELETE] 旧投票委员会机制，由 PhenotypePredictor 取代。
    ├── Export.vb                                         # [MODIFY] 适配新 PhenotypeModel / PhenotypePrediction，输出 ReportJSON 表。
    ├── ReportJSON.vb / PredictionResult.vb               # [MODIFY] 字段适配（分类标签名、回归值、置信度、KeyFeatures 类型）。
    ├── Models/GenomeSample.vb, Models/PhyloTreeNode.vb   # [不变]（GenomeSample.PhyleticProfile 继续保留，作为预测输入的另一种来源）
    ├── Modules/GenomeAnnotation.vb, DataFusion.vb, PhylogenyAncestral.vb  # [不变] 无 SVM 依赖
    └── Utils/
        ├── FileParser.vb                                 # [MODIFY] 删除 ParsePhenotypeTable/ParsePfamDescription/ParseBiasFile/ParseFeatsFile/ParseNonZeroWeightsFile 等旧 Traitar txt 模型解析；保留 GFF/FASTA/hmmsearch/Newick 解析。
        └── MathUtils.vb                                  # [不变]

MetaFunction/test/
├── metaTraitsTest.vb                                     # [MODIFY] 实现 trainingTest()（读 tsv + pfam 目录 → 归一化嵌入 → 全量训练 → 落盘模型目录）与 predicttest()（加载模型目录 → 对样本基因组做表型预测 → 打印/保存报告）。
├── TraitarTest.vb                                        # [DELETE] 旧 Traitar CLI（依赖已删除的 ModelLoader/SVMClassifier/EnsembleVoting）。
└── Program.vb                                            # [MODIFY] 移除 Sub Main 中对 TraitarVB.Program.Main2({}) 的调用，保证 test 项目可编译。
```

## 关键代码结构

```
' metaTraits.Traitar —— 表型元数据 → SVM 模型类型的映射契约
Public Class PhenotypeTrait
    Public Property trait_name As String
    Public Property data_type As String        ' boolean / categorical / numeric (continuous)
    Public Property unit As String
    Public Property group_1_category As String
    Public Property group_2_subcategory As String
    Public ReadOnly Property svmType As SvmType ' boolean|categorical -> C_SVC; numeric -> EPSILON_SVR
End Class

' Pfam 嵌入：命中计数 + per-genome 归一化
Public Enum PfamEncoding
    NormalizedCount   ' 默认：count / max(count in genome)，取值 [0,1]
    Binary            ' 对照：count > 0 -> 1.0，否则 0.0
End Enum

Public Class PfamEmbedding
    Public Property dimensionNames As String()      ' PF 词表（排序，长度 = 特征维度）
    Public Property encoding As PfamEncoding = PfamEncoding.NormalizedCount
    Public Property minGenomes As Integer = 1       ' 剪枝阈值，1 = 不剪枝

    ' 单基因组：PfamString() -> PF 命中总次数字典
    Public Shared Function CountDomains(proteins As IEnumerable(Of PfamString)) As Dictionary(Of String, Integer)
    ' 归一化：count / per-genome max（max = 0 时全 0）
    Public Function Embed(counts As Dictionary(Of String, Integer)) As Dictionary(Of String, Double)
    ' 1-based 稠密特征向量（含 0 值位）
    Public Function ToNodes(profile As Dictionary(Of String, Double)) As Node()
End Class

' 每一条表型对应一个 SVM 模型实例（替代原 SVMSubModel 投票列表）
Public Class PhenotypeModel
    Public Property Trait As PhenotypeTrait
    Public Property Model As SVMModel          ' 来自 Microsoft.VisualBasic.MachineLearning.SVM
    Public Property SampleCount As Integer
    Public Property CVScore As Double
    Public Property Status As String           ' trained / skipped
    Public Property ErrorMessage As String
    Public Property KeyFeatures As KeyFeature()
End Class

' 训练引擎对外契约（替代 SVMClassifier.Train）
Public Function TrainAll(problems As ProblemTable,
                         traits As IEnumerable(Of PhenotypeTrait),
                         Optional paramFactory As Func(Of PhenotypeTrait, Parameter) = Nothing,
                         Optional nrfold As Integer = 5) As Dictionary(Of String, PhenotypeModel)
```

## 实施注意事项（防回归）

- **嵌入一致性**：训练端与预测端必须使用同一份 PF 词表、同一顺序与同一 `PfamEncoding`；词表与编码配置随模型写入 `pfam_vocabulary.json`，`predicttest` 必须从该文件还原 `PfamEmbedding`，不得重新从数据推导词表。
- **分母为 0 保护**：某基因组无任何 Pfam 注释时 `maxCount = 0`，归一化直接返回全 0 向量（不得除零）；该样本在 `TraitProblemBuilder` 中可直接跳过。
- `Node.index` 必须 1-based 且**稠密**（含 0 值位），否则 `RangeTransform` 缩放后预测结果错误（归一化模式下仍有大量 0 值位）。
- numeric 表型禁用 `LibSVM.getSvmModel`，必须手动组装 `SVMModel` 且 `transform` 为 `RangeTransform`，否则 `SvmModelJSON.CreateSVMModel()` 在 `gaussianTransform Is Nothing` 时抛空引用。
- 预测前必须先 `model.transform.Transform(nodes)` 再 `Prediction.Predict(model.model, scaled)`；SVC 用 `factors.GetColor(pred.class).name` 还原标签名，SVR 直接取 `pred.unifyValue`（此时 `pred.class = Integer.MinValue`）。
- `Parameter.gamma` 绝不可为 0；RBF 下取 `1 / 词表维度`。
- 训练循环必须逐表型 `Try/Catch`，失败写入 `index.json`，保证 2652 个表型全量跑完不中断。
- `test.vbproj` 默认编译 `test\*.vb`，删除 `TraitarTest.vb` 后必须同步去掉 `Program.vb` 中对 `TraitarVB.Program.Main2` 的调用，否则 test 项目编译失败。
- 日志：沿用 `Console.WriteLine` 风格（与现有 `ModelLoader`/`SVMClassifier` 一致），仅输出表型名、样本数、CV 分数、失败原因，不输出完整特征向量，避免日志爆炸。

## Agent Extensions

### SubAgent

- **code-explorer**
- Purpose：在删除 `Modules\SVMClassifier.vb`、`Modules\EnsembleVoting.vb` 之前，彻底扫描 `metaTraits.Traitar` 与 `MetaFunction\test` 中所有引用 `SVMClassifier`、`SVMSubModel`、`PhenotypeModel.SubModels`、`EnsembleVoting.VotingResult`、`ModelLoader.LoadPhenotypeSVM` 的调用点，确认无遗漏的编译依赖。
- Expected outcome：输出一份完整的受影响文件与符号清单，保证删除旧体系后 `MetaFunction` 与 `test` 两个项目均可编译。