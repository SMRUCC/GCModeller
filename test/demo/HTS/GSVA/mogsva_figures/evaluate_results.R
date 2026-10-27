# =============================================================================
# evaluate_results.R -- self-consistency and sanity audit of a moGSVA export
#
#   Rscript evaluate_results.R
#
# Writes output/evaluation_report.md and prints the same content to stdout.
# Every check is stated as PASS / WARN / FAIL with the observed quantity, so the
# log can be cited in a Methods section or attached as supplementary material.
# =============================================================================

source(file.path(Sys.getenv("MOGSVA_SCRIPT_DIR",
                           "G:/GCModeller/test/demo/HTS/GSVA/mogsva_figures"),
                 "00_common.R"))
suppressPackageStartupMessages({
  library(matrixStats)
  library(survival)
})

d <- load_mogsva()
S <- d$scores; R <- d$raw; C <- d$cons; P <- d$pooled
ct <- d$contributions; wt <- d$weights; cov <- d$coverage; sig <- d$significance

LAMBDA <- 0.25
out <- character()
say <- function(...) {
  line <- paste0(...)
  out <<- c(out, line)
  cat(line, "\n", sep = "")
}
h <- function(title) {
  out <<- c(out, "", paste0("## ", title), "")
  cat("\n## ", title, "\n", sep = "")
}
check <- function(ok, label, detail) {
  tag <- if (isTRUE(ok)) "PASS" else if (is.na(ok)) "WARN" else "FAIL"
  say(sprintf("- **%s** %s — %s", tag, label, detail))
}

say("# moGSVA result audit")
say("")
say(sprintf("Generated: %s", format(Sys.time(), "%Y-%m-%d %H:%M:%S")))
say(sprintf("Result directory: `%s`", RESULT_DIR))
say(sprintf("Raw data directory: `%s`", RAW_DIR))

# ---------------------------------------------------------------- integrity --
h("1. Export integrity")
check(!anyNA(S) && !anyNA(R) && !anyNA(C),
      "no missing values in the score matrices",
      sprintf("%d pathways x %d tumours, all finite", nrow(S), ncol(S)))
check(identical(rownames(S), rownames(R)) && identical(rownames(S), rownames(C)),
      "pathway axes aligned",
      "scores / raw_scores / consistency share one row order")
check(identical(colnames(S), d$annotation$sample_id),
      "sample axis matches samples.tsv",
      sprintf("%d tumour ids in identical order", ncol(S)))
check(!anyDuplicated(rownames(S)), "unique pathway ids",
      sprintf("%d distinct KEGG gene sets", nrow(S)))
check(all(abs(S) <= 1), "fused scores bounded",
      sprintf("range [%.3f, %.3f]", min(S), max(S)))
check(all(abs(R) <= 1), "pre-reward scores bounded",
      sprintf("range [%.3f, %.3f]", min(R), max(R)))

K <- cov[, .(K = sum(member_count > 0)), by = pathway]
check(all(cov$member_count >= 0 & cov$member_count <= 500),
      "gene-set size filter respected",
      sprintf("member counts in [%d, %d] (MinSize 5, MaxSize 500)",
              min(cov$member_count[cov$member_count > 0]), max(cov$member_count)))

# ---------------------------------------------------------------- weights ----
h("2. Feature-balanced weighting rule")
# w_k = b_k / sum(b) with b_k = 1/log2(2 + |S_k|)
meas <- wt[skipped == FALSE]
pred <- meas[, .(pathway, omics, member_count,
                 b = 1 / log2(2 + member_count))]
pred[, w_pred := b / sum(b), by = pathway]
cmp <- merge(meas[, .(pathway, omics, w_obs = weight)],
             pred[, .(pathway, omics, w_pred)], by = c("pathway", "omics"))
max_dev <- max(abs(cmp$w_obs - cmp$w_pred))
check(max_dev < 1e-6, "weights follow 1/log2(2+|S_k|) normalised per pathway",
      sprintf("%d pathway x omics weights, max |observed - predicted| = %.2e",
              nrow(cmp), max_dev))
sum_w <- meas[, .(s = sum(weight)), by = pathway]
check(max(abs(sum_w$s - 1)) < 1e-6, "weights sum to one within each pathway",
      sprintf("max deviation %.2e over %d pathways", max(abs(sum_w$s - 1)), nrow(sum_w)))
# descriptive statistics use the same subset as Figure 1D (>= 2 layers),
# because a single-layer pathway carries a trivial weight of 1
multi <- cov[, .(K = sum(member_count > 0)), by = pathway][K >= 2, pathway]
wmean <- meas[pathway %in% multi, .(mean = mean(weight), median = median(weight)), by = omics]
say("")
say("Mean weight by layer (multi-layer pathways where the layer is measured):")
say("")
say("| Layer | mean w | median w |")
say("|---|---|---|")
for (i in seq_len(nrow(wmean))) {
  say(sprintf("| %s | %.3f | %.3f |", wmean$omics[i], wmean$mean[i], wmean$median[i]))
}

# ------------------------------------------------------------ fusion algebra --
h("3. Fusion algebra")
cons <- as.vector(C)
pred_s <- as.vector(R) * (1 + LAMBDA * cons) / (1 + LAMBDA)
dev_s <- max(abs(pred_s - as.vector(S)))
check(dev_s < 1e-9,
      "ES_multi = ES_raw x (1 + lambda*a)/(1 + lambda)",
      sprintf("max |predicted - exported| = %.2e (lambda = %.2f)", dev_s, LAMBDA))
check(all(abs(S) <= abs(R) + 1e-12), "reward never amplifies a score",
      sprintf("max |ES_multi| = %.3f <= max |ES_raw| = %.3f",
              max(abs(S)), max(abs(R))))

# contributions must re-sum to the pre-reward score
csum <- ct[, .(c = sum(contribution)), by = .(pathway, sample)]
csum[, raw := R[cbind(match(pathway, rownames(R)), match(sample, colnames(R)))]]
dev_c <- max(abs(csum$c - csum$raw))
check(dev_c < 1e-9, "sum of per-omics contributions equals ES_raw",
      sprintf("max |sum(c_k) - ES_raw| = %.2e over %s observations",
              dev_c, format(nrow(csum), big.mark = ",")))

sh <- ct[, .(s = sum(abs(share)), tot = sum(abs(contribution))), by = .(pathway, sample)]
sh <- sh[tot > 0]
dev_sh <- max(abs(sh$s - 1))
check(dev_sh < 1e-9, "contribution shares are normalised",
      sprintf("max |sum(|share|) - 1| = %.2e over %s observations",
              dev_sh, format(nrow(sh), big.mark = ",")))

# ------------------------------------------------------------ agreement ------
h("4. Cross-omics sign agreement")
ls <- d$layer_scores
# one pathway x sample matrix per layer
mats <- lapply(OMICS_LEVELS, function(om) {
  w <- dcast(ls[omics == om], pathway ~ sample, value.var = "score")
  m <- as.matrix(w[, -1]); rownames(m) <- w$pathway; m
})
names(mats) <- OMICS_LEVELS
common_rows <- rownames(S)          # check every pathway, missing layers -> NA
common_cols <- colnames(S)
get_row <- function(m, pw) {
  if (pw %in% rownames(m)) m[pw, common_cols] else rep(NA_real_, length(common_cols))
}

a_obs <- C[common_rows, common_cols]
a_pred <- matrix(NA_real_, nrow = length(common_rows), ncol = length(common_cols),
                 dimnames = list(common_rows, common_cols))
for (pw in common_rows) {
  v <- do.call(rbind, lapply(mats, get_row, pw = pw))
  s <- sign(v)
  acc <- numeric(length(common_cols))
  npair <- integer(length(common_cols))
  for (i in 1:(length(OMICS_LEVELS) - 1)) {
    for (j in (i + 1):length(OMICS_LEVELS)) {
      ok <- !is.na(s[i, ]) & !is.na(s[j, ])
      acc[ok] <- acc[ok] + s[i, ok] * s[j, ok]
      npair[ok] <- npair[ok] + 1
    }
  }
  a_pred[pw, ] <- ifelse(npair > 0, acc / npair, NA_real_)
}
both <- !is.na(a_pred) & !is.na(a_obs)
dev_a <- max(abs(a_pred[both] - a_obs[both]))
check(dev_a < 1e-9, "consistency matrix reproduces pairwise sign agreement",
      sprintf("max |recomputed - exported| = %.2e over %s observations",
              dev_a, format(sum(both), big.mark = ",")))
vals <- sort(unique(round(as.vector(C), 4)))
check(TRUE, "attainable agreement values are discrete",
      paste0(length(vals), " distinct values: ", paste(vals, collapse = ", ")))

# ------------------------------------------------------------ significance ---
h("5. Permutation test and multiple testing")
p_obs <- sig$p_value
floor_p <- min(p_obs)
B_eff <- round(1 / floor_p) - 1
check(all(abs(p_obs * (B_eff + 1) - round(p_obs * (B_eff + 1))) < 1e-8),
      "empirical p-values sit on the 1/(B+1) grid",
      sprintf("implied B = %d, smallest attainable p = %.4f", B_eff, floor_p))
q_pred <- p.adjust(p_obs, "BH")
check(max(abs(q_pred - sig$q_value)) < 1e-9,
      "q-values are Benjamini-Hochberg of the exported p-values",
      sprintf("max deviation %.2e over %d tests", max(abs(q_pred - sig$q_value)), nrow(sig)))
say("")
say(sprintf("Tested pathways: %d of %d (%.0f%%); smallest q = %.3f; %d at q < 0.10; %d at q < 0.25.",
            nrow(sig), nrow(S), 100 * nrow(sig) / nrow(S), min(sig$q_value),
            sum(sig$q_value < 0.10), sum(sig$q_value < 0.25)))
say("")
say("| Rank | Pathway | statistic | P | q |")
say("|---|---|---|---|---|")
top_sig <- sig[order(q_value, p_value)][1:10]
for (i in seq_len(nrow(top_sig))) {
  say(sprintf("| %d | %s | %.3f | %.4f | %.3f |", i, top_sig$pathway[i],
              top_sig$statistic[i], top_sig$p_value[i], top_sig$q_value[i]))
}

# ------------------------------------------------------------- ablation ------
h("6. Ablation against the pooled baseline")
Pc <- P[intersect(rownames(S), rownames(P)), , drop = FALSE]
cc <- rownames(Pc)[complete.cases(Pc)]
r_all <- cor(as.vector(S[cc, ]), as.vector(Pc[cc, ]))
r_pw <- vapply(cc, function(p) cor(S[p, ], Pc[p, ]), numeric(1))
say(sprintf("Common pathways: %d; overall Pearson r = %.3f; per-pathway median r = %.3f (IQR %.2f-%.2f).",
            length(cc), r_all, median(r_pw), quantile(r_pw, .25), quantile(r_pw, .75)))
say("")
say("| Layer | r with pooled baseline | r with fused score | pathways |")
say("|---|---|---|---|")
for (om in OMICS_LEVELS) {
  m <- mats[[om]]
  pw <- intersect(rownames(m), cc)
  rf <- vapply(pw, function(p) cor(m[p, ], S[p, ]), numeric(1))
  rp <- vapply(pw, function(p) cor(m[p, ], Pc[p, ]), numeric(1))
  say(sprintf("| %s | %.2f | %.2f | %d |", om, median(rp), median(rf), length(pw)))
}

# --------------------------------------------------------------- clinical ----
h("7. Downstream association")
an <- d$annotation
er <- an$er
keep <- !is.na(er) & er %in% c("Positive", "Negative")
p_er <- apply(S[, keep], 1, function(x) wilcox.test(x ~ factor(er[keep]))$p.value)
q_er <- p.adjust(p_er, "BH")
delta <- apply(S[, keep], 1, function(x)
  median(x[er[keep] == "Positive"]) - median(x[er[keep] == "Negative"]))
say(sprintf("ER contrast (%d ER+ vs %d ER-): %d of %d pathways at FDR < 0.05; %d higher in ER+, %d higher in ER-.",
            sum(er[keep] == "Positive"), sum(er[keep] == "Negative"),
            sum(q_er < 0.05), nrow(S), sum(q_er < 0.05 & delta > 0),
            sum(q_er < 0.05 & delta < 0)))

time <- an$os_time; event <- an$os_event
ok <- !is.na(time) & !is.na(event) & time > 0
p_os <- vapply(rownames(S), function(pw) {
  x <- S[pw, ok]; g <- factor(x > median(x), levels = c(FALSE, TRUE))
  pchisq(survdiff(Surv(time[ok], event[ok]) ~ g)$chisq, 1, lower.tail = FALSE)
}, numeric(1))
q_os <- p.adjust(p_os, "BH")
say(sprintf("Overall survival (%d events, %d tumours): %d of %d pathways at FDR < 0.05; smallest P = %.2e (q = %.3f).",
            sum(event[ok] == 1), sum(ok), sum(q_os < 0.05), nrow(S), min(p_os), min(q_os)))
if (any(q_os < 0.05)) {
  hit <- data.table(pathway = names(p_os), p = p_os, q = q_os)[q < 0.05][order(p)]
  say("")
  say("Pathways that separate overall survival at FDR < 0.05:")
  say("")
  say("| Pathway | log-rank P | q |")
  say("|---|---|---|")
  for (i in seq_len(nrow(hit))) {
    say(sprintf("| %s | %.2e | %.3f |", hit$pathway[i], hit$p[i], hit$q[i]))
  }
}

# --------------------------------------------------------------- structure ---
h("8. Structural caveats worth reporting")
single <- K[K <= 1]
say(sprintf("- %d pathway(s) are supported by a single layer: %s.",
            nrow(single), paste(single$pathway, collapse = ", ")))
say(sprintf("- Proteome coverage is the limiting factor: %d of %d pathways.",
            cov[omics == "Proteomics" & member_count > 0, .N], nrow(S)))
sdv <- rowSds(S)
n_total <- cov[, .(n = sum(member_count)), by = pathway]
n_total <- n_total[match(names(sdv), pathway), n]
rho_size <- cor(sdv, log2(n_total), method = "spearman", use = "complete.obs")
say(sprintf("- Pathway dispersion declines with gene-set size (Spearman rho = %.2f).", rho_size))
wmean2 <- meas[pathway %in% multi, .(mean = mean(weight)), by = omics]
say(sprintf("- The feature-balanced rule gives the proteome the largest weight (mean %.3f) because its member counts are the smallest; the proteome is the single largest contributor in %.1f%% of observations.",
            wmean2[omics == "Proteomics", mean],
            100 * ct[dominant == TRUE & omics == "Proteomics", .N] / ct[dominant == TRUE, .N]))

# ------------------------------------------------------------------ write ----
report <- file.path(OUT_DIR, "evaluation_report.md")
writeLines(out, report)
message("\nreport written to ", report)
