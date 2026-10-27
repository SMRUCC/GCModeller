# =============================================================================
# stats_for_legends.R -- print every number cited in figure_legends.md
# Run once after the figures are built; nothing is written to disk.
# =============================================================================

source(file.path(Sys.getenv("MOGSVA_SCRIPT_DIR",
                           "G:/GCModeller/test/demo/HTS/GSVA/mogsva_figures"),
                 "00_common.R"))
suppressPackageStartupMessages({
  library(matrixStats)
  library(survival)
})

d <- load_mogsva()
S <- d$scores; C <- d$cons; an <- d$annotation
cat("== dimensions ==\n")
cat("pathways:", d$n_pathway, " tumours:", d$n_sample, "\n")
print(d$feature_count)

cov <- d$coverage
cat("\n== coverage ==\n")
print(cov[, .(measured = sum(member_count > 0)), by = omics])
K <- cov[, .(K = sum(member_count > 0)), by = pathway]
print(K[, .N, by = K][order(K)])

w <- d$weights[skipped == FALSE][pathway %in% K[K >= 2, pathway]]
cat("\n== weights (multi-layer, measured) ==\n")
print(w[, .(mean = mean(weight), median = median(weight)), by = omics])

cat("\n== agreement ==\n")
ag <- round(as.vector(C), 3)
print(round(100 * prop.table(table(factor(ag, levels = c(-1, -0.333, 0, 1)))), 1))

cat("\n== layer concordance (median per-pathway r) ==\n")
ls <- d$layer_scores
wide <- dcast(ls[!is.na(score)], pathway + sample ~ omics, value.var = "score")
for (p in combn(OMICS_LEVELS, 2, simplify = FALSE)) {
  sub <- wide[!is.na(get(p[1])) & !is.na(get(p[2]))]
  r <- sub[, cor(get(p[1]), get(p[2])), by = pathway]$V1
  cat(sprintf("%-22s vs %-16s n=%3d median r=%6.3f\n", p[1], p[2], length(r), median(r)))
}
cat("agreement vs SD spearman:", round(cor(rowMeans(C), rowSds(S), method = "spearman"), 3), "\n")

cat("\n== contributions ==\n")
print(d$contributions[dominant == TRUE, .N, by = omics][, pct := round(100 * N / sum(N), 1)])
ps <- d$contributions[, .(m = mean(abs(share))), by = omics]
print(ps)

cat("\n== ablation ==\n")
P <- d$pooled[intersect(rownames(S), rownames(d$pooled)), , drop = FALSE]
common <- rownames(P)[complete.cases(P)]
S2 <- S[common, ]; P2 <- P[common, ]
cat("n common:", length(common), " overall r:", round(cor(as.vector(S2), as.vector(P2)), 3), "\n")
rp <- sapply(common, function(p) cor(S2[p, ], P2[p, ]))
cat("median per-pathway r:", round(median(rp), 3), " IQR:", round(quantile(rp, c(.25, .75)), 3), "\n")
cat("worst:", names(which.min(rp)), round(min(rp), 3), " best:", names(which.max(rp)), round(max(rp), 3), "\n")
for (om in OMICS_LEVELS) {
  wd <- dcast(d$layer_scores[omics == om & pathway %in% common], pathway ~ sample, value.var = "score")
  m <- as.matrix(wd[, -1]); rownames(m) <- wd$pathway
  pw <- intersect(rownames(m), common)
  rf <- sapply(pw, function(p) cor(m[p, ], S2[p, ]))
  rq <- sapply(pw, function(p) cor(m[p, ], P2[p, ]))
  cat(sprintf("%-24s pooled=%.2f fused=%.2f n=%d\n", om, median(rq), median(rf), length(pw)))
}

cat("\n== significance ==\n")
sig <- d$significance
cat("tested:", nrow(sig), " min p:", min(sig$p_value), " min q:", min(sig$q_value),
    " B:", round(1 / min(sig$p_value)) - 1, "\n")
cat("max stat:", round(max(sig$statistic), 3), "\n")

cat("\n== clinical ==\n")
er <- an$er; keep <- er %in% c("Positive", "Negative")
cat("ER+:", sum(er[keep] == "Positive"), " ER-:", sum(er[keep] == "Negative"), "\n")
p <- apply(S[, keep], 1, function(x) wilcox.test(x ~ factor(er[keep]))$p.value)
q <- p.adjust(p, "BH")
delta <- apply(S[, keep], 1, function(x) median(x[er[keep] == "Positive"]) - median(x[er[keep] == "Negative"]))
cat("FDR<0.05:", sum(q < 0.05), " higher in ER+:", sum(q < 0.05 & delta > 0),
    " higher in ER-:", sum(q < 0.05 & delta < 0), "\n")
print(table(an$subtype))
time <- an$os_time; event <- an$os_event
ok <- !is.na(time) & !is.na(event) & time > 0
pw <- "Hedgehog signaling pathway"
x <- S[pw, ok]; g <- factor(x > median(x), levels = c(FALSE, TRUE))
sd <- survdiff(Surv(time[ok], event[ok]) ~ g); cx <- coxph(Surv(time[ok], event[ok]) ~ g)
cat(pw, "logrank p:", signif(pchisq(sd$chisq, 1, lower.tail = FALSE), 3),
    " HR:", round(exp(coef(cx))[1], 2), " CI:", round(exp(confint(cx))[1, ], 2),
    " deaths:", sum(event[ok]), "\n")
cat("shrinkage (median SD reduction):", round(100 * (1 - median(rowSds(S) / rowSds(d$raw))), 1), "%\n")
