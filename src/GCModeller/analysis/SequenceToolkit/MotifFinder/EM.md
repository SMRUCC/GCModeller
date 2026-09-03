# 通过 EM 方法发现 Motif：原理与流程详解
EM（Expectation-Maximization）算法是 motif 发现中最经典的概率方法之一，其代表实现是 **MEME**（Multiple EM for Motif Elicitation，Bailey & Elkan, 1994）。它的核心思想是：**把 motif 在序列中的位置视为“隐藏变量”，通过交替执行“估计位置”和“更新模型”两步，不断迭代直到 motif 模型（PWM）收敛。**
---
## 一、问题建模
### 输入与假设
- 输入：N 条序列 $S_1, S_2, \dots, S_N$，序列 $i$ 的长度为 $L_i$
- 待发现 motif 宽度为 $W$
- 序列由一个**二分量混合模型**生成：
  - **Motif 模型**（位置特异性）：用 PWM $\theta$ 描述，$\theta_{k,a}$ 表示 motif 第 $k$ 列出现碱基 $a$ 的概率
  - **背景模型**：$\theta_0$，通常用序列整体碱基频率（可含 Markov 高阶）
### 隐藏变量
引入指示变量 $Z_{ij} \in \{0, 1\}$：
$$Z_{ij} = \begin{cases} 1, & \text{序列 } i \text{ 的位置 } j \text{ 是 motif 实例的起点} \\ 0, & \text{否则} \end{cases}$$
这些 $Z_{ij}$ 是**不可观测的**——我们不知道 motif 在哪里，这正是 EM 要解决的问题。
### 目标
极大化观测序列的似然函数：
$$\ell(\theta) = \sum_i \log P(S_i \mid \theta, \theta_0)$$
由于 $Z$ 未知，直接最大化困难，故采用 EM 迭代。
---
## 二、E 步：估计 motif 位置的后验概率
**给定当前 PWM $\theta^{(t)}$，计算每个候选窗口是 motif 起点的概率。**
对序列 $i$ 的每个位置 $j$（$1 \le j \le L_i - W + 1$）：
**Step 1：计算窗口在 motif 模型下的概率**
$$P(W_{ij} \mid \text{motif}) = \prod_{k=1}^{W} \theta_{k,\, S_i[j+k-1]}$$
**Step 2：计算窗口在背景模型下的概率**
$$P(W_{ij} \mid \text{bg}) = \prod_{k=1}^{W} \theta_0[S_i[j+k-1]]$$
**Step 3：用贝叶斯公式计算后验概率 $Z_{ij}$**
引入先验 $\lambda$（motif 出现概率），则：
$$Z_{ij} = P(Z_{ij}=1 \mid W_{ij}) = \frac{\lambda \cdot P(W_{ij} \mid \text{motif})}{\lambda \cdot P(W_{ij} \mid \text{motif}) + (1-\lambda) \cdot P(W_{ij} \mid \text{bg})}$$
其中 $\lambda$ 在 OOPS 模型下固定为 $\frac{1}{L_i - W + 1}$，在 ZOOPS/ANR 模型下可在 M 步一起更新。
> **直觉理解**：E 步做的事情是“拿当前 motif 模型去扫描所有序列”，对每个窗口打一个 0 到 1 之间的“软分”——分数越高说明该窗口越像 motif。与 Gibbs 采样“按概率随机抽一个”不同，EM 保留所有窗口的加权信息（“软指派”），因此更稳定但也更容易陷入局部最优。
---
## 三、M 步：更新 PWM 参数
**给定所有 $Z_{ij}$，重新估计使似然最大化的 $\theta$。**
对 PWM 中每个位置 $k$（$1 \le k \le W$）和每种碱基 $a \in \{A, C, G, T\}$：
**Step 1：计算加权计数**
$$n_{k,a} = \sum_{i=1}^{N} \sum_{j=1}^{L_i - W + 1} Z_{ij} \cdot \mathbb{1}\!\left[S_i[j+k-1] = a\right]$$
即：每个窗口按照其概率 $Z_{ij}$ 的权重“贡献”到对应的 PWM 列。
**Step 2：加入伪计数并归一化**
$$\theta_{k,a} = \frac{n_{k,a} + b_a}{\sum_{a'} n_{k,a'} + \sum_{a'} b_{a'}}$$
其中 $b_a$ 为伪计数（如 0.1–1），防止某列某碱基计数为 0 导致概率极端化。
**Step 3（可选）：更新 $\lambda$**
$$\lambda = \frac{\sum_{i,j} Z_{ij}}{\sum_i (L_i - W + 1)}$$
> **直觉理解**：M 步做的事情是“根据 E 步给出的软指派，重新构建 motif 模型”。高概率的窗口对 PWM 贡献大，低概率的窗口贡献小。构建出的新 PWM 又用于下一轮 E 步，如此循环。
---
## 四、完整算法流程
```text
1. 初始化
   - 选定 motif 宽度 W
   - 构造初始 PWM θ⁽⁰⁾（策略见下文）
2. 重复以下两步直至收敛：
   E 步：用当前 θ 计算 Z_ij（所有位置的后验概率）
   M 步：用 Z_ij 加权更新 θ
3. 收敛判据
   - 相邻两次迭代的对数似然变化 < 阈值 ε（如 10⁻⁴）
   - 或 PWM 变化幅度足够小
4. 输出
   - 最终 PWM
   - 每个位置的 Z_ij（可作为位点显著性得分）
   - 总对数似然（可用于多 motif 比较）
```
**单调收敛保证**：EM 的一个重要理论性质是每次迭代后似然**不降**，即 $\ell(\theta^{(t+1)}) \ge \ell(\theta^{(t)})$，因此算法必然收敛到某个驻点（通常是局部极大值）。
---
## 五、初始化策略（决定成败的关键）
由于 EM 只保证收敛到**局部最优**，初始化至关重要。MEME 的做法包括：
| 策略 | 描述 | 优缺点 |
|---|---|---|
| **穷举种子** | 把序列中每个可能的 $W$-mer 都作为种子初始化一次 | 结果可靠，但计算量大（MEME 默认） |
| **抽样种子** | 随机选取若干子串作为起点 | 速度快，适合长序列 |
| **K-mer 富集** | 先统计过表达的 $W$-mer 作为种子 | 常用于 ChIP-seq 类数据 |
| **一致性种子** | 从多序列比对或已知 motif 出发 | 有先验知识时最稳 |
**典型做法**：对每个种子独立跑 EM，最终保留**对数似然最高**的 PWM 作为结果。
---
## 六、三种位点分布模型
EM 框架可以灵活处理 motif 出现次数的不同假设：
| 模型 | 全称 | 假设 | $Z_{ij}$ 修正 |
|---|---|---|---|
| **OOPS** | One Occurrence Per Sequence | 每条序列**恰好有 1 个** motif 实例 | $\sum_j Z_{ij} = 1$，对所有 $i$ |
| **ZOOPS** | Zero or One Occurrence Per Sequence | 每条序列**最多有 1 个** motif 实例 | $\sum_j Z_{ij} \le 1$，通过引入“无 motif”状态 |
| **ANR** | Any Number of Repetitions | 每条序列可有**任意多个** motif 实例（含串联重复） | $Z_{ij}$ 独立采样 |
**ZOOPS 是实际最常用的**——因为真实生物数据中，很多序列可能根本不包含 motif。强行假设 OOPS 会产生假阳性位点。
---
## 七、多 Motif 发现（呼应你之前的问题）
EM 同样可通过 **masking + 重跑** 发现多个 motif：
```text
1. 用 EM 找到第 1 个 motif M1 及其位点
2. 将 M1 的位点所在窗口屏蔽（替换为 N 或降低采样权重）
3. 重新初始化，用 EM 找第 2 个 motif M2
4. 重复直到新 motif 的 E-value 超过阈值
```
MEME 工具中的 `-nmotifs` 参数正是这样实现的。另外 ANR 模型还能发现**串联重复**型的多个相同 motif 拷贝。
---
## 八、EM vs Gibbs：关键区别对比
| 维度 | EM（如 MEME） | Gibbs Sampler |
|---|---|---|
| **指派方式** | 软指派（保留所有位置的加权信息） | 硬指派（每轮按概率随机抽一个） |
| **收敛性质** | 确定性、单调收敛，但易陷局部最优 | 随机游走，理论上可跳出局部最优 |
| **计算成本** | 每轮需要计算所有位置的 $Z_{ij}$（与 EM 相同量级） | 每轮只需重算一条序列 |
| **结果复现性** | 相同种子 → 相同结果 | 相同种子 → 统计意义上相同 |
| **适合场景** | motif 信号较强、需要稳定结果 | motif 信号弱、有多模态后验分布 |
**有趣的联系**：Gibbs 采样可视为 EM 的**随机化版本**——EM 的 M 步在所有 $Z_{ij}$ 上加权求和，而 Gibbs 则按这些权重随机抽取一个位置作为“硬指派”再更新。两者优化的目标（对数似然）本质一致。
---
## 九、实用工具与参数建议
- **MEME Suite**（https://meme-suite.org/）：最成熟的 EM 实现
  - `-mod zoops`：推荐默认
  - `-nmotifs 5`：找 5 个 motif
  - `-minw` / `-maxw`：motif 宽度范围（如 8–20）
  - `-revcomp`：同时考虑反向互补链
- **STREME**：MEME 的新一代替代，更快，支持富集统计
- **判读结果**：关注 **E-value** 和 **log-likelihood ratio**，E-value < 0.05 通常视为显著
---
## 十、一个简化示例（3 条序列，W = 3）
假设输入序列为：`ACGTA`、`TGCAA`、`GGTAC`，W = 3
**E 步**（示意）：
- 对 `ACGTA`：位置 1（`ACG`）、位置 2（`CGT`）、位置 3（`GTA`）各自计算在 motif 和背景下的比值，得到 $Z_{11}=0.6, Z_{12}=0.3, Z_{13}=0.1$
**M 步**（示意）：
- PWM 第 1 列：$n_{1,A}$ 来自所有 $Z_{ij} \times \mathbb{1}[\text{首字母}=A]$ 的加权和
- 归一化后得到新的 $\theta_{1,A}, \theta_{1,C}, \dots$
**迭代**：新的 PWM 会更偏好像 motif 的窗口，从而在下一轮 E 步中给这些窗口更高的 $Z_{ij}$——形成正反馈，最终收敛到某个高似然的 motif 模型。

