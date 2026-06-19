' ============================================================================
' DIABLO_Analysis_Documentation.vb
' ============================================================================
' VB.NET DIABLO 多组学数据关联分析 - 算法数学原理文档
' 本文件以注释形式详细记录每个算法步骤的数学推导
' ============================================================================

' ============================================================================
' 第一章: DIABLO 算法数学原理
' ============================================================================
'
' 1.1 问题定义
' -------------
' 给定 K 个组学数据块 X_1, X_2, ..., X_K (每个为 n×p_k 矩阵，n为样本数，
' p_k为第k块的变量数) 和一个分类响应变量 Y (n×1)，DIABLO的目标是找到
' K 组权重向量 (载荷) u_1, u_2, ..., u_K，使得:
'
'   (a) 各块的潜在变量 t_k = X_k * u_k 之间高度相关
'   (b) 潜在变量能够区分Y的类别
'   (c) 通过稀疏化实现变量选择
'
'
' 1.2 目标函数
' -------------
' 对于第 h 个成分 (h = 1, 2, ..., H)，DIABLO优化如下目标:
'
'   max  Σ_{k≠l} design[k,l] · cov(t_kh, t_lh) + Σ_k design[k,Y] · cov(t_kh, Y·c_h)
'   s.t. ||u_kh||_2 = 1  (单位范数约束)
'        ||u_kh||_1 ≤ λ_k (L1稀疏约束)
'
' 其中:
'   - t_kh = X_k · u_kh 是第k块第h成分的潜在变量
'   - design[k,l] 是设计矩阵中块k与块l的连接权重
'   - c_h 是Y的权重向量
'   - λ_k 控制稀疏度 (等价于keepX参数)
'
' 设计矩阵 design 是 (K+1)×(K+1) 的对称矩阵:
'   - design[k,l] = 1: 块k和块l完全连接
'   - design[k,l] = 0: 块k和块l无连接
'   - design[k,Y]: 块k与Y的连接
'   - 对角线为0
'
'
' 1.3 迭代优化算法
' -----------------
' DIABLO使用交替优化策略求解，对每个成分h:
'
' 步骤0: 初始化
'   对每个块k，使用SVD初始化权重向量 u_kh
'   u_kh^(0) = X_k 的第一个右奇异向量
'
' 步骤1: 更新权重向量 (对每个块k)
'   计算 M_k = X_k^T · [Σ_{l≠k} design[k,l] · (X_l · u_lh) + design[k,Y] · (Y · c_h)]
'
'   这一步的含义:
'   - X_l · u_lh = t_lh 是第l块的潜在变量
'   - design[k,l] · t_lh 是加权后的第l块贡献
'   - X_k^T · (加权贡献之和) 给出了使块k与其它块协方差最大化的方向
'
' 步骤2: 提取主方向
'   u_kh = M_k / ||M_k||_2
'   (取M_k的方向作为新的权重)
'
' 步骤3: 稀疏化 (L1惩罚)
'   u_kh = Sparsify(u_kh, keepX[k])
'   保留 |u_kh| 中最大的 keepX[k] 个分量，其余置零
'   然后重新归一化: u_kh = u_kh / ||u_kh||_2
'
' 步骤4: 更新Y权重
'   M_Y = Y^T · [Σ_l design[Y,l] · (X_l · u_lh)]
'   c_h = M_Y / ||M_Y||_2
'
' 步骤5: 收敛判断
'   如果 max_k ||u_kh^(new) - u_kh^(old)||_2 < tol，则收敛
'   否则返回步骤1
'
'
' 1.4 矩阵缩减 (Deflation)
' ---------------------------
' 提取第h个成分后，需要从数据矩阵中移除该成分的贡献:
'
'   X_k ← X_k - t_kh · p_kh^T
'
' 其中:
'   t_kh = X_k · u_kh (潜在变量)
'   p_kh = X_k^T · t_kh / (t_kh^T · t_kh) (回归系数)
'
' 这确保后续成分是在残余矩阵上提取的，与之前成分正交
'
'
' 1.5 预测方法
' -------------
' 对新样本 x_new = (x_1_new, x_2_new, ..., x_K_new):
'
' 步骤1: 中心化 (减去训练集均值)
' 步骤2: 投影到成分空间
'   score_kh = x_k_new^T · u_kh
' 步骤3: 合并得分 (跨块平均)
'   score_h = (1/K) Σ_k score_kh
' 步骤4: 分类
'   - 质心法: 分配到最近的类质心
'     class = argmin_c ||score - μ_c||
'   - 最大距离法: 对每个成分取最大距离
'   - 马氏距离: 考虑类内协方差
'
'
' ============================================================================
' 第二章: 稀疏化 (Sparsification) 数学原理
' ============================================================================
'
' 2.1 L1惩罚与软阈值
' --------------------
' DIABLO中的变量选择基于L1惩罚 (Lasso):
'
'   min ||u - v||_2^2 + λ||u||_1
'
' 其解为软阈值算子:
'   S_λ(x) = sign(x) · max(|x| - λ, 0)
'
' 在mixOmics/DIABLO中，稀疏度通过keepX参数控制:
'   - keepX = 保留的变量数
'   - 等价于选择 |v| 中最大的keepX个分量
'
' 这比连续的L1惩罚更直接，允许精确控制选中的变量数
'
'
' 2.2 SparsifyVector 算法
' -------------------------
' 输入: 向量 v, 保留数 keepX
' 输出: 稀疏向量 v'
'
' 1. 计算 |v_i|, i = 1, ..., p
' 2. 按 |v_i| 降序排列
' 3. 保留前 keepX 个最大分量
' 4. 其余分量置零
' 5. 归一化: v' = v' / ||v'||_2
'
'
' ============================================================================
' 第三章: 交叉验证数学原理
' ============================================================================
'
' 3.1 K折交叉验证
' -----------------
' 用于选择最优 ncomp 和 keepX:
'
' 1. 将数据随机分为 K 折
' 2. 对每折 f = 1, ..., K:
'    a. 训练集 = 除第f折外的所有数据
'    b. 测试集 = 第f折
'    c. 在训练集上拟合DIABLO模型
'    d. 在测试集上预测并计算BER
' 3. 平均K折的BER得到最终评估
'
'
' 3.2 平衡错误率 (BER)
' ----------------------
' BER = (1/C) Σ_{c=1}^{C} (错误分类的c类样本数 / c类总样本数)
'
' BER对类别不平衡具有鲁棒性，是mixOmics中的主要评估指标
'
'
' 3.3 参数调优策略
' ------------------
' 两步调优:
'   第一步: 固定keepX (保留全部变量)，调优ncomp
'   第二步: 固定ncomp，调优keepX (网格搜索)
'
' keepX网格通常取:
'   {p/50, p/20, p/10, p/5, p/2}
' 其中p是变量数
'
'
' ============================================================================
' 第四章: 辅助分析方法数学原理
' ============================================================================
'
' 4.1 RGCCA协方差分析
' ---------------------
' 正则化广义典型相关分析 (RGCCA) 是DIABLO的理论基础:
'
' C[i,j] = ||(1/(n-1)) X_i^T X_j||_F · design[i,j]
'
' 正则化:
'   C[i,i] = (1-τ_i) · C[i,i] + τ_i
'
' τ_i ∈ [0,1]:
'   τ_i = 0: 最大化协方差 (类似PCA)
'   τ_i = 1: 最大化相关 (类似CCA)
'
'
' 4.2 Procrustes分析
' --------------------
' 评估两个数据块在成分空间中的配置一致性:
'
' 给定两个得分矩阵 X1 (n×p) 和 X2 (n×p):
' 1. 中心化: X1_c = X1 - mean(X1), X2_c = X2 - mean(X2)
' 2. SVD分解: M = X2_c^T · X1_c = U · S · V^T
' 3. 最优旋转: R = V · U^T
' 4. 旋转X2: X2_rot = X2_c · R
' 5. Procrustes统计量: ss = ||X1_c - X2_rot||_F^2
' 6. Procrustes相关: m^2 = 1 - ss / (||X1_c||_F^2 + ||X2_c||_F^2)
'
'
' 4.3 网络拓扑指标
' ------------------
' 从变量选择结果构建网络:
'   - 节点: 选中的变量
'   - 边: 变量间的相关性 (阈值化)
'
' 计算指标:
'   - 度 (degree): 与节点相连的边数
'   - 加权度: 边权重之和
'   - Hub变量: 加权度最高的变量
'   - 聚类系数: 节点邻居之间的连接比例
'     CC_i = 2·T_i / (k_i · (k_i - 1))
'     T_i = 节点i的邻居之间的边数
'   - 网络密度: 实际边数 / 最大可能边数
'
'
' ============================================================================
' 第五章: 模块架构说明
' ============================================================================
'
' 本实现包含以下核心模块:
'
' 1. Matrix 类 - 基础矩阵运算
'    - 算术运算: +, -, *, 转置, 逐元素乘法
'    - 范数计算: L1, L2, Frobenius
'    - 统计运算: 列均值, 列标准差, 中心化, 标准化
'    - 矩阵操作: 子矩阵提取, 拼接, 深拷贝
'
' 2. LinearAlgebra 类 - 高级线性代数
'    - PowerIteration: 幂迭代法求最大特征值/向量
'    - SVD: 奇异值分解
'    - PseudoInverse: 伪逆矩阵
'    - CorrelationMatrix: 相关系数矩阵
'    - PearsonCorrelation: 皮尔逊相关系数
'
' 3. SparseUtils 类 - 稀疏化工具
'    - SoftThreshold: 软阈值函数
'    - SparsifyVector: L1稀疏化 (keepX方式)
'    - SparsifyByL1Norm: 基于L1范数约束的稀疏化
'
' 4. DIABLO 类 - 核心算法
'    - Fit(): 拟合DIABLO模型
'    - Predict(): 预测新样本类别
'    - ComputeBlockCorrelation(): 块间相关性
'    - ComputeVariableImportance(): 变量重要性
'    - GetSelectedVariables(): 获取选中变量
'
' 5. DIABLOCrossValidation 类 - 交叉验证
'    - TuneNComp(): 调优成分数
'    - TuneKeepX(): 调优稀疏度参数
'    - ComputeBER(): 计算平衡错误率
'
' 6. DIABLOUtils 类 - 辅助工具
'    - CreateFullDesign(): 创建全连接设计矩阵
'    - CreateCustomDesign(): 创建自定义设计矩阵
'    - SimulateMultiOmicsData(): 模拟多组学数据
'    - ComputeAUC(): 计算AUC
'
' 7. MultiBlockIntegration 类 - 扩展分析
'    - ComputeRGCCACovariance(): RGCCA协方差分析
'    - ComputeConsensusMatrix(): 共识矩阵
'    - ComputeNetworkMetrics(): 网络拓扑指标
'    - ProcrustesAnalysis(): Procrustes分析
'
' 8. DIABLOPipeline 类 - 一键分析流程
'    - Run(): 完整分析流程 (调优+拟合+评估)
'
'
' ============================================================================
' 第六章: 与R mixOmics包的对应关系
' ============================================================================
'
' R mixOmics 函数          →  VB.NET 对应方法
' ---------------------------------------------------------------
' block.splsda()           →  New DIABLO(...).Fit()
' predict.block.splsda()   →  model.Predict()
' plotDiablo()             →  (不实现，仅数学计算)
' circosPlot()             →  (不实现，仅数学计算)
' plotLoadings()           →  model.Loadings + model.ComputeVariableImportance()
' plotVar()                →  model.GetSelectedVariables()
' perf()                   →  DIABLOCrossValidation
' tune.block.splsda()      →  DIABLOCrossValidation.TuneNComp() / TuneKeepX()
' network()                →  MultiBlockIntegration.ComputeNetworkMetrics()
' design 矩阵              →  DIABLOUtils.CreateFullDesign() / CreateCustomDesign()
'
' 关键参数对应:
' R参数                    →  VB.NET 参数
' ---------------------------------------------------------------
' X (list of matrices)     →  X As List(Of Matrix)
' Y (factor)               →  YLabels As Integer() + classLabels As String()
' ncomp                    →  ncomp As Integer
' design                   →  design As Matrix
' keepX (list of vectors)  →  keepX As List(Of Integer())
' max.iter                 →  maxIter As Integer
' tol                      →  tol As Double
' dist (预测距离)           →  distMethod As String
'
' ============================================================================
