---
name: em-motif-review-and-tests
overview: 对 EmMotif 的 EM 算法实现做代码审查（已定位 16 处缺陷，含 4 处致命的循环变量遮蔽），先在 test\em_test 补齐能命中各缺陷的红灯测试，再逐条修复 EmMotif 核心代码直至全绿。
todos:
  - id: wire-test-entry
    content: 改造 test/Program.vb 增加 em 分发，并在 test.vbproj 复制 em_test\*.fa 到输出目录
    status: completed
  - id: build-harness
    content: 用 [subagent:code-explorer] 核对改动 API 的全部调用点，搭建 em_test 测试骨架并跑出基线结果
    status: completed
    dependencies:
      - wire-test-entry
  - id: red-tests-defects
    content: 编写 P0/P1 缺陷的红灯断言（PWM 循环遮蔽、U→T、Complement 越界、LL 首轮、ANR 双链），确认修复前失败
    status: completed
    dependencies:
      - build-harness
  - id: fix-p0-shadowing
    content: 修复 EmModel 中 InitFromSeed/MStep/Consensus/MaxDeltaTo 的 k/K 变量遮蔽缺陷
    status: completed
    dependencies:
      - red-tests-defects
  - id: fix-p1-numeric
    content: 修复 Alphabet U→T 与 Complement 安全、WindowLogR 歧义、FullLogLik 显式 revcomp、OOPS 死代码、ANR 双链配对
    status: completed
    dependencies:
      - fix-p0-shadowing
  - id: add-semantic-tests
    content: 补充 E/M 步公式对照与 oracle 交叉验证、LL 单调、种植恢复、双链、ANR、多 motif、χ²、边界与端到端测试
    status: completed
    dependencies:
      - fix-p1-numeric
  - id: fix-p2-and-report
    content: 修复 P2 缺陷（宽度择优改 E-value、种子计数、空序列、LL 自洽），全量绿灯并输出 CODE_REVIEW.md 与校准 README
    status: completed
    dependencies:
      - add-semantic-tests
---

## 产品概述

对 `EmMotif`（基于 EM 算法的生物序列 motif 发现模块）做一次"文档→实现"的一致性代码审查，找出并修复算法实现中的错误，同时在 `test\em_test` 下建立一套可运行、可复现、覆盖充分的测试体系。

## 核心功能

1. **算法代码审查**：以 `EM.md`（问题建模 / E 步后验 / M 步重估 / 收敛判据 / 初始化策略 / OOPS·ZOOPS·ANR 三模型 / 多 motif 屏蔽 / 双链扫描 / E-value）为基准，逐条比对 `EmMotif\Core\*.vb` 的实现，输出缺陷清单（含严重级别、代码行、错误后果、修复方案）。
2. **测试体系完善**：在 `test\em_test` 中建立分层测试：字母表与编码、E 步三模型后验（与 `EM.md` 公式逐项对照 + 独立 oracle 实现交叉验证）、M 步加权计数与 λ 更新、PWM 归一化、对数似然单调收敛、种植 motif 恢复（DNA/蛋白/双链/ANR 多位点）、多 motif 屏蔽重跑、χ² 与 E-value、边界与异常输入、端到端 CLI 与 JSON 往返。
3. **测试可执行化**：接通测试入口（现有 `em_test` 代码因入口被 Gibbs 冒烟测试占用而从未运行），使 `dotnet run -- em selftest` 能一键跑完全部用例。
4. **缺陷修复与证据链**：采用"红灯优先"——每个缺陷先有断言在修复前失败，修复后转绿，形成"缺陷 → 测试 → 修复 → 验证"闭环。

## 用户已确认的约束

- 先补测试暴露失败，再逐条修复，保留"缺陷→测试"证据链。
- 测试入口：在 `test/Program.vb` 增加命令行分发（`args(0)="em"` 转发到 `EmMotif.Program`），保留 Gibbs 冒烟为默认，不新增独立 vbproj。
- 测试框架：沿用现有手写控制台 runner（`Check(cond, name)` 风格），零第三方 NuGet 依赖。

## 技术栈

- 语言/框架：VB.NET（.NET 10，`net10.0`），`dotnet 10.0.400`，Windows / PowerShell
- 算法库宿主：`MotifFinder.vbproj`（RootNamespace `SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif`）— `EmMotif\**` 由默认 glob 编入该库
- 测试宿主：`test\test.vbproj`（Exe，RootNamespace `test`）— `test\em_test\**` 由默认 glob 编入
- 依赖：仅 .NET BCL + 仓库内既有项目（`Microsoft.VisualBasic.Core` 的 `Microsoft.VisualBasic.Math.log1p`、`Bio.Assembly` 的 `FastaFile/FastaSeq/SeqTypes/BioSequenceValidator`、`System.Text.Json`）
- 测试运行形态：控制台自检（`selftest` 子命令），断言失败计数 + 非零退出码

## 实施策略

### 总体路线（红灯优先，三步闭环）

```
阶段 A 搭骨架 + 跑基线   →  阶段 B 写红灯断言并确认失败  →  阶段 C 逐条修复转绿  →  阶段 D 补语义/边界/端到端
```

每个 P0/P1 缺陷必须满足：**修复前至少 1 条断言失败（红灯留证），修复后全部转绿**。

### 关键决策与权衡

1. **循环变量命名规范（P0 根因）**：VB 大小写无关，`EmModel` 中 `For k/K As Integer = 0 To W - 1` 的循环变量遮蔽了字段 `Public ReadOnly K As Int32`（字母表大小），导致所有内层 `For a = 0 To k - 1` 实际按"列号"遍历。修复不改字段 `K`（避免扩散），而是**将列循环变量改名为 `col`、字母表大小一律写 `Me.K`**，从命名上根除歧义。这比"改成 `Me.K`"更彻底，因为后者仍留下同名遮蔽的陷阱。
2. **测试模块拆分而非堆进 `SelfTest.vb`**：现有 `SelfTest.vb`（475 行）集数据生成、断言、用例于一体，无法按缺陷定位。拆为 `TestAssert`（断言原语）/ `TestData`（确定性数据工厂）/ `TestAlphabet` / `TestEmMath` / `TestEmSearch` / `TestChiSquare` + `SelfTest`（纯编排），便于"一个缺陷一组用例"的证据链。
3. **测试独立性**：现有 `_rng` 为模块级单例，用例间共享随机流导致顺序耦合。改为**每个用例自带 `New Random(fixedSeed)`**，任何用例可单独运行且结果可复现。断言一律用容差（数学恒等式如"列和=1""ΣZ=1"用 1e-9，统计量如共识匹配数用阈值）。
4. **双 oracle 交叉验证**：E 步/M 步/LL 除与 `EM.md` 公式对照外，在测试内另写一份**朴素直算实现**（非对数空间、无 log-sum-exp，仅在 W 小、数值安全时使用），与生产实现逐元素比对（容差 1e-9）。这是捕获"公式正确但实现错位"类缺陷（如本次的循环边界错误）最有效的手段。
5. **不引入 xUnit**：全仓库 0 处引用，且 NuGet 离线风险；控制台 runner 与 `README` 的"仅 .NET BCL"约束一致。
6. **不改算法选型**：ZOOPS 后验取 Bailey & Elkan 1994 的序列级混合式（README 已论证 `EM.md §2` 窗口式不满足 `§6` 的 ΣZ≤1 约束）；ANR 取窗口独立式；λ 更新按模型区分 —— 这些是**正确的设计决策，予以保留**，仅修实现错误。

## 架构设计

### 分层与依赖

```mermaid
flowchart TD
    A[test/Program.vb<br/>Sub Main 入口分发] -->|args(0)=em| B[em_test/Program.vb<br/>Main2: selftest / discover]
    A -->|默认| G[Gibbs findTopN 冒烟]
    B --> C[em_test/SelfTest.vb<br/>用例编排 + 失败汇总]
    C --> D[TestAlphabet / TestEmMath / TestEmSearch / TestChiSquare]
    D --> E[TestAssert 断言原语]
    D --> F[TestData 确定性数据工厂]
    D --> H[(EmMotif.Core<br/>Alphabet/EmModel/EmSearch/ChiSquare)]
    B --> I[(EmMotif.Model<br/>JSON DTO)]
```

### 数据流（被测对象内部）

```
FASTA → Alphabet.Encode → Int32()（-1=歧义）
  → EmSearch.GenerateSeeds(W)      [EM.md §5 初始化]
  → 逐种子 EmModel.InitFromSeed    [§5 one-hot+伪计数]
  → 循环 { EStep(Z_ij) → MStep(θ,λ) → FullLogLik }  [§2/§3/§4]
  → SoftLlr → ChiSquare.MotifEValue → 择优(E-value/LLR)
  → MaskSites(Z>0.5) → 下一 motif  [§7]
  → Program 组装 MotifReport → System.Text.Json
```

## 实施要点（Execution Notes）

### 缺陷清单（已定位，须逐条落测试）

**P0 致命 —— 循环变量 `k/K` 遮蔽字母表大小字段 `K`（VB 大小写无关）**

| # | 位置 | 错误 | 后果 |
| --- | --- | --- | --- |
| 1 | `EmModel.vb:74-88 InitFromSeed` | 内层 `For a = 0 To K - 1` 中 K=列号 | 第 0 列循环体不执行（`s=0` 归一化跳过，`Pwm(0,seed(0))=1+pc` 未归一化）；第 col 列只覆盖前 col 个字母 |
| 2 | `EmModel.vb:245-254 MStep` | `For a = 0 To k - 1` | 加权计数/伪计数/归一化只覆盖前 col 个字母，其余保留陈旧值 → 每列概率和≠1，PWM 迭代退化 |
| 3 | `EmModel.vb:395-405 Consensus` | `For a = 1 To k - 1` | argmax 只在字母表前 col 个字母内找；col=0/1 恒返回 'A' → DNA 共识近似全 A |
| 4 | `EmModel.vb:408-417 MaxDeltaTo` | `For a = 0 To K - 1` | 收敛判据只比较前 col 个字母，可能提前误判收敛 |


**P1 数值/崩溃**

| # | 位置 | 错误 |
| --- | --- | --- |
| 5 | `Alphabet.vb:45` | `_encode("U"c)=1`，但 `Letters="ACGT"`，T 索引为 3 → RNA 的 U 被编成 C（注释与代码自相矛盾） |
| 6 | `EmModel.vb:209` | 负链先算 `Complement(enc(...))` 再判 `<0`；`Complement(-1)` → `_compMap(-1)` → `IndexOutOfRangeException`。`MaskSites` 置 −1 且 `--revcomp` 时必崩 |
| 7 | `EmModel.vb:295` | `FullLogLik` 用 `HasMinus(sitesList)` 反推链模式；`RunEm` 首轮 sitesList 全空 ⇒ revcomp=False、次轮 True ⇒ LL 轨迹首轮"模型切换"跳变，单调性与 ΔLL 收敛失真。改为显式参数 |
| 8 | `EmModel.vb:303-318` | OOPS 分支 `lrs.Add(WindowLogR(...,True))` 未过滤 −∞ 后又被 `lrs.Clear()` 全量重算 → 死代码 + 逻辑重复 |
| 9 | `EmModel.vb:164-183` | ANR+revcomp 假定候选恒 (j,+)/(j,−) 成对；正链窗口因歧义被排除时，`logRf` 取到负链值却以 `StrandMinus=False` 输出 → 链向与分值错配（需先修 #6 才显现） |


**P2 语义/健壮性**

| # | 位置 | 错误 |
| --- | --- | --- |
| 10 | `EmSearch.vb:79-83,101-106` | 跨宽度按原始 LL 择优；不同 W 的 LL 不可比（ZOOPS 的 ΣR 随 W 放大，OOPS 还缺 1/nwin 常数）→ 系统性偏向 maxw。改 E-value（df=(K−1)W 已含宽度惩罚）优先、LLR 次之（`README` 第 12 行与第 48 行本身就矛盾） |
| 11 | `EmSearch.vb:213-224` | `counter(useKey)=New Double(w-1){}` 却对所有列 +1 → 各列恒为出现次数，非注释所称列字母计数；`InitFromCounts`（期望 `[w,K]`）从未被调用 → 死代码 |
| 12 | `Alphabet.vb:32-38` | `If DNA … Else` 把 `SeqTypes.RNA(2)` 与 `Unknown(0)` 一并当蛋白质（`SeqTypes.vb:64-81`） |
| 13 | `Alphabet.vb:60-66` | 空序列 → `Dim outArr(-1)` 抛异常 |
| 14 | `EmModel.vb:298-327` | OOPS 缺 `log(1/nwin)` 常数项（不影响 argmax，但报告 LL 与真实似然差常数，跨宽度比较不可忽略） |
| 15 | `EmSearch.vb:301-306` | 退出后重做最终 E 步，但报告 `LogLikelihood` 是上一轮值，与最终 PWM/位点不自洽 |
| 16 | `em_test/Program.vb:164-180` | `motifs.Count=0` 时 `background_frequencies` 为空；未校验 minw 与序列长度下界 |


**测试基础设施（必改）**

| # | 问题 |
| --- | --- |
| 17 | `em_test/Program.vb` 入口是 `Main2`，`test/Program.vb` 独占 `Sub Main` ⇒ em_test 全部代码（含自称"10 组自检"）从未执行 |
| 18 | `test/test.vbproj` 未复制 `em_test\*.fa` ⇒ 端到端无法定位数据文件 |
| 19 | `EmMotif/README.md` 文件清单与实际不符（声称有 `EmMotif.vbproj / Program.vb / SelfTest.vb / Core\FastaIO.vb`，实际均无） |


### 执行注意

- **先编译再断言**：阶段 A 必须先 `dotnet build test\test.vbproj` 确认基线可编译（已核实 `log1p`、`FastaFile.Read(Path=String)`、`FastaFile : IList(Of FastaSeq)`、`FastaSeq.locus_tag/SequenceData/Length`、`BioSequenceValidator.IdentifySequence`、`SeqTypes` 均可用，预期可编过）。
- **变量遮蔽是编译期合法的**：`For k As Integer` 遮蔽字段在 VB 中不会报错，只会静默出错 —— 因此**必须靠断言而非编译告警发现**，这也是"红灯优先"的必要性。
- **测试文件加 `Option Strict On`**（仅新文件），避免隐式窄化掩盖类型错误；若触发历史代码报错则退回该文件的默认值。
- **LLM 不得改算法选型**：ZOOPS 序列级后验、ANR 窗口独立式、λ 按模型区分均正确，只修实现。
- **回归保护**：修 #2（M 步）会显著改变所有下游数值结果；修复后需重跑全部用例，确认"共识恢复/位点定位"从"全 A/随机"变为"≥9/10 & ≥80%"，以证明修复有效而非掩盖。
- **性能**：E 步为热路径（O(序列总长 × W × 种子数 × 迭代数)）。测试数据集控制在 20~30 条 × 150~300 bp、种子数 ≤ 10、迭代 ≤ 150，保证自检 30s 内完成；`enriched` 策略的 k-mer 字典在 W=12/L=300/N=30 下约 9k 条，无压力。
- **日志**：自检输出沿用 `[PASS]/[FAIL]` + 分组标题，失败时打印期望值/实际值/容差，不打印整条序列；汇总行输出 `N TEST(S) FAILED`，进程退出码 = 失败数（上限 125）以便 CI 判红。

## 目录结构

```
MotifFinder/
├── EmMotif/
│   ├── Core/
│   │   ├── Alphabet.vb          # [MODIFY] 修 #5（U→T 索引 3）、#6（Complement 对 a<0 返回 −1 而非越界）、
│   │   │                        #          #12（显式分支 DNA/RNA→DNA 语义、Protein、Unknown 报错）、#13（空串返回空数组）
│   │   ├── EmModel.vb           # [MODIFY] 修 #1-#4（列循环改名 col，字母表大小统一 Me.K）、#6（WindowLogR 先判 a<0
│   │   │                        #          再取互补）、#7（FullLogLik 增加显式 revcomp 参数）、#8（删 OOPS 死代码）、
│   │   │                        #          #9（ANR 双链改为按位置聚合 fwd/rev，不再假定成对）、#14（OOPS 补 log(1/nwin)）
│   │   ├── EmSearch.vb          # [MODIFY] 修 #10（跨宽度择优改 E-value→LLR→LL 三级）、#11（k-mer 列计数改真实列字母
│   │   │                        #          计数并启用 InitFromCounts，或删除死代码）、#15（报告 LL 与最终 PWM 自洽）
│   │   └── ChiSquare.vb         # [MODIFY] 仅补注释与边界（x<=0 / 超大 df）；公式经验证正确，不改数值路径
│   ├── Model/
│   │   └── ResultModel.vb       # [MODIFY] 视 #16 需要补字段可空性与默认值，保证 motifs 为空时 JSON 仍合法
│   ├── README.md                # [MODIFY] 校准文件清单（#19）、补"缺陷审查与修复记录"章节、如实更新验证结论
│   └── CODE_REVIEW.md           # [NEW] 审查报告：缺陷表（级别/位置/错误/后果/修复/对应测试用例名）、
│                                #        EM.md 章节→代码→测试三方映射、红灯留证输出片段、已知边界
└── test/
    ├── Program.vb               # [MODIFY] 入口分发：args(0)="em" → EmMotif.Program.Main2(args.Skip(1))；
    │                            #          其余保持 Gibbs findTopN 冒烟不变
    ├── test.vbproj              # [MODIFY] 增 <None Include="em_test\*.fa" CopyToOutputDirectory="PreserveNewest" />
    └── em_test/
        ├── Program.vb           # [MODIFY] Main2 保持 discover/selftest CLI；修 #16（无 motif 时背景仍输出、参数校验）；
        │                        #          新增 --only <关键字> 过滤，便于单组复跑
        ├── SelfTest.vb          # [MODIFY] 改为纯编排器：分组调用下列用例模块，统一计数/计时/汇总/退出码；
        │                        #          原有 9 组用例迁移到对应模块
        ├── TestAssert.vb        # [NEW] 断言原语：Check / CheckNear / CheckThrows / CheckNoThrow / Section，
        │                        #          失败统一打印 期望/实际/容差，进程级失败计数
        ├── TestData.vb          # [NEW] 确定性数据工厂：MakeRng(seed) 每例独立；PlantDna/PlantProtein/PlantRevcomp/
        │                        #          PlantAnr（返回序列+真值位置/链向）；BgOf；手工 PWM 构造；常量字母表
        ├── TestAlphabet.vb      # [NEW] 编码/解码往返、U→T(#5)、歧义→−1、Complement(-1)=−1 不抛(#6)、
        │                        #          Revcomp 正确性、蛋白字母表、空序列(#13)、RNA/Unknown 分支(#12)
        ├── TestEmMath.vb        # [NEW] 核心：InitFromSeed 列和=1 且 one-hot 峰值正确(#1)；MStep 列和=1 且等于
        │                        #          独立 oracle 加权计数(#2)；Consensus = 逐列 argmax(#3)；MaxDeltaTo 全格比较(#4)；
        │                        #          WindowLogR 对照直算 Π θ/θ0、歧义→−∞、负链=revcomp 窗口、含 −1 不抛(#6)；
        │                        #          E 步三模型后验对照 EM.md §2/§6 + 独立 oracle（OOPS ΣZ=1、ZOOPS ΣZ≤1、
        │                        #          ANR 独立 + 双链链向正确 #9）；λ 更新按模型；FullLogLik 三模型显式 revcomp(#7)；
        │                        #          LL 单调不降（三模型 × 单/双链，容差 1e-6）
        ├── TestEmSearch.vb      # [NEW] 种植恢复（DNA ZOOPS/OOPS、蛋白、双链链向、ANR 多位点）；种子策略
        │                        #          enriched/random/all 可用性与确定性；同 seed 结果可复现；多 motif 屏蔽互异(#7)；
        │                        #          宽度择优不偏向 maxw(#10)；端到端读 dna.fa/protein.fa + JSON 往返；
        │                        #          边界（序列短于 W、全歧义、单条序列、W=2）
        ├── TestChiSquare.vb     # [NEW] χ² sf 对文献分位数（df=1,2,4,10 @0.05/0.01，容差 5e-4）；
        │                        #          E-value 随 LLR 单调递减、随窗口数单调递增；极端输入不 NaN/Inf
        ├── dna.fa               # [KEEP] 端到端输入（种植 motif 的合成 DNA）
        └── protein.fa           # [KEEP] 端到端输入（种植 motif 的合成蛋白）
```

## 关键代码结构

```
' test/em_test/TestAssert.vb — 断言原语（新建文件建议 Option Strict On）
Public Module TestAssert
    Sub Check(cond As Boolean, name As String)
    Sub CheckNear(actual As Double, expected As Double, tol As Double, name As String)
    Sub CheckThrows(Of TEx As Exception)(action As Action, name As String)
    Sub CheckNoThrow(action As Action, name As String)
    Sub Section(title As String)
    Function Failures() As Integer
End Module

' test/em_test/TestData.vb — 确定性数据工厂（每例独立 RNG，保证可单跑、可复现）
Public Module TestData
    Function MakeRng(seed As Integer) As Random
    Function Plant(seqCount%, seqLen%, motif$, mutationRate#, withSiteRatio#,
                   seed%, revcompFraction#,
                   <Out> ByRef truth As List(Of SiteTruth)) As List(Of String)
    Function BgOf(encs As List(Of Integer()), alpha As Alphabet, Optional pc# = 0.1) As Double()
End Module

' EmMotif/Core/EmModel.vb — 修复后的签名约定（列循环一律 col，字母表大小一律 Me.K）
'   Public Sub InitFromSeed(seed() As Int32)
'   Public Sub InitFromCounts(counts As Double(,))      ' [w, Me.K]
'   Public Sub MStep(encList, sitesList, Optional revcomp As Boolean = False)
'   Public Function FullLogLik(encList, sitesList, Optional revcomp As Boolean = False) As Double
'   Public Function Alphabet.Complement(a As Int32) As Int32   ' a < 0 → 返回 −1（不再越界）
```

## Agent Extensions

### SubAgent

- **code-explorer**
- 用途：在动手修改前，全仓核对 `EmModel.Pwm/InitFromSeed/MStep/Consensus/MaxDeltaTo`、`Alphabet.Complement/Encode`、`EmSearch.Discover/RunEm` 的全部调用点（库内 + `test` + 其他分析项目），确认修复签名（如 `FullLogLik` 增加 `revcomp` 参数）不会遗漏调用方、不会有隐藏的重载/晚期绑定依赖。
- 预期产出：调用点清单与影响面结论，作为"零回归"改动边界的依据。