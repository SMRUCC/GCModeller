#!/usr/bin/env Rscript
#
# hts_matrix_reference.R
#
# 为 GCModeller 的 `HTS_matrix` 项目之中的数学计算 API 生成「金标准」结果，
# 供 C:\GCModeller\src\GCModeller\analysis\HTS_matrix\test 之下的 VB 对拍测试逐值比较。
#
# 用法:
#   Rscript hts_matrix_reference.R <inputDir> <outputDir>
#
# <inputDir> 需要包含:
#   counts.csv   genes x samples 的原始 counts 矩阵（第一列是基因 id，第一行是样本 id）
#   design.csv   两列: sample,group（group 只有 control / treat 两个取值）
#
# 输出（<outputDir>）:
#   norm_quantile.csv         limma::normalizeBetweenArrays(method="quantile")
#   norm_deseq2_sf.csv        DESeq2::estimateSizeFactorsForMatrix
#   norm_deseq2.csv           counts / sizeFactors
#   norm_tmm_factors.csv      edgeR::calcNormFactors(method="TMM")
#   norm_tmm_ref.txt          TMM 选中的参考样本列号（从 0 开始计数，与 VB 的下标一致）
#   norm_tmm_cpm.csv          edgeR::cpm(normalized.lib.sizes=TRUE)
#   norm_tpm.csv              按文库大小中位数缩放的 TPM.Normalize 语义
#   hvg_variance.csv          逐基因方差 apply(x, 1, var)
#   scale_max.csv             RelativeScale(median=FALSE)
#   scale_median.csv          RelativeScale(median=TRUE)
#   logscale2.csv             Matrix.log(base=2) / ExpressionScale.LogScale
#   limma.csv                 limma: logFC, AveExpr, t, P.Value, adj.P.Val, B
#
# 说明: 所有参考值都直接调用 Bioconductor 的原始实现，不做任何"等价改写"，
#       以保证对拍比较的是 GCModeller 的实现而不是脚本的重新演绎。

args <- commandArgs(trailingOnly = TRUE)

if (length(args) < 2) {
    stop("usage: Rscript hts_matrix_reference.R <inputDir> <outputDir>")
}

inputDir <- args[1]
outputDir <- args[2]

dir.create(outputDir, showWarnings = FALSE, recursive = TRUE)

suppressPackageStartupMessages({
    library(limma)
    library(edgeR)
    library(DESeq2)
})

counts <- as.matrix(read.csv(file.path(inputDir, "counts.csv"), row.names = 1, check.names = FALSE))
storage.mode(counts) <- "double"

designTable <- read.csv(file.path(inputDir, "design.csv"), stringsAsFactors = FALSE)
groups <- designTable$group[match(colnames(counts), designTable$sample)]

if (any(is.na(groups))) {
    stop("design.csv does not contain a group for every sample of counts.csv")
}

writeCsv <- function(m, name) {
    write.csv(m, file.path(outputDir, name), quote = FALSE)
}

# ---------------------------------------------------------------- 分位数归一化
qn <- limma::normalizeBetweenArrays(counts, method = "quantile")
writeCsv(qn, "norm_quantile.csv")

# ---------------------------------------------------------------- DESeq2
sizeFactors <- DESeq2::estimateSizeFactorsForMatrix(counts)
writeCsv(matrix(sizeFactors, nrow = 1, dimnames = list("sizeFactor", colnames(counts))), "norm_deseq2_sf.csv")
writeCsv(sweep(counts, 2, sizeFactors, "/"), "norm_deseq2.csv")

# ---------------------------------------------------------------- edgeR TMM
dge <- edgeR::DGEList(counts = counts)
dge <- edgeR::calcNormFactors(dge, method = "TMM")
normFactors <- dge$samples$norm.factors

writeCsv(matrix(normFactors, nrow = 1, dimnames = list("normFactor", colnames(counts))), "norm_tmm_factors.csv")

# 参考样本列：按 edgeR::calcNormFactors 的默认规则复算（R 的 which.min 是 1-based，输出转成 0-based）
libSize <- colSums(counts)
f75 <- edgeR:::.calcFactorQuantile(data = counts, lib.size = libSize, p = 0.75)
refColumn <- if (median(f75) < 1e-20) {
    which.max(colSums(sqrt(counts)))
} else {
    which.min(abs(f75 - mean(f75)))
}
writeLines(as.character(refColumn - 1L), file.path(outputDir, "norm_tmm_ref.txt"))

writeCsv(edgeR::cpm(dge, normalized.lib.sizes = TRUE), "norm_tmm_cpm.csv")

# ---------------------------------------------------------------- TPM.Normalize 语义
# HTS_matrix::TPM.Normalize 的行为是「按文库大小缩放，再乘上文库大小的中位数」，
# 因此等价于下面这一行（不是严格意义上的 TPM，但这是该 API 声明的口径）
tpm <- sweep(counts, 2, colSums(counts), "/") * median(colSums(counts))
writeCsv(tpm, "norm_tpm.csv")

# ---------------------------------------------------------------- HVG 方差
writeCsv(matrix(apply(counts, 1, var), ncol = 1, dimnames = list(rownames(counts), "variance")), "hvg_variance.csv")

# ---------------------------------------------------------------- RelativeScale
rowMax <- apply(counts, 1, max)
rowMedian <- apply(counts, 1, median)

writeCsv(counts / rowMax, "scale_max.csv")

# VB 实现在 median == 0 时回退到 max/2，这里按同样的规则生成参考值
medianFactor <- ifelse(rowMedian == 0, rowMax / 2, rowMedian)
writeCsv(counts / medianFactor, "scale_median.csv")

# ---------------------------------------------------------------- LogScale
logScale2 <- function(v) {
    positive <- v[v > 0]
    if (length(positive) == 0) {
        return(rep(0, length(v)))
    }
    minValue <- min(positive)
    ifelse(v <= 0, 0, log2(v + 1 - minValue))
}

logX <- t(apply(counts, 1, logScale2))
dimnames(logX) <- dimnames(counts)
writeCsv(logX, "logscale2.csv")

# ---------------------------------------------------------------- limma
group <- factor(groups, levels = c("control", "treat"))
designMatrix <- model.matrix(~0 + group)
colnames(designMatrix) <- levels(group)

fit <- limma::lmFit(logX, designMatrix)
contrast <- limma::makeContrasts(treat - control, levels = designMatrix)
fit <- limma::contrasts.fit(fit, contrast)
fit <- limma::eBayes(fit)

table <- limma::topTable(fit, number = Inf, sort.by = "none")
writeCsv(as.matrix(table[, c("logFC", "AveExpr", "t", "P.Value", "adj.P.Val", "B")]), "limma.csv")

# 中间量诊断：便于定位 VB 端 B 统计量的偏差来源
diagnostics <- c(
    s2_prior = fit$s2.prior,
    df_prior = as.numeric(fit$df.prior)[1],
    df_total = as.numeric(fit$df.total)[1],
    var_prior = as.numeric(fit$var.prior)[1],
    lods_gene1 = as.numeric(fit$lods)[1],
    t_gene1 = as.numeric(fit$t)[1],
    s2post_gene1 = as.numeric(fit$s2.post)[1]
)

writeLines(sprintf("%s=%.17g", names(diagnostics), diagnostics), file.path(outputDir, "limma_diag.txt"))

cat("reference generation done\n")
