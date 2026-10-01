# GCModeller vs GNU R WGCNA 对照报告

目录: `G:/GCModeller/src/GCModeller/annotations/WGCNA/run_test/ath_blockwise`
生成时间: 2026-10-02 06:11:00

### cor 矩阵

_跳过：缺少 `cor.csv` 或 `R_cor.csv`_

### adj 矩阵

_跳过：缺少 `adj.csv` 或 `R_adj.csv`_

### tom 矩阵

_跳过：缺少 `tom.csv` 或 `R_tom.csv`_

### 模块划分一致性（整表分块 blockwise 对照）

| 指标 | GCModeller | GNU R |
|---|---|---|
| 基因数 | 26382 | 34262 |
| 模块数 | 81 | 59 |
| 已标注基因 | 26382 | 34262 |

adjusted Rand index = 0.188951

### GCModeller 分段耗时（毫秒）

| 阶段 | 毫秒 |
|---|---|
| load | 205 |
| backend | 0 |
| filter | 1 |
| precluster | 51 |
| beta | 3 |
| blocks | 187177 |
| collect | 2 |
| merge | 80 |
| eigengene | 16 |
| total | 187338 |
| block1.cor | 262 |
| block1.adjacency | 114 |
| block1.tom | 21008 |
| block1.dissim | 11 |
| block1.hclust | 1589 |
| block1.cut | 97 |
| block2.cor | 187 |
| block2.adjacency | 60 |
| block2.tom | 2958 |
| block2.dissim | 19 |
| block2.hclust | 108 |
| block2.cut | 10 |
| block3.cor | 73 |
| block3.adjacency | 131 |
| block3.tom | 1685 |
| block3.dissim | 0 |
| block3.hclust | 97 |
| block3.cut | 21 |
| block4.cor | 107 |
| block4.adjacency | 70 |
| block4.tom | 10544 |
| block4.dissim | 5 |
| block4.hclust | 11911 |
| block4.cut | 50 |
| block5.cor | 209 |
| block5.adjacency | 93 |
| block5.tom | 19214 |
| block5.dissim | 9 |
| block5.hclust | 1148 |
| block5.cut | 35 |
| block6.cor | 72 |
| block6.adjacency | 21 |
| block6.tom | 1833 |
| block6.dissim | 1 |
| block6.hclust | 77 |
| block6.cut | 3 |
| block7.cor | 139 |
| block7.adjacency | 65 |
| block7.tom | 9425 |
| block7.dissim | 5 |
| block7.hclust | 523 |
| block7.cut | 11 |
| block8.cor | 217 |
| block8.adjacency | 115 |
| block8.tom | 25308 |
| block8.dissim | 11 |
| block8.hclust | 50691 |
| block8.cut | 15 |
| block9.cor | 106 |
| block9.adjacency | 52 |
| block9.tom | 5975 |
| block9.dissim | 4 |
| block9.hclust | 1126 |
| block9.cut | 44 |
| block10.cor | 215 |
| block10.adjacency | 90 |
| block10.tom | 18029 |
| block10.dissim | 9 |
| block10.hclust | 1095 |
| block10.cut | 37 |
| wallclock | 187547 |

### GNU R 分段耗时（秒）

| 阶段 | 秒 |
|---|---|
| load | 0.113147974014282 |
| stageA | 26.6027591228485 |

