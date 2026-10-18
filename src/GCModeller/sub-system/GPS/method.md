# 全基因组基因表达调控网络建模与虚拟扰动实验
## ——基于先验知识网络 + 时序表达矩阵 + GNN-Transformer融合模型的算法研发指导文档
---
## 文档信息
| 项目 | 内容 |
|------|------|
| **文档目的** | 指导算法研发人员从零构建一个全基因组范围的动态基因表达调控网络（GRN）模型，并基于该模型开展虚拟扰动实验 |
| **适用读者** | 正在学习算法开发、具备Python/PyTorch基础、了解基本生物学概念的研发人员 |
| **核心输入数据** | ① TF+motif扫描构建的先验调控网络；② 时间序列基因表达矩阵 |
| **核心输出** | ① 全基因组GRN模型；② 基因表达虚拟扰动（in-silico perturbation）预测系统 |
---
## 目录
1. [总体设计概述](#一总体设计概述)
2. [数据准备与预处理](#二数据准备与预处理)
3. [核心架构：GNN与Transformer的融合原理](#三核心架构gnn与transformer的融合原理)
4. [模型详细设计与伪代码](#四模型详细设计与伪代码)
5. [训练策略与损失函数](#五训练策略与损失函数)
6. [虚拟扰动实验设计与实现](#六虚拟扰动实验设计与实现)
7. [模型评估与验证](#七模型评估与验证)
8. [完整技术栈与工程实现建议](#八完整技术栈与工程实现建议)
9. [常见问题与调试指南](#九常见问题与调试指南)
10. [参考文献与延伸阅读](#十参考文献与延伸阅读)
---
## 一、总体设计概述
### 1.1 项目要解决的问题
**问题定义**：给定时间序列基因表达矩阵 $X \in \mathbb{R}^{N \times T}$（$N$个基因、$T$个时间点）和一个稀疏的先验调控网络 $\mathcal{G}_{prior} = (V_{prior}, E_{prior})$（$V_{prior}$为TF集合与靶基因，$E_{prior}$为TF→靶基因调控边），目标是：
1. **构建一个动态GRN模型**：能够预测任意时刻全基因组基因的表达状态；
2. **进行虚拟扰动**：在不需要湿实验的情况下，模拟敲除/过表达某基因后，全基因组表达状态如何变化。
### 1.2 为什么选择"GNN+Transformer融合"架构
单用GNN或单用Transformer都有明显短板，融合架构取长补短：
| 组件 | 优势 | 劣势 |
|------|------|------|
| **GNN（消息传递）** | ① 显式利用先验网络的拓扑结构；② 参数少、归纳偏置强，适合小数据；③ 局部邻居聚合高效 | ① 过平滑（层数深时所有基因表示趋同）；② 过挤压；③ 感受野局限于k跳邻居，难以捕捉长程调控 |
| **Transformer（全局注意力）** | ① 任意两个基因可直接交互，天然捕捉长程调控；② 表达能力强，无WL测试表达能力上限；③ 注意力权重可解释为"调控相关性" | ① 不知道图拓扑，纯注意力对稀疏生物网络浪费容量；② $O(N^2)$复杂度；③ 小数据易过拟合 |
| **融合（GPS范式）** | ① 局部消息传递 + 全局注意力并行，两者互补；② 复杂度可控；③ 对"先验网络覆盖率低"的场景尤其鲁棒 | 实现复杂度略高 |
### 1.3 整体架构总览图
```
┌─────────────────────────────────────────────────────────────────┐
│                        数据层                                    │
│  时序表达矩阵 X[N×T]  +  先验网络 G_prior(V_prior, E_prior)      │
└──────────────┬──────────────────────────────┬───────────────────┘
               ▼                              ▼
┌──────────────────────────────┐  ┌──────────────────────────────┐
│     图构建与编码模块            │  │      节点特征工程              │
│ · 邻接矩阵A(可学习,motif初始化)  │  │ · 滑窗切片: x[t-w+1..t]       │
│ · 拉普拉斯位置编码(LapPE)      │  │ · 度中心性编码(degree embedding)│
│ · 最短路径空间编码(SPD)         │  │ · GO/属性embedding(可选)       │
└──────────────┬───────────────┘  └──────────────┬───────────────┘
               ▼                                 ▼
┌─────────────────────────────────────────────────────────────────┐
│           GNN-Transformer融合主干（GPS范式，L层堆叠）               │
│  ┌─────────────────────────────────────────────────────────┐    │
│  │  第l层:                                                   │    │
│  │   ┌──────────────────┐   ┌────────────────────────┐       │    │
│  │   │ 局部消息传递(GCN/  │   │ 全局自注意力(带图偏置的   │       │    │
│  │   │ GAT/GINE)         │   │ Multi-head Attention)  │       │    │
│  │   │ —— 沿A_prior聚合  │ + │ —— + Graphormer偏置     │       │    │
│  │   └────────┬─────────┘   └───────────┬────────────┘       │    │
│  │            └────────────┬───────────┘                     │    │
│  │                         ▼                                 │    │
│  │              相加融合 + FFN + 残差 + LayerNorm               │    │
│  └─────────────────────────────────────────────────────────┘    │
└──────────────┬──────────────────────────────────────────────────┘
               ▼
┌──────────────────────────────┐
│     时序编码器                 │
│  GRU / TCN / Transformer      │
│  处理各时刻基因嵌入序列           │
└──────────────┬───────────────┘
               ▼
┌──────────────────────────────┐
│     输出层（基因特异性解码）      │
│  预测 X̂[t+1] (下一时刻表达)     │
│  + 不确定性 σ̂ (可选)           │
└──────────────────────────────┘
               ▼
┌──────────────────────────────┐
│   虚拟扰动模块（训练完成后使用）   │
│  敲除: A[:,k]=0, A[k,:]=0     │
│  过表达: x[k] += Δ            │
│  → 前向传播 → 预测扰动后表达      │
└──────────────────────────────┘
```
**设计核心思想**（借鉴GraphGPS范式与Graphormer）：
- **GNN部分**沿先验网络执行局部消息传递，保证模型尊重生物学先验；
- **Transformer部分**做全局注意力，允许先验网络之外的基因（如你的目标基因）之间直接建立联系；
- **Graphormer式偏置**把图结构信息注入注意力计算，使全局注意力"知道"哪些基因在先验网络中相邻；
- **时序编码器**捕捉基因表达随时间演化的动力学规律。
---
## 二、数据准备与预处理
### 2.1 输入数据规格
**输入1：时序表达矩阵**
```
X: numpy array, shape (N, T)
   N = 基因数（全基因组范围，如20000）
   T = 时间点数（如12个时间点）
   X[i, t] = 第i个基因在第t个时间点的表达量
```
**输入2：先验调控网络**
```
G_prior: 边列表
   每条边: (tf_id, target_id, weight)
   weight = motif结合得分（可作为边特征）
   注意：G_prior只覆盖很小一部分基因
```
### 2.2 预处理伪代码
```
函数 preprocess_data(X, G_prior, gene_names):
    # ========= 步骤1: 表达矩阵标准化 =========
    # 对每个时间点做文库归一化（TPM/CPM）
    for t in 1..T:
        X[:, t] = X[:, t] / sum(X[:, t]) * 1e6
    # log转换，稳定方差
    X = log2(X + 1)
    # 按基因做z-score标准化（跨时间）
    for i in 1..N:
        mu[i] = mean(X[i, :])
        sd[i] = std(X[i, :])
        X[i, :] = (X[i, :] - mu[i]) / (sd[i] + 1e-8)
    # ========= 步骤2: 基因集对齐 =========
    genes_all   = X的行索引（全基因组）
    genes_prior = G_prior中出现的基因
    # 先验图外的基因也纳入节点集（关键！）
    # 它们作为"孤立节点"参与训练，通过Transformer注意力获得信息
    N_total = |genes_all|
    # ========= 步骤3: 构建邻接矩阵 =========
    A_prior = zeros(N_total, N_total)  # 稀疏存储
    for (tf, target, w) in G_prior.edges:
        A_prior[tf_idx, target_idx] = w   # 有向：TF→靶基因
    # ========= 步骤4: 图结构编码 =========
    # 4a. 拉普拉斯位置编码（PE）
    L = D^{-1/2} (D - A_sym) D^{-1/2}   # 对称归一化拉普拉斯
    LapPE = 特征分解(L)取前K个最小非平凡特征向量    # shape (N, K)
    # 4b. 最短路径距离（SPD）用于空间编码
    SPD = floyd_warshall(A_prior_binarized)  # shape (N, N)
    SPD[SPD == inf] = MAX_DIST + 1  # 不连通的基因对
    # ========= 步骤5: 度中心性编码（Graphormer思路）=========
    deg_in[i]  = sum(A_prior[:, i])   # 入度：被多少TF调控
    deg_out[i] = sum(A_prior[i, :])   # 出度：调控多少靶基因
    # ========= 步骤6: 时序滑窗切片 =========
    windows = []
    for t in w..T-1:   # w = 窗口长度
        x_in  = X[:, t-w+1 : t+1]      # 输入: [t-w+1, ..., t]
        y_out = X[:, t+1]              # 标签: t+1时刻表达
        windows.append((x_in, y_out))
    return X, A_prior, LapPE, SPD, deg_in, deg_out, windows
```
**关键点说明**：
- **孤立节点问题**：先验网络外的基因在 `A_prior` 中对应的行列全为0，但它们仍保留在节点集中。它们的信息来源：①自身时序特征（通过时序编码器）；②Transformer全局注意力（可以和任何基因交互）。
- **SPD编码**对孤立基因对会返回"不连通"标记，模型会学到一个可学习的嵌入来表示这种关系，而不是报错。
- **LapPE** 使Transformer能够区分不同"图位置"的基因（否则注意力对节点顺序置换不变）。
---
## 三、核心架构：GNN与Transformer的融合原理
这是本文档最重要的部分，用从简到繁的方式讲解"如何融合"。
### 3.1 两种融合范式
#### 范式A：Graphormer式（图信息注入注意力）
Transformer本身对图结构"无感"，Graphormer通过三种编码把图信息注入注意力：
**① 中心性编码**——把节点度信息加到输入嵌入：
$$h_i^{(0)} = x_i + z^-_{\deg_{in}(i)} + z^+_{\deg_{out}(i)}$$
其中 $z^-, z^+$ 是可学习的度嵌入向量（入度和出度分别编码，因为有向图）。
**② 空间编码**——把最短路径距离转为注意力偏置：
$$b_{ij} = b_{\text{SPD}(i,j)} \quad\text{（可学习标量，查表得到）}$$
如果 $b$ 是SPD的递减函数，图上距离近的基因对注意力得分更高。
**③ 边编码**——把沿途边的特征编码进注意力：
$$c_{ij} = \frac{1}{n}\sum_{n=1}^{N} x_{e_n} \cdot w_n \quad\text{（最短路径上边的特征加权平均）}$$
**最终注意力公式**：
$$\text{Attention}(i,j) = \underbrace{\frac{Q_i K_j^T}{\sqrt{d}}}_{\text{标准自注意力}} + \underbrace{b_{\text{SPD}(i,j)}}_{\text{空间偏置}} + \underbrace{c_{ij}}_{\text{边编码偏置}}$$
#### 范式B：GPS式（局部消息传递与全局注意力并行）
GPS（General, Powerful, Scalable）把**MPNN层和Transformer层在同一层内并行执行后融合**，而不是串联：
$$h_i^{(l)} = \text{LocalMPNN}^{(l)}(h_i, \{h_j : j \in \mathcal{N}(i)\}) + \text{GlobalAttn}^{(l)}(h_i, \{h_j : \forall j\})$$
$$h_i^{(l+1)} = \text{FFN}^{(l)}(h_i^{(l)})$$
GPS的关键洞察：**MPNN负责沿真实边传播局部信息、把边特征"消化"进节点表示；全局注意力负责长程依赖；两者在每层互补**。
### 3.2 本项目的融合方案（A+B混合）
推荐方案：**GPS骨架 + Graphormer偏置**。理由：
- GPS的并行结构解决了GNN过平滑和Transformer无图感知两个问题；
- Graphormer的空间偏置让全局注意力知道先验网络拓扑，弥补GPS中注意力不直接看边的缺陷；
- 对孤立节点（先验图外基因），空间偏置的"不连通"标记 + 可学习嵌入，让它们也能参与注意力。
### 3.3 逐层计算流程（直觉版）
以第 $l$ 层为例，单个基因 $i$ 的表示更新过程：
```
第1步【局部消息传递】: 
    基因i从先验网络中的调控邻居(TF/靶基因)收集信息
    → 得到 h_local_i（"我的直接调控者说了什么"）
第2步【全局注意力】:
    基因i和全基因组所有基因计算注意力
    （注意力得分被SPD偏置调整：先验网络中近的基因权重略高）
    → 得到 h_global_i（"全基因组中谁和我相关"）
第3步【融合】:
    h_i = h_local_i + h_global_i
    h_i = FFN(LayerNorm(h_i))
    → 得到本层输出
```
堆叠 $L$ 层（建议 $L = 3 \sim 6$）后，每个基因的嵌入既包含局部调控上下文，也包含全局转录组上下文。
---
## 四、模型详细设计与伪代码
### 4.1 模型超参数配置
```python
CONFIG = {
    # —— 图与特征 ——
    'n_genes':        20000,   # 基因数N（按你的数据修改）
    'pe_dim':         16,      # 拉普拉斯位置编码维度
    'spd_max_dist':   5,       # SPD最大编码距离（超过截断）
    # —— 融合主干 ——
    'd_model':        256,     # 隐藏维度
    'n_layers':       4,       # GPS层数
    'n_heads':        8,       # 注意力头数
    'ffn_dim':        512,     # FFN中间维度
    'dropout':        0.1,
    # —— 时序 ——
    'window':         5,       # 输入时间窗w
    'time_encoder':   'gru',   # 'gru' / 'tcn' / 'transformer'
    'gru_hidden':     256,
    # —— 训练 ——
    'lr':             1e-4,
    'batch_size':     8,       # 按滑窗样本数调整
    'epochs':         300,
    'lambda_l1':      1e-4,    # 稀疏正则
    'lambda_prior':   1e-2,    # 先验距离正则
    'lambda_dir':     0.5,     # 方向感知损失权重
}
```
### 4.2 融合主干层（GPS + Graphormer偏置）伪代码
```
═══════════════════════════════════════════════════════
模块: GPSGraphormerLayer（一层融合层）
输入: 
    H        (N, d_model)    当前所有基因的节点嵌入
    A_prior  稀疏邻接矩阵      先验网络（固定）
    B_spd    (N, N)          空间偏置张量（可学习查找表，共享于所有层）
输出:
    H_out    (N, d_model)    更新后的节点嵌入
═══════════════════════════════════════════════════════
函数 GPSGraphormerLayer.forward(H, A_prior, B_spd):
    # ---------- 支路1: 局部消息传递（GNN） ----------
    # 对每个基因i，沿先验网络的边聚合邻居信息
    for i in 1..N:
        neighbors_in  = {j : A_prior[j,i] > 0}   # 调控i的TF
        neighbors_out = {j : A_prior[i,j] > 0}   # i调控的靶基因
        # 边感知的邻居消息（边特征=motif得分）
        m_in  = W_v · mean_{j∈neighbors_in} ( H[j] * edge_feat(j→i) )
        m_out = W_v · mean_{j∈neighbors_out}( H[j] * edge_feat(i→j) )
        # 门控更新（GINE风格：融合自身与邻居消息）
        h_local[i] = MLP_gate( H[i] + m_in + m_out )
    h_local = Dropout(h_local)
    h_local = h_local + H            # 残差连接
    h_local = LayerNorm_1(h_local)
    # ---------- 支路2: 全局自注意力（Transformer） ----------
    # 标准 QKV 投影
    Q = H · W_q        # (N, d_model)
    K = H · W_k
    V = H · W_v
    # 注意力得分 + Graphormer空间偏置   ★融合的核心★
    # scores[i,j] = Q_i·K_j/√d + B_spd[i,j]
    scores = (Q · K^T) / sqrt(d_model) + B_spd
    # softmax归一化，得到N×N注意力权重矩阵
    attn = softmax(scores, axis=-1)
    # 加权聚合所有基因的信息（全局感受野）
    h_global = attn · V                # (N, d_model)
    h_global = Dropout(h_global)
    h_global = h_global + h_local      # 残差
    h_global = LayerNorm_2(h_global)
    # ---------- 支路3: FFN ----------
    h_ffn = FFN(h_global)              # Linear→GELU→Linear
    H_out = h_global + Dropout(h_ffn)  # 残差
    H_out = LayerNorm_3(H_out)
    return H_out
```
**融合要点解读**（初学者必读）：
1. **`B_spd` 是GNN思想进入Transformer的关键通道**——它是一个可学习的查找表 `nn.Embedding(spd_max_dist+2, n_heads)`，按SPD查值。不连通的基因对查到"不存在"类别。训练后，直接先验邻居的偏置值通常为正（注意力被鼓励），不连通对的偏置接近0或负——**这就实现了"先验边软性引导注意力"**。
2. **两条支路是"加法融合"而非"串联"**——GPS论文证明并行融合优于"先GNN后Transformer"的串联（后者会因过平滑丢失早期信息）。
3. **残差+LayerNorm是深层Transformer的标准配置**，防止梯度消失。
### 4.3 完整模型伪代码
```
═══════════════════════════════════════════════════════
模块: GRNModel（完整模型）
═══════════════════════════════════════════════════════
class GRNModel:
    组件:
        gene_emb        : Embedding(N, d_model)          # 可学习基因嵌入
        pe_proj         : Linear(pe_dim, d_model)        # LapPE投影
        deg_in_emb      : Embedding(max_deg+1, d_model)  # 入度中心性编码
        deg_out_emb     : Embedding(max_deg+1, d_model)  # 出度中心性编码
        spd_bias        : Embedding(spd_max+2, n_heads)  # 空间偏置查找表
        gps_layers      : [GPSGraphormerLayer × L]       # 融合主干
        time_encoder    : GRU(d_model, gru_hidden)       # 时序编码器
        gene_decoder    : Linear(gru_hidden, 1) 每基因独立  # 基因特异性输出
        uncertainty_head: Linear(gru_hidden, 1)          # 不确定性(可选)
    函数 forward(x_seq, A_prior, LapPE, SPD, deg_in, deg_out):
        # x_seq: (N, w) 输入窗口内w个时间点的表达
        # 输出: x̂_next (N,) 下一时刻预测表达
        # ===== 步骤1: 构建初始节点嵌入 =====
        # 对每个时间点的表达向量，升维成节点特征
        T_steps = w
        node_feats = []
        for t in 1..w:
            x_t = x_seq[:, t]                        # (N,)
            x_t_proj = Linear_in(x_t)                # (N, d_model)
            # ★ Graphormer 中心性编码 ★
            h0_t = x_t_proj
                 + pe_proj(LapPE)                    # 图位置
                 + deg_in_emb[deg_in]                # 入度
                 + deg_out_emb[deg_out]              # 出度
            node_feats.append(h0_t)
        # node_feats: (w, N, d_model)
        # ===== 步骤2: 预计算注意力偏置 =====
        # B_spd[h, i, j] = spd_bias[h][SPD[i,j]]
        # 共享给所有层和头，只算一次（效率关键）
        B_spd = spd_bias_lookup(SPD)    # (n_heads, N, N)
        # ===== 步骤3: 融合主干逐时间步编码 =====
        gene_embeddings_over_time = []
        for t in 1..w:
            H = node_feats[t]                     # (N, d_model)
            for layer in gps_layers:
                H = layer(H, A_prior, B_spd)      # GPS+Graphormer融合
            gene_embeddings_over_time.append(H)   # (N, d_model)
        # gene_embeddings_over_time: (w, N, d_model)
        # ===== 步骤4: 时序编码 =====
        # 对每个基因，沿时间维做GRU
        # 变换维度为 (N, w, d_model) → 按基因逐个过GRU（批量化实现）
        for i in 1..N:
            seq_i = gene_embeddings_over_time[:, i, :]   # (w, d_model)
            h_T_i, _ = GRU(seq_i)                        # (gru_hidden,)
            gene_reps[i] = h_T_i
        # gene_reps: (N, gru_hidden)
        # ===== 步骤5: 基因特异性解码 =====
        x̂_next[i] = gene_decoder[i](gene_reps[i])       # 每基因独立线性层
        σ̂_next[i] = softplus(uncertainty_head(gene_reps[i]))  # 可选
        return x̂_next, σ̂_next
```
**为什么用"基因特异性解码器"？** 这是借鉴GEARS的设计：每个基因有自己的扰动响应模式，共享解码器会抹平这种异质性；独立线性层（`w_u ∈ R^{d}, b_u`）让每个基因学自己的"读出方式"。参数量增加 $N \times (d+1)$，对$N=20000$、$d=256$约500万参数，可接受。
### 4.4 数据流水线伪代码
```
═══════════════════════════════════════════════════════
模块: DataLoader（训练数据生成）
═══════════════════════════════════════════════════════
class TimeSeriesWindowDataset:
    函数 __init__(X, A_prior, LapPE, SPD, deg, window, mode):
        self.windows = 滑窗切片(X, window)   # 见2.2节步骤6
        # mode = 'train': 用前80%时间点生成的窗口
        # mode = 'test' : 用后20%时间点生成的窗口（严格按时间切分！）
    函数 __getitem__(idx):
        x_in, y_true = self.windows[idx]
        return (x_in, y_true, A_prior, LapPE, SPD, deg)
        # 注意: 图结构是共享的，实际工程中放collate_fn外
    函数 __len__():
        return len(self.windows)   # = T - window
```
**重要工程提示**：时序数据的时间点通常很少（$T \le 20$），滑窗后训练样本可能只有十几个。缓解手段：
1. **掩码重建自监督预训练**（见5.2节）；
2. **时间插值增强**：用样条插值在相邻真实时间点间生成伪时间点（仅用于训练，验证用真实点）；
3. **模型小型化**：$L=2\sim3$ 层、$d=128$ 起步，避免过拟合。
---
## 五、训练策略与损失函数
### 5.1 主任务损失：预测 + 方向感知
借鉴GEARS的autofocus direction-aware loss——普通MSE会被大量"不变化的基因"主导，而生物学上重要的是差异表达基因：
$$L_{\text{afocus}} = \frac{1}{N}\sum_{i=1}^{N}(y_i - \hat{y}_i)^{(2+\gamma)} \quad (\gamma \approx 1)$$
误差指数从2提升到$2+\gamma$，使大误差（差异基因）被指数级放大权重。
**方向感知损失**——预测变化方向错误要额外惩罚：
$$L_{\text{dir}} = \frac{1}{N}\sum_{i=1}^{N}\left[\text{sign}(y_i - y_i^{t}) - \text{sign}(\hat{y}_i - y_i^{t})\right]^2$$
### 5.2 完整损失函数与训练循环
```
═══════════════════════════════════════════════════════
函数: compute_total_loss(y_true, ŷ, A_learned, A_prior, y_prev)
═══════════════════════════════════════════════════════
    L_afocus = mean( |y_true - ŷ|^(2+γ) )              # autofocus
    L_dir    = mean( (sign(y_true - y_prev)
                     - sign(ŷ      - y_prev))^2 )      # 方向感知
    L_l1     = ||A_learned||_1                          # 稀疏正则
    L_prior  = ||A_learned - A_prior||_F^2              # 先验距离正则
    L = L_afocus + λ_dir·L_dir + λ_l1·L_l1 + λ_prior·L_prior
    return L
```
```
═══════════════════════════════════════════════════════
阶段1: 掩码重建自监督预训练（时间点少时强烈推荐）
═══════════════════════════════════════════════════════
for epoch in 1..E_pretrain:
    for batch in dataloader:
        x_seq = batch                     # (N, w)
        # 随机掩码15%基因的最后一帧表达
        mask_idx = random_sample(N, ratio=0.15)
        x_masked = x_seq.clone()
        x_masked[mask_idx, -1] = MASK_TOKEN  # 可学习掩码向量
        h = model.encode(x_masked, ...)       # 冻结解码，只用主干
        x̂_recon = reconstruction_head(h)      # 重建被掩基因
        loss_pre = MSE(x̂_recon[mask_idx], x_seq[mask_idx, -1])
        backward_and_step(loss_pre)
```
```
═══════════════════════════════════════════════════════
阶段2: 下一时刻预测微调（主任务）
═══════════════════════════════════════════════════════
model.load_pretrained_weights()
for epoch in 1..E_finetune:
    for (x_in, y_true) in train_windows:
        ŷ, σ̂ = model.forward(x_in, A_prior, LapPE, SPD, deg)
        # 可选: 贝叶斯不确定性损失（GEARS式）
        L_unc = mean( exp(-log(σ̂)) · |y_true - ŷ|^(2+γ) + log(σ̂) )
        L = compute_total_loss(y_true, ŷ, A_learned, A_prior, x_in[:,-1])
        optimizer.zero_grad()
        L.backward()
        clip_grad_norm_(model.parameters(), 1.0)   # 梯度裁剪
        optimizer.step()
    # 按时间切分的验证集上早停
    val_mse = evaluate_on_test_windows()
    scheduler.step(val_mse)
    if val_mse > best_mse for patience epochs:
        break
```
**关于可学习邻接矩阵的说明**：如果希望模型在训练中精炼先验网络，可把 `A_learned` 设为 `nn.Parameter`，初始化为 `A_prior`，并加入上面的L1和先验距离正则。**但鉴于你的先验覆盖率很低、目标基因在图外，建议第一阶段先固定 `A_prior`（只进消息传递和偏置），确认基线可行后再实验可学习版本**——图结构学习在不完整先验下容易退化。
---
## 六、虚拟扰动实验设计与实现
模型训练完成后，进入本项目的最终目标：**虚拟扰动**。核心思路借鉴GEARS和scTenifoldKnk两类方法：
### 6.1 三类虚拟扰动操作
| 扰动类型 | 模拟方式 | 对应生物学场景 |
|---------|---------|--------------|
| **敲除** | 将该基因出边置零 + 表达置零 | CRISPR knockout |
| **敲低** | 该基因表达 × (1 - knockdown%) | CRISPRi / siRNA |
| **过表达** | 该基因表达 + Δ | CRISPRa / 转基因 |
### 6.2 扰动预测的两种模式
**模式一：单步扰动（表达式扰动）**
```
═══════════════════════════════════════════════════════
函数: virtual_perturb_expression(model, x_seq, target_gene, 
                                  perturb_type, strength)
═══════════════════════════════════════════════════════
    x_seq_pert = x_seq.clone()
    if perturb_type == 'knockout':
        x_seq_pert[target_gene, :] = 0
    elif perturb_type == 'knockdown':
        x_seq_pert[target_gene, :] *= (1 - strength)   # strength∈(0,1)
    elif perturb_type == 'overexpression':
        x_seq_pert[target_gene, :] += strength * std(X[target_gene])
    # 模型前向传播，预测扰动后的下一时刻表达
    ŷ_pert, σ̂_pert = model.forward(x_seq_pert, ...)
    # 同时计算无扰动基线
    ŷ_ctrl, _ = model.forward(x_seq, ...)
    # 扰动效应 = 差异表达向量
    DE = ŷ_pert - ŷ_ctrl
    return {
        'predicted_DE':    DE,                    # 全基因组变化
        'top_up_genes':    argsort_desc(DE)[:50], # 上调最显著基因
        'top_down_genes':  argsort_asc(DE)[:50],  # 下调最显著基因
        'uncertainty':     σ̂_pert,                # 置信度
    }
```
**模式二：网络扰动（调控边删除，scTenifoldKnk式）**
如果模型含可学习邻接矩阵 `A_learned`，可以模拟"删除某基因的所有调控输出"：
```
═══════════════════════════════════════════════════════
函数: virtual_knockout_network(model, A_learned, target_gene)
═══════════════════════════════════════════════════════
    A_KO = A_learned.clone()
    A_KO[target_gene, :] = 0      # 删除该基因的所有出边（调控输出）
    # 用扰动后的网络重新做前向传播（从同一初始状态出发）
    ŷ_KO, _ = model.forward(x_seq, A_perturbed=A_KO, ...)
    ŷ_WT, _ = model.forward(x_seq, A_perturbed=A_learned, ...)
    DE = ŷ_KO - ŷ_WT
    return DE, top_genes_by_|DE|
```
### 6.3 组合扰动与多步模拟
```
═══════════════════════════════════════════════════════
函数: virtual_perturb_combinatorial(model, x_seq, gene_list)
    # 模拟多基因联合扰动（捕捉非加性效应/synergy）
═══════════════════════════════════════════════════════
    # 1. 联合扰动的预测
    x_pert = apply_all_perturbations(x_seq, gene_list)
    ŷ_joint, _ = model.forward(x_pert, ...)
    # 2. 单基因扰动分别预测（用于加性基线）
    ŷ_single = []
    for g in gene_list:
        x_1 = apply_perturbation(x_seq, g)
        ŷ_1, _ = model.forward(x_1, ...)
        ŷ_single.append(ŷ_1)
    # 3. 加性期望 = 单基因效应之和（Bliss独立性假设）
    ŷ_additive = ŷ_ctrl + Σ_g (ŷ_single[g] - ŷ_ctrl)
    # 4. 非加性效应（协同/拮抗）
    synergy = ŷ_joint - ŷ_additive
    return {
        'joint_result':  ŷ_joint,
        'additive_expect': ŷ_additive,
        'synergy_score':   ||synergy||_∞,   # 显著非零→存在协同/拮抗
        'synergy_genes':   top genes by |synergy|
    }
```
```
═══════════════════════════════════════════════════════
函数: simulate_temporal_cascade(model, x_seq, target_gene, n_steps)
    # 多步递归预测：扰动效应随时间的传播级联
═══════════════════════════════════════════════════════
    x_current = apply_perturbation(x_seq, target_gene)
    trajectory = [x_current]
    for t in 1..n_steps:
        ŷ_next, σ̂ = model.forward(x_current_window, ...)
        trajectory.append(ŷ_next)
        x_current = shift_window(x_current, ŷ_next)  # 滑窗前进
    # trajectory: (n_steps+1, N) 扰动后时间轨迹
    # 可分析: 哪些基因最早响应（直接靶点）
    #         哪些基因延迟响应（间接下游效应）
    return trajectory
```
### 6.4 虚拟扰动结果的解读规范
| 观察对象 | 解读 | 注意事项 |
|---------|------|---------|
| `top_down_genes` 中富集的通路 | 该基因敲除后主要抑制的生物学过程 | 需做GO/KEGG富集检验 |
| `synergy_genes` | 潜在协同治疗靶点组合 | 非加性效应是模型外推，不确定性最高 |
| 时间级联中的早期响应基因 | 可能的直接调控靶标 | 需motif回检支持 |
| `uncertainty` 高的区域 | 模型没把握，**仅作假设不做结论** | 优先选低不确定性的预测做验证 |
**必须强调**：虚拟扰动的预测本质是模型在训练分布内的插值/外推，不构成因果证据。任何进入实验验证的候选都应先经过：①不确定性过滤；②独立数据库（ChIP-seq/TRRUST等）交叉印证；③与经典方法（CellOracle、scTenifoldKnk）结果的一致性检查。
---
## 七、模型评估与验证
### 7.1 数据划分（严格时间切分）
```
时间点:  t1  t2  t3  t4  t5  t6  t7  t8  t9  t10
划分:    |←—————— 训练 ——————→|← 验证 →|← 测试 →|
         (滑窗样本)              (早停)    (最终报告)
```
**绝不允许**随机打乱时间窗口做划分——这会造成时间信息泄漏，虚高评估结果。
### 7.2 评估指标体系
```
═══════════════════════════════════════════════════════
函数: evaluate_model(model, test_windows)
═══════════════════════════════════════════════════════
    # —— 1. 表达预测精度 ——
    MSE_all  = mean((y_true - ŷ)^2)              # 全基因
    R2       = 1 - MSE/Var(y_true)                # 决定系数
    Pearson_r = corr(y_true, ŷ)                   # 全基因组相关性
    # —— 2. 差异基因捕捉能力（GEARS式评估）——
    # 只看真实变化最大的top-20基因上模型的表现
    for each window:
        top20 = argsort_by_|y_true - y_prev|[:20]
        MSE_top20 = MSE restricted to top20      # 硬任务指标
    # —— 3. 方向准确性 ——
    dir_acc = mean( sign(ŷ-ŷ_prev) == sign(y-y_prev) )
    # —— 4. 虚拟扰动合理性（间接验证）——
    # 敲除已知必需基因（如核糖体蛋白）→ 预测广谱下调
    # 敲除某TF → 预测靶基因（先验网络中）应显著变化
    sanity_pass_rate = proportion_of_expected_responses
    return metrics_dict
```
### 7.3 消融实验清单（证明每个组件的贡献）
| 消融配置 | 目的 |
|---------|------|
| 完整模型 vs **纯时序模型**（去掉图和注意力） | 证明图信息有增量 |
| 完整模型 vs **纯GNN**（去掉全局注意力） | 证明Transformer有增量 |
| 完整模型 vs **纯Transformer**（去掉局部MPNN） | 证明GNN有增量 |
| 完整模型 vs **去掉Graphormer偏置** | 证明空间偏置有增量 |
| 完整模型 vs **去掉中心性编码** | 证明度编码有增量 |
| **基因特异性解码器 vs 共享解码器** | 证明解码器设计 |
特别是针对你的应用场景（目标基因在先验图外），务必单独报告：**图外目标基因子集上的预测R²**，并与“自身历史AR基线”对比——这是判断GNN-Transformer是否真正为图外基因带来了跨基因信息的直接证据。
---
## 八、完整技术栈与工程实现建议
### 8.1 推荐技术栈
```
语言/框架:   Python 3.10+, PyTorch 2.x
图库:       PyTorch Geometric (PyG) —— 内置GPSConv、Graphormer层
            或 DGL（内置Graphormer实现）
时序:       PyTorch内置 nn.GRU / 自实现TCN
数据处理:   numpy, pandas, scanpy（若为单细胞数据）
图算法:     networkx（最短路径、连通分量）
基线对比:   GENIE3 / GRNBoost2（R或Python实现）
富集分析:   gseapy (Python版GSEA/GO富集)
实验管理:   Weights & Biases 或 TensorBoard
```
### 8.2 模块化代码结构建议
```
project/
├── data/
│   ├── expression_matrix.h5ad      # 时序表达数据
│   └── prior_network.tsv           # TF-motif先验边
├── src/
│   ├── preprocessing.py            # 第2章逻辑
│   ├── graph_utils.py              # LapPE/SPD/度计算
│   ├── model/
│   │   ├── gps_layer.py            # GPSGraphormerLayer (4.2节)
│   │   ├── grn_model.py            # GRNModel (4.3节)
│   │   └── time_encoder.py         # GRU/TCN封装
│   ├── losses.py                   # 第5章损失函数
│   ├── train.py                    # 两阶段训练循环
│   ├── perturbation.py             # 第6章虚拟扰动
│   └── evaluation.py               # 第7章评估
├── configs/config.yaml
└── notebooks/                      # 探索性分析
```
### 8.3 内存与计算优化要点
- **稀疏邻接**：`A_prior` 用 `torch.sparse` 或边列表存储，全基因组$N=20000$的稠密邻接矩阵占$20000^2 \times 4B \approx 1.6GB$，必须稀疏化；
- **注意力复杂度**：全局注意力$O(N^2)$在$N=20000$时不可行。解决方案按优先级：
  1. **分块注意力**：只在"先验邻居 + top-k相似基因"间做注意力（稀疏注意力，如Exphormer思路）；
  2. **线性注意力**：Performer核注意力，$O(N)$；
  3. **基因预过滤**：先用方差/共表达筛选保留高变异基因（如5000个）建模；
- **混合精度训练**（`torch.cuda.amp`）加速一倍以上；
- **图编码预计算**：LapPE、SPD、度只需算一次，缓存到磁盘。
---
## 九、常见问题与调试指南
| 问题 | 现象 | 排查与解决 |
|------|------|----------|
| **过拟合** | 训练MSE↓但测试MSE↑ | 减小层数/维度、增大dropout、加L1正则、掩码预训练 |
| **孤立基因预测差** | 图外基因R²显著低于图内 | 检查SPD编码是否覆盖不连通对；增大注意力对特征相似性的依赖（减小B_spd影响）；加入基因属性embedding |
| **注意力塌缩** | 所有注意力权重趋近均匀 | 检查B_spd初始化（加温度系数）、降低学习率、检查QKV初始化 |
| **GNN支路被忽略** | 消融中GNN贡献≈0 | 检查先验边是否有信号（先单独跑GCN基线验证）；调大MPNN支路初始化权重 |
| **梯度爆炸** | loss为NaN | 梯度裁剪（clip=1.0）、降低lr、检查log变换是否做了 |
| **虚拟扰动结果不敏感** | 敲除后DE≈0 | 确认扰动注入位置（输入端 vs 邻接端）；检查该基因的出边数（出边为0的基因敲除当然无网络效应）|
| **虚拟扰动过于敏感** | 敲除任何基因都天翻地覆 | 检查是否训练数据量太小导致模型外推失控；用不确定性输出过滤；限制扰动幅度 |
**调试顺序建议**（学习路径）：
1. 先跑通"纯GRU自身历史预测"基线 → 建立性能下界；
2. 加固定图GNN（GCN）→ 验证先验网络有信号；
3. 换成GPS融合层 → 验证融合有增量；
4. 加Graphormer偏置和中心性编码 → 精调；
5. 上虚拟扰动模块 → 结合生物学知识检验合理性。
---
## 十、参考文献与延伸阅读
**核心方法来源**：
- **GraphGPS**（Rampášek et al. 2022）：GPS融合范式的原始论文与代码，本文主干架构的直接依据
- **Graphormer**（Ying et al. 2021）：中心性/空间/边编码注入注意力的原创工作，"GNN与Transformer融合"的里程碑
- **GEARS**（Roohani et al. 2023, *Nature Biotech*）：虚拟扰动预测的标杆模型，本文扰动模块与autofocus损失的设计来源
- **scTenifoldKnk**（Osorio et al. 2022, *Patterns*）：基于网络边删除的虚拟敲除范式，本文模式二扰动的参考
**工具文档**：
- PyTorch Geometric Graph Transformer教程：`pytorch-geometric.readthedocs.io`（GPSConv可直接使用）
- DGL Graphormer实现：`dgl.ai`（SpatialEncoder、BiasedMHA开箱即用）
- GEARS官方代码：`github.com/snap-stanford/GEARS`
- scTenifoldKnk：`github.com/cailab-tamu/scTenifoldKnk`（含R/Python/MATLAB实现）
---
## 附录A：关键公式速查表
| 公式 | 含义 | 出处章节 |
|------|------|---------|
| $\text{Attn}(i,j) = \frac{Q_iK_j^T}{\sqrt{d}} + b_{\text{SPD}(i,j)} + c_{ij}$ | 带图偏置的注意力（融合核心） | 3.1 |
| $h_i^{(0)} = x_i + z^-_{\deg_{in}} + z^+_{\deg_{out}}$ | 中心性编码 | 3.1 |
| $h^{(l)} = \text{MPNN}^{(l)}(h) + \text{GlobalAttn}^{(l)}(h)$ | GPS并行融合 | 3.1 |
| $L = L_{\text{afocus}} + \lambda_{dir}L_{dir} + \lambda_1\|A\|_1 + \lambda_p\|A - A_{prior}\|_F^2$ | 总损失 | 5.1 |
| $L_{\text{unc}} = \mathbb{E}[\exp(-s)\cdot\|y-\hat{y}\|^{2+\gamma} + s]$ | 贝叶斯不确定性损失 | 5.2 |
| $synergy = \hat{y}_{joint} - \hat{y}_{additive}$ | 非加性组合扰动效应 | 6.3 |
## 附录B：从零实现的最小学习路线（4周计划）
| 周次 | 任务 | 里程碑 |
|------|------|--------|
| 第1周 | PyTorch基础 + 数据预处理管线 + 图编码（LapPE/SPD） | 能可视化先验网络和位置编码 |
| 第2周 | 单层GPSGraphormerLayer实现 + 小规模过拟合测试 | 在10个基因子图上训练收敛 |
| 第3周 | 完整模型 + 两阶段训练 + 消融实验 | 全基因组预测R²报告完成 |
| 第4周 | 虚拟扰动模块 + 富集分析 + 结果解读 | 端到端demo + 扰动预测报告 |
---
*文档版本：v1.0 | 本文档基于GraphGPS、Graphormer、GEARS、scTenifoldKnk等方法框架整理，结合"小覆盖率先验网络 + 时序表达 + 无扰动湿实验"的具体数据条件定制。实施中请以实验结果为准，动态调整架构与超参数。*