# =============================================================================
# Figure 4 -- decomposing the fused score into per-omics contributions
#   A  mean absolute contribution share of the most variable pathways
#   B  which layer dominates, over all pathway x tumour observations
#   C  pathway-level signed contribution share
# =============================================================================

source(file.path(Sys.getenv("MOGSVA_SCRIPT_DIR",
                           "G:/GCModeller/test/demo/HTS/GSVA/mogsva_figures"),
                 "00_common.R"))
suppressPackageStartupMessages(library(matrixStats))

d <- load_mogsva()
ct <- d$contributions
S <- d$scores
vv <- rowVars(S)

ps <- ct[, .(mean_abs = mean(abs(share)), mean_signed = mean(share)),
         by = .(pathway, omics)]
n_layer <- d$coverage[, .(n_layer = sum(member_count > 0)), by = pathway]
multi <- n_layer[n_layer >= 2, pathway]

wide_abs <- dcast(ps, pathway ~ omics, value.var = "mean_abs")
wide_abs[, pwvar := vv[pathway]]

## ---- A: composition of the 20 most variable multi-layer pathways -----------
sel <- wide_abs[pathway %in% multi][order(-pwvar)][1:20]$pathway

comp <- ps[pathway %in% sel]
comp[, tot := sum(mean_abs), by = pathway]
comp[, frac := mean_abs / tot]
comp[, pathway := factor(pathway, levels = rev(sel))]

pA <- ggplot(comp, aes(frac, pathway, fill = omics)) +
  geom_col(width = 0.74, colour = "white", linewidth = 0.3) +
  scale_fill_manual(values = PAL_OMICS, labels = OMICS_LABEL[OMICS_LEVELS],
                    name = NULL) +
  scale_x_continuous(expand = expansion(mult = c(0, 0.01)),
                     labels = function(x) paste0(x * 100, "%")) +
  scale_y_discrete(labels = function(x) wrap_pathway(x, 36),
                   expand = expansion(add = 0.55)) +
  guides(fill = guide_legend(nrow = 2, byrow = TRUE)) +
  labs(x = "Mean |contribution share|", y = NULL,
       title = "Which layer drives the fused score",
       subtitle = "20 most variable pathways measured in >= 2 layers (none with proteomics)") +
  theme(panel.grid.major.y = element_blank(),
        axis.text.y = element_text(size = 5.3, lineheight = 0.88),
        legend.position = "bottom",
        legend.key.size = unit(6, "pt"))

## ---- B: dominant layer -----------------------------------------------------
dom <- ct[dominant == TRUE, .N, by = omics]
dom[, pct := 100 * N / sum(N)]
dom[, omics := factor(omics, levels = OMICS_LEVELS)]

pB <- ggplot(dom, aes(omics, pct, fill = omics)) +
  geom_col(width = 0.64, colour = "grey25", linewidth = 0.25) +
  geom_text(aes(label = sprintf("%.1f%%", pct)), vjust = -0.5, size = 2.3,
            colour = "grey15") +
  scale_fill_manual(values = PAL_OMICS, guide = "none") +
  scale_x_discrete(labels = function(x) OMICS_LABEL[x]) +
  scale_y_continuous(expand = expansion(mult = c(0, 0.2))) +
  labs(x = NULL, y = "Dominant observations (%)",
       title = "Dominant layer",
       subtitle = sprintf("%s pathway x tumour observations", comma(nrow(ct) / 4))) +
  theme(panel.grid.major.x = element_blank(),
        axis.text.x = element_text(angle = 32, hjust = 1, size = 5.6))

## ---- C: per-pathway dominance profile --------------------------------------
# proportion of tumours in which each layer is the single largest contributor
dom_pw <- ct[, .(pct = 100 * mean(dominant)), by = .(pathway, omics)]
topc <- wide_abs[pathway %in% multi][order(-pwvar)][1:24]$pathway

dp <- dom_pw[pathway %in% topc]
wide_dom <- dcast(dp, pathway ~ omics, value.var = "pct")
mat <- as.matrix(wide_dom[, ..OMICS_LEVELS])
rownames(mat) <- wide_dom$pathway
mat <- mat[topc, , drop = FALSE]

lead <- OMICS_LEVELS[max.col(mat, ties.method = "first")]
o <- order(factor(lead, levels = rev(OMICS_LEVELS)), -mat[cbind(seq_along(lead), max.col(mat))])
mat <- mat[o, , drop = FALSE]
lead <- lead[o]

tile <- data.table(
  pathway = rep(rownames(mat), times = ncol(mat)),
  omics   = rep(colnames(mat), each = nrow(mat)),
  pct     = as.vector(mat)
)
tile[, lead := factor(rep(lead, times = ncol(mat)), levels = rev(OMICS_LEVELS))]
tile[, pathway := factor(pathway, levels = rev(rownames(mat)))]
tile[, omics := factor(omics, levels = OMICS_LEVELS)]

pC <- ggplot(tile, aes(omics, pathway, fill = pct)) +
  geom_tile(colour = "white", linewidth = 0.5) +
  geom_text(aes(label = ifelse(pct >= 5, sprintf("%.0f", pct), ""),
                colour = pct > 62),
            size = 1.95, show.legend = FALSE) +
  scale_colour_manual(values = c(`FALSE` = "grey15", `TRUE` = "white")) +
  facet_grid(lead ~ ., scales = "free_y", space = "free_y",
             labeller = as_labeller(c("Copy number alteration" = "CNA",
                                      "Methylation" = "MET",
                                      "Transcriptomics" = "TX",
                                      "Proteomics" = "PROT"))) +
  scale_fill_gradient(low = "#F2F6FA", high = "#1F4E79", limits = c(0, 100),
                      breaks = c(0, 25, 50, 75, 100),
                      labels = function(x) paste0(x, "%"),
                      name = "Tumours dominated",
                      guide = guide_colourbar(direction = "horizontal",
                                              barwidth = unit(24, "mm"),
                                              barheight = unit(2.4, "mm"),
                                              title.vjust = 0.85)) +
  scale_x_discrete(labels = c("Copy number alteration" = "CNA", "Methylation" = "MET",
                              "Transcriptomics" = "TX", "Proteomics" = "PROT"),
                   position = "top") +
  scale_y_discrete(labels = function(x) wrap_pathway(x, 44),
                   expand = expansion(add = 0.45)) +
  labs(x = NULL, y = NULL,
       title = "Layer dominance is pathway specific",
       subtitle = "Share of tumours dominated by each layer") +
  theme(panel.grid = element_blank(),
        axis.text.x.top = element_text(angle = 0, hjust = 0.5, size = 5.2),
        axis.text.y = element_text(size = 4.9, lineheight = 0.88),
        strip.background = element_blank(),
        strip.text.y = element_text(angle = -90, size = 4.9, face = "bold"),
        panel.spacing.y = unit(0.9, "mm"),
        legend.position = "bottom",
        legend.title = element_text(size = 5.4))

## ---- assemble --------------------------------------------------------------
fig4 <- pA + pB + pC +
  plot_layout(design = "
    AB
    AC", widths = c(1.22, 1), heights = c(1, 2.2)) +
  plot_annotation(tag_levels = "A") &
  theme(plot.tag = element_text(size = 10, face = "bold"))

save_fig(fig4, "Figure4_omics_contributions", width_mm = 183, height_mm = 185)
