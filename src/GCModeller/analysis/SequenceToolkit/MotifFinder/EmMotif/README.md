# EmMotif — EM 算法 motif 发现（MEME 三种位点分布模型）

VB.NET (.NET 10) 命令行项目，**零第三方依赖**（仅 .NET BCL，JSON 用 System.Text.Json）。
按《通过 EM 方法发现 Motif：原理与流程详解》文档实现：把 motif 位置作为隐藏变量，
交替执行 E 步（估计位点后验）与 M 步（加权计数重估 PWM），迭代至收敛；
支持 **OOPS / ZOOPS / ANR** 三种位点分布模型、**核酸与氨基酸**双字母表、
双链扫描、多 motif 屏蔽重跑、宽度范围搜索，输出结构化 JSON。

## 一、构建与运行

```bash
cd EmMotif
dotnet build -c Release

# DNA（默认 zoops；自动识别字母表）
dotnet run -c Release -- discover --input test/dna.fa --model zoops --minw 8 --maxw 12 --nmotifs 2 --out motifs.json --pretty

# 氨基酸序列
dotnet run -c Release -- discover --input test/protein.fa --alphabet protein --model zoops --minw 8 --maxw 10 --out prot.json

# 双链扫描 + 多 motif
dotnet run -c Release -- discover --input test/dna.fa --model oops --minw 10 --maxw 10 --nmotifs 2 --revcomp --out rc.json

# 内置自检（E 步约束 / 种植恢复 / LL 单调 / χ² / JSON 往返 等 10 组）
dotnet run -c Release -- selftest
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
| λ 更新 | [§3 Step3] | OOPS λ≡1；ZOOPS λ = ΣZ/N；ANR λ = ΣZ/窗口总数 |
| EM 迭代至 ΔLL < ε 收敛 | [§4] | `EmSearch.RunEm`（--epsilon，默认 1e-4） |
| LL 单调不降（EM 保证） | [§4] | `FullLogLik` + selftest 断言 |
| 一致序列输出 | [§4] | `EmModel.Consensus` |
| 种子初始化（one-hot + 伪计数） | [§5] | `EmModel.InitFromSeed` |
| 三种初始化策略（穷举/抽样/K-mer 富集/一致性） | [§5 表] | `EmSearch.GenerateSeeds`（enriched=K-mer 富集 / random=随机窗口 / all=全去重 W-mer 上限 maxSeeds） |
| 多 motif：屏蔽 + 重跑 | [§7] | `EmSearch.MaskSites`（Z>0.5 窗口置歧义）→ `Discover` 循环 |
| 停止条件：nmotifs / E-value 阈值 | [§7] | `SearchOptions.EvalueMax`（默认 10） |
| 收敛后验分布多模态 → 多种子重启 | [§8] | 逐种子 EM 取最优 LL |
| -revcomp 双链 | [§9] | `EStep` 双链候选 (j,+)/(j,−)；负链第 k 列 = enc(j+W−1−k) 互补 |
| -minw/-maxw 宽度范围 | [§9] | `EmSearch.Discover`（逐宽度全流程，LL 最高者胜出） |
| E-value 与 LLR（E<0.05 显著） | [§9] | `Core/ChiSquare.vb`：LLR = 2ΣZ·logR，χ²(df=(K−1)W) 生存函数（不完全伽马，文献分位数验证 ≤5e-4），E = 候选窗口总数 × p（保守近似） |
| Gibbs 对照（EM vs 随机硬指派） | [§8] | 未实现 Gibbs（文档定位为对照方法），ZOOPS/ANR 的窗口独立性语义与之一致 |

### 对文档的两处修正（根本逻辑，非表面）

1. **ZOOPS E 步公式**：文档 §2 给出窗口级 `Z=λP_m/(λP_m+(1−λ)P_b)`，但这不满足文档 §6 自己要求的
   约束 Σ_j Z_ij ≤ 1（两条好窗口会同时拿到高 Z）。正确后验（Bailey & Elkan 1994，MEME 原文）是在
   「无位点状态 vs 全部候选位点」之间竞争：`Z_ij = λR_ij/((1−λ)+λΣ_j R_ij)`。实现取正确式。
2. **λ 更新的模型区分**：文档 §3 Step3 的 `λ = ΣZ/Σ(L−W+1)` 只适用于 ANR（每位点独立先验）；
   ZOOPS 的 MLE 应为 `λ = Σ_ij Z_ij/N`（每序列至多一个位点，期望含位点序列数/N）。实现按模型区分。

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

1. **开发期 Python 镜像对拍**（`_validation/validate_em.py`，与 VB 逐式对应）：
   - E 步约束：OOPS ΣZ=1（1e-9 精度）、ZOOPS ≤1、ANR 独立 ✓
   - 种植恢复：DNA ZOOPS 共识 10/10、位点定位误差 0.00、LL 单调 ✓；蛋白 8/8 ✓；双链 8/8 ✓
   - χ² 生存函数 vs 文献分位数（df=1,2,4,10 @0.05/0.01）≤5e-4 ✓
2. **内置自检** `EmMotif selftest`（10 组）与 Python 验证一一对应：
   约束 / DNA 恢复（含 λ 收敛与 LL 单调断言）/ 蛋白恢复 / 反义链定位 / ANR 多位点 /
   χ² / 归一化 / 多 motif 屏蔽互异 / JSON 往返 / FASTA 解析与歧义字符。

## 五、已知边界（如实声明）

| 项 | 现状 |
|---|---|
| 背景模型 | order-0 独立字母 + 0.1 伪计数；Markov 高阶未实现（文档标注"可含"，为可选扩展） |
| E-value | 保守近似（候选窗口数 × χ²p 值）；MEME 精确 E-value 基于 LLR 词分布序统计，未实现；判读时以 LLR 排序为主、E 为辅 |
| ANR 似然 | 窗口独立式（与 Bailey & Elkan 1994 一致）；重叠窗口的背景项重复计入是该表述的已知特性 |
| 收敛 | ΔLL<ε 或 ΔPWM<1e-12；EM 只保证局部最优，靠多种子缓解（文档 §8）；对弱信号/双峰后验可能需增大 seed-count |
| 歧义字符 | 窗口含歧义字母 → 候选排除（Z=0）；序列长度 < W 的序列跳过 |
| 熵/复杂度过滤 | 未实现 DUST/SEG 类低复杂度屏蔽；富含单一重复的序列可能产生伪 motif，建议预先过滤 |
| 蛋白双链 | 不适用（revcomp 仅核酸，--revcomp 自动忽略） |

## 六、文件清单

```
EmMotif/
├── EmMotif.vbproj          net10.0 控制台项目（无 PackageReference）
├── Program.vb              CLI（discover / selftest）
├── SelfTest.vb             内置自检（10 组）
├── Core/
│   ├── Alphabet.vb         字母表/编码/歧义字符/反向互补/自动识别
│   ├── FastaIO.vb          FASTA 读写
│   ├── EmModel.vb          PWM、E 步（三模型）、M 步、全似然、一致序列
│   ├── ChiSquare.vb        不完全伽马 + χ² sf + E-value
│   └── EmSearch.vb         种子策略、逐种子 EM、择优、屏蔽、宽度范围
├── Model/ResultModel.vb    JSON DTO
└── test/                   dna.fa / protein.fa（种植 motif 的合成数据）
```
