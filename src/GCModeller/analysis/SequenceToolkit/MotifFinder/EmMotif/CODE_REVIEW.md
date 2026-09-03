# EmMotif 算法代码审查报告

- **审查基准**：`EM.md`（问题建模 / E 步 / M 步 / 收敛 / 初始化 / OOPS·ZOOPS·ANR / 多 motif / 双链 / E-value）
- **审查对象**：`EmMotif/Core/Alphabet.vb`、`EmModel.vb`、`EmSearch.vb`、`ChiSquare.vb`、`EmMotif/Model/ResultModel.vb`
- **执行方式**：红灯优先 —— 先在 `test/em_test` 写出能命中各缺陷的断言并确认修复前失败，再逐条修复，最后全量转绿
- **结论**：发现 **19 项缺陷**（4 项致命、5 项数值/崩溃、6 项语义、4 项健壮性），已全部修复；自检从 **117 断言 / 43 失败** 变为 **230 断言 / 0 失败**

---

## 一、缺陷总表

| # | 级别 | 位置 | 错误 | 后果 | 修复 | 对应用例 |
|---|---|---|---|---|---|---|
| 1 | **P0** | `EmModel.InitFromSeed` | 列循环变量 `K` 遮蔽字段 `K`（字母表大小），内层 `For a = 0 To K - 1` 按列号遍历 | 第 0 列未归一化；`W > 字母表大小` 时数组越界崩溃 | 列循环改名 `col`，字母表大小一律 `Me.K` | `种子初始化 InitFromSeed` |
| 2 | **P0** | `EmModel.MStep` | 同上，`For a = 0 To k - 1` | 加权计数/伪计数/归一化只覆盖前 col 个字母，其余保留陈旧值 → 列概率和 ≠ 1，PWM 逐轮退化 | 同上 | `M 步加权计数与归一化` |
| 3 | **P0** | `EmModel.Consensus` | `For a = 1 To k - 1` | argmax 只在字母表前 col 个字母内找，DNA 下共识近似全 `A` | 同上 | `一致序列 Consensus` |
| 4 | **P0** | `EmModel.MaxDeltaTo` | `For a = 0 To K - 1` | 收敛判据只比较前 col 个字母，可能提前误判收敛 | 同上 | `PWM 变化量 MaxDeltaTo` |
| 5 | P1 | `Alphabet.New` | `_encode("U"c) = 1`，但 `Letters="ACGT"` 中 T 的索引是 3 | RNA 的 U 被编码成 C | 改为复用 T 的索引 | `字母表与编码` |
| 6 | P1 | `EmModel.WindowLogR` / `Alphabet.Complement` | 负链先取互补再判 `< 0`；`Complement(-1)` 访问 `_compMap(-1)` 越界 | 多 motif 屏蔽（置 −1）+ `--revcomp` 必然抛 `IndexOutOfRangeException` | `Complement` 对越界返回 −1；`WindowLogR` 先判原始编码 | `字母表与编码`、`窗口似然比 WindowLogR` |
| 7 | P1 | `EmModel.FullLogLik` | 用「后验里有没有负链条目」反推是否双链 | EM 首轮后验为空 ⇒ 按单链算，次轮起按双链算；LL 轨迹出现假跳变，破坏单调性保证与 ΔLL 收敛判据 | 改为显式参数 `Optional revcomp As Boolean` | `似然的链模式显式化` |
| 8 | P1 | `EmModel.FullLogLik` OOPS 分支 | 先收集未过滤 −∞ 的候选，随后整段 `lrs.Clear()` 重算 | 死代码 + 逻辑重复 | 统一一次候选收集 | `全似然 vs Oracle` |
| 9 | P1 | `EmModel.EStep` ANR+双链 | 假定候选恒为 (j,+)/(j,−) 成对排列 | 正链候选缺失时会把负链 logR 当正链输出（链向与分值错配） | 改为按位置分组聚合 | `E 步后验 vs Oracle` |
| 10 | P2 | `EmSearch` 跨宽度择优 | 直接比原始对数似然 | 不同 W 的 LL 不可比（ZOOPS 的 ΣR 随 W 放大）⇒ 稳定选中 `maxw` | 新增 `BetterAcrossWidths`：按 E-value（df=(K−1)W 天然惩罚过宽）→ LLR → LL | `宽度范围择优` |
| 11 | P2 | `EmSearch.GenerateSeeds` | `counter` 声明为按列却对所有列 +1，各列恒等于出现次数 | 蛋白（K=20）下所有 k-mer 计数都是 1，排序退化为字典序 = 随机种子；`InitFromCounts` 从未被调用（死代码） | 改真实出现次数 + **Hamming≤1 邻域聚合**做富集度；删除 `InitFromCounts` | `种子初始化策略`、`蛋白种植恢复` |
| 12 | P2 | `Alphabet.New` | `If DNA … Else` 把 `RNA(2)` 与 `Unknown(0)` 一并当蛋白质 | 自动识别模式下 RNA 被当蛋白；未知类型静默降级 | RNA 走核酸语义并归一化 `Kind`；`Unknown` 抛 `ArgumentException` | `字母表与编码` |
| 13 | P2 | `Alphabet.Encode` / `Decode` | `Dim outArr(-1)` 抛异常；`Decode(-1)` 越界 | 空序列/歧义索引崩溃 | 空串返回空数组；`Decode(-1)` 返回 `N`/`X` | `字母表与编码` |
| 14 | P2 | `EmModel.FullLogLik` OOPS | 缺位点位置的均匀先验 `1/nCand` | 报告的 LL 与真实似然差一个随 W 变化的常数 | 补 `-Math.Log(lrs.Count)` | `全似然 vs Oracle` |
| 15 | P2 | `EmSearch.RunEm` | 收敛后重做 E 步，但报告的 `LogLikelihood` 仍是上一轮的值 | 输出 JSON 中 LL 与最终 PWM/位点不自洽 | 收敛后按最终参数重算 LL | `数值回归快照` |
| 16 | P2 | `em_test/Program.RunDiscover` | `motifs.Count = 0` 时 `background_frequencies` 为空；未校验 `--max-iter / --pseudocount` | JSON 缺键；非法参数静默接受 | 背景无条件输出；补充参数校验与使用说明 | `FASTA 端到端与 JSON` |
| 17 | **P1**（新发现） | `EmModel.MStep` ANR λ | 双链下 `nwinTotal *= 2` | ANR 的 E 步/似然让同一位置的正负链共享一个「无位点」状态，槽位数不应翻倍；λ 被低估一半，M 步不再最大化 Q 函数，**实测双链 ANR 的 LL 单轮下降 17.5**，单调性保证失效 | 分母改为候选位置数 | `EM 单调收敛`、`λ 更新` |
| 18 | 基建 | `test/em_test/Program.vb` | 入口是 `Main2`，而 `test/Program.vb` 独占 `Sub Main` | em_test 下全部代码（含自称的「10 组自检」）**从未被执行** | `test/Program.vb` 增加 `em` 分发 | — |
| 19 | 基建 | `test/test.vbproj`、`README.md` | `.fa` 未复制到输出目录；README 文件清单与实际不符 | 端到端用例无法定位数据；文档误导 | 补 `None Update … CopyToOutputDirectory`；校准 README | `FASTA 端到端与 JSON` |

---

## 二、P0 根因详解：VB 大小写无关导致的循环变量遮蔽

这是本次最严重的一类缺陷，**4 处同源**。VB 不区分标识符大小写，因此：

```vb
Public ReadOnly K As Int32        ' 字母表大小（字段）

Public Sub InitFromSeed(seed() As Int32)
    For K As Integer = 0 To W - 1        ' ← 循环变量 K 遮蔽了字段 K
        For a = 0 To K - 1               ' ← 这里的 K 是「列号」，不是字母表大小！
            Pwm(K, a) = Pseudocount
        Next
        ...
```

- 第 0 列：`For a = 0 To -1` 不执行 → `s = 0`，归一化循环同样不执行 → `Pwm(0, seed(0)) = 1 + pc`，**未归一化**
- 第 col 列：只覆盖前 col 个字母；当 `col ≥ 字母表大小` 时 `Pwm(col, a)` 越界

对 DNA（`K=4`）+ `W=10` 的典型配置，`col=5` 时访问 `Pwm(5, 4)` 直接抛 `IndexOutOfRangeException`。
**实测修复前自检在第一组用例即崩溃**，README 声称的「共识恢复 10/10」在 VB 实现上从未成立（`_validation/validate_em.py` 是独立 Python 镜像，验证的是公式而非 VB 代码）。

**修复约定**（已写入代码注释）：列下标一律 `col`，字母表大小一律 `Me.K`，从命名上根除歧义。

---

## 三、红灯留证（修复前 → 修复后）

修复前（`em selftest`，117 断言 / 43 失败）关键输出：

```text
### 字母表与编码
  [FAIL] U 并入 T，两者索引相同 [缺陷 #5] — 期望 <3>，实际 <1>
  [FAIL] Complement(−1) 不抛异常 [缺陷 #6] — 期望 True，实际 False
  [FAIL] RNA 走核酸字母表（大小 = 4）[缺陷 #12] — 期望 <4>，实际 <20>
  [FAIL] Decode(−1) 不抛异常 [缺陷 #13] — 期望 True，实际 False
### 种子初始化 InitFromSeed
  [FAIL] W=10 > 字母表大小 4 时 InitFromSeed 不抛异常 [缺陷 #1] — 期望 True，实际 False
  [FAIL] 种子初始化 InitFromSeed — 用例组异常终止：IndexOutOfRangeException
### M 步加权计数与归一化
  [FAIL] M 步后每列概率和 = 1 [缺陷 #2]（最大列偏差 1）
  [FAIL] 全 T 窗口的加权计数落在 T 列（期望 0.785714）[缺陷 #2]
### 蛋白种植恢复
  [FAIL] 蛋白共识恢复 ≥7/8（实际 1/8）[缺陷 #3]
  [FAIL] 蛋白位点定位 ≥80%（实际 0/25（0.0%））[缺陷 #2]
### EM 单调收敛
  [FAIL] Anr/双链 LL 逐轮单调不降（最大下降 -17.5）[em.md §4]      ← 缺陷 #17
断言总数 117，失败 43
```

修复后（230 断言 / 0 失败）关键输出：

```text
### DNA ZOOPS 种植恢复
         共识 = ACGTTACGTA（匹配 10/10）λ=0.999 LLR=564.6 E=3.55E-96 迭代 20 轮
### 蛋白种植恢复
         共识 = GASTLSKL（匹配 8/8）定位 25/25（100.0%）
### 双链扫描恢复
         共识 = ACGTCGTA（匹配 8/8）定位 18/20（90.0%） 链向正确 18/20
### ANR 多位点
         共识 = TTGACAAT（匹配 8/8）强位点 Z>0.5：77
### 宽度范围择优
         选中宽度 W=8（真实 8，搜索范围 6..14）共识 = ACGTCGTA
### FASTA 端到端与 JSON
         dna.fa motif_1：共识 = ACGTTACGTA（最佳匹配 10/10）
         dna.fa motif_2：共识 = TTGGCCAGGA（最佳匹配 10/10）
断言总数 230，失败 0，用时 8.9s
```

---

## 四、保留的正确设计决策（本次未改动）

审查确认以下实现是对 `EM.md` 的**正确修正**，予以保留：

1. **ZOOPS 后验取序列级混合式** `Z_ij = λR_ij / ((1−λ) + λΣ_j R_ij)`。
   `EM.md §2` 的窗口级式 `Z = λP_m/(λP_m + (1−λ)P_b)` 不满足 `EM.md §6` 自己要求的 `Σ_j Z_ij ≤ 1`（两条好窗口会同时拿到高 Z）。实现取 Bailey & Elkan 1994 的正确式。
2. **λ 更新按模型区分**：OOPS λ≡1；ZOOPS `λ = ΣZ/N`；ANR `λ = ΣZ/位置数`（见缺陷 #17，分母是槽位数而非链数）。
3. **ANR 窗口独立式**似然，与 E 步语义自洽。
4. **双链候选枚举**：`j` 升序、同一位置正链在前负链在后；负链第 col 列读 `enc(j+W−1−col)` 的互补。
5. **E-value 为保守近似**（窗口数 × χ² p 值），`README` 已如实声明未实现 MEME 的精确序统计 E-value。

---

## 五、测试体系

### 目录

```
test/em_test/
├── Program.vb        CLI：discover / selftest [--only <关键字>]
├── SelfTest.vb       编排器：分組调度 + 计时 + 汇总 + 退出码（= 失败断言数）
├── TestAssert.vb     断言原语：Check / CheckEqual / CheckNear(All) / CheckThrows / CheckNoThrow / Guard
├── TestData.vb       确定性数据工厂（每例独立 RNG）+ 位移容忍的匹配度量
├── TestAlphabet.vb   字母表 / 编码 / 歧义 / 互补 / 序列类型
├── TestEmMath.vb     E 步 / M 步 / 似然 / 一致序列 / λ / 单调性（含独立 Oracle）
├── TestEmSearch.vb   种植恢复 / 双链 / ANR / 多 motif / 种子策略 / 宽度 / 边界 / 端到端
├── TestChiSquare.vb  不完全伽马 / χ² / E-value
├── dna.fa            40 条 × 200bp，种植 ACGTTACGTA(25) 与 TTGGCCAGGA(22)
└── protein.fa        30 条 × 150aa，种植 GASTLSKL(16) 与 WYHKRDLN(14)
```

### 运行

```bash
cd test
dotnet run -c Debug -- em selftest                 # 全部用例（约 9s）
dotnet run -c Debug -- em selftest --only 双链      # 只跑名字含「双链」的组
dotnet run -c Debug -- em selftest --only M 步      # 定位单个缺陷
dotnet run -c Debug -- em discover --input em_test\dna.fa --minw 8 --maxw 12 --nmotifs 2 --out motifs.json --pretty
```

（`args(0) = "em"` 由 `test/Program.vb` 分发；不带 `em` 时仍为原有的 Gibbs `findTopN` 冒烟测试。）

### 三个关键手法

1. **独立 Oracle 交叉验证**：`TestEmMath` 内另写一份朴素参考实现（`Oracle*`），直接按 `EM.md` 公式在**线性空间**计算、不复用生产代码路径、循环边界显式使用字母表大小。与生产实现逐元素比对（容差 1e-9）。这是捕获「公式正确但循环边界错位」类缺陷（即本次的 P0）最有效的手段。
2. **`Guard` 包裹每组用例**：单组崩溃记为一条失败而不中断整轮。修复前 `InitFromSeed` 崩溃时仍能看到全部 43 个失败点，而不是一处崩溃掩盖其余。
3. **位移容忍的恢复度量**：EM 找到的窗口寄存器可能与植入位置错开甚至循环移位（如植入 `CAGGTAGCA`、找回 `ACAGGTAGC`），这是 motif 发现的固有现象；因此恢复质量用 `BestShiftedMatch`（允许位移的最佳比对）而非逐位比对。

### 覆盖矩阵

| em.md 章节 | 用例组 |
|---|---|
| §1 建模 / 字母表 | 字母表与编码、蛋白种植恢复 |
| §2 E 步 | E 步后验约束（三模型）、E 步后验 vs Oracle |
| §3 M 步 | M 步加权计数与归一化、λ 更新、伪计数平滑作用 |
| §4 收敛 / 输出 | EM 单调收敛、全似然 vs Oracle、一致序列 Consensus、PWM 变化量 MaxDeltaTo、数值回归快照 |
| §5 初始化 | 种子初始化 InitFromSeed、种子初始化策略 |
| §6 三模型 | DNA ZOOPS / OOPS 种植恢复、ANR 多位点 |
| §7 多 motif | 多 motif 屏蔽重跑 |
| §9 双链 / 宽度 / E-value | 双链扫描恢复、反向互补不变性、宽度范围择优、χ² 生存函数与 E-value |

---

## 六、已知边界与局限（如实声明）

| 项 | 现状 |
|---|---|
| **单 PWM 无法表示两个 motif** | `protein.fa`（30 条序列：16 条 GASTLSKL + 14 条 WYHKRDLN，均无突变）下，覆盖全部 30 条的「混合 PWM」比只覆盖 16 条的干净 motif 具有**更高的似然与 LLR**，因此 EM 会收敛到混合解（`TLSKL`+`DLN`）。这是 EM 单 motif 模型的固有局限（ZOOPS 的 λ→1 使得「多覆盖」总是占优），**不是实现缺陷**。该文件的端到端用例因此只断言「管路跑通 + 至少一个 motif 命中种植 motif 核心」；恢复质量由合成蛋白数据集用例负责（当前 8/8、定位 100%） |
| 背景模型 | order-0 独立字母 + 0.1 伪计数；Markov 高阶未实现 |
| E-value | 保守近似（候选窗口数 × χ² p 值）；未实现 MEME 基于 LLR 词分布序统计的精确 E-value |
| 收敛 | EM 只保证局部最优，靠多种子缓解；MAP-EM（伪计数）下未惩罚似然在收敛点附近允许 ~1e-7 相对量级的抖动，单调性断言按相对容差判定 |
| 种子富集 | Hamming≤1 邻域聚合为 O(不同 k-mer 数 × W × K)；基因组规模输入下该步骤会成为瓶颈 |
| 低复杂度屏蔽 | 未实现 DUST/SEG；富含单一重复的序列可能产生伪 motif |
| 蛋白双链 | 不适用（`--revcomp` 对蛋白自动忽略） |
