suppressMessages({library(data.table)})
RAW <- "K:/brca_tcga_pub2015/multipleOmics"
md <- fread(file.path(RAW, "samples.tsv"))
print(table(md$HER2_FISH_STATUS, md$IHC_HER2, useNA = "ifany"))
cat("\nOS parse:\n")
print(head(md$OS_STATUS)); print(head(md$OS_MONTHS))
cat("\nDFS:\n"); print(table(md$DFS_STATUS, useNA = "ifany"))
cat("\nAGE range:", range(md$AGE), "\n")
cat("\nHIST:", "\n"); print(table(md$HISTOLOGICAL_DIAGNOSIS, useNA = "ifany"))
