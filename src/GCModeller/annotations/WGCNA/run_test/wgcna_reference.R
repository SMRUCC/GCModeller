#!/usr/bin/env Rscript
# ---------------------------------------------------------------------------
#  GNU R WGCNA 对照脚本
#
#  用法：
#    Rscript wgcna_reference.R <矩阵.csv> <输出目录> [topN] [power] [blockSize]
#
#  阶段 A（单块数值对照）：
#    按方差取 topN 个基因，计算 cor / adjacency / TOM / 模块标签并导出，
#    用于与 GCModeller 做逐元素比对。
#  阶段 B（整表分块对照）：
#    用 WGCNA::blockwiseModules 在全表上跑分块 WGCNA 并导出模块标签。
#
#  输出：
#    A: R_cor.csv / R_adj.csv / R_tom.csv / R_labels.csv
#    B: R_blocks_labels.csv
#    R_timing.csv
# ---------------------------------------------------------------------------

suppressWarnings(suppressMessages({
  library(WGCNA)
  library(dynamicTreeCut)
}))

args <- commandArgs(trailingOnly = TRUE)
if (length(args) < 2) {
  stop("usage: Rscript wgcna_reference.R <expr.csv> <outdir> [topN] [power] [blockSize]")
}

file      <- args[1]
outdir    <- args[2]
topN      <- if (length(args) >= 3) as.integer(args[3]) else 3000
power     <- if (length(args) >= 4) as.numeric(args[4]) else 6
blockSize <- if (length(args) >= 5) as.integer(args[5]) else 5000

dir.create(outdir, recursive = TRUE, showWarnings = FALSE)

options(stringsAsFactors = FALSE)
allowWGCNAThreads()

timing <- list()

cat("reading", file, "...\n")
t0 <- Sys.time()
X <- as.matrix(read.csv(file, row.names = 1, check.names = FALSE))
timing[["load"]] <- as.numeric(difftime(Sys.time(), t0, units = "secs"))
cat("  matrix:", nrow(X), "genes x", ncol(X), "samples\n")

# 方差过滤：与 GCModeller 的 GeneFilter.ByVariance 一致（样本方差，降序取前 N，保持原顺序）
varFilt <- function(M, n) {
  if (is.null(n) || n <= 0 || n >= nrow(M)) return(M)
  v <- apply(M, 1, var, na.rm = TRUE)
  v[is.na(v)] <- 0
  ord <- order(-v)[1:n]
  M[sort(ord), , drop = FALSE]
}

## =========================================================================
## 阶段 A：单块数值对照
## =========================================================================
cat("\n== stage A: single block numeric reference ==\n")

t0 <- Sys.time()
Xa <- varFilt(X, topN)
cat("  subset:", nrow(Xa), "genes\n")

# WGCNA 要求基因在列、样本在行
exprA <- t(Xa)
geneA <- rownames(Xa)

corA  <- cor(exprA, use = "p")               # 基因 x 基因 Pearson
adjA  <- abs(corA) ^ power                   # unsigned 软阈值邻接（R 的 adjacency 会把对角线置 0）
diag(adjA) <- 0
tomA  <- TOMsimilarity(adjA, TOMType = "unsigned", TOMDenom = "min")
dimnames(tomA) <- list(geneA, geneA)
diag(tomA) <- 1

distA <- 1 - tomA
hcA   <- hclust(as.dist(distA), method = "average")
labA  <- cutreeDynamic(hcA,
                       cutHeight      = 0.995,
                       minClusterSize = 20,
                       method         = "hybrid",
                       deepSplit      = 2,
                       distM          = distA,
                       verbose        = 0)
timing[["stageA"]] <- as.numeric(difftime(Sys.time(), t0, units = "secs"))
cat("  modules:", length(unique(labA[labA != 0])), "\n")

write.csv(corA, file.path(outdir, "R_cor.csv"))
write.csv(adjA, file.path(outdir, "R_adj.csv"))
write.csv(tomA, file.path(outdir, "R_tom.csv"))
write.csv(data.frame(gene = geneA, module = labA),
          file.path(outdir, "R_labels.csv"), row.names = FALSE)

## =========================================================================
## 阶段 B：整表分块对照
## =========================================================================
cat("\n== stage B: blockwiseModules on the full matrix ==\n")

t0 <- Sys.time()
exprB <- t(X)
geneB <- colnames(exprB)

set.seed(54321)
bw <- blockwiseModules(
  exprB,
  maxBlockSize        = blockSize,
  power               = power,
  networkType         = "unsigned",
  TOMType             = "unsigned",
  TOMDenom            = "min",
  deepSplit           = 2,
  detectCutHeight     = 0.995,
  minModuleSize       = 20,
  mergeCutHeight      = 0.15,
  numericLabels       = TRUE,
  randomSeed          = 54321,
  verbose             = 3,
  nThreads            = 0
)
timing[["stageB"]] <- as.numeric(difftime(Sys.time(), t0, units = "secs"))

labB <- bw$colors
cat("  modules:", length(unique(labB[labB != 0])), "\n")

write.csv(data.frame(gene = geneB, module = labB),
          file.path(outdir, "R_blocks_labels.csv"), row.names = FALSE)

write.csv(data.frame(stage = names(timing),
                     secs  = as.numeric(unlist(timing))),
          file.path(outdir, "R_timing.csv"), row.names = FALSE)

cat("\ndone.\n")
