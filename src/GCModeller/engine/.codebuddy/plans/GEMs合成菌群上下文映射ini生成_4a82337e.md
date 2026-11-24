---
name: GEMs合成菌群上下文映射ini生成
overview: 检查 N:\GMNDesigner\Acetoin\models_test2\GEMs 下 6 个 GCMarkup_3.0 模型的化合物 ID 体系，编写可复用的角色匹配器与 /context 子命令，并用 smoke 测试程序把匹配结果写入 Definition 后保存为 ini，供 vCella /run --context-map 使用。
todos:
  - id: context-builder
    content: 新增 ContextMappingBuilder.vb，实现角色候选匹配、Definition 装配、mapping.csv 与匹配报告，并用 [subagent:code-explorer] 核对 ini 往返与锚点
    status: completed
  - id: context-command
    content: 在 Program.vb 新增 /context 子命令，支持通配符与分号模型列表、ini 输出与布尔开关 args 回退
    status: completed
    dependencies:
      - context-builder
  - id: smoke-project
    content: 新增 vCella\smoke 工程，加载 6 个模型、生成 db_context.ini、回读逐项比对并输出 DebugAudit 报告
    status: completed
    dependencies:
      - context-builder
  - id: generate-ini
    content: 运行 smoke 生成 db_context.ini 与 mapping.csv，确认核心角色齐全且跨 6 模型一致、0 failures
    status: completed
    dependencies:
      - smoke-project
  - id: run-verify
    content: 用 /run --context-map 跑 1x1x1 单步模拟，确认培养基组分全部来自 ini 且无未知编号告警
    status: completed
    dependencies:
      - generate-ini
---

## 需求概述

为 `N:\GMNDesigner\Acetoin\models_test2\GEMs` 下的 6 个 GCMarkup_3.0 合成菌群模型生成可直接用于 `vCella /run --context-map` 的上下文映射 `Definition` ini 配置文件。

## 核心功能

- **模型 ID 体系勘察**：确认该批模型的化合物 `ID` 是“名称派生标识符”（如 `alpha-D-Glucose`），KEGG/MetaCyc 编号只存在于 `referenceIds`/`db_xrefs` 别名中，因此不能直接套用 `DbContextMapping.MetaCyc/KEGG`。
- **角色匹配器**：按“角色 → 候选编号/名称”规则，从模型化合物的 `ID`、语义名、`referenceIds`、`db_xrefs` 中解析出培养基所需的真实模型 ID（KEGG 编号优先锚定，MetaCyc 名称、精确名次之，关键词兜底且要求唯一命中）。
- **Definition 装配**：把匹配结果写入 `Water/Oxygen/Ammonia/PI/PPI/ATP/ADP/GTP/GDP`、`CultureMediumComponents`（role/id/concentration）、`Ions`、`Cofactors`、`WasteProducts`、`transmembrane.passive`、`CultureMedium`。
- **ini 落盘与回读**：`Definition.SaveIni` 输出到 `GEMs\db_context.ini`，并用 `Definition.LoadIni` 逐项回读校验，保证节点/字典/数组结构可往返。
- **可复用命令**：新增 `/context` 子命令，支持通配符或分号分隔的模型列表，输出 `mapping.csv`（角色、模型 ID、匹配证据、6 个模型是否齐全）与匹配报告；后续更换数据集可一键重建。
- **smoke 测试程序**：独立 console 工程加载 6 个模型、生成 ini、执行 DebugAudit 断言（核心角色齐全、培养基 ID 在 6 个模型中均存在、ini 往返一致、0 failures），并输出报告。
- **端到端衔接验证**：用生成的 ini 跑 `--context-map` 的单步模拟，确认打印出的培养基组分全部来自 ini 且为模型真实 ID。

## 输出物

- `N:\GMNDesigner\Acetoin\models_test2\GEMs\db_context.ini`（供 `/run --context-map`）
- `GEMs\context\mapping.csv`、`smoke_report.txt`、`smoke_report.json`

## 技术栈

- VB.NET / `net10.0`，沿用 `engine\vCella\vCella.vbproj`（Exe，RootNamespace `vCella`）、`engine\Dynamics\Dynamics.NET5.vbproj`、`engine\IO\GCMarkupLanguage\GCMarkupXML.NET5.vbproj`、sciBASIC `Core.vbproj`。
- 模型加载：sciBASIC 泛型扩展 `LoadXml(Of VirtualCell)(path)`（`SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2.VirtualCell`）。
- ini 序列化：`Definition.SaveIni(file)` / `Definition.LoadIni(file)`（内部 `ClassMapper.WriteClass/LoadIni`）。
- 测试记录：`Microsoft.VisualBasic.ApplicationServices.Debugging.DebugAudit`（Mark/Check/AreEqual/Dump/ToJson）。

## 已核实的关键事实

- 6 个模型均为 `schemaVersion="3.0"`、命名空间 `GCMarkup_3.0`，可直接反序列化。
- 化合物节点形态：`<compound ID="alpha-D-Glucose" referenceIds="C00267 ALPHA-GLUCOSE" formula="C6H12O6" db_xrefs="PubChem:79025 ChEBI:17925 C00267 ...">alpha-D-Glucose<smiles>…</smiles></compound>`。
- `C00031`（D-葡萄糖）在 `referenceIds` 中出现于 **6/6** 个模型，说明 KEGG 编号是跨模型稳定锚点。
- `Definition` 已具备 `CultureMediumComponents As MediumComponent()`、`Ions`、`Cofactors`、`WasteProducts`、`CultureMedium`、`transmembrane`、`context`、`SaveIni/LoadIni`；`MediumComponent` 为 `role/id/concentration`。
- `vCella\Simulation.resolveDefinition` 优先使用 `--context-map` 指定的 ini；`parseMedium(csv, define, available)` 会按模型化合物集合过滤 `CultureMediumComponents`，因此 ini 中的 ID 若不匹配会被自动剔除（并在控制台告警）。

## 实现方案

### A. 角色匹配器（新增 `engine\vCella\ContextMappingBuilder.vb`，`Public Module ContextMapping`）

对外 API：

```
Public Function BuildDefinition(modelFiles As String(),
                                Optional initMass As Double = 100,
                                Optional requireShared As Boolean = True,
                                Optional withContext As Boolean = False) As Definition
Public Function SaveIni(define As Definition, path As String) As Boolean
Public ReadOnly Property LastReport As MappingReport   ' 角色 → id / 证据 / 各模型是否命中
```

匹配优先级（全部 `OrdinalIgnoreCase`，按空白切分 token 后精确比较）：

1. KEGG 编号命中 `referenceIds` 或 `db_xrefs`（如 `C00031`）；
2. MetaCyc 名称命中 `referenceIds`（如 `GLC`、`OXYGEN-MOLECULE`、`Pi`、`AMMONIUM`、`SULFATE`、`PROTON`、`WATER`、`ACET`、`CARBON-DIOXIDE`）；
3. `ID` 或语义名精确匹配（如 `Water`、`Oxygen`）；
4. 关键词包含匹配，**仅当唯一命中**时采用，否则记入歧义告警（如 `D-Glucose` 与 `alpha-D-Glucose` 必须靠 `C00031`/`C00267` 区分）。

角色表（role → 候选 token → 默认浓度 mM）：

- 基础：`glucose`(C00031/GLC,10)、`oxygen`(C00007/OXYGEN-MOLECULE,1)、`ammonium`(C00014/AMMONIUM,5)、`phosphate`(C00009/Pi,1)、`sulfate`(C00059/SULFATE,1)、`water`(C00001/WATER,55.5)、`proton`(C00080/PROTON,1e-4)
- 离子：K(C00238)、Mg(C00305)、Ca(C00076)、Cl(C00115)、Fe2(C14818)、Fe3(C14819)、Zn(C00038)、Mn(C19610)、Cu(C00070)、Co(C00175)、Ni(C19609)、Mo(C06232)
- 辅因子：NAD/NADH/NADP/NADPH/FAD/FADH2/CO-A/ACETYL-COA/THIAMINE-PP/PYRIDOXAL-P/COBALAMIN
- 废物：CO2(C00011)、HCO3(C00288)、LACTATE(C00186)、ETHANOL(C00469)、FORMATE(C00058)、ACETATE(C00033)、H2(C00282)、H2S(C00283)、NH3(C00014)
- 能量：ATP(C00002)、ADP(C00008)、PPI(C00013)、GTP(C00044)、GDP(C00035)
- 本项目关注：`acetoin`(C01755/ACETOIN)、`2,3-butanediol`(C03044) —— 匹配不到只告警，不进入培养基

装配规则：

- `CultureMediumComponents`：仅在 `requireShared:=True` 时保留“6 个模型都存在”的角色，浓度取默认值；
- `Ions/Cofactors/WasteProducts`：`[角色键 => 模型 ID]`；
- `transmembrane.passive`：水/氧/CO2/氨/乙酸/乙醇/乳酸/丙酮醇（匹配到的）；
- `CultureMedium = "Extracellular"`；
- `context` 默认不写入（ID 含 `[`、`]`、`=`、`;`，作为 ini 字典键有转义风险，且模拟不依赖），`--with-context` 时才生成。

### B. `/context` 子命令（修改 `engine\vCella\Program.vb`）

```
/context --models 'N:\...\GEMs\*.xml' --output 'N:\...\GEMs\db_context.ini'
         [--init-mass 100 --with-context --report --verbose]
```

- `--models` 支持通配符（目录 + `*.xml`）与分号分隔列表；
- 输出匹配表（role / id / 证据字段 / 6 模型命中情况）与 `mapping.csv`；
- 与既有 `/compile`、`/run` 风格一致：布尔开关从 `args` 回退读取（`GetValue(Of Boolean)`），避免 `RunCLI` 对连字符参数名绑定失败。

### C. smoke 测试工程（新增 `engine\vCella\smoke\smoke.vbproj` + `Program.vb`）

- ProjectReference：`..\vCella.vbproj`、`..\..\Dynamics\Dynamics.NET5.vbproj`、`..\..\IO\GCMarkupLanguage\GCMarkupXML.NET5.vbproj`、sciBASIC `Core.vbproj`。
- 流程：枚举 6 个 xml → 逐个 `LoadXml` 统计化合物数 → `ContextMapping.BuildDefinition` → `SaveIni` 到 GEMs 根 → `Definition.LoadIni` 回读逐项比对 → `DebugAudit` 断言 → 写 `context\mapping.csv`、`smoke_report.txt/json`。
- 断言清单：

1. 6 个模型全部加载成功且化合物数 > 0；
2. `glucose/oxygen/ammonium/phosphate/sulfate/water/proton` 全部匹配；
3. 每个入选培养基 ID 在 6 个模型中都存在；
4. `CultureMediumComponents.Length >= 15`；
5. ini 存在且回读后 `role/id/concentration`、`Ions/Cofactors/WasteProducts`、标量逐项一致；
6. `DebugAudit.FailureCount = 0`。

### D. 端到端衔接验证

```
vCella.exe /run --model "<GEMs 中任一模型>" --context-map "N:\...\GEMs\db_context.ini" \
                -t 1 --resolution 1 --cells 1 --no-state
```

断言：控制台 `medium` 段落中的组分 ID 全部来自 ini，且非空、无 “medium compound not found in any model” 告警。

## 关键代码结构

```
' engine\vCella\ContextMappingBuilder.vb
Public Class MediumRole
    Public Property role As String
    Public Property candidates As String()      ' KEGG/MetaCyc/名称 候选
    Public Property concentration As Double
End Class

Public Class MappingReport
    Public Property Matched As List(Of String())     ' role, id, evidence, models
    Public Property Missing As List(Of String)
    Public Property Ambiguous As List(Of String)
    Public Property NotShared As List(Of String)
End Class

Public Module ContextMapping
    Function BuildDefinition(modelFiles As String(), Optional initMass# = 100,
                             Optional requireShared As Boolean = True,
                             Optional withContext As Boolean = False) As Definition
    Function SaveIni(define As Definition, path As String) As Boolean
    ReadOnly Property LastReport As MappingReport
End Module
```

## 目录结构

```
engine/
├── vCella/
│   ├── ContextMappingBuilder.vb   # [NEW] 角色匹配、Definition 装配、mapping.csv 与匹配报告
│   ├── Program.vb                 # [MODIFY] 新增 /context 子命令（通配符模型列表、ini 输出）
│   ├── Simulation.vb              # [不改动] 已有 --context-map 接入
│   └── smoke/
│       ├── smoke.vbproj           # [NEW] net10.0 console，引用 vCella/Dynamics/GCMarkupXML/Core
│       └── Program.vb             # [NEW] 6 模型加载、生成 ini、回读校验、DebugAudit 报告
└── Dynamics/Config/Definition/
    ├── Definition.vb              # [不改动] 复用 SaveIni/LoadIni 与培养基字段
    └── MediumComponent.vb         # [不改动] role/id/concentration
```

## 实施要点

- **序列化兜底**：若 `ClassMapper` 无法往返 `MediumComponent()`，回退为 `MediumRecipe As Dictionary(Of String, String)`（`role => "id|浓度"`），并在 `Simulation.resolveDefinition` 之后重建 `CultureMediumComponents`；smoke 的 round-trip 断言会在第一时间暴露该问题。
- **特殊字符安全**：模型 ID 只作为 ini 的 value（`role=id`），不写入字典键；`context` 默认关闭。
- **跨模型一致性**：`requireShared:=True` 过滤“部分模型缺失”的角色，并写入 `mapping.csv` 的 `models` 列（如 `6/6`、`4/6`），避免只有部分物种能摄取的隐性偏差。
- **告警可见**：匹配失败/歧义/未共享三类问题在控制台与报告中分级输出，便于人工确认后再跑模拟。
- **性能**：单模型化合物数千条，匹配为 O(角色数 × 化合物数) 的 token 集合查表，单次毫秒级；6 个模型加载为秒级。

### SubAgent

- **code-explorer**
- Purpose: 核对 `ClassMapper` 对 `MediumComponent()`/`Dictionary(Of String,String)` 的 ini 往返能力，以及在 6 个模型中抽样验证 `referenceIds`/`db_xrefs` 的 token 形态与关键角色（葡萄糖、氧、铵、磷酸、硫酸、质子、离子）的可用锚点。
- Expected outcome: 输出可序列化性结论与每个角色的实际匹配样本（ID + 证据字段 + 命中文件数），作为匹配表实现与冒烟断言阈值的依据。

### Skill

- **lsp-code-analysis**
- Purpose: 分析 `Definition.SaveIni/LoadIni`、`MediumComponent`、`Simulation.resolveDefinition`、`parseMedium` 的引用与调用层级，确保新增 `/context` 与 smoke 工程不破坏既有 `/run` 调用链。
- Expected outcome: 得到完整引用清单与受影响文件列表，确认 ini 结构与 `parseMedium` 的过滤逻辑一致。