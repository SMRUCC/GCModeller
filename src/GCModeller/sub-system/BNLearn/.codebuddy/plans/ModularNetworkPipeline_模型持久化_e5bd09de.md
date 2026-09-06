---
name: ModularNetworkPipeline 模型持久化
overview: 实现 ModularNetwork\ModularNetworkPipeline.vb 的 SaveModel/LoadModel，把训练好的模块化贝叶斯网络流水线（全局基因索引、全局雅可比矩阵 A、全局聚合网络及 CPD、标准化表达矩阵、WGCNA 模块划分与 hub、各子网络、全部训练/传播参数）序列化为 zip 并支持原样还原；同时对 BlockNetwork.vb 做最小可见性调整以便回填内部状态；最后在 test\WGCNADemo.vb 新增往返一致性测试并实际运行验证。
todos:
  - id: adjust-blocknetwork
    content: 将 BlockNetwork.vb 五个内部字段改 Friend 并新增无参构造器
    status: completed
  - id: impl-persistence
    content: 在 ModularNetworkPipeline.vb 实现 SaveModel/LoadModel 与 zip 读写辅助函数
    status: completed
    dependencies:
      - adjust-blocknetwork
  - id: add-test
    content: 在 test\WGCNADemo.vb 新增 RunPersistenceTest 并在 Program.vb 加参数分派
    status: completed
    dependencies:
      - impl-persistence
  - id: build
    content: 编译 test.vbproj 与 Erica.sln，修正编译错误
    status: completed
    dependencies:
      - add-test
  - id: run-test
    content: 运行持久化往返测试，核对断言输出并修复问题
    status: completed
    dependencies:
      - build
---

## 产品概述

为 BNLearn 子系统的 WGCNA 模块化贝叶斯网络流水线 `ModularNetworkPipeline` 补齐模型持久化能力：把训练完成的流水线（全局基因索引、全局雅可比矩阵 A、全局聚合网络及其 CPD、标准化表达矩阵、WGCNA 模块划分与 hub 基因、各模块子网络、全部训练与传播参数）序列化为 zip 压缩包，并支持从该 zip 原样还原出一个可直接执行虚拟扰动的流水线对象。

## 核心功能

- **模型导出（SaveModel）**：将 `ModularNetworkPipeline` 的全部状态写入 zip 压缩包，输出流由调用方提供并负责释放。
- **模型载入（LoadModel）**：从 zip 反向重建 `ModularNetworkPipeline`，还原后无需重新训练即可直接调用 `GetModuleHubSources`、`InsilicoPerturbation`（Jacobian 与 CascadeSampling 两种传播）、`SaveResults`。
- **版本与完整性校验**：zip 内写入格式版本号，载入时校验；元数据缺失或版本不符时抛出可读的 `InvalidDataException`；未训练即导出时抛出友好异常。
- **往返保真**：全局雅可比矩阵 A、全局网络拓扑与 CPD、标准化表达矩阵、模块划分与 hub、各模块子网络、全部标量参数均需无损还原，载入后重跑虚拟扰动应得到与原模型逐元素一致的结果。
- **验证闭环**：在 `test\WGCNADemo.vb` 新增持久化往返测试，实际编译并运行，打印逐项断言结果与 PASS/FAIL 汇总。

## 技术栈

- 语言/框架：VB.NET，目标框架 `net10.0`（`BNLearn.vbproj` / `test.vbproj`，`OptionStrict Off`、`OptionInfer On`）
- 压缩：`System.IO.Compression.ZipArchive` / `ZipArchiveEntry`（BCL 内置，不新增依赖）
- 数值序列化：文本用 `G17` + `CultureInfo.InvariantCulture`；大矩阵（表达矩阵、雅可比矩阵 A）用 `BinaryWriter` / `BinaryReader` 二进制块
- 构建：`G:\Erica\src\Erica.sln`（含 `BNLearn.vbproj`）+ `test\test.vbproj`
- 运行测试：`test\bin\x64\Debug\net10.0\test.exe`

## 实现方案

**策略**：完全对齐上一轮已在 `Core\BNLearnWorkflow.vb` 落地并通过验证的 zip 持久化模式（同样的 `WriteText` / `GetEntry` / `ReadLines` / `ReadNames` / `ReadMeta` / `WriteDoubles` / `ReadDoubles` / `WriteMatrix` / `ReadMatrix` 约定，同样的 `leaveOpen:=True` 语义、版本校验、`G17` 数值格式、`.info` 收尾日志）。不引入新架构、不新增文件，新增代码内聚在 `ModularNetworkPipeline` 类内。

**为什么必须落盘表达矩阵与全局网络**：`InsilicoPerturbation` 的 CascadeSampling 分支构造 `New BnInterventionAnalyzer(Model._globalNet, Model._exprStd)`，两者缺一即空引用；Jacobian 分支直接用 `Model._A` 与 `Model._genes`。因此"只存标量 + 系数矩阵"不足以支撑级联传播。

**为什么显式存 A 而不从全局 CPD 反推**：`BuildGlobalNetwork` 最后确实用 CPD 重写了 A，二者理论上互推；但 A 是 Jacobian 传播的唯一输入、也是本模型的核心产物，显式落盘可保证 bit 级无损且不依赖两套数据的一致性假设，代价是 n×n 个 double（2000 基因约 32MB，Deflate 后显著缩小）。

**为什么跳过原始 `_expr`**：它是未标准化全量输入矩阵，体积与 `_exprStd` 相当，且仅在 `Learn()` 末尾一句 debug 日志里用到 `.GeneNames`。载入时令 `_expr = _exprStd` 降级并在注释中写明。

**对 `BlockNetwork.vb` 的最小改动**：`_expr` / `_gIndex` / `_moduleGenes` / `_moduleHubs` / `_subNets` 五个 `Private` 字段改 `Friend`（与既有 `_globalNet` / `_A` / `_exprStd` 已是 `Friend` 的风格一致），并新增一个无参 `Friend Sub New()` 供反序列化构造。相比在 `BlockNetwork.vb` 里塞进上百行序列化代码，这种方式改动面最小。

## 实现要点（执行细节）

**Imports 追加**：`System.Globalization`、`System.IO.Compression`。

**注意**：该文件与 `BNLearnWorkflow.vb` 一样，`Math.Round` / `Math.Max` 会因 `Math` 被其他类型遮蔽而编译报 BC30456，一律改用 `CInt()` 与 `If(a &gt; b, a, b)`。

**类内新增**：

- `Private Const ModelFormatVersion As Integer = 1`
- `WriteText` / `GetEntry` / `ReadLines` / `ReadNames`（保留空行，避免行号与索引错位）/ `ReadMeta`
- `WriteDoubles` / `ReadDoubles`（Int32 长度 + 数据体）、`WriteMatrix` / `ReadMatrix`（Int32 nG + Int32 nS + 行优先数据体）
- `WriteNet(zip, prefix, net)` / `ReadNet(zip, prefix)`：把 `BayesianNetwork` 的 `nodes.txt` / `edges.tsv` / `cpt.tsv` 三段读写抽成一对函数，全局网络与每个子网络共用
- `Num(d)` / `ParseNum(s)` / `JoinNums` / `JoinInts` / `ParseNums` / `ParseInts` / `Sanitize`（先 `IndexOfAny` 快探，无非法字符直接返回，避免数十万次无谓字符串替换）/ `GetValue` / `GetBool` / `GetInt` / `GetDouble`

**zip 布局**：

| entry | 内容 |
| --- | --- |
| `meta.txt` | `version` / `genes` / `modules` / `subnets` / `samples` / `has_global` / `has_expr` |
| `settings.txt` | pipeline 标量：`Propagation`（枚举名）、`MaxSteps`、`Tolerance`、`NSamples`、`RandomSeed`、`NormalizeData` |
| `network.txt` | BlockNetwork 标量：`HubTopN`、`CrossModuleCorThreshold`、`CrossGeneCorThreshold`、`CrossScale` + `StructureParams` 全字段（`Algorithm` 用枚举名，其余 `Alpha`/`MaxParents`/`TabuLength`/`MaxIterations`/`BICPenalty`/`UseWhitelist`/`UseBlacklist`/`RandomSeed`） |
| `propagate.txt` | `MaxSteps` / `Tolerance` / `NSamples` / `RandomSeed` |
| `genes.txt` | 全局基因名，行号即 `GetGlobalIndex` 的索引 |
| `A.bin` | 二进制 `nG, nG, Double(nG*nG)`（行优先 = child-major） |
| `global/nodes.txt`、`global/edges.tsv`、`global/cpt.tsv` | 全局聚合网络（含全部 CPD） |
| `expr/genes.txt`、`expr/samples.txt`、`expr/timepoints.bin`、`expr/matrix.bin` | `_exprStd` 标准化表达矩阵 |
| `modules.tsv` | `color \t geneCount \t hubCount`，一行一模块，行号即模块序 |
| `modules/0000/genes.txt`、`modules/0000/hubs.txt` | 模块成员基因与 hub 基因 |
| `subnets/0000/{nodes.txt,edges.tsv,cpt.tsv}` | 各模块子网络（`_subNets` 的序号目录，与 `modules` 目录解耦，因为子网络会跳过基因数 &lt; 2 的模块） |


**SaveModel 流程**：`s Is Nothing` → `ArgumentNullException`；`model Is Nothing` 或 `model._globalNet Is Nothing` → 抛"尚未调用 Learn 训练"的友好异常；`Using zip As New ZipArchive(s, ZipArchiveMode.Create, leaveOpen:=True)` 按上表写出；结尾 `.info` 打印模块数 / 基因数 / 边数量级摘要。

**LoadModel 流程**：`Using zip As New ZipArchive(s, ZipArchiveMode.Read, leaveOpen:=True)` → 读 `meta.txt` 校验 version → 读三个标量文件回填 → `New BlockNetwork()`（新无参构造器）→ 按 `genes.txt` 重建 `_genes` 与 `_gIndex` → 读 `A.bin` → `ReadNet(zip, "global/")` 重建 `_globalNet`（用 `AddNode` 建节点、`AddEdge` 加边以同步维护 `Parents`/`Children`，再回填 CPD）→ 读 `expr/*` 重建 `_exprStd`（`_expr` 置为同一实例）→ 读 `modules.tsv` + `modules/0000/*` 重建 `_moduleGenes` / `_moduleHubs` → 读 `subnets/*` 重建 `_subNets` → 构造 `BlockPropagate` 并回填四参数与 `Model` → 组装返回。所有 `GetEntry` 返回 `Nothing` 的分支走空集合/默认值降级，不抛 NRE。

**日志**：沿用项目既有的 `"..." .info` / `"..." .debug` 扩展，只打印规模摘要，不 dump 矩阵内容。

## 架构设计

本次为定点增强，不改变现有分层：

- 持久化逻辑以 `Private Shared` 辅助方法内聚在 `ModularNetworkPipeline` 类内，与 `BNLearnWorkflow`、`BlockBayesianNetwork` 各自持有私有 zip 辅助的现有约定保持一致（不引入共享模块，避免触碰上一轮已验证通过的代码路径造成回归）。
- `SaveModel` / `LoadModel` 签名与同项目另两个模型类完全对称（实例 `Sub SaveModel(Stream)` + `Shared Function LoadModel(Stream)`），便于后续按需在 `bnlearn.vb` 里补 `writeBin` / `readBin` 绑定（本轮不做）。
- `BlockNetwork` 仅放宽字段可见性 + 补无参构造器，不承载序列化职责。

## 目录结构

```
g:\GCModeller\src\GCModeller\sub-system\BNLearn\
├── ModularNetwork/
│   ├── ModularNetworkPipeline.vb   # [MODIFY] 主改动
│   │   # 1) Imports 追加 System.Globalization、System.IO.Compression
│   │   # 2) 新增 Private Const ModelFormatVersion = 1
│   │   # 3) 实现 Public Sub SaveModel(s As Stream)（替换 185-187 行空体）
│   │   # 4) 实现 Public Shared Function LoadModel(s As Stream) As ModularNetworkPipeline
│   │   #    （替换 194-196 行空体）
│   │   # 5) 新增 Private Shared 辅助：WriteText/GetEntry/ReadLines/ReadNames/ReadMeta/
│   │   #    WriteDoubles/ReadDoubles/WriteMatrix/ReadMatrix/WriteNet/ReadNet/
│   │   #    Num/ParseNum/JoinNums/JoinInts/ParseNums/ParseInts/Sanitize/
│   │   #    GetValue/GetBool/GetInt/GetDouble
│   │   # 6) 所有新增成员补 XML 注释（项目 GenerateDocumentationFile=True）
│   └── BlockNetwork.vb             # [MODIFY] 最小改动
│       # 1) _expr / _gIndex / _moduleGenes / _moduleHubs / _subNets：Private → Friend
│       # 2) 新增 Friend Sub New()（仅供反序列化回填）
└── test/
    ├── WGCNADemo.vb                # [MODIFY] 新增 Sub RunPersistenceTest()
    │   # 读数据 → 训练 → 取少量源跑 Jacobian(+少量 Cascade) 得 before
    │   # → SaveModel 到 zip → LoadModel 回来
    │   # → 断言：基因名序列 / A 逐元素 / 全局网络节点+边+CPD 各字段 /
    │   #   表达矩阵维度与抽样值 / 模块 hub 源集合 / 全部标量
    │   # → 在载入模型上重跑同样扰动，Effects 与 before 逐元素比对
    │   # → 再 SaveModel 一次，逐条目比对两次 zip 解压内容（往返保真）
    │   # → 打印 PASS/FAIL 汇总
    └── Program.vb                  # [MODIFY] Main 增加参数分派
        # args 含 "persistence" → WGCNADemo.RunPersistenceTest()，否则仍调 WGCNADemo.Run()
        # （保持既有 Run() 行为不变）
```

## 关键代码结构

zip 元数据契约（`meta.txt`，`key=value` 每行一条，读取时大小写不敏感）：

```
' version=1
' genes=<_genes.Length>
' modules=<_moduleGenes.Count>
' subnets=<_subNets.Count>
' samples=<_exprStd.NSample>
' has_global=0|1
' has_expr=0|1
```

网络三段式契约（`WriteNet` / `ReadNet` 共用，`prefix` 为 `global/` 或 `subnets/0000/`）：

```
' <prefix>nodes.txt   每行一个节点名，行号即节点索引
' <prefix>edges.tsv   fromIdx \t toIdx
' <prefix>cpt.tsv     nodeIndex \t intercept \t coeffs(逗号) \t parentIndices(逗号) _
'                     \t residualSD \t residualVariance \t rsquared \t bic \t nsamples
```

二进制块契约：

```
' A.bin                Int32 nG ; Int32 nG ; Double[nG*nG]（第 i 个 child 的第 j 个 parent 位于 i*nG+j）
' expr/matrix.bin      Int32 nG ; Int32 nS ; Double[nG*nS]（行优先，gene-major）
' expr/timepoints.bin  Int32 count ; Double[count]
```