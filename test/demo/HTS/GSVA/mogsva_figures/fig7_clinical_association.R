# =============================================================================
# Figure 7 -- clinical context of the fused pathway activity
#   A  pathways that differ between ER-positive and ER-negative tumours
#   B  volcano plot of the ER contrast over all pathways
#   C  activity profile across IHC-defined subtypes
#   D  overall survival by pathway activity
# =============================================================================

source(file.path(Sys.getenv("MOGSVA_SCRIPT_DIR",
                           "G:/GCModeller/test/demo/HTS/GSVA/mogsva_figures"),
                 "00_common.R"))
suppressPackageStartupMessages({
  library(survival)
  library(ggrepel)
})

d <- load_mogsva()
S <- d$scores
an <- d$annotation

## ---- ER contrast -----------------------------------------------------------
er <- an$er
keep <- !is.na(er) & er %in% c("Positive", "Negative")
n_pos <- sum(er[keep] == "Positive")
n_neg <- sum(er[keep] == "Negative")

er_tab <- data.table(
  pathway = rownames(S),
  p = apply(S[, keep], 1, function(x) stats::wilcox.test(x ~ factor(er[keep]))$p.value),
  delta = apply(S[, keep], 1, function(x)
    median(x[er[keep] == "Positive"]) - median(x[er[keep] == "Negative"]))
)
er_tab[, q := p.adjust(p, "BH")]
er_tab[, direction := fifelse(delta > 0, "Higher in ER+", "Higher in ER-")]

## ---- A: top pathways by FDR ------------------------------------------------
topA <- er_tab[order(p)][1:6]$pathway
longA <- data.table(
  pathway = rep(topA, times = keep_n <- sum(keep)),
  score   = as.vector(t(S[topA, keep])),
  er      = rep(er[keep], times = length(topA))
)
longA[, pathway := factor(pathway, levels = rev(topA))]
labA <- er_tab[pathway %in% topA][order(match(pathway, topA))]
labA[, pathway := factor(pathway, levels = rev(topA))]

pA <- ggplot(longA, aes(score, pathway, fill = er)) +
  geom_vline(xintercept = 0, linetype = 3, linewidth = 0.3, colour = "grey55") +
  geom_boxplot(width = 0.66, outlier.shape = NA, linewidth = 0.28,
               position = position_dodge(width = 0.75), colour = "grey25") +
  geom_text(data = labA, aes(x = Inf, y = pathway, label = fmt_q(q)),
            inherit.aes = FALSE, hjust = 1.05, vjust = -0.35, size = 2.0,
            colour = "grey25") +
  scale_fill_manual(values = c("Positive" = PAL_STATUS[["Positive"]],
                               "Negative" = PAL_STATUS[["Negative"]]),
                    name = "ER (IHC)") +
  scale_y_discrete(labels = function(x) wrap_pathway(x, 34)) +
  scale_x_continuous(expand = expansion(mult = c(0.03, 0.03))) +
  labs(x = "ES_multi", y = NULL,
       title = "Most pathways differ by ER status",
       subtitle = sprintf("Top 6 of %d pathways; %d ER+ vs %d ER- tumours; Wilcoxon, BH",
                          nrow(er_tab), n_pos, n_neg)) +
  guides(fill = guide_legend(nrow = 1, override.aes = list(linewidth = 0))) +
  theme(panel.grid.major.y = element_blank(),
        axis.text.y = element_text(size = 5.3, lineheight = 0.88),
        legend.position = "bottom")

## ---- B: volcano ------------------------------------------------------------
er_tab[, logq := -log10(q)]
er_tab[, sig := q < 0.05 & abs(delta) > 0.05]
labB <- er_tab[sig == TRUE][order(-abs(delta))][1:8]

pB <- ggplot(er_tab, aes(delta, logq)) +
  geom_hline(yintercept = -log10(0.05), linetype = 2, linewidth = 0.3, colour = "grey45") +
  geom_vline(xintercept = 0, linewidth = 0.25, colour = "grey80") +
  geom_point(data = er_tab[sig == FALSE], colour = "grey78", size = 0.6, stroke = 0) +
  geom_point(data = er_tab[sig == TRUE], aes(colour = direction), size = 0.8, stroke = 0) +
  geom_text_repel(data = labB, aes(label = wrap_pathway(pathway, 22), colour = direction),
                  size = 1.75, lineheight = 0.9, segment.size = 0.18,
                  min.segment.length = 0, max.overlaps = 20,
                  box.padding = 0.18, point.padding = 0.1, show.legend = FALSE) +
  scale_colour_manual(values = c("Higher in ER+" = PAL_STATUS[["Positive"]],
                                 "Higher in ER-" = PAL_STATUS[["Negative"]]),
                      name = NULL) +
  scale_y_continuous(expand = expansion(mult = c(0.02, 0.12))) +
  labs(x = "Median ES_multi difference (ER+ minus ER-)", y = "-log10 q",
       title = "ER-negative tumours carry more altered pathways",
       subtitle = sprintf("%d of %d pathways at FDR < 0.05 (BH over all tests)",
                          sum(er_tab$q < 0.05), nrow(er_tab))) +
  guides(colour = guide_legend(nrow = 1, override.aes = list(size = 1.2))) +
  theme(legend.position = "bottom")

## ---- C: subtype profile ----------------------------------------------------
sub_levels <- c("ER+/HER2-", "ER+/HER2+", "ER-/HER2-", "ER-/HER2+")
sub <- an$subtype
ok <- sub %in% sub_levels
topC <- er_tab[order(p)][1:18]$pathway

prof <- data.table(
  pathway = rep(topC, times = sum(ok)),
  score   = as.vector(t(S[topC, ok])),
  subtype = rep(sub[ok], each = length(topC))
)
prof[, subtype := factor(subtype, levels = sub_levels)]
mean_prof <- prof[, .(m = mean(score)), by = .(pathway, subtype)]
mean_prof[, pathway := factor(pathway, levels = rev(topC))]
n_sub <- table(factor(sub[ok], levels = sub_levels))

pC <- ggplot(mean_prof, aes(subtype, pathway, fill = m)) +
  geom_tile(colour = "white", linewidth = 0.6) +
  scale_fill_gradient2(low = GRAD_LOW, mid = GRAD_MID, high = GRAD_HIGH,
                       midpoint = 0, name = "Mean ES",
                       guide = guide_colourbar(barwidth = unit(2.4, "mm"),
                                               barheight = unit(20, "mm"))) +
  scale_x_discrete(labels = function(x) paste0(x, "\n(", n_sub[x], ")")) +
  scale_y_discrete(labels = function(x) wrap_pathway(x, 40)) +
  labs(x = NULL, y = NULL,
       title = "Subtype-specific activity",
       subtitle = "Mean ES_multi; 18 pathways with the strongest ER effect") +
  theme(panel.grid = element_blank(),
        axis.text.x = element_text(size = 5.4, lineheight = 0.95),
        axis.text.y = element_text(size = 5.0, lineheight = 0.88))

## ---- D: overall survival ---------------------------------------------------
time <- an$os_time
event <- an$os_event
ok_os <- !is.na(time) & !is.na(event) & time > 0

os_p <- vapply(rownames(S), function(pw) {
  x <- S[pw, ok_os]
  g <- factor(x > median(x), levels = c(FALSE, TRUE))
  sd <- survdiff(Surv(time[ok_os], event[ok_os]) ~ g)
  stats::pchisq(sd$chisq, df = 1, lower.tail = FALSE)
}, numeric(1))
os_q <- p.adjust(os_p, "BH")
# pre-specified: the pathway with the strongest ER contrast (panel A), so that
# the survival panel stays anchored to the same biological axis
best <- er_tab[order(p)][1, pathway]

x <- S[best, ok_os]
grp <- factor(ifelse(x > median(x), "High activity", "Low activity"),
              levels = c("Low activity", "High activity"))
sf <- survfit(Surv(time[ok_os], event[ok_os]) ~ grp)
km <- data.table(
  time = sf$time,
  surv = sf$surv,
  lower = sf$lower,
  upper = sf$upper,
  grp = rep(sub("grp=", "", names(sf$strata)), sf$strata)
)
km <- rbind(data.table(time = 0, surv = 1, lower = 1, upper = 1, grp = levels(grp)), km)
cox <- coxph(Surv(time[ok_os], event[ok_os]) ~ grp)
hr <- exp(coef(cox))[1]
ci <- exp(confint(cox))[1, ]

cens <- data.table(time = sf$time, surv = sf$surv, n.censor = sf$n.censor,
                   grp = rep(sub("grp=", "", names(sf$strata)), sf$strata))
cens <- cens[n.censor > 0]

pD <- ggplot(km, aes(time, surv, colour = grp)) +
  geom_step(linewidth = 0.55) +
  geom_point(data = cens, shape = 124, size = 1.6, stroke = 0.35, show.legend = FALSE) +
  scale_colour_manual(values = c("Low activity" = "#7F7F7F", "High activity" = "#A6362A"),
                      name = NULL) +
  scale_x_continuous(limits = c(0, ceiling(max(time[ok_os]) / 10) * 10),
                     expand = expansion(mult = c(0.01, 0.02))) +
  scale_y_continuous(limits = c(0, 1.02), expand = expansion(mult = c(0.01, 0.02)),
                     labels = number_format(accuracy = 0.1)) +
  annotate("text", x = Inf, y = 0.98, hjust = 1.02, vjust = 1, size = 2.2,
           colour = "grey15", lineheight = 1.05,
           label = sprintf("Log-rank %s\nHR = %.2f (%.2f-%.2f)",
                           fmt_p(os_p[best]), hr, ci[1], ci[2])) +
  labs(x = "Overall survival (months)", y = "Survival probability",
       title = sprintf("Survival by %s", wrap_pathway(best, 30)),
       subtitle = sprintf("Median split, n = %d per group; %d deaths",
                          sum(grp == "High activity"), sum(event[ok_os] == 1))) +
  guides(colour = guide_legend(nrow = 1)) +
  theme(legend.position = "bottom")

## ---- assemble --------------------------------------------------------------
fig7 <- (pA | pB) / (pC | pD) +
  plot_layout(heights = c(1, 1), widths = c(1, 1.08)) +
  plot_annotation(tag_levels = "A") &
  theme(plot.tag = element_text(size = 10, face = "bold"))

save_fig(fig7, "Figure7_clinical_association", width_mm = 183, height_mm = 178)
