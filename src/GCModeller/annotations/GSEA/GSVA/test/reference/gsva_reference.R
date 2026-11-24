#!/usr/bin/env Rscript
##
## GSVA 金标准参考分数生成脚本
##
## 用 GCModeller 的 VB.NET 实现做准确度比对时，以本脚本产出的分数为准。
##
## 用法:
##   Rscript gsva_reference.R [dataDir]
##
## 输入（均位于 dataDir 下）:
##   ath_norm.csv        表达矩阵，第一列为基因 ID（行名），其余各列为样本
##   gsva_genesets.tsv   基因集长表，两列: pathway / gene（制表符分隔）
##
## 输出（位于 dataDir 下）:
##   gsva_reference.csv / ssgsea_reference.csv / zscore_reference.csv / plage_reference.csv
##   均为 通路 x 样本 的分数矩阵，第一列为通路名
##

suppressMessages(library(GSVA))

args    <- commandArgs(trailingOnly = TRUE)
dataDir <- if (length(args) >= 1) args[1] else "G:/GCModeller/test/demo/HTS/GSVA"

cat("== GSVA 金标准参考分数生成 ==\n")
cat("R     :", R.version.string, "\n")
cat("GSVA  :", as.character(packageVersion("GSVA")), "\n")
cat("data  :", dataDir, "\n\n")

## ---------------------------------------------------------------
## 1. 读取表达矩阵
## ---------------------------------------------------------------
expr <- read.csv(file.path(dataDir, "ath_norm.csv"),
                 row.names = 1, check.names = FALSE)
expr <- as.matrix(expr)
cat("expression matrix:", nrow(expr), "genes x", ncol(expr), "samples\n")

## ---------------------------------------------------------------
## 2. 读取基因集长表
## ---------------------------------------------------------------
sets.long <- read.delim(file.path(dataDir, "gsva_genesets.tsv"),
                        header = TRUE, sep = "\t",
                        stringsAsFactors = FALSE,
                        colClasses = c("character", "character"))

stopifnot(all(c("pathway", "gene") %in% names(sets.long)))

geneSets <- split(sets.long$gene, sets.long$pathway)
cat("gene sets        :", length(geneSets), "\n")
cat("set size         : min", min(lengths(geneSets)),
    "max", max(lengths(geneSets)), "\n\n")

## 基因集成员必须全部能在表达矩阵行名中找到，否则两端的基因全集不一致
missing <- setdiff(unique(sets.long$gene), rownames(expr))
if (length(missing) > 0) {
    stop("genes present in gene sets but missing from expression matrix: ",
         paste(head(missing, 5), collapse = ", "))
}

## ---------------------------------------------------------------
## 3. 逐方法计算参考分数
## ---------------------------------------------------------------
write_ref <- function(es, name) {
    df <- as.data.frame(es)
    df <- cbind(pathway = rownames(df), df, row.names = NULL)
    write.csv(df, file.path(dataDir, paste0(name, "_reference.csv")),
              row.names = FALSE)
    cat(sprintf("  %-8s : %4d x %d  range [%.6f, %.6f]\n",
                name, nrow(es), ncol(es), min(es), max(es)))
}

cat("computing reference scores...\n")

## --- GSVA (Hanzelmann et al. 2013) ---
## kcdf = "Gaussian" 对应连续型数据（log-CPM / microarray），
## tau = 1, maxDiff = TRUE, absRanking = FALSE 均为官方默认值
es <- gsva(gsvaParam(expr, geneSets,
                     kcdf      = "Gaussian",
                     tau       = 1,
                     maxDiff   = TRUE,
                     absRanking = FALSE,
                     minSize   = 1,
                     maxSize   = Inf,
                     sparse    = FALSE,
                     checkNA   = "no"),
           verbose = FALSE)
write_ref(es, "gsva")

## --- ssGSEA (Barbie et al. 2009) ---
## 注意 ssGSEA 不过滤恒定表达行（与官方 removeConstant = FALSE 一致）
es <- gsva(ssgseaParam(expr, geneSets,
                       alpha      = 0.25,
                       normalize  = TRUE,
                       minSize    = 1,
                       maxSize    = Inf,
                       checkNA    = "no"),
           verbose = FALSE)
write_ref(es, "ssgsea")

## --- combined z-score (Lee et al. 2008) ---
es <- gsva(zscoreParam(expr, geneSets,
                       minSize = 1,
                       maxSize = Inf),
           verbose = FALSE)
write_ref(es, "zscore")

## --- PLAGE (Tomfohr et al. 2005) ---
es <- gsva(plageParam(expr, geneSets,
                      minSize = 1,
                      maxSize = Inf),
           verbose = FALSE)
write_ref(es, "plage")

## --- GSVA, kcdf = "none"（不使用核函数，直接经验 CDF） ---
es <- gsva(gsvaParam(expr, geneSets,
                     kcdf      = "none",
                     tau       = 1,
                     maxDiff   = TRUE,
                     absRanking = FALSE,
                     minSize   = 1,
                     maxSize   = Inf,
                     sparse    = FALSE,
                     checkNA   = "no"),
           verbose = FALSE)
write_ref(es, "gsva_ecdf")

cat("\n~all done!\n")
