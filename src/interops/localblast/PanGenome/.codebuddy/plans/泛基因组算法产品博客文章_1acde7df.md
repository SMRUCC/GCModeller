---
name: 泛基因组算法产品博客文章
overview: 基于 PanGenome.vbproj 模块的 VB.NET 源码、entropy.md/sv_entropy.md 原理文档以及 cdhit_family.R 示例脚本，撰写一篇中文博客文章，系统介绍泛基因组分析的需求背景、常规流程、本模块的分析方法原理、统计计算原理、R# 编程使用方法，并穿插讲解分析流程产出的各类 csv 表格内容。文章需多使用生动的比喻。
todos:
  - id: explore-table-models
    content: 使用 [subagent:code-explorer] 探索 Output/PanGenomeResult.vb、Output/Table/、Models/UnionFind.vb、SV熵实现及 pangenome.vb 导出函数，提取13类csv的准确列名与统计计算细节
    status: completed
  - id: write-background-pipeline
    content: 撰写文章第1-2段：泛基因组分析需求背景与微生物群落泛基因组分析常规流程（比喻开篇）
    status: completed
    dependencies:
      - explore-table-models
  - id: write-algorithm-principles
    content: 撰写第3段：结合 GenomeAnalyzer.vb 源码讲解模块分析方法原理（并查集、PAV四分类、共线性、SV检测、蒙特卡洛曲线、遗传距离）
    status: completed
    dependencies:
      - explore-table-models
  - id: write-statistics-principles
    content: 撰写第4段：结合 entropy.md、sv_entropy.md 与源码讲解统计计算原理（PAV熵两种口径、SV双熵、KMeans四象限、PCA、类别占比）
    status: completed
    dependencies:
      - explore-table-models
  - id: write-rsharp-tutorial-csv
    content: 撰写第5-6段：基于 cdhit_family.R 的 R# 编程教程，穿插13类csv表格的列名与生物学含义说明
    status: completed
    dependencies:
      - write-algorithm-principles
      - write-statistics-principles
  - id: assemble-and-review
    content: 整合全文为完整 Markdown 文件保存至工作区，并校对段落衔接、比喻风格与csv列名准确性
    status: completed
    dependencies:
      - write-background-pipeline
      - write-rsharp-tutorial-csv
---

## 产品概述

为 GCModeller 泛基因组分析模块（PanGenome.vbproj）撰写一篇简体中文算法产品博客文章，面向生物信息学用户，风格生动（大量使用比喻），以 Markdown 文件形式保存在工作区内，不修改任何现有源代码。

## 核心内容（文章段落结构）

1. **需求背景**：泛基因组分析在微生物群落分析中的需求背景（为什么单一个参考基因组不够，泛基因组=连锁品牌全菜单的比喻）。
2. **常规流程**：针对微生物群落进行泛基因组分析的常规流程（蛋白序列获取 → 基因家族聚类 → PAV矩阵 → 分类统计 → 进化分析）。
3. **分析方法原理**：结合 GenomeAnalyzer.vb 源码讲解模块的分析方法原理（并查集家族合并、整数索引化、PAV矩阵构建、核心/软核心/壳/云基因四分类、共线性区块、SV结构变异检测、蒙特卡洛泛基因组曲线、Jaccard/单拷贝直系同源遗传距离）。
4. **统计计算原理**：结合 entropy.md、sv_entropy.md 与源码讲解统计计算原理（PAV香农信息熵两种口径、SV双熵 CopyNumber熵/Median熵、KMeans四象限聚类、PCA散点、基因家族类别占比两种口径）。
5. **R#使用教程**：参考 G:\GCModeller\src\workbench\pkg\test\pangenome\cdhit_family.R 脚本，逐步讲解如何通过 R#（调用 G:\GCModeller\src\workbench\R#\comparative_toolkit\pangenome.vb 暴露的 API）完成完整分析流程：read.fasta → cdhit_clusters → family_groups → multiple_genome_alignment → read_genetable → build_context → analysis → writeBin/readBin → 各类导出函数 → report_html。
6. **CSV表格内容说明**：在文章中穿插说明整个 R# 流程产生的 13 类 csv 表格（genome_stats.csv、pav_pca.csv、pav_entropy.csv、genetic_distance.csv、pav_matrix.csv、pangenome_curve.csv、sv_table.csv、pav_table.csv、sv_copy_number_matrix.csv、sv_median_matrix.csv、category_percent_gene.csv、category_percent_family.csv、sv_entropy.csv）的列名与生物学含义，需结合 Output/Table/、PanGenomeResult.vb 及 pangenome.vb 中 stats_frame/pca_frame/entropy_frame/sv_entropy_frame/sv_table/pav_table 等转换函数给出准确列名。

## 技术方案

- **交付物**：一篇 Markdown 格式博客文章，保存为 `g:/GCModeller/src/interops/localblast/PanGenome/泛基因组分析模块算法产品博客.md`（工作区内新文件，不改动任何现有代码）。
- **信息来源（全部已验证）**：
- `GenomeAnalyzer.vb`：分析主流程 8 个步骤（索引化 → 家族合并 → PAV+分类 → 遗传距离 → 共线性 → SV检测 → 泛基因组曲线 → 统计汇总+SV熵聚类），阈值参数（CoreThreshold=1.0、SoftCoreThreshold=0.95、ShellThreshold=0.15、CNV_Gain_Factor=2.0、CNV_Loss_Factor=0.5、MinCollinearGenes=5、CurveIterations=100、SVClusterCount=4）。
- `entropy.md` / `sv_entropy.md`：信息熵两种口径与 SV 双熵四象限解读。
- `cdhit_family.R`：R# 端完整调用脚本与 13 类 csv 导出。
- `pangenome.vb`：R# API 层 23 个公开函数签名。
- **写作规范**：简体中文；多用生动比喻（核心基因=招牌必点菜、云基因=季节限定隐藏菜单、并查集=朋友圈合并、位图Jaccard=点名单对照、蒙特卡洛=洗牌抽牌、熵=信息量的"惊讶程度"等）；代码片段引用关键源码并加以讲解；csv 说明以表格形式呈现列名/类型/含义。

## Agent Extensions

### SubAgent

- **code-explorer**
- Purpose：深入探索尚未读取的关键源码文件，提取 csv 表格的准确列名与数据模型细节，包括 `Output/PanGenomeResult.vb`、`Output/Table/` 下的 SVTable/PAVTable/GenomeStatRow 等类型定义、`Models/UnionFind.vb`、SV 熵实现（SVDomainEntropy，含分箱/Z-score/聚类细节），以及 `G:/GCModeller/src/workbench/R#/comparative_toolkit/pangenome.vb` 中各导出函数（stats_frame、pav_table、sv_table、category_percent_matrix、sv_entropy 等）的 dataframe 列结构。
- Expected outcome：获得文章第4、6段所需的精确列名清单与计算细节，确保 csv 表格说明与代码实现完全一致，不出现臆造列名。