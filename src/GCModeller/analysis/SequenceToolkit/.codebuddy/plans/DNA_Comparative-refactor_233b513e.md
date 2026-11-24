---
name: DNA_Comparative-refactor
overview: 按 Karlin 1998《Comparative DNA Analysis》论文重构 DNA_Comparative 项目：修正现有算法实现错误（ρ* 对称化、CodonSignature、CAI），补全全部 9 个算法模块（ρ*签名+分级、δ*距离、密码子位点签名、τ*四核苷酸、r-scan、CAI/B(F|C)、二维阈值外来基因判别、滑动窗口δ*曲线、(C−G)/(C+G)链偏差），按论文概念彻底重命名公共 API 并同步更新全仓库调用方，并做面向大规模基因组数据的性能优化（消除 SlideWindow 对象分配、单趟计数缓存、SIMD 适用性评估）。
todos:
  - id: verify-callers-and-apis
    content: 使用 [subagent:code-explorer] 核对8个调用方文件中旧API的具体调用形态，以及Codon/TranslTable/SlideWindow等基础库API签名，产出重写对接备忘
    status: completed
  - id: rewrite-signature-core
    content: 重写NucleicAcid.vb为整数计数矩阵签名缓存（单趟扫描、对称化计数、数组索引ρ*），并修正GenomeSignatures.vb为ρ*对称化+六级显著性分级，清理BIAS/BIAS_p冗余
    status: completed
    dependencies:
      - verify-callers-and-apis
  - id: delta-star-distance
    content: 将DifferenceMeasurement.vb改名DeltaStarDistance.vb，合并5个重复重载为δ*=(1/16)Σ|Δρ*|主路径，调整SimilarDiscriptions枚举命名
    status: completed
    dependencies:
      - rewrite-signature-core
  - id: codon-signature-tetranucleotide
    content: 新建CodonSignature.vb实现位点特异ρXY(1,2)/ρYZ(2,3)/ρXZ(1,3)/ρZW(3,4)基因集合统计，重写CodonBiasVector语义；新建TetranucleotideBias.vb实现τ*广义odds ratio
    status: completed
    dependencies:
      - rewrite-signature-core
  - id: cai-bias-alien
    content: 重写CAI/RelativeCodonBiases.vb为两步式w表+几何平均，适配XML持久化；新建CodonBiasMeasure.vb实现B(F|C)偏差距；新建AlienGeneDetection.vb实现二维阈值外来基因判别
    status: completed
    dependencies:
      - codon-signature-tetranucleotide
  - id: rscan-window-asymmetry
    content: vbproj新增stats-netcore5引用；新建RScanWordDistribution.vb对接基础库RScanCore.Scan做词分布检验；新建SlidingWindowDelta.vb（增量滑窗δ*曲线）与ReplicationAsymmetry.vb（(C−G)/(C+G)链偏差）
    status: completed
    dependencies:
      - rewrite-signature-core
  - id: toolsapi-refactor
    content: 重构ToolsAPI.vb（修复比较序列O(n)重复构造、滑窗δ*剖面改增量计算、按论文概念重命名函数与ExportAPI名），更新IdentityResult.vb为DeltaStarMatrix并复用预构建签名
    status: completed
    dependencies:
      - delta-star-distance
  - id: global-rename-callers
    content: 使用 [skill:lsp-code-analysis] 按论文概念彻底重命名全仓库公共符号（Sigma→DeltaStar、DinucleotideBIAS→GenomeSignature等），同步更新8个调用方文件及ExportAPI字符串名
    status: completed
    dependencies:
      - toolsapi-refactor
  - id: simd-analysis-doc
    content: 对16维|ρf−ρg|求和与(C−G)/(C+G)剖面选择性应用Microsoft.VisualBasic.Math.SIMD向量计算（含SIMDEnvironment回退标量），并在代码注释与readme.md中写入SIMD适用性分析结论
    status: completed
    dependencies:
      - delta-star-distance
  - id: build-verify
    content: dotnet build构建DNA_Comparative.netcoreapp.vbproj及全部受影响调用方项目，用测试序列验证ρ*对称性、δ*自比较为0、CAI合理区间等基本正确性，更新readme.md记录重构结果
    status: completed
    dependencies:
      - global-rename-callers
      - simd-analysis-doc
---

## 需求概述

针对 `DNA_Comparative\DNA_Comparative.netcoreapp.vbproj` 项目，以 Karlin, Campbell & Mrázek (1998)《Comparative DNA Analysis Across Diverse Genomes》论文（算法讲解见 `DNA_Comparative\readme.md`）为基准进行整体重构，共四项任务：

### 1. 修正算法实现错误

- `DinucleotideBIAS`（ρ）：未做论文要求的双链对称化（应将序列与其反向互补序列拼接后统计得 ρ\*XY），且缺少六级显著性分级（`−−−`(<0.50)/`−−`(0.50–0.70)/`−`(0.70–0.78)/`+`(1.23–1.30)/`++`(1.30–1.50)/`+++`(>1.50)，阈值 0.78/1.23）
- `GenomeSignatures.CodonSignature`：错误地用基因组级二核苷酸 odds ratio 冒充位点特异密码子签名；正确应为基于基因集合的 ρXY(1,2)/ρYZ(2,3)/ρXZ(1,3)/ρZW(3,4)（密码子位点 1-2、2-3、1-3 及密码子间接合处 3-4）
- CAI（`RelativeCodonBiases`）：以单个 ORF 自身作参照集（CAI 恒≈1），并用 CodonBiasVector 欧氏范数冒充密码子频率；正确应为：以高表达基因集 H 统计 f^H(codon)，w(codon)=f^H(codon)/max_同义 f^H(·)，CAI(gene)=(∏wᵢ)^(1/L) 几何平均，两步式 API（先构建 w 表、再对目标基因求值）
- `DifferenceMeasurement.Sigma`：5 个重复/冲突重载需清理合并
- `SimilarDiscriptions` 分级逻辑正确，仅需按论文措辞调整命名

### 2. 性能优化（面向大规模基因组数据）

- 消除 `NucleicAcid` 缓存类为每个相邻位置分配 `SlideWindow(Of DNA)` 对象的巨大开销（5Mb 基因组约 500 万对象），改为单趟扫描的整数计数矩阵（16 维二核苷酸计数 + 4 维单碱基计数），按需计算 ρ\*
- `biasTable` 字符串键字典查找改为数组索引
- 修复 `ToolsAPI.GenomeSigmaDifference_p` 中比较序列被 O(n) 次重复构造的问题
- 滑窗 δ\* 曲线采用增量更新（窗口滑动时仅更新进出各一个二核苷酸）
- 分析并选择性应用基础库 SIMD 模块（`Microsoft.VisualBasic.Core\src\Math\SIMD`）：热点为整数单趟词频统计（标量循环已足够），SIMD 适用于 16 维 ρ\*/δ\* 向量的 |a−b| 求和与 (C−G)/(C+G) 剖面计算；结论需写入代码注释/文档
- 窗口级计算并行化（PLINQ/Partitioner）

### 3. 补全缺失算法（全部 9 个模块）

1. ρ\* 基因组签名（双链对称化 + 六级显著性分级）
2. δ\* 距离（16 维签名向量平均绝对差，×1000 六级相似度标尺）
3. 密码子签名（位点特异 ρXY(1,2)/ρYZ(2,3)/ρXZ(1,3)/ρZW(3,4)）
4. τ\* 四核苷酸相对丰度（低阶子词校正的广义 odds ratio，限制位点回避分析）
5. r-scan 词空间分布检验（对接基础库 `Math.Statistics\RScan` 模块：RScanCore.Scan/GapsOf/PLeftTail/PRightTail，检测成簇/过度分散/等间距）
6. CAI 与 B(F|C) 类间密码子偏差距（氨基酸频率加权的逐密码子绝对差，非对称）
7. 外来基因二维阈值判别（横轴 B(g|RP)、纵轴 B(g|all)；B(g|all)>0.42 且 B(g|RP)>0.45 → alien；B(g|all)>0.42 但 B(g|RP)<0.45 → 高表达基因）
8. 滑动窗口 δ\* 曲线（50kb 窗口签名 vs 全局签名，定位致病岛/水平转移区）
9. (C−G)/(C+G) 链偏差滑动曲线（推断复制起点 oriC 与复制方式）

### 4. 按论文概念彻底重命名 + 同步更新调用方

- `Sigma`/`DifferenceMeasurement` → `DeltaStar`/δ\* 相关命名
- `DinucleotideBIAS` → 基因组签名 ρ\* 命名
- `GenomeSigmaDifference_p` → 滑窗 δ\* 剖面命名
- `SimilarDiscriptions` → δ\* 分级命名
- 同步更新仓库内全部调用方：`SequenceTools/CLI/DNA_Comparative.vb`、`workbench/R#/seqtoolkit/PlasmidComparative.vb`、`workbench/R#/comparative_toolkit/SigmaDifference.vb`、`interops/visualize/Circos/.../DeltaDiff.vb`、`interops/meme_suite/MEME/.../{MotifDeltaSimilarity.vb, MotifScans.vb, TestAPI.vb}`、`analysis/Motifs/MotifGraph/{SequenceGraph.vb, Builder.vb}`

### 交付验证

- `dotnet build` 编译 `DNA_Comparative.netcoreapp.vbproj` 通过
- 受影响调用方项目同步编译通过
- 用简单测试序列验证 ρ\* 对称性、δ\* 自比较为 0、CAI 数值合理等基本正确性

## Tech Stack

- 语言/运行时：VB.NET（.NET SDK 项目，net10.0，VB 源码，不可更改语言）
- 核心依赖（现有）：`Core.vbproj`（sciBASIC# 基础库，含 `Math.SIMD`、`SlideWindow`、`<Package>`/`<ExportAPI>` CLI 反射框架）、`Math.NET5.vbproj`、`biocore-netcore5.vbproj`（生物序列模型 `NucleotideModels`、`FASTA`、`GenBank`）
- 新增依赖：`stats-netcore5.vbproj`（`Math.Statistics\RScan`，r-scan 检验）
- 序列底层表示：`SMRUCC.genomics.SequenceModel.NucleotideModels`（`DNA` 枚举、`NucleicAcid`、`Codon`、`TranslTable`）

## Implementation Approach

### 架构策略

采用"分层重构 + 兼容性重定向"：底层 `NucleicAcid` 缓存类整体重写为基于整数计数矩阵的轻量签名对象（一次性构建、O(n) 单趟扫描、零堆分配热点），其上以论文符号体系（ρ\*、δ\*、τ\*、B(F|C)、CAI）重新组织算法模块；现有 `<Package>`/`<ExportAPI>` CLI 反射元数据全部保留（仅改函数名与 API 名，保持 R# 脚本层可用）。

### 关键技术决策

1. **签名计数矩阵取代 SlideWindow 对象**：以 2bit 编码（A/T/G/C→0..3）单趟扫描序列，得到 4×4 整数矩阵（二核苷酸计数）与 4 元数组（单碱基计数），对称化版本在拼接序列（正向 + 反向互补）上同一趟完成。复杂度 O(n) 时间、O(1) 额外空间，替代现有 O(n) 对象分配，是最大性能瓶颈的根治点。
2. **保持 `NucleicAcid` 类型名与构造签名不变**：`ToolsAPI`、`IdentityResult`、外部调用方均依赖 `New NucleicAcid(fasta)`；内部实现全部替换，公共契约不变，控制爆炸半径。
3. **密码子签名按基因集合计算**：新实现接受 CDS 基因集合（FastaFile 或 GenBank 提取），按位点 (1,2)/(2,3)/(1,3)/(3,4) 分别统计 fXY(i,j) 与边缘频率 fX(i)，输出 16 维 ×4 组的签名向量；与现有 `CodonBiasVector` 结构对接但重写语义。
4. **CAI 两步式**：`CodonAdaptationIndex` 参照集 H（FastaFile 高表达基因集）构建 w 表（61 个有义密码子权重，XML 可序列化沿用现有 `CAI/XML` 持久化模式），目标基因按翻译表逐密码子取 w 求几何平均。保留 `GeneticCodes` 密码表参数。
5. **r-scan 直接复用基础库**：`RScanCore.Scan`（RScanOptions/RScanResult）提供左尾聚集/右尾分散检验；本项目新增薄封装层负责"词在基因组中的位点扫描 → Positions 数组 → RScan 调用 → 结果输出"，不在本项目重复实现统计核心。
6. **滑窗 δ\* 剖面增量更新**：固定步长滑窗（默认 50kb，论文口径），窗口内 16 维计数矩阵增量维护（右进左出各一个二核苷酸），每窗 O(1) 更新 + O(16) 距离计算，替代现有每窗 O(winSize) 重建。
7. **SIMD 适用性结论**（写入文档与代码注释）：热点为整数词频统计，SIMD（Double/Single 向量）不适用于计数主循环；仅对 16 维 |ρf−ρg| 求和、(C−G)/(C+G) 剖面等少数 Double 向量运算选择性应用 `Microsoft.VisualBasic.Math.SIMD`（同一 Core.vbproj 内，无新依赖成本），并通过 `SIMDEnvironment.IsEnabled` 做运行时回退到标量路径。
8. **性能基线**：单基因组 5Mb 签名计算应从现有"每碱基 1 个对象 + 字典查找"降至单趟数组扫描（预计 1–2 个数量级提升）；两两矩阵（IdentityResult）复用预构建签名对象，消除重复计数。

## Implementation Notes

- 消除 `DifferenceMeasurement` 中 5 个重复重载：保留 `NucleicAcid` 缓存版本为主路径，字符串/FastaSeq 重载委托之
- `biasTable` 改为 `Double()(4,4)` 索引访问；`GetValue(X,Y)` 签名保留为兼容包装
- 随机序列（含 N 等简并碱基）需在计数时跳过或归入兜底桶，防止除零（现有代码无此防护）
- `GenomeSigmaDifference_p` 修复：比较序列签名在循环外构建一次
- 日志沿用现有 `.debug`/`Console.WriteLine` 模式，进度输出保留
- 重命名通过 LSP 全局符号查找定位引用，逐一更新；`<ExportAPI("...")>` 字符串名同步调整（外部 R# 脚本口径）
- 构建：`dotnet build DNA_Comparative.netcoreapp.vbproj -c Debug`；再编译 CLI/workbench 受影响项目

## Architecture Design

```mermaid
graph LR
    subgraph 基础库
        CORE[Core.vbproj<br/>SIMD / SlideWindow / CLI框架]
        BIO[biocore<br/>FASTA / GenBank / NucleotideModels]
        MATH[Math.NET5]
        STAT[Math.Statistics<br/>RScanCore r-scan]
    end
    subgraph DNA_Comparative 重构后
        SIG[NucleicAcid 签名缓存<br/>整数计数矩阵+ρ*]
        GS[GenomeSignatures<br/>ρ*对称化+分级]
        DD[DeltaStarDistance<br/>δ* + 六级标尺]
        CS[CodonSignature<br/>位点特异签名]
        TB[TetranucleotideBias τ*]
        CAI[CAI / B(F|C)<br/>两步式]
        AG[AlienGeneDetection<br/>二维阈值]
        SW[SlidingWindowDelta<br/>增量δ*曲线]
        RA[ReplicationAsymmetry<br/>(C−G)/(C+G)]
        RS[RScanWordDistribution<br/>r-scan封装]
    end
    CORE --> SIG
    BIO --> SIG
    STAT --> RS
    SIG --> GS --> DD
    SIG --> TB
    SIG --> SW --> DD
    CS --> CAI --> AG
    SIG --> RA
    RS
```

## Directory Structure

```
DNA_Comparative/
├── DNA_Comparative.netcoreapp.vbproj      # [MODIFY] 新增 stats-netcore5.vbproj 项目引用（RScan）
├── DeltaSimilarity1998/
│   ├── NucleicAcid.vb                     # [MODIFY] 重写为整数计数矩阵签名缓存：单趟扫描、(4,4)计数矩阵、
│   │                                      #   按需ρ*计算、保留类型名与构造签名；正反链对称化计数
│   ├── GenomeSignatures.vb                # [MODIFY] ρ*双链对称化odds ratio + 六级显著性分级(−−−/−−/−/+/++/+++)；
│   │                                      #   移除错误CodonSignature（迁至新文件）；清理BIAS/BIAS_p冗余
│   ├── DeltaStarDistance.vb               # [MODIFY-改名自DifferenceMeasurement.vb] δ*=(1/16)Σ|ρ*f−ρ*g|，
│   │                                      #   合并5个重复重载；SimilarDescription保留（×1000标尺）
│   ├── SimilarDiscriptions.vb             # [MODIFY] 按论文措辞调整枚举命名为DeltaStarLevels语义
│   ├── CodonSignature.vb                  # [NEW] 位点特异密码子签名：ρXY(1,2)/ρYZ(2,3)/ρXZ(1,3)/ρZW(3,4)，
│   │                                      #   基于基因集合的位点频率统计；CodonBiasVector重写语义对接
│   ├── TetranucleotideBias.vb             # [NEW] τ*四核苷酸广义odds ratio（低阶子词校正分子分母），
│   │                                      #   限制位点回避分析（如CTAG稀有度）
│   ├── RScanWordDistribution.vb           # [NEW] 词位点扫描→基础库RScanCore.Scan封装：成簇/过度分散/等间距检验
│   ├── CodonBiasMeasure.vb                # [NEW] B(F|C)=Σₐpₐ(F)·Σ|f−c|氨基酸加权偏差距（非对称）
│   ├── AlienGeneDetection.vb              # [NEW] 二维阈值法：B(g|all)>0.42且B(g|RP)>0.45→alien；
│   │                                      #   B(g|all)>0.42但B(g|RP)<0.45→高表达基因
│   ├── SlidingWindowDelta.vb              # [NEW] 50kb滑窗δ*曲线（增量计数更新），定位水平转移区/致病岛
│   ├── ReplicationAsymmetry.vb            # [NEW] (C−G)/(C+G)滑窗曲线，推断复制起点oriC与复制方式
│   ├── CAI/
│   │   ├── RelativeCodonBiases.vb         # [MODIFY] 重写为两步式：参照集H构建w表（w=f^H/max同义），
│   │   │                                  #   CAI(gene)=(∏w)^(1/L)；移除欧氏范数假频率逻辑
│   │   └── XML/CodonAdaptationIndex.vb    # [MODIFY] w表XML持久化结构适配（沿用现有GetXml模式）
│   ├── ReferenceRule.vb                   # [MODIFY] 保留dnaA-gyrB外标尺（少量命名对齐）
│   ├── ToolsAPI/ToolsAPI.vb               # [MODIFY] 修复GenomeSigmaDifference_p比较序列O(n)重复构造；
│   │                                      #   函数与ExportAPI名按论文概念重命名
│   └── IdentityResult.vb                  # [MODIFY] SigmaMatrix→DeltaStarMatrix，复用预构建签名对象
└── GCOutlier.vb                           # [MODIFY] 少量命名对齐（GCSkew/(C−G)/(C+G)口径统一）

受影响调用方（同步更新）：
├── GCModeller/analysis/SequenceTools/CLI/DNA_Comparative.vb
├── workbench/R#/seqtoolkit/PlasmidComparative.vb
├── workbench/R#/comparative_toolkit/SigmaDifference.vb
├── interops/visualize/Circos/Circos.Extensions/data/DeltaDiff.vb
├── interops/meme_suite/MEME/Analysis/Similarity/MotifDeltaSimilarity.vb
├── interops/meme_suite/MEME/Analysis/MotifScanning/MotifScan/{MotifScans.vb, TestAPI.vb}
└── GCModeller/analysis/Motifs/MotifGraph/{SequenceGraph.vb, Builder.vb}
```

## Key Code Structures

```
''' <summary>重构后的签名缓存核心（示意）：单趟整数计数矩阵，替代 SlideWindow 对象数组</summary>
Public Class NucleicAcid
    ''' <summary>对称化二核苷酸计数 (4,4)：序列+反向互补拼接后单趟扫描所得</summary>
    Friend ReadOnly SymmetricDimerCounts As Integer(,)
    ''' <summary>对称化单碱基计数(4)</summary>
    Friend ReadOnly SymmetricMonomerCounts As Integer()
    ''' <summary>ρ*XY = fXY / (fX·fY)，对称化（论文 ρ*）</summary>
    Public Function RelativeAbundance(X As DNA, Y As DNA) As Double
    ''' <summary>六级显著性符号：−−−/−−/−/+/++/+++（0.50/0.70/0.78/1.23/1.30/1.50）</summary>
    Public Function SignificanceSymbol(X As DNA, Y As DNA) As String
End Class

''' <summary>δ* 距离（示意）</summary>
Public Module DeltaStarDistance
    ''' <summary>δ*(f,g) = (1/16)Σ|ρ*XY(f) − ρ*XY(g)|</summary>
    Public Function DeltaStar(f As NucleicAcid, g As NucleicAcid) As Double
End Module

''' <summary>CAI 两步式（示意）</summary>
Public Class CodonAdaptationIndex
    ''' <summary>以高表达基因集 H 构建权重表 w(codon)=f^H(codon)/max_同义 f^H(·)</summary>
    Public Sub New(referenceGenes As FastaFile, Optional code As GeneticCodes = GeneticCodes.StandardCode)
    ''' <summary>CAI(gene) = (∏ wᵢ)^(1/L) 几何平均</summary>
    Public Function Evaluate(gene As FastaSeq) As Double
End Class
```

## Agent Extensions

### Skill

- **lsp-code-analysis**
- Purpose: 在彻底重命名阶段，通过符号查找/引用检索定位 `DinucleotideBIAS`、`Sigma`、`DifferenceMeasurement`、`SimilarDiscriptions`、`GenomeSigmaDifference_p`、`NucleicAcid` 等公共符号在全仓库的所有引用点，确保 8 个调用方文件无遗漏更新
- Expected outcome: 完整的重命名影响清单，重构后无残留旧符号引用、编译零错误

### SubAgent

- **code-explorer**
- Purpose: 在执行前快速核对调用方文件中旧 API 的具体用法（如 `SigmaMatrix`、`DinucleotideBIAS` 调用形态），以及 `Codon`/`TranslTable`/`SlideWindow` 等基础库 API 的准确签名，避免重写时误用
- Expected outcome: 调用方用法清单与基础库 API 签名备忘，供重写代码时精确对接