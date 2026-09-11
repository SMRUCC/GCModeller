---
name: gcmodeller-nuget-metadata-update
overview: 为 GCModeller 仓库中 129 个 RootNamespace 以 SMRUCC.genomics 开头的 vbproj 项目，基于其代码内容生成并写入统一的 NuGet 包元数据（Title/Tags/Description/ReleaseNotes 及图标、URL、License、Copyright、Authors、Company、Product）。
todos:
  - id: build-mapping
    content: 按 RootNamespace 前缀枚举 129 个 vbproj，生成项目清单与 assets/logo.png 相对路径的基础映射表 nuget-metadata.json
    status: completed
  - id: metadata-core
    content: 用 [subagent:code-explorer] 和 [skill:lsp-code-analysis] 分析 core/analysis/annotations 组源码，生成英文 Title/Description/Tags/ReleaseNotes 并写入映射表
    status: completed
    dependencies:
      - build-mapping
  - id: metadata-models
    content: 用 [subagent:code-explorer] 和 [skill:lsp-code-analysis] 分析 models/engine/visualize/data/sub-system/foundation 组源码，补齐英文描述化元数据
    status: completed
    dependencies:
      - build-mapping
  - id: metadata-interops
    content: 用 [subagent:code-explorer] 分析 interops/repository/workbench/runtime/R-sharp 组源码，补齐英文描述化元数据
    status: completed
    dependencies:
      - build-mapping
  - id: build-updater
    content: 编写 update-nuget-metadata.ps1 幂等脚本：统一 URL/Icon/License/Copyright/Authors/Company/Product 并按原编码换行最小侵入写回
    status: completed
    dependencies:
      - build-mapping
  - id: apply-metadata
    content: 在样板项目 dry-run 通过后，批量写回全部 129 个 vbproj 的统一字段与描述性元数据
    status: completed
    dependencies:
      - metadata-core
      - metadata-models
      - metadata-interops
      - build-updater
  - id: verify-all
    content: 运行 verify-nuget-metadata.ps1 校验 XML 良构、字段覆盖 129/129、去重与 PackageLicenseFile 清理，并抽查 diff 修复残留
    status: completed
    dependencies:
      - apply-metadata
---

## 需求概述

扫描仓库内全部 `.vbproj` 项目文件，筛选出 `<RootNamespace>` 以 `SMRUCC.genomics` 起始的项目（共 129 个），依据每个项目源码文件的内容，批量更新其 NuGet 包元数据，使所有 SMRUCC.genomics 程序包的发布信息风格统一、描述准确。

## 核心功能

- 按 RootNamespace 前缀筛选目标项目并逐一处理
- 依据项目源码内容总结生成英文 Title / Description / PackageTags / PackageReleaseNotes
- 统一 PackageIcon 为 assets/logo.png（按各项目深度自动计算相对路径引用，保留 logo.png 打包文件名）
- 统一 PackageProjectUrl 为 https://gcmodeller.org/
- 统一 RepositoryUrl 为 https://github.com/SMRUCC/GCModeller.git
- 统一 License 为标准 SPDX 标识 GPL-3.0-or-later（PackageLicenseExpression）
- 统一 Copyright 为 Copyright © SMRUCC genomics, GuiLin China, 2026
- 统一 Authors 为 xieguigang&lt;xie.guigang@gcmodeller.org&gt;
- 统一 Company 为 SMRUCC genomics institute，Product 为 GCModeller

## 范围与约束

- 覆盖全部 129 个项目（含未启用 GeneratePackageOnBuild 的项目）
- 描述性文本一律使用英文，与既有 biocore/GSVA 等项目风格一致
- 仅修改 vbproj 文件，不改动源码、csproj 与其他构建产物
- 保持各文件原有编码（UTF-8/BOM）与换行风格，项目结构不被破坏

## 技术栈

- 脚本执行环境：Windows PowerShell 5.1 + .NET（System.IO / System.Xml.XDocument / ConvertFrom-Json），本工作区为 win32/PowerShell，原生可用
- 目标对象：129 个 SDK-style VB.NET `.vbproj`（MSBuild 属性与 ItemGroup）
- 数据交换：JSON 映射表（项目路径 → 描述性元数据 + logo 相对路径）
- 分析辅助：`[subagent:code-explorer]`、`[skill:lsp-code-analysis]`

## 实现思路

1. 规则化字段（PackageProjectUrl / RepositoryUrl / PackageLicenseExpression / Copyright / Authors / Company / Product / PackageIcon）完全确定，由一个幂等脚本统一写入，避免 129 次手工编辑导致不一致。
2. 描述性字段（Title / Description / PackageTags / PackageReleaseNotes）需要语义理解：先采集每个项目的代表性源码（排除 `obj/`、`bin/`、`test/`、`My Project/*.Designer.vb`），由 LLM 生成英文文本，写入 JSON 映射表。
3. 脚本读取映射表，对每个 vbproj 做最小侵入的文本级更新：在第一个无条件 PropertyGroup 内插入/更新属性；替换或新增 logo 的 None Include；移除 PackageLicenseFile。

## 关键决策

- 编码安全：用 `[System.IO.File]::ReadAllBytes` 检测 UTF-8 BOM，按原编码与原换行写回；不使用 Get-Content/Set-Content 以避免编码丢失（此前已被环境安全策略拦截）。
- 文本级编辑而非 XML 序列化：避免 XML 往返重排使 63KB 级别的文件（如 biocore-netcore5.vbproj）产生海量无关 diff。
- 幂等性：属性存在则替换值、不存在则插入；logo 的 None Include 先查重再改；重复执行结果稳定。
- 相对路径计算：`..\` × depth + `assets\logo.png`，depth 为 vbproj 相对仓库根 `G:\GCModeller` 的目录层数（现有 images\logo.png 引用深度为 4~6，可作对照校验）。
- License：写入 `PackageLicenseExpression=GPL-3.0-or-later` 并删除 PackageLicenseFile；保留 PackageRequireLicenseAcceptance。

## 执行细节（防回归）

- Authors 在 XML 中需转义为 `xieguigang&lt;xie.guigang@gcmodeller.org&gt;`；`©` 为非 ASCII，保持 UTF-8。
- 统一属性插入到第一个无条件 PropertyGroup 内，避免与条件 PropertyGroup（按 Configuration/Platform）冲突。
- logo ItemGroup：若已存在 `<None Include="...logo.png">` 则原地替换 Include 路径（保留 Pack/PackagePath 元数据）；不存在则在 `</Project>` 之前新增独立 ItemGroup；确保不出现重复 logo 条目。
- 已存在 `PackageLicenseFile` 的项目需删除该属性；残留的 LICENSE None 打包项保持不动以缩小改动面。
- 批量应用前先在少量样板（biocore、BNLearn、GSVA）上 dry-run 校验输出，再全量执行。

## 校验方案

- 用 `[xml]` 解析全部 129 个文件，确认 XML 良构。
- 断言每个文件都包含全部统一字段且取值正确；统计 Title/Description/PackageTags/PackageReleaseNotes 覆盖率应为 129/129。
- 断言无重复 logo None 条目、无残留 PackageLicenseFile。
- 抽查样板项目 diff，人工确认换行/编码未变化。

## 目录结构

本次为批量元数据维护，新增少量可复现脚本，并修改 129 个既有项目文件。

```
G:/GCModeller/
├── msbuild/
│   └── nuget_metadata/                              # [NEW] 元数据维护工具目录
│       ├── nuget-metadata.json                      # [NEW] 项目→元数据映射表（含 logo 相对路径、Title/Description/Tags/ReleaseNotes）
│       ├── update-nuget-metadata.ps1                # [NEW] 幂等写回脚本：统一字段 + 描述字段，按原编码/换行最小侵入更新
│       └── verify-nuget-metadata.ps1                # [NEW] 校验脚本：XML 良构、字段覆盖、去重、License 清理检查
├── src/GCModeller/
│   ├── core/Bio.Assembly/biocore-netcore5.vbproj                # [MODIFY] 统一字段 + 刷新 Title/Description/Tags/ReleaseNotes、logo 改 assets
│   ├── core/Bio.Annotation/annotation.NET5.vbproj               # [MODIFY] 同上
│   ├── core/Bio.InteractionModel/InteractionModel.NET5.vbproj   # [MODIFY] 同上
│   ├── analysis/**/*.vbproj                                     # [MODIFY] HTS_matrix、Microarray、OperonMapper、Motifs、SequenceToolkit、Metagenome 等
│   ├── annotations/**/*.vbproj                                  # [MODIFY] GO、KEGG、Proteomics、GSEA(GSEA/GSVA/PFSNet/FELLA/Fisher)、WGCNA、P-NET
│   ├── models/**/*.vbproj                                       # [MODIFY] SBML、BioCyc、Networks(STRING/KEGG/Microbiome/Regulons/BLAST)、GPML、BIOM、RouterAdapter
│   ├── engine/**/*.vbproj                                       # [MODIFY] Model.Core、Dynamics、Compiler(Services)、BootstrapLoader、GCModeller.ModellingEngine、vcell(kit)
│   ├── visualize/**/*.vbproj                                    # [MODIFY] DataVisualization(Extensions/Tools)、ChromosomeMap、SyntenyVisual
│   ├── data/**/*.vbproj                                         # [MODIFY] KEGG、Reactome、Rhea、SABIO-RK、STRING、uniref、Xfam、RegulonDatabase、ExternalDBSource、BASys、RCSB PDB
│   ├── sub-system/**/*.vbproj                                   # [MODIFY] FBA、BNLearn、CellPhenotype、GEARS、Metaboliq、PLAS.NET
│   ├── foundation/**/*.vbproj                                   # [MODIFY] OBO_Foundry、PSICQUIC
│   ├── CLI_tools/**/*.vbproj                                    # [MODIFY] 各命令行工具项目
│   └── ...
├── src/interops/**/*.vbproj                                     # [MODIFY] localblast、meme_suite、RNA-Seq、visualize(Circos/Cytoscape/Phylip)
├── src/repository/**/*.vbproj                                  # [MODIFY] Bio.Repository、HMP_client、Model_Repository、nt、ncbi_datasets
├── src/workbench/**/*.vbproj                                   # [MODIFY] modules(SeqFeature/Knowledge_base/keggReport/ExperimentDesigner)、R#工具
├── src/runtime/**/*.vbproj                                     # [MODIFY] 少量 RootNamespace 为 SMRUCC.genomics 的项目
└── src/R-sharp/**/*.vbproj                                     # [MODIFY] 少量 RootNamespace 为 SMRUCC.genomics 的项目
```

注：`src/**` 下待修改文件共 129 个，完整清单由 `update-nuget-metadata.ps1` 在运行时按 `RootNamespace` 前缀动态枚举并输出，确保与筛选条件严格一致。

## Agent Extensions

### SubAgent

- **code-explorer**
- Purpose: 批量扫描 129 个目标项目目录下代表性 `.vb` 源码（排除 obj/bin/test），收集关键类型、模块与 XML 注释签名，作为生成英文 Title/Description/Tags/ReleaseNotes 的事实依据。
- Expected outcome: 按目录分组输出每个项目的功能要点摘要，支撑映射表内容生成，避免凭空编造描述。

### Skill

- **lsp-code-analysis**
- Purpose: 对代表性源文件提取代码大纲与公共符号（file outline / symbol lookup / code structure），补强核心与大型项目（如 Bio.Assembly、ModellingEngine、vcell）的描述准确性。
- Expected outcome: 关键项目公共 API 大纲，用于校准 Title 与 Description 的术语与范围。