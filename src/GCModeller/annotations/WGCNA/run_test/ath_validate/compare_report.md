# GCModeller vs GNU R WGCNA 对照报告

目录: `G:/GCModeller/src/GCModeller/annotations/WGCNA/run_test/ath_validate`
生成时间: 2026-10-02 05:54:43

### 树剪切实现对照（同一棵 hclust 树）

| 指标 | GCModeller | GNU R cutreeHybrid |
|---|---|---|
| 模块数 | 19 | 19 |
| 已标注基因 | 3000 | 3000 |

adjusted Rand index = 1.000000

> 该指标接近 1 即说明 DynamicTreeCut 与 R 的 cutreeHybrid 行为一致；
> 若该指标高而「单块对照」的 ARI 低，则差异来自层次聚类的并列（tie）处理。

### cor 矩阵

| 指标 | 值 |
|---|---|
| 共同基因数 | 3000 |
| 最大绝对误差 | 2.887e-15 |
| 平均绝对误差 | 2.828e-16 |
| RMSE | 3.521e-16 |
| 元素级 Pearson | 1.000000000000 |

### adj 矩阵

| 指标 | 值 |
|---|---|
| 共同基因数 | 3000 |
| 最大绝对误差 | 1.000e+00 |
| 平均绝对误差 | 3.333e-04 |
| RMSE | 1.826e-02 |
| 元素级 Pearson | 0.997262369282 |

### tom 矩阵

| 指标 | 值 |
|---|---|
| 共同基因数 | 3000 |
| 最大绝对误差 | 5.218e-15 |
| 平均绝对误差 | 3.645e-16 |
| RMSE | 5.605e-16 |
| 元素级 Pearson | 1.000000000000 |

### 模块划分一致性（单块对照）

| 指标 | GCModeller | GNU R |
|---|---|---|
| 基因数 | 3000 | 3000 |
| 模块数 | 19 | 19 |
| 已标注基因 | 3000 | 3000 |

adjusted Rand index = 1.000000

### 模块划分一致性（整表分块 blockwise 对照）

| 指标 | GCModeller | GNU R |
|---|---|---|
| 基因数 | 3000 | 34262 |
| 模块数 | 19 | 59 |
| 已标注基因 | 3000 | 34262 |

adjusted Rand index = 0.428623

### GCModeller 分段耗时（毫秒）

| 阶段 | 毫秒 |
|---|---|
| load | 201 |
| cor | 115 |
| beta | 0 |
| adjacency | 46 |
| tom | 4614 |
| hclust | 883 |
| cut | 100 |
| wallclock | 10977 |

### GNU R 分段耗时（秒）

| 阶段 | 秒 |
|---|---|
| load | 0.113147974014282 |
| stageA | 26.6027591228485 |

