suppressMessages({library(data.table)})
RES <- "G:/GCModeller/test/demo/HTS/GSVA/brca_tcga_pub2015_mogsva"
RAW <- "K:/brca_tcga_pub2015/multipleOmics"
S <- as.matrix(fread(file.path(RES, "scores.csv")), rownames = 1)
sig <- fread(file.path(RES, "significance.csv"))
cat("sig == first 40 rows of scores? ", all(sig$pathway == rownames(S)[1:40]), "\n")
cat("p values unique:\n"); print(sort(unique(sig$p_value)))

md <- fread(file.path(RAW, "samples.tsv"))
cat("samples.tsv n", nrow(md), " match order:", identical(md$SAMPLE_ID, colnames(S)), "\n")
for (cc in c("SAMPLE_TYPE", "ER_STATUS_BY_IHC", "PR_STATUS_BY_IHC", "IHC_HER2", "AJCC_PATHOLOGIC_TUMOR_STAGE", "OS_STATUS", "DFS_STATUS")) {
  cat("\n--", cc, "\n"); print(table(md[[cc]], useNA = "ifany"))
}

ls <- fread(file.path(RES, "layer_scores.csv"))
cat("\nlayer_scores dims", dim(ls), "\n")
print(ls[, .(n = .N, min = min(score), max = max(score), sd = sd(score)), by = omics])

w <- fread(file.path(RES, "weights.csv"))
cv <- fread(file.path(RES, "coverage.csv"))
m <- merge(w, cv, by = c("pathway", "omics"), suffixes = c("", ".cv"))
print(m[omics == "Proteomics"][order(-member_count)][1:5])

# pathway size in gmt vs member counts
gmt <- fread(file.path(RAW, "kegg_v3.gmt"), sep = "\t", header = FALSE, fill = TRUE)
cat("\ngmt sets", nrow(gmt), "\n")

# layer-layer correlation
L <- split(ls, ls$omics)
wide <- lapply(names(L), function(nm) dcast(L[[nm]], pathway ~ sample, value.var = "score"))
names(wide) <- names(L)
common <- Reduce(intersect, lapply(wide, function(d) d$pathway))
cat("pathways present in all 4 layers:", length(common), "\n")
om <- names(wide)
M <- matrix(NA, 4, 4, dimnames = list(om, om))
for (i in 1:4) for (j in 1:4) {
  a <- as.matrix(wide[[i]][pathway %in% common][, -1]); b <- as.matrix(wide[[j]][pathway %in% common][, -1])
  rs <- sapply(seq_len(nrow(a)), function(k) suppressWarnings(cor(a[k, ], b[k, ])))
  M[i, j] <- median(rs, na.rm = TRUE)
}
print(round(M, 3))
