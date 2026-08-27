---
name: WGCNA子网络全局虚拟扰动流程
overview: 基于DBNBlocks.md思路，利用WGCNA模块划分，为每个模块子集训练静态BN（结构+参数学习），拼出全局系数矩阵，实现雅可比矩阵传播与级联采样传播两种全局虚拟扰动方法（可参数切换，默认雅可比），并改写WGCNADemo.vb测试生成TSV+控制台结果。
todos:
  - id: explore-apis
    content: 用 [subagent:code-explorer] 确认 BnStructureLearner、BnParameterLearner、StructureLearningParams、WriteModel 的精确签名
    status: completed
  - id: add-submatrix
    content: 为 GeneExpressionData 新增 GetSubMatrix 按基因名提取子矩阵方法
    status: completed
    dependencies:
      - explore-apis
  - id: build-pipeline
    content: 新增 WGCNADBN 命名空间：Pipeline、GlobalPerturbationResult、PropagationMethod 实现训练与雅可比传播
    status: completed
    dependencies:
      - add-submatrix
  - id: cascade-mode
    content: 暴露 Intervention 的干预网络/采样辅助，实现级联采样跨模块传播与方法切换
    status: completed
    dependencies:
      - build-pipeline
  - id: rewrite-demo
    content: 改造 WGCNADemo.vb 调用流程，写出全局扰动 TSV 并打印摘要
    status: completed
    dependencies:
      - cascade-mode
---

## 用户需求

基于 DBNBlocks.md 文档思路，构建一套"WGCNA 模块划分 → 分模块训练 bnlearn 静态贝叶斯子网络 → 全局虚拟扰动"的完整流程，并改造测试代码生成可供检查的扰动结果。

## 产品概述

在现有 BNLearn 项目内新增一条流水线：读取 WGCNA 模块划分结果（基因-模块色-成员度kME）与表达矩阵，按模块切分基因集合，为每个模块独立训练一个高斯贝叶斯子网络（结构学习 + 参数学习），再依文档把各子网回归系数拼成块对角全局系数矩阵 A；随后在整合后的全局网络上执行虚拟扰动传播，并支持两种传播方法由参数切换。

## 核心功能

- 模块切分：基于 WGCNA 读取的 GeneModuleColor（geneID / moduleColor / kME）把基因归入各颜色模块，并建立全局基因索引（基因名→全局序号）。
- 子网络训练：对每个模块内的基因子集，复用现有 BnStructureLearner + BnParameterLearner 训练静态 BN，提取每节点回归系数 Coeffs 与 ParentIndices。
- 全局系数矩阵拼接：将所有子网的回归系数填入全局 N×N 雅可比矩阵 A（A[i,j]=基因j→i系数，块内非零、块间默认零），同时记录每模块 hub 基因（kME 最高若干）作为模块接口。
- 全局虚拟扰动（两种方法，参数切换，默认雅可比）：
- 雅可比传播：δ 在源基因处置扰动值，e_k = A^k·δ 迭代多步至收敛，得到全局效应向量。
- 级联采样传播：从源模块 do-演算扰动出发，按拓扑序把上一步均值作为相邻模块证据/输入迭代采样多步，跨模块传播。
- 结果输出：生成全局扰动响应矩阵（基因 × 扰动源），复用 IO.BnIO 写出 TSV，并在控制台打印 Top 变化基因摘要。
- 测试改造：修改 test\WGCNADemo.vb，调用上述流程，遍历模块代表源基因执行全局扰动并落盘 + 打印摘要供用户检查。

## 技术栈选择

- 语言/框架：VB.NET（与现有 BNLearn 项目完全一致，避免引入新依赖）
- 数学计算：复用 `Microsoft.VisualBasic.Math.LinearAlgebra.Matrix` 与现有 `MatrixOps.Inverse`；新增简单矩阵乘/迭代逻辑（无需第三方库）
- 数据流：复用现有 `Core.GeneExpressionData`、`Core.BayesianNetwork`、`Core.BnNode/BnCPD`、`WGCNA` 读取模块、`Intervention.BnInterventionAnalyzer`、`IO.BnIO`

## 实现方案

按"分而治之训练、合而为一扰动"的文档原则实现：

1. 新增 `Core/WGCNADBN` 命名空间，核心类 `WGCNASubnetworkPipeline` 串联 模块切分→子网络训练→全局矩阵拼接→扰动传播→导出。
2. 模块切分复用 `WGCNA.ReadModuleAssignment` 返回的 `GeneModuleColor[]`（geneID、moduleColor、kME）。构建 `Dictionary(Of String, Integer)` 全局索引，并把每个模块的基因集合 `Dictionary(Of String, List(Of String))` 缓存。
3. 子网络训练：对每个模块基因子集，从全局 `GeneExpressionData` 提取子矩阵（新增 `GetSubMatrix(geneNames)` 扩展方法，返回新 `GeneExpressionData`），复用 `StructureLearning.BnStructureLearner.Learn` + `BnParameterLearner.Learn` 得到 `BayesianNetwork`。保持与 `BNLearnWorkflow` 一致的 `NormalizeData`（Standardize）、`StructureParams`、`NSamples`、`RandomSeed` 参数风格。
4. 全局系数矩阵：遍历每个子网节点，用 `BnCPD.Coeffs(j)` 对应 `ParentIndices(j)` 全局序号填入全局 `A(i,j)`。块内非零、块间为零（初始全零）。同时按每个模块 kME 降序取 Top-N hub 基因，保存 `Dictionary(Of String, List(Of String))` 用于级联采样的跨模块证据桥接。
5. 两种扰动传播（参数 `PropagationMethod` 枚举，默认 `Jacobian`）：

- 雅可比：`delta0` 在源基因处置扰动幅度（敲低-1/过表达+幅度），迭代 `e_{k+1}=A·e_k`，收敛阈值 `tol` 或最大步数 `maxSteps`，返回稳态效应向量。复杂度 O(maxSteps·N²)，N 为全局基因数，可控。
- 级联采样：复用 `BnInterventionAnalyzer` 的 do-演算思想（为支持跨模块，新增 public 包装 `CreateInterventionNetwork` 与按初始向量采样的辅助，或在 pipeline 内基于 `BnInferenceEngine.Sample` 实现），从源模块扰动开始，将上一步各基因均值作为相邻模块网络的证据/父值，迭代 nSteps 跨模块传播；性能为 O(nSteps·Σ模块节点·nSamples)。

6. 结果结构 `GlobalPerturbationResult`：源基因、方法、最终效应向量、逐步效应、Top 变化基因。导出复用 `IO.BnIO.WriteInterventionResult` 风格写 TSV（全局响应矩阵 + 各源明细），并控制台打印摘要。
7. 测试改造 `test/WGCNADemo.vb`：读取 WGCNA 结果，构建 pipeline，对每个模块选取代表 hub 基因（或一个指定源基因列表）执行全局扰动，写出 TSV 并输出摘要。

## 实现注意

- 全局索引统一：所有子网络训练前后必须共用同一 `GlobalName→Index` 映射，保证 A 矩阵行/列对齐。
- 跨模块边：遵循文档，默认块间为零（硬切分），级联采样法通过 hub 基因作为模块接口传递均值，避免构造不存在的跨模块父边导致结构学习异常。
- 性能：雅可比法为纯矩阵乘，N 大时控制 maxSteps（建议默认 50）并做增量范数收敛判断；级联采样法 nSamples 复用 workflow 的 NSamples 默认值，避免过大。
- 向后兼容：不改动现有 `BNLearnWorkflow`、`BnInterventionAnalyzer`、`IO` 的公开接口语义；仅新增扩展方法与 public 包装，降低回归风险。
- 日志：复用现有 `.debug` 扩展方法打印关键阶段（模块数、各模块节点数、传播步数），避免大对象 dump。
- 空模块/单基因模块：跳过结构学习（无父节点），A 对应行保持零，不抛异常。

## 架构设计

```mermaid
graph TD
    A[WGCNA 结果 CSV] -->|ReadModuleAssignment| B(GeneModuleColor[])
    C[表达矩阵 CSV] -->|BnIO.ReadGeneExpressionMatrix| D(GeneExpressionData)
    B --> E[WGCNASubnetworkPipeline]
    D --> E
    E -->|模块切分+全局索引| F[各模块基因子集]
    F -->|GetSubMatrix| G[子网络训练 BnStructureLearner+BnParameterLearner]
    G --> H[模块 BayesianNetwork[]]
    H -->|BnCPD.Coeffs| I[全局系数矩阵 A N×N]
    H -->|kME TopN| J[模块 Hub 接口]
    I --> K{PropagationMethod}
    J --> K
    K -->|Jacobian 默认| L[雅可比多步传播]
    K -->|Cascade 采样| M[级联采样跨模块传播]
    L --> N[GlobalPerturbationResult]
    M --> N
    N -->|IO.BnIO 写 TSV| O[扰动响应矩阵文件]
    N -->|控制台摘要| P[Top 变化基因]
```

## 目录结构

```
BNLearn/
├── Core/
│   └── WGCNADBN/                         # [NEW] 新增命名空间目录，承载模块子网+全局扰动流程
│       ├── WGCNASubnetworkPipeline.vb    # [NEW] 流水线主类：模块切分、子网络训练、A矩阵拼接、扰动方法切换、结果导出。复用 WGCNA/GeneExpressionData/BnStructureLearner/BnParameterLearner/IO.BnIO；提供 RunFullPipeline、TrainSubnetworks、BuildGlobalJacobian、Propagate 方法。
│       ├── GlobalPerturbationResult.vb   # [NEW] 全局扰动结果结构：SourceGene、Method、Effects(N)、StepEffects、TopChangedGenes、ToTSV/PrintSummary 辅助。
│       └── PropagationMethod.vb          # [NEW] 枚举 Jacobian / CascadeSampling，供 pipeline 参数切换，默认 Jacobian。
├── Core/
│   └── GeneExpressionData.vb             # [MODIFY] 新增 GetSubMatrix(geneNames As String()) 扩展方法，按基因名子集提取并返回新的 GeneExpressionData（行列对齐、保留 TimePoints）。
├── Intervention/
│   └── Intervention.vb                   # [MODIFY] 将 CreateInterventionNetwork 暴露为 public/Friend，并新增基于给定初始均值向量采样的 public 辅助（供级联采样跨模块复用），不改动现有 AnalyzeIntervention/DynamicIntervention 行为。
└── test/
    └── WGCNADemo.vb                      # [MODIFY] 改写为：读取 WGCNA 模块结果与表达矩阵，构建 WGCNASubnetworkPipeline 训练子网，对每个模块 hub 代表基因执行全局虚拟扰动，复用 IO 写出 TSV 并打印 Top 变化摘要。
```

## 关键代码结构

```
Namespace Core.WGCNADBN

    Public Enum PropagationMethod
        Jacobian          ' 默认：全局系数矩阵多步线性传播
        CascadeSampling   ' 级联采样跨模块传播
    End Enum

    Public Class WGCNASubnetworkPipeline
        Public Property NormalizeData As Boolean = True
        Public Property StructureParams As New StructureLearning.StructureLearningParams()
        Public Property NSamples As Integer = 10000
        Public Property RandomSeed As Integer = 42
        Public Property Propagation As PropagationMethod = PropagationMethod.Jacobian
        Public Property MaxSteps As Integer = 50
        Public Property Tolerance As Double = 1.0E-6
        Public Property HubTopN As Integer = 20

        Public Function Run(assignment As GeneModuleColor(),
                            expr As Core.GeneExpressionData) As List(Of GlobalPerturbationResult)
        ' 1) 切分模块+全局索引 2) 训练子网 3) 拼 A 4) 各源基因扰动 5) 导出
    End Class

End Namespace
```

## Agent Extensions

### SubAgent

- **code-explorer**
- Purpose: 在实现前深入检索 BnStructureLearner、BnParameterLearner、StructureLearningParams、WriteModel 等现有类型与方法的确切签名，确保 pipeline 正确调用、避免编造 API。
- Expected outcome: 获得结构学习/参数学习的精确调用方式、参数对象字段，以及 WriteModel 写入格式，使新增代码一次编译通过。