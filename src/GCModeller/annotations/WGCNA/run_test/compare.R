#!/usr/bin/env Rscript
# ---------------------------------------------------------------------------
#  GCModeller vs GNU R WGCNA 对照比对脚本
#
#  用法：
#    Rscript compare.R <目录> [报告输出文件]
#
#  期望 <目录> 下存在：
#    GCModeller: cor.csv adj.csv tom.csv  gene_module.csv  timing.csv  summary.txt
#    GNU R     : R_cor.csv R_adj.csv R_tom.csv R_labels.csv R_timing.csv
#                （整表对照时）R_blocks_labels.csv 与 gene_module.csv
#
#  输出：控制台 + <目录>/compare_report.md
# ---------------------------------------------------------------------------

suppressWarnings(suppressMessages({
  library(igraph)
}))

args <- commandArgs(trailingOnly = TRUE)
if (length(args) < 1) stop("usage: Rscript compare.R <dir> [report.md]")

dir <- args[1]
report <- if (length(args) >= 2) args[2] else file.path(dir, "compare_report.md")

readMat <- function(f) {
  as.matrix(read.csv(f, row.names = 1, check.names = FALSE))
}

# 把两个带行列名的矩阵按共同的 gene 顺序对齐后比较
cmpMat <- function(A, B) {
  ids <- intersect(rownames(A), rownames(B))
  ids <- intersect(ids, colnames(A))
  ids <- intersect(ids, colnames(B))
  if (length(ids) < 2) return(NULL)
  Aa <- A[ids, ids, drop = FALSE]
  Bb <- B[ids, ids, drop = FALSE]
  d  <- abs(Aa - Bb)
  ok <- is.finite(d)
  list(
    n        = length(ids),
    maxAbs   = if (any(ok)) max(d[ok]) else NA_real_,
    meanAbs  = if (any(ok)) mean(d[ok]) else NA_real_,
    rmse     = if (any(ok)) sqrt(mean((d[ok]) ^ 2)) else NA_real_,
    pearson  = suppressWarnings(cor(as.vector(Aa[ok]), as.vector(Bb[ok])))
  )
}

# 调整兰德指数（模块划分一致性）
ari <- function(lab1, lab2) {
  ids <- intersect(names(lab1), names(lab2))
  if (length(ids) < 2) return(NA_real_)
  a <- as.character(lab1[ids])
  b <- as.character(lab2[ids])
  # igraph::compare 需要 membership 向量
  suppressWarnings(
    igraph::compare(
      as.integer(factor(a)),
      as.integer(factor(b)),
      method = "adjusted.rand"
    )
  )
}

lines <- c()
say <- function(...) {
  s <- paste0(...)
  cat(s, "\n")
  lines <<- c(lines, s)
}

say("# GCModeller vs GNU R WGCNA 对照报告")
say("")
say("目录: `", dir, "`")
say("生成时间: ", format(Sys.time(), "%Y-%m-%d %H:%M:%S"))
say("")

## ---------------------------------------------------------------- 矩阵数值对照
matNames <- c(cor = "cor", adj = "adj", tom = "tom")

for (nm in names(matNames)) {
  fg <- file.path(dir, paste0(nm, ".csv"))
  fr <- file.path(dir, paste0("R_", nm, ".csv"))

  if (!file.exists(fg) || !file.exists(fr)) {
    say("### ", nm, " 矩阵")
    say("")
    say("_跳过：缺少 `", basename(fg), "` 或 `", basename(fr), "`_")
    say("")
    next
  }

  A <- readMat(fg)
  B <- readMat(fr)
  r <- cmpMat(A, B)

  say("### ", nm, " 矩阵")
  say("")
  if (is.null(r)) {
    say("_无法对齐共同基因_")
  } else {
    say("| 指标 | 值 |")
    say("|---|---|")
    say("| 共同基因数 | ", r$n, " |")
    say("| 最大绝对误差 | ", formatC(r$maxAbs, format = "e", digits = 3), " |")
    say("| 平均绝对误差 | ", formatC(r$meanAbs, format = "e", digits = 3), " |")
    say("| RMSE | ", formatC(r$rmse, format = "e", digits = 3), " |")
    say("| 元素级 Pearson | ", formatC(r$pearson, format = "f", digits = 12), " |")
  }
  say("")
}

## ---------------------------------------------------------------- 模块标签对照
fg <- file.path(dir, "gene_module.csv")
fr <- file.path(dir, "R_labels.csv")

if (file.exists(fg) && file.exists(fr)) {
  g <- read.csv(fg, stringsAsFactors = FALSE, check.names = FALSE)
  r <- read.csv(fr, stringsAsFactors = FALSE, check.names = FALSE)

  lg <- setNames(as.character(g$module), as.character(g$gene))
  lr <- setNames(as.character(r$module), as.character(r$gene))

  say("### 模块划分一致性（单块对照）")
  say("")
  say("| 指标 | GCModeller | GNU R |")
  say("|---|---|---|")
  say("| 基因数 | ", length(lg), " | ", length(lr), " |")
  say("| 模块数 | ", length(unique(lg)), " | ", length(unique(lr)), " |")
  say("| 已标注基因 | ", sum(lg != "0" & lg != "M0"), " | ", sum(lr != "0"), " |")
  say("")
  say("adjusted Rand index = ", formatC(ari(lg, lr), format = "f", digits = 6))
  say("")
}

## ---------------------------------------------------------------- 整表分块对照
fgb <- file.path(dir, "gene_module.csv")
frb <- file.path(dir, "R_blocks_labels.csv")

if (file.exists(fgb) && file.exists(frb)) {
  g <- read.csv(fgb, stringsAsFactors = FALSE, check.names = FALSE)
  r <- read.csv(frb, stringsAsFactors = FALSE, check.names = FALSE)

  lg <- setNames(as.character(g$module), as.character(g$gene))
  lr <- setNames(as.character(r$module), as.character(r$gene))

  say("### 模块划分一致性（整表分块 blockwise 对照）")
  say("")
  say("| 指标 | GCModeller | GNU R |")
  say("|---|---|---|")
  say("| 基因数 | ", length(lg), " | ", length(lr), " |")
  say("| 模块数 | ", length(unique(lg)), " | ", length(unique(lr)), " |")
  say("| 已标注基因 | ", sum(lg != "0"), " | ", sum(lr != "0"), " |")
  say("")
  say("adjusted Rand index = ", formatC(ari(lg, lr), format = "f", digits = 6))
  say("")
}

## ---------------------------------------------------------------- 耗时对照
cat("\n")

ft <- file.path(dir, "timing.csv")
fr <- file.path(dir, "R_timing.csv")

if (file.exists(ft)) {
  tt <- read.csv(ft, stringsAsFactors = FALSE)
  say("### GCModeller 分段耗时（毫秒）")
  say("")
  say("| 阶段 | 毫秒 |")
  say("|---|---|")
  for (i in seq_len(nrow(tt))) say("| ", tt$stage[i], " | ", tt$ms[i], " |")
  say("")
}

if (file.exists(fr)) {
  rt <- read.csv(fr, stringsAsFactors = FALSE)
  say("### GNU R 分段耗时（秒）")
  say("")
  say("| 阶段 | 秒 |")
  say("|---|---|")
  for (i in seq_len(nrow(rt))) say("| ", rt$stage[i], " | ", rt$secs[i], " |")
  say("")
}

writeLines(lines, report)
cat("report ->", report, "\n")
