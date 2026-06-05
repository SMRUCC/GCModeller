#!/usr/bin/env Rscript
###############################################################################
# BNLearn Gene Regulatory Network Virtual Perturbation Analysis
# Comprehensive R Plotting Script for Publication-Quality Figures
#
# This script generates PDF and PNG figures from 12 CSV result tables
# produced by the InterventionComparisonExporter module.
#
# All figure annotations, titles, and text elements are in English.
#
# Required R packages: pheatmap, ggplot2, reshape2, RColorBrewer, scales,
#   ggrepel, grid, gridExtra, ComplexHeatmap, circlize, dplyr, tidyr,
#   ggpubr, viridis, cowplot
#
# Usage: Rscript bnlearn_plotting_script.R <data_dir> <output_dir>
#   data_dir:   directory containing the 12 CSV files (default: ./bnlearn_results)
#   output_dir: directory for output PDF/PNG files (default: ./figures)
###############################################################################

# ── Command-line arguments ───────────────────────────────────────────────────
args <- commandArgs(trailingOnly = TRUE)
DATA_DIR  <- ifelse(length(args) >= 1, args[1], "./bnlearn_results")
OUT_DIR   <- ifelse(length(args) >= 2, args[2], "./figures")

dir.create(OUT_DIR, recursive = TRUE, showWarnings = FALSE)

# ── Helper: read CSV skipping comment lines (#) and handling BOM ─────────────
read_csv_skip_comments <- function(filepath) {
  lines <- readLines(filepath, encoding = "UTF-8")
  # Remove BOM if present
  if (length(lines) > 0 && startsWith(lines[1], "\uFEFF")) {
    lines[1] <- sub("^\uFEFF", "", lines[1])
  }
  # Keep only non-comment, non-empty lines
  data_lines <- lines[!grepl("^\\s*#", lines) & nzchar(trimws(lines))]
  if (length(data_lines) == 0) return(data.frame())
  read.csv(text = data_lines, stringsAsFactors = FALSE, check.names = FALSE)
}

# ── Helper: save a ggplot object to both PDF and PNG ─────────────────────────
save_fig <- function(p, name, width = 10, height = 8, dpi = 300) {
  pdf_path <- file.path(OUT_DIR, paste0(name, ".pdf"))
  png_path <- file.path(OUT_DIR, paste0(name, ".png"))
  ggsave(pdf_path, plot = p, width = width, height = height, device = cairo_pdf)
  ggsave(png_path, plot = p, width = width, height = height, dpi = dpi)
  message(sprintf("  Saved: %s & %s", basename(pdf_path), basename(png_path)))
}

# ── Helper: save a base-R / pheatmap / ComplexHeatmap plot ───────────────────
save_base_fig <- function(plot_fn, name, width = 10, height = 8, dpi = 300) {
  pdf_path <- file.path(OUT_DIR, paste0(name, ".pdf"))
  png_path <- file.path(OUT_DIR, paste0(name, ".png"))
  cairo_pdf(pdf_path, width = width, height = height)
  plot_fn()
  dev.off()
  png(png_path, width = width, height = height, units = "in", res = dpi)
  plot_fn()
  dev.off()
  message(sprintf("  Saved: %s & %s", basename(pdf_path), basename(png_path)))
}

# ── Color palettes ───────────────────────────────────────────────────────────
# Diverging palette for FoldChange / Z-Score
div_palette <- circlize::colorRamp2(c(-2, 0, 2), c("#2166AC", "#F7F7F7", "#B2182B"))
# Sequential palette for PercentChange
seq_palette <- circlize::colorRamp2(c(0, 100, 300), c("#FFFFCC", "#FD8D3C", "#800026"))
# Binary palette for significance
bin_palette <- c("0" = "#F7F7F7", "1" = "#D6604D")
# Correlation palette
corr_palette <- circlize::colorRamp2(c(-1, 0, 1), c("#2166AC", "#F7F7F7", "#B2182B"))

# ── Condition label shortener ────────────────────────────────────────────────
short_cond <- function(cond) {
  # e.g. "sigH_Knockout" -> "sigH\nKO"
  parts <- strsplit(cond, "_")[[1]]
  gene <- parts[1]
  mode <- ifelse(length(parts) >= 2, parts[2], "")
  mode_short <- switch(mode,
    "Knockout"       = "KO",
    "Knockdown"      = "KD",
    "Overexpression" = "OE",
    mode)
  paste0(gene, "\n", mode_short)
}

short_cond_vec <- function(conds) sapply(conds, short_cond)

# ── Load all data ────────────────────────────────────────────────────────────
message("Loading data from: ", DATA_DIR)

fc_mat     <- read_csv_skip_comments(file.path(DATA_DIR, "foldchange_matrix.csv"))
pc_mat     <- read_csv_skip_comments(file.path(DATA_DIR, "percentchange_matrix.csv"))
sig_mat    <- read_csv_skip_comments(file.path(DATA_DIR, "significance_matrix.csv"))
zs_mat     <- read_csv_skip_comments(file.path(DATA_DIR, "zscore_matrix.csv"))
wt_mat     <- read_csv_skip_comments(file.path(DATA_DIR, "wildtype_means_matrix.csv"))
mt_mat     <- read_csv_skip_comments(file.path(DATA_DIR, "mutant_means_matrix.csv"))
comp_tab   <- read_csv_skip_comments(file.path(DATA_DIR, "comprehensive_comparison.csv"))
cond_sim   <- read_csv_skip_comments(file.path(DATA_DIR, "condition_similarity.csv"))
gene_sens  <- read_csv_skip_comments(file.path(DATA_DIR, "gene_sensitivity.csv"))
int_rank   <- read_csv_skip_comments(file.path(DATA_DIR, "intervention_ranking.csv"))
path_sum   <- read_csv_skip_comments(file.path(DATA_DIR, "pathway_summary.csv"))
cross_imp  <- read_csv_skip_comments(file.path(DATA_DIR, "cross_impact_matrix.csv"))

# ── Prepare matrix objects (gene as row, conditions as columns) ──────────────
make_matrix <- function(df, row_col = "Gene") {
  mat <- as.matrix(df[, -which(names(df) == row_col)])
  rownames(mat) <- df[[row_col]]
  storage.mode(mat) <- "numeric"
  return(mat)
}

fc_m  <- make_matrix(fc_mat)
pc_m  <- make_matrix(pc_mat)
sig_m <- make_matrix(sig_mat)
zs_m  <- make_matrix(zs_mat)
wt_m  <- make_matrix(wt_mat)
mt_m  <- make_matrix(mt_mat)

cond_names <- colnames(fc_m)

###############################################################################
# TABLE 1 — FoldChange Matrix
###############################################################################

message("\n=== Table 1: FoldChange Matrix ===")

# ── Fig 1a: Gene x Condition FoldChange Heatmap ─────────────────────────────
message("  Fig 1a: FoldChange heatmap ...")
save_base_fig(function() {
  pheatmap::pheatmap(
    fc_m,
    color = colorRampPalette(c("#2166AC", "#F7F7F7", "#B2182B"))(100),
    breaks = seq(-max(abs(range(fc_m, na.rm = TRUE))), max(abs(range(fc_m, na.rm = TRUE))), length.out = 101),
    cluster_rows = TRUE,
    cluster_cols = TRUE,
    show_rownames = FALSE,
    fontsize_col = 9,
    main = "FoldChange Heatmap: Genes x Perturbation Conditions",
    labels_col = short_cond_vec(colnames(fc_m)),
    border_color = NA,
    fontsize = 10
  )
}, "fig1a_foldchange_heatmap", width = 9, height = 12)

# ── Fig 1b: Hierarchical Clustering Heatmap (with dendrograms) ──────────────
message("  Fig 1b: Hierarchical clustering heatmap ...")
save_base_fig(function() {
  ComplexHeatmap::Heatmap(
    fc_m,
    name = "FoldChange",
    col = colorRampPalette(c("#2166AC", "#F7F7F7", "#B2182B"))(100),
    cluster_rows = TRUE,
    cluster_columns = TRUE,
    show_row_names = FALSE,
    column_labels = short_cond_vec(colnames(fc_m)),
    column_title = "Hierarchical Clustering Heatmap of FoldChange",
    heatmap_legend_param = list(direction = "horizontal"),
    border = TRUE
  )
}, "fig1b_foldchange_clustering_heatmap", width = 9, height = 12)

# ── Fig 1c: Top 20 variable genes bar chart ─────────────────────────────────
message("  Fig 1c: Top variable genes bar chart ...")
{
  gene_var <- apply(fc_m, 1, var, na.rm = TRUE)
  top20 <- names(sort(gene_var, decreasing = TRUE))[1:20]
  fc_top20 <- fc_m[top20, , drop = FALSE]
  fc_long <- reshape2::melt(fc_top20)
  colnames(fc_long) <- c("Gene", "Condition", "FoldChange")
  fc_long$Condition <- factor(fc_long$Condition, levels = cond_names)
  fc_long$Gene <- factor(fc_long$Gene, levels = rev(top20))

  p1c <- ggplot(fc_long, aes(x = Gene, y = FoldChange, fill = Condition)) +
    geom_bar(stat = "identity", position = "dodge", width = 0.7) +
    scale_fill_brewer(palette = "Set2", labels = short_cond_vec(cond_names)) +
    coord_flip() +
    labs(
      title = "Top 20 Most Variable Genes: FoldChange Across Conditions",
      x = "Gene", y = "FoldChange", fill = "Condition"
    ) +
    theme_bw(base_size = 11) +
    theme(
      plot.title = element_text(face = "bold", hjust = 0.5),
      legend.position = "right"
    )
  save_fig(p1c, "fig1c_top20_variable_genes_bar", width = 11, height = 7)
}

###############################################################################
# TABLE 2 — PercentChange Matrix
###############################################################################

message("\n=== Table 2: PercentChange Matrix ===")

# ── Fig 2a: PercentChange Heatmap ────────────────────────────────────────────
message("  Fig 2a: PercentChange heatmap ...")
{
  # Winsorize extreme values for visualization
  pc_vis <- pc_m
  pc_vis[pc_vis > 300] <- 300
  pc_vis[pc_vis < -300] <- -300

  save_base_fig(function() {
    pheatmap::pheatmap(
      pc_vis,
      color = colorRampPalette(c("#2166AC", "#F7F7F7", "#B2182B"))(100),
      breaks = seq(-300, 300, length.out = 101),
      cluster_rows = TRUE,
      cluster_cols = TRUE,
      show_rownames = FALSE,
      fontsize_col = 9,
      main = "PercentChange Heatmap (winsorized at +/-300%)",
      labels_col = short_cond_vec(colnames(pc_vis)),
      border_color = NA,
      fontsize = 10
    )
  }, "fig2a_percentchange_heatmap", width = 9, height = 12)
}

# ── Fig 2b: FoldChange vs PercentChange Scatter ─────────────────────────────
message("  Fig 2b: FC vs PC scatter ...")
{
  fc_vec <- as.vector(fc_m)
  pc_vec <- as.vector(pc_m)
  sig_vec <- as.vector(sig_m)
  scatter_df <- data.frame(
    FoldChange = fc_vec,
    PercentChange = pc_vec,
    Significant = factor(sig_vec, levels = c("0", "1"))
  )
  scatter_df <- scatter_df[complete.cases(scatter_df), ]

  p2b <- ggplot(scatter_df, aes(x = FoldChange, y = PercentChange, color = Significant)) +
    geom_point(alpha = 0.4, size = 1) +
    scale_color_manual(values = c("0" = "#999999", "1" = "#D6604D"),
                       labels = c("Not significant", "Significant")) +
    labs(
      title = "FoldChange vs PercentChange",
      subtitle = "Low-baseline genes show high percentage changes",
      x = "FoldChange", y = "PercentChange (%)", color = ""
    ) +
    theme_bw(base_size = 12) +
    theme(plot.title = element_text(face = "bold", hjust = 0.5),
          plot.subtitle = element_text(hjust = 0.5, face = "italic"))
  save_fig(p2b, "fig2b_fc_vs_pc_scatter", width = 9, height = 7)
}

# ── Fig 2c: PercentChange Box Plot by Condition ─────────────────────────────
message("  Fig 2c: PercentChange box plot ...")
{
  pc_long <- reshape2::melt(pc_m)
  colnames(pc_long) <- c("Gene", "Condition", "PercentChange")
  pc_long$Condition <- factor(pc_long$Condition, levels = cond_names)
  # Remove extreme outliers for visualization
  pc_long <- pc_long[abs(pc_long$PercentChange) < 500, ]

  p2c <- ggplot(pc_long, aes(x = Condition, y = PercentChange, fill = Condition)) +
    geom_boxplot(outlier.size = 0.5, outlier.alpha = 0.3) +
    scale_fill_brewer(palette = "Set2", labels = short_cond_vec(cond_names)) +
    scale_x_discrete(labels = short_cond_vec(cond_names)) +
    labs(
      title = "Distribution of PercentChange Across Perturbation Conditions",
      x = "Perturbation Condition", y = "PercentChange (%)"
    ) +
    theme_bw(base_size = 12) +
    theme(
      plot.title = element_text(face = "bold", hjust = 0.5),
      legend.position = "none"
    )
  save_fig(p2c, "fig2c_percentchange_boxplot", width = 9, height = 6)
}

###############################################################################
# TABLE 3 — Significance Matrix
###############################################################################

message("\n=== Table 3: Significance Matrix ===")

# ── Fig 3a: Significance Binary Heatmap ──────────────────────────────────────
message("  Fig 3a: Significance heatmap ...")
save_base_fig(function() {
  pheatmap::pheatmap(
    sig_m,
    color = c("#F7F7F7", "#D6604D"),
    cluster_rows = TRUE,
    cluster_cols = TRUE,
    show_rownames = FALSE,
    fontsize_col = 9,
    main = "Significance Matrix (1 = significant, p < 0.05)",
    labels_col = short_cond_vec(colnames(sig_m)),
    border_color = NA,
    fontsize = 10,
    legend_breaks = c(0, 1),
    legend_labels = c("Not significant", "Significant")
  )
}, "fig3a_significance_heatmap", width = 9, height = 12)

# ── Fig 3b: FoldChange Heatmap with Significance Overlay ────────────────────
message("  Fig 3b: FC heatmap with significance overlay ...")
save_base_fig(function() {
  # Create annotation for significance
  sig_cells <- which(sig_m == 1, arr.ind = TRUE)

  ComplexHeatmap::Heatmap(
    fc_m,
    name = "FoldChange",
    col = colorRampPalette(c("#2166AC", "#F7F7F7", "#B2182B"))(100),
    cluster_rows = TRUE,
    cluster_columns = TRUE,
    show_row_names = FALSE,
    column_labels = short_cond_vec(colnames(fc_m)),
    column_title = "FoldChange Heatmap with Significance Markers",
    cell_fun = function(j, i, x, y, width, height, fill) {
      if (!is.na(sig_m[i, j]) && sig_m[i, j] == 1) {
        grid::grid.text("*", x, y, gp = grid::gpar(fontsize = 10, fontface = "bold"))
      }
    },
    heatmap_legend_param = list(direction = "horizontal"),
    border = TRUE
  )
}, "fig3b_fc_significance_overlay", width = 9, height = 12)

# ── Fig 3c: Significant Gene Count per Condition ────────────────────────────
message("  Fig 3c: Significant gene count bar chart ...")
{
  sig_counts <- colSums(sig_m, na.rm = TRUE)
  sig_df <- data.frame(
    Condition = names(sig_counts),
    Count = as.numeric(sig_counts)
  )
  sig_df$Condition <- factor(sig_df$Condition, levels = cond_names)

  p3c <- ggplot(sig_df, aes(x = Condition, y = Count, fill = Condition)) +
    geom_bar(stat = "identity", width = 0.6) +
    geom_text(aes(label = Count), vjust = -0.5, size = 4) +
    scale_fill_brewer(palette = "Set2", labels = short_cond_vec(cond_names)) +
    scale_x_discrete(labels = short_cond_vec(cond_names)) +
    labs(
      title = "Number of Significantly Changed Genes per Condition",
      x = "Perturbation Condition", y = "Significant Gene Count"
    ) +
    theme_bw(base_size = 12) +
    theme(
      plot.title = element_text(face = "bold", hjust = 0.5),
      legend.position = "none"
    )
  save_fig(p3c, "fig3c_significant_gene_count", width = 8, height = 6)
}

###############################################################################
# TABLE 4 — ZScore Matrix
###############################################################################

message("\n=== Table 4: ZScore Matrix ===")

# ── Fig 4a: Z-Score Heatmap with Diverging Color ────────────────────────────
message("  Fig 4a: Z-Score heatmap ...")
save_base_fig(function() {
  zmax <- max(abs(range(zs_m, na.rm = TRUE)))
  zmax <- min(zmax, 5)  # cap at 5 for better contrast
  pheatmap::pheatmap(
    zs_m,
    color = colorRampPalette(c("#2166AC", "#F7F7F7", "#B2182B"))(100),
    breaks = seq(-zmax, zmax, length.out = 101),
    cluster_rows = TRUE,
    cluster_cols = TRUE,
    show_rownames = FALSE,
    fontsize_col = 9,
    main = "Z-Score Heatmap (Standardized Effect Size)",
    labels_col = short_cond_vec(colnames(zs_m)),
    border_color = NA,
    fontsize = 10
  )
}, "fig4a_zscore_heatmap", width = 9, height = 12)

# ── Fig 4b: Z-Score Heatmap with Significance Asterisks ─────────────────────
message("  Fig 4b: Z-Score heatmap with significance markers ...")
save_base_fig(function() {
  zmax <- min(max(abs(range(zs_m, na.rm = TRUE))), 5)
  ComplexHeatmap::Heatmap(
    zs_m,
    name = "Z-Score",
    col = colorRampPalette(c("#2166AC", "#F7F7F7", "#B2182B"))(100),
    cluster_rows = TRUE,
    cluster_columns = TRUE,
    show_row_names = FALSE,
    column_labels = short_cond_vec(colnames(zs_m)),
    column_title = "Z-Score Heatmap (* p<0.05, ** p<0.01)",
    cell_fun = function(j, i, x, y, width, height, fill) {
      zval <- zs_m[i, j]
      if (!is.na(zval)) {
        if (abs(zval) > 2.58) {
          grid::grid.text("**", x, y, gp = grid::gpar(fontsize = 8, fontface = "bold", col = "yellow"))
        } else if (abs(zval) > 1.96) {
          grid::grid.text("*", x, y, gp = grid::gpar(fontsize = 10, fontface = "bold", col = "yellow"))
        }
      }
    },
    heatmap_legend_param = list(direction = "horizontal"),
    border = TRUE
  )
}, "fig4b_zscore_significance_heatmap", width = 9, height = 12)

# ── Fig 4c: Z-Score Distribution Histogram ──────────────────────────────────
message("  Fig 4c: Z-Score distribution histogram ...")
{
  z_vec <- as.vector(zs_m)
  z_vec <- z_vec[!is.na(z_vec)]

  p4c <- ggplot(data.frame(ZScore = z_vec), aes(x = ZScore)) +
    geom_histogram(bins = 80, fill = "#4393C3", color = "white", alpha = 0.8) +
    geom_vline(xintercept = c(-1.96, 1.96), linetype = "dashed", color = "#D6604D", linewidth = 0.8) +
    geom_vline(xintercept = c(-2.58, 2.58), linetype = "dotted", color = "#B2182B", linewidth = 0.8) +
    annotate("text", x = 2.1, y = Inf, vjust = 2, hjust = 0,
             label = "|Z|>1.96\np<0.05", size = 3, color = "#D6604D") +
    annotate("text", x = 2.7, y = Inf, vjust = 2, hjust = 0,
             label = "|Z|>2.58\np<0.01", size = 3, color = "#B2182B") +
    labs(
      title = "Distribution of Z-Scores Across All Gene-Condition Pairs",
      x = "Z-Score", y = "Frequency"
    ) +
    theme_bw(base_size = 12) +
    theme(plot.title = element_text(face = "bold", hjust = 0.5))
  save_fig(p4c, "fig4c_zscore_histogram", width = 9, height = 6)
}

###############################################################################
# TABLE 5 — Wildtype Mean Matrix
###############################################################################

message("\n=== Table 5: Wildtype Mean Matrix ===")

# ── Fig 5a: Wildtype Baseline Bar Chart ──────────────────────────────────────
message("  Fig 5a: Wildtype baseline bar chart ...")
{
  # Use mean across conditions (should be very similar)
  wt_mean <- rowMeans(wt_m, na.rm = TRUE)
  wt_df <- data.frame(Gene = names(wt_mean), WildtypeMean = wt_mean)
  wt_df <- wt_df[order(wt_df$WildtypeMean, decreasing = TRUE), ]
  # Show top 30 and bottom 30
  wt_show <- rbind(
    head(wt_df, 30),
    tail(wt_df, 30)
  )
  wt_show$Gene <- factor(wt_show$Gene, levels = rev(wt_show$Gene))

  p5a <- ggplot(wt_show, aes(x = Gene, y = WildtypeMean)) +
    geom_bar(stat = "identity", fill = "#4393C3", width = 0.7) +
    coord_flip() +
    labs(
      title = "Wildtype Baseline Expression (Top & Bottom 30 Genes)",
      x = "Gene", y = "Wildtype Mean Expression"
    ) +
    theme_bw(base_size = 10) +
    theme(
      plot.title = element_text(face = "bold", hjust = 0.5),
      axis.text.y = element_text(size = 7)
    )
  save_fig(p5a, "fig5a_wildtype_baseline_bar", width = 10, height = 9)
}

# ── Fig 5b: Wildtype vs Mutant Paired Bar Chart (top variable genes) ────────
message("  Fig 5b: Wildtype vs Mutant paired bar chart ...")
{
  # Select top 15 genes by variance in fold change
  gene_var <- apply(fc_m, 1, var, na.rm = TRUE)
  top15 <- names(sort(gene_var, decreasing = TRUE))[1:15]

  paired_df <- data.frame(
    Gene = rep(top15, times = 2),
    Mean = c(rowMeans(wt_m[top15, ], na.rm = TRUE),
             rowMeans(mt_m[top15, ], na.rm = TRUE)),
    Type = rep(c("Wildtype", "Mutant"), each = length(top15))
  )
  paired_df$Gene <- factor(paired_df$Gene, levels = rev(top15))

  p5b <- ggplot(paired_df, aes(x = Gene, y = Mean, fill = Type)) +
    geom_bar(stat = "identity", position = "dodge", width = 0.7) +
    scale_fill_manual(values = c("Wildtype" = "#4393C3", "Mutant" = "#D6604D")) +
    coord_flip() +
    labs(
      title = "Wildtype vs Mutant Expression (Top 15 Variable Genes)",
      x = "Gene", y = "Mean Expression", fill = ""
    ) +
    theme_bw(base_size = 11) +
    theme(plot.title = element_text(face = "bold", hjust = 0.5))
  save_fig(p5b, "fig5b_wildtype_mutant_paired_bar", width = 10, height = 7)
}

###############################################################################
# TABLE 6 — Mutant Mean Matrix
###############################################################################

message("\n=== Table 6: Mutant Mean Matrix ===")

# ── Fig 6a: Post-Perturbation Expression Heatmap ────────────────────────────
message("  Fig 6a: Mutant expression heatmap ...")
save_base_fig(function() {
  pheatmap::pheatmap(
    mt_m,
    color = colorRampPalette(c("#2166AC", "#F7F7F7", "#B2182B"))(100),
    cluster_rows = TRUE,
    cluster_cols = TRUE,
    show_rownames = FALSE,
    fontsize_col = 9,
    main = "Post-Perturbation Expression Mean Heatmap",
    labels_col = short_cond_vec(colnames(mt_m)),
    border_color = NA,
    fontsize = 10
  )
}, "fig6a_mutant_heatmap", width = 9, height = 12)

# ── Fig 6b: Wildtype vs Mutant Side-by-Side Heatmap ────────────────────────
message("  Fig 6b: Wildtype vs Mutant paired heatmap ...")
save_base_fig(function() {
  ht1 <- ComplexHeatmap::Heatmap(
    wt_m, name = "Wildtype",
    col = colorRampPalette(c("#2166AC", "#F7F7F7", "#B2182B"))(100),
    cluster_rows = TRUE, cluster_columns = FALSE,
    show_row_names = FALSE,
    column_labels = short_cond_vec(colnames(wt_m)),
    column_title = "Wildtype",
    show_column_names = TRUE,
    border = TRUE
  )
  ht2 <- ComplexHeatmap::Heatmap(
    mt_m, name = "Mutant",
    col = colorRampPalette(c("#2166AC", "#F7F7F7", "#B2182B"))(100),
    cluster_rows = TRUE, cluster_columns = FALSE,
    show_row_names = FALSE,
    column_labels = short_cond_vec(colnames(mt_m)),
    column_title = "Mutant",
    show_column_names = TRUE,
    border = TRUE
  )
  ComplexHeatmap::draw(ht1 + ht2,
                       column_title = "Wildtype vs Mutant Expression Comparison")
}, "fig6b_wildtype_mutant_paired_heatmap", width = 12, height = 12)

###############################################################################
# TABLE 7 — Comprehensive Comparison Table
###############################################################################

message("\n=== Table 7: Comprehensive Comparison ===")

# ── Fig 7a: Volcano Plot ────────────────────────────────────────────────────
message("  Fig 7a: Volcano plot ...")
{
  comp_tab$NegLogP <- ifelse(comp_tab$Significant == 1,
                              -log10(0.05),
                              -log10(0.5))  # approximate
  # Use |ZScore| as y-axis proxy for -log10(p)
  comp_tab$AbsZ <- abs(as.numeric(comp_tab$ZScore))
  comp_tab$SigLabel <- ifelse(comp_tab$Significant == 1, "Significant", "Not significant")

  p7a <- ggplot(comp_tab, aes(x = as.numeric(FoldChange), y = AbsZ, color = SigLabel)) +
    geom_point(alpha = 0.4, size = 1) +
    scale_color_manual(values = c("Not significant" = "#999999", "Significant" = "#D6604D")) +
    geom_hline(yintercept = 1.96, linetype = "dashed", color = "#4393C3", linewidth = 0.6) +
    geom_vline(xintercept = 0, linetype = "dotted", color = "gray50", linewidth = 0.4) +
    ggrepel::geom_text_repel(
      data = subset(comp_tab, Significant == 1),
      aes(label = Gene),
      size = 3, max.overlaps = 20, box.padding = 0.5
    ) +
    labs(
      title = "Volcano Plot: FoldChange vs |Z-Score|",
      x = "FoldChange", y = "|Z-Score|", color = ""
    ) +
    theme_bw(base_size = 12) +
    theme(plot.title = element_text(face = "bold", hjust = 0.5))
  save_fig(p7a, "fig7a_volcano_plot", width = 10, height = 8)
}

# ── Fig 7b: Multi-Condition Volcano (faceted) ───────────────────────────────
message("  Fig 7b: Faceted volcano plot ...")
{
  comp_tab$FC_num <- as.numeric(comp_tab$FoldChange)
  comp_tab$Z_num  <- as.numeric(comp_tab$ZScore)

  p7b <- ggplot(comp_tab, aes(x = FC_num, y = abs(Z_num), color = SigLabel)) +
    geom_point(alpha = 0.4, size = 0.8) +
    scale_color_manual(values = c("Not significant" = "#999999", "Significant" = "#D6604D")) +
    geom_hline(yintercept = 1.96, linetype = "dashed", color = "#4393C3", linewidth = 0.5) +
    facet_wrap(~ Condition, ncol = 3, labeller = labeller(Condition = short_cond_vec)) +
    labs(
      title = "Volcano Plot by Perturbation Condition",
      x = "FoldChange", y = "|Z-Score|", color = ""
    ) +
    theme_bw(base_size = 10) +
    theme(
      plot.title = element_text(face = "bold", hjust = 0.5),
      strip.text = element_text(face = "bold"),
      legend.position = "bottom"
    )
  save_fig(p7b, "fig7b_faceted_volcano", width = 12, height = 8)
}

# ── Fig 7c: Parallel Coordinates Plot ───────────────────────────────────────
message("  Fig 7c: Parallel coordinates plot ...")
{
  # Select top 30 variable genes for clarity
  gene_var <- apply(fc_m, 1, var, na.rm = TRUE)
  top30 <- names(sort(gene_var, decreasing = TRUE))[1:30]

  par_df <- data.frame(Gene = rownames(fc_m)[rownames(fc_m) %in% top30])
  for (cn in cond_names) {
    par_df[[cn]] <- fc_m[par_df$Gene, cn]
  }
  par_long <- reshape2::melt(par_df, id.vars = "Gene",
                              variable.name = "Condition", value.name = "FoldChange")
  par_long$Condition <- factor(par_long$Condition, levels = cond_names)

  p7c <- ggplot(par_long, aes(x = Condition, y = FoldChange, group = Gene, color = Gene)) +
    geom_line(alpha = 0.6, linewidth = 0.6) +
    geom_point(size = 1.5, alpha = 0.7) +
    scale_x_discrete(labels = short_cond_vec(cond_names)) +
    scale_color_viridis_d(option = "H", begin = 0.1, end = 0.9) +
    labs(
      title = "Parallel Coordinates: FoldChange Profiles of Top 30 Variable Genes",
      x = "Perturbation Condition", y = "FoldChange", color = "Gene"
    ) +
    theme_bw(base_size = 11) +
    theme(
      plot.title = element_text(face = "bold", hjust = 0.5),
      legend.position = "right",
      legend.text = element_text(size = 7)
    )
  save_fig(p7c, "fig7c_parallel_coordinates", width = 11, height = 7)
}

###############################################################################
# TABLE 8 — Condition Similarity Matrix
###############################################################################

message("\n=== Table 8: Condition Similarity ===")

# ── Fig 8a: Correlation Heatmap ──────────────────────────────────────────────
message("  Fig 8a: Correlation heatmap ...")
{
  sim_m <- as.matrix(cond_sim[, -1])
  rownames(sim_m) <- cond_sim[[1]]
  storage.mode(sim_m) <- "numeric"

  save_base_fig(function() {
    pheatmap::pheatmap(
      sim_m,
      color = colorRampPalette(c("#2166AC", "#F7F7F7", "#B2182B"))(100),
      breaks = seq(-1, 1, length.out = 101),
      cluster_rows = TRUE,
      cluster_cols = TRUE,
      display_numbers = TRUE,
      number_format = "%.2f",
      fontsize_number = 8,
      fontsize_col = 9,
      fontsize_row = 9,
      main = "Condition Similarity (Pearson Correlation of FoldChange Profiles)",
      labels_col = short_cond_vec(rownames(sim_m)),
      labels_row = short_cond_vec(rownames(sim_m)),
      border_color = "white",
      fontsize = 10
    )
  }, "fig8a_condition_similarity_heatmap", width = 8, height = 7)
}

# ── Fig 8b: Hierarchical Clustering Dendrogram ──────────────────────────────
message("  Fig 8b: Dendrogram ...")
{
  dist_mat <- dist(sim_m)
  hc <- hclust(dist_mat, method = "complete")

  p8b <- ggdendro::ggdendrogram(hc, rotate = FALSE, size = 4) +
    labs(
      title = "Hierarchical Clustering Dendrogram of Perturbation Conditions",
      x = "Perturbation Condition", y = "Euclidean Distance"
    ) +
    theme(plot.title = element_text(face = "bold", hjust = 0.5))
  save_fig(p8b, "fig8b_condition_dendrogram", width = 8, height = 6)
}

# ── Fig 8c: Network Graph ───────────────────────────────────────────────────
message("  Fig 8c: Network graph ...")
{
  if (requireNamespace("igraph", quietly = TRUE)) {
    # Build adjacency from correlation (threshold > 0.1)
    adj <- sim_m
    adj[abs(adj) < 0.1] <- 0
    diag(adj) <- 0
    g <- igraph::graph_from_adjacency_matrix(adj, mode = "undirected",
                                              weighted = TRUE, diag = FALSE)
    lay <- igraph::layout_with_fr(g, weights = abs(igraph::E(g)$weight))
    edge_w <- abs(igraph::E(g)$weight)
    edge_c <- ifelse(igraph::E(g)$weight > 0, "#4393C3", "#D6604D")

    save_base_fig(function() {
      par(mar = c(1, 1, 3, 1))
      plot(g, layout = lay,
           vertex.label = short_cond_vec(igraph::V(g)$name),
           vertex.size = 25, vertex.color = "#FEE08B",
           vertex.label.cex = 0.9, vertex.label.color = "black",
           edge.width = edge_w * 5, edge.color = edge_c,
           main = "Condition Similarity Network\n(Blue=positive, Red=negative correlation)")
    }, "fig8c_condition_network", width = 8, height = 7)
  } else {
    message("  Skipping network graph (igraph not available)")
  }
}

###############################################################################
# TABLE 9 — Gene Sensitivity Matrix
###############################################################################

message("\n=== Table 9: Gene Sensitivity ===")

# ── Fig 9a: Sensitivity Score Ranked Bar Chart ──────────────────────────────
message("  Fig 9a: Sensitivity score bar chart ...")
{
  sens_df <- gene_sens[order(as.numeric(gene_sens$SensitivityScore), decreasing = TRUE), ]
  # Show top 30
  top30_sens <- head(sens_df, 30)
  top30_sens$Gene <- factor(top30_sens$Gene, levels = rev(top30_sens$Gene))

  p9a <- ggplot(top30_sens, aes(x = Gene, y = as.numeric(SensitivityScore))) +
    geom_bar(stat = "identity", fill = "#4393C3", width = 0.7) +
    coord_flip() +
    labs(
      title = "Top 30 Most Sensitive Genes (Ranked by SensitivityScore)",
      x = "Gene", y = "Sensitivity Score (Mean |FoldChange|)"
    ) +
    theme_bw(base_size = 11) +
    theme(plot.title = element_text(face = "bold", hjust = 0.5))
  save_fig(p9a, "fig9a_sensitivity_score_bar", width = 10, height = 8)
}

# ── Fig 9b: SensitivityScore vs AffectedCount Scatter ───────────────────────
message("  Fig 9b: Sensitivity vs AffectedCount scatter ...")
{
  sens_scatter <- data.frame(
    SensitivityScore = as.numeric(gene_sens$SensitivityScore),
    AffectedCount    = as.numeric(gene_sens$AffectedCount),
    Gene             = gene_sens$Gene
  )
  # Label top genes
  top_genes <- head(sens_scatter[order(sens_scatter$SensitivityScore, decreasing = TRUE), ], 10)

  p9b <- ggplot(sens_scatter, aes(x = AffectedCount, y = SensitivityScore)) +
    geom_point(alpha = 0.5, color = "#4393C3", size = 1.5) +
    ggrepel::geom_text_repel(
      data = top_genes, aes(label = Gene),
      size = 3, max.overlaps = 15, box.padding = 0.4
    ) +
    labs(
      title = "Gene Sensitivity Score vs Number of Affected Conditions",
      x = "Affected Condition Count", y = "Sensitivity Score"
    ) +
    theme_bw(base_size = 12) +
    theme(plot.title = element_text(face = "bold", hjust = 0.5))
  save_fig(p9b, "fig9b_sensitivity_vs_affected_scatter", width = 9, height = 7)
}

# ── Fig 9c: Radar Chart for Top Sensitive Genes ─────────────────────────────
message("  Fig 9c: Radar chart ...")
{
  if (requireNamespace("ggradar", quietly = TRUE)) {
    # Select top 5 sensitive genes
    top5_sens <- head(sens_df, 5)
    radar_data <- top5_sens[, c("Gene", cond_names)]
    for (cn in cond_names) {
      radar_data[[cn]] <- as.numeric(radar_data[[cn]])
    }

    p9c <- ggradar::ggradar(radar_data, group.colour = NULL,
                             grid.label.size = 4, axis.label.size = 3,
                             plot.title = "Radar Chart: Top 5 Sensitive Genes")
    save_fig(p9c, "fig9c_sensitivity_radar", width = 9, height = 9)
  } else {
    # Fallback: use fmsb radar chart
    if (requireNamespace("fmsb", quietly = TRUE)) {
      top5_sens <- head(sens_df, 5)
      radar_mat <- as.matrix(top5_sens[, cond_names])
      storage.mode(radar_mat) <- "numeric"
      rownames(radar_mat) <- top5_sens$Gene
      # Normalize to 0-1
      radar_norm <- apply(radar_mat, 2, function(x) (x - min(x)) / (max(x) - min(x) + 1e-10))
      radar_norm <- rbind(rep(1, ncol(radar_norm)), rep(0, ncol(radar_norm)), radar_norm)
      colnames(radar_norm) <- short_cond_vec(cond_names)

      save_base_fig(function() {
        par(mar = c(1, 1, 3, 1))
        fmsb::radarchart(radar_norm,
                         title = "Radar Chart: Top 5 Sensitive Genes",
                         pcol = rainbow(5), plwd = 2,
                         pfcol = scales::alpha(rainbow(5), 0.1))
        legend("topright", legend = top5_sens$Gene,
               col = rainbow(5), lwd = 2, cex = 0.7)
      }, "fig9c_sensitivity_radar", width = 8, height = 8)
    } else {
      # Final fallback: grouped bar chart
      top5_sens <- head(sens_df, 5)
      bar_df <- data.frame()
      for (i in 1:nrow(top5_sens)) {
        for (cn in cond_names) {
          bar_df <- rbind(bar_df, data.frame(
            Gene = top5_sens$Gene[i],
            Condition = cn,
            FoldChange = as.numeric(top5_sens[i, cn])
          ))
        }
      }
      bar_df$Condition <- factor(bar_df$Condition, levels = cond_names)
      bar_df$Gene <- factor(bar_df$Gene, levels = top5_sens$Gene)

      p9c <- ggplot(bar_df, aes(x = Condition, y = FoldChange, fill = Gene)) +
        geom_bar(stat = "identity", position = "dodge", width = 0.7) +
        scale_fill_brewer(palette = "Set1") +
        scale_x_discrete(labels = short_cond_vec(cond_names)) +
        labs(
          title = "FoldChange Profile of Top 5 Sensitive Genes",
          x = "Perturbation Condition", y = "|FoldChange|", fill = "Gene"
        ) +
        theme_bw(base_size = 11) +
        theme(plot.title = element_text(face = "bold", hjust = 0.5))
      save_fig(p9c, "fig9c_sensitivity_radar", width = 10, height = 7)
    }
  }
}

###############################################################################
# TABLE 10 — Intervention Ranking Table
###############################################################################

message("\n=== Table 10: Intervention Ranking ===")

# ── Fig 10a: Top-10 Gene Lollipop Chart per Condition ───────────────────────
message("  Fig 10a: Top-10 lollipop chart ...")
{
  int_rank$FoldChange <- as.numeric(int_rank$FoldChange)
  int_rank$Rank <- as.numeric(int_rank$Rank)
  int_rank$Gene <- as.character(int_rank$Gene)

  # Top 10 per condition
  top10_rank <- do.call(rbind, lapply(split(int_rank, int_rank$Condition), function(d) {
    d <- d[order(d$Rank), ]
    head(d, 10)
  }))
  top10_rank$Condition <- factor(top10_rank$Condition, levels = cond_names)

  p10a <- ggplot(top10_rank, aes(x = reorder(Gene, -FoldChange), y = FoldChange)) +
    geom_segment(aes(x = reorder(Gene, -FoldChange), xend = reorder(Gene, -FoldChange),
                     y = 0, yend = FoldChange), color = "gray60") +
    geom_point(aes(color = FoldChange > 0), size = 3) +
    scale_color_manual(values = c("TRUE" = "#B2182B", "FALSE" = "#2166AC"),
                       labels = c("Down-regulated", "Up-regulated"),
                       name = "Direction") +
    coord_flip() +
    facet_wrap(~ Condition, ncol = 3, scales = "free_x",
               labeller = labeller(Condition = short_cond_vec)) +
    labs(
      title = "Top 10 Affected Genes per Perturbation Condition",
      x = "Gene", y = "FoldChange"
    ) +
    theme_bw(base_size = 10) +
    theme(
      plot.title = element_text(face = "bold", hjust = 0.5),
      strip.text = element_text(face = "bold", size = 9),
      axis.text.y = element_text(size = 7)
    )
  save_fig(p10a, "fig10a_top10_lollipop", width = 14, height = 10)
}

# ── Fig 10b: UpSet Plot of Shared Genes ─────────────────────────────────────
message("  Fig 10b: UpSet plot ...")
{
  if (requireNamespace("ComplexHeatmap", quietly = TRUE)) {
    # Build a list of top-50 gene sets per condition
    gene_sets <- lapply(split(int_rank, int_rank$Condition), function(d) {
      d$Gene[order(d$Rank)][1:min(50, nrow(d))]
    })

    save_base_fig(function() {
      m <- ComplexHeatmap::make_comb_mat(gene_sets)
      ComplexHeatmap::UpSet(m,
                            set_names = short_cond_vec(names(gene_sets)),
                            column_title = "Shared Top-50 Affected Genes Across Conditions",
                            column_title_gp = grid::gpar(fontsize = 13, fontface = "bold"))
    }, "fig10b_upset_shared_genes", width = 10, height = 7)
  } else {
    message("  Skipping UpSet plot (ComplexHeatmap not available)")
  }
}

# ── Fig 10c: Bubble Chart ───────────────────────────────────────────────────
message("  Fig 10c: Bubble chart ...")
{
  top20_rank <- do.call(rbind, lapply(split(int_rank, int_rank$Condition), function(d) {
    d <- d[order(d$Rank), ]
    head(d, 20)
  }))
  top20_rank$Condition <- factor(top20_rank$Condition, levels = cond_names)
  top20_rank$AbsFC <- abs(as.numeric(top20_rank$FoldChange))

  p10c <- ggplot(top20_rank, aes(x = Condition, y = Rank, size = AbsFC, color = FoldChange)) +
    geom_point(alpha = 0.7) +
    scale_size_continuous(range = c(1, 10), name = "|FoldChange|") +
    scale_color_gradient2(low = "#2166AC", mid = "#F7F7F7", high = "#B2182B",
                          midpoint = 0, name = "FoldChange") +
    scale_x_discrete(labels = short_cond_vec(cond_names)) +
    scale_y_reverse() +
    labs(
      title = "Bubble Chart: Top 20 Affected Genes by Condition",
      x = "Perturbation Condition", y = "Rank (by |FoldChange|)"
    ) +
    theme_bw(base_size = 12) +
    theme(
      plot.title = element_text(face = "bold", hjust = 0.5),
      legend.position = "right"
    )
  save_fig(p10c, "fig10c_bubble_chart", width = 10, height = 7)
}

###############################################################################
# TABLE 11 — Pathway Summary Matrix
###############################################################################

message("\n=== Table 11: Pathway Summary ===")

# ── Parse pathway_summary (values are "meanFC(sigCount)" format) ─────────────
parse_pathway_value <- function(val, type = c("meanFC", "sigCount")) {
  type <- match.arg(type)
  # Format: "0.2044(3)" or "0.0112(0)"
  val <- as.character(val)
  meanFC <- as.numeric(sub("\\(.*", "", val))
  sigCount <- as.numeric(sub(".*\\(", "", sub("\\)", "", val)))
  if (type == "meanFC") return(meanFC) else return(sigCount)
}

# ── Fig 11a: Pathway x Condition Heatmap ────────────────────────────────────
message("  Fig 11a: Pathway heatmap ...")
{
  pw_cond_cols <- setdiff(names(path_sum), c("PathwayID", "PathwayName", "N_Genes"))
  pw_meanFC_mat <- sapply(pw_cond_cols, function(cn) {
    sapply(path_sum[[cn]], parse_pathway_value, type = "meanFC")
  })
  rownames(pw_meanFC_mat) <- path_sum$PathwayName

  save_base_fig(function() {
    pheatmap::pheatmap(
      pw_meanFC_mat,
      color = colorRampPalette(c("#F7F7F7", "#FEE08B", "#D73027"))(100),
      cluster_rows = TRUE,
      cluster_cols = TRUE,
      display_numbers = TRUE,
      number_format = "%.3f",
      fontsize_number = 7,
      fontsize_row = 9,
      fontsize_col = 8,
      main = "Pathway-Level Mean |FoldChange| by Perturbation Condition",
      labels_col = short_cond_vec(colnames(pw_meanFC_mat)),
      border_color = "white",
      fontsize = 10
    )
  }, "fig11a_pathway_heatmap", width = 10, height = 7)
}

# ── Fig 11b: Pathway Bubble Chart ───────────────────────────────────────────
message("  Fig 11b: Pathway bubble chart ...")
{
  pw_sigRatio_mat <- sapply(pw_cond_cols, function(cn) {
    sapply(path_sum[[cn]], function(v) {
      sigC <- parse_pathway_value(v, "sigCount")
      nG   <- as.numeric(path_sum$N_Genes[which(path_sum[[cn]] == v)[1]])
      if (is.na(nG) || nG == 0) 0 else sigC / nG
    })
  })
  rownames(pw_sigRatio_mat) <- path_sum$PathwayName

  pw_long <- reshape2::melt(pw_meanFC_mat)
  colnames(pw_long) <- c("Pathway", "Condition", "MeanFC")
  pw_long$SigRatio <- as.vector(pw_sigRatio_mat)
  pw_long$Condition <- factor(pw_long$Condition, levels = cond_names)

  p11b <- ggplot(pw_long, aes(x = Condition, y = Pathway,
                                size = SigRatio, color = MeanFC)) +
    geom_point(alpha = 0.8) +
    scale_size_continuous(range = c(2, 15), name = "Significant\nGene Ratio") +
    scale_color_gradient2(low = "#2166AC", mid = "#F7F7F7", high = "#B2182B",
                          midpoint = median(pw_long$MeanFC, na.rm = TRUE),
                          name = "Mean |FC|") +
    scale_x_discrete(labels = short_cond_vec(cond_names)) +
    labs(
      title = "Pathway Perturbation Bubble Chart",
      x = "Perturbation Condition", y = ""
    ) +
    theme_bw(base_size = 11) +
    theme(
      plot.title = element_text(face = "bold", hjust = 0.5),
      axis.text.y = element_text(size = 8)
    )
  save_fig(p11b, "fig11b_pathway_bubble", width = 11, height = 7)
}

# ── Fig 11c: Pathway Perturbation Network ───────────────────────────────────
message("  Fig 11c: Pathway network ...")
{
  if (requireNamespace("igraph", quietly = TRUE)) {
    # Build bipartite network: conditions <-> pathways (edge if sigRatio > 0)
    pw_sig_long <- reshape2::melt(pw_sigRatio_mat)
    colnames(pw_sig_long) <- c("Pathway", "Condition", "SigRatio")
    pw_sig_long <- pw_sig_long[pw_sig_long$SigRatio > 0, ]

    if (nrow(pw_sig_long) > 0) {
      g <- igraph::graph_from_data_frame(pw_sig_long, directed = FALSE)
      V(g)$type <- ifelse(V(g)$name %in% cond_names, "Condition", "Pathway")
      V(g)$color <- ifelse(V(g)$type == "Condition", "#FEE08B", "#A6D96A")
      E(g)$width <- E(g)$SigRatio * 10

      save_base_fig(function() {
        par(mar = c(1, 1, 3, 1))
        lay <- igraph::layout_with_kk(g)
        plot(g, layout = lay,
             vertex.size = 18, vertex.label.cex = 0.7,
             vertex.label.color = "black",
             edge.color = "gray60",
             main = "Pathway Perturbation Network\n(Edges: significant gene ratio > 0)")
        legend("bottomright",
               legend = c("Condition", "Pathway"),
               fill = c("#FEE08B", "#A6D96A"), cex = 0.8)
      }, "fig11c_pathway_network", width = 10, height = 8)
    } else {
      message("  No significant pathway-condition pairs for network graph")
    }
  } else {
    message("  Skipping pathway network (igraph not available)")
  }
}

###############################################################################
# TABLE 12 — Cross-Impact Matrix
###############################################################################

message("\n=== Table 12: Cross-Impact Matrix ===")

# ── Fig 12a: Cross-Impact Heatmap ───────────────────────────────────────────
message("  Fig 12a: Cross-impact heatmap ...")
{
  ci_m <- as.matrix(cross_imp[, -1])
  rownames(ci_m) <- cross_imp[[1]]
  storage.mode(ci_m) <- "numeric"
  # Shorten pathway column names
  pw_short <- gsub("pathway_", "", colnames(ci_m))
  pw_short <- gsub("_", " ", tools::toTitleCase(pw_short))

  save_base_fig(function() {
    pheatmap::pheatmap(
      ci_m,
      color = colorRampPalette(c("#FFFFCC", "#FD8D3C", "#800026"))(100),
      breaks = seq(0, max(ci_m, na.rm = TRUE), length.out = 101),
      cluster_rows = TRUE,
      cluster_cols = TRUE,
      display_numbers = TRUE,
      number_format = "%.2f",
      fontsize_number = 7,
      fontsize_row = 9,
      fontsize_col = 8,
      main = "Cross-Impact Matrix: Perturbation Condition x Pathway\n(Fraction of Significantly Affected Genes)",
      labels_col = pw_short,
      labels_row = short_cond_vec(rownames(ci_m)),
      border_color = "white",
      fontsize = 10
    )
  }, "fig12a_cross_impact_heatmap", width = 11, height = 7)
}

# ── Fig 12b: Biclustering Heatmap ───────────────────────────────────────────
message("  Fig 12b: Biclustering heatmap ...")
save_base_fig(function() {
  ComplexHeatmap::Heatmap(
    ci_m,
    name = "Impact",
    col = colorRampPalette(c("#FFFFCC", "#FD8D3C", "#800026"))(100),
    cluster_rows = TRUE,
    cluster_columns = TRUE,
    row_labels = short_cond_vec(rownames(ci_m)),
    column_labels = pw_short,
    column_title = "Biclustering Heatmap: Condition x Pathway Cross-Impact",
    heatmap_legend_param = list(direction = "horizontal"),
    border = TRUE,
    cell_fun = function(j, i, x, y, width, height, fill) {
      val <- ci_m[i, j]
      if (!is.na(val) && val > 0) {
        grid::grid.text(sprintf("%.2f", val), x, y,
                        gp = grid::gpar(fontsize = 7, col = "black"))
      }
    }
  )
}, "fig12b_cross_impact_bicluster", width = 11, height = 7)

# ── Fig 12c: Parallel Coordinates for Cross-Impact ──────────────────────────
message("  Fig 12c: Parallel coordinates plot ...")
{
  ci_df <- as.data.frame(ci_m)
  ci_df$Condition <- rownames(ci_m)
  ci_long <- reshape2::melt(ci_df, id.vars = "Condition",
                             variable.name = "Pathway", value.name = "Impact")
  ci_long$Pathway <- factor(ci_long$Pathway, levels = colnames(ci_m))
  ci_long$Condition <- factor(ci_long$Condition, levels = rownames(ci_m))

  p12c <- ggplot(ci_long, aes(x = Pathway, y = Impact, group = Condition, color = Condition)) +
    geom_line(linewidth = 1, alpha = 0.8) +
    geom_point(size = 2.5) +
    scale_color_brewer(palette = "Set2", labels = short_cond_vec(rownames(ci_m))) +
    scale_x_discrete(labels = pw_short) +
    labs(
      title = "Parallel Coordinates: Perturbation Impact Across Pathways",
      x = "Pathway", y = "Impact (Fraction of Significant Genes)", color = "Condition"
    ) +
    theme_bw(base_size = 11) +
    theme(
      plot.title = element_text(face = "bold", hjust = 0.5),
      axis.text.x = element_text(angle = 30, hjust = 1, size = 9),
      legend.position = "right"
    )
  save_fig(p12c, "fig12c_cross_impact_parallel", width = 12, height = 7)
}

###############################################################################
# Summary
###############################################################################
message("\n========================================")
message("All figures have been generated!")
message(sprintf("Output directory: %s", normalizePath(OUT_DIR, mustWork = FALSE)))
message("========================================")
