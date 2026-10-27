# =============================================================================
# Figure 6 -- permutation-based significance and the determinants of power
#   A  observed test statistic for the tested pathways
#   B  distribution of empirical p values against the uniform expectation
#   C  the test statistic is driven by pathway dispersion
#   D  dispersion itself is set by gene-set size and layer number
# =============================================================================

source(file.path(Sys.getenv("MOGSVA_SCRIPT_DIR",
                           "G:/GCModeller/test/demo/HTS/GSVA/mogsva_figures"),
                 "00_common.R"))
suppressPackageStartupMessages(library(matrixStats))

d <- load_mogsva()
S <- d$scores
sig <- copy(d$significance)
sig[, logp := -log10(p_value)]

sdv <- rowSds(S)
mem <- d$coverage[, .(n_total = sum(member_count), n_layer = sum(member_count > 0)),
                  by = pathway]
mem[, sd := sdv[pathway]]
sig <- merge(sig, mem[, .(pathway, n_total, n_layer, sd)], by = "pathway")

B <- round(1 / min(sig$p_value)) - 1   # effective number of permutations

## ---- A: observed statistics ------------------------------------------------
top <- sig[order(-statistic)][1:15]
top[, pathway := factor(pathway, levels = rev(pathway))]
top[, star := fifelse(q_value < 0.25, "q < 0.25", "")]

pA <- ggplot(top, aes(statistic, pathway)) +
  geom_segment(aes(x = 0, xend = statistic, yend = pathway, colour = logp),
               linewidth = 0.55, lineend = "round") +
  geom_point(aes(colour = logp), size = 1.5) +
  geom_text(aes(label = sprintf("P = %.3f", p_value)), hjust = -0.35, size = 2.0,
            colour = "grey30") +
  scale_colour_gradient(low = "#C9D6E2", high = "#1F4E79", name = "-log10 P") +
  scale_y_discrete(labels = function(x) wrap_pathway(x, 40),
                   expand = expansion(add = 0.6)) +
  scale_x_continuous(expand = expansion(mult = c(0, 0.28))) +
  labs(x = "Observed statistic  max|ES_multi|", y = NULL,
       title = "No pathway reaches FDR < 5%",
       subtitle = sprintf("15 of %d tested pathways, %d permutations; smallest q = %.3f",
                          nrow(sig), B, min(sig$q_value))) +
  theme(panel.grid.major.y = element_blank(),
        axis.text.y = element_text(size = 5.2, lineheight = 0.88),
        legend.position = "bottom",
        legend.direction = "horizontal",
        legend.key.width = unit(12, "pt"),
        legend.title = element_text(size = 5.2))

## ---- B: p-value ECDF -------------------------------------------------------
ec <- data.table(p = sort(sig$p_value))
ec[, obs := seq_len(.N) / .N]

pB <- ggplot(ec, aes(p, obs)) +
  geom_abline(slope = 1, intercept = 0, linetype = 2, linewidth = 0.35, colour = "grey45") +
  geom_step(linewidth = 0.5, colour = "#3C5488") +
  annotate("text", x = 0.03, y = 0.9, hjust = 0, size = 2.2, colour = "grey25",
           label = "uniform (null)") +
  annotate("text", x = 0.50, y = 0.06, hjust = 0, size = 2.2, colour = "#A6362A",
           lineheight = 0.95,
           label = sprintf("smallest attainable P = %.4f\n(empirical resolution 1/(B+1))",
                           1 / (B + 1))) +
  scale_x_continuous(labels = number_format(accuracy = 0.1),
                     expand = expansion(mult = c(0.01, 0.02))) +
  scale_y_continuous(expand = expansion(mult = c(0.01, 0.02))) +
  labs(x = "Empirical P value", y = "Cumulative fraction of pathways",
       title = "The permutation test is conservative",
       subtitle = sprintf("max_j |ES_multi|, %d pathways, %d permutations", nrow(sig), B))

## ---- C: statistic vs dispersion --------------------------------------------
rC <- cor(sig$statistic, sig$sd)

pC <- ggplot(sig, aes(sd, statistic)) +
  geom_point(aes(colour = logp), size = 1.0, alpha = 0.9, stroke = 0) +
  geom_smooth(method = "lm", formula = y ~ x, se = TRUE, linewidth = 0.4,
              colour = "#A6362A", fill = "grey88", alpha = 0.55) +
  scale_colour_gradient(low = "#C9D6E2", high = "#1F4E79", name = "-log10 P") +
  annotate("text", x = Inf, y = Inf, hjust = 1.1, vjust = 1.6, size = 2.3,
           colour = "grey15", label = sprintf("Pearson r = %.2f", rC)) +
  labs(x = "SD of ES_multi across tumours", y = "Observed statistic  max|ES_multi|",
       title = "Power follows pathway dispersion",
       subtitle = sprintf("The %d tested pathways", nrow(sig))) +
  theme(legend.position = c(0.98, 0.02), legend.justification = c(1, 0),
        legend.direction = "horizontal",
        legend.key.width = unit(10, "pt"),
        legend.title = element_text(size = 5.2))

## ---- D: dispersion vs set size ---------------------------------------------
rho <- cor(mem$sd, log2(mem$n_total), method = "spearman")
mem[, n_layer_f := factor(n_layer, levels = 4:1)]

pD <- ggplot(mem, aes(n_total, sd)) +
  geom_point(aes(fill = n_layer_f), shape = 21, size = 1.25, stroke = 0.15,
             colour = "grey25", alpha = 0.85) +
  geom_smooth(method = "loess", formula = y ~ x, se = TRUE, linewidth = 0.45,
              colour = "grey20", fill = "grey85", alpha = 0.5, span = 1) +
  scale_fill_manual(values = c(`4` = "#1F4E79", `3` = "#6C93B8", `2` = "#B9CBDB", `1` = "#E3E8ED"),
                    name = "Layers", drop = FALSE) +
  scale_x_continuous(trans = "log2", labels = comma) +
  annotate("text", x = Inf, y = Inf, hjust = 1.1, vjust = 1.6, size = 2.3,
           colour = "grey15",
           label = sprintf("Spearman \u03C1 = %.2f", rho)) +
  labs(x = "Total mapped members (log2 scale)", y = "SD of ES_multi",
       title = "Small gene sets are noisier",
       subtitle = sprintf("%d KEGG pathways", nrow(mem))) +
  theme(legend.position = c(0.02, 0.02), legend.justification = c(0, 0),
        legend.direction = "horizontal",
        legend.title = element_text(size = 5.2),
        legend.key.size = unit(6, "pt"))

## ---- assemble --------------------------------------------------------------
fig6 <- (pA | pB) / (pC | pD) +
  plot_layout(heights = c(1.12, 1)) +
  plot_annotation(tag_levels = "A") &
  theme(plot.tag = element_text(size = 10, face = "bold"))

save_fig(fig6, "Figure6_significance_and_power", width_mm = 183, height_mm = 158)
