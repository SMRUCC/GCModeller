# =============================================================================
#  合成菌群 (SynCom) 转录组：DNMF 分解 + KEGG GSVA 结果的全套可视化
#
#  输入:
#    syncom/total.csv          并集定量总表达矩阵 (5236 基因 x 24 样本)
#    syncom/genomes.csv        菌株基因组注释 (strain/isolate/gene_id/symbol/KO/pathway)
#    syncom/families.csv       跨菌株同源基因家族 (family_id/gene_id)
#    syncom/pathways.gmt       KEGG 通路基因集 (18 条通路)
#    decomposed/*.csv          DNMF 分解出的 4 个菌株矩阵 (已按全基因集对齐, 5236 x 24)
#                              + abundance.csv + decomposition_summary.csv
#    decomposed_own/*.csv      同一分解结果, 但按各菌株自身基因组基因集抽取 (1317/1285 x 24)
#    gsva/pre_community.csv    群落水平 (分解前) GSVA 得分
#    gsva/post_<strain>.csv    菌株水平 (分解后) GSVA 得分
#
#  输出:
#    figures/fig01 ... fig11   出版级 PNG (300 dpi)
#    results/*.csv             绘图背后的统计表 (可直接作为补充材料)
#    results/key_findings.txt  关键数值清单
#
#  运行:
#    & "C:\Program Files\R\R-4.6.1\bin\Rscript.exe" <路径>\syncom_visualization.R
#
#  设计解读 (样本命名 <菌群>_<除草剂>_<群体感应>_<重复>):
#    YKB = Y1 + K1 + B1 ;  HKB = H1 + K1 + B1 (YKB 不含 H1, HKB 不含 Y1)
#    WT = 除草剂处理      NH = 无除草剂对照
#    WT = QS 正常         QSR = QS 受抑 (AHL/AI-2 干预)
#  若标签语义不同, 只需改 DESIGN_LABEL。
# =============================================================================

## ---------------------------------------------------------------- 0. 环境 --
for (loc in c("zh_CN.UTF-8", "Chinese_China.utf8", ".UTF-8")) {
  if (!is.na(suppressWarnings(Sys.setlocale("LC_CTYPE", loc)))) break
}

suppressPackageStartupMessages({
  library(data.table); library(ggplot2); library(dplyr); library(tidyr)
  library(tibble); library(ragg); library(patchwork); library(scales)
  library(ComplexHeatmap); library(circlize); library(grid)
})

ROOT <- "E:/codebuddy/GCModeller/test/omics/community"
FIG  <- file.path(ROOT, "figures"); dir.create(FIG, showWarnings = FALSE, recursive = TRUE)
RES  <- file.path(ROOT, "results"); dir.create(RES, showWarnings = FALSE, recursive = TRUE)

## ComplexHeatmap 构建期需要测量文本宽度；先挂一个 ragg 空设备，
## 避免回落到默认 pdf/基础 cairo 设备、导致中文字体在 PostScript 字库中缺失的告警。
.measure_dev <- tempfile(fileext = ".png")
agg_png(.measure_dev, width = 12, height = 8, units = "in", res = 72, background = "white")

FONT <- "Arial"   # 图内文字已全部改为英文, 统一使用 Arial

## 中文字体在基础 graphics 字库中缺失时, grid 会退回 PostScript 字库并抛出
## 度量告警 (仅影响度量, 不影响 ragg 实际出图)。这里只屏蔽这一条已知告警。
.qm <- function(expr) withCallingHandlers(expr, warning = function(w) {
  if (grepl("PostScript font database", conditionMessage(w))) invokeRestart("muffleWarning")
})
HT <- function(...) .qm(Heatmap(...))
HAnno <- function(...) .qm(HeatmapAnnotation(...))
theme_set(theme_bw(base_size = 10, base_family = FONT) +
            theme(panel.grid.minor = element_blank(),
                  panel.grid.major = element_line(colour = "grey92", linewidth = 0.3),
                  strip.background = element_rect(fill = "grey95", colour = "grey65"),
                  strip.text = element_text(face = "bold", size = 9),
                  axis.text = element_text(colour = "grey15"),
                  plot.title = element_text(face = "bold", size = 11),
                  plot.subtitle = element_text(size = 8.5, colour = "grey35"),
                  legend.key.size = unit(0.9, "lines"),
                  plot.tag = element_text(face = "bold", size = 12)))

## ------------------------------------------------------------ 1. 读数据 --
read_df <- function(f) {
  d <- fread(file.path(ROOT, f), data.table = FALSE, check.names = FALSE)
  names(d) <- sub("^\ufeff", "", names(d))
  d
}
read_mat <- function(f) {
  d <- read_df(f)
  m <- as.matrix(d[, -1, drop = FALSE]); rownames(m) <- d[[1]]
  storage.mode(m) <- "double"; m
}
## Windows 下若目标 PNG 正被预览程序占用, agg 会静默写入失败;
## 因此先写临时文件, 再替换目标文件。
replace_file <- function(tmp, dst) {
  if (file.exists(dst)) invisible(file.remove(dst))
  if (!file.rename(tmp, dst)) {
    file.copy(tmp, dst, overwrite = TRUE)
    invisible(file.remove(tmp))
  }
}
save_fig <- function(p, file, w, h, res = 300) {
  dst <- file.path(FIG, file); tmp <- paste0(dst, ".tmp.png")
  agg_png(tmp, width = w, height = h, units = "in", res = res, background = "white")
  print(p); invisible(dev.off())
  replace_file(tmp, dst); cat("  ->", file, "\n")
}
save_ht <- function(ht, file, w, h) {
  dst <- file.path(FIG, file); tmp <- paste0(dst, ".tmp.png")
  agg_png(tmp, width = w, height = h, units = "in", res = 300, background = "white")
  .qm(draw(ht)); invisible(dev.off())
  replace_file(tmp, dst); cat("  ->", file, "\n")
}

total <- read_mat("syncom/total.csv")
pre   <- read_mat("gsva/pre_community.csv")
gen   <- read_df("syncom/genomes.csv")
fam   <- read_df("syncom/families.csv")
abund <- read_mat("decomposed/abundance.csv")
dsum  <- read_df("decomposed/decomposition_summary.csv")

## 分解矩阵有两个版本, 用途不同:
##   dmat_full : 全基因集对齐 (每株 5 236 x 24) —— 用于逐基因的四菌组成、可回加校验;
##   dmat_own  : 按各菌自身基因组抽取 (1 317/1 285 x 24) —— 用于仅在本菌成员内成立的统计。
DCOMP_FULL <- "decomposed"
DCOMP_OWN  <- "decomposed_own"

STRAINS     <- c("Y1", "K1", "B1", "H1")
STRAIN_FULL <- c(Y1 = "Comamonas sp. Y1", K1 = "Acinetobacter sp. K1",
                 B1 = "Pseudomonas sp. B1", H1 = "Bacillus sp. H1")
MEMBERS     <- list(YKB = c("Y1", "K1", "B1"), HKB = c("K1", "B1", "H1"))

dmat_full <- setNames(lapply(STRAINS, function(s)
  read_mat(paste0(DCOMP_FULL, "/", STRAIN_FULL[[s]], ".csv"))), STRAINS)
dmat_own  <- setNames(lapply(STRAINS, function(s)
  read_mat(paste0(DCOMP_OWN,  "/", STRAIN_FULL[[s]], ".csv"))), STRAINS)

## 两版本应在本菌自有基因上一致(不一致说明两次导出口径不同)
own_check <- sapply(STRAINS, function(s) {
  g <- intersect(rownames(dmat_own[[s]]), rownames(dmat_full[[s]]))
  if (!length(g)) return(NA_real_)
  cor(as.numeric(dmat_own[[s]][g, ]), as.numeric(dmat_full[[s]][g, ]))
})

post <- setNames(lapply(STRAINS, function(s)
  read_mat(sprintf("gsva/post_%s.csv", gsub("[. ]+", "_", STRAIN_FULL[[s]])))), STRAINS)

## ------------------------------------------------- 2. 设计表与配色/字典 --
DESIGN_LABEL <- list(herb = c(WT = "Herbicide", NH = "No herbicide"),
                     qs   = c(WT = "QS active",  QSR = "QS inhibited"))

design <- data.frame(sample = colnames(total), stringsAsFactors = FALSE) %>%
  mutate(consortium = sub("_.*$", "", sample),
         herb = sub("^[A-Za-z]+_([A-Za-z]+)_.*$", "\\1", sample),
         qs   = sub("^[A-Za-z]+_[A-Za-z]+_([A-Za-z]+)_R[0-9]+$", "\\1", sample),
         rep  = sub(".*_", "", sample),
         herb_lab = unname(DESIGN_LABEL$herb[herb]),
         qs_lab   = unname(DESIGN_LABEL$qs[qs]),
         cond      = factor(paste(herb, qs, sep = "."),
                            levels = c("WT.WT", "WT.QSR", "NH.WT", "NH.QSR")),
         cond_lab  = factor(sprintf("%s\n%s", herb_lab, qs_lab),
                            levels = c("Herbicide\nQS active", "Herbicide\nQS inhibited",
                                       "No herbicide\nQS active", "No herbicide\nQS inhibited")),
         consortium = factor(consortium, levels = c("YKB", "HKB")))
rownames(design) <- design$sample
design <- design[colnames(total), ]

PAL_CON    <- c(YKB = "#0072B2", HKB = "#D55E00")
PAL_STRAIN <- c(Y1 = "#009E73", K1 = "#0072B2", B1 = "#CC79A7", H1 = "#E69F00")
PAL_CAT    <- c("Xenobiotic degradation" = "#E6A23C", "Amino acid metabolism" = "#4E9A51",
                "Energy metabolism" = "#C0392B", "Sulfur metabolism" = "#8E7CC3",
                "Quorum sensing & transport" = "#2E86C1", "Genetic information" = "#7F8C8D")

PATH_META <- tribble(
  ~pathway,   ~name_en,                       ~cat,
  "map00791", "Atrazine degradation",         "Xenobiotic degradation",
  "map00627", "Aminobenzoate degradation",    "Xenobiotic degradation",
  "map00983", "Drug metabolism - other enzymes", "Xenobiotic degradation",
  "map00380", "Tryptophan metabolism",        "Amino acid metabolism",
  "map00400", "Aromatic amino acid biosynthesis", "Amino acid metabolism",
  "map00270", "Cysteine and methionine metabolism", "Sulfur metabolism",
  "map00920", "Sulfur metabolism",            "Sulfur metabolism",
  "map00010", "Glycolysis / Gluconeogenesis", "Energy metabolism",
  "map00020", "Citrate cycle (TCA cycle)",    "Energy metabolism",
  "map00190", "Oxidative phosphorylation",    "Energy metabolism",
  "map02024", "Quorum sensing",               "Quorum sensing & transport",
  "map02025", "Biofilm formation",            "Quorum sensing & transport",
  "map02010", "ABC transporters",             "Quorum sensing & transport",
  "map03440", "Homologous recombination",     "Genetic information",
  "map03430", "Mismatch repair",              "Genetic information",
  "map03420", "Nucleotide excision repair",   "Genetic information",
  "map03010", "Ribosome",                     "Genetic information",
  "map00970", "Aminoacyl-tRNA biosynthesis",  "Genetic information"
)
PW_LEVELS <- PATH_META$pathway
pw_lab <- setNames(sprintf("%s %s", PATH_META$pathway, PATH_META$name_en), PATH_META$pathway)
pw_cat <- setNames(PATH_META$cat, PATH_META$pathway)

## 热图的行分块标题: 英文类别名较长, 若按中文原样竖排会彼此重叠,
## 故另备一套"两行短名", 并把行标题改为水平排布。
CAT_SHORT <- c("Xenobiotic degradation"     = "Xenobiotic\ndegradation",
               "Amino acid metabolism"      = "Amino acid\nmetabolism",
               "Energy metabolism"          = "Energy\nmetabolism",
               "Sulfur metabolism"          = "Sulfur\nmetabolism",
               "Quorum sensing & transport" = "QS &\ntransport",
               "Genetic information"        = "Genetic\ninformation")
pw_cat2 <- setNames(unname(CAT_SHORT[pw_cat]), names(pw_cat))
CAT_LV2 <- unname(CAT_SHORT[unique(PATH_META$cat)])

## ---------------------------------------------------------- 3. 派生矩阵 --
logcpm   <- function(m) log2(t(t(m) / colSums(m)) * 1e6 + 1)
lg_total <- logcpm(total)
idx_wt   <- design$herb == "WT" & design$qs == "WT"   # 基准状态: 除草剂 + QS 正常

## 3.1 原始 counts 的菌株构成 (独立于 DNMF 的旁证)
iso_of_gene <- sub("_.*$", "", rownames(total))
comp_raw <- sapply(colnames(total), function(s) tapply(total[, s], iso_of_gene, sum))
comp_raw <- as.data.frame(as.table(sweep(comp_raw, 2, colSums(comp_raw), "/")))
names(comp_raw) <- c("strain", "sample", "frac")
comp_raw <- comp_raw %>% mutate(sample = as.character(sample)) %>% left_join(design, by = "sample")

## 3.2 DNMF 相对丰度系数 -> 长表
ab_long <- as.data.frame(as.table(abund)); names(ab_long) <- c("strain_full", "sample", "factor")
ab_long <- ab_long %>%
  mutate(sample = as.character(sample),
         strain = unname(setNames(names(STRAIN_FULL), STRAIN_FULL)[strain_full])) %>%
  left_join(design, by = "sample")

## 3.3 GSVA -> 长表 (level = community / 菌株代号)
gsva_long <- function(m, tag) {
  d <- as.data.frame(as.table(m)); names(d) <- c("pathway", "sample", "score")
  d %>% mutate(sample = as.character(sample), level = tag) %>% left_join(design, by = "sample")
}
pre_long  <- gsva_long(pre, "community")
post_long <- bind_rows(lapply(STRAINS, function(s) gsva_long(post[[s]], s)))

## 3.4 对比检验: vary 一个因子, 固定另一个
paired_contrast <- function(d, vary, fixed, tag, levels_keep) {
  res <- list()
  keys <- d %>% filter(level %in% levels_keep) %>% distinct(level, consortium, pathway)
  for (i in seq_len(nrow(keys))) {
    k  <- keys[i, ]
    dd <- d %>% filter(level == k$level, consortium == k$consortium, pathway == k$pathway)
    for (fl in levels(factor(dd[[fixed]]))) {
      a <- dd$score[dd[[vary]] == "WT" & dd[[fixed]] == fl]
      b <- dd$score[dd[[vary]] != "WT" & dd[[fixed]] == fl]
      if (length(a) < 2 || length(b) < 2) next
      pv <- tryCatch(t.test(a, b)$p.value, error = function(e) NA_real_)
      res[[length(res) + 1]] <- data.frame(
        level = k$level, consortium = k$consortium, pathway = k$pathway, stratum = fl,
        contrast = tag, mean_WT = mean(a), mean_alt = mean(b),
        delta = mean(a) - mean(b), p = pv, stringsAsFactors = FALSE)
    }
  }
  out <- bind_rows(res)
  ## 每个"菌群 × 对比 × 分层"下 18 条通路构成一个检验族, 在该族内做 BH
  out %>% group_by(consortium, contrast, stratum) %>% mutate(padj = p.adjust(p, "BH")) %>% ungroup()
}

comm_herb <- paired_contrast(pre_long, "herb", "qs",  "herbicide", "community")
comm_qs   <- paired_contrast(pre_long, "qs",   "herb", "qs",       "community")

mem_levels <- unlist(MEMBERS, use.names = FALSE)
strain_herb <- paired_contrast(post_long, "herb", "qs",  "herbicide", mem_levels)
strain_qs   <- paired_contrast(post_long, "qs",   "herb", "qs",       mem_levels)

## 只保留该菌群真实存在的成员
keep_member <- function(d) {
  d %>% filter(mapply(function(cn, lv) lv %in% MEMBERS[[cn]], consortium, level))
}
strain_herb <- keep_member(strain_herb)
strain_qs   <- keep_member(strain_qs)

write.csv(bind_rows(comm_herb, comm_qs), file.path(RES, "table_contrasts_community.csv"), row.names = FALSE)
write.csv(bind_rows(strain_herb, strain_qs), file.path(RES, "table_contrasts_strain.csv"), row.names = FALSE)

## 3.4b 双因素方差分析: 2 x 2 设计下主效应与交互效应比逐格 t 检验更有检验力
anova_2way <- function(d, community = FALSE) {
  res <- list()
  for (cn in names(MEMBERS)) {
    lv_set <- if (community) "community" else MEMBERS[[cn]]
    for (lv in lv_set) {
      for (p in PW_LEVELS) {
        dd <- d %>% filter(consortium == cn, level == lv, pathway == p) %>%
          mutate(herb = factor(herb, levels = c("NH", "WT")),
                 qs   = factor(qs,   levels = c("QSR", "WT")))
        if (nrow(dd) < 8) next
        s <- summary(aov(score ~ herb * qs, data = dd))[[1]]
        rn <- trimws(rownames(s))          # summary.aov 会给行名补空格
        pick <- function(term, col) s[match(term, rn), col]
        ss_tot <- sum(s[, "Sum Sq"])
        res[[length(res) + 1]] <- data.frame(
          level = lv, consortium = cn, pathway = p,
          p_herb = pick("herb", "Pr(>F)"), p_qs = pick("qs", "Pr(>F)"),
          p_int  = pick("herb:qs", "Pr(>F)"),
          eta2_herb = pick("herb", "Sum Sq") / ss_tot,
          eta2_qs   = pick("qs", "Sum Sq") / ss_tot,
          stringsAsFactors = FALSE)
      }
    }
  }
  out <- bind_rows(res)
  out %>% group_by(level, consortium) %>%
    mutate(padj_herb = p.adjust(p_herb, "BH"), padj_qs = p.adjust(p_qs, "BH"),
           padj_int = p.adjust(p_int, "BH")) %>% ungroup()
}
anova_tab <- bind_rows(anova_2way(pre_long, community = TRUE), anova_2way(post_long))
write.csv(anova_tab, file.path(RES, "table_anova_2way.csv"), row.names = FALSE)

## 3.5 菌株贡献分解与群落得分的重构检验
ab_wide <- ab_long %>% select(sample, strain, factor) %>%
  mutate(strain = factor(strain, levels = STRAINS)) %>% pivot_wider(names_from = strain, values_from = factor)
ab_wide <- ab_wide[match(colnames(total), ab_wide$sample), ]

recon <- lapply(names(MEMBERS), function(cn) {
  ss <- colnames(total)[design$consortium == cn]
  w  <- as.matrix(ab_wide[match(ss, ab_wide$sample), MEMBERS[[cn]]])
  w  <- w / rowSums(w)
  obs <- pre[PW_LEVELS, ss]
  pred <- sapply(seq_along(ss), function(j) {
    sc <- sapply(MEMBERS[[cn]], function(s) post[[s]][PW_LEVELS, ss[j]])
    as.numeric(sc %*% w[j, ])
  })
  rownames(pred) <- PW_LEVELS
  list(consortium = cn, w = w, obs = obs, pred = pred, samples = ss)
})
names(recon) <- names(MEMBERS)

recon_df <- bind_rows(lapply(recon, function(r) {
  data.frame(consortium = r$consortium,
             pathway = rep(rownames(r$obs), ncol(r$obs)),
             sample = rep(colnames(r$obs), each = nrow(r$obs)),
             observed = as.numeric(r$obs), predicted = as.numeric(r$pred))
}))
r_all <- cor(recon_df$observed, recon_df$predicted)
write.csv(recon_df, file.path(RES, "table_reconstruction_check.csv"), row.names = FALSE)

## 3.6 家族 x 菌株 表达份额 (基准状态)
## 口径说明: 每个菌株只在"其所属菌群"的基准样本中评估, 分母取该样本内该家族的全成员
## 总表达量, 得到"群落内家族表达份额"; 再对 K1 这类同时存在于两个菌群的成员, 取其
## 所属菌群的均值。这样四个菌株才可直接横向比较。
## 若沿用"对全部 6 个基准样本直接取均值"的写法, Y1 / H1 会被其各自缺失的菌群稀释一半,
## 造成这两个菌株的偏好被系统性低估约 2 倍(并导致 H1 被误判为"零贡献")。
fam$isolate <- sub("_.*$", "", fam$gene_id)
fam_lv  <- sort(unique(fam$family_id))
fam_of  <- setNames(fam$family_id, fam$gene_id)
iso_of  <- setNames(fam$isolate,  fam$gene_id)
gene_all <- names(fam_of)

## 家族 x 样本 计数矩阵 (给定基因子集)
agg_fam <- function(genes) {
  if (!length(genes)) return(matrix(0, length(fam_lv), ncol(total), dimnames = list(fam_lv, colnames(total))))
  m <- rowsum(total[genes, , drop = FALSE], group = match(fam_of[genes], fam_lv), reorder = TRUE)
  full <- matrix(0, length(fam_lv), ncol(total), dimnames = list(fam_lv, colnames(total)))
  full[as.integer(rownames(m)), ] <- m
  full
}
## 每个菌群成员合计 (作分母)
den_cn <- lapply(MEMBERS, function(mem) agg_fam(gene_all[iso_of[gene_all] %in% mem]))

fam_wide <- sapply(STRAINS, function(s) {
  cs  <- agg_fam(gene_all[iso_of[gene_all] == s])
  cns <- names(MEMBERS)[vapply(MEMBERS, function(v) s %in% v, logical(1))]
  per_cn <- sapply(cns, function(cn) {
    ss <- colnames(total)[design$consortium == cn & idx_wt]
    rowMeans(cs[, ss, drop = FALSE] / den_cn[[cn]][, ss, drop = FALSE])
  })
  rowMeans(as.matrix(per_cn))
})
rownames(fam_wide) <- fam_lv

## 专一性: 四菌相对贡献中最大者所占份额 (均分基线 = 0.25)
fam_share <- sweep(fam_wide, 1, rowSums(fam_wide), "/")
fam_share[!is.finite(fam_share)] <- 0
fam_top   <- apply(fam_share, 1, function(v) STRAINS[which.max(v)])
fam_maxs  <- apply(fam_share, 1, max)
fam_tab <- data.frame(family_id = rownames(fam_wide),
                      total = rowSums(fam_wide),   # 该家族占群落总转录本的份额
                      top_strain = fam_top, top_share = fam_maxs, stringsAsFactors = FALSE)
write.csv(fam_tab[order(-fam_tab$top_share), ], file.path(RES, "table_family_specialization.csv"), row.names = FALSE)
## 明细: 四菌在关键家族中的群落内份额
write.csv(data.frame(family_id = rownames(fam_wide), as.data.frame(fam_wide), row.names = NULL),
          file.path(RES, "table_family_strain_share.csv"), row.names = FALSE)

## ------------------------------------------------------ 4. 汇总统计表 --
cond_mean_comm <- pre_long %>%
  group_by(consortium, herb, qs, pathway) %>%
  summarise(mean = mean(score), sd = sd(score), n = n(), .groups = "drop")
write.csv(cond_mean_comm, file.path(RES, "table_community_cond_mean.csv"), row.names = FALSE)

strain_mean_tab <- post_long %>%
  group_by(consortium, herb, qs, level, pathway) %>%
  summarise(mean = mean(score), sd = sd(score), n = n(), .groups = "drop")
write.csv(strain_mean_tab, file.path(RES, "table_strain_cond_mean.csv"), row.names = FALSE)

write.csv(comm_stat <- pre_long %>% group_by(pathway) %>%
            summarise(mean = mean(score), sd = sd(score), min = min(score), max = max(score)),
          file.path(RES, "table_community_gsva_summary.csv"), row.names = FALSE)

## =========================================================== FIG 1 设计 ====
cat("[FIG] 01 设计与数据总览\n")
dA <- design %>% distinct(consortium, cond, cond_lab)
pA <- ggplot(dA, aes(cond_lab, consortium, fill = consortium)) +
  geom_tile(colour = "white", linewidth = 1.4) +
  geom_text(label = "n = 3", family = FONT, size = 3.4, colour = "white", fontface = "bold") +
  scale_fill_manual(values = PAL_CON, guide = "none") +
  scale_y_discrete(labels = c(YKB = "YKB\nY1+K1+B1", HKB = "HKB\nH1+K1+B1")) +
  labs(x = NULL, y = NULL,
       title = "A  Experimental design: 2 consortia x 2 herbicide treatments x 2 QS states x 3 replicates = 24 samples") +
  theme(panel.grid = element_blank(), axis.text.y = element_text(size = 8.5, lineheight = 0.95))

## 所有基因均带 KEGG 注释, 故不再区分"全部/注释"两套柱, 改为标注覆盖的通路数
dB <- gen %>% mutate(code = sub("_.*$", "", gene_id)) %>%
  group_by(code) %>%
  summarise(genes = n(),
            n_pw = n_distinct(pathway[!is.na(pathway) & pathway != ""]), .groups = "drop") %>%
  mutate(code = factor(code, levels = STRAINS))
pB <- ggplot(dB, aes(code, genes, fill = code)) +
  geom_col(width = 0.62) +
  geom_text(aes(label = sprintf("%d genes\n%d pathways", genes, n_pw)), vjust = -0.28,
            size = 2.7, family = FONT, lineheight = 0.95) +
  scale_fill_manual(values = PAL_STRAIN, guide = "none") +
  scale_x_discrete(labels = STRAIN_FULL[STRAINS]) +
  scale_y_continuous(expand = expansion(mult = c(0, 0.20))) +
  labs(x = NULL, y = "Number of genes",
       title = "B  Genome size and KEGG annotation coverage per strain") +
  theme(axis.text.x = element_text(angle = 18, hjust = 1, size = 7.5))

gmt_lines <- readLines(file.path(ROOT, "syncom/pathways.gmt"))
gmt_tab <- data.frame(pathway = sub("\t.*$", "", gmt_lines),
                      n = lengths(strsplit(gmt_lines, "\t")) - 2)
dC <- gmt_tab %>% mutate(pathway = factor(pathway, levels = rev(PW_LEVELS)),
                         cat = pw_cat[as.character(pathway)],
                         name = pw_lab[as.character(pathway)])
pC <- ggplot(dC, aes(n, pathway, fill = cat)) +
  geom_col(width = 0.72) +
  geom_text(aes(label = n), hjust = -0.25, size = 2.7, family = FONT) +
  scale_fill_manual(values = PAL_CAT, name = "Pathway category") +
  scale_y_discrete(labels = setNames(sub(" ", "  ", dC$name), dC$pathway)) +
  scale_x_continuous(expand = expansion(mult = c(0, 0.13))) +
  labs(x = "Number of genes per pathway (union set)", y = NULL,
       title = "C  Gene-set size of the 18 KEGG pathways") +
  theme(axis.text.y = element_text(size = 6.6), legend.position = "right")

p1 <- pA / (pB | pC) + plot_layout(heights = c(1, 2.5)) +
  plot_annotation(caption = "Union quantification matrix: 5,236 genes x 24 samples; 97 cross-strain homologous families (3-4 copies per family)",
                  theme = theme(plot.caption = element_text(family = FONT, size = 7.5, colour = "grey40")))
save_fig(p1, "fig01_design_overview.png", 12, 7.6)

## ==================================================== FIG 2 全局表达 ======
cat("[FIG] 02 全局表达结构与 PCA\n")
d2 <- as.data.frame(lg_total) %>% tibble::rownames_to_column("geneID") %>%
  pivot_longer(-geneID, names_to = "sample", values_to = "logcpm") %>% left_join(design, by = "sample") %>%
  mutate(lab = sprintf("%s·%s·%s", herb, qs, rep),
         lab = factor(lab, levels = unique(lab[order(sample)])))

p2a <- ggplot(d2, aes(lab, logcpm, fill = cond)) +
  geom_boxplot(width = 0.72, outlier.size = 0.25, linewidth = 0.28) +
  facet_wrap(~consortium, scales = "free_x") +
  scale_fill_manual(values = c("WT.WT" = "#1F5FA8", "WT.QSR" = "#77ACD8",
                               "NH.WT" = "#C97B1E", "NH.QSR" = "#E9BC7C"), guide = "none") +
  labs(x = NULL, y = "Gene log2(CPM+1)",
       title = "A  Expression distribution of 5,236 genes per sample (library composition is consistent)") +
  theme(axis.text.x = element_text(angle = 90, vjust = 0.5, size = 5.2))

## 全局表达的 PCA 被菌群身份主导 (PC1 ≈ 96%), 处理效应几乎不可见;
## 因此 B 面板改用 18 条通路得分的 PCA, 直接呈现处理层面的结构。
pca_expr <- prcomp(t(lg_total[head(order(apply(lg_total, 1, var), decreasing = TRUE), 2000), ]))
ve_expr  <- 100 * pca_expr$sdev^2 / sum(pca_expr$sdev^2)

pca <- prcomp(t(pre), scale. = TRUE)   # 24 个样本为观测、18 条通路为变量
ve  <- round(100 * pca$sdev^2 / sum(pca$sdev^2), 1)
d2b <- data.frame(pca$x[, 1:2], sample = rownames(pca$x)) %>% left_join(design, by = "sample")
p2b <- ggplot(d2b, aes(PC1, PC2, colour = cond, shape = consortium)) +
  geom_point(size = 3.2, stroke = 0.9) +
  scale_colour_manual(values = c("WT.WT" = "#1F5FA8", "WT.QSR" = "#2E9BD6",
                                 "NH.WT" = "#C97B1E", "NH.QSR" = "#E9BC7C"),
                      labels = c("Herbicide, QS active", "Herbicide, QS inhibited",
                                 "No herbicide, QS active", "No herbicide, QS inhibited"),
                      name = NULL) +
  scale_shape_manual(values = c(YKB = 16, HKB = 17), name = NULL) +
  labs(x = sprintf("PC1 (%.1f%%)", ve[1]), y = sprintf("PC2 (%.1f%%)", ve[2]),
       title = "B  PCA of GSVA scores across the 18 KEGG pathways",
       subtitle = sprintf(paste0("Consortium identity (shape) and treatment (colour) jointly form the main variance;\n",
                                 "by contrast, PC1 of the whole-gene matrix alone explains %.0f%% and reflects almost only consortium identity"),
                          ve_expr[1]))
p2 <- p2a / p2b + plot_layout(heights = c(1, 1.15))
save_fig(p2, "fig02_global_expression_pca.png", 11, 8.6)

## ================================================ FIG 3 分解质量/丰度 ====
cat("[FIG] 03 DNMF 分解质量与菌株丰度\n")
comp <- comp_raw %>% mutate(strain = factor(strain, levels = STRAINS))
p3a <- ggplot(comp, aes(sample, frac, fill = strain)) +
  geom_col(width = 0.85) +
  facet_wrap(~consortium, scales = "free_x") +
  scale_fill_manual(values = PAL_STRAIN, labels = STRAIN_FULL, name = NULL) +
  scale_y_continuous(labels = percent_format(accuracy = 1), expand = c(0, 0)) +
  labs(x = NULL, y = "Fraction of total community counts",
       title = "A  Strain composition of raw counts (decomposition-free evidence)") +
  guides(fill = guide_legend(nrow = 2, byrow = TRUE)) +
  theme(axis.text.x = element_text(angle = 90, vjust = 0.5, size = 4.6),
        legend.position = "bottom", legend.text = element_text(size = 6.4),
        legend.key.size = unit(3.0, "mm"), legend.margin = margin(t = -2, b = 0))

d3b <- ab_long %>% mutate(strain = factor(strain, levels = STRAINS),
                          is_member = mapply(function(cn, s) s %in% MEMBERS[[cn]], consortium, strain),
                          grp = ifelse(is_member, as.character(strain), "Non-member"))
p3b <- ggplot(d3b, aes(cond_lab, factor, colour = grp, linetype = is_member,
                       group = interaction(consortium, grp))) +
  stat_summary(fun = mean, geom = "line", linewidth = 0.7) +
  stat_summary(fun = mean, geom = "point", size = 2.4) +
  stat_summary(fun.data = mean_se, geom = "errorbar", width = 0.12, linewidth = 0.4) +
  facet_wrap(~consortium) +
  scale_colour_manual(values = c(PAL_STRAIN, "Non-member" = "grey62"),
                      labels = c(STRAIN_FULL, "Non-member" = "Non-member (noise)"), name = NULL) +
  scale_linetype_manual(values = c(`TRUE` = "solid", `FALSE` = "22"), guide = "none") +
  labs(x = NULL, y = "DNMF relative abundance coefficient",
       title = "B  Strain abundance coefficients from the decomposition",
       subtitle = "Grey dashed = members absent from that consortium; all values are on the rescaled scale") +
  guides(colour = guide_legend(nrow = 2, byrow = TRUE)) +
  theme(axis.text.x = element_text(size = 7.0), legend.position = "bottom",
        legend.text = element_text(size = 6.4), legend.key.size = unit(3.6, "mm"),
        legend.margin = margin(t = -2, b = 0))

## 四株矩阵逐基因相加 (矩阵已按全基因集对齐, 直接同序相加)
rec_sum <- rowSums(sapply(STRAINS, function(s) {
  v <- setNames(rep(0, nrow(total)), rownames(total))
  m <- dmat_full[[s]]
  v[rownames(m)] <- m[, 1]
  v
}))
d3c_all <- data.frame(total = total[, 1], rec = rec_sum) %>% filter(total > 0)
d3c     <- d3c_all %>% filter(total >= 10, rec > 0)
p3c <- ggplot(d3c, aes(total, rec)) +
  geom_abline(slope = 1, intercept = 0, colour = "#C0392B", linetype = "22", linewidth = 0.5) +
  geom_point(size = 0.45, alpha = 0.35, colour = "#2C6E9B") +
  scale_x_log10(breaks = c(10, 30, 100, 300, 1000)) +
  scale_y_log10(breaks = c(10, 30, 100, 300, 1000)) +
  annotation_logticks(sides = "bl", linewidth = 0.2, size = 1.4) +
  annotate("text", x = 12, y = 900, hjust = 0,
           label = sprintf("r = %.4f (%d genes shown)", cor(d3c$total, d3c$rec), nrow(d3c)),
           family = FONT, size = 3.1) +
  labs(x = "Counts in the original community matrix (sample R1)",
       y = "Sum of the four strain matrices",
       title = "C  Additivity of the four strain matrices against the community matrix",
       subtitle = sprintf(paste0("All %d genes covered; per-gene r = %.3f. Each sample column is rescaled to the community total, so the four strain matrices sum to %.2f x the community total ",
                                 "(dashed line = 1:1)."),
                          nrow(total), cor(d3c$total, d3c$rec), sum(rec_sum) / sum(total[, 1])))

p3 <- (p3a | p3b) / p3c + plot_layout(heights = c(1.15, 1))
save_fig(p3, "fig03_decomposition_quality.png", 13.5, 8.4)

## ============================================ FIG 4 群落水平 GSVA 热图 ==
cat("[FIG] 04 群落 GSVA 热图\n")
zs <- t(scale(t(pre[PW_LEVELS, ])))
dimnames(zs)[[1]] <- pw_lab[PW_LEVELS]   # 标签写入行名, 随聚类一起重排
colnames(zs) <- design$sample
ha <- HAnno(
  Consortium = design$consortium,
  Herbicide = factor(design$herb_lab, levels = c("Herbicide", "No herbicide")),
  QS = factor(design$qs_lab, levels = c("QS active", "QS inhibited")),
  col = list(Consortium = PAL_CON,
             Herbicide = c("Herbicide" = "#C97B1E", "No herbicide" = "#E9BC7C"),
             QS = c("QS active" = "#1F5FA8", "QS inhibited" = "#77ACD8")),
  annotation_name_gp = gpar(fontfamily = FONT, fontsize = 8),
  annotation_legend_param = list(
    Consortium = list(title_gp = gpar(fontfamily = FONT, fontsize = 8), labels_gp = gpar(fontfamily = FONT, fontsize = 7.5)),
    Herbicide = list(title_gp = gpar(fontfamily = FONT, fontsize = 8), labels_gp = gpar(fontfamily = FONT, fontsize = 7.5)),
    QS = list(title_gp = gpar(fontfamily = FONT, fontsize = 8), labels_gp = gpar(fontfamily = FONT, fontsize = 7.5))),
  simple_anno_size = unit(3.2, "mm"), gap = unit(1.2, "mm"))
ht4 <- HT(zs, name = "Z-score", col = colorRamp2(c(-2, 0, 2), c("#2166AC", "white", "#B2182B")),
               row_split = factor(pw_cat2[PW_LEVELS], levels = CAT_LV2),
               row_title_rot = 0,
               cluster_row_slices = FALSE,
               cluster_rows = TRUE, cluster_columns = FALSE,
               column_split = design$consortium, column_gap = unit(2.5, "mm"),
               row_names_gp = gpar(fontfamily = FONT, fontsize = 7.6),
               column_names_gp = gpar(fontfamily = FONT, fontsize = 5.6),
               column_names_rot = 55,
               row_title_gp = gpar(fontfamily = FONT, fontsize = 8.5),
               column_title_gp = gpar(fontfamily = FONT, fontsize = 10, fontface = "bold"),
               heatmap_legend_param = list(title_gp = gpar(fontfamily = FONT, fontsize = 8),
                                           labels_gp = gpar(fontfamily = FONT, fontsize = 7.5)),
               top_annotation = ha,
               column_title = c("YKB (Y1 + K1 + B1)", "HKB (H1 + K1 + B1)"))
save_ht(ht4, "fig04_community_gsva_heatmap.png", 12.2, 7.2)

## ================================== FIG 5 群落水平关键通路因素效应 ====
cat("[FIG] 05 关键通路的处理效应\n")
KEY <- c("map00791", "map00627", "map00380", "map00010", "map00920", "map02024")
d5 <- pre_long %>% filter(pathway %in% KEY) %>%
  mutate(pathway = factor(pathway, levels = KEY),
         lbl = factor(pw_lab[as.character(pathway)], levels = pw_lab[KEY]))

## 显著性注释: x 位置与 cond 顺序一一对应
star_df <- bind_rows(comm_herb, comm_qs) %>%
  mutate(contrast = ifelse(contrast == "herbicide", "herbicide", "qs"),
         xpos = case_when(contrast == "herbicide" & stratum == "WT" ~ 2,
                          contrast == "herbicide" & stratum == "QSR" ~ 3,
                          contrast == "qs" & stratum == "WT" ~ 1.5,
                          contrast == "qs" & stratum == "NH" ~ 3.5),
         path = pathway,
         sig = as.character(cut(padj, c(-Inf, 0.001, 0.01, 0.05, Inf),
                                labels = c("***", "**", "*", "")))) %>%
  filter(path %in% KEY, padj < 0.05) %>%
  mutate(pathway = factor(path, levels = KEY),
         lbl = factor(pw_lab[path], levels = pw_lab[KEY]))

yr <- d5 %>% group_by(lbl, consortium) %>%
  summarise(lo = min(score), hi = max(score), .groups = "drop") %>%
  mutate(span = hi - lo)
star_df <- star_df %>% left_join(yr, by = c("lbl", "consortium")) %>%
  mutate(y = hi + ifelse(contrast == "qs", 0.26, 0.13) * span)

p5 <- ggplot(d5, aes(cond_lab, score)) +
  geom_hline(yintercept = 0, colour = "grey55", linewidth = 0.35) +
  geom_boxplot(aes(fill = cond), width = 0.62, outlier.shape = NA, linewidth = 0.35, alpha = 0.92) +
  geom_point(aes(colour = cond), position = position_jitter(width = 0.11, height = 0, seed = 1),
             size = 1.25, alpha = 0.85, show.legend = FALSE) +
  geom_text(data = star_df, aes(xpos, y, label = sig), family = FONT, size = 3.4, fontface = "bold", inherit.aes = FALSE) +
  facet_grid(consortium ~ lbl, scales = "free_y") +
  scale_fill_manual(values = c("WT.WT" = "#1F5FA8", "WT.QSR" = "#77ACD8",
                               "NH.WT" = "#C97B1E", "NH.QSR" = "#E9BC7C"), guide = "none") +
  scale_colour_manual(values = c("WT.WT" = "#123D6B", "WT.QSR" = "#3C7FB0",
                                 "NH.WT" = "#8E5310", "NH.QSR" = "#B98C4A"), guide = "none") +
  scale_y_continuous(expand = expansion(mult = c(0.07, 0.34))) +
  labs(x = NULL, y = "GSVA pathway activity score (community level)",
       title = "Treatment effects on key KEGG pathways at community level (n = 3 independent replicates)",
       subtitle = "Top row: YKB; bottom row: HKB. Asterisks: Welch t-test with within-family BH correction: * <0.05, ** <0.01, *** <0.001") +
  theme(axis.text.x = element_text(size = 6.3, angle = 32, hjust = 1, vjust = 1),
        strip.text = element_text(size = 8.5),
        panel.spacing.x = unit(2, "mm"), panel.spacing.y = unit(2.5, "mm"))
save_fig(p5, "fig05_community_pathway_effects.png", 15, 6.2)

## ================================== FIG 6 菌株水平 GSVA 分层热图 ====
cat("[FIG] 06 菌株水平通路活性\n")
## 基准状态 (除草剂 + QS 正常) 的菌株/群落均值
strain_base <- bind_rows(lapply(names(MEMBERS), function(cn) {
  sel <- design$consortium == cn & idx_wt
  bind_rows(lapply(MEMBERS[[cn]], function(s) {
    data.frame(consortium = cn, strain = s, pathway = PW_LEVELS,
               score = as.numeric(rowMeans(post[[s]][PW_LEVELS, sel, drop = FALSE])),
               stringsAsFactors = FALSE)
  }))
}))
blocks <- lapply(names(MEMBERS), function(cn) {
  sel <- design$consortium == cn & idx_wt
  comm <- rowMeans(pre[PW_LEVELS, sel, drop = FALSE])
  st <- sapply(MEMBERS[[cn]], function(s) rowMeans(post[[s]][PW_LEVELS, sel, drop = FALSE]))
  cbind(Community = comm, st)
})
names(blocks) <- names(MEMBERS)
mat6 <- do.call(cbind, blocks)
colnames(mat6) <- c("Community", "Y1", "K1", "B1", "Community", "K1", "B1", "H1")
mat6 <- mat6[PW_LEVELS, ]
rownames(mat6) <- pw_lab[PW_LEVELS]
## 用 ggplot 热图绘制: facet 的分块顺序由 factor 水平唯一决定, 不会出现色块与标题错配
comm_base <- bind_rows(lapply(names(MEMBERS), function(cn) {
  sel <- design$consortium == cn & idx_wt
  data.frame(consortium = cn, strain = "Community", pathway = PW_LEVELS,
             score = as.numeric(rowMeans(pre[PW_LEVELS, sel, drop = FALSE])),
             stringsAsFactors = FALSE)
}))
mat6 <- bind_rows(strain_base, comm_base) %>%
  mutate(consortium = factor(consortium, levels = c("YKB", "HKB")),
         strain = factor(strain, levels = c("Community", "Y1", "K1", "B1", "H1")),
         pathway = factor(pathway, levels = rev(PW_LEVELS)),
         cat = factor(pw_cat2[as.character(pathway)], levels = CAT_LV2),
         txt_col = ifelse(abs(score) > 0.6, "white", "grey20"))

p6 <- ggplot(mat6, aes(strain, pathway, fill = score)) +
  geom_tile(colour = "white", linewidth = 0.5) +
  geom_text(aes(label = sprintf("%.2f", score), colour = txt_col),
            family = FONT, size = 2.85) +
  scale_colour_identity() +
  scale_fill_gradient2(low = "#2166AC", mid = "white", high = "#B2182B",
                       midpoint = 0, limits = c(-1, 1), name = "GSVA\nscore") +
  facet_grid(cat ~ consortium, scales = "free", space = "free_y", switch = "y") +
  scale_x_discrete(drop = TRUE) +
  labs(x = NULL, y = NULL,
       title = "Strain-level KEGG pathway activity (baseline: herbicide, QS active)",
       subtitle = paste0("Each cell = GSVA mean of 3 replicates; ",
                         "'Community' column = computed from the pre-decomposition matrix")) +
  theme(panel.grid = element_blank(),
        axis.text.x = element_text(size = 8.5, colour = "grey10"),
        axis.text.y = element_text(size = 7),
        strip.text.y.left = element_text(angle = 0, size = 7.5),
        strip.background = element_rect(fill = "grey96", colour = "grey70"),
        panel.spacing = unit(1.2, "mm"), legend.position = "right")
save_fig(p6, "fig06_strain_level_heatmap.png", 8.4, 7.6)

## ==================================== FIG 7 菌株贡献与群落信号重构 ====
cat("[FIG] 07 菌株贡献分解\n")
KEY6 <- c("map00791", "map00380", "map00010", "map00920", "map02024", "map00983")
contrib <- bind_rows(lapply(names(MEMBERS), function(cn) {
  ss <- colnames(total)[design$consortium == cn & idx_wt]
  w  <- colMeans(recon[[cn]]$w)
  bind_rows(lapply(MEMBERS[[cn]], function(s) {
    data.frame(consortium = cn, strain = s, pathway = PW_LEVELS,
               contrib = w[s] * post[[s]][PW_LEVELS, ss, drop = FALSE] %>% rowMeans(),
               stringsAsFactors = FALSE)
  }))
})) %>% mutate(strain = factor(strain, levels = STRAINS),
                consortium = factor(consortium, levels = c("YKB", "HKB")),
                pathway = factor(pathway, levels = rev(PW_LEVELS)))
obs_comm <- data.frame(consortium = rep(names(MEMBERS), each = length(PW_LEVELS)),
                       pathway = rep(PW_LEVELS, 2),
                       observed = c(rowMeans(pre[PW_LEVELS, design$consortium == "YKB" & idx_wt]),
                                    rowMeans(pre[PW_LEVELS, design$consortium == "HKB" & idx_wt]))) %>%
  mutate(pathway = factor(pathway, levels = rev(PW_LEVELS)),
         consortium = factor(consortium, levels = c("YKB", "HKB")))

p7a <- ggplot(contrib, aes(contrib, pathway, fill = strain)) +
  geom_col(width = 0.74) +
  geom_point(data = obs_comm, aes(observed, pathway), inherit.aes = FALSE,
             shape = 124, size = 4.4, colour = "black", stroke = 1.2) +
  geom_vline(xintercept = 0, colour = "grey40", linewidth = 0.35) +
  facet_wrap(~consortium, nrow = 1) +
  scale_fill_manual(values = PAL_STRAIN, labels = STRAIN_FULL, name = NULL) +
  scale_y_discrete(labels = setNames(sub(" ", "  ", pw_lab[PW_LEVELS]), rev(PW_LEVELS))) +
  labs(x = "Abundance-weighted strain contribution (stack = reconstructed community score)", y = NULL,
       title = "A  Who contributes the community signal: strain activity x relative abundance (baseline)",
       subtitle = "Black vertical bars = measured community GSVA score; coloured stacked bars = summed weighted contributions of the members") +
  theme(axis.text.y = element_text(size = 6.4), legend.position = "bottom")

## 功能归属矩阵: 每条通路由哪株菌拿到最高得分, 与丰度位次是否一致
assign_tab <- bind_rows(lapply(names(MEMBERS), function(cn) {
  sel <- design$consortium == cn & idx_wt
  abr <- rank(-as.numeric(ab_wide[match(colnames(total)[sel][1], ab_wide$sample), MEMBERS[[cn]]]),
              ties.method = "min")
  names(abr) <- MEMBERS[[cn]]
  bind_rows(lapply(PW_LEVELS, function(p) {
    sc <- sapply(MEMBERS[[cn]], function(s) mean(post[[s]][p, sel]))
    top <- names(which.max(sc))
    data.frame(consortium = cn, pathway = p, top_strain = top,
               top_rank = abr[[top]], top_score = max(sc), stringsAsFactors = FALSE)
  }))
})) %>% mutate(consortium = factor(consortium, levels = c("YKB", "HKB")),
               pathway = factor(pathway, levels = rev(PW_LEVELS)),
               top_strain = factor(top_strain, levels = STRAINS),
               lab = sprintf("%s (%d)", top_strain, top_rank))

p7b <- ggplot(assign_tab, aes(consortium, pathway, fill = top_strain)) +
  geom_tile(colour = "white", linewidth = 1.1) +
  geom_text(aes(label = lab), family = FONT, size = 2.9, colour = "white", fontface = "bold") +
  scale_fill_manual(values = PAL_STRAIN, labels = STRAIN_FULL, name = "Functionally dominant strain") +
  scale_y_discrete(labels = setNames(sub(" ", "  ", pw_lab[PW_LEVELS]), rev(PW_LEVELS))) +
  labs(x = NULL, y = NULL,
       title = "B  Functional assignment and abundance rank",
       subtitle = "Cell text = top-scoring strain (bracket = abundance rank, 1 = most abundant)") +
  theme(panel.grid = element_blank(), axis.text.y = element_text(size = 6.4),
        axis.text.x = element_text(size = 8.5), legend.position = "bottom")

sign_agree <- mean(sign(recon_df$observed) == sign(recon_df$predicted))
p7c <- ggplot(recon_df, aes(observed, predicted)) +
  geom_hline(yintercept = 0, colour = "grey82", linewidth = 0.3) +
  geom_vline(xintercept = 0, colour = "grey82", linewidth = 0.3) +
  geom_point(aes(colour = consortium), size = 1.5, alpha = 0.65) +
  geom_smooth(aes(colour = consortium), method = "lm", se = FALSE, linewidth = 0.55,
              formula = y ~ x) +
  scale_colour_manual(values = PAL_CON, name = NULL) +
  annotate("text", x = -Inf, y = Inf, hjust = -0.12, vjust = 1.6,
           label = sprintf("r = %.2f; sign agreement = %.0f%%", r_all, 100 * sign_agree),
           family = FONT, size = 3) +
  labs(x = "Measured community GSVA score", y = "Weighted sum of strain scores",
       title = "C  GSVA scores are not linearly additive",
       subtitle = "GSVA is rank-based, so stratified GSVA is required\n(cf. r = 0.96 at expression level, Fig. 3C)") +
  theme(legend.position = "bottom")

p7 <- p7a / (p7b | p7c) + plot_layout(heights = c(1.3, 1.1))
save_fig(p7, "fig07_strain_contribution.png", 13, 9.6)

## ======================================== FIG 8 家族冗余与分工 ====
cat("[FIG] 08 同源家族冗余\n")
## 先在菌株维度去基线: 减去该菌株在所有家族上的平均表达, 才能看出"家族特异的偏好",
## 否则热图只会重复各菌株整体表达量高低的差异。
fam_center <- sweep(fam_wide, 2, colMeans(fam_wide), "-")
m8 <- t(scale(t(fam_center)))
m8[!is.finite(m8)] <- 0
## 排序: 按 B1 与 K1 的相对偏好差异, 把"偏好 B1"与"偏好 K1"的家族分块
ord <- order(-(m8[, "B1"] - m8[, "K1"]))
pref_strain <- STRAINS[apply(m8, 1, which.max)]
ha8 <- rowAnnotation(`Preferred` = pref_strain[ord],
                     col = list(`Preferred` = PAL_STRAIN),
                     annotation_name_gp = gpar(fontfamily = FONT, fontsize = 7.5),
                     annotation_legend_param = list(`Preferred` = list(
                       title_gp = gpar(fontfamily = FONT, fontsize = 8.5),
                       labels_gp = gpar(fontfamily = FONT, fontsize = 8),
                       grid_width = unit(4, "mm"), grid_height = unit(3, "mm"))),
                     simple_anno_size = unit(3.2, "mm"))
ht8 <- HT(m8[ord, ], name = "Z-score", col = colorRamp2(c(-1.5, 0, 1.5), c("#2166AC", "white", "#B2182B")),
               cluster_columns = FALSE, cluster_rows = FALSE,
               column_labels = STRAIN_FULL[STRAINS],
               column_names_gp = gpar(fontfamily = FONT, fontsize = 9),
               row_labels = rownames(m8)[ord], row_names_gp = gpar(fontsize = 5),
               right_annotation = ha8,
               heatmap_legend_param = list(title_gp = gpar(fontfamily = FONT, fontsize = 8.5),
                                           labels_gp = gpar(fontfamily = FONT, fontsize = 8),
                                           legend_height = unit(3, "cm")),
               column_title = "Expression preference of 97 cross-strain homologous families\n(strain-level overall baseline removed)",
               column_title_gp = gpar(fontfamily = FONT, fontsize = 9.5, fontface = "bold"))

## 各菌株在群落总转录本中的份额 (基准状态), 作为热图的"基线"参照
share_bar <- comp_raw %>% filter(herb == "WT", qs == "WT") %>%
  group_by(consortium, strain) %>% summarise(frac = mean(frac), .groups = "drop") %>%
  mutate(strain = factor(strain, levels = STRAINS),
         consortium = factor(consortium, levels = c("YKB", "HKB")))
p8b <- ggplot(share_bar, aes(consortium, frac, fill = strain)) +
  geom_col(position = position_dodge(width = 0.75), width = 0.68) +
  geom_text(aes(label = sprintf("%.0f%%", 100 * frac)), position = position_dodge(width = 0.75),
            vjust = -0.45, size = 2.7, family = FONT) +
  scale_fill_manual(values = PAL_STRAIN, labels = STRAIN_FULL, name = NULL) +
  scale_y_continuous(labels = percent_format(accuracy = 1), expand = expansion(mult = c(0, 0.16))) +
  labs(x = NULL, y = "Fraction of total community counts",
       title = "Baseline transcript share of each strain (baseline condition)",
       subtitle = "Read family-level preferences against this baseline; absent members sit at ~1% (noise)") +
  theme(legend.position = "bottom", legend.text = element_text(size = 7.5))
save_ht(ht8, "fig08_family_redundancy_heatmap.png", 6.2, 9.6)
save_fig(p8b, "fig08b_strain_baseline_share.png", 5.6, 4.6)

## ================================== FIG 9 差异活性热图 (Δ) ====
cat("[FIG] 09 处理效应 Δ 热图\n")
d9 <- bind_rows(
  comm_herb %>% filter(stratum == "WT") %>% mutate(eff = "Herbicide effect\n(QS active)"),
  comm_qs   %>% filter(stratum == "WT") %>% mutate(eff = "QS effect\n(herbicide)")
) %>% mutate(col_id = paste(consortium, eff, sep = " | "),
             pathway = factor(pathway, levels = rev(PW_LEVELS)))

mat9 <- d9 %>% select(pathway, col_id, delta) %>% pivot_wider(names_from = col_id, values_from = delta) %>%
  column_to_rownames("pathway") %>% as.matrix()
padj9 <- d9 %>% select(pathway, col_id, padj) %>% pivot_wider(names_from = col_id, values_from = padj) %>%
  column_to_rownames("pathway") %>% as.matrix()
## 注意: 行顺序必须与 row_labels 的顺序一致, 否则标签会与色块错位
mat9 <- mat9[PW_LEVELS, ]; padj9 <- padj9[PW_LEVELS, ]
rownames(mat9) <- pw_lab[PW_LEVELS]
colnames(mat9) <- gsub(" \\| ", "\n", colnames(mat9)); colnames(padj9) <- colnames(mat9)
lim9 <- max(abs(mat9), na.rm = TRUE)
star9 <- ifelse(padj9 < 0.001, "***", ifelse(padj9 < 0.01, "**", ifelse(padj9 < 0.05, "*", "")))
ht9 <- HT(mat9, name = "Δ GSVA",
               col = colorRamp2(c(-lim9, 0, lim9), c("#2166AC", "white", "#B2182B")),
               cluster_rows = FALSE, cluster_columns = FALSE,
               row_names_gp = gpar(fontfamily = FONT, fontsize = 7.6),
               column_names_gp = gpar(fontfamily = FONT, fontsize = 7.6),
               column_split = factor(sub("\n.*$", "", colnames(mat9)), levels = c("YKB", "HKB")),
               column_gap = unit(3, "mm"),
               row_split = factor(pw_cat2[PW_LEVELS], levels = CAT_LV2),
               row_title_rot = 0,
               cluster_row_slices = FALSE,
               row_title_gp = gpar(fontfamily = FONT, fontsize = 8.5),
               column_title_gp = gpar(fontfamily = FONT, fontsize = 9.5, fontface = "bold"),
               cell_fun = function(j, i, x, y, w, h, fill) {
                 v <- mat9[i, j]
                 txt <- ifelse(is.na(v), "", sprintf("%.2f%s", v, star9[i, j]))
                 col <- ifelse(abs(v) > lim9 * 0.55, "white", "grey20")
                 grid.text(txt, x, y, gp = gpar(fontfamily = FONT, fontsize = 6.2, col = col))
               },
               heatmap_legend_param = list(title_gp = gpar(fontfamily = FONT, fontsize = 8),
                                           labels_gp = gpar(fontfamily = FONT, fontsize = 7.5)))
save_ht(ht9, "fig09_delta_effects.png", 7.6, 7.4)

## ============================== FIG 10 菌株水平 Δ 热图 ====
cat("[FIG] 10 菌株水平 Δ 热图\n")
d10 <- bind_rows(
  strain_herb %>% filter(stratum == "WT") %>% mutate(eff = "Herbicide effect"),
  strain_qs   %>% filter(stratum == "WT") %>% mutate(eff = "QS effect")
) %>% mutate(col = sprintf("%s|%s|%s", eff, consortium, level),
             pathway = factor(pathway, levels = rev(PW_LEVELS)))
meta10 <- d10 %>% distinct(col, eff, consortium, level) %>%
  mutate(ord = order(match(eff, c("Herbicide effect", "QS effect")),
                     match(consortium, c("YKB", "HKB")),
                     match(level, STRAINS))) %>%
  arrange(ord)
mat10 <- d10 %>% select(pathway, col, delta) %>% pivot_wider(names_from = col, values_from = delta) %>%
  column_to_rownames("pathway") %>% as.matrix()
p10 <- d10 %>% select(pathway, col, padj) %>% pivot_wider(names_from = col, values_from = padj) %>%
  column_to_rownames("pathway") %>% as.matrix()
mat10 <- mat10[PW_LEVELS, meta10$col]; p10 <- p10[PW_LEVELS, meta10$col]
rownames(mat10) <- pw_lab[PW_LEVELS]
colnames(mat10) <- sprintf("%s\n%s", meta10$consortium, meta10$level)
st10 <- ifelse(p10 < 0.05, "*", "")
lim10 <- max(abs(mat10), na.rm = TRUE)
ht10 <- HT(mat10, name = "Δ GSVA",
                col = colorRamp2(c(-lim10, 0, lim10), c("#2166AC", "white", "#B2182B")),
                cluster_rows = FALSE, cluster_columns = FALSE,
                row_names_gp = gpar(fontfamily = FONT, fontsize = 7.6),
                column_names_gp = gpar(fontfamily = FONT, fontsize = 7.4),
                column_split = factor(meta10$eff, levels = c("Herbicide effect", "QS effect")),
                row_split = factor(pw_cat2[PW_LEVELS], levels = CAT_LV2),
                row_title_rot = 0,
                cluster_row_slices = FALSE,
                row_title_gp = gpar(fontfamily = FONT, fontsize = 8.5),
                column_title_gp = gpar(fontfamily = FONT, fontsize = 9.5, fontface = "bold"),
                cell_fun = function(j, i, x, y, w, h, fill) {
                  v <- mat10[i, j]
                  txt <- ifelse(is.na(v), "", sprintf("%.2f%s", v, st10[i, j]))
                  col <- ifelse(abs(v) > lim10 * 0.55, "white", "grey20")
                  grid.text(txt, x, y, gp = gpar(fontfamily = FONT, fontsize = 5.6, col = col))
                },
                heatmap_legend_param = list(title_gp = gpar(fontfamily = FONT, fontsize = 8),
                                            labels_gp = gpar(fontfamily = FONT, fontsize = 7.5)))
save_ht(ht10, "fig10_strain_delta_heatmap.png", 9.6, 7.6)

## ====================== FIG 11 基因层 PCA + 饼图符号 (pie glyph) ====
cat("[FIG] 11 基因层 PCA(饼图符号)\n")
## 图意: 每个"点"不是普通散点, 而是该基因的"菌株组成饼图"。
##   扇区份额 = 该基因所属跨菌株同源家族, 其 3-4 个拷贝的 DNMF 分解表达量在四菌间的比例。
##              注: 单个基因只存在于一个菌株的基因组(见 decomposed/ 各矩阵行数), 若按基因自身
##              统计份额恒为 [1,0,0,0], 饼图退化; 只有同源家族口径才会出现"接近均分"的情形。
##   饼图半径 = 与四菌间香农熵的"倒数"同向: spec = 1 - H/log2(4),
##              四菌均分(spec = 0)半径最小, 越接近 [1,0,0,0](spec = 1)半径越大。
##              本数据集的 spec 实测范围为 0.003(atzA, 几乎均分) - 0.295(fumA), 故半径按
##              观测范围线性铺满, 图例标注真实的 spec 数值而非 0-1 归一值。

## --- 11.1 家族口径的四菌组成 --------------------------------------------------
## 单个基因只属于一个菌株的基因组, 若逐基因统计"四菌组成"会退化为 [1,0,0,0](饼图为单色整圆);
## 只有跨菌株同源家族(3-4 个拷贝, 分处不同菌株)才存在可解释的四菌比例 —— 这也是本数据集中
## 唯一会出现"接近平均均分"情形的口径。
## 计算: 每个拷贝取其"归属菌株"的分解矩阵, 逐样本算四菌份额, 再对 24 个样本平均。
FAM_LV <- sort(unique(fam$family_id))
arrF <- array(0, c(length(FAM_LV), length(STRAINS), ncol(total)),
              dimnames = list(FAM_LV, STRAINS, colnames(total)))
for (s in STRAINS) {
  sub <- fam[fam$isolate == s, , drop = FALSE]
  if (!nrow(sub)) next
  arrF[sub$family_id, s, ] <- dmat_full[[s]][sub$gene_id, colnames(total)]
}
denF <- apply(arrF, c(1, 3), sum)
p_fam <- sapply(seq_along(STRAINS), function(k) rowMeans(arrF[, k, ] / denF))
dimnames(p_fam) <- list(FAM_LV, STRAINS)
p_fam[!is.finite(p_fam)] <- 1 / length(STRAINS)

H_fam    <- -rowSums(ifelse(p_fam > 0, p_fam * log2(p_fam), 0))
spec_fam <- 1 - H_fam / log2(length(STRAINS))      # 0 = 四菌均分, 1 = 单菌独占

## 基因层面: 家族内基因取所属家族的组成; 其余为自身菌株 100%
gene11   <- rownames(lg_total)
gene_fam <- setNames(fam$family_id, fam$gene_id)
gene_own <- sub("_.*$", "", gene11)
in_fam   <- gene11 %in% names(gene_fam)

p_gene <- matrix(0, length(gene11), length(STRAINS), dimnames = list(gene11, STRAINS))
p_gene[in_fam, ] <- p_fam[gene_fam[gene11[in_fam]], , drop = FALSE]
p_gene[cbind(which(!in_fam), match(gene_own[!in_fam], STRAINS))] <- 1
H_all    <- -rowSums(ifelse(p_gene > 0, p_gene * log2(p_gene), 0))
spec_all <- 1 - H_all / log2(length(STRAINS))

## --- 11.2 基因层面 PCA(全部基因) -------------------------------------------
pca11  <- prcomp(lg_total[gene11, , drop = FALSE], center = TRUE, scale. = FALSE)
ve11   <- 100 * pca11$sdev^2 / sum(pca11$sdev^2)
xy11 <- data.frame(gene   = rownames(pca11$x),
                   PC1    = pca11$x[, 1],
                   PC2    = pca11$x[, 2],
                   family = unname(gene_fam[rownames(pca11$x)]),
                   stringsAsFactors = FALSE)
xy11$f_ <- in_fam[match(xy11$gene, gene11)]
xy11$spec <- unname(spec_all[xy11$gene])

## --- 11.3 组成专一性(spec) 保留为数值指标, 不再用于半径 ----------------------
## 本版散点尺寸固定, spec 仅作为表内指标输出(specificity 列), 供后续筛选/排序使用。

## --- 11.4 CMYK 配色 + 散点 --------------------------------------------------
## 组成 -> CMYK -> RGB: 四个菌株的份额直接作为 C/M/Y/K 分量,
##   R = 255(1-C)(1-K),  G = 255(1-M)(1-K),  B = 255(1-Y)(1-K)
## 四菌均分 -> 中灰; 偏向单一菌株 -> 青 / 品红 / 黄 / 黑
CM_ORDER <- c(Y1 = "C", K1 = "M", B1 = "Y", H1 = "K")
cm <- p_gene[match(xy11$gene, rownames(p_gene)), , drop = FALSE]
cm <- cm / rowSums(cm)
R8 <- 255 * (1 - cm[, "Y1"]) * (1 - cm[, "H1"])
G8 <- 255 * (1 - cm[, "K1"]) * (1 - cm[, "H1"])
B8 <- 255 * (1 - cm[, "B1"]) * (1 - cm[, "H1"])
## 纯 CMYK 转出的 RGB 过于饱和(尤其 K 分量给出的纯黑)。若向白色混合, 黄色会被冲淡到
## 几乎看不见; 故改为向中灰收敛 —— 各色相同等降饱和, 深色被提亮、亮色被压深, 可见性守恒。
SOFT_GRAY <- 150
SOFT_MIX  <- 0.42
soften <- function(v) v * (1 - SOFT_MIX) + SOFT_GRAY * SOFT_MIX
R8 <- soften(R8); G8 <- soften(G8); B8 <- soften(B8)
xy11$R <- R8; xy11$G <- G8; xy11$B <- B8
xy11$col <- rgb(R8, G8, B8, maxColorValue = 255)

p11 <- ggplot(xy11, aes(PC1, PC2)) +
  geom_point(aes(colour = col), size = 1.0, alpha = 0.75, stroke = 0) +
  scale_colour_identity() +
  coord_fixed(clip = "off") +
  labs(x = sprintf("PC1 (%.1f%%)", ve11[1]), y = sprintf("PC2 (%.1f%%)", ve11[2]),
       title = "Gene-level PCA of the SynCom expression matrix, coloured by strain composition in CMYK",
       subtitle = paste0("Each point = one gene (all 5,236 genes), constant size. Colour = strain composition in CMYK: shares of Y1 / K1 / B1 / H1 act as the C / M / Y / K components.\n",
                         "R = 255(1-C)(1-K), G = 255(1-M)(1-K), B = 255(1-Y)(1-K). Even across the four strains -> mid-grey; dominated by one strain -> cyan / magenta / yellow / black.")) +
  theme(plot.title    = element_text(size = 14, face = "bold"),
        plot.subtitle = element_text(size = 9.2, colour = "grey30", lineheight = 1.2),
        axis.title    = element_text(size = 11.5),
        axis.text     = element_text(size = 10.5),
        legend.text   = element_text(size = 10))

## 图例(手工嵌入): 四个纯分量对应的颜色
leg_c <- data.frame(x = c(1, 2, 1, 2), y = c(2, 2, 1, 1),
                    strain = STRAINS,
                    fill = rgb(soften(c(0, 255, 255, 0)), soften(c(255, 0, 255, 0)),
                               soften(c(255, 255, 0, 0)), maxColorValue = 255),
                    txt  = c("black", "black", "black", "white"),
                    stringsAsFactors = FALSE)
p_leg <- ggplot(leg_c, aes(x, y)) +
  geom_tile(aes(fill = fill), colour = "white", linewidth = 0.7, width = 0.94, height = 0.94) +
  geom_text(aes(label = strain, colour = txt), size = 3.2, family = FONT, fontface = "bold") +
  scale_fill_identity() + scale_colour_identity() +
  coord_fixed(xlim = c(0.4, 2.6), ylim = c(0.4, 2.6), expand = FALSE) +
  labs(title = "CMYK = strain share") +
  theme_void(base_family = FONT) +
  theme(plot.title = element_text(size = 9, hjust = 0.5),
        plot.background = element_rect(fill = alpha("white", 0.85), colour = NA),
        plot.margin = margin(2, 2, 2, 2))
p11 <- p11 + inset_element(p_leg, left = 0.02, bottom = 0.02, right = 0.155, top = 0.215)
save_fig(p11, "fig11_gene_pca_pie.png", 11.5, 9, res = 600)

## 明细表: 基因 -> PC 坐标 / 所属家族 / 四菌份额 / 熵 / 半径
write.csv(data.frame(gene = xy11$gene,
                     family = xy11$family,
                     PC1 = xy11$PC1, PC2 = xy11$PC2,
                     p_gene[xy11$gene, , drop = FALSE],
                     H = unname(H_all[xy11$gene]),
                     H_norm = unname(H_all[xy11$gene]) / log2(length(STRAINS)),
                     specificity = xy11$spec,
                     R = xy11$R, G = xy11$G, B = xy11$B, colour = xy11$col,
                     row.names = NULL, check.names = FALSE),
          file.path(RES, "table_gene_pca_pie.csv"), row.names = FALSE, na = "")

## ==================================================== 11. 关键数值 ====
cat("[SUM] 写出关键数值\n")
fmt <- function(x, d = 3) formatC(x, format = "f", digits = d)
sel_pw <- c("map00791", "map00627", "map00380", "map00010", "map00920", "map02024")
strain_base_wide <- strain_base %>% filter(pathway %in% sel_pw) %>%
  mutate(col = sprintf("%s.%s", consortium, strain)) %>%
  select(pathway, col, score) %>% pivot_wider(names_from = col, values_from = score)
ab_base <- function(cn, s) ab_wide[match(paste0(cn, "_WT_WT_R1"), ab_wide$sample), s]
ab_rel <- ab_long %>%
  mutate(base = mapply(function(cn, s) as.numeric(ab_base(cn, s)), consortium, strain),
         rel = 100 * factor / base) %>%
  filter(cond != "WT.WT") %>%
  group_by(consortium, cond, strain) %>% summarise(rel = mean(rel), .groups = "drop")
d9_top <- head(d9[order(d9$padj), ], 12)
lines <- c(
  "# SynCom DNMF + GSVA 关键数值 (由 syncom_visualization.R 自动生成)",
  sprintf("基因数 %d ; 通路数 %d ; 样本数 %d ; 家族数 %d",
          nrow(total), length(PW_LEVELS), ncol(total), nrow(fam_tab)),
  sprintf("四矩阵之和/群落总量 = %s (RescaleToTotal 已启用, 逐列归一化到群落总量) ; 逐基因 r = %s",
          fmt(sum(rec_sum) / sum(total[, 1]), 4), fmt(cor(d3c$total, d3c$rec), 4)),
  sprintf("decomposed_own 与 decomposed 在本菌基因上的一致性 r = %s",
          paste(sprintf("%s:%.4f", STRAINS, own_check), collapse = "  ")),
  sprintf("逐基因四菌组成: spec(1-H/log2 4) 中位 = %s, 范围 %s - %s",
          fmt(median(spec_all), 4), fmt(min(spec_all), 4), fmt(max(spec_all), 4)),
  "",
  "## 菌株丰度系数 (基准状态: 除草剂 + QS 正常)",
  paste(sprintf("  YKB %-6s = %s", STRAINS, fmt(as.numeric(ab_base("YKB", STRAINS)), 3)), collapse = "\n"),
  paste(sprintf("  HKB %-6s = %s", STRAINS, fmt(as.numeric(ab_base("HKB", STRAINS)), 3)), collapse = "\n"),
  "",
  "## 丰度系数相对基准状态的变化 (%)",
  paste(sprintf("  %s %-8s %-6s: %s%%", ab_rel$consortium, ab_rel$cond, ab_rel$strain,
                fmt(ab_rel$rel, 0)), collapse = "\n"),
  "",
  "## 群落水平对比: 按 padj 排序的前 12 项",
  paste(sprintf("  %s | %-22s | %s | %s: Δ=%s padj=%s", d9_top$consortium,
                gsub("\\n", " ", d9_top$eff),   # eff 内含换行, 纯文本摘要里压平
                d9_top$pathway, d9_top$stratum, fmt(d9_top$delta), fmt(d9_top$padj, 4)),
        collapse = "\n"),
  "",
  "## 基准状态菌株层得分 (仅该菌群真实成员)",
  paste(sprintf("  %-9s %s", strain_base_wide$pathway,
                apply(strain_base_wide[, -1], 1, function(v)
                  paste(sprintf("%s=%s", names(v), fmt(as.numeric(v), 3)), collapse = "  "))),
        collapse = "\n"),
  "",
  sprintf("菌株层加权重构 vs 群落实测: r = %s ; 符号一致率 = %s%%", fmt(r_all, 4), fmt(100 * sign_agree, 0)),
  sprintf("家族层面: 优势份额中位数 %s ; 优势菌株分布 %s", fmt(median(fam_tab$top_share), 3),
          paste(sprintf("%s=%d", names(table(fam_tab$top_strain)),
                        as.integer(table(fam_tab$top_strain))), collapse = " ")),
  "",
  paste("生成时间:", format(Sys.time(), "%Y-%m-%d %H:%M:%S"))
)
writeLines(lines, file.path(RES, "key_findings.txt"))
cat(paste(lines, collapse = "\n"), "\n")
while (dev.cur() > 1) invisible(dev.off())
cat("\n[DONE] 全部图件与统计表已输出到 figures/ 与 results/\n")
