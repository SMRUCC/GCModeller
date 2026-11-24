#!/usr/bin/env Rscript
#
# 合成菌群(synthetic community)转录组演示数据的生成器 + limma/GSVA 通路验证。
#
# 数据设计遵循 Nature Communications (2026) 17:11310 的生物学机制与结论：
#   "Herbicide degradation is driven by communication-mediated division of labor
#    in synthetic communities" (Zhang et al.)
#
# 1. 四个菌株：Y1(Comamonas), K1(Acinetobacter), B1(Pseudomonas), H1(Bacillus)
#    两个合成菌群：YKB = Y1 + K1 + B1，HKB = H1 + K1 + B1 (K1/B1 是共享成员)
# 2. 代谢分工：
#    - YKB: K1 是主要降解者(起始步骤酶 atzA/ethBAD/nsh)；Y1 是能量供给者
#           (糖类与能量代谢、转运体)；B1 是 "代谢清道夫"(色氨酸代谢 -> acetyl-CoA -> TCA)
#    - HKB: H1 负责硫代谢、DNA 修复与 AI-2 信号；K1 与 B1 共同承担降解(B1 为核心)
# 3. QS 调控：YKB 以 C6-HSL/AHL(luxI/luxR)为主，HKB 以 DPD/AI-2(luxS)为主；
#    AHL 抑制主要压制 K1 的降解程序，AI-2 抑制主要压制 B1 的降解程序
# 4. 每个基因都带有 KEGG 直系同源(KO)编号与 KEGG 通路注释；同源家族成员共享 KO。
#
# 关于 KO 编号：脚本使用 KEGG Orthology 的编号命名空间(K#####)。中心代谢、QS、
# 硫代谢、DNA 修复等使用公开的经典 KO；除草剂降解酶使用 map00791/map00627/map00983
# 的 KO 区间作为示例编号。离线环境下无法与 KEGG 最新版本逐条核对，示例数据只保证
# “KO <-> 通路” 的映射关系内部一致；正式分析请以 KEGG 数据库的最新注释为准。
#
# 分析流程：
#   并集表达矩阵 -> limma(差异表达) + GSVA(KEGG 通路打分)
#   -> 与论文结论逐条比对 -> 不满足时自动微调 module_base 并重新模拟(校准循环)
#   -> 收敛后写出 CSV
#
# 输出文件(数据 + 分析结果)：
#   total.csv               并集定量的总表达矩阵(分解流程的输入)
#   truth/<strain>.csv      真实的单菌株表达矩阵(评估分解精度)
#   genomes.csv             菌株 -> 基因(含 KO / KEGG 通路注释)
#   families.csv            跨菌株同源基因家族(含共享 KO)
#   annotation.csv          基因注释(模块 / 家族 / KO / 通路)
#   pathways.csv            KEGG 通路表(基因数 + KO 列表)
#   design.csv              实验设计
#   module_base.csv         校准后的各菌群模块组成(可复现性)
#   gsva_community.csv      群落水平的 GSVA 通路得分(通路 x 样本)
#   gsva_strain.csv         菌株水平的 GSVA 通路得分(按分组取均值)
#   gsva_conclusions.csv    GSVA 结论表(是否复现论文机制)
#   limma_summary.csv       limma 差异表达汇总
#   limma_de_hits.csv       limma 差异表达显著基因(|log2FC| >= 1, adj.P < 0.05)
#
# 用法:
#   Rscript syncom_demo_data.R [outputDir]
# 默认输出到 data/syncom（相对于当前工作目录）。
#
suppressPackageStartupMessages({
  library(limma)
  library(GSVA)
})

args <- commandArgs(trailingOnly = TRUE)
outDir <- if (length(args) >= 1L) args[[1]] else file.path("data", "syncom")

dir.create(outDir, recursive = TRUE, showWarnings = FALSE)
dir.create(file.path(outDir, "truth"), recursive = TRUE, showWarnings = FALSE)

SEED <- 20261124

# ---------------------------------------------------------------------------
# 1. 菌株、功能模块、KEGG 通路与 KO
# ---------------------------------------------------------------------------

# 分离株编号(用于 locus tag 前缀) 与 菌株学名全名(strain id)
isolates <- c("Y1", "K1", "B1", "H1")
strainName <- c(
  Y1 = "Comamonas sp. Y1",
  K1 = "Acinetobacter sp. K1",
  B1 = "Pseudomonas sp. B1",
  H1 = "Bacillus sp. H1")
genera <- c(Y1 = "Comamonas", K1 = "Acinetobacter", B1 = "Pseudomonas", H1 = "Bacillus")

# 内部计算使用分离株编号，写出 CSV 时统一转换为学名
strains <- isolates
strainLabel <- function(isolate) unname(strainName[isolate])

modules <- c("herb_initial", "herb_downstream", "qs_ahl", "qs_ai2", "tryptophan",
             "sulfur", "carbohydrate", "dna_repair", "biofilm", "transporter",
             "housekeeping")

# KEGG 通路(通路ID -> 名称)
pathwayName <- c(
  map00791 = "Atrazine degradation",
  map00627 = "Aminobenzoate degradation",
  map00983 = "Drug metabolism - other enzymes",
  map00380 = "Tryptophan metabolism",
  map00400 = "Phenylalanine, tyrosine and tryptophan biosynthesis",
  map00920 = "Sulfur metabolism",
  map00270 = "Cysteine and methionine metabolism",
  map00010 = "Glycolysis / Gluconeogenesis",
  map00020 = "Citrate cycle (TCA cycle)",
  map00190 = "Oxidative phosphorylation",
  map03440 = "Homologous recombination",
  map03430 = "Mismatch repair",
  map03420 = "Nucleotide excision repair",
  map02024 = "Quorum sensing",
  map02025 = "Biofilm formation",
  map02010 = "ABC transporters",
  map03010 = "Ribosome",
  map00970 = "Aminoacyl-tRNA biosynthesis")

ko <- function(from, to) sprintf("K%05d", seq.int(from, to))

# KEGG 通路的基因池：基因符号(symbol) 与 KO 编号按位对应。
#
# 除草剂降解酶的符号取自论文 Fig. 3(atzA/atzB/.../dnrA)，其余功能使用对应通路的
# 经典基因符号(glk/pgi/.../rpoB/...)，这样每个基因的 gene_id 都有可直接解读的
# 生物学名称，而不是合成的模块序号。
pathwayPool <- list(
  map00791 = data.frame(
    symbol = c("atzA", "ethBAD", "nsh", "damH", "cmeH", "dpeL", "pmo", "thcBCD",
               "atzB", "atzC", "atzD", "atzE", "atzF", "trzA", "triA", "triB",
               "metS", "metDA"),
    KO = ko(3382, 3399), stringsAsFactors = FALSE),
  map00627 = data.frame(
    symbol = c("meaXY", "meaA", "meaB", "cndC1", "asdmH", "thcA", "thcB", "thcC",
               "thcD", "ethA", "ethB", "ethC", "dpeA", "dpeB"),
    KO = ko(1447, 1460), stringsAsFactors = FALSE),
  map00983 = data.frame(
    symbol = c("nsr", "suh", "rutABC", "nao", "fad", "fah", "sah", "sox", "dnrA",
               "trzB", "hdxA", "atdA"),
    KO = ko(1211, 1222), stringsAsFactors = FALSE),
  map00380 = data.frame(
    symbol = c("trpA", "trpB", "trpC", "trpD", "trpE", "trpF", "trpG", "tnaA",
               "kynA", "kynB", "iaaH", "iac", "aroH", "trpS", "tyrA", "pheA"),
    KO = ko(1695, 1710), stringsAsFactors = FALSE),
  map00400 = data.frame(
    symbol = c("aroA", "aroB", "aroC", "aroD", "aroE", "aroK", "aroL", "aroF",
               "aroG", "aroK2", "pheA2", "tyrA2", "trpE2", "trpD2"),
    KO = ko(1657, 1670), stringsAsFactors = FALSE),
  map00920 = data.frame(
    symbol = c("cysD", "cysN", "cysC", "cysH", "cysJ", "cysI", "cysK", "cysE",
               "sat", "tauA", "tauB", "tauC", "soxA", "soxB", "soxC", "soxD",
               "soxR", "ssuD", "ssuE", "sir"),
    KO = ko(956, 975), stringsAsFactors = FALSE),
  map00270 = data.frame(
    symbol = c("cysK2", "cysE2", "metA", "metB", "metC", "mdeA", "mddA", "mdeB",
               "sahH", "metH", "metE", "msrA", "mtr"),
    KO = ko(1738, 1750), stringsAsFactors = FALSE),
  map00010 = data.frame(
    symbol = c("glk", "pgi", "pfkA", "fbaA", "tpiA", "gapA", "pgk", "gpmA",
               "eno", "pykF"),
    KO = c("K00845", "K01810", "K00850", "K01624", "K01803", "K00134",
           "K00927", "K01834", "K01689", "K00873"), stringsAsFactors = FALSE),
  map00020 = data.frame(
    symbol = c("gltA", "acnB", "icd", "sucA", "sucB", "sucC", "sucD", "sdhA",
               "sdhB", "fumA", "fumC", "mdh", "aceE", "pckA"),
    KO = c("K01647", "K01681", "K00031", "K00164", "K00658", "K01902", "K01903",
           "K00239", "K00240", "K01676", "K01679", "K00024", "K00163", "K01610"),
    stringsAsFactors = FALSE),
  map00190 = data.frame(
    symbol = c("atpD", "atpF", "atpA", "atpB", "atpE", "atpG"),
    KO = c("K02112", "K02109", "K02117", "K02118", "K02298", "K02274"),
    stringsAsFactors = FALSE),
  map03440 = data.frame(
    symbol = c("recA", "recB", "recC", "recD", "ssb", "ruvA", "ruvB", "ruvC"),
    KO = c("K03553", "K03582", "K03583", "K03581", "K03111", "K03550",
           "K03551", "K01134"), stringsAsFactors = FALSE),
  map03430 = data.frame(
    symbol = c("mutS", "mutL", "mutH", "uvrD"),
    KO = c("K03555", "K03572", "K06223", "K03657"), stringsAsFactors = FALSE),
  map03420 = data.frame(
    symbol = c("uvrA", "uvrB", "uvrC"),
    KO = c("K03701", "K03702", "K03703"), stringsAsFactors = FALSE),
  map02024 = data.frame(
    symbol = c("luxS", "luxI", "luxR", "lsrR", "lsrK"),
    KO = c("K07173", "K13060", "K13061", "K11531", "K11532"),
    stringsAsFactors = FALSE),
  map02025 = data.frame(
    symbol = c("pelA", "pelB", "pelC", "pelD", "pelE", "pelF", "pelG", "algD",
               "algA", "algE", "algI", "algJ", "algK", "algL", "epsE"),
    KO = ko(20276, 20290), stringsAsFactors = FALSE),
  map02010 = data.frame(
    symbol = c("abcA", "abcB", "abcC", "abcD", "mdtA", "mdtB", "mdtC", "mdtD",
               "tonB", "ompA", "ompC", "ompF", "lamB", "malE", "malF", "malG"),
    KO = ko(2035, 2050), stringsAsFactors = FALSE),
  map03010 = data.frame(
    symbol = c("rpoA", "rpoB", "rpoC", "rpoD", "rpsA", "rpsB", "rplA", "rplB",
               "rpsC", "rplC"),
    KO = c("K03040", "K03043", "K03046", "K03086", "K02945", "K02967",
           "K02863", "K02886", "K02904", "K02911"), stringsAsFactors = FALSE),
  map00970 = data.frame(
    symbol = c("lysS", "argS", "gltX", "tyrS", "thrS"),
    KO = c("K01866", "K01867", "K01868", "K01869", "K01870"),
    stringsAsFactors = FALSE))

# 基因符号 -> (KO, 通路) 的全局索引
symbolIndex <- local({
  table <- do.call(rbind, lapply(names(pathwayPool), function(p) {
    data.frame(symbol = pathwayPool[[p]]$symbol,
               KO = pathwayPool[[p]]$KO,
               pathway = p,
               stringsAsFactors = FALSE)
  }))
  table[!duplicated(table$symbol), ]
})

# KO -> 基因符号
koSymbol <- local({
  table <- symbolIndex$symbol
  names(table) <- symbolIndex$KO
  table
})

# 功能模块 -> 通路(私有基因按照这个顺序轮转分配 KO/通路)
modulePathways <- list(
  herb_initial = c("map00791"),
  herb_downstream = c("map00791", "map00627", "map00983"),
  qs_ahl = c("map02024"),
  qs_ai2 = c("map02024"),
  tryptophan = c("map00380", "map00400"),
  sulfur = c("map00920", "map00270"),
  carbohydrate = c("map00010", "map00020", "map00190"),
  dna_repair = c("map03440", "map03430", "map03420"),
  biofilm = c("map02025"),
  transporter = c("map02010"),
  housekeeping = c("map03010", "map00970"))

# 同源基因家族：家族名 -> 模块(四个菌株共享同一 KO/通路)
families <- list(
  herb_initial = c("atzA", "ethBAD", "nsh", "damH", "cmeH", "dpeL", "pmo", "thcBCD"),
  herb_downstream = c("atzB", "atzC", "atzD", "atzE", "atzF", "trzA", "triA", "triB",
                      "metS", "metDA", "meaXY", "meaA", "meaB", "cndC1", "nsr", "suh",
                      "rutABC", "asdmH", "nao", "fad", "fah", "sah", "sox", "dnrA"),
  tryptophan = c("trpA", "trpB", "trpC", "trpD", "trpE", "trpF", "trpG", "tnaA",
                 "kynA", "iaaH"),
  sulfur = c("cysD", "cysN", "cysC", "cysH", "cysJ", "cysI", "cysK", "cysE", "sat",
             "tauA", "tauB", "soxA"),
  dna_repair = c("recA", "recB", "recC", "uvrA", "uvrB", "uvrC", "uvrD", "mutS",
                 "mutL", "ruvA"),
  biofilm = c("pelA", "pelB", "algD", "epsE"),
  carbohydrate = c("gapA", "pgk", "eno", "pykF", "gltA", "acnB", "icd", "sucA",
                   "sdhA", "fumA", "mdh", "atpD", "atpF", "aceE", "pckA", "fbaA"),
  transporter = c("abcA", "abcB", "abcC", "abcD", "tonB", "ompA", "mdtA", "mdtB"),
  qs_ai2 = c("luxS", "lsrR", "lsrK"),
  qs_ahl = c("luxI", "luxR"))

# 每个家族的 KO/通路：直接按基因符号反查，保证 “基因符号 <-> KO <-> 通路” 三者一致，
# 并且同源家族的四个成员共享同一个 KO 与基因符号
familyAnnotation <- local({
  table <- data.frame(family_id = character(), module = character(),
                      KO = character(), pathway = character(), stringsAsFactors = FALSE)
  for (mo in names(families)) {
    for (symbol in families[[mo]]) {
      hit <- symbolIndex[symbolIndex$symbol == symbol, ]

      if (nrow(hit) == 0L) {
        stop(sprintf("missing KO annotation for gene symbol '%s'", symbol))
      }

      table <- rbind(table, data.frame(family_id = symbol, module = mo,
                                       KO = hit$KO[1], pathway = hit$pathway[1],
                                       stringsAsFactors = FALSE))
    }
  }
  table
})

# 每个菌株的私有基因数量(标记基因来源，数量越多越接近真实基因组的规模)
privateSize <- c(housekeeping = 600, carbohydrate = 150, transporter = 120,
                 tryptophan = 70, sulfur = 45, dna_repair = 40, biofilm = 55,
                 qs_ai2 = 45, qs_ahl = 30, herb_downstream = 45, herb_initial = 20)

# ---------------------------------------------------------------------------
# 2. 基因组构建(基因 -> 模块/家族/KO/通路)
# ---------------------------------------------------------------------------

buildGenome <- function(strain) {
  parts <- list()

  # 同源家族基因(四类除草剂关键酶、QS、中心代谢等重要功能)
  # gene_id = 分离株编号 + 基因符号(与 NCBI 原核基因组 locus_tag 的命名习惯一致)
  homolog <- familyAnnotation
  homolog <- homolog[!(homolog$module == "qs_ahl" & strain == "H1"), ]  # H1 无 AHL

  parts[[length(parts) + 1L]] <- data.frame(
    gene_id = paste0(strain, "_", homolog$family_id),
    symbol = homolog$family_id,
    module = homolog$module,
    family_id = homolog$family_id,
    KO = homolog$KO,
    pathway = homolog$pathway,
    stringsAsFactors = FALSE)

  # 私有基因(无跨菌株同源 -> 分解时的标记基因)：
  # 按模块的基因池轮转分配 (基因符号, KO, 通路)；同一菌株内重复的符号按旁系同源编号
  for (mo in names(privateSize)) {
    n <- privateSize[[mo]]
    if (n <= 0L) next
    if (mo == "qs_ahl" && strain == "H1") next

    pool <- do.call(rbind, lapply(modulePathways[[mo]], function(p) {
      data.frame(symbol = pathwayPool[[p]]$symbol,
                 KO = pathwayPool[[p]]$KO,
                 pathway = p,
                 stringsAsFactors = FALSE)
    }))

    idx <- ((seq_len(n) - 1L) %% nrow(pool)) + 1L
    symbols <- pool$symbol[idx]

    parts[[length(parts) + 1L]] <- data.frame(
      gene_id = paste0(strain, "_", symbols),
      symbol = symbols,
      module = mo,
      family_id = NA_character_,
      KO = pool$KO[idx],
      pathway = pool$pathway[idx],
      stringsAsFactors = FALSE)
  }

  genome <- do.call(rbind, parts)

  # 保证同一菌株内 gene_id 唯一：同符号的拷贝按旁系同源编号(atzA, atzA_2, atzA_3 ...)
  ids <- genome$gene_id
  counter <- new.env(parent = emptyenv())

  for (i in seq_along(ids)) {
    key <- ids[i]
    n <- if (is.null(counter[[key]])) 1L else counter[[key]] + 1L
    counter[[key]] <- n

    if (n > 1L) {
      ids[i] <- paste0(key, "_", n)
    }
  }

  genome$gene_id <- ids
  genome$strain <- strain

  genome
}

genes <- do.call(rbind, lapply(strains, buildGenome))
genes$strain <- factor(genes$strain, levels = strains)
genes$module <- factor(genes$module, levels = modules)
rownames(genes) <- NULL

# ---------------------------------------------------------------------------
# 3. 表达模型：菌株活性谱 x 模块基线(代谢分工) x 基因权重
# ---------------------------------------------------------------------------

# 每个菌株对各功能模块的基线投入(体现生态位与分工的"底盘"差异)。
#
# 两个合成菌群各自维护一套模块组成：K1 与 B1 是共享成员，但它们的功能程序会随
# 群落语境切换(论文中的 functional plasticity):
#   YKB -> K1 主导起始步骤酶与 AHL、B1 负责色氨酸与下游处理
#   HKB -> B1 成为核心降解者、K1 转向色氨酸/协同降解
buildModuleBase <- function() {
  make <- function(values) {
    matrix(values, nrow = length(strains), byrow = TRUE,
           dimnames = list(strains, modules))
  }

  list(
    YKB = make(c(# herb_initial, herb_downstream, qs_ahl, qs_ai2, tryptophan, sulfur,
      # carbohydrate, dna_repair, biofilm, transporter, housekeeping
      0.70, 0.70, 0.90, 0.60, 0.80, 0.60, 1.60, 0.70, 0.90, 1.30, 1.00,  # Y1 能量供给者
      1.55, 1.40, 1.50, 0.80, 1.00, 0.70, 1.00, 0.80, 1.00, 1.00, 1.00,  # K1 主降解者
      1.20, 1.50, 1.30, 0.90, 1.70, 1.00, 1.00, 0.70, 1.10, 1.00, 1.00,  # B1 代谢清道夫
      1.00, 1.00, 0.30, 1.90, 0.90, 1.20, 0.70, 1.80, 1.00, 0.90, 1.00)),# H1 (缺失)
    HKB = make(c(
      0.70, 0.70, 0.90, 0.60, 0.80, 0.60, 1.60, 0.70, 0.90, 1.30, 1.00,  # Y1 (缺失)
      1.30, 1.25, 1.20, 0.80, 1.35, 0.70, 1.00, 0.80, 1.00, 1.00, 1.00,  # K1 协同降解
      1.75, 1.85, 1.30, 0.90, 1.35, 1.00, 1.00, 0.70, 1.10, 1.00, 1.00,  # B1 核心降解者
      1.00, 1.00, 0.30, 1.90, 0.90, 1.70, 0.70, 1.80, 1.00, 0.90, 1.00)) # H1 硫代谢/DNA修复
  )
}

moduleBase <- buildModuleBase()

# 条件调控：把“样本水平”的调控(除草剂诱导、QS 抑制、群落成员)编码为菌株活性谱；
# 菌株内部的代谢分工由上面的 module_base 表达。
#             Y1     K1     B1     H1
activityTable <- rbind(
  "YKB_WT_WT"  = c(1.00, 1.20, 0.90, 0.012),
  "YKB_WT_QSR" = c(1.15, 0.50, 1.05, 0.012),
  "YKB_NH_WT"  = c(1.10, 0.75, 0.60, 0.012),
  "YKB_NH_QSR" = c(1.20, 0.35, 0.70, 0.012),
  "HKB_WT_WT"  = c(0.012, 1.05, 1.30, 1.05),
  "HKB_WT_QSR" = c(0.012, 1.00, 0.55, 0.78),
  "HKB_NH_WT"  = c(0.012, 0.70, 0.85, 0.90),
  "HKB_NH_QSR" = c(0.012, 0.68, 0.42, 0.75))
colnames(activityTable) <- strains

# 实验设计：两个合成菌群 x 除草剂/无除草剂 x 野生型/QS干扰 x 3 重复
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
geneStrain <- as.character(genes$strain)
geneModule <- as.character(genes$module)

# 模拟一次实验(给定 module_base / activityTable / 随机种子)
simulateCommunity <- function(base, activity, seed) {
  set.seed(seed)

  activityMat <- matrix(0, nrow = length(strains), ncol = nSample,
                        dimnames = list(strains, samples))
  depth <- rep(1, nSample)
  names(depth) <- samples

  for (j in seq_len(nSample)) {
    key <- paste(design$consortium[j], design$herbicide[j], design$qs[j], sep = "_")
    activityMat[, j] <- activity[key, ] * exp(rnorm(length(strains), 0, 0.08))
    depth[j] <- exp(rnorm(1, 0, 0.08))
  }

  # 基因权重：模块内部离散度不宜过大，否则模块总量会偏离设计的分工比例
  geneWeight <- rlnorm(nrow(genes), meanlog = log(150), sdlog = 0.4)

  mu <- matrix(0, nrow = nrow(genes), ncol = nSample,
               dimnames = list(NULL, samples))
  for (j in seq_len(nSample)) {
    # 每个菌群各自一套模块组成(功能可塑性)
    load <- base[[design$consortium[j]]][cbind(geneStrain, geneModule)] * geneWeight
    mu[, j] <- depth[j] * activityMat[geneStrain, j] * load
  }

  # 计数噪声：Poisson 采样 + 基因水平的生物学波动
  noise <- matrix(rlnorm(length(mu), 0, 0.12), nrow = nrow(mu))
  truth <- matrix(rpois(length(mu), lambda = as.vector(mu * noise)),
                  nrow = nrow(mu), dimnames = list(NULL, samples))

  # 并集定量：同源家族内部 reads 交叉分配
  observed <- truth
  for (family in unique(genes$family_id[!is.na(genes$family_id)])) {
    idx <- which(!is.na(genes$family_id) & genes$family_id == family)
    if (length(idx) < 2L) next

    similarity <- matrix(runif(length(idx)^2, 0.55, 1.0),
                         nrow = length(idx), ncol = length(idx))
    diag(similarity) <- 0
    prob <- sweep(similarity, 1, rowSums(similarity), "/")
    rate <- runif(1, 0.15, 0.35)

    observed[idx, ] <- (1 - rate) * truth[idx, ] + rate * (t(prob) %*% truth[idx, ])
  }
  observed <- round(observed)

  list(truth = truth, observed = observed, activity = activityMat)
}

# ---------------------------------------------------------------------------
# 4. limma 差异表达 + GSVA 通路打分
# ---------------------------------------------------------------------------

logCpm <- function(counts) {
  lib <- pmax(colSums(counts), 1)
  log2(t(t(counts) / lib) * 1e6 + 1)
}

geneSetsOf <- function(geneIds, pathways) {
  sets <- split(geneIds, pathways)
  sets <- lapply(sets, unique)
  # GSVA 对基因集大小敏感：过滤过小/过大的集合
  sets[vapply(sets, length, integer(1)) >= 10L]
}

runLimma <- function(expr, design) {
  group <- factor(paste(design$consortium, design$herbicide, design$qs, sep = "."))
  designMatrix <- model.matrix(~0 + group)
  colnames(designMatrix) <- levels(group)

  fit <- lmFit(expr, designMatrix)
  contrasts <- makeContrasts(
    YKB_QSR_vs_WT = YKB.WT.QSR - YKB.WT.WT,
    HKB_QSR_vs_WT = HKB.WT.QSR - HKB.WT.WT,
    YKB_NH_vs_WT = YKB.NH.WT - YKB.WT.WT,
    HKB_NH_vs_WT = HKB.NH.WT - HKB.WT.WT,
    levels = designMatrix)
  fit2 <- eBayes(contrasts.fit(fit, contrasts))

  result <- lapply(colnames(contrasts), function(name) {
    table <- topTable(fit2, coef = name, number = Inf, sort.by = "P")
    table$gene_id <- rownames(table)
    table$contrast <- name
    table
  })

  do.call(rbind, result)
}

runGsva <- function(expr, geneSets) {
  sets <- lapply(geneSets, function(g) intersect(g, rownames(expr)))
  sets <- sets[vapply(sets, length, integer(1)) >= 10L]
  param <- gsvaParam(expr, sets, kcdf = "Gaussian", absRanking = FALSE)
  gsva(param, verbose = FALSE)
}

# 每个菌株的真实单菌株矩阵 -> GSVA 打分(用于菌株水平的分工结论)
strainGsvaScores <- function(truth) {
  sets <- geneSetsOf(genes$gene_id, genes$pathway)
  scores <- list()

  for (strain in strains) {
    idx <- which(geneStrain == strain)
    mat <- truth[idx, , drop = FALSE]
    rownames(mat) <- genes$gene_id[idx]
    scores[[strain]] <- runGsva(logCpm(mat), sets)
  }

  scores
}

groupMean <- function(scores, pathway, pattern) {
  if (is.null(scores) || !(pathway %in% rownames(scores))) return(NA_real_)
  cols <- grepl(pattern, colnames(scores), fixed = TRUE)
  if (!any(cols)) return(NA_real_)
  mean(scores[pathway, cols])
}

strainMean <- function(strainScores, strain, pathway, pattern) {
  groupMean(strainScores[[strain]], pathway, pattern)
}

# ---------------------------------------------------------------------------
# 5. 论文结论清单(基于 GSVA 通路得分)与校准
# ---------------------------------------------------------------------------

# 每条结论: id / 描述 / 评估函数 / 失败时用于校准的 (菌群, 菌株, 模块, 方向)
conclusions <- list(
  list(id = "C1_YKB_herbicide_induction", target = c("YKB", "K1", "herb_initial", "up"),
       text = "herbicide exposure induces atrazine degradation (map00791) in SynCom YKB",
       check = function(s) groupMean(s$community, "map00791", "YKB_WT_WT") >
                              groupMean(s$community, "map00791", "YKB_NH_WT")),
  list(id = "C2_HKB_herbicide_induction", target = c("HKB", "B1", "herb_downstream", "up"),
       text = "herbicide exposure induces atrazine degradation (map00791) in SynCom HKB",
       check = function(s) groupMean(s$community, "map00791", "HKB_WT_WT") >
                              groupMean(s$community, "map00791", "HKB_NH_WT")),
  list(id = "C3_YKB_AHL_inhibition", target = c("YKB", "K1", "qs_ahl", "up"),
       text = "AHL inhibition (luxI RNAi) suppresses quorum sensing (map02024) in YKB",
       check = function(s) groupMean(s$community, "map02024", "YKB_WT_WT") >
                              groupMean(s$community, "map02024", "YKB_WT_QSR")),
  list(id = "C4_HKB_AI2_inhibition", target = c("HKB", "B1", "qs_ai2", "up"),
       text = "AI-2 inhibition (luxS RNAi) suppresses quorum sensing (map02024) in HKB",
       check = function(s) groupMean(s$community, "map02024", "HKB_WT_WT") >
                              groupMean(s$community, "map02024", "HKB_WT_QSR")),
  list(id = "C5_YKB_energy_supply", target = c("YKB", "Y1", "carbohydrate", "up"),
       text = "YKB invests more in carbohydrate/energy metabolism (map00010) than HKB",
       check = function(s) groupMean(s$community, "map00010", "YKB_WT_WT") >
                              groupMean(s$community, "map00010", "HKB_WT_WT")),
  list(id = "C6_HKB_sulfur_metabolism", target = c("HKB", "H1", "sulfur", "up"),
       text = "HKB invests more in sulfur metabolism (map00920) than YKB",
       check = function(s) groupMean(s$community, "map00920", "HKB_WT_WT") >
                              groupMean(s$community, "map00920", "YKB_WT_WT")),
  list(id = "C7_YKB_K1_degrader", target = c("YKB", "K1", "herb_initial", "up"),
       text = "in YKB, K1 dominates herbicide degradation (map00791 GSVA)",
       check = function(s) strainMean(s$strain, "K1", "map00791", "YKB_WT_WT") >
                              strainMean(s$strain, "B1", "map00791", "YKB_WT_WT")),
  list(id = "C8_YKB_B1_janitor", target = c("YKB", "B1", "tryptophan", "up"),
       text = "in YKB, B1 dominates tryptophan metabolism (map00380 GSVA)",
       check = function(s) strainMean(s$strain, "B1", "map00380", "YKB_WT_WT") >
                              strainMean(s$strain, "K1", "map00380", "YKB_WT_WT")),
  list(id = "C9_YKB_Y1_energy", target = c("YKB", "Y1", "carbohydrate", "up"),
       text = "in YKB, Y1 dominates carbohydrate/energy metabolism (map00010 GSVA)",
       check = function(s) strainMean(s$strain, "Y1", "map00010", "YKB_WT_WT") >
                              strainMean(s$strain, "K1", "map00010", "YKB_WT_WT")),
  list(id = "C10_HKB_B1_core_degrader", target = c("HKB", "B1", "herb_downstream", "up"),
       text = "in HKB, B1 dominates herbicide degradation (map00791 GSVA)",
       check = function(s) strainMean(s$strain, "B1", "map00791", "HKB_WT_WT") >
                              strainMean(s$strain, "K1", "map00791", "HKB_WT_WT")),
  list(id = "C11_HKB_H1_sulfur", target = c("HKB", "H1", "sulfur", "up"),
       text = "in HKB, H1 dominates sulfur metabolism (map00920 GSVA)",
       check = function(s) strainMean(s$strain, "H1", "map00920", "HKB_WT_WT") >
                              strainMean(s$strain, "B1", "map00920", "HKB_WT_WT")),
  list(id = "C12_HKB_H1_repair", target = c("HKB", "H1", "dna_repair", "up"),
       text = "in HKB, H1 dominates DNA repair (map03440 GSVA)",
       check = function(s) strainMean(s$strain, "H1", "map03440", "HKB_WT_WT") >
                              strainMean(s$strain, "B1", "map03440", "HKB_WT_WT")),
  list(id = "C13_HKB_H1_AI2", target = c("HKB", "H1", "qs_ai2", "up"),
       text = "in HKB, H1 dominates AI-2 signalling (map02024 GSVA)",
       check = function(s) strainMean(s$strain, "H1", "map02024", "HKB_WT_WT") >
                              strainMean(s$strain, "B1", "map02024", "HKB_WT_WT")),
  list(id = "C14_YKB_K1_QS_shift", target = c("YKB", "K1", "herb_initial", "up"),
       text = "AHL inhibition suppresses K1 herbicide degradation in YKB",
       check = function(s) strainMean(s$strain, "K1", "map00791", "YKB_WT_WT") >
                              strainMean(s$strain, "K1", "map00791", "YKB_WT_QSR")),
  list(id = "C15_HKB_B1_QS_shift", target = c("HKB", "B1", "herb_downstream", "up"),
       text = "AI-2 inhibition suppresses B1 herbicide degradation in HKB",
       check = function(s) strainMean(s$strain, "B1", "map00791", "HKB_WT_WT") >
                              strainMean(s$strain, "B1", "map00791", "HKB_WT_QSR")))

analyse <- function(sim) {
  expr <- logCpm(sim$observed)
  rownames(expr) <- genes$gene_id

  community <- runGsva(expr, geneSetsOf(genes$gene_id, genes$pathway))
  strain <- strainGsvaScores(sim$truth)

  list(community = community, strain = strain,
       limma = runLimma(expr, design))
}

# 校准循环：GSVA 结论不满足时，按结论给出的 (菌群, 菌株, 模块, 方向) 微调模块组成
calibrate <- function(maxRounds = 6L, step = 1.12) {
  base <- moduleBase
  history <- list()
  sim <- NULL
  scores <- NULL
  pass <- NULL

  for (round in seq_len(maxRounds)) {
    sim <- simulateCommunity(base, activityTable, SEED + round)
    scores <- analyse(sim)
    pass <- vapply(conclusions, function(cl) isTRUE(cl$check(scores)), logical(1))

    history[[length(history) + 1L]] <- list(
      round = round,
      failed = vapply(conclusions, `[[`, "", "id")[!pass])

    cat(sprintf("[calibration] round %d: %d/%d conclusions passed\n",
                round, sum(pass), length(pass)))

    if (all(pass)) {
      return(list(base = base, sim = sim, scores = scores, pass = pass,
                  rounds = round, history = history))
    }

    for (i in seq_along(conclusions)) {
      if (pass[i]) next

      target <- conclusions[[i]]$target
      consortium <- target[1]
      strain <- target[2]
      module <- target[3]
      direction <- target[4]
      factor <- if (identical(direction, "up")) step else 1 / step

      base[[consortium]][strain, module] <- base[[consortium]][strain, module] * factor
      cat(sprintf("   adjust %s: %s/%s x %.3f\n", consortium, strain, module, factor))
    }
  }

  list(base = base, sim = sim, scores = scores, pass = pass,
       rounds = maxRounds, history = history)
}

# ---------------------------------------------------------------------------
# 6. 运行校准并写出 CSV
# ---------------------------------------------------------------------------

cat(sprintf("genome size: %d genes x %d samples, %d shared families\n",
            nrow(genes), nSample, length(unique(na.omit(genes$family_id)))))

fit <- calibrate()

# 校准后的模块组成(记录在案，保证数据可复现)
moduleBase <- fit$base
sim <- fit$sim
scores <- fit$scores

writeMatrix <- function(mat, path) {
  table <- data.frame(geneID = genes$gene_id, mat, check.names = FALSE)
  write.csv(table, path, row.names = FALSE, quote = FALSE)
}

# strain id 统一使用菌株的学名全名(Comamonas sp. Y1 等)
geneStrainLabel <- unname(strainName[geneStrain])

writeMatrix(as.matrix(sim$observed), file.path(outDir, "total.csv"))

for (strain in strains) {
  idx <- which(geneStrain == strain)
  table <- data.frame(geneID = genes$gene_id[idx], sim$truth[idx, , drop = FALSE],
                      check.names = FALSE)
  write.csv(table, file.path(outDir, "truth", paste0(strainLabel(strain), ".csv")),
            row.names = FALSE, quote = FALSE)
}

write.csv(data.frame(strain = geneStrainLabel, isolate = geneStrain,
                     gene_id = genes$gene_id, symbol = genes$symbol,
                     genus = unname(genera[geneStrain]), KO = genes$KO,
                     pathway = genes$pathway, stringsAsFactors = FALSE),
          file.path(outDir, "genomes.csv"), row.names = FALSE, quote = FALSE)

familyRows <- which(!is.na(genes$family_id))
write.csv(data.frame(family_id = genes$family_id[familyRows],
                     gene_id = genes$gene_id[familyRows],
                     symbol = genes$symbol[familyRows],
                     KO = genes$KO[familyRows],
                     stringsAsFactors = FALSE),
          file.path(outDir, "families.csv"), row.names = FALSE, quote = FALSE)

write.csv(data.frame(gene_id = genes$gene_id, symbol = genes$symbol,
                     strain = geneStrainLabel, isolate = geneStrain,
                     module = geneModule,
                     family_id = ifelse(is.na(genes$family_id), "", genes$family_id),
                     KO = genes$KO, pathway = genes$pathway,
                     stringsAsFactors = FALSE),
          file.path(outDir, "annotation.csv"), row.names = FALSE, quote = FALSE)

write.csv(design, file.path(outDir, "design.csv"), row.names = FALSE, quote = FALSE)

# KEGG 通路表(基因数 + KO 列表 + 基因符号列表)
pathwayRows <- lapply(names(pathwayPool), function(p) {
  idx <- which(genes$pathway == p)
  data.frame(pathway = p, name = unname(pathwayName[p]),
             genes = length(idx),
             KO = paste(sort(unique(genes$KO[idx])), collapse = ";"),
             symbols = paste(sort(unique(genes$symbol[idx])), collapse = ";"),
             stringsAsFactors = FALSE)
})
write.csv(do.call(rbind, pathwayRows), file.path(outDir, "pathways.csv"),
          row.names = FALSE, quote = FALSE)

# GSVA：群落水平(通路 x 样本) 与 菌株水平(菌株 x 通路 x 分组均值)
communityGsva <- scores$community
write.csv(data.frame(pathway = rownames(communityGsva), communityGsva,
                     check.names = FALSE),
          file.path(outDir, "gsva_community.csv"), row.names = FALSE, quote = FALSE)

strainRows <- list()
for (strain in strains) {
  mat <- scores$strain[[strain]]
  for (p in rownames(mat)) {
    for (group in unique(paste(design$consortium, design$herbicide, design$qs, sep = "_"))) {
      cols <- grepl(group, colnames(mat), fixed = TRUE)
      if (!any(cols)) next
      strainRows[[length(strainRows) + 1L]] <- data.frame(
        strain = strainLabel(strain), pathway = p, group = group,
        gsva = mean(mat[p, cols]), stringsAsFactors = FALSE)
    }
  }
}
write.csv(do.call(rbind, strainRows), file.path(outDir, "gsva_strain.csv"),
          row.names = FALSE, quote = FALSE)

# limma 差异表达：显著基因 + 汇总
limmaTable <- scores$limma
annotation <- data.frame(gene_id = genes$gene_id, symbol = genes$symbol,
                         strain = geneStrainLabel, module = geneModule,
                         KO = genes$KO, pathway = genes$pathway,
                         stringsAsFactors = FALSE)
limmaTable <- merge(limmaTable, annotation, by = "gene_id", all.x = TRUE)

# 参考论文的差异表达阈值: |log2FC| >= 1 且 adj.P < 0.05
significant <- limmaTable[!is.na(limmaTable$adj.P.Val) & limmaTable$adj.P.Val < 0.05 &
                            abs(limmaTable$logFC) >= 1, ]
write.csv(significant[, c("contrast", "gene_id", "symbol", "strain", "module", "KO",
                          "pathway", "logFC", "AveExpr", "t", "P.Value", "adj.P.Val")],
          file.path(outDir, "limma_de_hits.csv"), row.names = FALSE, quote = FALSE)

limmaSummary <- do.call(rbind, lapply(split(limmaTable, limmaTable$contrast), function(d) {
  hit <- !is.na(d$adj.P.Val) & d$adj.P.Val < 0.05 & abs(d$logFC) >= 1
  data.frame(contrast = d$contrast[1],
             tested = nrow(d),
             significant = sum(hit),
             up = sum(hit & d$logFC > 0),
             down = sum(hit & d$logFC < 0),
             stringsAsFactors = FALSE)
}))
write.csv(limmaSummary, file.path(outDir, "limma_summary.csv"),
          row.names = FALSE, quote = FALSE)

# GSVA 结论表(数据集是否复现论文机制)
conclusionRows <- lapply(seq_along(conclusions), function(i) {
  cl <- conclusions[[i]]
  data.frame(id = cl$id, description = cl$text,
             passed = isTRUE(fit$pass[i]), stringsAsFactors = FALSE)
})
conclusionTable <- do.call(rbind, conclusionRows)
# 结论的描述文本含有逗号，必须加引号，否则下游解析会错列
write.csv(conclusionTable, file.path(outDir, "gsva_conclusions.csv"),
          row.names = FALSE, quote = TRUE)

# 校准后的模块组成(便于复现与审阅；菌株列使用学名)
base <- do.call(rbind, lapply(names(moduleBase), function(consortium) {
  table <- as.data.frame(moduleBase[[consortium]])
  data.frame(consortium = consortium,
             strain = unname(strainName[rownames(moduleBase[[consortium]])]),
             isolate = rownames(moduleBase[[consortium]]),
             table, check.names = FALSE)
}))
write.csv(base, file.path(outDir, "module_base.csv"), row.names = FALSE, quote = FALSE)

cat(sprintf("calibration rounds: %d, conclusions passed: %d/%d\n",
            fit$rounds, sum(fit$pass), length(fit$pass)))
cat(sprintf("limma: %d gene-contrast hits at adj.P < 0.05\n", nrow(significant)))
cat(sprintf("output: %s\n", normalizePath(outDir)))

if (!all(fit$pass)) {
  failed <- conclusionTable$id[!conclusionTable$passed]
  cat("WARNING: the following conclusions are still not satisfied:\n")
  cat(paste0("  - ", failed, collapse = "\n"), "\n")
}
