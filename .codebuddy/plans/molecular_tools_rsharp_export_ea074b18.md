---
name: molecular_tools_rsharp_export
overview: 在 molecular_kit/molecular_tools.vb 中新建 R# 包 `molecular`，把 SMILES/InChI 解析、分子图坐标计算、PDB 导出与 2D/3D/网格结构绘图以 ExportAPI + `plot` 重载的形式导出给 R# 脚本使用。
todos:
  - id: scaffold-package
    content: 用 [subagent:code-explorer] 复核上游 API 后搭建 molecular 包骨架：Package 特性、Imports 别名、Sub Main 注册 plot/dataframe/console 回调
    status: completed
  - id: parse-api
    content: 实现解析 API：parse_smiles、parse_inchi、parse_molecule、parse_molecules、read.molecule、write_smiles、molecule_format
    status: completed
    dependencies:
      - scaffold-package
  - id: query-api
    content: 实现信息查询 API：分子式/分子量/原子表/键表/环/连通分量/告警/摘要，并接入 as.data.frame 与控制台格式化
    status: completed
    dependencies:
      - parse-api
  - id: coord-pdb-api
    content: 实现坐标与 PDB API：layout2d、embed3d、add/remove_hydrogens、molecule_coordinates、as.pdb、write.pdb
    status: completed
    dependencies:
      - query-api
  - id: theme-plot
    content: 实现 chem_theme 与 plot 单分子重载，支持 dim=2/3、mode、主题参数，并保证不改动 ChemTheme.Default
    status: completed
    dependencies:
      - coord-pdb-api
  - id: grid-plot-save
    content: 实现 plot 多分子网格重载（ncol 排布）与 save_png、save_svg 保存接口
    status: completed
    dependencies:
      - theme-plot
  - id: build-verify
    content: 编译 molecular_kit 工程（.NET 10.0）并修复全部编译错误与警告
    status: completed
    dependencies:
      - grid-plot-save
---

## 产品概述

将 MoleculeKit 的 SMILES/InChI 结构式解析、分子图坐标计算与 PDB 导出能力，以及 SMILESView 的分子结构绘图能力，统一导出为 R# 脚本可调用的 `molecular` 包 API，使脚本能够"解析结构式 → 计算坐标 → 导出 PDB → 绘制 2D/3D 结构图"一条链完成。

## 核心功能

- **结构式解析**：支持 SMILES 与 InChI（自动识别），支持字符串、字符串向量与文件/流输入；可回写 SMILES 文本
- **分子信息查询**：分子式、分子量、原子表、化学键表、环信息、连通分量、解析告警
- **坐标计算**：2D 平面布局、3D 立体嵌入、加氢/去氢
- **PDB 导出**：分子图转换为 PDB 结构对象，或直接写为 PDB 文件（残基名、链号、HETATM、是否含氢等可配置）
- **可视化**：`plot` 重载绘制 2D 键线式结构图与 3D 球棍/空间填充模型，可将多个分子按列数排布为网格图；另提供 PNG/SVG 保存接口，并提供可复用的配色主题对象
- **交互体验**：脚本中直接 `print` 分子对象即可看到分子式、原子/键/环数量等概要；`as.data.frame` 可直接得到原子/键表格用于后续分析

## 技术栈

- 语言/平台：VB.NET，.NET 10.0（沿用 `molecular_kit.vbproj` 现有配置）
- 宿主：R# 脚本运行时（`SMRUCC.Rsharp`，`Rsharp-netcore5.vbproj` 已被引用）
- 上游依赖：`SMILES.vbproj`（解析/坐标/PDB 导出）、`SMILESView.vbproj`（2D/3D 渲染）、`imaging.NET5.vbproj`（GDI+ 画布）
- 工程文件不改动：`molecular_kit.vbproj` 已引用上述全部项目，无需新增 ProjectReference/Import

## 实现方案

采用与 `visualkit/visualPlot.vb`、`molecular_kit/pdbTools.vb`、`Rsharp_kit/MLkit/dataMining/clustering.vb` 完全一致的包编写范式：

1. 用 `<Package("molecular", Category:=APICategories.ResearchTools, Publisher:="xie.guigang@gcmodeller.org")>` 修饰 `Public Module molecular_tools`；
2. 在 `Sub Main()` 中做三件注册：

- `RInternal.generic.add("plot", GetType(Mol.MoleculeGraph), AddressOf plotMolecule)`
- `RInternal.generic.add("plot", GetType(Mol.MoleculeGraph()), AddressOf plotMolecules)`
- `RInternal.Object.Converts.makeDataframe.addHandler(GetType(Mol.MoleculeGraph), AddressOf toAtomFrame)` 与 `RInternal.ConsolePrinter.AttachConsoleFormatter(Of Mol.MoleculeGraph)(AddressOf showMolecule)`

3. 其余能力用 `<ExportAPI("...")>` 平铺导出，命名沿用 R# 习惯（`parse_smiles`、`read.molecule`、`molecule_atoms`、`layout2d`、`as.pdb`、`write.pdb`、`save_png`…）；
4. 绘图统一走 `plot` 重载：单一 `MoleculeGraph` 重载用 `dim=2|3` 选择 2D/3D，`mode` 选择球棍/空间填充；`MoleculeGraph()` 重载按 `ncol` 网格排布；
5. 主题参数统一收敛到 `chem_theme(...)` 构造出的 `ChemTheme` 对象，plot/save 的 args 可用 `theme=` 直接传入，避免十几个散落参数重复解析。

关键决策与理由：

- **不重新实现渲染**：直接复用 `SMILESView.Draw2D.MoleculeDraw2D` / `Draw3D.MoleculeDraw3D` / `StructureViewer`，R# 层只做参数解析与画布生命周期管理，避免逻辑分叉；
- **不使用 `InteropArgumentHelper`/`RColorPalette`**：它们在 `R_graphic.interop` 共享工程中，本项目未 Import；改用 `graphicsPipeline.getSize`、`graphicsPipeline.getDpi`、`graphicsPipeline.GetRawColor`、`list.getValue(Of T)`（均位于已引用的 Rsharp 核心）；
- **返回值取 `Microsoft.VisualBasic.Imaging.Image`**：R# 对 `Image`/`Bitmap` 均有内建渲染与 `dim()` 支持（见 `base.vb`、`stringr.vb`、`file.vb` 的类型判断），返回图像比返回自定义包装对象兼容性更好；
- **网格图按"单元格画布 + 整图拼贴"实现**：每个分子渲染到独立小画布再 `DrawImage` 到总画布，内存占用与分子数线性相关，且无需改动 SMILESView 支持视口矩形。

性能与可靠性：

- `MoleculeGraph` 内部对邻接表/环/分子式/Morgan 标签做了惰性缓存，`molecule_atoms`/`molecule_bonds` 均为单次 O(N) 遍历，不引入 N+1；
- 网格绘图在循环内 `Using` 释放单元格画布，避免 GDI+ 句柄泄漏；
- 3D 渲染无需显式相机：`MoleculeScene.Compile` 在无 Z 坐标时自动 `Embed3D()`，`CreateCamera` 按主题的 `CameraAngleX/Y/Z`、`FieldOfView` 取景；
- 所有解析入口对空串、非法结构式、空分子图返回 `Message`（`RInternal.debug.stop` / 捕获 `MoleculeParseException`）而不是让异常穿透脚本。

## Implementation Notes（执行要点）

- **命名冲突**：`SMILESView.RenderMode` 与 sciBASIC `Drawing3D` 内同名枚举、`System.Drawing.Bitmap` 与 `Microsoft.VisualBasic.Imaging.Bitmap` 均需 `Imports` 别名（`ChemRenderMode`、`Bitmap`/`Image` 别名）。
- **命名空间别搞错**：受 `RootNamespace` 影响，全名是 `SMILESView.StructureViewer`、`SMILESView.RenderMode`、`SMILESView.Styling.ChemTheme`（不是全局命名空间）。
- **绝不能改 `ChemTheme.Default`**：它是 `Shared ReadOnly` 单例；由 args 构造主题时必须 `New ChemTheme` 后逐字段赋值（或先 `Clone`），否则会污染后续所有绘图调用。
- **画布释放**：`StructureViewer.Render2D/Render3D` 返回的 `IGraphics` 由调用方释放；`GdiRasterGraphics.ImageResource` 内部为 `GDIPlusImage`（构造时已 `New System.Drawing.Bitmap(img)` 拷贝像素），因此在 `Using` 内取图后再释放画布是安全的。
- **2D 画布背景**：`Render2D` 已按 `theme.BackgroundColor` 填充，无需额外 `Clear`；网格单元格的画布需自行填背景色。
- **网格单元格主题**：单元格内建议用 `Padding` 缩小、`ShowTitle=False` 的副主题，避免标题互相覆盖。
- **文件输入**：`read.molecule` 支持路径与流，统一走 `SMRUCC.Rsharp.GetFileStream(file, FileAccess.Read, env, is_filepath:=byref)`，`is_filepath` 为真时需 `Dispose` 流；读取整段文本后交给 `MoleculeIO.Parse`/`ParseFile`。
- **文档注释**：导出函数需写 `<summary>/<param>/<returns>/<example>`，XML 文档是 R# 帮助系统的数据来源。
- **影响面控制**：只新增 `molecular_tools.vb` 内容，不改动 SMILES/SMILESView 两个上游项目，不改工程文件，不动 `pdbTools`。

## 架构设计

```
R# 脚本
  └─ molecular 包（molecular_tools.vb）
       ├─ 解析层   parse_smiles / parse_inchi / parse_molecule / read.molecule / write_smiles
       ├─ 查询层   molecule_formula / weight / atoms / bonds / rings / components / warnings / summary
       ├─ 坐标层   layout2d / embed3d / add_hydrogens / remove_hydrogens
       ├─ 导出层   as.pdb / write.pdb  →  MoleculeToPdb  →  RCSB PDB
       └─ 绘图层   plot(2D/3D) / plot(网格) / save_png / save_svg / chem_theme
                     ↓
        SMILESView.StructureViewer · MoleculeDraw2D · MoleculeDraw3D · ChemTheme
                     ↓
              IGraphics（GDI+）→ Image（返回脚本）
```

分层内所有"参数解析（size/dpi/colors/theme）"集中在私有帮助函数中，业务函数只做编排，符合 SRP 与 DRY。

## 目录结构

```
src/workbench/R#/molecular_kit/
└── molecular_tools.vb   # [MODIFY] 唯一改动文件（当前为空 Module）
                         # 1) Package 特性 + Imports 别名 + Sub Main() 注册（plot 两个重载、dataframe 转换器、控制台格式化器）
                         # 2) 解析：parse_smiles / parse_inchi / parse_molecule / parse_molecules / read.molecule / write_smiles / molecule_format
                         # 3) 查询：molecule_formula / molecule_weight / molecule_atoms / molecule_bonds / molecule_rings /
                         #    molecule_components / molecule_warnings / molecule_summary；私有 toAtomFrame / toBondFrame / showMolecule
                         # 4) 坐标：layout2d / embed3d / add_hydrogens / remove_hydrogens / molecule_coordinates
                         # 5) PDB：as.pdb / write.pdb；私有 getPdbOptions(args, env)
                         # 6) 绘图：chem_theme / plotMolecule(2D|3D) / plotMolecules(网格) / save_png / save_svg
                         #    私有 getTheme(args, env)（含 copyDefaultTheme）、getRenderMode、renderMolecule(canvas)
                         # 要求：所有导出函数带 XML 文档与 <example>；对空/非法输入返回 Message 而非抛异常
```

## Key Code Structures

```
' plot 重载统一签名（与 RInternal.generic.add 的 GenericFunction 委托一致）
<ExportAPI("plot")>
Public Function plotMolecule(mol As Mol.MoleculeGraph, args As list, env As Environment) As Object

<ExportAPI("plot")>
Public Function plotMolecules(molecules As Mol.MoleculeGraph(), args As list, env As Environment) As Object

' 主题构造：从 args 生成新实例，禁止写入 ChemTheme.Default
Private Function getTheme(args As list, env As Environment) As SMILESView.Styling.ChemTheme
```

## Agent Extensions

### SubAgent

- **code-explorer**
- Purpose: 在动手写代码前，用 "medium" 力度复核 `SMILES`、`SMILESView` 两个工程的公开成员签名（尤其 `StructureViewer` 的重载、`ChemTheme` 字段、`PdbExportOptions`、`MoleculeToPdb` 返回值），确认计划中列出的 API 名称与参数在实现时无需猜测
- Expected outcome: 输出一份已核对的公开 API 清单（类型全名 + 方法签名），使 `molecular_tools.vb` 的调用代码一次编译通过