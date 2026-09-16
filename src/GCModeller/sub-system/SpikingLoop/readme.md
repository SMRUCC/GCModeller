# SNN脉冲神经网络算法原理
SNN通过模拟生物神经元的膜电位累积与脉冲发放机制，以离散脉冲序列作为信息载体进行时空计算，是第三代神经网络的核心范式，其关键特征是事件驱动、原生时序建模与极低功耗。
下面从与ANN的差异、神经元动力学、信息编码、学习规则四个层面为您系统拆解SNN的算法原理。
---
## 一、为什么需要SNN：与传统ANN的关键区别
传统ANN以连续浮点激活值为信息载体，逐层执行稠密矩阵乘法，依赖GPU的冯·诺依曼架构进行同步、密集计算，能效受限。SNN则从生物大脑中汲取灵感：神经元之间只在膜电位越过阈值时才发放离散的0/1脉冲，没有脉冲时几乎不消耗能量，并通过突触将"存储"与"计算"交织在一起。
| 特性 | ANN（CNN/Transformer） | SNN |
|---|---|---|
| 信息载体 | 连续浮点激活值 | 离散二值脉冲序列 |
| 计算范式 | 时钟驱动，稠密矩阵乘 | 事件驱动，稀疏激活 |
| 时间建模 | 需额外结构（RNN/TCN/Attention） | 原生时序动力学 |
| 能量效率 | GPU推理数百瓦 | μJ级推理，功耗低1–3个数量级 |
| 可导性 | 激活函数可导，支持反向传播 | 脉冲发放不可导，训练困难 |
| 硬件适配 | 通用GPU/TPU | 神经形态芯片 |
正是这种**"稀疏+事件驱动+时间维度"**的范式，让SNN在边缘端低功耗、异步时序信号处理场景下不可替代，但同时也带来了训练困难与精度损失的核心挑战。
---
## 二、神经元模型：从Hodgkin-Huxley到LIF
脉冲神经元是SNN的基本计算单元，其动力学模型决定了网络的计算精度与开销，主流模型按"生物真实性—计算成本"权衡如下。
### 2.1 Hodgkin-Huxley（HH）模型
最接近生物真实的模型，通过一组非线性微分方程显式描述Na⁺/K⁺离子通道的门控动力学，能精确复现动作电位的形态。其包含4个状态变量（膜电位V、m/h钠门、n钾门），计算代价极高，工程上几乎不直接用于大规模SNN。
### 2.2 LIF模型（核心使用）
LIF是精度与成本的平衡点，工业与学术最常用。连续形式为：
$$\tau_m \frac{du(t)}{dt} = -u(t) + R\,I(t)$$
其中 $u(t)$ 为膜电位，$\tau_m$ 为膜时间常数，$I(t)$ 为输入突触电流。当 $u(t) \geq V_{th}$ 时神经元发放脉冲，随后电位重置 $u \to V_{reset}$，并通常进入不应期。
在离散时间步 $t$ 下，LIF的递推形式可写作：
$$u[t] = \lambda\,u[t-1] + \sum_j w_{ij}\,s_j[t-1]$$
$$s_i[t] = \Theta(u[t] - V_{th}),\quad u[t] \leftarrow u[t]\,(1 - s_i[t]) + V_{reset}\,s_i[t]$$
其中 $\lambda = e^{-\Delta t/\tau_m}$ 为衰减系数，$w_{ij}$ 为突触权重，$s_j[t]\in\{0,1\}$ 为前突触脉冲，$\Theta$ 为Heaviside阶跃函数。硬重置将膜电位直接归零，软重置则减去阈值以保留残余信息。研究表明，泄漏机制在频域上起低通滤波作用，能提升SNN对噪声的鲁棒性与泛化能力。
### 2.3 Izhikevich模型
通过两个耦合常微分方程（膜电位 $v$ 与恢复变量 $u$）逼近HH的生物合理性，同时保持接近LIF的计算效率，能复现tonic、bursting等多种放电模式：
$$\frac{dv}{dt} = 0.04v^2 + 5v + 140 - u + I,\qquad \frac{du}{dt} = a(bv - u)$$
当 $v \geq 30$ 时发放脉冲，并执行 $v \leftarrow c$，$u \leftarrow u + d$。适合需要丰富动力学行为的应用，但梯度传播更复杂。
---
## 三、信息编码：连续值如何变成脉冲
SNN的输入与输出都需要在脉冲域中表达，编码方式直接决定了网络的能效与信息容量。
| 编码方式 | 原理 | 适用场景 |
|---|---|---|
| **频率编码** | 信息承载于脉冲发放频率，输入强度→单位时间脉冲数 | 静态图像分类（MNIST/CIFAR），最常用 |
| **时间编码** | 信息由脉冲精确时间戳传递，如Time-to-First-Spike | 高速动态视觉（DVS相机），低能耗 |
| **群体编码** | 多神经元协同发放模式表达信息 | 语音识别、多模态融合 |
**频率编码**实现简单但脉冲冗余、能耗高；**时间编码**（如TTFS）限制每个神经元最多发放一次，能将功耗降低数个数量级，但抗噪性弱、对时序敏感。例如在MNIST中，将像素亮度映射为脉冲延迟：亮像素早发放，暗像素晚发放，从而用极少脉冲完成编码。
---
## 四、学习规则：权重如何更新
脉冲函数 $\Theta(\cdot)$ 在阈值处不可导，使得标准反向传播无法直接应用，这是SNN训练的核心难题。当前主流有三种思路。
### 4.1 STDP：生物启发的局部无监督学习
STDP（Spike-Timing-Dependent Plasticity）基于突触前后脉冲的时间因果关系进行局部权重更新：若前突触先于后突触发放（$\Delta t = t_{post} - t_{pre} > 0$），权重增强（LTP）；反之权重抑制（LTD）。
$$\Delta w = \begin{cases} A_+\,\exp(-\Delta t / \tau_+), & \Delta t > 0 \\ -A_-\,\exp(\Delta t / \tau_-), & \Delta t < 0 \end{cases}$$
STDP完全局部、事件驱动、与神经形态硬件天然兼容，适合在线无监督学习；但难以训练深层网络，收敛慢、精度有限。
### 4.2 代理梯度法：监督学习的工程方案
用一段可导的平滑函数（如Sigmoid、ATen、Dspike）在反向传播时替代阶跃函数的导数，前向仍用真实脉冲：
$$\frac{\partial s}{\partial u}\Big|_{\text{surrogate}} = \sigma'\!\left(\frac{u - V_{th}}{\beta}\right)$$
沿时间步展开（BPTT）计算梯度。该方法兼容反向传播、支持任意深度结构、所需仿真步数少，是目前训练深层SNN的主流；但替代导数与真实阶跃导数存在"梯度不匹配"，误差随深度累积，导致深网精度仍逊于ANN。近期工作通过自适应替代梯度形状、Dspike等改进缓解该问题。
### 4.3 ANN-to-SNN转换：快速部署的折中
先用常规ReLU-ANN训练，再将其ReLU神经元替换为IF（Integrate-and-Fire）神经元、迁移权重，并做阈值归一化与权重缩放校准。其理论基础在于：在频率编码下，IF神经元的发放率与ReLU激活值渐近等价。
转换法无需重训、部署快，但丢失时序信息、需较多仿真步才能逼近ANN精度，且仅适用于ReLU类网络。
---
## 五、优势、局限与应用场景
**核心优势**：事件驱动使计算仅在脉冲发生时进行，结合二值脉冲与存算一体，使推理功耗可降至μJ级，比GPU低2–3个数量级；原生时序动力学使其天然适合异步时序信号处理；STDP等局部规则使其具备在线学习能力。
**主要局限**：脉冲不可导导致训练困难，依赖代理梯度近似；时间步仿真带来额外计算开销；算法成熟度落后，ImageNet上最高精度约90%，落后ANN约5%；深度网络存在梯度误差累积问题。
**典型应用**：神经形态芯片与硬件——Intel Loihi、SpiNNaker、清华天机芯片功耗降至传统GPU的1/1000量级；事件相机/动态视觉处理（无人机避障延迟<10ms、功耗<1W）；脑机接口（毫秒级脉冲解码）；边缘端低功耗部署（心电图分类、可穿戴设备）。
---
**底层逻辑总结**：SNN的本质是用**"膜电位累积→阈值发放→离散脉冲"**的动力学替代ANN的连续激活，用**稀疏事件驱动**替代稠密矩阵乘，用**STDP/代理梯度**适配不可导性。其能效优势源于计算与存储的局部化，训练困难则源于脉冲函数的非光滑性——这两点构成了SNN理论与实践的全部张力，也指明了未来混合架构（SNN-Transformer）、新型神经元（自适应阈值/重置）与硬件算法协同的突破方向。


# 基于脉冲神经网络回归的基因表达调控网络建模与虚拟扰动实验开发指南
> **用膜电位累积→阈值发放→离散脉冲的动力学替代连续激活，用代理梯度解决脉冲不可导问题，用先验网络（TF-Target + WGCNA）约束突触拓扑**。
---
## 〇、总体架构总览
```
┌─────────────────────────────────────────────────────────────┐
│                    离线数据准备阶段                            │
│  1. TF-Target有向调控网络（基因组分析/ChIP-seq/Motif）        │
│  2. WGCNA共表达网络（补充边，无向，含权重）                    │
│  3. Monocle3伪时间排序的基因表达矩阵（训练数据）               │
└──────────────────────┬──────────────────────────────────────┘
                       ▼
┌─────────────────────────────────────────────────────────────┐
│                  SNN-GRN 模型核心                             │
│  - 基因 = LIF神经元                                          │
│  - TF-Target = 有向突触（可训练权重）                         │
│  - WGCNA = 结构先验（初始化 + 正则化约束）                    │
│  - 输入编码：表达/扰动量 → 输入电流 I(t)                      │
│  - 输出解码：脉冲计数/膜电位 → 连续表达值（线性回归头）        │
│  - 训练：代理梯度 + BPTT + MSE损失                            │
└──────────────────────┬──────────────────────────────────────┘
                       ▼
┌─────────────────────────────────────────────────────────────┐
│                  虚拟扰动实验阶段                              │
│  在输入层注入扰动电流（敲除=置零/抑制，过表达=增强），         │
│  运行SNN演化，读取输出层的回归值作为扰动后表达谱               │
└─────────────────────────────────────────────────────────────┘
```
---
## 一、数据准备模块
### 1.1 构建先验图结构
**输入：**
- `TF_target_edges`：有向边集合 `(tf_gene, target_gene)`，可带调控方向/证据强度
- `wgcna_adj`：对称权重矩阵，`wgcna_adj[i][j] ∈ [0,1]`，表示基因 i、j 的共表达关联强度
**输出：**
- 邻接矩阵 `A[i][j]`（有向：基因 i 的脉冲传向基因 j 的神经元）
- 先验权重初始化矩阵 `W_init`
- 结构约束掩码 `mask`（允许梯度更新的突触位置）
**伪代码：**
```
FUNCTION build_prior_graph(TF_target_edges, wgcna_adj, gene_list, top_k_wgcna=20):
    N = LENGTH(gene_list)
    A = ZERO_MATRIX(N, N)          # 邻接矩阵（二值，表示突触存在）
    W_init = ZERO_MATRIX(N, N)     # 初始权重
    mask = ZERO_MATRIX(N, N)       # 梯度掩码
    # ---------- 第一步：TF-Target 有向边（基础骨架） ----------
    FOR EACH (tf, target, evidence) IN TF_target_edges:
        i = INDEX_OF(tf); j = INDEX_OF(target)
        A[i][j] = 1
        # 初始权重：若 wgcna 也有强关联，则用其强度；否则用证据强度
        W_init[i][j] = MAX(evidence, wgcna_adj[i][j])
        mask[i][j] = 1
    # ---------- 第二步：WGCNA 补充边（弱连接/双向） ----------
    FOR EACH PAIR (i, j) WHERE i < j:
        IF A[i][j] == 0 AND A[j][i] == 0:
            IF wgcna_adj[i][j] > THRESHOLD AND RANK_OF_CORR(i,j) <= top_k_wgcna:
                # 视为双向弱连接，作为结构正则化项
                A[i][j] = 1; A[j][i] = 1
                W_init[i][j] = 0.5 * wgcna_adj[i][j]
                W_init[j][i] = 0.5 * wgcna_adj[i][j]
                mask[i][j] = 1; mask[j][i] = 1
    # ---------- 第三步：稀疏化与归一化 ----------
    W_init = W_init * mask
    FOR EACH 行 i:                     # 按突触前神经元归一化（稳定发放率）
        total = SUM(W_init[i, :])
        IF total > 0: W_init[i, :] = W_init[i, :] / total
    RETURN A, W_init, mask
```
**关键设计要点（来自参考文本）：**
- 有向性冲突处理：TF-Target 作为有向边基础；WGCNA 仅作为"潜在连接存在性"约束或双向弱连接，不直接当突触主权重。
- 网络规模控制：先选几百~几千个核心基因（特定通路）做概念验证，不要直接上全基因组 2 万基因——BPTT 在超大规模稀疏图上梯度易消失/爆炸。
### 1.2 伪时间序列数据离散化
```
FUNCTION prepare_training_data(expr_matrix, pseudotime, n_bins=T):
    # expr_matrix: 细胞 × 基因 的表达矩阵（建议先做平滑/插补降低Dropout）
    # pseudotime:  每个细胞的伪时间值
    # 沿伪时间排序细胞
    order = SORT(pseudotime)
    X_sorted = expr_matrix[order, :]
    # 将连续伪时间等分为 T 个时间窗（= SNN 时间步）
    bin_edges = QUANTILE_BINNning(pseudotime[order], n_bins=T)
    # 每个时间窗内的细胞表达取平均（或用滑动窗口平滑）
    U_seq = ZERO_MATRIX(T, N_genes)
    FOR t IN 1..T:
        cells_in_bin = INDICES(pseudotime[order] IN bin t)
        U_seq[t, :] = MEAN(X_sorted[cells_in_bin, :])
    # 平滑处理（推荐：RNA velocity 辅助或 LOESS / 图正则化平滑）
    U_seq = SMOOTH_ALONG_TIME(U_seq)     # 缓解Dropout导致的虚假零值
    # 标准化到非负区间（便于编码为输入电流）
    U_seq = NORMALIZE_MINMAX(U_seq, to=[0, 1])
    RETURN U_seq      # 形状 T × N，即每个时间步的"基因表达状态"
```
**关键点：** 伪时间是相对量，必须离散化为 SNN 的离散时间步 t=1..T；SNN 的膜时间常数 τ 需与该离散步长匹配（后续作为超参数调优）。
---
## 二、模型定义模块
### 2.1 LIF 神经元（每个基因一个神经元）
离散递推形式（参考文本 2.2 节）：
```
u[t]   = λ * u[t-1] + Σ_j W[j][i] * s_j[t-1] + I_ext[t][i]
s_i[t] = Θ(u[t] - V_th)                     # 阶跃发放
u[t]   ← u[t] * (1 - s_i[t]) + V_reset * s_i[t]    # 重置（硬重置）
```
伪代码：
```
CLASS LIFNeuronLayer:
    PARAMETERS:
        W          # N × N 突触权重（被 mask 约束的可训练参数）
        lambda_    # 衰减系数，λ = exp(-Δt/τ_m)，可训练或固定为超参
        V_th       # 阈值，可设为自适应可训练参数
        V_reset    # 重置电位（硬重置: 0；软重置: u - V_th）
        surrogate_beta   # 代理梯度形状参数
    FORWARD(输入电流 I_ext, 时间步 T, 初始状态 u0):
        u = u0                     # 初始膜电位（可由 t=0 表达状态初始化）
        S = []                     # 脉冲序列
        FOR t IN 1..T:
            # ---------- 突触传递：稀疏矩阵乘 ----------
            # 仅当 A[j][i]=1 时才累加，保证先验拓扑约束
            synaptic = SPARSE_MATMUL(S[t-1], W, adjacency_mask=A)
            # 或用掩码实现：synaptic = (S[t-1] @ W) * A
            # ---------- 膜电位累积（泄漏 = 低通滤波，提升噪声鲁棒性） ----------
            u = lambda_ * u + synaptic + I_ext[t]
            # ---------- 阈值发放（前向用真实阶跃，反向用代理梯度） ----------
            s = STEP(u - V_th)                       # 前向：0/1 脉冲
            # s = SURROGATE_STEP(u - V_th)           # 反向梯度路径
            # ---------- 重置 ----------
            u = u * (1 - s) + V_reset * s            # 硬重置
            S.APPEND(s)
        RETURN S, u
```
### 2.2 代理梯度（核心难点，参考文本 4.2 节）
前向使用真实 Heaviside 阶跃（保证脉冲的稀疏二值性），反向用平滑函数替代其导数：
```
FUNCTION STEP(x):                       # 前向
    RETURN 1 IF x >= 0 ELSE 0
FUNCTION SURROGATE_GRADIENT(x, beta):   # 反向
    # 常用代理函数（任选其一）：
    #   Sigmoid 型:  σ'(x/β) = σ(x/β)*(1-σ(x/β))
    #   快速Sigmoid: 1 / (β*|x|+1)^2
    #   ATan 型:     β / (2*(1+(π*β*x/2)^2))
    RETURN SIGMOID_DERIVATIVE(x / beta)
```
工程实现上，这通过**自定义自动微分算子**完成：前向注册为阶跃，反向注册为代理导数。使用 `snnTorch` 或 `SpikingJelly` 框架可直接调用内置实现。
### 2.3 输入编码器（表达量/扰动量 → 脉冲或电流）
参考文本第三节：频率编码（简单、适合静态表达）或时间编码 TTFS（低能耗、抗噪弱）。对回归任务，推荐**直接编码为输入电流**（模拟恒流注入）或**频率编码**：
```
FUNCTION encode_input(expression_value_or_perturbation, mode):
    # mode = "current": 表达量直接作为输入电流（最简、适合回归）
    IF mode == "current":
        I_ext = SCALE(expression_value_or_perturbation, to=[0, I_max])
    ELIF mode == "rate":                  # 频率编码：高表达→高发放率
        FOR t IN 1..T:
            I_ext[t] = POISSON_SPIKES(rate = BASE_RATE + expression_value_or_perturbation * GAIN)
    ELIF mode == "ttfs":                  # 时间编码：值越大脉冲越早
        spike_time = INVERSE_MAP(expression_value_or_perturbation)
        I_ext = DELTA_PULSE_AT(spike_time)
    RETURN I_ext       # 形状 T × N
```
**扰动注入的设计：**
```
FUNCTION inject_perturbation(I_ext_baseline, gene_id, perturb_type, strength):
    # perturb_type ∈ {"KO", "KD", "OE", "none"}
    I = COPY(I_ext_baseline)
    IF perturb_type == "KO":     I[:, gene_id] = 0                    # 敲除：无输入
    ELIF perturb_type == "KD":   I[:, gene_id] *= (1 - strength)      # 敲低：按比例衰减
    ELIF perturb_type == "OE":   I[:, gene_id] += strength * I_MAX    # 过表达：增强电流
    RETURN I
```
### 2.4 输出解码器（脉冲序列 → 连续表达值）
SNN 输出是离散 0/1 脉冲，回归目标（log-normalized expression）是连续值，因此需要解码头（参考文本"时间窗口内脉冲计数或膜电位读取"）：
```
FUNCTION decode_output(S, u_final):
    # 方案A（推荐）：膜电位读取 —— 梯度路径更平滑，训练更稳
    y_hat = LINEAR_LAYER(u_final)                    # N × N 的线性映射到连续值
    # 方案B：脉冲计数 / 发放率解码
    # rate = SUM(S, axis=time) / T
    # y_hat = LINEAR_LAYER(rate)
    # 方案C：脉冲计数 + 膜电位拼接后线性回归
    # y_hat = LINEAR_LAYER(CONCAT(rate, u_final))
    RETURN y_hat          # 预测的下一时刻（或当前时刻）基因表达向量
```
---
## 三、训练模块
### 3.1 训练目标
**自回归式单步预测**：用 t 时刻的表达状态 + 输入电流，预测 t+Δt 时刻的表达：
```
损失 = MSE(y_hat[t], U_seq[t+Δt])  +  α * 结构正则化项  +  β * 稀疏正则化项
```
### 3.2 完整训练伪代码
```
FUNCTION train_snn_grn(U_seq, A, W_init, mask, epochs, T):
    # U_seq: T × N 伪时间离散化表达矩阵
    # A:     邻接掩码（先验拓扑）
    # mask:  可训练突触掩码
    W = PARAMETER(W_init * mask)         # 初始化为先验权重
    optimizer = ADAM(learning_rate=1e-3)
    FOR epoch IN 1..epochs:
        total_loss = 0
        FOR t IN 1 .. T - delta_t:
            # ---------- Step 1: 编码输入 ----------
            I_ext = ENCODE_INPUT(U_seq[t], mode="current")    # T_local × N
            #   若模拟扰动传播过程，可用多个时间步的持续电流
            # ---------- Step 2: SNN 前向演化 ----------
            u0 = MAP_TO_MEMBRANE(U_seq[t])     # 用当前表达状态初始化膜电位
            S, u_final = LIF_FORWARD(I_ext, u0, W, A)
            # ---------- Step 3: 解码回归 ----------
            y_hat = DECODE_OUTPUT(S, u_final)
            # ---------- Step 4: 损失计算 ----------
            loss_mse = MSE(y_hat, U_seq[t + delta_t])
            loss_prior = PRIOR_REGULARIZER(W, W_init, mask)
                # 例如: MEAN( ((W - W_init) * mask)^2 * confidence )
                #   高置信度边（TF-Target直接证据）惩罚更强，防漂移
            loss_sparse = L1(W) * sparsity_weight
            loss = loss_mse + alpha * loss_prior + beta * loss_sparse
            # ---------- Step 5: 反向传播（BPTT 沿时间步展开） ----------
            GRADIENTS = BACKWARD_THROUGH_TIME(loss, surrogate=SIGMOID_SURROGATE)
            #   脉冲发放节点的梯度 = 代理梯度
            #   稀疏掩码 A/mask 保持冻结，仅更新 W 的自由参数
            optimizer.APPLY_GRADIENTS(GRADIENTS)
            total_loss += loss_mse
        LOG(epoch, total_loss)
        # 早停/验证：留出部分伪时间区间或部分基因作验证集
    # 训练完成后：学习到的有效调控权重
    learned_W = W * mask
    RETURN learned_W, lambda_, V_th
```
### 3.3 梯度流关键细节
```
# 脉冲节点的自定义梯度（语言无关表述）：
#
#   前向:  s = H(u - V_th)
#   反向:  ∂s/∂u := σ'((u - V_th)/β)     # 替代真实导数（δ函数）
#
# 注意事项：
#   1. "梯度不匹配"误差会随深度/时间步累积 → 参考文本建议：
#      - 使用自适应替代梯度形状（如随训练动态调整β）
#      - 控制仿真时间步数，避免过长的BPTT链
#   2. 衰减系数λ应与伪时间步长匹配，作为超参数扫描（例如 τ_m ∈ {10ms, 20ms, 50ms} 的模拟对应值）
#   3. 重置方式选择：硬重置简单但丢失残余信息；软重置(u ← u - V_th)保留更多信息，
#      对回归任务通常更优，可对比实验
```
---
## 四、虚拟扰动实验模块
模型训练完成后，利用 SNN 的**递归动力学**进行虚拟扰动——只需在输入层修改特定基因的电流，网络自动沿突触级联传播扰动信号（这正是参考文本第五节强调的核心优势）：
```
FUNCTION virtual_perturbation_experiment(trained_model, U_seq, perturb_specs):
    # perturb_specs: [{gene: "GENE_X", type: "KO"}, {gene: "GENE_Y", type: "OE", strength: 0.8}]
    # ---------- 起始状态：基线表达 ----------
    u_state = MAP_TO_MEMBRANE(U_seq[baseline_timepoint])
    results = []
    FOR t IN 1..T_sim:     # 模拟时间步（可长于训练时的T，观察长时间演化）
        # ---------- Step 1: 基线输入电流 ----------
        I_ext = ENCODE_INPUT(CURRENT_EXPRESSION_ESTIMATE)
        # ---------- Step 2: 注入扰动（通常只在 t=0 或起始阶段注入） ----------
        IF t == 1:
            FOR EACH spec IN perturb_specs:
                g = INDEX_OF(spec.gene)
                IF spec.type == "KO":  I_ext[:, g] = 0
                ELIF spec.type == "KD": I_ext[:, g] *= (1 - spec.strength)
                ELIF spec.type == "OE": I_ext[:, g] += spec.strength * I_MAX
        # ---------- Step 3: SNN 演化（脉冲沿 TF→Target 有向突触级联传播） ----------
        S, u_state = LIF_FORWARD_STEP(I_ext, u_state, trained_W, A)
        # ---------- Step 4: 记录解码后的基因表达状态 ----------
        expr_state = DECODE_OUTPUT(S, u_state)
        results.APPEND(expr_state)
    # 输出：扰动后的伪时间表达轨迹（时空表达图谱）
    RETURN results      # 形状 T_sim × N
```
**批量扰动扫描（Perturb-seq 模拟）：**
```
FUNCTION batch_perturbation_scan(model, gene_list):
    all_results = {}
    FOR EACH candidate_gene IN gene_list:
        results = VIRTUAL_PERTURBATION_EXPERIMENT(model, U_seq, [{gene: candidate_gene, type: "KD"}])
        all_results[candidate_gene] = results
    RETURN all_results     # 可用于: 下游效应基因排序、关键调控节点识别
```
---
## 五、模型验证与评估模块
```
FUNCTION validate_model(model, holdout_data):
    # ---------- 1. 表达预测精度 ----------
    # 留出部分伪时间区间/独立数据集，计算预测表达 vs 真实表达
    R2, PCC, RMSE = REGRESSION_METRICS(y_hat, y_true)
    # ---------- 2. 调控关系合理性 ----------
    # 学习到的权重 learned_W 与已知 TF-Target 集合对比
    AUROC, AUPRC = LINK_PREDICTION_METRICS(learned_W, known_TF_target_set)
    # ---------- 3. 虚拟扰动有效性（最重要） ----------
    # 用真实 Perturb-seq 数据做基准：
    FOR EACH (knocked_gene, observed_signature) IN REAL_PERTURB_SEQ_DATA:
        predicted_signature = VIRTUAL_PERTURBATION_EXPERIMENT(
            model, U_seq, [{gene: knocked_gene, type: "KO"}])
        delta_pcc = PEARSON_CORR(predicted_signature - baseline,
                                 observed_signature - control)
        COLLECT(delta_pcc)
    # ---------- 4. 扰动传播时序合理性 ----------
    # 检查扰动信号是否按"上游先响应、下游延迟响应"的顺序传播
    CHECK_TIME_LAG_STRUCTURE(results, known_signaling_hierarchy)
    RETURN summary_report
```
---
## 六、实施路线图与风险清单
### 推荐开发顺序
| 阶段 | 任务 | 关键验证点 |
|---|---|---|
| P0 | 选定通路子网络（几百~几千基因） | 子网络覆盖目标生物学过程 |
| P1 | 数据管道：TF-Target + WGCNA + Monocle3 离散化 | U_seq 平滑无剧烈跳变 |
| P2 | 最小可行模型：固定权重（仅用 W_init）前向演化 | 脉冲传播路径合理，无全体持续发放/静默 |
| P3 | 加入代理梯度训练 | MSE 收敛，验证集 PCC > 0.6（经验目标） |
| P4 | 虚拟扰动实验 | 与已知扰动表型（文献/Perturb-seq）方向一致 |
| P5 | 规模化与消融实验 | 消融 WGCNA 先验 / τ 扫描 / 硬软重置对比 |
### 关键风险与对策（对照参考文本）
| 风险 | 依据（参考文本） | 对策 |
|---|---|---|
| 梯度不匹配随深度累积 | 4.2 节 | 自适应代理梯度形状（如 Dspike）；控制 BPTT 展开步数 |
| Dropout 导致虚假"无脉冲" | 二.2 节 | 预先平滑/插补 + RNA velocity 辅助，避免零值直接映射为无脉冲 |
| 伪时间 ≠ 真实时间 | 二.1 节 | τ_m 作为超参随伪时间步长调优；结果解释时避免绝对时间断言 |
| WGCNA 无向 vs SNN 有向冲突 | 一.1 节 | WGCNA 仅作存在性约束/正则化，方向由 TF-Target 决定 |
| 全基因组规模不可训练 | 一.2 节 | 子网络概念验证先行 |
| 脉冲冗余导致能耗/过拟合 | 三节 | 优先膜电位读取解码；必要时引入发放率正则化惩罚过度发放 |

---
## 七、一句话底层逻辑总结
> **以 LIF 动力学（膜电位累积→阈值发放→脉冲传播）为"计算引擎"，以 TF-Target 有向图 + WGCNA 权重为"结构先验"，以伪时间离散化表达为"训练轨迹"，以代理梯度+BPTT 拟合突触参数，最终通过在输入层注入/移除电流实现基因虚拟扰动——SNN 的事件驱动与原生时序特性，恰好对应基因调控的稀疏激活与延迟级联，这是该建模路线成立的根本依据。**
