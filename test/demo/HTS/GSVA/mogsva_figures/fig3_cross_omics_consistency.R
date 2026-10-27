# =============================================================================
# Figure 3 -- cross-omics agreement and how it reshapes the activity scores
#   A  distribution of the per-sample sign agreement a
#   B  agreement composition by number of contributing layers
#   C  layer-to-layer concordance of per-omics enrichment scores
#   D  agreement versus pathway-level dispersion
# =============================================================================

source(file.path(Sys.getenv("MOGSVA_SCRIPT_DIR",
                           "G:/GCModeller/test/demo/HTS/GSVA/mogsva_figures"),
                 "00_common.R"))
suppressPackageStartupMessages(library(matrixStats))

d <- load_mogsva()
S <- d$scores
C <- d$cons

PAL_AGREE <- c("-1" = "#2F5D8C", "-0.333" = "#7098BC", "0" = "#E8A87C", "1" = "#A6362A")
LAB_AGREE <- c("-1" = "\u22121", "-0.333" = "\u22121/3", "0" = "0", "1" = "1")

## ---- A: agreement distribution ---------------------------------------------
ag <- data.table(a = round(as.vector(C), 3))
ag[, a_lab := factor(a, levels = c(-1, -0.333, 0, 1))]
n_total <- nrow(ag)
tab <- ag[!is.na(a_lab), .N, by = a_lab][order(as.numeric(as.character(a_lab)))]
tab[, pct := 100 * N / n_total]

pA <- ggplot(tab, aes(a_lab, pct, fill = a_lab)) +
  geom_col(width = 0.68, colour = "grey25", linewidth = 0.25) +
  geom_text(aes(label = sprintf("%s\n(%.1f%%)", comma(N), pct)),
            vjust = -0.2, size = 2.25, lineheight = 0.9, colour = "grey15") +
  scale_fill_manual(values = PAL_AGREE, guide = "none") +
  scale_x_discrete(labels = LAB_AGREE[tab$a_lab]) +
  scale_y_continuous(expand = expansion(mult = c(0, 0.16)),
                     breaks = seq(0, 60, 10), labels = function(x) paste0(x, "%")) +
  labs(x = "Cross-omics sign agreement  a", y = "Share of observations",
       title = "Agreement is rarely unanimous",
       subtitle = sprintf("%s pathway x tumour observations; %s fully concordant",
                          comma(n_total), comma(tab[a_lab == "1", N]))) +
  theme(panel.grid.major.x = element_blank())

## ---- B: agreement composition by number of contributing layers -------------
# a is a pairwise sign average, so its attainable values depend on K:
# K = 2 -> {-1, 1}; K = 3 -> {-1, -1/3, 1}; K = 4 -> {-1, -2/3, -1/3, 0, 1/3, 2/3, 1}
K <- d$coverage[, .(n_layer = sum(member_count > 0)), by = pathway]
k_long <- merge(
  data.table(pathway = rep(rownames(C), times = ncol(C)),
             a = round(as.vector(C), 3)),
  K, by = "pathway"
)
k_long[, a_lab := factor(a, levels = c(-1, -0.333, 0, 1))]
k_tab <- k_long[!is.na(a_lab) & n_layer >= 2,
                .N, by = .(n_layer, a_lab)][, pct := 100 * N / sum(N), by = n_layer]
k_n <- k_long[n_layer >= 2, .(n_path = uniqueN(pathway)), by = n_layer]

pB <- ggplot(k_tab, aes(factor(n_layer), pct, fill = a_lab)) +
  geom_col(width = 0.66, colour = "white", linewidth = 0.35) +
  geom_text(aes(label = ifelse(pct > 7, sprintf("%.0f%%", pct), "")),
            position = position_stack(vjust = 0.5), size = 2.1, colour = "grey10") +
  scale_fill_manual(values = PAL_AGREE, labels = LAB_AGREE, name = "Agreement",
                    drop = FALSE) +
  scale_x_discrete(labels = function(x) {
    paste0(x, " layers\n(", k_n$n_path[match(as.integer(x), k_n$n_layer)], " pathways)")
  }) +
  scale_y_continuous(expand = expansion(mult = c(0, 0.03)),
                     labels = function(x) paste0(x, "%")) +
  labs(x = "Omics layers contributing to a pathway", y = "Share of observations",
       title = "Agreement is quantised by layer number",
       subtitle = "Pairwise sign average a can only take K(K-1)/2 + 1 values") +
  theme(panel.grid.major.x = element_blank())

## ---- C: layer-to-layer concordance -----------------------------------------
ls <- d$layer_scores
wide <- dcast(ls[!is.na(score)], pathway + sample ~ omics, value.var = "score")
setnames(wide, OMICS_LEVELS, c("CNA", "MET", "TX", "PROT"))
col_of <- c("Copy number alteration" = "CNA", "Methylation" = "MET",
            "Transcriptomics" = "TX", "Proteomics" = "PROT")

pairs <- CJ(a = OMICS_LEVELS, b = OMICS_LEVELS, sorted = FALSE)
pairs[, `:=`(a = factor(a, levels = OMICS_LEVELS),
             b = factor(b, levels = OMICS_LEVELS),
             median_r = NA_real_, n_path = NA_integer_)]
for (i in seq_len(nrow(pairs))) {
  x <- col_of[as.character(pairs$a[i])]
  y <- col_of[as.character(pairs$b[i])]
  pairs$n_path[i] <- uniqueN(wide$pathway[!is.na(wide[[x]])])
  if (x == y) {
    pairs$median_r[i] <- 1
    next
  }
  sub <- wide[!is.na(get(x)) & !is.na(get(y))]
  if (nrow(sub) == 0) next
  r <- sub[, stats::cor(get(x), get(y)), by = pathway]$V1
  pairs$median_r[i] <- median(r)
}

lab_col <- ifelse(is.na(pairs$median_r), "grey20",
                  ifelse(abs(pairs$median_r) > 0.55, "white", "grey10"))

pC <- ggplot(pairs, aes(a, b, fill = median_r)) +
  geom_tile(colour = "white", linewidth = 0.7) +
  geom_text(aes(label = ifelse(is.na(median_r), "NA",
                               sprintf("%.2f\n%d", median_r, n_path))),
            size = 2.1, lineheight = 0.85, colour = lab_col) +
  scale_fill_gradient2(low = GRAD_LOW, mid = GRAD_MID, high = GRAD_HIGH,
                       midpoint = 0, limits = c(-0.3, 1),
                       breaks = c(-0.3, 0, 0.3, 0.6, 1),
                       na.value = "grey92", name = "Median r",
                       guide = guide_colourbar(direction = "horizontal",
                                               barwidth = unit(18, "mm"),
                                               barheight = unit(2.2, "mm"),
                                               title.vjust = 0.85)) +
  scale_x_discrete(labels = function(x) OMICS_LABEL[x], position = "top") +
  scale_y_discrete(labels = function(x) OMICS_LABEL[x]) +
  labs(x = NULL, y = NULL,
       title = "Layers carry partly independent information",
       subtitle = "Median per-pathway Pearson r across tumours (number of pathways)") +
  theme(panel.grid = element_blank(),
        legend.position = "bottom",
        legend.title = element_text(size = 5.4),
        axis.text.x.top = element_text(angle = 0, hjust = 0.5)) +
  coord_fixed()

## ---- D: agreement vs dispersion --------------------------------------------
disp <- data.table(
  pathway = rownames(S),
  sd      = rowSds(S),
  agree   = rowMeans(C)
)
rho <- cor(disp$agree, disp$sd, method = "spearman")

pD <- ggplot(disp, aes(sd, agree)) +
  geom_hline(yintercept = 0, linetype = 3, linewidth = 0.3, colour = "grey55") +
  geom_point(size = 0.6, alpha = 0.5, colour = "#3C5488", stroke = 0) +
  geom_smooth(method = "lm", formula = y ~ x, se = TRUE, linewidth = 0.45,
              colour = "#A6362A", fill = "grey85", alpha = 0.6) +
  annotate("text", x = max(disp$sd) * 0.99, y = min(disp$agree) - 0.05,
           hjust = 1, vjust = 0, size = 2.4, colour = "grey15",
           label = sprintf("Spearman \u03C1 = %.2f", rho)) +
  scale_y_continuous(expand = expansion(mult = c(0.1, 0.05))) +
  labs(x = "SD of ES_multi across tumours", y = "Mean agreement a",
       title = "Coherent pathways vary more",
       subtitle = "One point per KEGG pathway") +
  theme(panel.grid.major.x = element_line(linewidth = 0.2, colour = "grey93"))

## ---- assemble --------------------------------------------------------------
fig3 <- (pA | pB) / (pC | pD) +
  plot_layout(heights = c(1, 1.15), widths = c(1, 1.02)) +
  plot_annotation(tag_levels = "A") &
  theme(plot.tag = element_text(size = 10, face = "bold"))

save_fig(fig3, "Figure3_cross_omics_consistency", width_mm = 183, height_mm = 172)
