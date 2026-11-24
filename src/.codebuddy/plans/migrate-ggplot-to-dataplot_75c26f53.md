---
name: migrate-ggplot-to-dataplot
overview: 将 runtime/ggplot/src 下三个 R# 包项目（ggplot、ggpubr、ggraph）的绘图引擎从旧 Plots/Plots-statistics 切换到 DataPlot：保留 ggplot 的图层模型（layers/aes/scale/跨层联合坐标轴），在 DataPlot 侧新增共享画布与图层叠加适配层、画布级图例聚合、以及 CSS Theme → GgplotTheme.Theme → PlotTheme 的桥接，最终移除三个 vbproj 对旧项目的引用。
todos:
  - id: dataplot-canvas-compat
    content: 使用 [subagent:code-explorer] 核实旧基元精确签名后，在 DataPlot 新建 Canvas 目录：移植 CanvasTheme、PlotBase、GraphicsRegion/PaddingLayout、DataScaler、DrawAxis、LegendObject/DrawLegends
    status: completed
  - id: dataplot-layer-mode
    content: 改造 PlotEngine 支持图层模式（DrawFrame/AutoRange 开关、可写 PlotArea、画布级图例），并为约 11 个图型类补 New(g As IGraphics, theme) 共享画布构造
    status: completed
    dependencies:
      - dataplot-canvas-compat
  - id: theme-bridge
    content: 实现 ThemeBridge：CanvasTheme 序列化 CSS → CssThemeMapper.ParseCss → GgplotTheme.Theme → PlotTheme，缺失字段安全回退
    status: completed
    dependencies:
      - dataplot-canvas-compat
  - id: ggplot-core-render
    content: 迁移 ggplot 核心：Internal/ggplot.vb 基类换 PlotBase、render/{chart2D,g2d,g3d}.vb 管线换 DataPlot DataScaler/Axis/Plot3D，并批量切 Imports
    status: completed
    dependencies:
      - dataplot-layer-mode
      - theme-bridge
  - id: ggplot-layers
    content: 使用 [skill:lsp-code-analysis] 枚举 22 处 ggplotLayer.Plot 重写，逐图层委派给新引擎图型类（scatter/line/histogram/bar/boxplot/violin/jitter/pie/tile/principalCurve 等）
    status: completed
    dependencies:
      - ggplot-core-render
  - id: ggpubr-ggraph
    content: 迁移 ggpubr 的 ggplotConfidenceEllipse 与 ggplotTextRepelLabel，以及 ggraph 的 graphRender 与 nodeRender 至 DataPlot Canvas 体系
    status: completed
    dependencies:
      - ggplot-core-render
  - id: vbproj-refs
    content: 全项目搜索确认无 ChartPlots 残留后，将三个 vbproj 的 plots/plots_extensions 引用替换为 DataPlot.vbproj（保留 network/physics/stats 等非绘图引用）
    status: completed
    dependencies:
      - ggplot-layers
      - ggpubr-ggraph
  - id: build-and-smoke
    content: 依次编译 DataPlot、ggplot、ggpubr、ggraph 修正错误，并用 runtime/ggplot/test 下的 R 脚本做出图冒烟验证
    status: completed
    dependencies:
      - vbproj-refs
---

## 产品概述

将 `runtime\ggplot\ggplot.NET5.sln` 下的三个 R# 包项目（ggplot、ggpubr、ggraph）的底层绘图引擎，从旧库 `plots-netcore5.vbproj` / `plots_extensions-netcore5.vbproj` 切换到新引擎 `DataPlot.vbproj`，在保留 ggplot 图层语义（layers / aes / scale / 跨图层联合坐标轴 / 多 dataset）的前提下，彻底移除对旧绘图引擎的项目引用。

## 核心功能

- **DataPlot 图层适配层**：新增共享画布能力（各图型类 `New(g As IGraphics, theme)` 重载）、图层叠加模式（抑制框架重绘 + 外部钉死坐标轴范围与绘图区）、画布级跨图层图例聚合
- **旧基元收容**：把旧引擎被 ggplot 依赖的底层设施（`Canvas.Theme` CSS 主题、`GraphicsRegion`/`PlotRegion`/`PaddingLayout.EvaluateFromCSS`、`DataScaler`、`Axis.DrawAxis`、`LegendObject`/`LegendStyles`）移植进 DataPlot，而不引用旧项目
- **主题桥接**：`Canvas.Theme`（CSS 字符串）→ `CssThemeMapper.ParseCss` → `GgplotTheme.Theme`（ThemeElement/Unit/Margin）→ `PlotTheme`（强类型），渐进迁移，`ggplotTheme.vb` 暂不改写出方式
- **ggplot 迁移**：保留图层模型，22 个 geom 图层内部改为委派给 DataPlot 图型类（ScatterPlot/LinePlot/HistogramPlot/BarPlot/BoxPlot/ViolinPlot/JitterPlot/PiePlot/HeatmapPlot 等），约 45 个文件需触碰
- **ggpubr / ggraph 迁移**：ggpubr 2 个文件（PCA 置信椭圆、文本排斥标签），ggraph 2 个文件（网络图渲染、节点图例）；其网络布局依赖 network 库，不受影响
- **引用清理与验证**：三个 vbproj 移除旧引擎引用改引 DataPlot，逐项目编译通过，R# 对外 API 与 R 脚本行为不回归

## 技术栈

- 语言：VB.NET / .NET 10.0（三个 ggplot 项目与 DataPlot 同为 net10.0）
- 新目标引擎：`runtime/sciBASIC#/Data_science/Visualization/DataPlot/DataPlot.vbproj`（RootNamespace `Microsoft.VisualBasic.Data.Plots`，已支持 `Drivers` 画布与 `GraphicsData` 输出）
- 依赖保持不变：imaging（GDI+/IGraphics/驱动体系）、Core、html（CSS）、Math、ANOVA/stats/linear/dataframeUtils、graph / network 三件套 / physics（ggpubr、ggraph 的非绘图依赖）

## 实现方案

### 核心策略：保留图层模型 + 新引擎补适配层

旧引擎给 ggplot 提供的是「底层基元」（`Plot` 基类 + `PlotInternal(ByRef g, canvas)`、`DataScaler`、`GraphicsRegion`、`Axis.DrawAxis`、`LegendObject`、CSS `Theme`），真正的 ggplot 图层引擎（`ggplot.vb` / `ggplotLayer.vb` / `ggplotAdapter.vb` 及 22 个 `Overrides Plot(stream)` 子类）全部位于 `runtime/ggplot/src` 内部，可整体保留。

迁移因此分成两件事：

1. **DataPlot 侧「承接」旧基元**（新增独立目录 `Canvas/`，不引入旧项目引用），使 ggplot 的图层骨架继续可用；
2. **DataPlot 侧「开放图层能力」**（共享画布构造 + 抑制框架重绘 + 外置坐标范围 + 画布级图例），使每个 geom 可以把实体绘制委派给新引擎图型类。

```mermaid
flowchart LR
    subgraph ggplot 图层引擎（保留）
        A[ggplot.vb 继承 PlotBase]
        B[ggplotAdapter 跨图层联合坐标轴]
        C[22 个 ggplotLayer 子类]
    end
    subgraph DataPlot Canvas 承接层（新增）
        D[CanvasTheme CSS 主题]
        E[GraphicsRegion / PaddingLayout]
        F[DataScaler]
        G[Axis.DrawAxis]
        H[LegendObject / DrawLegends]
        I[ThemeBridge: CSS→GgplotTheme.Theme→PlotTheme]
    end
    subgraph DataPlot 图型类（已有 + 补共享画布）
        J[ScatterPlot / LinePlot / HistogramPlot]
        K[BarPlot / BoxPlot / ViolinPlot / JitterPlot / PiePlot]
        L[HeatmapPlot / ContourPlot / PrincipalCurvePlot]
        M[PlotEngine 图层模式 DrawFrame=False]
    end
    A --> D
    B --> F
    C --> J
    C --> K
    C --> L
    A --> G
    A --> H
    D --> I
    I --> M
    J --> M
    K --> M
```

### 关键技术决策与权衡

| 决策 | 理由 |
| --- | --- |
| 旧 `Canvas.Theme` 移植而非重写 | 用户选定渐进迁移；`ggplotTheme.vb:99-199` 把 `element_*` 翻译成 18 个 CSS 字段集中写入，重写为 `ThemeElement` 模型风险高、回归面大；移植后用 `CssThemeMapper.ParseCss` 天然桥接 |
| 保留 `PlotInternal(ByRef g, canvas)` 契约 | 22 个图层与 `chart2D/g2d/g3d` 渲染管线都挂在这个入口上；改为新引擎的「独占画布」模型会直接摧毁图层叠加语义（多 dataset、zindex、跨层轴） |
| 图层叠加用「DrawFrame 开关 + 外置轴范围」而非新建图层类 | `PlotEngine` 已具备 `XMin/XMax/YMin/YMax`（:139-142）、`New(g As IGraphics, theme)`（:226）、`_ownsGraphics`（:127）；补一个开关即可让第 2..N 层只画几何体，改动集中在基类，避免复制 36 个图型类 |
| 图例提升到画布级绘制 | 旧 `DrawLegend(seriesList)` 是单层语义；ggplot 是 `DrawLegends(IEnumerable(Of IggplotLegendElement))` 统一排版七种图例，必须在画布层实现，否则多图层图例位置/去重全部错乱 |
| `DrawAxis` 移植而非改写调用点 | 单点调用（`chart2D.vb:162-179`，16 个命名参数），移植成本远低于把 22 个图层改成新引擎自己的坐标轴（`DrawAxisAndGrid` 语义与 ggplot 的坐标/维度不完全一致） |


### 性能与稳定性要点

- 共享画布：`_ownsGraphics = False` 已存在，图层模式禁止 `PlotEngine.Dispose` 释放宿主画布，避免第 2 层绘制后画布失效
- 避免重复计算：跨图层联合坐标轴由 `ggplotAdapter.getXAxis/getYAxis/getZAxis` 一次性算好，通过 `XMin/XMax/YMin/YMax` 钉死，图层内不再 `AutoRange`
- 资源管理：图型类实例按图层 `Using` 包裹，仅最后一块画布由 `ggsave` 走 `DriverLoad.GetData` 导出
- 日志/排错：绘制转换失败时保留原异常并把图层名/geom 类型带上，避免静默空白图

## 架构设计

### DataPlot 新增模块（收容 + 适配，均在 `Microsoft.VisualBasic.Data.Plots` 命名空间下）

- `Canvas/CanvasTheme.vb`：移植旧 CSS 主题字段模型（`background`/`padding`/`mainCSS`/`legend*CSS`/`axis*CSS`/`tagCSS`/`gridStroke*`/`lineStroke`/`colorSet`/`nticksX,Y`/`drawGrid`/`X,YaxisTickFormat`/`GetX,YAxisDecimals`/`xAxisLayout`/`yAxisLayout` 等），`Implements ICloneable`
- `Canvas/GraphicsRegion.vb`：`GraphicsRegion` + `PlotRegion(css)` 扩展 + `Padding.TryParse` / `PaddingLayout.EvaluateFromCSS` / `LayoutVector`（如 imaging 已提供同功能则直接复用，仅补缺失的扩展）
- `Canvas/DataScaler.vb`：`DataScaler`（`AxisTicks`/`region`/`X`/`Y`）与必要的 `YScaler` 基类
- `Canvas/Axis.vb`：`DrawAxis(g, canvas, scaler, ...)` 16 参数移植；刻度复用已有 `Engine/AxisTicks.vb` 的 `CreateAxisTicks`
- `Canvas/Legend.vb`：`LegendObject` / `LegendStyles` 与画布级 `DrawLegends(legendList, g, canvas, location)`（复用 `Plot3D/Legend/LegendObject.vb` 与 `LegendPlot.vb:292` 的 `DrawLegend`）
- `Canvas/PlotBase.vb`：旧 `ChartPlots.Graphic.Plot` 等价基类（`theme`、`MustOverride PlotInternal(ByRef g, canvas)`、`Plot(size,dpi,driver)`、`Plot(size$,ppi,driver)`、`Plot(ByRef g, layout)`），内部走 `g.GraphicsPlots` / `DriverLoad`
- `Canvas/ThemeBridge.vb`：`CanvasTheme → CSS → CssThemeMapper.ParseCss → GgplotTheme.Theme → PlotTheme` 单向转换，含缺失字段的安全回退
- `Engine/PlotEngine.vb`（改动）：新增 `DrawFrame As Boolean = True`、`AutoRange As Boolean = True`、可写 `PlotArea`（或 `SetPlotArea`）、画布级图例入口
- 各图型类：补 `Public Sub New(g As IGraphics, Optional theme As PlotTheme = Nothing)` 重载（BarPlot、HistogramPlot、ContourPlot、StackedBarPlot、StackedAreaPlot、AreaPlot、BoxPlot、ViolinPlot、JitterPlot、PiePlot、HeatmapPlot 等）

### 迁移后数据流

`ggsave(file, size, driver)` → `DriverLoad.CreateGraphicsDevice(size, fill, driver)` 得共享 `IGraphics` → `ggplot.Plot(g, layout)` → `PlotInternal` → `chart2D.plot2D`（先画坐标轴框架一次）→ 按 `zindex` 顺序各图层 `Plot(stream)`（框架抑制、轴范围已钉死、几何体委派给新引擎图型类）→ 画布级统一 `DrawLegends` → `DriverLoad.GetData(g, padding)` 返回 `GraphicsData`

## 目录结构

```
runtime/sciBASIC#/Data_science/Visualization/DataPlot/
├── Canvas/
│   ├── CanvasTheme.vb        # [NEW] 移植旧 CSS 主题字段模型；承载 ggplot 的 theme 输出
│   ├── PlotBase.vb           # [NEW] 旧 ChartPlots.Graphic.Plot 等价基类（PlotInternal + GraphicsPlots + driver）
│   ├── GraphicsRegion.vb     # [NEW] GraphicsRegion/PlotRegion(css)/PaddingLayout.EvaluateFromCSS 等画布布局工具
│   ├── DataScaler.vb         # [NEW] 数据-像素缩放器（AxisTicks/region/X/Y）
│   ├── Axis.vb               # [NEW] DrawAxis 坐标轴绘制（复用 Engine/AxisTicks.CreateAxisTicks）
│   ├── Legend.vb             # [NEW] LegendObject/LegendStyles + 画布级 DrawLegends 聚合排版
│   └── ThemeBridge.vb        # [NEW] CanvasTheme→CSS→GgplotTheme.Theme→PlotTheme 单向桥接
├── Engine/
│   ├── PlotEngine.vb         # [MODIFY] 新增 DrawFrame/AutoRange 开关、可写 PlotArea、画布级图例入口
│   └── AxisTicks.vb          # [已有] 被 Canvas/Axis.vb 复用
├── Basic|Advanced|Statistics/# [MODIFY] 各图型类补 New(g As IGraphics, theme) 共享画布构造（约 11 个类）
└── Extensions.vb             # [MODIFY] 图层模式下的 AsGraphicsData/Save 保持不释放宿主画布

runtime/ggplot/src/ggplot/
├── ggplot.NET5.vbproj        # [MODIFY] plots-netcore5 → DataPlot.vbproj
├── Internal/ggplot.vb        # [MODIFY] Inherits Plot → PlotBase；2D/3D 入口换 DataPlot Canvas 类型
├── Internal/render/chart2D.vb, g2d.vb, g3d.vb   # [MODIFY] DataScaler/DrawAxis/3D 管线换 DataPlot
├── Internal/layers/**/*.vb   # [MODIFY] 22 个 geom：Scatter→ScatterPlot、Line→LinePlot、Histogram→HistogramPlot、
│                             #          Bar→BarPlot、Boxplot→BoxPlot、Violin→ViolinPlot、Jitter→JitterPlot、
│                             #          Pie→PiePlot、Tile/Scatterheatmap→HeatmapPlot、PrincipalCurve 等委派新引擎
├── Internal/colors/**.vb、elements/**.vb、options/**.vb、zzz.vb、ggplot2.vb、ggplot3.vb
│                             # [MODIFY] 多数仅换 Imports；options/ggplotTheme.vb 保持 CSS 写出（经 ThemeBridge 转换）
└── Interop/ggplotFunction.vb # [MODIFY] Canvas/Legend 命名空间切换

runtime/ggplot/src/ggpubr/
├── ggpubr.vbproj             # [MODIFY] plots_extensions-netcore5 → DataPlot.vbproj
└── layers/ggplotConfidenceEllipse.vb, ggplotTextRepelLabel.vb   # [MODIFY] PCA 置信椭圆与文本排斥改 DataPlot

runtime/ggplot/src/ggraph/
├── ggraph.vbproj             # [MODIFY] plots-netcore5 → DataPlot.vbproj（保留 network/physics/graph 引用）
└── Internal/graphRender.vb, Internal/render/nodeRender.vb       # [MODIFY] PlotInternal 与 Legend 改 DataPlot Canvas
```

## 实施注意

- **编译顺序**：DataPlot（Canvas 层 + 图型类重载）→ ggplot → ggpubr → ggraph，每步 `dotnet build` 保证可编译，禁止一次性大改
- **命名空间统一**：三个项目的 RootNamespace 均为 `ggplot`，改 Imports 时留意 `ggplotType`/同名类型二义性，必要时用别名 Imports
- **旧类型残留检查**：移除 vbproj 引用前全项目搜索确认无 `Microsoft.VisualBasic.Data.ChartPlots` 残留 Imports
- **R# 对外契约不变**：`ggplot()`、`ggsave()`、各 `geom_*`/`element_*`/`theme_*` 函数名与参数保持原样，`runtime/ggplot/test` 与 `R/` 下脚本行为不回归
- **已知缺口**：wordcloud / 独立 `geom_text` / 散点饼图（scatterpie）在新引擎无直接对应，需基于 `IGraphics` 原语或 `FillPolygons` 自绘实现，不要静默丢图
- **ggsave 导出**：沿用 `DriverLoad.CreateGraphicsDevice` + `DriverLoad.GetData`，保证 png/svg/pdf/postscript 驱动行为与旧引擎一致

## Agent Extensions

### SubAgent

- **code-explorer**
- Purpose：每一步迁移前精确定位旧类型调用点、核实 DataPlot 侧对应 API 的确切签名与缺失项，以及迁移后做残留 `ChartPlots` 依赖的全库搜索
- Expected outcome：获得旧 API 与新 API 的逐条对照证据（绝对路径 + 行号），避免签名猜测导致编译错误与漏改

### Skill

- **lsp-code-analysis**
- Purpose：对 `ggplotLayer.Plot` 的 22 处重写、`Axis.DrawAxis`、`DrawLegends` 等符号做定义/引用/实现导航，确认调用链与影响面
- Expected outcome：完整掌握被改动符号的全部调用点，保证批量改 Imports 与基类切换不遗漏、不误改