# =============================================================================
# Figure 2 -- the fused multi-omics pathway activity landscape
#   A  distribution of the most variable pathways
#   B  pathway-level variance before / after the consistency reward
#   C  heatmap of the 40 most variable pathways across 460 tumours
# (panel letters follow the reading order of the layout)
# =============================================================================

source(file.path(Sys.getenv("MOGSVA_SCRIPT_DIR",
                           "G:/GCModeller/test/demo/HTS/GSVA/mogsva_figures"),
                 "00_common.R"))
suppressPackageStartupMessages({
  library(ComplexHeatmap)
  library(circlize)
  library(matrixStats)
})

d <- load_mogsva()
S <- d$scores
R <- d$raw
an <- d$annotation

pv <- rowVars(S)
ord <- order(pv, decreasing = TRUE)

## ---- A: distribution of the most variable pathways -------------------------
top_a <- rownames(S)[ord][1:10]
long_a <- data.table(
  pathway = rep(top_a, times = ncol(S)),
  score   = as.vector(t(S[top_a, , drop = FALSE]))
)
long_a[, pathway := factor(pathway, levels = rev(top_a))]
med_a <- long_a[, .(med = median(score)), by = pathway]

p_violin <- ggplot(long_a, aes(score, pathway)) +
  geom_violin(aes(fill = pathway), scale = "width", colour = NA,
              alpha = 0.9, trim = TRUE) +
  geom_vline(xintercept = 0, linetype = 2, linewidth = 0.3, colour = "grey35") +
  geom_point(data = med_a, aes(x = med, y = pathway), shape = 21, size = 1.1,
             fill = "white", colour = "black", stroke = 0.3) +
  scale_fill_manual(values = rep("grey72", 10), guide = "none") +
  scale_y_discrete(labels = function(x) wrap_pathway(x, 34),
                   expand = expansion(add = 0.6)) +
  labs(x = "Fused activity score (ES_multi)", y = NULL,
       title = "Most variable pathways",
       subtitle = sprintf("Top 10 of %d KEGG pathways; open dot = median", d$n_pathway)) +
  theme(panel.grid.major.y = element_blank(),
        axis.text.y = element_text(size = 5.6, lineheight = 0.9))

## ---- B: pathway-level variance ---------------------------------------------
sd_dt <- data.table(
  pathway  = rownames(S),
  sd_raw   = rowSds(R),
  sd_fused = rowSds(S)
)
shrink <- 1 - median(sd_dt$sd_fused / sd_dt$sd_raw)

p_shrink <- ggplot(sd_dt, aes(sd_raw, sd_fused)) +
  geom_abline(slope = 1, intercept = 0, linetype = 2, linewidth = 0.3,
              colour = "grey40") +
  geom_point(size = 0.6, alpha = 0.5, colour = "#3C5488", stroke = 0) +
  annotate("text", x = max(sd_dt$sd_raw) * 0.99, y = min(sd_dt$sd_fused) * 1.03,
           hjust = 1, vjust = 0, size = 2.4, colour = "grey15", lineheight = 0.95,
           label = sprintf("median reduction\n%.1f%% of SD", 100 * shrink)) +
  annotate("text", x = max(sd_dt$sd_raw) * 0.42, y = max(sd_dt$sd_fused) * 0.55,
           hjust = 0, vjust = 1, size = 2.2, colour = "grey35", label = "y = x") +
  scale_x_continuous(expand = expansion(mult = c(0.02, 0.03))) +
  scale_y_continuous(expand = expansion(mult = c(0.03, 0.03))) +
  labs(x = "SD before consistency reward", y = "SD of ES_multi",
       title = "Agreement terms compress noisy pathways",
       subtitle = "One point per KEGG pathway") +
  coord_equal()

## ---- C: heatmap of the 40 most variable pathways ---------------------------
top_c <- rownames(S)[ord][1:40]
mat <- row_z(S[top_c, , drop = FALSE])
mat[mat > 2.5] <- 2.5
mat[mat < -2.5] <- -2.5

ann_col <- data.frame(
  ER      = factor(an$er, levels = c("Positive", "Negative")),
  PR      = factor(an$pr, levels = c("Positive", "Negative", "Indeterminate")),
  HER2    = factor(an$her2, levels = c("Positive", "Negative")),
  Subtype = factor(an$subtype,
                   levels = c("ER+/HER2-", "ER+/HER2+", "ER-/HER2-", "ER-/HER2+",
                              "Not classified")),
  row.names = an$sample_id,
  check.names = FALSE
)

ha <- HeatmapAnnotation(
  df = ann_col,
  col = list(
    ER      = c("Positive" = PAL_STATUS[["Positive"]], "Negative" = PAL_STATUS[["Negative"]]),
    PR      = c("Positive" = PAL_STATUS[["Positive"]], "Negative" = PAL_STATUS[["Negative"]],
                "Indeterminate" = PAL_STATUS[["Equivocal"]]),
    HER2    = c("Positive" = "#8E7CC3", "Negative" = "#C9BEDC"),
    Subtype = PAL_SUBTYPE
  ),
  na_col = "grey92",
  simple_anno_size = unit(1.5, "mm"),
  gp = gpar(col = "white", lwd = 0.25),
  annotation_name_gp = gpar(fontsize = 5.4),
  annotation_name_side = "left",
  gap = unit(0.5, "mm"),
  annotation_legend_param = list(
    ER      = list(title = "ER (IHC)", title_gp = gpar(fontsize = 5.6),
                   labels_gp = gpar(fontsize = 5.2), grid_height = unit(2.2, "mm"),
                   grid_width = unit(2.2, "mm")),
    PR      = list(title = "PR (IHC)", title_gp = gpar(fontsize = 5.6),
                   labels_gp = gpar(fontsize = 5.2), grid_height = unit(2.2, "mm"),
                   grid_width = unit(2.2, "mm")),
    HER2    = list(title = "HER2", title_gp = gpar(fontsize = 5.6),
                   labels_gp = gpar(fontsize = 5.2), grid_height = unit(2.2, "mm"),
                   grid_width = unit(2.2, "mm")),
    Subtype = list(title = "IHC subtype", title_gp = gpar(fontsize = 5.6),
                   labels_gp = gpar(fontsize = 5.2), grid_height = unit(2.2, "mm"),
                   grid_width = unit(2.2, "mm"))
  )
)

col_fun <- colorRamp2(c(-2.5, 0, 2.5), c(GRAD_LOW, GRAD_MID, GRAD_HIGH))

ht <- Heatmap(
  mat,
  name = "Row z-score",
  col = col_fun,
  top_annotation = ha,
  column_split = 3,
  column_gap = unit(1.4, "mm"),
  cluster_column_slices = TRUE,
  show_column_dend = FALSE,
  row_split = 3,
  row_gap = unit(1.2, "mm"),
  row_title = NULL,
  cluster_rows = TRUE,
  cluster_columns = TRUE,
  show_column_names = FALSE,
  row_names_side = "left",
  row_names_gp = gpar(fontsize = 5.1, lineheight = 0.85),
  column_title_gp = gpar(fontsize = 6.4, fontface = "bold"),
  column_title = sprintf("Tumours clustered by pathway activity (n = %d)", d$n_sample),
  row_dend_width = unit(7, "mm"),
  border = TRUE,
  border_gp = gpar(col = "grey20", lwd = 0.3),
  rect_gp = gpar(col = NA),
  heatmap_legend_param = list(
    title = "Row z-score", direction = "horizontal",
    title_position = "leftcenter", at = c(-2.5, 0, 2.5),
    labels = c("\u2264 -2.5", "0", "\u2265 2.5"),
    title_gp = gpar(fontsize = 5.6), labels_gp = gpar(fontsize = 5.2),
    legend_width = unit(22, "mm"), grid_height = unit(2.4, "mm")
  )
)

p_heat <- as_panel(ht, heatmap_legend_side = "bottom",
                   annotation_legend_side = "right",
                   padding = unit(c(0.5, 0.5, 0.5, 0.5), "mm"))

## ---- assemble --------------------------------------------------------------
fig2 <- (p_violin | p_shrink) / p_heat +
  plot_layout(heights = c(0.62, 1), widths = c(1.02, 1)) +
  plot_annotation(tag_levels = "A") &
  theme(plot.tag = element_text(size = 10, face = "bold"))

save_fig(fig2, "Figure2_activity_landscape", width_mm = 183, height_mm = 205)
