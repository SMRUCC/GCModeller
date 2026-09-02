---
name: BNLearnWorkflow 模型持久化
overview: 实现 Core\BNLearnWorkflow.vb 中 SaveModel/LoadModel 两个函数，将训练好的 BNLearnWorkflow（网络结构 + CPD 参数 + 先验网络 + 表达矩阵 + 学习参数/结果）序列化为 zip 压缩包并支持反序列化还原，随后编译 Erica.sln（Rsharp_app_release|x64）并运行 K:\hsa_grn\bnlearn.R 流程测试。
todos:
  - id: impl-persistence
    content: 在 Core\BNLearnWorkflow.vb 实现 SaveModel/LoadModel 及 zip 读写辅助函数
    status: completed
  - id: check-lints
    content: 检查 BNLearnWorkflow.vb 的编译诊断并修正语法错误
    status: completed
    dependencies:
      - impl-persistence
  - id: build-erica
    content: 以 Rsharp_app_release|x64 编译 G:\Erica\src\Erica.sln 解决方案
    status: completed
    dependencies:
      - check-lints
  - id: run-pipeline
    content: 在 G:\GCModeller\src\R-sharp\App\net10.0 下运行 K:\hsa_grn\bnlearn.R 流程测试
    status: completed
    dependencies:
      - build-erica
  - id: fix-issues
    content: 根据测试输出修复往返还原问题并重新编译验证
    status: completed
    dependencies:
      - run-pipeline
---

## 产品概述

在 BNLearn 子系统的高层工作流入口 `BNLearnWorkflow` 中补齐模型持久化能力：把训练完成的贝叶斯网络调控模型（结构 + CPD 参数 + 先验网络 + 训练表达矩阵 + 学习参数与学习结果）序列化为 zip 压缩包，并支持从该 zip 原样还原出一个可直接执行虚拟扰动实验的工作流对象。

## 核心功能

- **模型导出（SaveModel）**：将 `BNLearnWorkflow` 的全部状态写入 zip 压缩包，输出流由调用方（R# `writeBin`）提供并负责释放。
- **模型载入（LoadModel）**：从 zip 压缩包反向重建 `BNLearnWorkflow`，还原后无需重新训练即可直接调用 `knockouts` / `overexpress` / `knockdown` 等虚拟扰动接口。
- **版本与完整性校验**：zip 内写入格式版本号，载入时校验版本并在元数据缺失/损坏时抛出可读的 `InvalidDataException`。
- **往返保真**：网络拓扑（节点、有向边、白/黑名单）、每个节点的高斯 CPD 参数、结构学习/参数学习统计量、工作流开关（`NormalizeData`/`NSamples`/`RandomSeed`/`Strict`）与结构学习参数均需无损还原。
- **验证闭环**：以 `Rsharp_app_release|x64` 编译 `Erica.sln`，再运行 `K:\hsa_grn\bnlearn.R` 流程脚本，确认 `writeBin` → `readBin` → 批量扰动 → `make_exports` 全链路通过。

## 技术栈

- 语言/框架：VB.NET，目标框架 `net10.0`（`BNLearn.vbproj`，`OptionStrict Off`、`OptionInfer On`）
- 压缩：`System.IO.Compression.ZipArchive` / `ZipArchiveEntry`（BCL 内置，无需新增依赖）
- 数值序列化：文本用 `G17` + `CultureInfo.InvariantCulture`；大矩阵用 `BinaryWriter`/`BinaryReader` 二进制块
- 上层绑定：R# `writeBin` / `readBin.bnlearn`（`g:\GCModeller\src\workbench\R#\biosystem\bnlearn.vb`，已就绪，不改动）
- 构建：`G:\Erica\src\Erica.sln`，配置 `Rsharp_app_release|x64`

## 实现方案

**策略**：完全对齐同项目已有先例 `ModularNetwork/BlockBayesianNetwork.vb`（第 383-912 行）的 zip 持久化模式——同样的 `WriteText` / `GetEntry` / `ReadLines` / `ReadMeta` 辅助函数约定、同样的 `leaveOpen:=True` 语义、同样的版本校验与 `G17` 数值格式、同样的 `.info` 收尾日志。不引入新架构、不新增文件，仅在 `BNLearnWorkflow` 类内部新增常量与私有辅助方法。

**为何必须持久化表达矩阵**：`KnockoutGene` / `OverexpressGene` / `KnockDownGene` / `DynamicKnockout` 在 `NormalizeData = True`（默认）时第一行就执行 `ExpressionData.Standardize`，`ExpressionData` 为 `Nothing` 会直接 NRE；同时它们要求 `ParameterResult IsNot Nothing`。因此"只存网络结构"是不够的，必须连同表达矩阵与学习结果一起落盘。

**性能取舍**：表达矩阵约 2000 基因 × 300 样本 = 60 万 double。若用文本 G17 落盘会膨胀到约 12MB 且需 60 万次 `Double.Parse`（秒级）；改为二进制 entry（4.8MB，顺序读写）可将 I/O 与解析耗时压到毫秒级，且 Deflate 后体积更小。代价是矩阵不再是可人读文本——但这部分本就无需人工查看，网络结构/CPD/参数等需要排查的部分仍全部保持 TSV 文本。

## 实现要点（执行细节）

**Imports 追加**：`System.Globalization`、`System.IO.Compression`（当前文件只有 `System.IO`、`System.Text` 等）。

**类内新增**：

- `Private Const ModelFormatVersion As Integer = 1`
- `WriteText(zip, name, write As Action(Of TextWriter))`、`GetEntry(zip, name)`、`ReadLines(entry)`、`ReadMeta(entry)`（与 `BlockBayesianNetwork` 同名同签名，逐行对齐）
- `WriteDoubles(zip, name, values As Double())` / `ReadDoubles(entry)`：二进制写 `Int32 count` + N 个 double
- `WriteMatrix(zip, name, m As Double(,))` / `ReadMatrix(entry, nG, nS)`：写 `Int32 nG`、`Int32 nS` + 行优先（gene-major）的 N×M 个 double
- 数值/文本工具：`Num(d) = d.ToString("G17", CultureInfo.InvariantCulture)`、`ParseNum(s)`；分割统一用制表符 `vbTab` + `StringSplitOptions.None`（保留空的 Evidence 字段）；**禁止用 `|` 作分隔符**

**zip 布局**（`SaveModel` 写入顺序即下表）：

| entry | 内容 |
| --- | --- |
| `meta.txt` | `version` / `nodes` / `edges` / `genes` / `samples` / `has_expr` / `has_prior` / `has_struct` / `has_param` / `has_external` |
| `settings.txt` | `NormalizeData` `NSamples` `RandomSeed` `Strict` + `StructureParams` 全字段（`Algorithm` 用枚举名、`Alpha` `MaxParents` `TabuLength` `MaxIterations` `BICPenalty` `UseWhitelist` `UseBlacklist` `RandomSeed`） |
| `nodes.txt` | 每行一个节点名，行号即节点索引（保证 `NameToIndex` 与 `BnNode.Index` 一致） |
| `edges.tsv` | `fromIdx \t toIdx` |
| `whitelist.tsv` / `blacklist.tsv` | `fromIdx \t toIdx` |
| `cpt.tsv` | `nodeIndex \t intercept \t coeffs(逗号) \t parentIndices(逗号) \t residualSD \t residualVariance \t rsquared \t bic \t nsamples`；`CPD Is Nothing` 的节点跳过 |
| `prior_edges.tsv` | `TF \t TargetGene \t regulationType(int) \t confidence \t evidence` |
| `expression/genes.txt` / `expression/samples.txt` | 每行一个名称 |
| `expression/timepoints.bin` | 二进制 `Double()` |
| `expression/matrix.bin` | 二进制 `nG, nS, Double(nG*nS)` |
| `structure.txt` | `FinalBIC` / `Iterations` / `ElapsedMs` / `bic_history`(逗号分隔) |
| `parameter.txt` | `TotalLogLikelihood` / `TotalBIC` / `AverageRSquared` / `ElapsedMs` |
| `external.txt` | `ExternalExpression` 与 `ExternalEvidence`（`gene \t value`）及 `ExternalInitialState`（单行逗号分隔），无则写入空标记 |


**SaveModel 流程**：`file Is Nothing` → `ArgumentNullException`；`FittedNetwork Is Nothing` → 抛出"模型尚未完成结构学习"的中文友好异常；`Using zip As New ZipArchive(file, ZipArchiveMode.Create, leaveOpen:=True)`；边通过 `FittedNetwork.GetEdges()` 导出（0 节点时 `Adjacency` 可能为 `Nothing`，需先判 `Nodes.Count > 0` 再取边）；结尾 `Call $"[BNLearnWorkflow] 模型已导出: nodes=..., edges=..., genes=..." .info`。

**LoadModel 流程**：`Using zip As New ZipArchive(file, ZipArchiveMode.Read, leaveOpen:=True)` → 读 `meta.txt` 校验 `version`（不符抛 `InvalidDataException`）→ 读 `settings.txt` 回填开关与 `StructureParams` → 按 `nodes.txt` 顺序 `net.AddNode(name)`（自动建立 `Adjacency` 与 `NameToIndex`）→ 逐条 `net.AddEdge(f, t)`（会同步维护 `Parents`/`Children`；`blackEdges` 在新建网络中是空集，不会被误拦截）→ 回填 `Whitelist`/`Blacklist` 两个 `List` → 逐行恢复 `node.CPD` → 恢复 `PriorNetwork`（走 `prior.AddEdge(...)` 让 `TFNames`/`TargetNames` 两个 HashSet 自动重建）→ 恢复 `GeneExpressionData`（含 `TimePoints`，重建时 `_uniqueTimes` 保持 `Nothing` 让其惰性重算）→ 重建 `StructureResult` / `ParameterResult`（`Network` 字段指向同一个还原出的 `FittedNetwork`，避免持有多份网络副本）→ 恢复外部表达三件套 → 结尾 `.info` 日志。

**兼容与兜底**：所有 `GetEntry` 返回 `Nothing` 的分支走"空集合/默认值"降级，不抛 NRE；`meta` 缺 `version` 才视为非法文件；布尔字段用 `Boolean.Parse` 并对空值回落默认值。

**日志**：沿用项目既有的 `"..." .info` / `"..." .debug` 扩展（项目级默认导入已覆盖），只打印节点/基因/样本数量级摘要，不 dump 矩阵内容。

## 架构设计

本次为单文件局部增强，不改变现有分层：

- 持久化逻辑以 `Private Shared` 辅助方法内聚在 `BNLearnWorkflow` 类内，不新增 `IO` 层模块，避免与 `IO/BnIO.vb`、`IO/WriteModel.vb`（后者负责 TSV 结果导出，职责不同）产生职责重叠。
- `SaveModel`/`LoadModel` 签名与 `BlockBayesianNetwork` 完全对称（实例 `Sub SaveModel(Stream)` + `Shared Function LoadModel(Stream)`），R# 侧两套 `writeBin`/`readBin` 绑定因此保持一致的调用契约。

## 目录结构

```
g:\GCModeller\src\GCModeller\sub-system\BNLearn\
└── Core/
    └── BNLearnWorkflow.vb   # [MODIFY] 唯一改动文件
        # 1) Imports 追加 System.Globalization、System.IO.Compression
        # 2) 新增 Private Const ModelFormatVersion = 1
        # 3) 实现 Public Sub SaveModel(file As Stream)（替换第 558-560 行空实现）
        # 4) 实现 Public Shared Function LoadModel(file As Stream) As BNLearnWorkflow
        #    （替换第 567-569 行 Throw New NotImplementedException）
        # 5) 新增私有辅助：WriteText / GetEntry / ReadLines / ReadMeta /
        #    WriteDoubles / ReadDoubles / WriteMatrix / ReadMatrix 及数值格式化小工具
        # 6) 所有新增成员补 XML 注释（项目 GenerateDocumentationFile=True）

g:\GCModeller\src\workbench\R#\biosystem\bnlearn.vb   # [只读参照，不改动]
G:\Erica\src\Erica.sln                                 # [构建目标，不改动]
K:\hsa_grn\bnlearn.R                                   # [测试脚本，不改动]
```

## 关键代码结构

zip 内元数据契约（`meta.txt`，`key=value` 每行一条，大小写不敏感读取）：

```
' meta.txt
' version=1
' nodes=<FittedNetwork.Nodes.Count>
' edges=<FittedNetwork.EdgeCount>
' genes=<ExpressionData.NGene>       ' 无表达数据时为 0
' samples=<ExpressionData.NSample>   ' 无表达数据时为 0
' has_expr=0|1
' has_prior=0|1
' has_struct=0|1
' has_param=0|1
' has_external=0|1
```

CPD 行契约（`cpt.tsv`，制表符分隔 9 列，数值一律 G17）：

```
' nodeIndex \t intercept \t coeffs(逗号分隔) \t parentIndices(逗号分隔) _
'           \t residualSD \t residualVariance \t rsquared \t bic \t nsamples
```

矩阵二进制块契约（`expression/matrix.bin`，行优先 = gene-major）：

```
' Int32 nG ; Int32 nS ; Double[nG * nS]（第 i 个基因的第 j 个样本位于 i * nS + j）
```