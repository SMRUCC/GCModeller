# =============================================================================
# Figure 1 -- multi-omics input landscape and KEGG pathway coverage
# =============================================================================

source(file.path(Sys.getenv("MOGSVA_SCRIPT_DIR",
                           "G:/GCModeller/test/demo/HTS/GSVA/mogsva_figures"),
                 "00_common.R"))

d <- load_mogsva()
cov <- copy(d$coverage)
cov[, omics := factor(omics, levels = OMICS_LEVELS)]
cov[, measured := member_count > 0]

## ---- A: features per layer -------------------------------------------------
fc <- data.table(
  omics = factor(OMICS_LEVELS, levels = rev(OMICS_LEVELS)),
  n     = as.integer(d$feature_count[OMICS_LEVELS])
)

pA <- ggplot(fc, aes(y = omics)) +
  geom_segment(aes(x = 60, xend = n, yend = omics, colour = omics),
               linewidth = 1.1, lineend = "round") +
  geom_point(aes(x = n, colour = omics), size = 1.6) +
  geom_text(aes(x = n, label = comma(n)), hjust = -0.3, size = 2.3, colour = "grey15") +
  scale_colour_manual(values = PAL_OMICS, guide = "none") +
  scale_x_continuous(trans = "log10", labels = comma,
                     limits = c(60, 9e4), expand = expansion(mult = c(0, 0.02))) +
  labs(x = "Molecular features (log scale)", y = NULL,
       title = "Assayed features per layer",
       subtitle = sprintf("%d primary tumours, four omics layers", d$n_sample)) +
  theme(panel.grid.major.y = element_blank(),
        axis.text.y = element_text(size = 6.4))

## ---- B: pathway members per layer -----------------------------------------
meas_n <- cov[measured == TRUE, .(n_measured = uniqueN(pathway)), by = omics]
cov_lab <- merge(cov[measured == TRUE], meas_n, by = "omics")

pB <- ggplot(cov_lab, aes(omics, member_count, fill = omics)) +
  geom_violin(scale = "width", colour = NA, alpha = 0.75, trim = TRUE) +
  geom_boxplot(width = 0.13, outlier.shape = NA, fill = "white",
               colour = "grey20", linewidth = 0.3) +
  scale_fill_manual(values = PAL_OMICS, guide = "none") +
  scale_y_continuous(trans = "log1p",
                     breaks = c(1, 5, 10, 50, 100, 200),
                     labels = c("1", "5", "10", "50", "100", "200"),
                     limits = c(0.8, 700)) +
  scale_x_discrete(labels = function(x) {
    sapply(x, function(k) {
      n <- meas_n[omics == k, n_measured]
      paste0(OMICS_LABEL[[k]], "\n", n, "/", d$n_pathway)
    })
  }) +
  labs(x = NULL, y = "Pathway members measured (log scale)",
       title = "Pathway coverage is layer dependent",
       subtitle = "Numbers below labels: pathways with >= 1 measured member") +
  theme(panel.grid.major.x = element_blank())

## ---- C: how many layers support a pathway ---------------------------------
lay <- cov[, .(n_layer = sum(measured)), by = pathway]
lay_tab <- lay[, .N, by = n_layer][order(n_layer)]
lay_tab[, frac := N / sum(N)]

pC <- ggplot(lay_tab, aes(factor(n_layer), N)) +
  geom_col(width = 0.62, fill = "grey55", colour = "grey20", linewidth = 0.25) +
  geom_text(aes(label = sprintf("%d\n(%.1f%%)", N, 100 * frac)),
            vjust = -0.25, size = 2.3, lineheight = 0.85, colour = "grey15") +
  scale_y_continuous(expand = expansion(mult = c(0, 0.22))) +
  labs(x = "Omics layers contributing", y = "KEGG pathways",
       title = "Multi-layer support of gene sets") +
  theme(panel.grid.major.x = element_blank())

## ---- D: fusion weights -----------------------------------------------------
# pathways supported by a single layer carry a trivial weight of 1 and are
# excluded here so that the weighting rule itself is visible
multi_layer <- cov[, .(n_layer = sum(measured)), by = pathway][n_layer >= 2, pathway]
w <- d$weights[skipped == FALSE & pathway %in% multi_layer]
w[, omics := factor(omics, levels = OMICS_LEVELS)]
wn <- w[, .N, by = omics]

pD <- ggplot(w, aes(omics, weight, fill = omics)) +
  geom_boxplot(width = 0.55, outlier.size = 0.25, outlier.alpha = 0.35,
               colour = "grey20", linewidth = 0.3) +
  stat_summary(fun = mean, geom = "point", shape = 23, size = 0.9,
               fill = "white", colour = "grey10") +
  scale_fill_manual(values = PAL_OMICS, guide = "none") +
  scale_x_discrete(labels = function(x) paste0(OMICS_LABEL[x], "\n(", wn$N[match(x, wn$omics)], ")")) +
  scale_y_continuous(limits = c(0, 0.62), expand = expansion(mult = c(0.01, 0.06))) +
  labs(x = NULL, y = "Fusion weight",
       title = "Feature-balanced omics weights",
       subtitle = "Balanced weighting, normalised per pathway; diamond = mean") +
  theme(panel.grid.major.x = element_blank())

## ---- assemble --------------------------------------------------------------
fig1 <- (pA | pB) / (pC | pD) +
  plot_layout(heights = c(1, 1), widths = c(1, 1.08)) +
  plot_annotation(tag_levels = "A") &
  theme(plot.tag = element_text(size = 10, face = "bold"))

save_fig(fig1, "Figure1_input_landscape", width_mm = 183, height_mm = 128)
