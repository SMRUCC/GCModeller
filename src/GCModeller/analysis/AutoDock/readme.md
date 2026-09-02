# AutoDock Vina 与 MM-GBSA / Nwat-MMGBSA 算法深度解析（面向开发视角）
## 一、AutoDock Vina 算法架构
Vina 的整体框架可概括为 "**经验性打分函数 + 迭代局部搜索（ILS）全局优化 + BFGS 梯度精修**" 三层结构，由 Oleg Trott 于 2010 年设计，源码以 Apache 2.0 协议发布在 GitHub（ccsb-scripps/AutoDock-Vina）。
### 1.1 打分函数：加权的原子对相互作用项
Vina 的核心打分函数是 **构象依赖项之和**，形式如下：
$$c = \sum_{i<j} f_{t_i t_j}(r_{ij}) + c_{\text{intra}}$$
其中求和遍历所有能相对移动的原子对（排除 1–4 相互作用，即相隔 3 个共价键的原子），每个原子按 X-Score 方案分配原子类型 $t_i$，$f_{t_i t_j}$ 是对称的距离相互作用函数。
具体到每个原子对，相互作用被分解为 5 个经验项：
$$e_{\text{pair}}(d) = w_1 \cdot \text{gauss}_1(d) + w_2 \cdot \text{gauss}_2(d) + w_3 \cdot \text{Repulsion}(d) + w_4 \cdot \text{Hydrophobic}(d) + w_5 \cdot \text{HBond}(d)$$
其中表面距离定义为 $d = r_{ij} - R_{t_i} - R_{t_j}$，$R_t$ 为原子范德华半径。
**各数学子项的具体形式**：
| 项 | 数学表达 | 物理含义 |
|---|---|---|
| **gauss₁** | $\exp\left[-\left(\frac{d-o_1}{s_1}\right)^2\right]$ | 第一高斯吸引项，主吸引势阱 |
| **gauss₂** | $\exp\left[-\left(\frac{d-o_2}{s_2}\right)^2\right]$ | 第二高斯吸引项，加宽势阱 |
| **Repulsion** | $d^2$ (当 $d \le 0$)；$0$ (当 $d > 0$) | 抛物线型排斥项，避免原子重叠 |
| **Hydrophobic** | $1$（$d \le 0.5\,$Å）；线性过渡（$0.5<d<1.5\,$Å）；$0$（$d \ge 1.5\,$Å） | 疏水作用，仅在两个疏水原子对之间计算 |
| **HBond** | $1$（$d \le -0.7\,$Å）；线性过渡（$-0.7<d<0$）；$0$（$d \ge 0$） | 非方向性氢键，要求给体-受体对 |
**Vina 1.1 默认权重**（来自 Trott 原论文表 1）：
$$w_1 = -0.0356,\quad w_2 = -0.00516,\quad w_3 = 0.840,\quad w_4 = -0.0351,\quad w_5 = -0.587$$
**构象无关项**（结合能换算）：
$$\Delta G = g(c_{\text{inter}}) = 0.0585 \cdot N_{\text{rot}} + c_{\text{inter}}$$
其中 $N_{\text{rot}}$ 是配体中重原子之间的活性可旋转键数，用于惩罚结合时配体构象熵的损失。
**值得注意的是**：Vina 的打分函数被作者明确描述为 "更接近机器学习而非纯物理推导"——它从 PDBbind 2007 数据集通过非线性回归训练得到权重，融合了知识基势和经验打分的优点。Vina 采用**联合原子模型**，打分只涉及重原子（氢原子仅用于原子类型判断），因此输出构象中的氢位置是任意的。
### 1.2 全局优化：Iterated Local Search (ILS)
Vina 抛弃了 AutoDock 4 的 Lamarckian 遗传算法，选择 **Iterated Local Search** 全局优化器（与 Abagyan 等人的方案类似）。算法由多个独立 "run" 组成，每个 run 内部循环执行 "**随机扰动 → 局部优化 → Metropolis 接受判定**"。
**ILS 主循环伪代码**：
```
# 全局参数：num_runs (由 exhaustiveness 决定)，max_steps (自适应)
# 自由度向量 x = {平移(3), 旋转(3), 可旋转键扭转角(N_torsion)}
for run in 1..num_runs:                       # 多线程并行
    x ← random_pose_in_box()                   # 随机初始构象
    x ← BFGS_local_optimize(x)                 # 初始局部优化
    best ← x
    step_count ← 0
    
    while step_count < adaptively_determined_max_steps:
        # Step 1: 随机扰动
        x' ← perturb(x, amplitude)             # 平移/旋转/扭转角加随机噪声
        
        # Step 2: 局部优化（BFGS）
        x'' ← BFGS_local_optimize(x')          # 用梯度信息快速收敛到附近极小值
        
        # Step 3: Metropolis 准则接受/拒绝
        Δc = score(x'') − score(x)
        if Δc < 0 or random_uniform() < exp(−Δc / T):
            x ← x''                            # 接受
        else:
            x ← x                              # 拒绝，保留原状
            
        if score(x) < score(best):
            best ← x
        step_count += 1
        
    collection.add(best)                       # 收集到全局解集
    
return cluster_and_rank(collection)            # 聚类，输出前 9 个 pose
```
每个 run 的**步数是自适应确定的**，根据问题的表观复杂度调整；多个 run 在共享内存多核 CPU 上多线程并行执行，最后合并所有 run 找到的显著极小值，做结构精修和聚类输出。
### 1.3 BFGS 局部优化：为什么用梯度
BFGS（Broyden-Fletcher-Goldfarb-Shanno）是一种拟牛顿法，相比纯能量评估能**显著加速收敛**。Vina 在局部优化中同时计算打分函数值 $c$ 和梯度 $\nabla c$：
- **对平移的偏导** = 配体所受负的总力
- **对旋转的偏导** = 配体所受负的总扭矩
- **对扭转角 $\theta_k$ 的偏导** = 扭转轴上投影的负扭矩
BFGS 通过维护近似的逆 Hessian 矩阵 $B^{-1}$ 迭代更新搜索方向，用较少的能量评估即可收敛到附近的极小值。Vina 的 `exhaustiveness` 参数（默认 8）直接控制 run 的数量，对该值越大采样越充分，对极性代谢物建议设 16–32。
### 1.4 关键实现细节（开发者视角）
- **模块化设计**：Vina 的打分函数 $f_{t_i t_j}$ 和换算函数 $g$ 作为参数传递，理论上可替换原子类型方案（如 AD4 类型或 SYBYL 类型）。
- **Vina 1.2 扩展**：支持多配体同时对接、大环分子、水合对接协议、外部 AutoDock maps 读写、Python 3 绑定。
- **Smina 分叉**：由 Koes 等维护，提供 26 种可组合的能量项，是自定义打分函数开发的最佳起点；Vinardo 打分函数即基于 Smina 开发。
- **源码位置**：GitHub `ccsb-scripps/AutoDock-Vina/src/`，核心求解器在 `src/lib/` 下，可参考 `scoring.cpp`（打分）、`bfgs.h`（BFGS 实现）、`monte_carlo.cpp`（ILS 主循环）。
---
## 二、MM-GBSA：从对接姿态到结合自由能
MM-GBSA 属于**端点方法**——只需采样复合物状态（相比自由能微扰 FEP 需要采样多个非物理中间态），精度高于经验打分函数但计算量远小于 FEP。
### 2.1 完整公式
结合自由能定义为：
$$\Delta G_{\text{bind}} = \langle G_{\text{complex}} \rangle - \langle G_{\text{receptor}} \rangle - \langle G_{\text{ligand}} \rangle = \Delta H - T\Delta S$$
**单个状态 $X$（复合物/受体/配体）的自由能**由以下 6 项组成：
$$G_X = \langle E_{\text{MM}} \rangle + \langle G_{\text{solv}} \rangle - T\langle S \rangle$$
展开各子项：
$$\Delta G_{\text{bind}} = \Delta E_{\text{internal}} + \Delta E_{\text{electrostatic}} + \Delta E_{\text{vdW}} + \Delta G_{\text{polar}} + \Delta G_{\text{nonpolar}} - T\Delta S$$
| 项 | 含义 | 计算方法 |
|---|---|---|
| $E_{\text{internal}}$ | 键长、键角、二面角能量（力场） | AMBER ff14SB / GAFF |
| $E_{\text{electrostatic}}$ | 库仑静电相互作用 $\sum q_i q_j / (\varepsilon r_{ij})$ | 同上 |
| $E_{\text{vdW}}$ | Lennard-Jones 6-12 势 | 同上 |
| $G_{\text{polar}}$ | 极性溶剂化自由能 | **GB 方程**（隐式溶剂）或 PB 方程 |
| $G_{\text{nonpolar}}$ | 非极性溶剂化自由能 | $\gamma \cdot \text{SASA} + b$（SASA线性关系） |
| $-T\Delta S$ | 熵贡献（平动/转动/振动熵） | 正则模分析、准谐振近似（QH） |
### 2.2 GB（Generalized Born）极性溶剂化模型
MM-GBSA 用 GB 方程近似求解 Poisson-Boltzmann 方程，计算效率比 PB 高一个数量级。GB 方程的基本形式：
$$G_{\text{polar}} = -\frac{1}{2}\left(1 - \frac{1}{\varepsilon_{\text{solv}}}\right)\sum_{i,j} \frac{q_i q_j}{\sqrt{r_{ij}^2 + R_i R_j \exp\left(-\frac{r_{ij}^2}{4 R_i R_j}\right)}}$$
其中 $R_i$ 是原子 $i$ 的**有效 Born 半径**，取决于其周围介电环境；常用的 GB 模型有 GB-OBC1、GB-OBC2、**GB-Neck2**（Nwat-MMGBSA 推荐使用，对盐桥和氢键描述更准确）。
### 2.3 非极性溶剂化项
$$G_{\text{nonpolar}} = \gamma \cdot \text{SASA} + b$$
SASA 通过 LCPO 算法或 ICOSA 方法估算，$\gamma$ 约 0.0072 kcal/mol/Å²，$b$ 约 0 kcal/mol。
### 2.4 实操变体：1A vs 3A
**严格的三轨迹（3A-MM/GBSA）** 要求对复合物、自由受体、自由配体分别做 MD。**实际广泛采用的是单轨迹（1A-MM/GBSA）**——只对复合物做 MD，然后在每个快照下"删除"配体或受体得到后两者的构象，优点是 $E_{\text{internal}}$ 完全抵消，标准差降低 4–5 倍，且精度往往更优。
### 2.5 熵项的处理
$-T\Delta S$ 是 MM-GBSA 最具争议的项：
- **正则模分析**：对每个快照做 Hessian 极小化后计算振动频率，计算成本极高（复杂度 $O(N^3)$）
- **准谐振近似（QH）**：忽略低频模式，速度更快但精度略降
- **实践中常忽略**：当比较结构相似的化合物（同系列代谢物）时，熵贡献大致抵消；但比较结构差异大的分子时会引入较大系统误差
---
## 三、Nwat-MMGBSA：显式水分子改进
标准 MM-GBSA 将所有显式水删除后用隐式溶剂模型，这**丢失了介导受体-配体相互作用的关键水桥**。晶体学统计显示，约 2/3 的蛋白-配体复合物中至少有一个水分子介导接触。
### 3.1 核心思想
**Nwat-MMGBSA**（Contini & Maffucci，2016/2018）的做法是：在每个 MD 快照中，**保留 $N_{\text{wat}}$ 个距离配体最近的显式水分子**，其余水删除，并将这些水**视为受体的一部分**参与 MM-GBSA 计算。
$$\Delta G_{\text{bind}}^{\text{Nwat}} = \langle G_{\text{complex} + N_{\text{wat}}} \rangle - \langle G_{\text{receptor} + N_{\text{wat}}} \rangle - \langle G_{\text{ligand}} \rangle$$
**关键设计选择**：
- **固定的 $N_{\text{wat}}$**（而非固定距离截断）：保证每个快照采样的水数一致，相比"距离截断法"得到更好的实验相关性和重复性。
- **水从 MD 轨迹中选取**（而非晶体水）：避免晶体水位点是多个分子电子密度的平均值这一伪影。
- **典型取值**：$N_{\text{wat}} = 10, 30, 60, 100$。文献推荐多数体系用 30，溶剂暴露大的口袋（如 Rac1-Tiam1）可能需要 60–100。
### 3.2 完整工作流程
```
┌─────────────────────────────────────────────────────────────┐
│ 步骤 1: Vina 对接，输出前 10 个 pose                         │
│         (通常取排名前 1–10 的化合物，数百个)                  │
└────────────────────────┬────────────────────────────────────┘
                         ↓
┌─────────────────────────────────────────────────────────────┐
│ 步骤 2: 复合物在 TIP3P 水盒子中做 MD                          │
│   • AMBER ff14SB（蛋白）+ GAFF（配体）                       │
│   • AM1-BCC 电荷（antechamber）                              │
│   • 平衡（NVT/NPT）→ 1–4 ns 产生段（GPU 加速）               │
│   • 100 个均匀间隔快照                                       │
└────────────────────────┬────────────────────────────────────┘
                         ↓
┌─────────────────────────────────────────────────────────────┐
│ 步骤 3: cpptraj 处理                                         │
│   • 对每个快照，用 closest 命令保留 Nwat 个最近水             │
│   • 剥离其余所有水（trajout strip）                           │
└────────────────────────┬────────────────────────────────────┘
                         ↓
┌─────────────────────────────────────────────────────────────┐
│ 步骤 4: MMPBSA.py 计算                                       │
│   • GB-Neck2 隐式溶剂模型, 0.15 M 盐浓度                     │
│   • 计算复合物 + 受体(+Nwat 水) + 配体的能量                  │
│   • 熵项通常忽略（准谐振可选）                                │
└────────────────────────┬────────────────────────────────────┘
                         ↓
┌─────────────────────────────────────────────────────────────┐
│ 步骤 5: 求平均，得到每个化合物的 ΔG_bind，重新排序             │
└─────────────────────────────────────────────────────────────┘
```
### 3.3 性能提升的实证数据
Maffucci 等在 AmpC β-内酰胺酶和 Rac1-Tiam1 体系上的回顾性虚拟筛选结果显示：
- AmpC：Docking 单独 AUC 72% → 标准MM-GBSA 无显著提升 → **Nwat=30/60 时 ROC AUC 提升 20–30%**
- Rac1：Docking AUC 0.60 → 标准MM-GBSA 无改善 → **Nwat=60 AUC 提升 21%，Nwat=100 提升 29%**
- 青霉素肽酶体系：$r^2$ 从标准 MM-GBSA 的 0.3 提升到 Nwat-MMGBSA 的 **0.8**
- 在水桥作用不显著的 BCL-XL 体系，Nwat-MMGBSA **不会降低**相关性（$r^2$ 维持在 0.7），表明该方法"安全"
### 3.4 计算成本
优化后的协议在配备单块 GeForce GTX TITAN Black GPU 的工作站上，**每个配体约 1.5 小时**（含参数化、平衡、1 ns 产生段和 Nwat-MMGBSA 分析），与 12 节点 CPU 集群结果等价；每日可处理约 20 个化合物。
---
## 四、面向开发的工具链与扩展点
### 4.1 现成可用的开源工具
| 工具 | 功能 | 关键技术栈 |
|---|---|---|
| **AutoDock Vina** | 对接引擎 | C++（Boost），Python 绑定（SWIG）|
| **Smina** | Vina 分叉，26 种能量项，便于自定义打分 | C++ |
| **AutoDockTools (ADT) / Meeko** | PDBQT 准备 | Python |
| **AmberTools MMPBSA.py** | 标准 MM-GBSA 计算 | Python，依赖 AMBER |
| **gmx_MMPBSA** | GROMACS 轨迹的 MM-GBSA，开源，无需 AMBER 许可 | Python |
| **cpptraj** | MD 轨迹处理（`closest` 命令是 Nwat-MMGBSA 核心） | C++ |
| **Nwat-MMGBSA 官方脚本** | Contini 实验室发布的 bash/tcsh 工作流，论文补充材料提供 | Shell + AmberTools |
### 4.2 二次开发建议
**1. 自定义打分函数**：基于 Smina 的 26 种能量项（包括 gauss1/2、repulsion、hydrophobic、hbond、电静电项等），用线性回归/非线性回归在 PDBbind/CSAR 数据集上重新训练权重。Vinardo 就是这一路径的成功案例——通过"扫描-打分-再对接"的迭代流程，最终在所有测试集上超过原始 Vina。
**2. Vina + 机器学习融合**：Vina 1.2 的 Python 绑定支持程序化调用，可将 Vina score 作为特征之一，与 ML 模型（RF-Score、CNN）级联做两阶段筛选。
**3. Nwat-MMGBSA 自动化**：当前 Nwat-MMGBSA 的瓶颈是 MD 采样，可考虑：
- 用 **GROMACS + gmx_MMPBSA** 替代 AMBER，避免商业许可
- **缩短 MD 长度**：论文显示 1 ns 与 4 ns 结果等价，进一步优化平衡协议
- **并行化**：每个化合物独立 MD，天然可并行走 GPU 集群
- **Nwat 参数自动调优**：根据结合口袋的水密度图（论文用 `cpptraj` 做网格分析）自适应选择 Nwat
**4. 关键陷阱**：
- 熵项（正常模）计算成本 $O(N^3)$，建议在相对能量比较场景下忽略
- AM1-BCC 电荷对小代谢物的精度可能不足，关键体系可考虑 RESP 或 HF/6-31G* 电荷
- 显式水的"最近"定义需注意周期性边界条件（PBC）下的距离计算
- GB-Neck2 模型对带高电荷的代谢物（如磷酸化代谢物、氨基酸）仍可能偏差较大
### 4.3 数学公式复现路径
如需从零实现，建议按以下顺序：
1. **先实现 Vina 打分函数**（纯距离计算，无需力场），验证 PDBbind 复束能量复现
2. **加入 BFGS**（可用 GNU Scientific Library `gsl_multimin_fdfminimizer_vector_bfgs2`）
3. **实现 ILS 主循环**（带 Metropolis 准则）
4. **MM-GBSA 部分**：GB 方程（Born 半径计算是关键，推荐 GB-Neck2 参数化）+ SASA（LCPO 算法）+ 力场项（AMBER ff14SB 公式公开）
5. **Nwat 逻辑**：纯轨迹处理，用 MDTraj 或 MDAnalysis 即可在 Python 中实现
---
## 总结
Vina 的精髓在于 **"经验打分 + 梯度驱动的 ILS 全局优化"** 的极简组合，用机器学习的思路训练权重，而非硬拼物理势能项；MM-GBSA 的精髓在于 **"端点采样 + 力场 + GB 隐式溶剂 + SASA"** 的模块化组合，而 Nwat-MMGBSA 通过**保留 N 个最近显式水**这一最小侵入式修改，显著改善了对水桥介导的体系的预测能力。对开发者而言，Vina 源码 + Smina 分叉 + AmberTools/gmx_MMPBSA 这条开源路径已经提供了从对接到重打分的完整技术栈，最大的创新空间在于**打分函数与 ML 的融合**以及**Nwat 参数的自动化选择**。
