# 图神经网络实现基因表达调控网络虚拟扰动的算法原理
## 一、整体框架：为什么用 GNN 做虚拟扰动
基因调控网络（GRN）天然就是一个**有向图**结构：基因是节点，调控关系（激活/抑制）是有向边。当敲降某个基因时，扰动效应并非孤立的——它会沿着网络**级联传播**：TF-A 敲低 → 其直接靶基因 B、C 表达变化 → B 的下游靶基因 D、E 跟着变化 → 继续向下游传递。
图神经网络（GNN）的核心能力恰好是**在图结构上传播信息**，这与扰动的生物学级联效应在结构上同构。因此 GNN 可以把“扰动如何在网络中传播”这一过程参数化并学习。
---
## 二、算法流程全景
```
┌─────────────────────────────────────────────────────────┐
│  Step 1: 构建基因-基因调控图 G = (V, E)                  │
│          节点 V: 所有基因; 边 E: 调控关系(带方向和类型)  │
├─────────────────────────────────────────────────────────┤
│  Step 2: 初始化节点特征 h_i^(0)                          │
│          = 基础表达值 + 基因身份嵌入 + 扰动标记          │
├─────────────────────────────────────────────────────────┤
│  Step 3: 多轮消息传递 (GNN Layers)                       │
│          每轮: 聚合邻居信息 → 更新自身嵌入               │
│          → 扰动信号沿调控边向下游传播                    │
├─────────────────────────────────────────────────────────┤
│  Step 4: 解码预测                                        │
│          扰动后的节点嵌入 → MLP → 预测Δ表达(log fold change)│
├─────────────────────────────────────────────────────────┤
│  Step 5: 训练与泛化                                      │
│          用已有 Perturb-seq 数据训练，泛化到未见扰动     │
└─────────────────────────────────────────────────────────┘
```
---
## 三、Step 1：构建基因-基因调控图
### 3.1 图的来源
基因间调控边的来源通常有三种渠道：
| 来源 | 举例 | 特点 |
|---|---|---|
| **通路数据库** | KEGG、Reactome、BioGRID | 有方向和类型（激活/抑制），但覆盖有限 |
| **蛋白质相互作用** | STRING、IntAct | 多为物理结合关系，无方向 |
| **共表达/推断网络** | 从 scRNA-seq 用 GENIE3/GRNBoost2 推断 | 有方向权重，但可能有假阳性 |
**GEARS 的做法**：融合多个来源，为每条边标注“关系类型”（如 activation / inhibition / physical interaction），构建**异质图**（heterogeneous graph）。
### 3.2 图的数学表示
设图 $G = (V, E)$，包含 $N$ 个基因节点和 $M$ 条有向边。
- 邻接矩阵（带权重和方向）：$\mathbf{A} \in \mathbb{R}^{N \times N}$
- 边类型矩阵：$\mathbf{R} \in \mathbb{R}^{N \times N \times K}$（$K$ 种关系类型）
- 对于节点 $i$，其入边邻居集合（调控 $i$ 的基因）：$\mathcal{N}_{in}(i)$
- 其出边邻居集合（被 $i$ 调控的基因）：$\mathcal{N}_{out}(i)$
---
## 四、Step 2：节点特征的构建与扰动编码
### 4.1 基础节点特征
每个基因 $i$ 在 control 状态下有一个初始特征向量：
$$h_i^{(0)} = [\, \bar{x}_i \;\|\; e_i \,]$$
其中：
- $\bar{x}_i$：基因 $i$ 在 control 下的平均表达值（标量或分桶离散化后的向量）
- $e_i \in \mathbb{R}^{d}$：基因身份的可学习嵌入（类似 word embedding），维度 $d$ 通常为 64–256
### 4.2 扰动编码
对于一次虚拟扰动（如敲降基因 $k$），引入**扰动指示变量**：
$$p_i = \begin{cases} 1, & i = k \;\text{(被扰动)} \\ 0, & \text{otherwise} \end{cases}$$
对于**组合扰动**（同时敲降基因 $k_1, k_2, \ldots, k_m$），则为多热向量：
$$\mathbf{p} = [0, \ldots, 1_{k_1}, \ldots, 1_{k_2}, \ldots, 1_{k_m}, \ldots, 0]$$
最终输入 GNN 的节点特征为：
$$h_i^{(0)} = [\, \bar{x}_i \;\|\; e_i \;\|\; p_i \,]$$
**关键设计**：扰动基因自身的表达值 $\bar{x}_k$ 也会被修改（置为 0 或显著降低），这个“改变”会作为信息在网络中传播。
---
## 五、Step 3：消息传递——扰动信号的网络级传播
这是 GNN 实现虚拟扰动的**核心机制**。
### 5.1 单层消息传递的数学表达
在第 $l$ 层 GNN 中，节点 $i$ 的更新公式（以带注意力机制的 GAT 为例）：
$$m_{j \to i}^{(l)} = \alpha_{ij}^{(l)} \cdot W^{(l)} h_j^{(l)}$$
$$h_i^{(l+1)} = \sigma\left( W_{self}^{(l)} h_i^{(l)} + \sum_{j \in \mathcal{N}(i)} m_{j \to i}^{(l)} \right)$$
其中：
- $m_{j \to i}^{(l)}$：节点 $j$ 传给节点 $i$ 的消息（message）
- $\alpha_{ij}^{(l)}$：注意力权重，衡量邻居 $j$ 对 $i$ 的重要程度
- $W^{(l)}, W_{self}^{(l)}$：可学习的变换矩阵
- $\sigma(\cdot)$：非线性激活函数（如 ELU、ReLU）
### 5.2 注意力权重的计算
$$\alpha_{ij}^{(l)} = \frac{\exp\left( \text{LeakyReLU}\left( a^{T} [W h_i^{(l)} \| W h_j^{(l)}] \right) \right)}{\sum_{k \in \mathcal{N}(i)} \exp\left( \text{LeakyReLU}\left( a^{T} [W h_i^{(l)} \| W h_k^{(l)}] \right) \right)}$$
**生物学含义**：$\alpha_{ij}$ 可以理解为“在当前扰动条件下，基因 $j$ 对基因 $i$ 的调控强度”。注意力机制让模型可以**动态调整**不同调控边的重要性——例如当 TF-A 被敲除后，原本受 A 强调控的边变得重要，而受其他通路代偿的边权重可能降低。
### 5.3 多层传播 = 多跳间接效应
| GNN 层数 | 能捕捉的效应 | 生物学对应 |
|---|---|---|
| 1 层 | 直接邻居的信息 | 一级靶基因（直接结合调控） |
| 2 层 | 2-hop 邻居的信息 | 二级靶基因（通过中间基因介导） |
| $L$ 层 | $L$-hop 邻居的信息 | $L$ 级间接调控效应 |
以一个具体场景说明：
```
假设调控路径:  TF-A → 基因B → 基因C → 基因D
Layer 1 消息传递：
  h_B ← f(h_A, h_B)     // B 感知到 A 被敲降
  h_C ← f(h_C)          // C 还未感知（B 还未变化）
Layer 2 消息传递：
  h_C ← f(h_B, h_C)     // B 的变化传递到 C
  h_D ← f(h_D)          // D 还未感知
Layer 3 消息传递：
  h_D ← f(h_C, h_D)     // C 的变化传递到 D
→ 经过 3 层 GNN，D 的嵌入中已包含"A 被敲降"的信息
```
### 5.4 带边类型感知的消息传递（异质图）
对于异质图（不同边类型），消息传递需要区分激活/抑制关系：
$$m_{j \to i}^{(l)} = \alpha_{ij} \cdot W_{r_{ij}}^{(l)} h_j^{(l)}$$
其中 $r_{ij}$ 是边 $(j,i)$ 的关系类型索引，$W_{r_{ij}}^{(l)}$ 是该类型的专属变换矩阵。这使得模型能够学习到：
- **激活边**上传递“上调/下调”信号
- **抑制边**上传递“反转”信号（类似负号）
---
## 六、Step 4：解码——从嵌入到预测表达
### 6.1 解码器结构
经过 $L$ 层 GNN 后，每个基因的最终嵌入 $h_i^{(L)}$ 包含了其受扰动影响的“综合上下文信息”。解码器将其映射为预测的表达变化：
$$\Delta \hat{x}_i = \text{MLP}(h_i^{(L)})$$
或者更简单地用线性层：
$$\Delta \hat{x}_i = w^T h_i^{(L)} + b$$
### 6.2 最终预测
预测的扰动后表达值为：
$$\hat{x}_i^{pert} = \bar{x}_i^{control} + \Delta \hat{x}_i$$
### 6.3 预测目标
通常预测的是**差异表达值**（$\Delta$）而非绝对表达值，原因：
- Δ 值的分布更对称（均值≈0），训练更稳定
- 让模型专注于学习“扰动效应”，而不是重新学习 control 表达
---
## 七、Step 5：训练策略与泛化机制
### 7.1 训练数据
使用真实的 Perturb-seq 数据（CROP-seq、Perturb-seq 等技术产生的数据集），每个样本包含：
- 输入：扰动基因列表 $\mathbf{p}$ + control 表达谱 $\bar{\mathbf{x}}$
- 标签：扰动后的真实表达谱 $\mathbf{x}^{pert}$
### 7.2 损失函数
$$\mathcal{L} = \frac{1}{N} \sum_{i=1}^{N} \left( \Delta \hat{x}_i - \Delta x_i \right)^2$$
其中 $\Delta x_i = x_i^{pert} - \bar{x}_i^{control}$ 是真实的表达变化。
可加入正则化项防止过拟合：
$$\mathcal{L}_{total} = \mathcal{L}_{MSE} + \lambda_1 \|\mathbf{W}\|_2 + \lambda_2 \mathcal{L}_{graph\_regularization}$$
### 7.3 泛化到未见扰动的关键机制
GNN 能泛化到未见过的扰动（unseen perturbation），其根本原因在于**图的拓扑结构提供了共享的“知识骨架”**：
**场景 1：未见过的单基因扰动**
- 训练时见到了扰动基因 A 和基因 B
- 测试时要求预测扰动基因 C 的效果
- 由于 C 与 A/B 共享部分下游靶基因和通路，GNN 可以利用学到的“传播规则”（即 $W$、注意力机制）来推断 C 的扰动效应
**场景 2：组合扰动（GEARS 的重点优势）**
- 训练时见到了扰动（A,B）、（A,C）、（B,D）
- 测试时要求预测扰动
- 传统方法只能外推；GNN 则在图上**同时激活** A 和 B 的扰动标记，让两条扰动信号在共享的下游子网络上**叠加和交互**，从而预测非加性效应（协同/拮抗）
### 7.4 GEARS 特有的“双通道”设计
GEARS 在标准 GNN 之上增加了两个关键模块：
**（a）扰动基因集合编码器**：
```
被扰动基因集合 S = {A, B}
→ 每个扰动基因取其嵌入 e_A, e_B
→ 通过 Deep Sets / 注意力池化聚合为全局扰动向量 z_pert
→ z_pert 拼接到每个节点的特征中
```
**（b）共表达协方差图**：
- 除了先验调控图外，GEARS 还根据 control scRNA-seq 的共表达模式添加“共表达边”
- 这样即使某条调控关系未在数据库中记录，只要两基因在表达上相关，信息仍能传播
---
## 八、完整算法伪代码
```python
# ============ 训练阶段 ============
def train_gnn_perturbation(graph, perturb_seq_data, epochs):
    for epoch in range(epochs):
        for sample in perturb_seq_data:
            perturbed_genes = sample['perturbed_genes']  # e.g., ['TP53', 'BRCA1']
            control_expr = sample['control_expression']   # [N] 维向量
            true_pert_expr = sample['perturbed_expr']     # [N] 维向量
            
            # Step 1: 构建扰动标记
            pert_flag = one_hot(perturbed_genes, num_genes=N)
            
            # Step 2: 构建初始节点特征
            gene_identity_emb = Embedding(N, d)           # 可学习嵌入
            h0 = concat([control_expr, gene_identity_emb, pert_flag])
            
            # Step 3: 多层消息传递
            h = h0
            for layer in range(L):
                h = GNN_Layer(h, graph.adjacency, graph.edge_type)
                # 每层: 聚合邻居 + 自更新 + 非线性激活
            
            # Step 4: 解码预测
            delta_pred = Decoder(h)                       # [N] 维
            pred_expr = control_expr + delta_pred
            
            # Step 5: 计算损失并更新
            loss = MSE(pred_expr, true_pert_expr)
            optimizer.backward(loss)
    
    return trained_model
# ============ 推理阶段（虚拟敲降实验）============
def virtual_knockdown(model, graph, gene_to_knockdown, control_expr):
    # 用户指定要敲降的基因，无需任何实验
    pert_flag = one_hot([gene_to_knockdown], num_genes=N)
    h0 = concat([control_expr, Embedding(N, d), pert_flag])
    
    h = h0
    for layer in range(L):
        h = GNN_Layer(h, graph.adjacency, graph.edge_type)
    
    delta_pred = Decoder(h)
    virtual_pert_expr = control_expr + delta_pred
    
    # 返回所有基因的预测表达变化（整个下游级联效应）
    return virtual_pert_expr
```
---
## 九、与传统方法的关键区别
| 维度 | 传统方法（如 GENIE3） | GNN 方法（如 GEARS） |
|---|---|---|
| **网络作用** | 先建网络，然后**手工删边**模拟 KO | 网络结构**内嵌于模型参数中**，扰动信号自动传播 |
| **间接效应** | 需要多轮迭代或单独建模 | 通过多层 GNN **端到端**学习 |
| **组合扰动** | 难以处理（效应叠加假设过强） | 多个扰动信号在图上自然交互，可捕捉非加性 |
| **泛化能力** | 对未见基因需重新推断 | 通过共享图结构泛化到未见扰动 |
| **可训练性** | 无参数训练（纯算法推断） | 有大量可学习参数，用 Perturb-seq 监督训练 |
| **预测输出** | 网络拓扑变化（哪些边消失） | 精确到每个基因的**表达值变化** |
---
## 十、局限性与注意事项
1. **图质量依赖**：GNN 的预测上限受限于先验图的完整性和准确性。若关键调控边缺失，扰动信号无法传播到真实下游。
2. **分布外泛化**：对于与训练集差异很大的扰动（如靶向完全不同通路的基因），预测性能会下降。
3. **注意力可解释性有限**：虽然注意力权重 $\alpha_{ij}$ 被解释为“调控强度”，但它并非严格的因果效应，仅是模型学到的相关模式。
4. **多层堆叠的过平滑**：过深的 GNN 会导致所有节点嵌入趋同，通常 2–4 层为宜。
5. **细胞类型特异性**：同一个 GRN 在不同细胞类型中活性不同，训练数据需覆盖目标细胞类型的扰动，或使用条件 GNN（将 cell type 作为条件输入）。
---
## 总结
GNN 实现虚拟扰动的**核心逻辑**可以概括为一句话：
> **把“敲降一个基因”编码为图中对应节点的特征改变，让这个改变通过可学习的消息传递机制沿调控边向下游传播，最终在解码器输出处读出整个网络的响应。**
它将传统方法中“先建网络→再手工模拟→最后观察拓扑”的离散流程，转化为了“网络即模型、扰动即输入、响应即输出”的**端到端可微分**框架，使得模型可以从有限的 Perturb-seq 数据中学习到“扰动→响应”的一般规律，并泛化到大量未实验的扰动场景。
