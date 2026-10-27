# =============================================================================
# Figure 5 -- ablation against a naive pooled single-omics GSVA
#   A  fused score versus pooled baseline, all pathway x tumour observations
#   B  per-pathway agreement between the two scores
#   C  how well each score recovers the individual omics layers
#   D  the best and the worst agreeing pathway
# =============================================================================

source(file.path(Sys.getenv("MOGSVA_SCRIPT_DIR",
                           "G:/GCModeller/test/demo/HTS/GSVA/mogsva_figures"),
                 "00_common.R"))
suppressPackageStartupMessages(library(matrixStats))

d <- load_mogsva()
S <- d$scores
P <- d$pooled[intersect(rownames(S), rownames(d$pooled)), , drop = FALSE]
common <- rownames(P)[stats::complete.cases(P)]
S2 <- S[common, , drop = FALSE]
P2 <- P[common, , drop = FALSE]

r_all <- stats::cor(as.vector(S2), as.vector(P2))
r_path <- vapply(common, function(p) stats::cor(S2[p, ], P2[p, ]), numeric(1))
med_r <- median(r_path)

## ---- A: fused vs pooled ----------------------------------------------------
obs <- data.table(fused = as.vector(S2), pooled = as.vector(P2))

pA <- ggplot(obs, aes(pooled, fused)) +
  geom_bin2d(bins = 90, colour = NA) +
  geom_abline(slope = 1, intercept = 0, linetype = 2, linewidth = 0.35, colour = "grey30") +
  geom_smooth(method = "lm", formula = y ~ x, se = FALSE, linewidth = 0.4,
              colour = "#A6362A") +
  scale_fill_gradient(low = "#EDF3F7", high = "#1F4E79", trans = "log10",
                      breaks = c(1, 10, 100, 1000), labels = comma,
                      name = "Observations") +
  annotate("text", x = -0.62, y = 0.66, hjust = 0, vjust = 1, size = 2.4,
           colour = "grey15", lineheight = 0.95,
           label = sprintf("Pearson r = %.2f\n%s pathways x %d tumours",
                           r_all, length(common), ncol(S2))) +
  labs(x = "Pooled single-omics GSVA score", y = "moGSVA fused score (ES_multi)",
       title = "Fusion and naive pooling correlate only moderately",
       subtitle = "Two-dimensional binning of all observations") +
  coord_fixed()

## ---- B: per-pathway correlation --------------------------------------------
rd <- data.table(pathway = names(r_path), r = as.numeric(r_path))

pB <- ggplot(rd, aes(r)) +
  geom_histogram(binwidth = 0.02, fill = "#8FA9C4", colour = "white", linewidth = 0.25) +
  geom_vline(xintercept = med_r, linetype = 2, linewidth = 0.35, colour = "#A6362A") +
  annotate("text", x = med_r + 0.015, y = Inf, vjust = 1.8, hjust = 0, size = 2.3,
           colour = "#A6362A", label = sprintf("median r = %.2f", med_r)) +
  scale_x_continuous(breaks = seq(0, 1, 0.25)) +
  coord_cartesian(xlim = c(-0.02, 1.02)) +
  scale_y_continuous(expand = expansion(mult = c(0, 0.12))) +
  labs(x = "Per-pathway Pearson r (fused vs pooled)", y = "KEGG pathways",
       title = "Disagreement is pathway specific",
       subtitle = sprintf("%d pathways scored by both methods", length(common))) +
  theme(panel.grid.major.x = element_line(linewidth = 0.2, colour = "grey93"))

## ---- C: which layers each score recovers -----------------------------------
ls <- d$layer_scores[pathway %in% common]
layer_r <- rbindlist(lapply(OMICS_LEVELS, function(om) {
  wide <- dcast(ls[omics == om], pathway ~ sample, value.var = "score")
  m <- as.matrix(wide[, -1]); rownames(m) <- wide$pathway
  pw <- intersect(rownames(m), common)
  data.table(
    omics = om,
    pathway = pw,
    fused  = vapply(pw, function(p) stats::cor(m[p, ], S2[p, ]), numeric(1)),
    pooled = vapply(pw, function(p) stats::cor(m[p, ], P2[p, ]), numeric(1))
  )
}))
long_r <- melt(layer_r, id.vars = c("omics", "pathway"),
               variable.name = "method", value.name = "r")
long_r[, method := factor(method, levels = c("pooled", "fused"),
                          labels = c("Pooled baseline", "moGSVA fused"))]
long_r[, omics := factor(omics, levels = OMICS_LEVELS)]
n_obs <- long_r[, .(n = uniqueN(pathway)), by = omics]

pC <- ggplot(long_r, aes(omics, r, fill = method)) +
  geom_hline(yintercept = 0, linetype = 3, linewidth = 0.3, colour = "grey50") +
  geom_boxplot(width = 0.62, position = position_dodge(width = 0.72),
               outlier.size = 0.25, outlier.alpha = 0.4, linewidth = 0.28,
               colour = "grey20") +
  scale_fill_manual(values = c("Pooled baseline" = "#BDBDBD", "moGSVA fused" = "#3C5488"),
                    name = NULL) +
  scale_x_discrete(labels = function(x) {
    paste0(OMICS_LABEL[x], "\n(", n_obs$n[match(x, n_obs$omics)], ")")
  }) +
  scale_y_continuous(limits = c(-0.8, 1), breaks = seq(-0.75, 1, 0.25)) +
  labs(x = NULL, y = "r (score vs per-omics ES)",
       title = "Fusion keeps every layer, pooling keeps the transcriptome",
       subtitle = "Per-pathway correlation with each layer score") +
  theme(panel.grid.major.x = element_blank(),
        legend.position = "bottom",
        axis.text.x = element_text(size = 5.4, lineheight = 0.95))

## ---- D: best and worst agreeing pathways -----------------------------------
pick <- rd[order(r)][c(1, .N)]
mk_ex <- function(pw, tag) {
  dt <- data.table(pooled = P2[pw, ], fused = S2[pw, ])
  ggplot(dt, aes(pooled, fused)) +
    geom_point(size = 0.65, alpha = 0.5, colour = "#3C5488", stroke = 0) +
    geom_smooth(method = "lm", formula = y ~ x, se = FALSE, linewidth = 0.45,
                colour = "#A6362A") +
    annotate("text", x = -Inf, y = Inf, hjust = -0.08, vjust = 1.6, size = 2.2,
             colour = "grey15", lineheight = 0.95,
             label = sprintf("r = %.2f", rd[pathway == pw, r])) +
    labs(x = "Pooled score", y = if (tag == "low") "Fused score" else NULL,
         title = if (tag == "low") "Lowest agreement" else "Highest agreement",
         subtitle = wrap_pathway(pw, 32)) +
    theme(plot.title = element_text(size = 6, face = "bold"),
          plot.subtitle = element_text(size = 5.8, colour = "grey20",
                                       lineheight = 0.95, margin = margin(b = 2)))
}
pD <- wrap_elements(mk_ex(pick$pathway[1], "low") | mk_ex(pick$pathway[2], "high"))

## ---- assemble --------------------------------------------------------------
fig5 <- (pA | pB) / (pC | pD) +
  plot_layout(heights = c(1.05, 1), widths = c(1, 1)) +
  plot_annotation(tag_levels = "A") &
  theme(plot.tag = element_text(size = 10, face = "bold"))

save_fig(fig5, "Figure5_ablation_pooled_baseline", width_mm = 183, height_mm = 168)
