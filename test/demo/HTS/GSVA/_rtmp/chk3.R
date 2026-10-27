source("G:/GCModeller/test/demo/HTS/GSVA/mogsva_figures/00_common.R")
suppressMessages(library(matrixStats))
d <- load_mogsva()
S <- d$scores; C <- d$cons
sdv <- rowSds(S); mag <- rowMeans(abs(S)); agree <- rowMeans(C)
cat("cor agree vs sd:", cor(agree, sdv, method="spearman"), "\n")
cat("cor agree vs mean|ES|:", cor(agree, mag, method="spearman"), "\n")
cat("agreement table:/n"); print(table(round(C,3)))
cat("mean agreement range:", range(agree), "\n")
# layer correlation detail: per-pair distribution of pathway r
ls <- d$layer_scores
wide <- dcast(ls, pathway + sample ~ omics, value.var="score")
pairs <- combn(OMICS_LEVELS, 2, simplify=FALSE)
for (pr in pairs) {
  sub <- wide[!is.na(get(pr[1])) & !is.na(get(pr[2]))]
  r <- sub[, .(r = cor(get(pr[1]), get(pr[2]))), by=pathway]
  cat(sprintf("%-24s vs %-16s n=%3d median r=%6.3f  IQR %6.3f..%6.3f\n", pr[1], pr[2], nrow(r), median(r$r), quantile(r$r,.25), quantile(r$r,.75)))
}
