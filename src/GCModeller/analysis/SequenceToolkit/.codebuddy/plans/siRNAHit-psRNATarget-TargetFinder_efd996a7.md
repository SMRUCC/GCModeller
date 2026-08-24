---
name: siRNAHit-psRNATarget-TargetFinder
overview: 在 SequenceAlignment\siRNAHit 中实现 psRNATarget 与 TargetFinder 两套 miRNA 靶标预测算法（基于反向互补 + Smith-Waterman 局部比对 + 位置加权罚分），并对两者结果取交集得到高置信靶标；同时在 test 项目中添加 miR-Demo1 与 T1~T7 靶标集的 demo 测试。
todos:
  - id: build-rna-helper
    content: 在siRNAHit\RNASeqHelper.vb实现RNA反向互补、配对分类与核酸GenericSymbol
    status: completed
  - id: define-hit-model
    content: 新建siRNAHit\siRNAHit.vb统一命中结果模型
    status: completed
    dependencies:
      - build-rna-helper
  - id: impl-targetfinder
    content: 实现TargetFinder.vb位置加权罚分与四条过滤及掩蔽重搜
    status: completed
    dependencies:
      - define-hit-model
  - id: impl-psrnatarget
    content: 实现psRNATarget.vb的V1/V2 Schema、翻译抑制判定与UPE预留接口
    status: completed
    dependencies:
      - define-hit-model
  - id: impl-intersection
    content: 实现Intersection.vb两算法结果交集合并器
    status: completed
    dependencies:
      - impl-targetfinder
      - impl-psrnatarget
  - id: add-demo-test
    content: 新建test\siRNADemo.vb并将test.vbproj的StartupObject指向该模块
    status: completed
    dependencies:
      - impl-intersection
---

## 用户需求

基于现有 Smith-Waterman 局部比对代码与 FASTA 序列模型，在 `SequenceAlignment\siRNAHit` 中实现 psRNATarget 与 TargetFinder 两款小RNA靶标预测算法，进行小RNA与目标mRNA的反向互补匹配分析，取两者结果的交集作为高置信度靶基因，并在 `SequenceAlignment\test\test.vbproj` 中添加 miR-Demo1 的 demo 测试。

## 产品概述

提供一套植物小RNA（miRNA/siRNA）靶标预测库：先对小RNA做 RNA 反向互补，再与候选mRNA做 Smith-Waterman 局部比对，最后用位置加权罚分体系量化互补质量，分别实现 psRNATarget（V1/V2 Schema + 翻译抑制判定 + 可选UPE接口）与 TargetFinder（位置加权矩阵 + 四条过滤 + 掩蔽重搜）两套算法，并对结果取交集。

## 核心功能

- RNA 反向互补工具（A<->U, G<->C），独立核酸打分矩阵构造
- 碱基配对分类：Watson-Crick / G:U wobble / mismatch / gap
- psRNATarget 算法：V1/V2 Schema、位置加权罚分、Seed区≤2错配限制、gap罚分、期望值阈值、切割位点翻译抑制判定、UPE可及性预留接口（默认关闭）
- TargetFinder 算法：位置加权（1位×1、2–13位×2、14–21位×1）、四条过滤规则、掩蔽重搜（two-hits）
- 统一命中结果模型（miRNA/靶标ID、坐标、罚分、错配/G:U/gap计数、比对串、是否翻译抑制候选）
- 交集合并器：按 (miRNA, 靶标, 靶位点坐标容差) 对齐两算法结果，输出高置信靶标集
- test 项目 demo：miR-Demo1 对 7 条 mRNA 的预测、两算法结果及交集打印与 PASS/FAIL 断言

## 技术栈

- 语言：VisualBasic .NET（SDK 风格 vbproj，源文件自动 glob）
- 比对引擎：复用 `Microsoft.VisualBasic.DataMining.DynamicProgramming.SmithWaterman.GSW(Of Char)`（现有泛型 SW 核心），通过自定义 `GenericSymbol(Of Char)`（+15/-10 核酸打分）驱动，不走重量级 `Output`
- 序列模型：`SMRUCC.genomics.SequenceModel.FASTA.FastaSeq`
- 工程：新增代码置于 `SequenceAlignment\siRNAHit\`，无需修改 `SequenceAlignment.vbproj` 引用（已含 DynamicProgramming/Bio.Assembly/Core）

## 实现方案

整体策略：以 miRNA 反向互补序列为正向 query，对每条 mRNA 调用 `GSW(Of Char).BuildMatrix()` 后取 `GetBestHSP`（轻量路径，避免复制 DP 矩阵进入 LOH），再从 HSP 对齐串中按 miRNA 5'→3' 逐位计算配对类型与位置加权罚分，最后按各算法规则过滤并取交集。

关键决策：

1. **不新建 DP**：直接 `New GSW(Of Char)(revCompToArray, mrnaArray, rnaSymbol).BuildMatrix()`，复用 `GetBestHSP(cutoff, minW)` 获取唯一最佳比对，时间复杂度 O(n·m)（n≈21, m≈50），批量 mRNA 也无需构建 `Output`。
2. **核酸打分符号**：构造 `GenericSymbol(Of Char)(equals:=相同碱基, similarity:=Function(x,y) If(isMatch(x,y),15,-10), toChar:=Identity, empty:=Function() "-"c)`。SW 中 gap 由 `INDEL_SCORE` 常量控制，但 `GSW` 的 INDEL 为固定 -9；为贴合 TargetFinder（-f -16 / -g -10）与 psRNATarget（open=2/ext=0.5），在 HSP 回溯源序列统计 gap open/extend 次数并单独施加罚分，SW 分数仅用于定位最佳局部比对，不直接作为期望/罚分。
3. **配对类型判定**：基于 miRNA 正向位与 mRNA 靶位点位的 `seq1/seq2` 对齐字符（已做反向互补，故比对为同向），对应位置 `('-','-')` 为 gap，`x=y` 为 WC，`(G,U)/(U,G)` 为 G:U，其余为 mismatch。
4. **位置权重**：从 miRNA 5' 端（query 起点）按 1× / 2–13×2 / 14–21×1 计数，G:U base=0.5、mismatch base=1.0、gap base=1.0。
5. **交集容差**：两算法靶位点坐标（mRNA fromB/toB）允许 ±3 nt 容差对齐，按 (miRNA.id, mRNA.id, 重叠位点) 取交集。

避免技术债：沿用 `Diamond` 模块的命中类/算法类/参数枚举分离风格；UPE 仅定义接口与默认关闭分支，不引入 Vienna RNA 外部依赖。

## 实现要点

- 性能：miRNA 短序列（21nt），每条 mRNA 一次 O(nm) SW，demo 仅 7 条，开销可忽略；使用 `GetBestHSP` 轻量路径，不在循环内构造 `Output`。
- 日志：复用 `Microsoft.VisualBasic` 的 `debug`/Console 打印，避免敏感信息；demo 用 Console.WriteLine + PASS/FAIL 断言。
- 兼容：不改动 `SequenceAlignment.vbproj` 既有结构；test 工程新增模块并通过 `<StartupObject>` 指向新模块，不影响现有 DiamondDemo。

## 架构设计

```mermaid
graph TD
    A[siRNADemo test module] --> B[siRNAHit Predictor API]
    B --> C[TargetFinder]
    B --> D[psRNATarget]
    C --> E[RNA ReverseComplement + GSW(Of Char) + PairingScore]
    D --> E
    B --> F[IntersectionMerger]
    C --> F
    D --> F
    F --> G[HighConfidenceTargets]
```

## 目录结构

```
SequenceAlignment/
├── siRNAHit/                      # [NEW] 小RNA靶标预测实现目录
│   ├── RNASeqHelper.vb            # [NEW] RNA反向互补(UCAG互换)、碱基配对分类(WC/G:U/mismatch/gap)、核酸GenericSymbol(+15/-10)构造
│   ├── siRNAHit.vb               # [NEW] 统一命中结果模型: miRNA/靶标ID、fromB/toB坐标、score、mismatch/G:U/gap计数、align字符串、TranslationInhibition候选标记
│   ├── TargetFinder.vb           # [NEW] TargetFinder算法: 位置加权罚分(1×/2-13×2/14-21×1)、四条过滤、掩蔽重搜get_additional、Run(mirna,mrnaSet)返回命中集
│   ├── psRNATarget.vb            # [NEW] psRNATarget算法: V1/V2 Schema(Seed 2-8/2-13, V2≤2错配, gap open2/ext0.5, 期望值阈值)、切割位点翻译抑制判定、UPE可选接口(默认关闭)、Run(mirna,mrnaSet)
│   └── Intersection.vb           # [NEW] 交集合并器: 按(miRNA,靶标,坐标容差±3)对齐两算法命中集, 输出高置信靶标
└── test/
    └── siRNADemo.vb              # [NEW] demo模块: 定义miR-Demo1与T1~T7, 调用两算法与交集, 打印结果并断言T6被过滤、T1在交集内
```

## 关键代码结构

```
Namespace SMRUCC.genomics.Analysis.SequenceAlignment.siRNAHit

    Public Class siRNAHit
        Public Property miRNA As String
        Public Property Target As String
        Public Property StartSite As Integer   ' mRNA 靶位点起点(fromB, 1-based)
        Public Property EndSite As Integer     ' mRNA 靶位点终点(toB)
        Public Property Expectation As Double  ' psRNATarget: 加权罚分; TargetFinder: penalty sum
        Public Property MismatchCount As Integer
        Public Property WobbleCount As Integer  ' G:U 数
        Public Property GapCount As Integer
        Public Property Alignment As String      ' 比对可视化
        Public Property TranslationInhibition As Boolean
    End Class

    Public Interface IAccessibilityEvaluator
        ' psRNATarget UPE 可选接口, 默认返回0(关闭)
        Function UPE(mrna As String, siteStart As Integer, siteEnd As Integer) As Double
    End Interface
End Namespace
```