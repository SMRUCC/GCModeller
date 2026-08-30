# NCBI BLAST+ 中 BLASTN 与 BLASTP 算法详解
**BLASTN 与 BLASTP 的本质区别：BLASTN 比对核酸序列、依赖精确 word match，速度快但灵敏度较低；BLASTP 比对蛋白质序列、依赖打分矩阵识别相似 word，灵敏度更高但计算量更大。** 二者均采用 BLAST 家族经典的 "seed-and-extend"（种子-延伸）启发式策略，先用短片段（word）作为种子命中候选位点，再对候选位点进行延伸和统计学评估，从而在远快于 Smith-Waterman 精确算法的前提下，仍能获得高显著性比对结果。
---
## 一、BLAST 核心算法流程
BLAST 是一种启发式算法，通过在查询序列和数据库序列之间寻找 word match（也叫"热点"）作为延伸起点，最终形成完整比对。整个流程可分为五个阶段：
```mermaid
flowchart LR
    A[过滤低复杂度区域<br/>DUST/SEG] --> B[构建查询序列 word 列表<br/>并建立查找表]
    B --> C[扫描数据库<br/>命中种子 word]
    C --> D[两-hit 法触发<br/>无 gap 延伸]
    D --> E[有 gap 延伸<br/>X-drop 动态规划]
    E --> F[计算 E-value/Bit Score<br/>输出比对结果]
```
**各阶段要点：**
1. **序列过滤**：BLASTN 默认使用 DUST 过滤低复杂度区域，BLASTP 默认使用 SEG 过滤低复杂度氨基酸片段，避免大量无生物学意义的命中。
2. **Word 匹配**：将查询序列切成长度为 W 的 word。BLASTN 要求 word **完全匹配**；BLASTP 则要求 word 的比对得分 ≥ 阈值 T，允许非精确匹配。
3. **两-hit 法**：经典的 gapped BLAST 引入"两个 word 命中必须在同一对角线上、相距不超过 A（默认 40）"这一条件，才触发无 gap 延伸，极大降低了假阳性延伸的次数。
4. **无 gap 延伸**：沿两个方向延伸种子，直到比对得分下降超过 X-dropoff（默认约 20 bits）才停止。得分超过阈值的片段对（HSP）被保留。
5. **有 gap 延伸**：从无 gap HSP 出发，用改进的 Smith-Waterman 动态规划算法做带 gap 的延伸，并使用 X-dropoff（预延伸用 Xg≈16，最终延伸用 Xg≈40–67）控制计算量。
---
## 二、BLASTN：核酸比对算法
### 2.1 任务类型与默认参数
BLASTN 在 BLAST+ 中通过 `-task` 参数选择不同的预设组合，不同任务在 word size、打分等参数上差异极大，直接决定了灵敏度与速度的取舍：
| 任务 | 适用场景 | Word size | Reward/penalty | Gap open/extend | 备注 |
|------|----------|-----------|----------------|------------------|------|
| **megablast** | 极相似序列（种内、测序错误校正） | **28** | +1 / -2 | 0 / 动态 | 默认任务，速度最快 |
| **dc-megablast** | 跨种核酸比对 | **11**（非连续） | +2 / -3 | 5 / 2 | 允许 word 中有错配 |
| **blastn** | 传统跨种比对 | **11** | +2 / -3 | 5 / 2 | word 必须完全匹配 |
| **blastn-short** | 短于 50 nt 的短序列 | **7** | +1 / -3 | 5 / 2 | 引物、miRNA 等短查询 |
**Megablast 的打分规则**：gap 延伸代价由 reward/penalty 推导，例如 reward=1、penalty=-5 时，gap 每延伸一个碱基的代价为 |2×(-5)-1|/2 = 5.5。
### 2.2 Discontiguous MegaBLAST（非连续种子）
dc-megablast 是 MegaBLAST 的扩展，用于**序列相似度低于约 80% 的远源比较**。它不再要求 word 完全连续匹配，而是采用"模板"（template）方式——在长度 L 的窗口内只要求特定位置（如 11/18 或 12/19 位）匹配即可命中，其余位置允许错配。
常用模板（W = 匹配数，t = 模板长度）：
- coding 模板（W=11, t=18）：`101101100101101101`
- optimal 模板（W=11, t=18）：`111010010110010111`
这种设计源自 PatternHunter 算法，通过"错配容忍的种子"大幅提升远源同源序列的检出率。
### 2.3 BLASTN 的 X-dropoff 参数
- `xdrop_ungap = 20`（无 gap 延伸终止阈值，单位 bit）
- `xdrop_gap = 30`（初步 gapped 延伸阈值）
- `xdrop_gap_final = 100`（最终 gapped 比对阈值）
---
## 三、BLASTP：蛋白质比对算法
### 3.1 默认参数详解
BLASTP 比对蛋白质序列时，因氨基酸有 20 种且具有理化性质差异，需要用**替换矩阵**代替简单的 match/mismatch 打分：
| 参数 | 默认值 | 说明 |
|------|--------|------|
| word_size | **3** | 初始 word 长度（蛋白常用 3） |
| matrix | **BLOSUM62** | 默认替换矩阵 |
| threshold | **11** | word 加入查找表的最低得分 T |
| gapopen | **11** | 打开一个 gap 的代价 |
| gapextend | **1** | gap 每延伸一个残基的代价 |
| window_size | 40 | 两-hit 法窗口 |
| xdrop_gap_final | 25 | 最终 gapped 延伸阈值 |
| comp_based_stats | 2 | 组成校正模式 |
### 3.2 替换矩阵的选择逻辑
替换矩阵反映氨基酸对之间的进化替换概率，对数几率矩阵是 BLASTP 的核心：
- **BLOSUM62**（默认）：基于相似度 ≤62% 的蛋白多重比对构建，是"平衡型"矩阵，适合大多数中等相似度的比对。
- **BLOSUM45** / **PAM250**：适合**远源**蛋白（相似度较低），允许更多非保守替换。
- **BLOSUM80** / **PAM30**：适合**近源**蛋白或**短序列**（如 blastp-short 默认使用 PAM30），偏向严格匹配。
**经验法则**：远源关系选高编号 PAM（如 PAM250）或低编号 BLOSUM（如 BLOSUM45）；近源关系选低编号 PAM 或高编号 BLOSUM。
### 3.3 组成校正
蛋白质序列的氨基酸组成往往偏离随机模型（如富含亮氨酸、酸性残基），会扭曲 E-value 的估计。BLASTP 默认启用 `comp_based_stats=2`，即"条件性组成打分矩阵校正"：在评估比对时，根据查询和目标序列的实际氨基酸组成对 BLOSUM62 矩阵逐项调整，从而获得更准确的 E-value。
- `comp_based_stats=0`：关闭校正
- `comp_based_stats=1`：仅做全局打分缩放（NAR 2001 方法）
- `comp_based_stats=2`：条件性矩阵调整（默认，Bioinformatics 2005 方法）
- `comp_based_stats=3`：无条件矩阵调整
---
## 四、BLASTN vs BLASTP 关键差异对照
| 维度 | BLASTN | BLASTP |
|------|--------|--------|
| **查询/数据库类型** | 核酸 vs 核酸 | 蛋白 vs 蛋白 |
| **Word 匹配方式** | **必须完全匹配**（除 dc-megablast） | **允许非精确匹配**，要求 word 得分 ≥ threshold |
| **打分系统** | match reward / mismatch penalty | 替换矩阵（BLOSUM/PAM 系列） |
| **典型 word size** | 11–28 | 2–3 |
| **低复杂度过滤** | DUST | SEG |
| **组成校正** | 不适用 | 组成校正（comp_based_stats） |
| **灵敏度** | 较低（受限于完全匹配） | 较高（替换矩阵捕捉保守替换） |
| **典型用途** | 核酸同源搜索、物种鉴定、引物验证 | 蛋白家族/同源搜索、功能注释 |
| **理论灵敏度** | 较低（4^11 才可能出现一个随机 11-mer 命中，但核酸只有 4 种字符） | 较高（20 种字符 + 矩阵，可捕捉保守替换） |
---
## 五、E-value 与 Bit Score 的统计学原理
BLAST 的显著性评估基于 **Karlin-Altschul 极值分布理论**。对于长度为 m 和 n 的两条序列的局部比对，得分 ≥ S 的 HSP 期望数量为：
$$E = K \cdot m \cdot n \cdot e^{-\lambda S}$$
其中 λ 和 K 是与打分系统和序列组成有关的统计参数。
**Bit Score** 是将原始得分 S 归一化到统一"位"单位的量：
$$S' = \frac{\lambda S - \ln K}{\ln 2}$$
Bit Score 与 E-value 的换算关系为：
$$E = m \cdot n \cdot 2^{-S'}$$
**关键解读**：
- E-value 越小越显著；E=1 表示随机数据库中平均能找到 1 个同分比对。
- 序列长度翻倍 → E-value 翻倍；得分翻倍 → E-value 指数级下降。
- E < 0.01 时，E-value 与 P-value 几乎相等。
- 数据库规模影响 E-value：相同比对在更大库中 E-value 更大。
---
## 六、常用命令行示例
**典型跨种核酸比对（默认任务为 megablast，需切换到 blastn）：**
```bash
blastn -task blastn -query seq.fa -db nt \
  -word_size 11 -reward 2 -penalty -3 \
  -gapopen 5 -gapextend 2 \
  -evalue 1e-5 -outfmt 6 -out result.tsv
```
**MegaBLAST 索引加速搜索：**
```bash
blastn -task megablast -query reads.fa -db nt \
  -use_index true -index_name nt_index
```
**典型 BLASTP 蛋白比对：**
```bash
blastp -query protein.fa -db swissprot \
  -matrix BLOSUM62 -word_size 3 -threshold 11 \
  -gapopen 11 -gapextend 1 \
  -comp_based_stats 2 \
  -evalue 1e-3 -outfmt "6 qseqid sseqid pident length evalue bitscore"
```
**短序列（如引物、短肽）搜索：**
```bash
blastn -task blastn-short -query primer.fa -db nt -evalue 1000
blastp -task blastp-short -query short_peptide.fa -db swissprot -evalue 100
```
---
## 七、常见误区与排查思路
**误区 1：用 megablast 跑跨种比对却发现结果很少。**
MegaBLAST 的 word size = 28，要求 28 个连续碱基完全匹配才能命中，跨种序列因 SNP 积累很难形成如此长的精确匹配。**排查**：切换为 `-task blastn` 或 `-task dc-megablast`。
**误区 2：word size 越小越灵敏，但会漏掉远源蛋白。**
BLASTP 的默认 word_size=3、threshold=11 已经平衡了灵敏度与速度；将 word_size 降到 2 会大幅增加无 gap 延伸次数，搜索速度指数级下降，且对远源同源的增益有限。**调优方向**：远源蛋白搜索应优先选择更宽松的矩阵（BLOSUM45）并适当提高 E-value，而非盲目减小 word size。
**误区 3：忽略低复杂度过滤导致假阳性爆棚。**
共线性重复（如 poly-A、富 Glu 区）会触发大量无意义命中。**排查**：BLASTN 用 DUST，BLASTP 用 SEG，默认已开启；若被过度过滤，可用 `-dust no` 或 `-seg no` 关闭，或用 `-soft_masking true` 仅在种子阶段屏蔽。
**误区 4：E-value 相同的比对并不一定同样可靠。**
E-value 受数据库大小和查询长度影响。短查询（如 30 aa 短肽）用默认参数几乎搜不到结果，需要用 `blastp-short` 任务并提高 E-value 阈值。
**误区 5：BLASTP 结果中得分偏低就认为没有同源。**
组成校正（comp_based_stats=2）会自动调整矩阵，偏远源但组成特殊的比对也能获得合理评分。关闭校正（`-comp_based_stats 0`）可能造成远源蛋白 E-value 被低估（过度显著）或高估（漏检）。
---
## 八、进阶：算法优化设计逻辑
BLAST 的速度优势来自两个精妙设计：
**1. 两-hit 法而非单-hit 法。** gapped BLAST 不再对单个 word 命中触发延伸，而是要求同一对角线上出现两个命中且距离 ≤ A（默认 40），才启动无 gap 延伸。这一改进使得无 gap 延伸次数下降约一个数量级，同时保留绝大多数真阳性。
**2. X-dropoff 动态截断。** 有 gap 延伸虽使用 Smith-Waterman 思想，但只在"最优得分 - X"范围内扩展动态规划矩阵单元（X-dropoff 策略），避免完整矩阵填充。最终 gapped 阶段 Xg 设得比预延伸阶段更大（如 67 vs 40），保证高置信度比对的完整性。
---
## 九、参数调优速查卡
<details>
<summary><b>场景：跨物种同源基因搜索</b></summary>
```bash
blastn -task dc-megablast -query gene.fa -db nt \
  -template_type optimal -template_length 18 \
  -reward 2 -penalty -3 -evalue 1e-5
```
非连续种子 + 宽松打分，远源核酸最佳起点。
</details>
<details>
<summary><b>场景：远源蛋白家族搜索</b></summary>
```bash
blastp -query protein.fa -db nr \
  -matrix BLOSUM45 -gapopen 14 -gapextend 2 \
  -word_size 3 -threshold 11 \
  -comp_based_stats 2 -evalue 10
```
BLOSUM45 + 提高阈值，捕捉约 25–30% 相似度的远源同源。
</details>
<details>
<summary><b>场景：短序列（≤30 aa 或 ≤50 nt）</b></summary>
```bash
blastn -task blastn-short -word_size 7 -evalue 1000 -dust no
blastp -task blastp-short -word_size 2 -matrix PAM30 -evalue 1000 -seg no
```
短序列必须用专用任务，否则默认参数会过滤掉几乎所有结果。
</details>
<details>
<summary><b>场景：大规模宏基因组/转录组快速比对</b></summary>
```bash
makembindex -input_type blastdb -db nt -index_name nt_idx
blastn -task megablast -query reads.fa -db nt \
  -use_index true -index_name nt_idx -num_threads 16
```
索引模式跳过库扫描，对短读比对提速显著（word_size 必须 ≥16）。
</details>
<details>
<summary><b>场景：只关心每个查询区域的最佳命中</b></summary>
```bash
blastn -query seq.fa -db nt -best_hit_overhang 0.25 -best_hit_score_edge 0.05
```
Best-Hit 过滤算法丢弃被更强 HSP "覆盖"的弱命中，减少冗余结果。
</details>
---
BLASTN 与 BLASTP 的设计差异本质上源于核酸（4 字符、保守性强、直接编码）与蛋白质（20 字符、化学性质分化、密码子简并）的生物物理差异：前者用长 word + 精确匹配换取速度，后者用短 word + 矩阵打分换取灵敏度。掌握两套默认参数背后的统计学与算法逻辑，才能针对具体研究问题做出正确的参数选择，而不是盲目套用默认值。
