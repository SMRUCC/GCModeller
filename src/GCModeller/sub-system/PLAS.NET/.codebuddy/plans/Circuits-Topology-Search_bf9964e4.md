---
name: Circuits-Topology-Search
overview: 在 PLAS.NET/Circuits 库（RootNamespace=SMRUCC.genomics.Analysis.Circuits）中实现 Ma et al. 2009 Cell 论文《Defining Network Topologies that Can Achieve Biochemical Adaptation》的"功能拓扑搜索"系统：三节点酶网络 3^9 拓扑穷举 + 连通性剪枝 → LHS 对数空间参数采样 → Michaelis-Menten ODE 两阶段阶跃仿真（CVODE BDF）→ 灵敏度/精度/鲁棒性 Q 值评估 → NFBLB/IFFLP 基序识别 + Hamming 层次聚类 + Venn 过度代表分析，全部产物落 CSV，并用论文给出的已知结论（19683→16038、2 节点零解、最小链接数=3、NFBLB 积分控制 C* 与输入无关、IFFLP 比例器 B*∝A*、395 个功能拓扑必含两类基序之一）作为自测锚点。
---

## 产品概述

在 PLAS.NET/Circuits 库（RootNamespace=SMRUCC.genomics.Analysis.Circuits）中实现 Ma et al. 2009 Cell 论文《Defining Network Topologies that Can Achieve Biochemical Adaptation》的"功能拓扑搜索"系统：对三节点酶网络穷举拓扑空间，经参数采样与 ODE 仿真评估适应性功能（灵敏度 S/精度 P/鲁棒性 Q），最终从统计上证明仅 NFBLB（带缓冲节点负反馈）与 IFFLP（带比例器节点非相干前馈）两类基序必要，并输出设计表。

## 核心功能

- **拓扑枚举**：9 条有向链接（顺序固定 [A→B, A→C, B→A, B→C, C→A, C→B, A→A, B→B, C→C]）各取 {+1,−1,0}，穷举 3^9=19,683 种；BFS 剪除 A→C 不可达的 3,645 种，得 16,038 可行拓扑；支持 2 节点对照与 ≤3 链接最小网络子集
- **参数采样**：每拓扑 LHS 对数空间采样（k∈[0.1,10]、K∈[0.001,100]），参数量 2·n_l+2（含输入链接对），背景酶 E_i=F_i=0.5 固定；按拓扑确定性种子保证可复现
- **ODE 仿真**：Michaelis-Menten 动力学（论文 Eq.7，节点总浓度=1、Hill=1），两阶段阶跃协议（I1=0.5 稳态 → I2=0.6 瞬态），CVODE BDF 刚性求解（rtol=1e-6、atol=1e-9），记录 O1/O2/O_peak；剔除病态电路（稳态值<0.001、非有限、不收敛、持续振荡）
- **功能评估**：S=|O_peak−O1|/O1÷|ΔI/I1|、P 为其倒数倒数，功能判据 S>1 且 P>10，Q 值=功能参数组计数，Q>10 为过度代表门槛；阈值均可配置
- **基序分析**：NFBLB（负反馈环经缓冲节点 B 且无直接 C→A 链接）/IFFLP（A 到 C 两路径符号相反且 B 直接调控 C）自动识别；Venn 交集统计复现 Figure 4B；Hamming 距离（9 维符号向量差位计数）+ average linkage 层次聚类，聚类质量用 Silhouette/Dunn 评估
- **产物输出**：拓扑表（id/符号向量/Q/基序标志）、电路明细表、聚类表、Venn 汇总、设计表（最小网络+附加基序→Q 提升）、运行报告；按拓扑 checkpoint 支持断点续跑
- **验证锚点**：19,683→16,038 计数精确一致；2 节点（81 拓扑）与 3 节点 2 链接零适应、最小可行链接数=3；NFBLB 积分控制 C*=F_B·k'_FB/k_CB 与输入无关；IFFLP 比例器 B*∝A*；同种子结果完全可复现

## 技术栈选择

- **语言/框架**：VB.NET net10.0（沿用 PLAS.NET 解决方案惯例，未开 Option Strict，与既有工程一致；跨程序集调用 Option Strict On 的依赖不受影响）
- **ODE 求解**：Sundials.CVODE（BDF 刚性多步法，`Microsoft.VisualBasic.Math.Sundials.CVODE`）——米氏方程在 K 低至 0.001 时高度刚性，必须刚性求解器（README 陷阱提醒第 4 条明确）
- **参数采样**：Math 库 LHS（`Microsoft.VisualBasic.Math.Distributions.LHS` 的 LatinHypercubeSampler + LhsScale.ScaleLog10）——用户指定依赖
- **拓扑距离/聚类**：DataMining 的 `HammingDistance.Between(Integer(), Integer())`（9 维符号向量距离）；层次聚因为 HammingClusteringEngine 是监督规则提取而非凝聚聚类，按 README 要求自写 average linkage（n≤几百时 O(n³) 可接受）
- **聚类评估**：DataMining Evaluation 的 ClusteringIndices（Silhouette/Dunn/DaviesBouldin/CalinskiHarabasz/ARI）——用户提示可能有用的评估库
- **CSV IO**：dataframework 的 `csv.File`/`csv.RowObject`/`table.Save(path, Encodings.UTF8)`——与同解决方案 SGrid/IO/NetworkWriter.vb 完全一致的既有模式
- **图库**：graph-netcore5 已引用但本模块可达性判定直接在 9 维符号向量上自写 BFS（约 15 行），避免依赖摩擦；图库保留供后续网络导出/可视化扩展

## 实现方法

**总体策略**：按 README 的"枚举—采样—仿真—评估—统计"四层流水线组织为六个松耦合模块 + 编排门面，与论文 Figure 1D 流程一一对应。每个模块单一职责、可独立单测。

**关键决策与权衡**：

1. **BDF 而非 Adams**：MM 项 K∈[0.001,100] 跨越 5 个数量级，刚性普遍存在；Adams 仅适合非刚性
2. **稳态早退**：分块积分（如每 10 时间单位）后检测相对变化