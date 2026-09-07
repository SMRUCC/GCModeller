# RetroPath — 基于广义反应规则的代谢网络逆向路径合成搜索

VB.NET (.NET 10) 命令行项目，**零第三方依赖**（仅 .NET BCL，JSON 用 System.Text.Json）。
按《基于反应规则的逆向合成算法原理》readme 实现：以目标代谢物 B 为起点，将广义反应规则
逆向应用于当前分子（反应中心匹配 + 原子映射），逐步分解直到全部前体落入底盘内源代谢物
集合（汇），再正向组装成完整路径并做多维评分——类比有机合成中的逆合成分析。

## 一、构建与运行

```bash
cd RetroPath
dotnet build -c Release

# 柠檬酸（自定义汇）
dotnet run -c Release -- search --target "OC(=O)CC(O)(CC(=O)O)C(=O)O" --sink test/sink.tsv --out paths.json --pretty

# 使用内置 E. coli 核心汇（演示）
dotnet run -c Release -- search --target "CC(O)C(N)C(=O)O" --out threonine.json

# 用户规则扩展 + DFS 策略
dotnet run -c Release -- search --target "CC(=O)NC(=O)O" --rules test/extra_rules.tsv --strategy dfs --max-depth 4

# 枚举内置规则库 / 自检
dotnet run -c Release -- enumerate-rules --pretty
dotnet run -c Release -- selftest
```

## 二、readme.md 文档 → 代码映射表

### §一 逆向合成整体逻辑

| 文档概念 | 实现 | 代码位置 |
|---|---|---|
| 源 = 目标 B；汇 = 底盘内源代谢物集合（GEM 提取） | `--target` + `--sink` TSV（内置 E. coli 核心演示集） | `Program.vb` |
| 搜索操作 = 规则逆方向应用于 B，递归反推 | `RuleEngine.ApplyReverse`（产物模式 → 反应物拓扑） | `Chem/RuleEngine.vb` |
| 反方向锚定搜索树，避免盲目枚举 | 状态 = 待汇化合物集合，仅沿规则可匹配方向扩展 | `Search/BeamSearch.vb` |
| 终止 = 所有前体落入汇集合 | `pending.Count = 0` | `BeamSearch.Search` |

### §二 广义反应规则（核心组件一）

| 文档概念 | 实现 | 代码位置 |
|---|---|---|
| **原子映射**（反应物第 i 个原子 → 产物哪个原子；NP 难，EC 方法） | 类号 `:n` 为 SMIRKS 原子映射货币；匹配返回 {类号→分子原子}，逐步原子映射随 JSON 输出 | `Chem/PatternMatcher.vb` + `RuleEngine.Apply` |
| **EC/Morgan 迭代标注**（环境标签→桶排序→迭代精化） | `MorganRanks`：初始不变量(元素,电荷,H,度,键级多重集) × 邻居标签迭代，分区收敛即停 | `Chem/Molecule.vb` |
| **反应中心提取**（键级变化的键） | 变换引擎：目标侧键级覆盖（成键/变级）、匹配侧有而目标侧无的键删除（断键=反应中心） | `RuleEngine.Apply` 步骤 3 |
| **规则泛化**（基团特异性通配，不限具体底物） | SMARTS 子集模式：`[C] [CH2] [CD3H0!O:1]`——Hn 氢约束 / Dn 度约束 / !O 否定 / `:n` 类号 | `PatternMatcher.ParsePattern` |
| 规则质量决定化学合理性 | **价态闸门**：变换后全分子价态校验，违规应用丢弃（自动排除羧基氧化等伪变换） | `Molecule.ValenceViolations` |

### §三 搜索策略

| 文档概念 | 实现 | 代码位置 |
|---|---|---|
| **束搜索**（RetroPath2.0 默认；每层保留 top-k 化合物空间） | 状态 = pending 化合物集合；按 (pending 数, 总原子数, ΔG) 确定性排序取前 beam-width | `BeamSearch.Prune` |
| 深度优先 | `--strategy dfs`（beam-width=1 变体路径枚举） | 同上 |
| **循环消除**（祖先屏蔽 + 状态去重） | 每分支 used 指纹集合；前体 ∈ used → 剪枝；状态键 = sorted(pending) 去重 | `BeamSearch.Search` |
| **预过滤**（规则元素过滤/复杂度过滤） | 匹配器候选预筛（元素/H/度约束先行），匹配上限 limit 防组合爆炸 | `PatternMatcher.Match` |
| 双向搜索 / MCTS | 未实现（文档化；MCTS 见 RetroPath-RL）——束+DFS 覆盖主流策略 | — |

### §四 路径评估

| 文档概念 | 实现 | 代码位置 |
|---|---|---|
| 热力学可行性（ΔG） | ΔG_path = Σ 步 ΔG（规则级启发式基团贡献代理值）→ thermo = σ(−ΔG/10) | `Search/Scoring.vb` |
| 酶可得性 | 层级代理 mean(1/tier)：1=常见 EC 家族 2=一般 3=特化（Selenzyme/BridgIT 指纹相似度需外部工具，文档化简化） | `Scoring.ScorePath` |
| 路径长度 | length = 1/nSteps | 同上 |
| FBA 通量 | 未实现（需 GEM；接口：把正向路径步骤映射为反应列表后可接 FBA） | — |
| 加权全局评分 | 0.4·thermo + 0.3·enzyme + 0.3·length（`--w-*` 可调） | `Scoring.ScorePath` |

### §五/六 工具对比与化学合理性

| 文档概念 | 实现 |
|---|---|
| 规则从酶机制抽象 → 组合出非天然反应序列 | 规则库与底物解耦：任何含反应中心的分子都可应用（自检含"非天然"卤化规则 U001） |
| 预测反应可能无酶催化 → 酶可得性指标缓解 | tier 评分 |
| 热力学不可行 → ΔG 指标 | thermo 评分 |
| 组合爆炸 → 搜索策略缓解 | 束剪枝 + 循环消除 + 匹配上限 |

## 三、JSON 输出结构

```json
{
  "program": "RetroPath", "version": "1.0.0", "target": "CC(O)C(N)C(=O)O",
  "parameters": { "strategy": "beam", "beam_width": 50, "max_depth": 6,
                   "sink_size": 16, "num_rules": 9,
                   "weights": { "thermo": 0.4, "enzyme": 0.3, "length": 0.3 } },
  "stats": { "applications_tried": 812, "states_generated": 143,
              "max_depth_reached": 2, "elapsed_ms": 38, "paths_found": 12 },
  "paths": [
    { "id": "path_1", "global_score": 0.71, "thermo_score": 0.95,
      "enzyme_score": 1.0, "length_score": 0.5, "delta_g_total": -30.0, "num_steps": 2,
      "steps": [
        { "rule_id": "R002", "rule_name": "转氨酶（酮↔胺）",
          "substrates": ["OC(=O)C(N)C=O", "N"], "products": ["CC(O)C(N)C(=O)O"],
          "delta_g": -10.0, "enzyme_tier": 1 },
        { "rule_id": "R003", "rule_name": "醛缩酶（β-羟羰基裂解/aldol）",
          "substrates": ["CC=O", "OC(=O)C=O"], "products": ["CC(O)C(=O)C=O"],
          "delta_g": -15.0, "enzyme_tier": 1 } ] } ]
}
```

`steps` 为**正向生物合成顺序**（汇前体 → 目标）：逆合成步骤序列反转 + 规则方向翻转。
每步 substrates = 前体（+共底物），products = 该步产物。原子映射（类号→原子）在搜索内
部用于精确定位反应中心；报告以 SMILES 交换。

## 四、验证体系

1. **开发期 Python 镜像对拍**（`_validation/validate_retro.py`，与 VB 逐式对应）：
   - 指纹不变性（苹果酸两种写法同键）✓；隐式氢 3,2,1 ✓；Kekulé 环 ✓；[NH4+] ✓
   - 匹配：醇正例/无 OH 负例/!O 排除羧基 ✓；价态闸门拒绝羧基氧化 ✓
   - 9 条规则正反向全部命中预期产物（含断键碎片 H2O/CO2/NH3）✓
   - 束搜索：柠檬酸 1 步、苏氨酸 1 步（醛缩）+ 2 步（转氨→醛缩）、苹果酸 1 步、循环消除 ✓
2. **内置自检** `selftest`（8 组）与 Python 验证一一对应，另含 SMILES 往返、
   规则 TSV、JSON 往返。

## 五、已知边界（如实声明）

| 项 | 现状 |
|---|---|
| SMILES 子集 | 支链/环/电价/显式氢/多组分；芳香小写不支持（用 Kekulé）；立体化学（@/@@/E/Z）不支持 |
| 指纹 | Morgan EC 精化的图指纹（同构图同键）；非严格规范 SMILES，正则图极端情形可能碰撞（代谢物尺度可忽略） |
| ΔG | 规则级启发式代理值，非基团贡献法（GCM）或 eQuilibrator 精确计算；pH/离子强度未建模 |
| 酶可得性 | 层级代理；真实 Selenzyme/BridgIT 需反应指纹相似度（需外部数据库） |
| FBA/生长耦合 | 未实现（novoPathFinder/GEM-Path 特性）；路径产物可作为 GEM 外源反应接入 |
| 规则库 | 内置 9 条核心酶促规则（氧化还原/转氨/醛缩/脱羧/水合/磷酸化/酯水解/Claisen/互变异构）+ TSV 扩展；BNICE.ch 级别库需文献规则集导入 |
| 立体选择性 | 规则不区分对映体；前手性中心的区域选择由模式约束近似 |
| 搜索完备性 | 束搜索非完备（宽度限制）；增大 --beam-width/--max-depth 提高召回 |

## 六、文件清单

```
RetroPath/
├── RetroPath.vbproj           net10.0 控制台项目（无 PackageReference）
├── Program.vb                 CLI（search / enumerate-rules / selftest）
├── SelfTest.vb                内置自检（8 组）
├── Chem/
│   ├── Molecule.vb            分子图 + 价态/隐式氢 + 分量 + Morgan 指纹
│   ├── SmilesIO.vb            SMILES 子集解析/确定性写出（往返保真）
│   ├── PatternMatcher.vb      SMARTS 子集解析 + 回溯子图单射匹配
│   └── RuleEngine.vb          规则定义 + SMIRKS 式双向变换 + 价态闸门
├── Search/
│   ├── RuleLibrary.vb         内置 9 条规则 + 货币集合 + TSV 扩展
│   ├── BeamSearch.vb          状态空间束搜索/DFS + 循环消除 + 剪枝
│   └── Scoring.vb             ΔG/酶/长度 → 全局分 + 正向组装
├── Model/ResultModel.vb       JSON DTO
└── test/                      sink.tsv（E. coli 核心演示汇）+ extra_rules.tsv
```
