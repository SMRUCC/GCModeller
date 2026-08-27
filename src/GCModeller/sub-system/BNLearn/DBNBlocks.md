按WGCNA颜色模块拆分后为每个模块独立训练子DBN，在"降低计算复杂度"这一目标上是可行且被认可的策略——模块化思路已被证明能把大尺度基因调控网络逆向工程问题拆分为可计算的功能单元。
---
## 一、可行性分析：按模块拆分训练DBN的收益与陷阱
WGCNA先聚类再用DBN建子网，本质是"先降维、再结构学习"的两步法，能显著降低DBN结构搜索的组合复杂度（DBN结构学习是NP难问题，节点数线性增长会导致搜索空间指数爆炸）。该策略在模块化基因网络逆向工程中已被验证可扩展到十基因以上的网络，并且DBN本身已被成功用于大规模基因调控网络的构建。
但需要注意，这种"硬切分"会引入若干系统偏差，整理如下：
| 维度 | 优势 | 潜在问题 |
|---|---|---|
| 计算效率 | 单模块节点数从8万降至几十到几百，结构学习复杂度骤降 | 需对每个模块独立运行，模块数过多时仍需并行调度 |
| 网络稀疏性 | 模块内基因共表达高，信号噪声比更好 | 模块内可能仍存在数千基因，需进一步用hub基因或kME筛选 |
| 生物学解释 | 模块通常对应功能通路，便于解释 | 跨模块调控关系（如TF调控另一模块靶基因）被完全切断 |
| 时间动态建模 | DBN能捕捉时序因果方向 | WGCNA基于静态相关，未利用时序信息；模块划分对时间窗口敏感 |
| 扰动模拟 | 子网内扰动结果可直接计算 | 扰动跨模块传播路径缺失，全局响应预测失真 |
**主要陷阱有三个**：一是模块间边丢失——WGCNA基于相关聚类，跨模块的相关性可能因软阈值或动态剪枝而被弱化，但这些边恰恰可能是真正的调控关系；二是hub基因选择偏差，仅用kME最高的基因代表模块会丢失模块内异质性；三是DBN的同质性假设（参数随时间不变），不适用于存在调控状态切换的非平稳时序。
---
## 二、扰动实验的正确整合流程
进行虚拟扰动实验需要把所有子网络整合成一个**全局邻接矩阵**（或雅可比矩阵），再在该矩阵上模拟扰动传播，而不是把子模块结果简单堆叠。
完整流程分四步：
### 步骤1：构建全局网络骨架
把每个子DBN学到的边集 $E_m$（模块 $m$ 内的有向加权边）合并，得到一个块对角的"分块邻接矩阵" $A_{\text{block}}$，其中块内非零、块间全零。同时，提取每个模块的**hub基因或代表TF**（按kME或module membership排名前N的基因，或模块内已知的转录因子）作为模块的"沟通接口"。
### 步骤2：补充模块间边
模块间边的补充有两种主流做法：
- **基于eigengene相关**：用各模块的eigengene（第一主成分）计算模块间的相关矩阵，作为模块级别的"相互作用骨架"。然后对每个模块对，用hub基因间的表达相关或互信息估计具体的跨模块边权重。
- **基于hub基因直接建边**：在所有hub基因集合上运行一次轻量级的DBN或GENIE3，专门学习hub之间的有向边，填入 $A_{\text{block}}$ 的跨模块块中。
最终得到全局邻接矩阵 $A$，既包含模块内的精细调控，也包含模块间的粗粒度连接。
### 步骤3：执行扰动传播
扰动不是在"单个模块"上做，而是在全局 $A$ 上做。常用做法是把 $A$ 视作线性化系统的雅可比矩阵 $J$，扰动向量为 $\Delta x_0$（在被扰动基因上取非零值，其余为零），下游响应按以下方式传播：
$$\Delta x_{t+1} = J \cdot \Delta x_t$$
迭代至收敛（或指定步数），得到稳态响应 $\Delta x^* = (I - J)^{-1} \Delta x_0$（在谱半径小于1的条件下）。这等价于让扰动沿着所有直接和间接调控路径在全局网络上传播。
如果保留的是DBN而非线性雅可比，可在整合后的全局2-TBN上用证据设置（把被扰动基因在 $t$ 时刻的取值设为敲低/过表达值）做概率推断，得到所有基因在后续时刻的后验分布变化。
### 步骤4：结果汇总与解释
最终输出是一个**全局扰动响应矩阵** `response[gene, perturbation]`，而非各子模块数据框的堆叠。可以直接在该矩阵上：
- 按受影响程度排序，识别扰动的直接靶点和间接响应基因；
- 比较扰动是否跨越多个模块（响应基因的模块分布）；
- 用通路富集验证响应模块的生物学合理性。
---
## 三、常见误区清单
- **误区1：直接rbind等于忽略模块间调控**。子模块扰动结果各自只覆盖本模块，跨模块的下游响应会完全缺失，导致扰动效应被严重低估。
- **误区2：模块划分过细或过粗**。模块内基因数过多（如>500）则DBN结构学习仍不可行；过细（如<10）则会过度碎片化，模块间边占比过高，反而失去降维意义。建议结合模块功能注释（GO/KEGG富集）和模块大小综合调参。
- **误区3：忽略hub基因作为模块接口**。hub基因（高kME或高模块内连接度）往往是模块的"输入/输出端口"，只训练模块内普通基因而不用hub作为锚点，会让跨模块传播路径完全断裂。
- **误区4：把WGCNA的静态相关当作时序因果**。WGCNA用的是全时段的相关，而DBN需要的是时序上有方向的调控。对时序数据，建议在划模块前先用时序信息（如limma-差异时序分析、或先把表达量按时间离散化）做一次预筛选，避免模块划分被静态相关主导。
- **误区5：扰动幅度设置过大导致线性假设失效**。雅可比传播只在小幅扰动下近似成立，敲除类扰动需借助非线性模型（如CellOracle用梯度场、dynamo用向量场雅可比）。
---
## 四、可复用的代码骨架
以下给出R风格的伪代码，覆盖从模块划分到全局扰动传播的关键环节：
```r
library(WGCNA)
library(bnlearn)  # 或 dbnR / BiDAG 用于DBN
# ===== 1. WGCNA模块划分 =====
# datExpr: 1800 samples x 80000 genes, 已过滤低表达
net <- blockwiseModules(datExpr, power = 6, TOMType = "unsigned",
                        minModuleSize = 30, deepSplit = 2,
                        numericLabels = TRUE)
moduleLabels <- net$colors
MEs <- net$MEs  # 模块eigengene
# ===== 2. 选取hub基因作为模块接口 =====
hubList <- lapply(unique(moduleLabels), function(m) {
  genes_in_m <- colnames(datExpr)[moduleLabels == m]
  kME <- cor(datExpr[, genes_in_m], MEs[, paste0("ME", m)])
  names(sort(kME, decreasing = TRUE))[1:20]  # 每模块取top20
})
names(hubList) <- unique(moduleLabels)
# ===== 3. 每模块训练子DBN =====
moduleEdges <- list()
for (m in names(hubList)) {
  subExpr <- datExpr[, hubList[[m]]]  # 用hub基因降低DBN规模
  # 用时序离散化后的数据学习DBN结构 (示例用bnlearn的hc + 方向约束)
  subNet <- hc(discretize(subExpr), score = "bic")
  moduleEdges[[m]] <- arcs(subNet)
}
# ===== 4. 构建全局邻接矩阵 + 补充跨模块边 =====
allGenes <- unique(unlist(hubList))
A <- matrix(0, nrow = length(allGenes), ncol = length(allGenes),
            dimnames = list(allGenes, allGenes))
for (m in names(moduleEdges)) {
  for (e in seq_len(nrow(moduleEdges[[m]]))) {
    from <- moduleEdges[[m]][e, "from"]; to <- moduleEdges[[m]][e, "to"]
    A[from, to] <- 1  # 可用边权重替代
  }
}
# 跨模块边：用eigengene相关 + hub间相关
eigCor <- cor(MEs)
for (i in 1:(length(hubList)-1)) {
  for (j in (i+1):length(hubList)) {
    if (abs(eigCor[i, j]) > 0.5) {  # 阈值可调
      # 在两模块hub间学习轻量DBN或用相关方向
      crossEdges <- inferCrossEdges(hubList[[i]], hubList[[j]], datExpr)
      A[crossEdges$from, crossEdges$to] <- crossEdges$weight
    }
  }
}
# ===== 5. 全局扰动传播 =====
perturbGene <- "GeneX"
delta0 <- setNames(rep(0, length(allGenes)), allGenes)
delta0[perturbGene] <- -1  # 敲低为-1，过表达为+1
# 线性传播（J = A），迭代到稳态
propagate <- function(J, delta0, maxIter = 100, tol = 1e-6) {
  delta <- delta0
  for (i in 1:maxIter) {
    deltaNew <- J %*% delta
    if (sqrt(sum((deltaNew - delta)^2)) < tol) break
    delta <- deltaNew
  }
  return(delta)
}
response <- propagate(A, delta0)
# ===== 6. 多个扰动源批量处理 =====
perturbGenes <- c("GeneA", "GeneB", "GeneC")
responseMat <- sapply(perturbGenes, function(g) {
  d0 <- setNames(rep(0, length(allGenes)), allGenes)
  d0[g] <- -1
  propagate(A, d0)
})
colnames(responseMat) <- perturbGenes
# responseMat: genes x perturbations 的全局响应矩阵
```
---
## 关键原则
**扰动实验的核心是在"整合后的全局网络"上模拟传播，而不是把子模块的结果拼接**。子模块训练阶段可以做"分而治之"，但扰动阶段必须做"合而为一"——通过hub基因接口和eigengene相关补全的模块间边，让扰动信号能在跨模块路径上正确传播，这样得到的响应矩阵才能反映真实的生物学调控逻辑。
