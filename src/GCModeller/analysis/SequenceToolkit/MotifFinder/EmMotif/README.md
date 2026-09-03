# EmMotif — EM 算法 motif 发现（MEME 三种位点分布模型）

VB.NET (.NET 10) 命令行项目，**零第三方依赖**（仅 .NET BCL，JSON 用 System.Text.Json）。
按《通过 EM 方法发现 Motif：原理与流程详解》文档实现：把 motif 位置作为隐藏变量，
交替执行 E 步（估计位点后验）与 M 步（加权计数重估 PWM），迭代至收敛；
支持 **OOPS / ZOOPS / ANR** 三种位点分布模型、**核酸与氨基酸**双字母表、
双链扫描、多 motif 屏蔽重跑、宽度范围搜索，输出结构化 JSON。

> **代码审查**：2026-09 对 `Core/*.vb` 做了一次完整的「文档 → 实现」一致性审查，
> 发现并修复 19 项缺陷（含 4 项致命的循环变量遮蔽、双链 λ 归一化错误导致 EM 单调性失效等）。
> 详见 [`CODE_REVIEW.md`](CODE_REVIEW.md)。

## 一、构建与运行

算法代码随 `MotifFinder.vbproj` 编译；命令行入口与自检位于 `test/em_test`。
`test/Program.vb` 的 `Main` 负责分发：首参为 `em` 时转交 EmMotif，其余情况保持原有的
Gibbs `findTopN` 冒烟测试。

```bash
cd test
dotnet build -c Release

# DNA（默认 zoops；自动识别字母表）
dotnet run -c Release -- em discover --input em_test/dna.fa --model zoops --minw 8 --maxw 12 --nmotifs 2 --out motifs.json --pretty

# 氨基酸序列
dotnet run -c Release -- em discover --input em_test/protein.fa --alphabet protein --model zoops --minw 8 --maxw 10 --out prot.json

# 双链扫描 + 多 motif
dotnet run -c Release -- em discover --input em_test/dna.fa --model oops --minw 10 --maxw 10 --nmotifs 2 --revcomp --out rc.json

# 自检（24 组用例 / 230 条断言，约 9s；退出码 = 失败断言数）
dotnet run -c Release -- em selftest
dotnet run -c Release -- em selftest --only 双链      # 只跑用例组名含「双链」的组
```

## 二、em.md 文档 → 代码映射表

| 文档概念 | 公式/约定 | 代码位置 |
|---|---|---|
| 二分量混合模型（PWM θ + 背景 θ0） | [§1] | `Core/EmModel.vb`（Pwm / Background） |
| 隐藏变量 Z_ij | [§1] | `SitePosterior`（E 步输出） |
| E 步：窗口似然比 R_ij = Π θ_k,a/θ0,a | [§2] | `EmModel.WindowLogR`（对数空间累加；歧义字母 → −∞ 排除） |
| OOPS 后验（Σ_j Z_ij = 1 精确） | [§6 约束] | `EmModel.EStep` — Oops 分支（softmax，背景项相消） |
| ZOOPS 后验（Σ_j Z_ij ≤ 1） | [§6] | `EStep` — Zoops 分支：Z = λR/((1−λ)+λΣR)，log-sum-exp 稳定化 |
| ANR 后验（窗口独立） | [§2 窗口式] | `EStep` — Anr 分支：Z = λR/(λR+1−λ)（logistic 形式） |
| M 步：n_{k,a} = Σ Z·1[S=a]；伪计数 0.1–1 | [§3 Step1-2] | `EmModel.MStep` |
| λ 更新 | [§3 Step3] | OOPS λ≡1；ZOOPS λ = ΣZ/N；ANR λ = ΣZ/**候选位置数**（同一位置的正负链共享一个「无位点」状态，故槽位数不随链数翻倍） |
| EM 迭代至 ΔLL < ε 收敛 | [§4] | `EmSearch.RunEm`（--epsilon，默认 1e-4） |
| LL 单调不降（EM 保证） | [§4] | `FullLogLik` + selftest 断言 |
| 一致序列输出 | [§4] | `EmModel.Consensus` |
| 种子初始化（one-hot + 伪计数） | [§5] | `EmModel.InitFromSeed` |
| 三种初始化策略（穷举/抽样/K-mer 富集/一致性） | [§5 表] | `EmSearch.GenerateSeeds`（enriched=K-mer 富集（Hamming≤1 邻域聚合）/ random=随机窗口 / all=全去重 W-mer 上限 maxSeeds） |
| 多 motif：屏蔽 + 重跑 | [§7] | `EmSearch.MaskSites`（Z>0.5 窗口置歧义）→ `Discover` 循环 |
| 停止条件：nmotifs / E-value 阈值 | [§7] | `SearchOptions.EvalueMax`（默认 10） |
| 收敛后验分布多模态 → 多种子重启 | [§8] | 逐种子 EM 取最优 LL |
| -revcomp 双链 | [§9] | `EStep` 双链候选 (j,+)/(j,−)；负链第 k 列 = enc(j+W−1−k) 互补 |
| -minw/-maxw 宽度范围 | [§9] | `EmSearch.Discover`（逐宽度全流程；跨宽度按 **E-value** 择优，同宽度内按 LL 择优） |
| E-value 与 LLR（E<0.05 显著） | [§9] | `Core/ChiSquare.vb`：LLR = 2ΣZ·logR，χ²(df=(K−1)W) 生存函数（不完全伽马，文献分位数验证 ≤5e-4），E = 候选窗口总数 × p（保守近似） |
| Gibbs 对照（EM vs 随机硬指派） | [§8] | 未实现 Gibbs（文档定位为对照方法），ZOOPS/ANR 的窗口独立性语义与之一致 |

### 对文档的三处修正（根本逻辑，非表面）

1. **ZOOPS E 步公式**：文档 §2 给出窗口级 `Z=λP_m/(λP_m+(1−λ)P_b)`，但这不满足文档 §6 自己要求的
   约束 Σ_j Z_ij ≤ 1（两条好窗口会同时拿到高 Z）。正确后验（Bailey & Elkan 1994，MEME 原文）是在
   「无位点状态 vs 全部候选位点」之间竞争：`Z_ij = λR_ij/((1−λ)+λΣ_j R_ij)`。实现取正确式。
2. **λ 更新的模型区分**：文档 §3 Step3 的 `λ = ΣZ/Σ(L−W+1)` 只适用于 ANR（每位点独立先验）；
   ZOOPS 的 MLE 应为 `λ = Σ_ij Z_ij/N`（每序列至多一个位点，期望含位点序列数/N）。实现按模型区分。
3. **跨宽度择优必须比 E-value，不能比对数似然**（文档 §9 未明确）：不同 W 的 LL 不可比 ——
   ZOOPS 的 ΣR 随 W 增大被逐列放大，OOPS 还差一个随 W 变化的 `log(1/nCand)` 常数项。
   直接按 LL 择优会稳定选中 `maxw`。实现按 `E-value`（df=(K−1)W 随 W 增大，天然惩罚过宽）
   → LLR → LL 三级比较；同一宽度内不同种子之间仍按 LL 比较（MEME 的做法）。

## 三、JSON 输出结构

```json
{
  "program": "EmMotif", "version": "1.0.0", "alphabet": "dna",
  "parameters": { "model": "zoops", "min_width": 8, "max_width": 12, "num_motifs": 2,
                   "revcomp": false, "seed_strategy": "enriched", "epsilon": 0.0001, "..." : 0 },
  "sequences": [ { "id": "dna_seq01", "length": 200, "ambiguous_positions": 0 } ],
  "background_frequencies": { "A": 0.25, "C": 0.25, "G": 0.25, "T": 0.25 },
  "motifs": [
    { "id": "motif_1", "width": 10, "model": "zoops", "consensus": "ACGTTACGTA",
      "lambda": 0.918, "log_likelihood": -8019.8, "log_likelihood_ratio": 168.4,
      "evalue": 3.2e-11, "iterations": 11, "converged": true,
      "letters": "ACGT",
      "pwm": { "A": [0.79, 0.1, ...], "C": [...], "G": [...], "T": [...] },
      "background": { "A": 0.25, "..." : 0 },
      "sites": [ { "sequence": "dna_seq01", "start": 57, "strand": "+",
                    "posterior": 0.98, "log_likelihood_ratio": 21.3, "segment": "ACGTTACGTA" } ],
      "log_likelihood_trace": [-8067.5, -8030.1, ...] } ]
}
```

位点坐标 1-based；负链位点给出反向互补后的 `segment`，`strand: "-"`。

## 四、验证体系

1. **Python 公式级镜像**（`_validation/validate_em.py`）：仅验证 E 步/M 步/似然的**公式**是否正确，
   与 VB 实现无代码共享。注意它**不能**替代对 VB 代码的验证 —— 2026-09 的审查发现 VB 侧存在
   4 处致命的循环边界错误，而这些在 Python 镜像里并不存在（Python 无变量遮蔽问题）。
2. **内置自检** `em selftest`（24 组 / 230 条断言，约 9s，退出码 = 失败断言数）：

   | 手法 | 说明 |
   |---|---|
   | **独立 Oracle 交叉验证** | 测试内另写一份朴素参考实现（`TestEmMath.Oracle*`），直接按公式在**线性空间**计算、不复用生产代码路径，与生产实现逐元素比对（1e-9）。这是捕获「公式正确但循环边界错位」类缺陷最有效的手段 |
   | **崩溃隔离** | 每组用例用 `TestAssert.Guard` 包裹，单组崩溃记为一条失败而不中断整轮 |
   | **位移容忍的恢复度量** | EM 找到的窗口寄存器可能与植入位置错开甚至循环移位，故用 `BestShiftedMatch` 而非逐位比对 |
   | **确定性数据** | 每个用例自带固定种子的 RNG，任何用例可单独运行且结果可复现 |

   覆盖：字母表与编码 / 种子初始化 / M 步加权计数 / 一致序列 / PWM 变化量 / 窗口似然比 /
   E 步三模型约束与 Oracle 对拍 / 似然链模式显式化 / 全似然 Oracle 对拍 / EM 单调收敛 /
   λ 更新 / 伪计数平滑 / 反向互补不变性 / χ² 与 E-value / DNA ZOOPS·OOPS 恢复 / 蛋白恢复 /
   双链恢复 / ANR 多位点 / 多 motif 屏蔽 / 种子策略 / 可复现性 / 宽度择优 / 边界输入 /
   FASTA 端到端与 JSON 往返 / 数值回归快照。

   当前结果：`断言总数 230，失败 0，用时 8.9s`。种植恢复实测：
   DNA ZOOPS 共识 10/10、定位 25/25（100%）；蛋白 8/8、定位 25/25（100%）；
   双链 8/8、链向判读 18/20（90%）；ANR 8/8；`dna.fa` 两个种植 motif 均 10/10 恢复。

## 五、已知边界（如实声明）

| 项 | 现状 |
|---|---|
| 背景模型 | order-0 独立字母 + 0.1 伪计数；Markov 高阶未实现（文档标注"可含"，为可选扩展） |
| E-value | 保守近似（候选窗口数 × χ²p 值）；MEME 精确 E-value 基于 LLR 词分布序统计，未实现；判读时以 LLR 排序为主、E 为辅 |
| ANR 似然 | 窗口独立式（与 Bailey & Elkan 1994 一致）；重叠窗口的背景项重复计入是该表述的已知特性 |
| **单 PWM 无法表示两个 motif** | 若一批序列里混有两个 motif 且都无突变，覆盖全部序列的「混合 PWM」会比只覆盖其中一部分的干净 motif 具有**更高的似然与 LLR**，EM 因此收敛到混合解（复现：`em_test/protein.fa`，30 条序列中 16 条 GASTLSKL + 14 条 WYHKRDLN → 得到 `TLSKL`+`DLN` 的混合）。这是 EM 单 motif 模型的固有局限，非实现缺陷 |
| 收敛 | ΔLL<ε 或 ΔPWM<1e-12；EM 只保证局部最优，靠多种子缓解（文档 §8）；对弱信号/双峰后验可能需增大 seed-count。MAP-EM（伪计数）下未惩罚似然在收敛点附近允许 ~1e-7 相对量级抖动，单调性断言按相对容差判定 |
| 歧义字符 | 窗口含歧义字母 → 候选排除（Z=0）；序列长度 < W 的序列跳过 |
| 熵/复杂度过滤 | 未实现 DUST/SEG 类低复杂度屏蔽；富含单一重复的序列可能产生伪 motif，建议预先过滤 |
| 蛋白双链 | 不适用（revcomp 仅核酸，--revcomp 自动忽略） |
| 种子富集开销 | Hamming≤1 邻域聚合为 O(不同 k-mer 数 × W × K)；基因组规模输入下该步骤会成为瓶颈 |

## 六、文件清单

```
EmMotif/                        算法库（随 MotifFinder.vbproj 编译）
├── CODE_REVIEW.md              代码审查报告：缺陷清单、红灯留证、覆盖矩阵
├── README.md
├── Core/
│   ├── Alphabet.vb             字母表/编码/歧义字符/反向互补/序列类型识别
│   ├── EmModel.vb              PWM、E 步（三模型）、M 步、全似然、一致序列
│   ├── ChiSquare.vb            不完全伽马 + χ² sf + E-value
│   └── EmSearch.vb             种子策略、逐种子 EM、择优、屏蔽、宽度范围
├── Model/ResultModel.vb        JSON DTO
├── _validation/validate_em.py  开发期 Python 公式级镜像（不替代 VB 侧测试）
└── EmMotif/../test/em_test/    命令行入口与自检（见下）

test/                           测试宿主工程（test.vbproj，Exe）
├── Program.vb                  Main：首参 "em" 分发给 EmMotif，否则跑 Gibbs 冒烟
└── em_test/
    ├── Program.vb              EmMotif CLI：discover / selftest [--only]
    ├── SelfTest.vb             自检编排器（24 组）
    ├── TestAssert.vb           断言原语
    ├── TestData.vb             确定性数据工厂
    ├── TestAlphabet.vb / TestEmMath.vb / TestEmSearch.vb / TestChiSquare.vb
    ├── dna.fa                  40 条 × 200bp，种植 ACGTTACGTA(25) 与 TTGGCCAGGA(22)
    └── protein.fa              30 条 × 150aa，种植 GASTLSKL(16) 与 WYHKRDLN(14)
```
