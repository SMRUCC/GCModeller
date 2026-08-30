# MiniBlast — 从头实现的 NCBI BLAST+ 核心算法（blastn / blastp）

VB.NET (.NET 10) 命令行项目，**零第三方依赖**（仅 .NET BCL，JSON 用 System.Text.Json），
按《NCBI BLAST+ 中 BLASTN 与 BLASTP 算法详解》文档从头实现 seed-and-extend 全流程，
输出结构化 JSON 比对结果供下游脚本解析。

## 一、构建与运行

```bash
cd MiniBlast
dotnet build -c Release

# blastn（跨种核酸比对任务）
dotnet run -c Release -- blastn --query test/nt_query.fa --db test/nt_db.fa --out nt_result.json --pretty

# megablast（极相似序列，word=28）
dotnet run -c Release -- blastn --task megablast --query test/nt_query.fa --db test/nt_db.fa

# dc-megablast（跨种，非连续模板种子）
dotnet run -c Release -- blastn --task dc-megablast --query test/nt_query.fa --db test/nt_db.fa

# blastp（蛋白比对，BLOSUM62 + 组成校正）
dotnet run -c Release -- blastp --query test/aa_query.fa --db test/aa_db.fa --comp-based-stats 1 --pretty

# 内置自检（λ 解、DP vs 参照 SW 交叉验证、DUST/SEG、端到端冒烟）
dotnet run -c Release -- selftest
```

## 二、README 文档 → 代码映射表

### §一 BLAST 核心流程（五阶段）

| 文档概念 | 公式/条件 | 代码位置 |
|---|---|---|
| 流程编排（过滤→查表→扫描→延伸→统计） | — | `Core/BlastEngine.vb` — `BuildDatabase` / `RunQuery` |
| [阶段1] DUST 低复杂度过滤（核酸） | S = Σ_v C_v(C_v−1)/2 / (W−1) > level/10，W=64, word=3 | `Core/LowComplexity.vb` — `Dust.Mask`（滑窗 O(L) 增量实现） |
| [阶段1] SEG 低复杂度过滤（蛋白） | 香农熵 H = −Σ p·log₂p < 2.2 触发，≥2.5 终止，W=12 | `Core/LowComplexity.vb` — `SegFilter.Mask` |
| [阶段2] BLASTN word 查找表 | 连续 word 精确匹配，base-4 打包 Long 键 | `Core/WordLookup.vb` — `NtWordLookup`（滚动编码） |
| [阶段2] dc-megablast 模板种子 | 11/18 模板 coding=`101101100101101101`，optimal=`111010010110010111`，只编码 care 位 | `Core/WordLookup.vb` — `DcWordLookup` |
| [阶段2] BLASTP 邻域 word | 查询 word 与 db word 比对得分 ≥ T（默认 11） | `Core/WordLookup.vb` — `AaWordLookup.ExpandNeighborhood`（按列最大得分上界剪枝的递归枚举） |
| [阶段3] 两-hit 法 | 同对角线 diag=i−j，两命中非重叠且距离 ≤ A=40 | `Core/SeedExtend.vb` — `SeedScanner.ScanSequence`（每对角线维护 lastHit 与已延伸区） |
| [阶段4] 无 gap X-drop 延伸 | 双侧行走，得分相对该侧最优下降 > X_ungap（20 bits）即停；最优段必含种子 | `Core/SeedExtend.vb` — `UngappedExtend` |
| [阶段5] 有 gap X-drop 延伸 | 仿射间隙 SW 变体：E=max(H↑−go, E↑−ge)，F=max(H←−go, F←−ge)，H=max(diag+E,F)；反对角线迭代 + X-drop 剪枝；双向合并 | `Core/SeedExtend.vb` — `GappedForward`（每格记录 H/E/F 三状态回溯方向）+ `GappedExtend`（正反向合并）+ `TracebackMoves`（状态机回溯） |
| 两级 gapped（预延伸 Xg → 最终延伸） | 预延伸 X=30 bits 定坐标 → 最终延伸 X=100 bits（blastp 25）重比对出串 | `Core/SeedExtend.vb` — `ScanSequence` 中两次 `GappedExtend` 调用 |

### §二 BLASTN 任务预设 [§2.1 表]

| 任务 | Word | Reward/penalty | Gap open/extend | 代码位置 |
|---|---|---|---|---|
| megablast | 28 | +1/−2 | 0 / 动态：[式2-1] \|2·penalty−reward\|/2（例：1/−5 → 5.5） | `Program.vb` — `ParseArgs` megablast 分支 |
| dc-megablast | 11/18 非连续 | +2/−3 | 5/2 | `Program.vb` + `DcWordLookup` |
| blastn | 11 | +2/−3 | 5/2 | `Program.vb` |
| blastn-short | 7 | +1/−3 | 5/2，dust 关 | `Program.vb` |

### §三 BLASTP [§3]

| 文档概念 | 代码位置 |
|---|---|
| word=3、BLOSUM62、T=11、gapopen 11/gapextend 1、window 40 | `Program.vb` blastp 预设 |
| 打分矩阵 BLOSUM45/62/80、PAM250（24×24 含 B/Z/X/*） | `Core/Scoring.vb`（内嵌标准矩阵） |
| SEG 过滤 | `Core/LowComplexity.vb` |
| comp_based_stats | 模式 0=关；模式 1=简化组成校正（查询×命中组成重估 λ）；模式 2/3（条件矩阵调整）未实现，回落为 1 → `Core/KarlinAltschul.vb` — `AdjustedParams` |

### §五 Karlin-Altschul 统计 [§5]

| 公式 | 编号 | 代码位置 |
|---|---|---|
| E = K·m·n·e^(−λS) | [式5-1] | `Core/KarlinAltschul.vb` — `KaParams.EValue` |
| S' = (λS − lnK)/ln2 | [式5-2] | `Core/KarlinAltschul.vb` — `KaParams.BitScore` |
| E = m·n·2^(−S') | [式5-3] | 恒等式，`SelfTest.TestStatsIdentity` 验证 |
| λ 精确解：F(λ)=Σ prob(v)·e^(λv)=1 二分求根 | — | `KarlinAltschul.SolveLambda` |
| H = λ·Σ v·prob(v)·e^(λv) | — | `KarlinAltschul.SolveH` |
| 蛋白矩阵 λ/K | — | `KarlinAltschul.ProteinParams`（内嵌 NCBI 文献表值：BLOSUM62 0.3176/0.1341 等） |
| 核酸 λ/K | — | `KarlinAltschul.NtParams`（λ 数值精确解 + K 以 (2,−3) 文献锚点 0.41 按首达常数比值缩放，启发式） |
| 背景频率 | — | RR 氨基酸频率 `AaBackground`；核酸均匀 0.25 |

## 三、JSON 输出结构

```json
{
  "program": "blastp",
  "task": "blastp",
  "version": "1.0.0",
  "parameters": {
    "word_size": 3, "matrix": "BLOSUM62", "threshold": 11,
    "gap_open": 11, "gap_extend": 1, "evalue_cutoff": 10,
    "two_hit_window": 40, "dust": false, "seg": true,
    "comp_based_stats": 0,
    "lambda": 0.3176, "K": 0.1341, "H": 0.4012,
    "db_sequences": 6, "db_residues": 806
  },
  "queries": [
    {
      "id": "hba_human", "description": "...", "length": 141,
      "hits": [
        {
          "id": "hba_exact", "length": 141,
          "hsps": [
            {
              "score": 315, "bit_score": 150.2, "evalue": 4.1e-44,
              "identities": 141, "positives": 141, "gaps": 0,
              "query_from": 1, "query_to": 141,
              "subject_from": 1, "subject_to": 141,
              "query_frame": 0,
              "query_seq": "MVLSPADK...", "midline": "MVLSPADK...",
              "subject_seq": "MVLSPADK..."
            }
          ]
        }
      ]
    }
  ]
}
```

下游解析：`jq '.queries[0].hits[0].hsps[0].evalue'`、Python `json.load` 等直接可用。
坐标为 1-based 闭区间（与 NCBI 惯例一致）；blastn 仅搜索正向链（frame=1），
反向互补链请自行提供 RC 序列（见"已知边界"）。

## 四、已知边界（如实声明）

| 项 | 现状 |
|---|---|
| 统计参数 λ/K | 核酸 λ 为理想模型精确解；蛋白矩阵内嵌 NCBI 表值。与 NCBI 全套实现（gapped 校正、边缘效应修正）存在偏差，E 值为同量级估计而非逐位对齐 |
| 核酸 K | (2,−3) 锚点为文献值；其他 reward/penalty 组合按首达常数比值缩放（启发式） |
| 反向互补链 | 未实现（文档未要求）；`--strand` 语义请自行提供 RC 查询 |
| comp_based_stats 2/3 | 条件矩阵调整未实现，回落模式 1（JSON 中 `comp_based_stats` 字段如实反映所用模式） |
| DUST/SEG | DUST 为 Morgulis 2006 对称版的滑窗实现；SEG 为窗口熵简化版，非逐位复刻 |
| 规模 | 数据库一次性载入内存；目标为 MB~GB 级 FASTA，百 GB 级请走分块外置方案 |
| gap 代价 | Double 打分（megablast 动态 gap 为 x.5 非整数） |

## 五、验证体系

1. **开发期 Python 镜像对拍**（`_validation/`，不参与交付编译）：
   - λ 求解器：+2/−3 理论精确值 0.633731 逐位一致；
   - X-drop gapped DP vs 暴力全序列 Smith-Waterman：60 随机用例得分一致；
   - traceback 重算得分 == DP 报告得分；无 gap 延伸 == 暴力对角线最大段；
   - X-drop 单调性；两-hit/模板种子行为。
2. **内置自检** `dotnet run -- selftest`：λ 解、E/S' 恒等式、DP vs 内嵌参照 SW、
   traceback 合法性、DUST/SEG 行为、dc 模板命中/不命中、blastn/blastp 端到端冒烟。

## 六、文件清单

```
MiniBlast/
├── MiniBlast.vbproj          net10.0 控制台项目（无 PackageReference）
├── Program.vb                CLI + 任务预设 + JSON 序列化
├── SelfTest.vb               内置自检
├── Core/
│   ├── Fasta.vb              FASTA 读取
│   ├── Alphabet.vb           核酸/氨基酸编码
│   ├── Scoring.vb            打分系统（含内嵌矩阵）
│   ├── LowComplexity.vb      DUST / SEG
│   ├── KarlinAltschul.vb     λ/H/K 参数与 E 值
│   ├── WordLookup.vb         三类 word 查找表
│   └── SeedExtend.vb         两-hit 扫描 + 无 gap / 有 gap X-drop 延伸
├── Model/BlastResult.vb      结构化结果对象（JSON DTO）
└── test/                     测试数据（含同源/突变/低复杂度/无关序列）
```
