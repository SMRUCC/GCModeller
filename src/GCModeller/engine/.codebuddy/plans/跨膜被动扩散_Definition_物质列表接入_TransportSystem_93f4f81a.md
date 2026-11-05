---
name: 跨膜被动扩散：Definition 物质列表接入 TransportSystem
overview: 把 Definition.transmembrane 的被动扩散物质与逐物质渗透系数接入 Cella 的跨膜转运：新增每物质 permeability、蓝图级配置传递、无转运蛋白时的被动摄取（含 ΔC 项）与基于胞内产生池的被动外排，并在 /context 生成与 /run 侧完成告警与开关。
todos:
  - id: config-schema
    content: 扩展 Transmembrane 的 permeability/defaultPermeability/PermeabilityOf，并在 CellaBlueprint 与 Bootstrapping 中完成配置注入（用 [subagent:code-explorer] 核对调用点）
    status: completed
  - id: produced-index
    content: 在 MetabolicNetwork 增加边界物质产生量稀疏索引与 ProducedAmount，验证 Stoichiometry 行列语义与空索引返回 0
    status: completed
    dependencies:
      - config-schema
  - id: passive-transport
    content: 改造 TransportSystem：被动摄取 Fick 项、被动外排池、驱动浓度折算与被动通量统计（用 [skill:lsp-code-analysis] 确认无调用点破坏）
    status: completed
    dependencies:
      - produced-index
  - id: vcell-wiring
    content: 在 Simulation/Program 传递 Definition、增加 --no-passive/--passive-permeability 与边界告警，并在 ContextMappingBuilder 输出 permeability 表
    status: completed
    dependencies:
      - passive-transport
  - id: smoke-verify
    content: 扩展 vCella smoke：ini 往返断言、被动物质无转运蛋白时摄取大于零、质量守恒与 DebugAudit 报告
    status: completed
    dependencies:
      - vcell-wiring
  - id: bench-ab
    content: 在 bench 增加被动扩散开关，运行关闭/开启两态基准，输出 passive_diffusion_report.md 与相位耗时对比
    status: completed
    dependencies:
      - smoke-verify
---

## 产品概述
补齐 Cella 模拟输出 zip 中的元数据：修复 `metadata.json` 里「基因→EC 编号」关联为空的问题，并用 KEGG `ko00001` 的「通路→EC」映射生成完整的通路信息；同时在 zip 顶层新增 `manifest.json`，导出每个物种的转录组 mRNA、蛋白质组蛋白、代谢组代谢物 id 清单，供后续可视化使用。

## 核心功能
- **基因→EC 关联修复**：当前 `ec_numbers` 全为 `[]`，原因是模型里基因写作 `protein_id="BAH98159.1"`，而酶表写作 `proteinID="Protein[BAH98159.1]"`，两边差一个 `Protein[...]` 外壳；匹配时做去壳归一化即可填充，并新增「EC→基因」反向索引。
- **pathways 富结构**：用 `BriteHText.Load_ko00001()` 解析 KEGG KO 层次，取 `C` 层通路节点（如 `00010 Glycolysis / Gluconeogenesis`）及其后代 `D` 层条目中的 `[EC:...]`，与模型实际拥有的 EC 取交集后输出 `{ko00010: {name, category, ec_numbers[], genes[], molecules[]}}`，多物种合并为一份。
- **manifest.json**：zip 顶层新增，按物种列出 `mRNA`（基因 locus_tag）、`protein`（模型原样 `Protein[...]`）、`metabolite`（化合物 id）。
- **体量与开关**：通路富结构可能使 `metadata.json` 增到数 MB，提供 `--metadata=full|basic` 开关（`basic` 不写 pathways 与 manifest）。

## 效果
解压结果后可直接得到「通路 → EC → 基因 → 代谢物」的完整链路与三组 id 清单：`metadata.json` 的 `ec_numbers`、`ec_genes`、`pathways` 不再是空值，`manifest.json` 与 `metadata` 的基因键集合一致，前端可视化的 id 对齐全流程打通。


## 技术栈
- VB.NET / net10.0，沿用 `engine\Cella`（快照 DTO 与 zip 归档）、`engine\vCella`（模拟主流程与元数据构建）。
- KEGG 通路映射：`core\Bio.Assembly\biocore-netcore5.vbproj` 中的 `SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry.BriteHText.Load_ko00001()`（内嵌资源 `Resources\KEGG\ko00001.txt`，4.2 MB）。
- 依赖关系已核实：`vCella.vbproj:64` 直接引用 `biocore-netcore5.vbproj`，`BriteHText` 可直接使用；`Cella` 项目不引用 biocore，因此通路推导放在 `vCella`，`Cella` 只持有纯数据 DTO（保持引擎无外部数据库依赖）。

## 现状与根因（已核实）
| 现象 | 根因 |
|---|---|
| `metadata.json` 的 `ec_numbers` 全为 `[]` | `Simulation.buildCellMetadata` 用 `enzyme.proteinID`（`Protein[BAH98159.1]`）建表，再用 `gene.protein_id`（`BAH98159.1`）查询，键不匹配 |
| `genes` 全为 `null` | GEM 模型的 `<genes>` 无 GO 子节点，`gene.goTerms` 为 `Nothing`（保持原语义，不伪造） |
| `pathways` 为 `{}` | `buildPathwaysOf` 依赖 `metabolismStructure.maps`，GEM 模型没有该段 |
| 模型可用数据 | `genome.GetAllGenes()`（locus_tag / protein_id）、`genome.proteins[].protein_id`（`Protein[...]` 原样）、`metabolismStructure.{compounds, enzymes(proteinID/ECNumber/catalysis), reactions}` |

ko00001 层级（实测）：
```
A09100 Metabolism
B  09101 Carbohydrate metabolism
C    00010 Glycolysis / Gluconeogenesis [PATH:ko00010]
D      K00844  HK; hexokinase [EC:2.7.1.1]
D      K13810  tal-pgi; ... [EC:2.2.1.2 5.3.1.9]
```
`BriteHText.entryID`：`C` 层为 5 位数字（`00010`），`D` 层为 `K00844`；`description` = 去掉 `entryID` 的文本（需再去掉 `[PATH:...]`）；`parent`/`categoryItems` 提供 A/B 祖先。

## 实现方案

### A. 快照 DTO（`engine\Cella\Snapshots\`）
1. `CellMetadata` 新增 `ec_genes As Dictionary(Of String, String())`（EC → 基因，反向索引），`ec_numbers`（基因→EC）语义不变。
2. 新增 `PathwayMetadata.vb`：`id`（`ko00010`）、`name`、`category`（`"Metabolism; Carbohydrate metabolism"`）、`ec_numbers As String()`、`genes As String()`、`molecules As String()`。
3. `Metadata.pathways` 类型由 `Dictionary(Of String, String())` 改为 `Dictionary(Of String, PathwayMetadata)`（当前为 `{}`，无兼容负担）。
4. `SnapshotWriter.BuildMetadata(..., Optional pathways As Dictionary(Of String, PathwayMetadata))` 同步改签名（`Cella\Snapshots\SnapshotWriter.vb:189`）。
5. 新增 `Manifest.vb`：`Manifest { total_time, species As Dictionary(Of String, SpeciesManifest) }`，`SpeciesManifest { taxonomy, mRNA As String(), protein As String(), metabolite As String() }`。
6. `SnapshotArchive` 新增 `Public Const ManifestEntryName As String = "manifest.json"` 与 `WriteManifest(manifest)`，写顶层条目（复用 `WriteMetadata` 的 `CountingStream` + `JsonSerializer` 模式，压缩等级 `SmallestSize`），并补 `ReadManifest()` 便于测试。

### B. 基因→EC（`engine\vCella\Simulation.vb`）
- 新增 `normalizeProteinId(id)`：大小写不敏感地把 `Protein[X]` 归一为 `X`（仅用于匹配，输出保持原样）。
- `buildCellMetadata`：
  1. 用「原样 + 去壳」两种键建立 `proteinID → EC` 索引（一个酶多 EC 时按空白拆分）；
  2. 遍历 `genome.GetAllGenes()`，对每个 `gene.protein_id` 先查原样、再查去壳，收集去重后的 EC 数组；
  3. 同时填充 `ec_genes` 反向索引；
  4. `genes` 仍写 `gene.goTerms`（GEM 无 GO 时为 null，属预期）。

### C. pathways（`engine\vCella\Simulation.vb`）
- `Private Shared ReadOnly KeggKo As New Lazy(Of BriteHText)(Function() BriteHText.Load_ko00001())`，全进程只解析一次。
- 递归遍历：`entryID` 匹配 `^\d{5}$` 的节点即为通路节点：
  - `id = "ko" & entryID`；`name` = `description` 去 `[PATH:...]`；`category` = A/B 祖先 `description` 以 `"; "` 连接；
  - `ec` = 该节点全部后代叶子 `classLabel` 中 `\[EC:([0-9. ]+)\]` 的并集。
- 与模型取交：模型 EC 集来自 `metabolismStructure.enzymes[].ECNumber`（精确匹配，不做模糊），仅保留交集非空的通路。
- `genes`：`gene→EC` 命中通路 EC 的 locus_tag（去重排序）。
- `molecules`：经 `enzyme.catalysis.reaction` 找到对应反应，收集 `substrate/product` 的 `compound`（去重，单通路上限 2000 以控体积）。
- 多物种按 `id` 合并（EC/genes/molecules 取并集），得到全局一份。

### D. manifest（`engine\vCella\Simulation.vb`）
`buildManifest(models, totalTime)`，按 `getSpeciesName(model)` 建键（重名加 `#n`，与 `Metadata.cells` 一致）：
- `mRNA` = `genome.GetAllGenes().locus_tag`（去重排序）
- `protein` = `genome.proteins[].protein_id`（`Protein[...]` 原样；为空时回退到 `enzymes[].proteinID`）
- `metabolite` = `metabolismStructure.compounds[].ID`

### E. 接线与开关
- `Simulation.RunModel`：`buildCellMetadata`/`buildPathways` 结果传入 `SnapshotWriter.BuildMetadata`；`archive.WriteMetadata(meta)` 之后调用 `archive.WriteManifest(manifest)`；summary 增加 `pathways / manifest` 统计行。
- 新增 `Optional metadata As String = "full"`，`/run --metadata=basic|full`：`basic` 时跳过 pathways 与 manifest（走 `args` 回退，保持既有参数绑定习惯）。

## 关键代码结构
```vb
' Cella\Snapshots\PathwayMetadata.vb
Public Class PathwayMetadata
    Public Property id As String
    Public Property name As String
    Public Property category As String
    Public Property ec_numbers As String()
    Public Property genes As String()
    Public Property molecules As String()
End Class

' Cella\Snapshots\Manifest.vb
Public Class Manifest
    Public Property total_time As Double
    Public Property species As Dictionary(Of String, SpeciesManifest)
End Class

' Cella\Snapshots\SnapshotArchive.vb
Public Const ManifestEntryName As String = "manifest.json"
Public Sub WriteManifest(manifest As Manifest)
```

## 目录结构
```
engine/
├── Cella/Snapshots/
│   ├── CellMetadata.vb        # [MODIFY] 新增 ec_genes 反向索引
│   ├── PathwayMetadata.vb     # [NEW] 通路富结构 DTO
│   ├── Manifest.vb            # [NEW] manifest DTO
│   ├── Metadata.vb            # [MODIFY] pathways 类型改为 PathwayMetadata
│   ├── SnapshotWriter.vb      # [MODIFY] BuildMetadata 的 pathways 参数类型
│   └── SnapshotArchive.vb     # [MODIFY] WriteManifest / ReadManifest / ManifestEntryName
├── vCella/
│   ├── Simulation.vb          # [MODIFY] normalizeProteinId、buildCellMetadata 修复、
│   │                          #          buildPathways 重写、buildManifest、--metadata 开关
│   ├── Program.vb             # [MODIFY] /run 透传 --metadata
│   └── smoke/Program.vb       # [MODIFY] 新增 metadata/manifest 断言
```

## 实施注意事项
- **只做精确 EC 匹配**：KEGG 的 `1.1.1.-` 之类不完整编号不参与匹配，避免污染；在输出中记录 `matched_ec / total_ec` 便于判断覆盖率。
- **`Protein[...]` 只在匹配时去壳**：`manifest` 与酶相关输出保持模型原样，避免与 XML 字面不一致。
- **体积控制**：`molecules` 单通路上限 2000；`ec_genes`/`pathways` 均为去重后数组；提供 `--metadata=basic`。
- **性能**：ko00001 只解析一次（`Lazy`，约 1–2 s）；按模型 EC 集做交集，不遍历全部 KO 叶子的 EC 组合。
- **兼容性**：`Metadata.pathways` 类型变更会破坏旧的 JSON 读取方（当前为空，风险可接受）；`ReadMetadata` 保持可读新结构。

## 验证
1. 编译：`Cella`、`CellaLoader`、`vCella`、`smoke` 均 0 error。
2. `/run`（GEM 目录，1 步，`--no-state`）后解压产物：
   - `metadata.json`：抽样 20 个基因，`ec_numbers` 非空比例 > 30%；`ec_genes` 条目 > 500；`pathways` 条目 > 100，且 `ko00010/ko00020/ko00620` 的 `ec_numbers`/`genes` 非空；
   - `manifest.json`：每物种 `mRNA ≥ 1000`、`protein ≥ 1000`、`metabolite ≥ 10000`，`mRNA` 与 `metadata.cells[*].ec_numbers` 键集合一致；
   - `--metadata=basic` 时两者均不出现额外内容且帧/回读正常。
3. `smoke` 增加上述断言，保持既有 ini/被动扩散断言 0 failures。
4. 记录 `metadata.json` 变更前后字节数（0.5 MB → 预计 2–5 MB）。


## Agent Extensions
### SubAgent
- **code-explorer**
  - Purpose: 实施前核对 `BriteHText` 的 `entryID/description/parent/categoryItems` 语义与 `ko00001` 层级边界，并确认 `genome.proteins`、`enzyme.catalysis`、`Reaction.substrate/product` 字段在 GEM 模型中的实际取值形态
  - Expected outcome: 输出精确到文件与行号的引用清单（含 `BriteHText.vb`、`Genome.vb`、`MetabolismStructure.vb`、`Simulation.vb`），确保通路解析与 manifest 字段来源无歧义
