# KOBAS 统计检验的 R 参考实现（用于与 VB.NET 移植代码对拍）
#
# 用法： Rscript kobas_reference.R <input.tsv> <output_dir>
#
# input.tsv 的列： id a b c d m1 n1 m2 n2
#   a/b/c/d 为标准 2x2 列联表（按行填充）
#   m1/n1   为前景命中数与前景基因总数
#   m2/n2   为背景命中数与背景基因总数
#
# 输出：
#   stats_r.tsv  : id fisher chisq binom
#   adjust_r.tsv : BH BY（对 stats_r.tsv 的 fisher 列做 p.adjust）

args <- commandArgs(trailingOnly = TRUE)

if (length(args) < 2) {
  stop("usage: Rscript kobas_reference.R <input.tsv> <output_dir>")
}

infile <- args[1]
outdir <- args[2]

table_in <- read.delim(infile, stringsAsFactors = FALSE)

fisher_p <- function(a, b, c, d) {
  fisher.test(matrix(c(a, b, c, d), ncol = 2, byrow = TRUE), alternative = "greater")$p.value
}

chisq_p <- function(a, b, c, d) {
  suppressWarnings(chisq.test(matrix(c(a, b, c, d), ncol = 2, byrow = TRUE))$p.value)
}

binom_p <- function(m1, n1, m2, n2) {
  binom.test(m1, n1, p = m2 / n2, alternative = "greater")$p.value
}

result <- data.frame(
  id = table_in$id,
  fisher = mapply(fisher_p, table_in$a, table_in$b, table_in$c, table_in$d),
  chisq = mapply(chisq_p, table_in$a, table_in$b, table_in$c, table_in$d),
  binom = mapply(binom_p, table_in$m1, table_in$n1, table_in$m2, table_in$n2)
)

write.table(result, file.path(outdir, "stats_r.tsv"), sep = "\t", quote = FALSE, row.names = FALSE)

adjust <- data.frame(
  BH = p.adjust(result$fisher, method = "BH"),
  BY = p.adjust(result$fisher, method = "BY")
)

write.table(adjust, file.path(outdir, "adjust_r.tsv"), sep = "\t", quote = FALSE, row.names = FALSE)
