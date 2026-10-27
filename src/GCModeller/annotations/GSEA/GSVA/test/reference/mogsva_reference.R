#!/usr/bin/env Rscript
##
## moGSVA 基线参考分数生成脚本
##
## 用法:
##   Rscript mogsva_reference.R <dataDir> [--install-baselines]
##
## 输入（由 VB 端写出，位于 <dataDir>/mogsva_input/）:
##   samples.txt              样本 id，一行一个，顺序与矩阵列一致
##   genesets.tsv             基因集长表，两列: pathway / gene
##   layer_<omics>.csv        各组学层的「特征 × 样本」矩阵（首列为特征 id）
##
## 输出（位于 <dataDir>/mogsva_input/reference/）:
##   r_<omics>_scores.csv     各组学层单独做 GSVA 的分数（通路 × 样本）
##   r_baseline_mean.csv      Baseline 1：逐组学 GSVA 后对共同通路/样本取算术平均
##   mogsa_scores.csv         （可选）MOGSA 结果
##   multigsea_scores.csv     （可选）multiGSEA 结果
##   baseline_status.tsv      每个基线的 OK/SKIP 状态与说明
##

suppressMessages(library(GSVA))

args    <- commandArgs(trailingOnly = TRUE)
dataDir <- if (length(args) >= 1) args[1] else "G:/GCModeller/src/GCModeller/annotations/GSEA/data/TCGA_COAD"
installBaselines <- "--install-baselines" %in% args

inputDir <- file.path(dataDir, "mogsva_input")
outDir   <- file.path(inputDir, "reference")
dir.create(outDir, showWarnings = FALSE, recursive = TRUE)

statusFile <- file.path(outDir, "baseline_status.tsv")
if (file.exists(statusFile)) file.remove(statusFile)

status <- function(name, ok, msg) {
  cat(sprintf("%s\t%s\t%s\n", name, if (ok) "OK" else "SKIP", msg),
      file = statusFile, append = TRUE)
}

cat("== moGSVA baseline reference ==\n")
cat("R    :", R.version.string, "\n")
cat("GSVA :", as.character(packageVersion("GSVA")), "\n")
cat("input:", inputDir, "\n\n")

samples <- readLines(file.path(inputDir, "samples.txt"))
sets.long <- read.delim(file.path(inputDir, "genesets.tsv"),
                        header = TRUE, sep = "\t",
                        stringsAsFactors = FALSE,
                        colClasses = c("character", "character"))
geneSets <- split(sets.long$gene, sets.long$pathway)
cat("gene sets :", length(geneSets), "\n")
cat("samples   :", length(samples), "\n\n")

write_ref <- function(es, name) {
  df <- as.data.frame(es)
  df <- cbind(pathway = rownames(df), df, row.names = NULL)
  write.csv(df, file.path(outDir, name), row.names = FALSE)
  cat(sprintf("  %-26s : %4d x %d\n", name, nrow(es), ncol(es)))
}

omics.arg <- grep("^--omics=", args, value = TRUE)
omics <- if (length(omics.arg) > 0) {
  strsplit(sub("^--omics=", "", omics.arg[1]), ",", fixed = TRUE)[[1]]
} else {
  c("mRNA", "protein", "methylation")
}
cat("omics     :", paste(omics, collapse = ", "), "\n\n")

exprs  <- list()
scores <- list()

for (omic in omics) {
  f <- file.path(inputDir, paste0("layer_", omic, ".csv"))

  if (!file.exists(f)) {
    status(omic, FALSE, "missing input matrix")
    next
  }

  expr <- as.matrix(read.csv(f, row.names = 1, check.names = FALSE))
  expr <- expr[, samples, drop = FALSE]
  exprs[[omic]] <- expr

  es <- gsva(gsvaParam(expr, geneSets,
                       kcdf       = "Gaussian",
                       tau        = 1,
                       maxDiff    = TRUE,
                       absRanking = FALSE,
                       minSize    = 1,
                       maxSize    = Inf,
                       sparse     = FALSE,
                       checkNA    = "no"),
             verbose = FALSE)

  write_ref(es, paste0("r_", omic, "_scores.csv"))
  scores[[omic]] <- es
  status(omic, TRUE, sprintf("%d pathways x %d samples", nrow(es), ncol(es)))
}

## ------------------------------------------------ Baseline 1: mean over omics
if (length(scores) > 0) {
  common <- Reduce(intersect, lapply(scores, rownames))
  cols   <- Reduce(intersect, lapply(scores, colnames))
  acc    <- 0

  for (m in scores) acc <- acc + m[common, cols, drop = FALSE]

  avg <- acc / length(scores)
  write_ref(avg, "r_baseline_mean.csv")
  status("baseline_mean", TRUE,
         sprintf("%d pathways averaged over %d omics", length(common), length(scores)))
} else {
  status("baseline_mean", FALSE, "no omics layer was scored")
}

## ------------------------------------------------ optional: MOGSA
install_if_missing <- function(pkg) {
  if (requireNamespace(pkg, quietly = TRUE)) return(TRUE)
  if (!installBaselines) return(FALSE)

  cat(sprintf("installing Bioconductor package '%s' ...\n", pkg))
  ok <- tryCatch({
    BiocManager::install(pkg, ask = FALSE, update = FALSE)
    requireNamespace(pkg, quietly = TRUE)
  }, error = function(e) FALSE)

  return(isTRUE(ok))
}

if (length(exprs) >= 2) {
  if (install_if_missing("mogsa")) {
    ok <- tryCatch({
      ## MOGSA 需要各层共享相同的特征行
      common <- Reduce(intersect, lapply(exprs, rownames))
      mats <- lapply(exprs, function(m) m[common, , drop = FALSE])

      moaObj <- mogsa::moa(mats, proc.row = "center_ssq1", w.data = "lambda1", w.row = NULL)
      ## prepSupMoa 只接受 character / matrix / list，不接受 moa 对象
      sup <- mogsa::prepSupMoa(mats, geneSets, minMatch = 5, maxMatch = 500)
      mg <- mogsa::mogsa(moaObj, sup, nf = 5, ks.stat = FALSE)
      score <- as.matrix(mogsa::getmgsa(mg, "score"))

      if (nrow(score) == 0 || ncol(score) == 0) stop("empty MOGSA score matrix")

      df <- as.data.frame(score)
      df <- cbind(id = rownames(df), df, row.names = NULL)
      write.csv(df, file.path(outDir, "mogsa_scores.csv"), row.names = FALSE)
      cat(sprintf("  %-26s : %4d x %d\n", "mogsa_scores.csv", nrow(score), ncol(score)))
      status("mogsa", TRUE, sprintf("%d gene set(s) x %d sample(s)", nrow(score), ncol(score)))
      TRUE
    }, error = function(e) {
      status("mogsa", FALSE, conditionMessage(e))
      FALSE
    })
  } else {
    status("mogsa", FALSE, "package 'mogsa' is not installed")
  }
} else {
  status("mogsa", FALSE, "MOGSA requires at least two omics layers")
}

## ------------------------------------------------ optional: multiGSEA
## multiGSEA(pathways, ranks) 是"各层分别做 fgsea 再合并 p 值"的队列级基线，
## 无表型标签时以「跨样本变异」作为基因排序统计量。
if (length(exprs) >= 2) {
  if (install_if_missing("multiGSEA")) {
    ok <- tryCatch({
      ranks <- lapply(exprs, function(m) {
        v <- apply(m, 1, stats::var)
        v[is.na(v)] <- 0
        stats::setNames(as.numeric(v), rownames(m))
      })
      pathways <- lapply(names(exprs), function(o) geneSets)
      names(pathways) <- names(exprs)

      res <- multiGSEA::multiGSEA(pathways = pathways, ranks = ranks)
      tab <- NULL

      for (omics in names(res)) {
        df <- as.data.frame(res[[omics]])[, c("pathway", "pval")]
        colnames(df) <- c("pathway", paste0(omics, "_pval"))
        tab <- if (is.null(tab)) df else merge(tab, df, by = "pathway", all = TRUE)
      }

      pcols <- setdiff(colnames(tab), "pathway")
      tab$combined_pval <- apply(tab[, pcols, drop = FALSE], 1, function(p) {
        p <- p[!is.na(p)]
        if (length(p) == 0) return(NA_real_)
        min(1, min(p) * length(p))
      })

      write.csv(tab, file.path(outDir, "multigsea_pvalues.csv"), row.names = FALSE)
      cat(sprintf("  %-26s : %4d pathway(s)\n", "multigsea_pvalues.csv", nrow(tab)))
      status("multiGSEA", TRUE, sprintf("%d pathway(s) x %d omics", nrow(tab), length(pcols)))
      TRUE
    }, error = function(e) {
      status("multiGSEA", FALSE, conditionMessage(e))
      FALSE
    })
  } else {
    status("multiGSEA", FALSE, "package 'multiGSEA' is not installed")
  }
} else {
  status("multiGSEA", FALSE, "multiGSEA requires at least two omics layers")
}

cat("\n~all done!\n")
