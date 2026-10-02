---
name: seqtoolkit-xml-docs
overview: 为 seqtoolkit R# 包项目中所有“公开可访问”的 R# 导出 API（带 &lt;ExportAPI&gt;/&lt;RGenericOverloads&gt;/&lt;RTypeExport&gt; 等特性的 Public 函数、&lt;Package&gt; 模块汇总注释，以及 models/ 中导出给 R# 的数据类型）补全 .NET XML 注释（'''）。既补齐缺失的注释，也补全已有但为空/过于简略的 &lt;param&gt;/&lt;returns&gt; 描述，使整包文档质量一致。
todos:
  - id: doc-hmmer-primers
    content: 补全 hmmer.vb 与 primers.vb 的模块 summary 及全部导出函数（load_interprodb、parse_hmmer_model、load_hmmer、hmmer_search、find_primers_region）的 XML 注释
    status: completed
  - id: doc-root-gaps
    content: 补全 patterns.vb、proteinKit.vb、Blast.vb 中缺失或空占位的导出 API 注释（含 openSeedFile、pullAllSeeds、top_sites、createSeeds、pdb_centroid、ligands、kmer_fingerprint、enzyme_builder、predict_sequence、readPfamString、gwANIMultipleAlignment 等）
    status: completed
  - id: doc-annotations
    content: 使用 [subagent:code-explorer] 盘点并补全 Annotations/ 下 8 个文件（blastPlus、context、genbankKit、genomics、snp、terms、uniprot、workflows）的全部 R# 导出 API 与模块 summary
    status: completed
  - id: doc-models
    content: 核查并补全 models/ 下 UniProtTable.vb 导出数据类型与 Enums.vb 被 RTypeExport 引用的枚举（TableTypes、BBHAlgorithm）的 XML 注释
    status: completed
  - id: verify-xml
    content: 重新构建项目生成 seqtoolkit.xml，验证文档完整且编译无 CS1591 等缺失注释警告
    status: completed
    dependencies:
      - doc-hmmer-primers
      - doc-root-gaps
      - doc-annotations
      - doc-models
---

## 用户需求

完善 `R#\seqtoolkit\seqtoolkit.vbproj` 项目中每一个“可以公开访问”的函数和模块的 .NET XML 注释文档（VB.NET 三引号 `'''` 注释，最终由 `GenerateDocumentationFile=True` 生成 `seqtoolkit.xml`）。

## 产品概述

`seqtoolkit` 是一个 R# 语言包项目，对外暴露生物学序列分析工具集（FASTA 读写、BLAST/HMMER、k-mer、引物设计、基因预测、蛋白结构等）。本次任务只为这些对 R# 用户公开可调用的 API 补齐并规范化 XML 文档，不涉及任何逻辑代码改动。

## 核心特性

- 仅针对带 `<ExportAPI>`/`<RGenericOverloads>`/`<RTypeExport>` 等特性的 Public 函数，以及 `<Package>` 模块汇总注释；含 `models/` 中导出给 R# 的数据类型（类/枚举/结构体）。
- 既补齐完全缺失 `'''` 注释的函数与模块，也补全已有但为空或仅占位的 `<param>`/`<returns>`/`<summary>` 描述，使整包文档质量一致。
- 不改动任何函数签名、R# 导出特性、逻辑代码；Private/Friend 内部辅助函数不添加文档。
- 遵循现有英文文档风格（summary / param / returns / remarks / example / see 交叉引用），描述面向 R# 用户（输入可接受的数据模型、返回对象、错误返回 `Message` 等）。

## 技术栈

- 语言/框架：VB.NET（目标框架 net10.0），R# 互操作（`SMRUCC.Rsharp.Runtime.Interop` 的 `<ExportAPI>`/`<Package>`/`<RTypeExport>`/`<RGenericOverloads>`）。
- 文档机制：VB.NET `'''` 三引号 XML 文档注释；由 `seqtoolkit.vbproj` 的 `GenerateDocumentationFile=True` 自动生成 `seqtoolkit.xml`。
- 工具辅助：`lsp-code-analysis` 用于跨文件符号核查，`code-explorer` 用于批量盘点尚未通读的 `Annotations/` 与 `models/`。

## 实现方案

采用“按文件扫描 + 模板化补写”策略：对每个源文件，定位所有 `<Package>` 模块声明与带 R# 导出特性的 `Public` 函数，按既有英文文档模板补齐/完善 `'''` 注释块。关键决定如下：

1. **范围严格限定为公开 R# API**：仅 `<ExportAPI>`/`<RGenericOverloads>`/`<RTypeExport>`/`Public` 导出函数与 `<Package>` 模块；`Private`/`Friend` 辅助函数（如 `viewXxx`、`xxxToString`、私有扩展）不加，避免污染 `seqtoolkit.xml` 并符合“公开访问”语义。
2. **复用现有文档风格而非新造**：参照 `Fasta.vb`/`kmersTools.vb`/`bifrost.vb` 的成熟写法（`<summary>` 必填，参数逐项 `<param>`，返回 `<returns>`，复杂函数加 `<remarks>` 与 R# 示例 `<example>`，类型交叉引用 `<see cref="..."/>`）。
3. **空占位也补全**：对 `<param name="file"></param>`、`<returns></returns>` 等已有但为空的标签，结合函数实现与参数含义补写完整描述（如 `file` 描述文件/流对象，`env` 统一为 “the R# runtime environment object.”）。
4. **models/ 枚举按需**：仅当 `Enums.vb` 中的 `TableTypes`/`BBHAlgorithm` 被某处 `<RTypeExport>` 引用为导出数据类型时才补 summary；实施时先确认引用关系，避免无谓改动。

## 实现注意

- **零逻辑改动**：仅插入/完善 `'''` 注释文本，不触碰任何 `Imports`、`Function` 签名、特性或语句，保证重新编译零风险。
- **性能/影响面**：纯注释变更，不影响运行时性能与二进制行为；改动面是分散的多文件 `'''` 文本，需逐文件核对避免重复或错位插入。
- **验证手段**：完成后以 Debug 或 Release 配置重新构建项目，确认 `seqtoolkit.xml` 正常生成且无编译器警告（如 CS1591 缺失公开成员 XML 注释），作为文档完整性的可验证标准。

## 架构设计

本任务不改变任何运行时架构，仅完善静态 XML 文档元数据。R# 包的导出结构保持现有约定：`Module` + `<Package>` 作为包命名空间，`Public Function` + `<ExportAPI("api.name")>` 作为可调函数，`<RTypeExport>` 声明导出数据类型。文档补充严格贴合该层次，不改结构。

## 目录结构与修改清单

```
R#/seqtoolkit/
├── hmmer.vb          # [MODIFY] 补模块 <summary>；load_interprodb / parse_hmmer_model / load_hmmer / hmmer_search 四个均缺 summary，需补全 summary/param/returns
├── primers.vb        # [MODIFY] 补模块 <Package> 的 <summary>；find_primers_region 函数缺 summary，补全 summary/param/returns
├── patterns.vb       # [MODIFY] 补 openSeedFile / pullAllSeeds / top_sites / createSeeds 的 <summary> 与参数说明
├── proteinKit.vb     # [MODIFY] 补 pdb_centroid / ligands / kmer_fingerprint / enzyme_builder / predict_sequence / readPfamString 的 summary/param/returns；并补全已有空占位 param/returns
├── Blast.vb          # [MODIFY] 补 gwANIMultipleAlignment 的 <summary> 与参数说明
├── Fasta.vb          # [MODIFY/核查] 已较完善，仅核查是否有空占位待补
├── kmersTools.vb     # [MODIFY/核查] 已较完善，仅核查空占位
├── bifrost.vb        # [MODIFY/核查] 已较完善，仅核查空占位
├── zzz.vb            # [不改] 入口类，无需文档
├── Annotations/
│   ├── blastPlus.vb     # [MODIFY] 审查并补全全部 R# 导出 API 与模块 <summary>
│   ├── context.vb       # [MODIFY] 同上
│   ├── genbankKit.vb    # [MODIFY] 同上
│   ├── genomics.vb      # [MODIFY] 同上
│   ├── snp.vb           # [MODIFY] 同上
│   ├── terms.vb         # [MODIFY] 同上
│   ├── uniprot.vb       # [MODIFY] 同上
│   └── workflows.vb     # [MODIFY] 同上
└── models/
    ├── UniProtTable.vb  # [MODIFY] 检查并补全导出给 R# 的数据类型/模块注释
    └── Enums.vb        # [MODIFY/按需] 仅当枚举被 <RTypeExport> 引用时补 TableTypes / BBHAlgorithm 的 <summary>
```

## 关键代码结构（注释模板）

为保证一致性，所有新增/补全的公开 API 注释遵循如下最小模板（依参数多少裁剪）：

```
''' <summary>
''' 一句话/一段功能概述（面向 R# 用户，英文）。
''' </summary>
''' <param name="x">参数含义、可接受的数据模型（如 FastaSeq/FastaFile/字符向量）。</param>
''' <param name="env">the R# runtime environment object.</param>
''' <returns>
''' 返回对象类型与含义；若输入非法返回 “a R# error message object”。
''' </returns>
''' <remarks>可选：算法说明、与其他 API 的关系、注意事项。</remarks>
```

## 智能体扩展

### SubAgent

- **code-explorer**
- 用途：在补充 `Annotations/` 与 `models/` 文档前，对这 10 个尚未通读的文件做“very thorough”级盘点，列出所有 `<ExportAPI>`/`<RGenericOverloads>`/`<RTypeExport>`/`<Package>` 导出点及缺失/空占位注释项，避免漏补。
- 预期结果：输出各文件待补注释的函数清单与模块清单，作为按文件补写注释的精确依据。