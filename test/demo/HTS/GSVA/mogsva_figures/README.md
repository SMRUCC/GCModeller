# moGSVA 论文插图脚本（TCGA-BRCA）

针对 `brca_tcga_pub2015_mogsva` 分析结果与 `K:\brca_tcga_pub2015\multipleOmics`
原始多组学数据，生成 7 张可直接投稿使用的多面板图（PDF 矢量 + 600 dpi PNG），
并附带一套可复算的结果自洽性审计。

## 一、目录结构

```
mogsva_figures/
├── 00_common.R                       # 路径、配色、主题、IO、公共函数
├── fig1_input_landscape.R            # 图 1 数据与通路覆盖度
├── fig2_activity_landscape.R         # 图 2 融合后的通路活性图谱
├── fig3_cross_omics_consistency.R    # 图 3 跨组学一致性
├── fig4_omics_contributions.R        # 图 4 组学贡献分解
├── fig5_ablation_pooled_baseline.R   # 图 5 与池化基线（消融）
├── fig6_significance_and_power.R     # 图 6 置换检验与检验效能
├── fig7_clinical_association.R       # 图 7 临床关联
├── run_all.R                         # 一键重建全部图
├── evaluate_results.R                # 结果自洽性审计（写 output/evaluation_report.md）
├── stats_for_legends.R               # 打印图注中引用的全部统计量
├── figure_legends.md                 # 英文图注（可直接粘进论文）
└── output/                           # 成品：FigureN_*.pdf/.png、评估报告、session_info.txt
```

## 二、运行方式

```bash
# Windows 下用系统 R（注意：本机 PATH 上的 Rscript 是 R# 运行时，不是 CRAN R）
set TMPDIR=C:\Users\Administrator\AppData\Local\Temp
"C:\Program Files\R\R-4.5.0\bin\Rscript.exe" --vanilla run_all.R          # 出图
"C:\Program Files\R\R-4.5.0\bin\Rscript.exe" --vanilla evaluate_results.R # 出审计报告
```

7 张图全部重建约 20 秒。结果表更新后**无需手动清缓存**：`00_common.R` 里的
`cache_rds()` 会比较 `.cache/mogsva_bundle.rds` 与结果目录/原始数据目录的
文件时间戳，过期自动重读；也可用 `MOGSVA_REFRESH=1` 强制刷新。

路径通过环境变量覆盖，无需改代码：

| 变量 | 默认值 |
|---|---|
| `MOGSVA_RESULT_DIR` | `G:/GCModeller/test/demo/HTS/GSVA/brca_tcga_pub2015_mogsva` |
| `MOGSVA_RAW_DIR` | `K:/brca_tcga_pub2015/multipleOmics` |
| `MOGSVA_SCRIPT_DIR` | `G:/GCModeller/test/demo/HTS/GSVA/mogsva_figures` |

依赖：ggplot2、data.table、patchwork、ComplexHeatmap、circlize、matrixStats、
ggrepel、survival、ragg、scales（本机 R 4.5.0 均已安装）。

## 三、版式与美术规范

* 宽度统一 183 mm（双栏通栏），高度 128–205 mm；PDF 用 `cairo_pdf`，
  字体 Arial 子集嵌入（`ArialMT` / `Arial-BoldMT`），可直接被期刊系统接受。
* PNG 600 dpi，尺寸精确等于 mm 规格（4322 px 宽）。
* 基线字号 7 pt，面板字母 10 pt 粗体，图内不出现中文。
* 组学配色全局固定：CNA `#4DBBD5`、Methylation `#E64B35`、
  Transcriptomics `#3C5488`、Proteomics `#00A087`；临床面板红=ER 阳性、蓝=ER 阴性。
* 热图 z-score 与差异色标统一为 `#2F5D8C`–白–`#A6362A`。

## 四、每张图讲什么（按最新一轮结果）

1. **图 1** 输入规模与通路覆盖：蛋白层仅 181 个特征、覆盖 158/335 条通路；
   156 条通路四层齐全、175 条三层、3 条两层、1 条单层。特征数平衡后
   **蛋白层拿到最大权重**（均值 0.363，其余三层 0.274–0.283）。
2. **图 2** 融合活性图谱：最可变通路呈双峰分布；一致性奖励把通路 SD 中位压低
   9.8%；40 条最可变通路热图显示免疫/炎症模块与代谢模块是两个独立变异轴。
3. **图 3** 一致性结构：50.0% 观测为 a = −1/3、23.3% 为 0、26.3% 完全同向；
   a 的取值被 K(K−1)/2+1 量化，**跨不同覆盖度不能直接比 a**；层间相关很弱
   （甲基化–转录组 0.32，蛋白组≈0），a 与通路 SD 的 Spearman ρ = 0.48。
4. **图 4** 贡献分解：蛋白组主导 33.6% 的观测（尽管只覆盖 158 条通路），
   转录组 29.2%、甲基化 20.8%、CNA 16.4%；前 20 条高变通路恰好都没有蛋白覆盖。
5. **图 5** 消融：与"拼接矩阵直接做单组学 GSVA"整体 r 仅 0.48（逐通路中位 0.47）；
   池化分数与甲基化层**负相关**（−0.20）、蛋白层仅 0.16，而融合分数对四层
   保持 0.46–0.82 —— 方法必要性最有力的一张图。
6. **图 6** 显著性：200 条通路、1000 次置换，最小 P = 0.0010（3 条触底）、
   最小 q = 0.057，**无通路过 FDR<0.05，但 11 条 q<0.10**；置换检验整体偏保守；
   统计量与通路 SD 相关（r = 0.42），SD 又随基因集规模下降（ρ = −0.54）。
7. **图 7** 临床：204/335 通路在 ER 阳阴之间 FDR<0.05（143 条在 ER 阴性更高）；
   四亚型活性谱清晰；Hedgehog 高低组 OS：HR 0.50（0.30–0.85），P = 0.0084。

## 五、本轮结果的评估要点

完整审计见 `output/evaluation_report.md`（由 `evaluate_results.R` 生成，可作补充材料）。
全部 7 大类检查通过：

| 检查 | 结果 |
|---|---|
| 导出完整性（无 NA、轴对齐、样本顺序与 samples.tsv 一致、335 唯一通路） | PASS |
| 权重规则 w = 1/log2(2+&#124;S_k&#124;) 逐通路归一 | PASS，最大偏差 1.1e-16 |
| 融合恒等式 ES_multi = ES_raw·(1+λa)/(1+λ) | PASS，最大偏差 1.1e-16 |
| Σ_k c_k = ES_raw、Σ&#124;share&#124; = 1 | PASS，最大偏差 5.6e-17 / 3.3e-16 |
| 由 layer_scores 重算 a 与 consistency.csv 一致 | PASS，153,640 个观测完全一致 |
| p 值落在 1/(B+1) 网格、q 值等于其 BH 校正 | PASS，B = 1000 |
| 通路尺寸过滤（5–500 成员） | PASS，实测 5–493 |

评估中发现、需要写进论文的三件事：

1. **蛋白层权重被"平衡"放大**。Balanced 策略按 1/log2(2+|S_k|) 补偿成员数，
   蛋白通路中位成员数最小（8），因此拿到最大权重（均值 0.363）并主导 33.6%
   的观测。这会让 181 个蛋白特征对全基因组通路活性有超比例影响——若审稿人追问，
   建议补一个 `balance_feature_size=FALSE` 的敏感性分析。
2. **单层通路**：`Metabolic pathways` 只有蛋白层通过尺寸过滤（493 成员），
   其"多组学"分数实质等于蛋白层单组学分数，已在图 2C 与统计中保留但未纳入
   图 4A 的 ≥2 层子集；正式表格中建议标注或剔除。
3. **显著性仍未过 FDR**：200 条通路、1000 次置换下最小 q = 0.057，最接近的是
   Retinol metabolism / Ribosome / Viral protein interaction with cytokine
   and cytokine receptor（P = 0.0010）。置换检验本身偏保守（图 6B），
   若要以 FDR 为结论，需要扩大置换数或改用分层零分布。
4. **生存关联是探索性的**：全基因组 log-rank 筛查（335 条）只有 2 条过 FDR<0.05
   —— Synaptic vesicle cycle（P = 1.7e-5，q = 0.006）与 Endometrial cancer
   （P = 2.4e-4，q = 0.040）。图 7D 有意选用 ER 轴最强的 Hedgehog，其 P 值未校正，
   图注已如实说明。

## 六、改图建议

* 换配色：改 `00_common.R` 中 `PAL_OMICS` / `PAL_STATUS` / `GRAD_*` 一处即可全局生效。
* 换正文字号：`theme_paper(base_size = 7)`，图内各面板的 `size=` 是按 7 pt 调的，
  若整体放大需同步调整 `save_fig()` 的宽高。
* 换通路名单：图 2C、图 4A/C 的筛选逻辑都写在各自脚本顶部的 `top_*` 变量附近。
* 若期刊要求 TIFF：`ragg::agg_tiff(..., compression = "lzw")` 替换 `save_fig()` 中的
  PNG 设备即可。
* 换数据集：设 `MOGSVA_RESULT_DIR` / `MOGSVA_RAW_DIR` 指向新的导出目录即可，
  脚本会在缓存过期时自动重读。
