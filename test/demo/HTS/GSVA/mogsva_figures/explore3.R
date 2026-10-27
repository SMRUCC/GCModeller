suppressMessages({library(data.table)})
RES <- "G:/GCModeller/test/demo/HTS/GSVA/brca_tcga_pub2015_mogsva"
RAW <- "K:/brca_tcga_pub2015/multipleOmics"
read_matrix <- function(p) { dt <- fread(p, showProgress = FALSE); m <- as.matrix(dt[, -1]); rownames(m) <- dt[[1]]; storage.mode(m) <- "double"; m }
S <- read_matrix(file.path(RES, "scores.csv"))
cat("dup pathways:", any(duplicated(rownames(S))), " max name len:", max(nchar(rownames(S))), "\n")
md <- fread(file.path(RAW, "samples.tsv"))
er <- md$ER_STATUS_BY_IHC[match(colnames(S), md$SAMPLE_ID)]
keep <- er %in% c("Positive", "Negative")
p <- apply(S[, keep], 1, function(x) wilcox.test(x ~ er[keep])$p.value)
delta <- apply(S[, keep], 1, function(x) median(x[er[keep] == "Positive"]) - median(x[er[keep] == "Negative"]))
q <- p.adjust(p, "BH")
o <- order(p)
cat("ER: q<0.05 count", sum(q < 0.05, na.rm = TRUE), "\n")
print(data.table(pathway = rownames(S)[o][1:15], p = p[o][1:15], q = q[o][1:15], delta = delta[o][1:15]))
# 免疫通路?
print(data.table(pathway = rownames(S)[o][1:40])[grepl("infection|immune|rejection|disease|leukocyte|T cell|B cell|cytokine|interferon|NK", pathway, ignore.case = TRUE)])

# subtype groups
her2 <- md$IHC_HER2[match(colnames(S), md$SAMPLE_ID)]
pr <- md$PR_STATUS_BY_IHC[match(colnames(S), md$SAMPLE_ID)]
sub <- ifelse(er == "Positive" & her2 == "Negative", "ER+/HER2-",
       ifelse(er == "Positive" & her2 == "Positive", "ER+/HER2+",
       ifelse(er == "Negative" & her2 == "Negative", "ER-/HER2-",
       ifelse(er == "Negative" & her2 == "Positive", "ER-/HER2+", NA))))
print(table(sub, useNA = "ifany"))
# OS
os <- as.numeric(sub("^\\d+:", "", md$OS_STATUS)) == 1
cat("OS events", sum(os), "\n")
