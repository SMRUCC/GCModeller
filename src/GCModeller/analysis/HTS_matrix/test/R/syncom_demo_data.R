#!/usr/bin/env Rscript
#
# 合成菌群(synthetic community)转录组演示数据的生成器。
#
# 数据设计严格遵循 Nature Communications (2026) 17:11310 的生物学机制与结论：
#   "Herbicide degradation is driven by communication-mediated division of labor
#    in synthetic communities" (Zhang et al.)
#
# 1. 四个菌株：Y1(Comamonas), K1(Acinetobacter), B1(Pseudomonas), H1(Bacillus)
#    两个合成菌群：YKB = Y1 + K1 + B1，HKB = H1 + K1 + B1 (K1/B1 是共享成员)
# 2. 代谢分工：
#    - YKB: K1 是主要降解者(AHL/LuxR 直接调控起始步骤酶 atzA/ethBAD/nsh)，
#           Y1 是能量供给者(糖类与能量代谢、转运体)，
#           B1 是 "代谢清道夫"(色氨酸代谢将中间产物转化为 acetyl-CoA 进入 TCA)
#    - HKB: H1 负责 DNA 修复、AI-2 信号与硫代谢，
#           K1 转向色氨酸代谢与降解协作，
#           B1 成为核心降解者(AI-2/LsrR 调控下游酶 atzC/meaB/fad)
# 3. 功能可塑性：K1 在 YKB 中主导降解、在 HKB 中转向色氨酸代谢；
#    B1 在 YKB 中辅助色氨酸代谢、在 HKB 中成为核心降解者
# 4. QS 调控：YKB 以 C6-HSL/AHL(luxI/luxR)为主，HKB 以 DPD/AI-2(luxS)为主；
#    抑制 AHL 主要打击起始步骤酶，抑制 AI-2 主要打击下游步骤酶
# 5. 四类除草剂(triazines/amides/sulfonylureas/ethers)的关键酶基因在四个菌株中
#    均为同源家族(并集定量时会交叉分配 reads)，这正是矩阵分解需要还原的部分
#
# 用法:
#   Rscript syncom_demo_data.R [outputDir]
# 默认输出到 data/syncom（相对于当前工作目录）。
#
suppressWarnings(suppressMessages({
  invisible(NULL)
}))

args <- commandArgs(trailingOnly = TRUE)
outDir <- if (length(args) >= 1L) args[[1]] else file.path("data", "syncom")

dir.create(outDir, recursive = TRUE, showWarnings = FALSE)
dir.create(file.path(outDir, "truth"), recursive = TRUE, showWarnings = FALSE)

set.seed(20261124)

# ---------------------------------------------------------------------------
# 1. 菌株、基因模块与关键酶
# ---------------------------------------------------------------------------

strains <- c("Y1", "K1", "B1", "H1")
genera <- c(Y1 = "Comamonas", K1 = "Acinetobacter", B1 = "Pseudomonas", H1 = "Bacillus")

modules <- c("herb_initial", "herb_downstream", "qs_ahl", "qs_ai2", "tryptophan",
             "sulfur", "carbohydrate", "dna_repair", "biofilm", "transporter",
             "housekeeping")

# 起始步骤酶(AHL/LuxR 直接结合启动子: atzA, ethBAD, nsh)
enzymes_initial <- c("atzA", "ethBAD", "nsh", "damH", "cmeH", "dpeL", "pmo", "thcBCD")

# 下游步骤酶(AI-2/LsrR 直接结合启动子: atzC, meaB, fad)
enzymes_downstream <- c("atzB", "atzC", "atzD", "atzE", "atzF", "trzA", "triA", "triB",
                        "metS", "metDA", "meaXY", "meaA", "meaB", "cndC1", "nsr", "suh",
                        "rutABC", "asdmH", "nao", "fad", "fah", "sah", "sox", "dnrA")

# 每个菌株的基因组：同源家族基因(酶/QS/核心代谢) + 菌株私有基因(标记基因)
buildGenome <- function(strain) {
  parts <- list()

  # 四类除草剂的关键降解酶：四个菌株都携带(同源家族)
  parts[[length(parts) + 1L]] <- data.frame(
    gene_id = paste0(strain, "_", c(enzymes_initial, enzymes_downstream)),
    module = c(rep("herb_initial", length(enzymes_initial)),
               rep("herb_downstream", length(enzymes_downstream))),
    family_id = c(enzymes_initial, enzymes_downstream),
    stringsAsFactors = FALSE)

  # QS 系统：AI-2(luxS/lsrR/lsrK) 在四个菌株中保守；AHL(luxI/luxR) 只存在于
  # 革兰氏阴性的 Y1/K1/B1（论文: YKB 三个成员均携带完整的 luxI/luxR）
  parts[[length(parts) + 1L]] <- data.frame(
    gene_id = paste0(strain, "_", c("luxS", "lsrR", "lsrK")),
    module = "qs_ai2",
    family_id = c("luxS", "lsrR", "lsrK"),
    stringsAsFactors = FALSE)

  if (strain != "H1") {
    parts[[length(parts) + 1L]] <- data.frame(
      gene_id = paste0(strain, "_", c("luxI", "luxR")),
      module = "qs_ahl",
      family_id = c("luxI", "luxR"),
      stringsAsFactors = FALSE)
  }

  # 色氨酸 / 硫代谢 / DNA 修复的核心基因(同源家族)
  parts[[length(parts) + 1L]] <- data.frame(
    gene_id = paste0(strain, "_", c("trpA", "trpB", "trpC", "trpD", "trpE")),
    module = "tryptophan",
    family_id = c("trpA", "trpB", "trpC", "trpD", "trpE"),
    stringsAsFactors = FALSE)

  parts[[length(parts) + 1L]] <- data.frame(
    gene_id = paste0(strain, "_", c("cysD", "cysJ", "cysN")),
    module = "sulfur",
    family_id = c("cysD", "cysJ", "cysN"),
    stringsAsFactors = FALSE)

  parts[[length(parts) + 1L]] <- data.frame(
    gene_id = paste0(strain, "_", c("recA", "uvrA")),
    module = "dna_repair",
    family_id = c("recA", "uvrA"),
    stringsAsFactors = FALSE)

  # 菌株私有基因(无跨菌株同源 -> 分解时的标记基因)
  private <- function(module, n) {
    data.frame(
      gene_id = paste0(strain, "_", module, "_", seq_len(n)),
      module = module,
      family_id = NA_character_,
      stringsAsFactors = FALSE)
  }

  parts[[length(parts) + 1L]] <- private("tryptophan", 20)
  parts[[length(parts) + 1L]] <- private("sulfur", 12)
  parts[[length(parts) + 1L]] <- private("dna_repair", 10)
  parts[[length(parts) + 1L]] <- private("carbohydrate", 40)
  parts[[length(parts) + 1L]] <- private("transporter", 30)
  parts[[length(parts) + 1L]] <- private("biofilm", 16)
  parts[[length(parts) + 1L]] <- private("herb_downstream", 15)
  parts[[length(parts) + 1L]] <- private("qs_ai2", 15)
  parts[[length(parts) + 1L]] <- private("housekeeping", 120)

  if (strain != "H1") {
    parts[[length(parts) + 1L]] <- private("qs_ahl", 10)
  }

  genome <- do.call(rbind, parts)
  genome$strain <- strain

  genome
}

genes <- do.call(rbind, lapply(strains, buildGenome))
genes$strain <- factor(genes$strain, levels = strains)
genes$module <- factor(genes$module, levels = modules)
rownames(genes) <- NULL

# ---------------------------------------------------------------------------
# 2. 表达模型：菌株活性 x 模块基线 x 条件调控 x 基因权重
# ---------------------------------------------------------------------------

# 每个菌株对各功能模块的基线投入（体现生态位与分工的"底盘"差异）
module_base <- matrix(
  c(# herb_initial, herb_downstream, qs_ahl, qs_ai2, tryptophan, sulfur,
    # carbohydrate, dna_repair, biofilm, transporter, housekeeping
    0.70, 0.70, 0.90, 0.60, 0.80, 0.60, 1.60, 0.70, 0.90, 1.30, 1.00,  # Y1 能量供给者
    1.60, 1.40, 1.50, 0.80, 1.00, 0.70, 1.00, 0.80, 1.00, 1.00, 1.00,  # K1 核心降解者
    1.25, 1.60, 1.30, 0.90, 1.70, 1.00, 1.10, 0.70, 1.10, 1.00, 1.00,  # B1 代谢清道夫
    1.00, 1.00, 0.30, 1.90, 0.90, 1.70, 0.90, 1.80, 1.00, 0.90, 1.00), # H1 硫代谢/DNA修复
  nrow = length(strains), byrow = TRUE,
  dimnames = list(strains, modules))

# 条件调控系数：群落组成(YKB/HKB) x 除草剂胁迫(WT/NH) x QS 状态(WT/QSR)
#
# 说明：本分解流程把每个菌株建模为一个秩1因子 (基因表达 = 基因载荷 x 菌株活性谱)，
# 因此“样本水平”的调控(群落成员、除草剂诱导、QS 抑制)只能编码在菌株活性上。
# 这里按论文的结论把
#   - 除草剂胁迫 -> 降解者的整体降解程序上调
#   - AHL 抑制   -> YKB 中主降解者 K1 的整套代谢程序被压制
#   - AI-2 抑制  -> HKB 中核心降解者 B1 的整套代谢程序被压制
# 编码为对应菌株在对应条件下的活性变化；菌株内部的代谢分工则由 module_base 表达。
activityTable <- local({
  # 条件键 -> 各菌株活性
  #             Y1     K1     B1     H1
  table <- rbind(
    "YKB_WT_WT"  = c(1.00, 1.20, 0.90, 0.012),
    "YKB_WT_QSR" = c(1.15, 0.50, 1.05, 0.012),
    "YKB_NH_WT"  = c(1.10, 0.75, 0.60, 0.012),
    "YKB_NH_QSR" = c(1.20, 0.35, 0.70, 0.012),
    "HKB_WT_WT"  = c(0.012, 1.05, 1.30, 1.05),
    "HKB_WT_QSR" = c(0.012, 1.00, 0.55, 0.78),
    "HKB_NH_WT"  = c(0.012, 0.70, 0.85, 0.90),
    "HKB_NH_QSR" = c(0.012, 0.68, 0.42, 0.75))

  colnames(table) <- strains
  table
})

# 实验设计：两个合成菌群 x 除草剂/无除草剂 x 野生型/QS干扰 x 3 生物学重复
design <- do.call(rbind, lapply(c("YKB", "HKB"), function(consortium) {
  do.call(rbind, lapply(c("WT", "NH"), function(herbicide) {
    do.call(rbind, lapply(c("WT", "QSR"), function(qs) {
      data.frame(
        sample = paste(consortium, herbicide, qs, paste0("R", 1:3), sep = "_"),
        consortium = consortium,
        herbicide = herbicide,
        qs = qs,
        stringsAsFactors = FALSE)
    }))
  }))
}))
rownames(design) <- NULL

samples <- design$sample
nSample <- length(samples)

# 菌株活性：群落中缺失的成员只保留痕量(交叉污染/测序噪声)
activity <- matrix(0, nrow = length(strains), ncol = nSample,
                   dimnames = list(strains, samples))
depth <- rep(1, nSample)
names(depth) <- samples

for (j in seq_len(nSample)) {
  key <- paste(design$consortium[j], design$herbicide[j], design$qs[j], sep = "_")

  activity[, j] <- activityTable[key, ] * exp(rnorm(length(strains), 0, 0.08))
  depth[j] <- exp(rnorm(1, 0, 0.08))
}

# 基因基线权重(中位 counts ~150)
# 基因权重的离散度不能太大：每个功能模块只有几十个基因，过大的基因间离散度会让
# 模块总量偏离设计的“代谢分工”比例，从而无法在分解结果中复现论文结论。
geneWeight <- rlnorm(nrow(genes), meanlog = log(150), sdlog = 0.4)

# 期望表达量：基因载荷(module_base x 基因权重) x 菌株活性谱
mu <- matrix(0, nrow = nrow(genes), ncol = nSample,
             dimnames = list(NULL, samples))
geneStrain <- as.character(genes$strain)
geneModule <- as.character(genes$module)
geneLoad <- module_base[cbind(geneStrain, geneModule)] * geneWeight

for (j in seq_len(nSample)) {
  mu[, j] <- depth[j] * activity[geneStrain, j] * geneLoad
}

# 计数噪声：Poisson 采样 + 基因水平的生物学波动(避免完美的秩1结构)
noise <- matrix(rlnorm(length(mu), 0, 0.12), nrow = nrow(mu))
truth <- matrix(rpois(length(mu), lambda = as.vector(mu * noise)),
                nrow = nrow(mu), dimnames = list(NULL, samples))

# ---------------------------------------------------------------------------
# 3. 并集定量：同源家族内部 reads 交叉分配
# ---------------------------------------------------------------------------

observed <- truth

families <- unique(genes$family_id[!is.na(genes$family_id)])

for (family in families) {
  idx <- which(!is.na(genes$family_id) & genes$family_id == family)
  if (length(idx) < 2L) next

  # 同源基因之间的序列相似度(决定交叉分配的方向与强度)
  similarity <- matrix(runif(length(idx) * length(idx), 0.55, 1.0),
                       nrow = length(idx), ncol = length(idx))
  diag(similarity) <- 0
  prob <- sweep(similarity, 1, rowSums(similarity), "/")  # 行归一: 每个成员的去向分布

  rate <- runif(1, 0.15, 0.35)   # 该家族整体被错误分配的比例

  observed[idx, ] <- (1 - rate) * truth[idx, ] + rate * (t(prob) %*% truth[idx, ])
}

observed <- round(observed)

# ---------------------------------------------------------------------------
# 4. 写出 CSV
# ---------------------------------------------------------------------------

writeMatrix <- function(mat, path) {
  table <- data.frame(geneID = genes$gene_id, mat, check.names = FALSE)
  write.csv(table, path, row.names = FALSE, quote = FALSE)
}

writeMatrix(as.matrix(observed), file.path(outDir, "total.csv"))

# 真实的单菌株表达矩阵(用于评估分解精度)
for (strain in strains) {
  idx <- which(geneStrain == strain)
  table <- data.frame(geneID = genes$gene_id[idx], truth[idx, , drop = FALSE],
                      check.names = FALSE)
  write.csv(table, file.path(outDir, "truth", paste0(strain, ".csv")),
            row.names = FALSE, quote = FALSE)
}

# 基因组(菌株 -> 基因列表)
write.csv(data.frame(strain = geneStrain, gene_id = genes$gene_id,
                     genus = genera[geneStrain], stringsAsFactors = FALSE),
          file.path(outDir, "genomes.csv"), row.names = FALSE, quote = FALSE)

# 基因家族(同源关系)
familyRows <- which(!is.na(genes$family_id))
write.csv(data.frame(family_id = genes$family_id[familyRows],
                     gene_id = genes$gene_id[familyRows],
                     stringsAsFactors = FALSE),
          file.path(outDir, "families.csv"), row.names = FALSE, quote = FALSE)

# 基因注释(菌株 + 功能模块)
annotation <- data.frame(gene_id = genes$gene_id,
                         strain = geneStrain,
                         module = geneModule,
                         family_id = ifelse(is.na(genes$family_id), "",
                                            genes$family_id),
                         stringsAsFactors = FALSE)
write.csv(annotation, file.path(outDir, "annotation.csv"),
          row.names = FALSE, quote = FALSE)

# 实验设计
write.csv(design, file.path(outDir, "design.csv"), row.names = FALSE, quote = FALSE)

cat(sprintf("syncom demo data: %d genes x %d samples\n", nrow(genes), nSample))
cat(sprintf("  strains : %s\n", paste(strains, collapse = ", ")))
cat(sprintf("  samples : %s\n", paste(samples, collapse = ", ")))
cat(sprintf("  families: %d\n", length(families)))
cat(sprintf("  output  : %s\n", normalizePath(outDir)))
