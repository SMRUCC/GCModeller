**Clustal Omega 是一款以 mBed 聚类 + UPGMA 引导树 + HHalign 隐马尔可夫剖面（HMM-HMM）比对为核心的渐进式多序列比对工具，能以近似线性对数复杂度 O(N·logN) 处理数十万条蛋白或核酸序列。** 它由 Sievers 等人于 2011 年发表（Molecular Systems Biology 7:539），完全重构了经典 ClustalW 的算法框架，将准确度、可扩展性与速度同时提升到工业级水准。
---
## 一、为什么需要重写：ClustalW 的两大瓶颈
理解 Clustal Omega 的设计动机，需要先回顾传统渐进式比对（Progressive Alignment）的通用流程：**距离计算 → 构建引导树 → 按树自底向上逐对合并比对**。ClustalW 的瓶颈主要有两点：
- **引导树构建的平方复杂度**：所有序列两两比对生成 N×N 距离矩阵，内存和时间均为 O(N²)，上万个序列即变得不可行。
- **比对引擎的精度上限**：ClustalW 使用序列-剖面打分（profile-sequence scoring）并基于简单权重与空位罚则，"once a gap, always a gap" 的早锁问题严重。
Clustal Omega 针对前者引入 mBed 聚类近似，针对后者引入 HHalign 的 HMM-HMM 剖面比对，并辅以迭代精修。
---
## 二、整体流水线：三阶段六步
Clustal Omega 默认模式下的完整流程如下：
```
输入序列 (FASTA等)
   │
   ▼
[1] k-tuple 距离估算（继承自 ClustalW 的快测法）
   │
   ▼
[2] mBed 聚类：随机选代表序列，将序列嵌入低维向量空间，
    k-means 划分为软上限 100 条的子簇
   │
   ▼
[3] 子簇内计算全距离矩阵 → Muscle 风格快速 UPGMA 建子树，
    层层串接成全局引导树
   │
   ▼
[4] 按引导树顺序，用 HHalign 做 HMM-HMM 剖面对剖面
    渐进合并（默认 Gonnet 矩阵，gap open 6 bits，gap extend 1 bit）
   │
   ▼
[5] 迭代精修（可选）：
    - 引导树迭代：用首次比对结果重算全比对距离，重建更优树
    - HMM 迭代：将首条 MSA 转为 HMM 作为外部剖面（EPA），
      伪计数传递"软化"后续比对
   │
   ▼
输出 MSA（Fasta/Clustal/Stockholm/Phylip 等 7 种格式）
```
下面逐个拆解关键技术模块。
---
## 三、关键技术模块详解
### 模块一：mBed 聚类——把 O(N²) 压到 O(N·logN)
mBed（Blackshields et al., 2010）是 Clustal Omega 可扩展性的核心。其思路是将序列嵌入到由少量"代表序列"（seed sequences）张成的低维向量空间：
- 随机抽取若干条代表序列（typically log(N) 数量级），每条输入序列只需与这些代表序列计算 k-tuple 距离，从而将 N² 次两两比对压缩为 N·log(N) 次。
- 得到低维坐标后，用 **k-means 聚类**将序列分组，每簇软上限默认 100 条（可用 `--cluster-size` 调整）。
- 簇内计算完整距离矩阵并建局部 UPGMA 子树，簇间再用上层引导树串接。
这解释了为何 Clustal Omega 可处理数十万条序列而内存不爆——它从不真正构造全局全距离矩阵。若确实需要完整矩阵，需显式加 `--full` 标志（仅建议小数据集）。
### 模块二：引导树——Muscle 风格的快速 UPGMA
距离矩阵到手后，Clustal Omega 借用 **MUSCLE 的快速 UPGMA 实现**构建引导树。相比 ClustalW 的 Neighbor-Joining，UPGMA 在大规模数据上更快，且配合 mBed 分层结构自然递归。EPA FAQ 明确指出：由于 mBed 采样的存在，"传统意义上的引导树"默认不输出，界面上的树是事后由邻接法单独计算的。
### 模块三：HHalign——HMM-HMM 剖面比对引擎
这是 Clustal Omega 精度跃升的根本原因。它直接集成 Johannes Söding 的 **HHalign 包**（2005，Bioinformatics 21:951-960），核心思想是：
- 渐进合并时，两侧不再是"序列 vs 剖面"而是 **"HMM 剖面 vs HMM 剖面"**，即把每条已对齐的子比对建模为 profile HMM，两两对齐时做 HMM-HMM 比对。
- HMM 天然编码了位置特异的替换概率（发射概率，由 Gonnet 矩阵 + 序列加权 + 伪计数初始化）、插入/删除状态转移概率，从而对远源序列更敏感，且天然处理"once a gap, always a gap"的僵化问题。
- 默认参数与 HHalign 一致：**Gonnet 替换矩阵、空位开放 6 bits、空位延伸 1 bit**。
### 模块四：迭代精修与外部剖面（EPA）
Clustal Omega 提供两种迭代机制，均可用 `--iterations=N` 控制：
- **引导树迭代**：首次 MSA 完成后，从比对结果重新计算"全比对距离"（可选 Kimura 校正），重建引导树并重新渐进比对，如此循环。
- **HMM 迭代 / 外部剖面比对（EPA）**：将初始 MSA 转成 HMM，作为外部剖面重新引导后续比对。EPA 还允许用户直接输入 HMM（`--hmm-in`，HMMer2/3 格式），例如来自 Pfam 的家族 HMM，把数据库先验知识注入比对过程——这在向已有比对添加新序列时尤为有用。
---
## 四、实现细节：源码、依赖与输入输出
### 开源实现与依赖
- **语言与许可**：纯 C 实现，源码托管于 `github.com/GSLBiotech/clustal-omega`（镜像），官方发布在 clustal.org，最新稳定版 1.2.4。
- **核心外部依赖**：HHalign 包（Söding）、Squid 序列 IO 库（Sean Eddy）、MUSCLE 的 UPGMA 实现片段。
### 三类输入的组合模式
Clustal Omega 接受三种输入：未比对/已比对序列（`-i`）、已比对剖面（`--p1` / `--p2`）、HMM（`--hmm-in`），支持多种组合：
| 组合 | 命令 | 用途 |
|---|---|---|
| 单文件未比对序列 | `-i seqs.fa` | 标准全新 MSA |
| 双剖面对剖面 | `--p1 a.aln --p2 b.aln` | 合并两个已有比对（列保持固定） |
| 序列 + 剖面 | `-i new.fa --p1 old.aln` | 向已有比对追加新序列 |
| 序列 + HMM | `-i seqs.fa --hmm-in fam.hmm` | 外部剖面引导（EPA），如 Pfam HMM |
若输入序列已是比对状态，程序会自动转为 HMM 作为 EPA 引导，除非显式加 `--dealign` 关闭。
### 典型命令行示例
```bash
# 基础用法：默认 mBed + HHalign
clustalo -i input.fasta -o aligned.fasta --outfmt=fasta -v
# 指定迭代次数（引导树 + HMM 合并迭代 2 轮）
clustalo -i input.fasta -o out.aln --iter=2
# 使用外部 Pfam HMM 引导比对（EPA）
clustalo -i seqs.fa --hmm-in PF00069.hmm -o out.aln
# 强制完整距离矩阵（仅适合 <10,000 条序列）
clustalo -i input.fasta -o out.aln --full --distmat-out=dist.mat
# 输出引导树与聚类细节，供下游分析
clustalo -i input.fasta -o out.aln --guidetree-out=tree.dnd --clustering-out=clusters.txt
```
---
## 五、性能特征与适用场景
### 时间复杂度对比
| 工具 | 引导树构建 | 比对引擎 | 大规模（>10 万条）表现 |
|---|---|---|---|
| ClustalW | O(N²) 全矩阵 | 剖面-序列 | 几乎不可行 |
| MUSCLE | 迭代精化 | log-expect | 中等规模可行 |
| MAFFT (default) | 近似 + PartTree | 剖面-剖面 | 良好 |
| **Clustal Omega** | **O(N·logN) mBed** | **HMM-HMM (HHalign)** | **优秀** |
在约 10,000–20,000 条序列规模上，Clustal Omega 在速度上超过 MAFFT 默认模式与 MUSCLE，同时保持与其余高精度工具相当的准确度。
### 适用与不适用
- **适合**：成千上万至数十万条蛋白序列的大规模 MSA；向现有比对/Pfam 家族追加新序列；系统发生分析前的比对准备。
- **慎用/不适合**：仅两条序列（应改用 pairwise 工具）；超长或高度分化的核酸序列（接近 10 万字符长度可能失败）；需要极高精度（如结构功能位点精细比对）时可考虑 MAFFT L-INS-i 等一致性方法。
---
## 六、总结
Clustal Omega 的本质是**"渐进式比对框架 + 三大现代化组件"**：用 mBed 的随机投影聚类解决可扩展性，用 HHalign 的 HMM-HMM 剖面比对解决精度，用引导树/HMM 双重迭代解决早锁问题。它在保持 Clustal 系列易用性的同时，把多序列比对从"千条级"推到了"十万条级"，至今仍是蛋白家族比对与大规模注释流程的主力工具之一。

---

## 七、本目录的托管代码实现

本节记录 `SequenceAlignment.vbproj` 中 `Clustal` 目录的 VB.NET 实现与上文算法的对应关系。

### 7.1 模块映射

| 流水线阶段 | 实现模块 |
|---|---|
| 序列归一化与类型判定 | `SequenceAlphabet.vb` |
| k-mer 计数与距离 | `Distance/KmerCounter.vb`、`Distance/MBedDistance.vb` |
| k-means 分簇 | `Clustering/KMeansCluster.vb` |
| 簇内/簇间 UPGMA + 引导树 | `Clustering/UPGMA.vb`、`Clustering/ClusterTreeBuilder.vb`、`Clustering/GuideTree.vb` |
| 替换矩阵(Gonnet 250 / DNA) | `Resources/Gonnet250.vb`、`HMM/ScoringMatrix.vb` |
| 三态 profile HMM 构建 | `HMM/ProfileHMM.vb` |
| 4 状态 pair-HMM 前向/后向 + 路径解码 | `HMM/PairHMMAligner.vb` |
| 渐进合并 | `Progressive/ProgressiveAligner.vb` |
| 迭代精炼 | `Progressive/Refinement.vb` |
| 统计(SP/一致性/保守列) | `Progressive/AlignmentStatistics.vb` |
| 结果容器与门面 | `ClustalOmegaResult.vb`、`ClustalOmega.vb` |
| 输出格式 | `IO/FastaWriter.vb`、`IO/ClustalWriter.vb`、`IO/StockholmWriter.vb`、`IO/PhylipWriter.vb`、`IO/MSAIO.vb` |

### 7.2 用法

```vbnet
Imports SMRUCC.genomics.Analysis.SequenceAlignment.Clustal

Dim options As New ClustalOmegaOptions With {
    .SeqType = SequenceType.Auto,      ' 自动判定蛋白/核酸
    .ClusterSize = 100,                ' mBed 聚类的软上限
    .Iterations = 2,                   ' 迭代精炼轮数
    .GapOpenBits = 6.0,                ' 与 clustalo/HHalign 一致
    .GapExtendBits = 1.0
}

Dim result As ClustalOmegaResult = ClustalOmega.AlignFile("./input.fasta", options)

Call Console.WriteLine(result.ToString())                 ' 表格 + 保守列标记
Call result.SaveAs(MSAFormat.Clustal, "./output.aln")     ' 也可以 Fasta/Stockholm/Phylip
Call Console.WriteLine(result.TreeNewick())               ' 引导树
```

### 7.3 参数说明

| 参数 | 默认值 | 说明 |
|---|---|---|
| `SeqType` | `Auto` | 碱基占比 ≥ 85% 判定为核酸，否则为蛋白 |
| `KmerSize` | 0(自动) | 蛋白 2、核酸 5 |
| `ClusterSize` | 100 | k-means 分簇的目标上限，簇数 = ⌈N / ClusterSize⌉ |
| `FullDistanceMatrix` | False | 对应 clustalo 的 `--full`，仅在 N ≤ `MaxFullDistanceSequences` 时生效 |
| `Iterations` | 0 | 迭代精炼轮数，仅对 N ≤ 2000 生效 |
| `GapOpenBits` / `GapExtendBits` | 6.0 / 1.0 | 单位 bit，与 HHalign 默认值一致 |
| `EndGapScale` | 0.25 | 终端空位罚分的缩放系数，0 = 半全局(可自由滑动)，1 = 纯全局 |
| `MacRamMb` | 2048 | 后验概率缓冲上限，超限自动退化为 Viterbi |
| `Decode` | `MaximumAccuracy` | 也可以显式选择 `Viterbi` |
| `Seed` | 20111 | 控制 k-means++ 的抽样，保证可复现 |

### 7.4 实现与 clustalo 的已知差异

1. **打分**：本实现使用 1/3 bit 单位的**整数取整** Gonnet 250 矩阵(与 `data/BLOSUM/GONNET.txt` 一致)，
   而不是 clustalo 内部的高精度浮点矩阵，因此远源序列的灵敏度略低；可通过
   `ScoringMatrix.Gonnet(scoreScale)` 或 `ScoringMatrix.LoadFile` 换用其它矩阵。
2. **profile HMM**：只保留 match/insert/delete 三种状态与位置特异空位罚分，省略了
   HHalign 的二级结构信息、`Neff` 之外的多重序列加权细则以及 MAC 的完整后验解码
   (见 `PairHMMAligner` 的类注释)。
3. **引导树**：簇内使用全距离 UPGMA，簇间以 medoid 为代表序列做上层 UPGMA；
   clustalo 的 mBed 内部细节(随机抽样次数、k-means 的软上限策略)与本实现不完全相同。
4. **可扩展性**：渐进合并阶段的时间复杂度仍是 O(ΣL²)，10^5 条序列需要数十小时级别的算力，
   而 clustalo 通过更激进的并行与 HMM 加速把常数压得更低。

因此**验收标准是结构性正确**，而不是与 clustalo 输出逐列一致：

- 所有行等长，且每行去掉空位后可以还原输入序列；
- SP 得分不劣于 `MSA/CenterStar.vb`(中心星)在同一输入上的结果；
- 引导树叶子数等于序列条数，Newick 可被下游解析；
- 四种输出格式可以互相回读(Stockholm 与项目内既有的 `MSA.Tabular.Reader` 互通)。

### 7.5 验证

`test/ClustalOmegaDemo.vb` 是控制台验证程序，覆盖核酸小样例、蛋白样例、四种格式的往返、
迭代精炼不劣化、k-means 分簇分支(120 条序列)以及空/单条/全同的退化输入，
所有断言通过 `Microsoft.VisualBasic.ApplicationServices.Debugging.DebugAudit` 记录，
运行结束后生成 `clustal-omega-test/clustal-omega.audit.json` 报告。

```bash
dotnet build test.vbproj -c Debug -p:StartupObject=test.ClustalOmegaDemo
dotnet bin/Debug/net10.0/test.dll
```