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
```

## 内置自检

自检位于独立的 `test` 工程，走与 CLI 完全相同的代码路径
（`TaskPresets` → `BlastSearch` → `BlastReportJson`）：

```bash
cd test && dotnet run
```

覆盖四层：单元自检（λ 解、E/BitScore 恒等式、gapped DP vs 参照 SW、
traceback 合法性、DUST/SEG、dc 模板种子）→ 扫描器延伸触发回归 →
基于 `test/*.fa` 的 blastn / megablast / dc-megablast / blastn-short /
blastp / blastp-short 端到端检索 → 每条 HSP 的结构不变量校验与 JSON 导出回读。

退出码 = 失败用例数（0 表示全绿，可直接接入 CI）。
各任务的比对报告导出到 `test/bin/Debug/net10.0/selftest_results/*.json`。

## 二、README 文档 → 代码映射表

### §一 BLAST 核心流程（五阶段）

| 文档概念 | 公式/条件 | 代码位置 |
|---|---|---|
| 流程编排（过滤→查表→扫描→延伸→统计） | — | `Core/BlastEngine.vb` — `BuildDatabase` / `RunQuery` |
| [阶段1] DUST 低复杂度过滤（核酸） | S = Σ_v C_v(C_v−1)/2 / (W−1) > level/10，W=64, word=3 | `Core/LowComplexity/Dust.vb` — `Dust.Mask`（滑窗 O(L) 增量实现） |
| [阶段1] SEG 低复杂度过滤（蛋白） | 香农熵 H = −Σ p·log₂p < 2.2 触发，≥2.5 终止，W=12 | `Core/LowComplexity/SegFilter.vb` — `SegFilter.Mask` |
| [阶段2] BLASTN word 查找表 | 连续 word 精确匹配，base-4 打包 Long 键 | `Core/WordLookup/NtWordLookup.vb`（滚动编码） |
| [阶段2] dc-megablast 模板种子 | 11/18 模板 coding=`101101100101101101`，optimal=`111010010110010111`，只编码 care 位 | `Core/WordLookup/DcWordLookup.vb`（按窗口起点整窗重算键） |
| [阶段2] BLASTP 邻域 word | 查询 word 与 db word 比对得分 ≥ T（默认 11） | `Core/WordLookup/AaWordLookup.vb` — `ExpandNeighborhood`（按列最大得分上界剪枝的递归枚举） |
| [阶段3] 两-hit 法 | 同对角线 diag=i−j，两命中非重叠（d ≥ W）且距离 ≤ A=40；**重叠命中不覆盖 lastHit**，超窗 A 则重置 | `Core/SeedExtend.vb` — `SeedScanner.ScanSequence`（每对角线维护 lastHit 与已延伸区 lastTrig） |
| [阶段4] 无 gap X-drop 延伸 | 双侧行走，得分相对该侧最优下降 > X_ungap（20 bits）即停；最优段必含种子 | `Core/SeedExtend.vb` — `UngappedExtend` |
| [阶段5] 有 gap X-drop 延伸 | 仿射间隙 SW 变体：E=max(H↑−(go+ge), E↑−ge)，F=max(H←−(go+ge), F←−ge)，H=max(diag, E, F)；反对角线迭代 + X-drop 剪枝；双向合并 | `Core/SeedExtend.vb` — `GappedForward`（每格记录 H/E/F 三状态回溯方向）+ `GappedExtend`（正反向合并）+ `TracebackMoves`（状态机回溯） |
| 两级 gapped（预延伸 Xg → 最终延伸） | 预延伸 X=30 bits 定坐标 → 最终延伸 X=100 bits（blastp 25）重比对出串；最终延伸退化时以**带 traceback** 的预延伸回退 | `Core/SeedExtend.vb` — `ScanSequence` 中两次 `GappedExtend` 调用 |
| HSP 一致性兜底 | 比对串 ↔ 坐标 ↔ 源序列三者不符时丢弃该 HSP | `Core/SeedExtend.vb` — `AlignMatchesSequence` |
| gapped 触发阈值 | E=K·m·n·e^(−λS) ≤ E_pass ⇒ S ≥ ln(K·m·n / E_pass)/λ，E_pass = EvalueCutoff × 0.001 | `Core/BlastEngine/BlastEngine.vb` — `GappedTriggerMargin` |
| 搜索编排 / 参数预设 / 结果导出 | 读 FASTA → 建库 → 逐查询 → 组装报告 → JSON | `Core/BlastSearch.vb`、`Options/TaskPresets.vb`、`Model/BlastReportJson.vb`（CLI 与自检共用） |

### §二 BLASTN 任务预设 [§2.1 表]

| 任务 | Word | Reward/penalty | Gap open/extend | 代码位置 |
|---|---|---|---|---|
| megablast | 28 | +1/−2 | 0 / 动态：[式2-1] \|2·penalty−reward\|/2（例：1/−5 → 5.5，**保留 Double 不取整**） | `Options/TaskPresets.vb` — `Apply` / `MegablastGapExtend` |
| dc-megablast | 11/18 非连续 | +2/−3 | 5/2 | `Options/TaskPresets.vb` + `Core/WordLookup/DcWordLookup.vb` |
| blastn | 11 | +2/−3 | 5/2 | `Options/TaskPresets.vb` |
| blastn-short | 7 | +1/−3 | 5/2，dust 关 | `Options/TaskPresets.vb` |

### §三 BLASTP [§3]

| 文档概念 | 代码位置 |
|---|---|
| word=3、BLOSUM62、T=11、gapopen 11/gapextend 1、window 40、xdrop_gap_final 25 | `Options/TaskPresets.vb` blastp 预设 |
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
| gap 代价 | Double 打分（megablast 动态 gap 为 x.5 非整数）。**约定与 NCBI 一致**：长度 k 的 gap 代价 = −(a + b·k)，即首个 gap 残基扣 `gapopen + gapextend`，其后每个残基扣 `gapextend`；故长度 1 的 gap 代价为 −(a+b)（见 <https://www.ncbi.nlm.nih.gov/blast/html/gaplambda.html>）。megablast 的 `gapopen=0` 在此约定下即线性 gap，不会出现"长度 1 的 gap 免费" |

## 五、验证体系

1. **开发期 Python 镜像对拍**（`_validation/`，不参与交付编译）：
   - λ 求解器：+2/−3 理论精确值 0.633731 逐位一致；
   - X-drop gapped DP vs 暴力全序列 Smith-Waterman：60 随机用例得分一致；
   - traceback 重算得分 == DP 报告得分；无 gap 延伸 == 暴力对角线最大段；
   - X-drop 单调性；两-hit/模板种子行为。
2. **内置自检** `cd test && dotnet run`（退出码 = 失败数，共 99 项检查）：
   - 单元自检：λ 解、E/S' 恒等式、gapped DP vs 内嵌参照 SW、traceback 重算合法性、
     DUST/SEG 行为、dc 模板命中/不命中 + 窗口滑动建表键一致性；
   - 延伸触发回归：精确自匹配与局部同源必须触发（两-hit 死锁的回归锁）；
   - 端到端：以 `test/*.fa` 跑 blastn / megablast / dc-megablast / blastn-short /
     blastp / blastp(+comp-based-stats) / blastp-short，断言各任务的敏感度边界
     （如 megablast 找回 5% 突变但不找回 25% 分歧）与反例零召回；
   - 结构不变量：每条 HSP 校验「比对串 ↔ 坐标 ↔ 源序列」三者一致、midline 与
     identities/positives/gaps 计数一致、重算 raw score == 报告 score、
     bit_score/evalue 满足式 5-2 / 5-1；
   - 导出链路：7 份 JSON 报告落盘后回读，逐字段比对。

## 六、文件清单

```
MiniBlast/
├── MiniBlast.vbproj          net10.0 控制台项目（无 PackageReference）
├── Program.vb                CLI 外壳（参数解析 → 委托下方三个公共 API）
├── Options/
│   ├── BlastOptions.vb       最终搜索参数
│   ├── SeedExtendOptions.vb  扫描/延伸参数
│   └── TaskPresets.vb        任务预设唯一来源（CLI 与自检共用）
├── Core/
│   ├── Alphabet/             核酸/氨基酸编码 + 打分器（含内嵌矩阵）
│   ├── LowComplexity/        DUST（核酸）/ SEG（蛋白）
│   ├── WordLookup/           NtWordLookup / AaWordLookup / DcWordLookup
│   ├── BlastEngine/          BlastEngine（单查询编排） / DbEntry·DbStatistics·BlastDb
│   ├── BlastSearch.vb        搜索编排（读 FASTA→建库→多查询→组装报告）
│   ├── KarlinAltschul.vb     λ/H/K 参数与 E 值
│   ├── KaParams.vb           E/BitScore 公式
│   ├── Data.vb / RawHsp.vb   中间数据结构
│   └── SeedExtend.vb         两-hit 扫描 + 无 gap / 有 gap X-drop 延伸
├── Model/                    结构化结果对象（JSON DTO）
│   ├── BlastReport.vb / BlastParameters.vb / QueryResult.vb / Hit.vb / Hsp.vb
│   └── BlastReportJson.vb    序列化 / 落盘 / 回读
├── _validation/              开发期 Python 镜像对拍（不参与编译）
└── test/                     独立自检工程
    ├── test.vbproj           net10.0 控制台（cd test && dotnet run）
    ├── SelfTest.vb           四层自检（单元 / 触发回归 / 端到端 / 不变量+导出）
    ├── nt_query.fa / nt_db.fa    核酸：精确副本 / 5% 突变 / 25% 分歧 / poly-A / 无关
    └── aa_query.fa / aa_db.fa    蛋白：精确副本 / 12% 突变 / 旁系同源 / 泛素 / 溶菌酶 / 随机
```
