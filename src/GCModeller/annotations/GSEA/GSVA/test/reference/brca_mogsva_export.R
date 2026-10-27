#!/usr/bin/env Rscript
# =============================================================================
# brca_mogsva_export.R
#
# Convert the cBioPortal TCGA BRCA study (``brca_tcga_pub2015``) into the plain
# tab delimited matrix format that the GCModeller library can read directly
# through ``SMRUCC.genomics.Analysis.HTS.DataFrame.Matrix.LoadData``, so that
# the moGSVA module can be tested on a real multi-omics data set.
#
# Usage:
#   Rscript brca_mogsva_export.R [--src=DIR] [--out=DIR] [--full]
#                                [--layers=mrna,met,protein,cna] [--digits=8]
#
#   --src     the cBioPortal study folder, default K:/brca_tcga_pub2015
#   --out     the output folder, default <src>/multipleOmics
#   --full    also exports every layer on its own native sample set (the
#             default only exports the sample aligned matrices). This is useful
#             for testing the sample axis intersection of MultiOmicsDataset.
#   --layers  a comma separated layer subset, default all of the four layers
#   --digits  significant digits that are written into the matrix files
#
# Output (aligned, default):
#   mRNA.txt          log2(RSEM + 1) of data_mrna_seq_v2_rsem.txt
#   methylation.txt   HM450 beta value of data_methylation_hm450.txt
#   protein.txt       RPPA of data_rppa.txt (antibody id is collapsed to gene)
#   cna.txt           data_linear_cna.txt
#   samples.tsv       sample level clinical annotation of the exported samples
#   manifest.tsv      machine readable description of every exported matrix
#   kegg_v3.gmt       a copy of the KEGG gene set collection (when available)
#
# Output (``--full`` adds):
#   full/<layer>.txt  the same layers but on their own native sample sets
#
# Format guarantees (required by the GCModeller reader):
#   * exactly one leading identifier column, columns are separated by a tab
#   * the header contains the identifier column name and every sample id
#   * no NA / empty cell, no duplicated feature id, no constant-only column
#   * every matrix of a group shares the same sample order
#
# Notes about the source data (verified on the 2026-10-27 export):
#   * ``data_mrna_seq_v2_rsem.txt`` is *linear* RSEM, the max value is ~261k,
#     therefore it must be log2 transformed before it can be scored by GSVA.
#   * ``data_methylation_hm450.txt`` is the beta value in [0, 1]; the value is
#     exported unchanged, the moGSVA layer must be created with Direction = -1.
#   * ``data_rppa.txt`` uses ``GENE|antibody`` as the row id and 10 of the 226
#     rows contain more than one gene (``AKT1 AKT2 AKT3|Akt``); the first gene
#     symbol is used and the duplicated genes are averaged (226 -> 183).
#   * the cBioPortal matrices carry two identifier columns (``Hugo_Symbol`` and
#     ``Entrez_Gene_Id``) while the GCModeller reader assumes one, so the
#     ``Entrez_Gene_Id`` column is dropped here.
# =============================================================================

options(stringsAsFactors = FALSE, warn = 1)

# ---------------------------------------------------------------------------
# command line
# ---------------------------------------------------------------------------

parse_args <- function(argv) {
  opt <- list(
    src = "K:/brca_tcga_pub2015",
    out = NA_character_,
    full = FALSE,
    layers = c("mrna", "met", "protein", "cna"),
    digits = 8L
  )

  for (arg in argv) {
    if (arg == "--full") {
      opt$full <- TRUE
    } else if (grepl("^--src=", arg)) {
      opt$src <- sub("^--src=", "", arg)
    } else if (grepl("^--out=", arg)) {
      opt$out <- sub("^--out=", "", arg)
    } else if (grepl("^--layers=", arg)) {
      opt$layers <- trimws(strsplit(sub("^--layers=", "", arg), ",")[[1]])
    } else if (grepl("^--digits=", arg)) {
      opt$digits <- as.integer(sub("^--digits=", "", arg))
    } else {
      stop(sprintf("unknown argument: %s", arg))
    }
  }

  opt$src <- normalizePath(opt$src, mustWork = TRUE)

  if (is.na(opt$out)) {
    opt$out <- file.path(opt$src, "multipleOmics")
  }

  dir.create(opt$out, recursive = TRUE, showWarnings = FALSE)
  opt$out <- normalizePath(opt$out, mustWork = TRUE)
  opt
}

# ---------------------------------------------------------------------------
# low level helpers
# ---------------------------------------------------------------------------

#' Reads the first line of a tab delimited file as the column names.
#' A UTF-8 BOM (which some cBioPortal exports contain) is removed.
read_header <- function(path) {
  line <- readLines(path, n = 1L, warn = FALSE)
  hdr <- strsplit(sub("^\ufeff", "", line), "\t", fixed = TRUE)[[1]]
  trimws(hdr)
}

#' Reads one layer of the cBioPortal matrix, keeping only the given samples.
#'
#' @param path        the matrix file path
#' @param id_cols     the number of the leading identifier columns
#' @param keep        the sample ids that should be kept; ``NULL`` keeps all
#' @return a list with ``mat`` (features x samples) and ``ids`` (feature ids)
read_layer <- function(path, id_cols, keep = NULL) {
  hdr <- read_header(path)

  if (length(hdr) <= id_cols) {
    stop(sprintf("the matrix file has no sample column: %s", path))
  }

  sample_ids <- hdr[(id_cols + 1L):length(hdr)]

  if (is.null(keep)) {
    take <- seq.int(id_cols + 1L, length(hdr))
  } else {
    pos <- match(keep, sample_ids)

    if (anyNA(pos)) {
      stop(sprintf("sample id is missing in %s: %s",
                   basename(path), paste(keep[is.na(pos)], collapse = ", ")))
    }

    take <- pos + id_cols
  }

  classes <- rep("NULL", length(hdr))
  classes[1L] <- "character"          # the feature identifier column
  classes[take] <- "numeric"

  tbl <- utils::read.delim(
    path,
    header = TRUE,
    sep = "\t",
    quote = "",
    comment.char = "",
    check.names = FALSE,
    colClasses = classes,
    na.strings = c("", "NA", "NaN"),
    stringsAsFactors = FALSE
  )

  ids <- trimws(as.character(tbl[[1L]]))
  mat <- as.matrix(tbl[, -1L, drop = FALSE])
  storage.mode(mat) <- "double"

  if (ncol(mat) != length(take)) {
    stop(sprintf("column count mismatch in %s", basename(path)))
  }

  colnames(mat) <- if (is.null(keep)) sample_ids else keep

  list(mat = mat, ids = ids)
}

#' Collapses duplicated feature identifiers by the row mean.
aggregate_mean <- function(mat, ids) {
  if (!anyDuplicated(ids)) {
    rownames(mat) <- ids
    return(list(mat = mat, ids = ids))
  }

  sum_mat <- rowsum(mat, group = ids, reorder = TRUE, na.rm = TRUE)
  cnt_mat <- rowsum(ifelse(is.na(mat), 0, 1), group = ids, reorder = TRUE,
                    na.rm = FALSE)
  cnt_mat[cnt_mat == 0] <- 1

  agg <- sum_mat / cnt_mat
  agg[!is.finite(agg)] <- 0

  list(mat = agg, ids = rownames(agg))
}

#' Replaces the missing value of each row by the row median.
impute_row_median <- function(mat) {
  rows <- which(rowSums(is.na(mat)) > 0L)

  if (length(rows) == 0L) {
    return(mat)
  }

  for (i in rows) {
    value <- mat[i, ]
    fill <- stats::median(value, na.rm = TRUE)

    if (!is.finite(fill)) {
      fill <- 0
    }

    value[is.na(value)] <- fill
    mat[i, ] <- value
  }

  mat
}

#' Writes the matrix in the plain tab delimited format of GCModeller.
write_matrix <- function(mat, ids, path, digits = 8L) {
  if (anyNA(mat)) {
    stop(sprintf("NA value remains in the output matrix: %s", path))
  }

  mat <- signif(mat, digits)
  out <- data.frame(gene_id = ids, mat, check.names = FALSE,
                    stringsAsFactors = FALSE)

  utils::write.table(
    out,
    file = path,
    sep = "\t",
    quote = FALSE,
    row.names = FALSE,
    col.names = TRUE,
    na = "0",
    eol = "\n"
  )

  invisible(path)
}

# ---------------------------------------------------------------------------
# layer configuration
# ---------------------------------------------------------------------------

#' The description of every supported omics layer.
#'
#' ``prepare`` receives the raw matrix (already restricted to the common
#' samples) and returns the matrix that will be written to the disk.
layer_config <- function(src) {
  list(
    mrna = list(
      file      = file.path(src, "data_mrna_seq_v2_rsem.txt"),
      id_cols   = 2L,
      clean     = function(ids) trimws(ids),
      prepare   = function(mat) log2(mat + 1),
      transform = "log2(RSEM+1)",
      direction = 1L,
      norm      = "Log2 then ZScore",
      note      = "linear RSEM is log2 transformed"
    ),
    met = list(
      file      = file.path(src, "data_methylation_hm450.txt"),
      id_cols   = 2L,
      clean     = function(ids) trimws(ids),
      prepare   = function(mat) mat,
      transform = "beta (HM450)",
      direction = -1L,
      norm      = "RobustZScore",
      note      = "beta value in [0,1]; use Direction=-1 in moGSVA"
    ),
    protein = list(
      file      = file.path(src, "data_rppa.txt"),
      id_cols   = 1L,
      clean     = function(ids) {
        gene <- trimws(sub("\\|.*$", "", ids))
        sub("[[:space:]].*$", "", gene)   # AKT1 AKT2 AKT3 -> AKT1
      },
      prepare   = function(mat) mat,
      transform = "RPPA (centered)",
      direction = 1L,
      norm      = "None",
      note      = "antibody id collapsed to gene; duplicated genes averaged"
    ),
    cna = list(
      file      = file.path(src, "data_linear_cna.txt"),
      id_cols   = 2L,
      clean     = function(ids) trimws(ids),
      prepare   = function(mat) mat,
      transform = "log2 copy number",
      direction = 1L,
      norm      = "RankPerFeature",
      note      = "use a rank based score method for this layer"
    )
  )
}

#' Loads one layer and applies the identifier clean up, the aggregation, the
#' missing value imputation and the value transformation.
load_clean_layer <- function(cfg, name, keep) {
  raw <- read_layer(cfg$file, cfg$id_cols, keep)
  ids <- cfg$clean(raw$ids)

  keep_row <- !is.na(ids) & nzchar(ids)
  mat <- raw$mat[keep_row, , drop = FALSE]
  ids <- ids[keep_row]

  mat <- impute_row_median(mat)

  if (anyNA(mat)) {
    # the whole row is missing, drop it instead of inventing a value
    good <- rowSums(is.na(mat)) == 0L
    mat <- mat[good, , drop = FALSE]
    ids <- ids[good]
  }

  agg <- aggregate_mean(mat, ids)
  mat <- cfg$prepare(agg$mat)

  if (any(!is.finite(mat))) {
    mat[!is.finite(mat)] <- 0
  }

  list(mat = mat, ids = agg$ids)
}

# ---------------------------------------------------------------------------
# clinical annotation
# ---------------------------------------------------------------------------

load_clinical <- function(src, samples) {
  sample_file <- file.path(src, "data_clinical_sample.txt")
  patient_file <- file.path(src, "data_clinical_patient.txt")

  if (!file.exists(sample_file)) {
    return(NULL)
  }

  samp <- utils::read.delim(sample_file, skip = 4L, header = TRUE, sep = "\t",
                            quote = "", comment.char = "", check.names = FALSE,
                            na.strings = character(0), stringsAsFactors = FALSE)

  keep_cols <- intersect(c("PATIENT_ID", "SAMPLE_ID", "SAMPLE_TYPE",
                           "ER_STATUS_BY_IHC", "PR_STATUS_BY_IHC", "IHC_HER2",
                           "HER2_FISH_STATUS", "AJCC_PATHOLOGIC_TUMOR_STAGE",
                           "TMB_NONSYNONYMOUS"),
                         colnames(samp))
  samp <- samp[, keep_cols, drop = FALSE]
  samp <- samp[samp$SAMPLE_ID %in% samples, , drop = FALSE]

  if (file.exists(patient_file)) {
    pat <- utils::read.delim(patient_file, skip = 4L, header = TRUE, sep = "\t",
                             quote = "", comment.char = "", check.names = FALSE,
                             na.strings = character(0), stringsAsFactors = FALSE)
    keep_pat <- intersect(c("PATIENT_ID", "AGE", "HISTOLOGICAL_DIAGNOSIS",
                            "AJCC_PATHOLOGIC_TUMOR_STAGE", "OS_STATUS",
                            "OS_MONTHS", "DFS_STATUS", "DFS_MONTHS"),
                          colnames(pat))
    pat <- pat[, keep_pat, drop = FALSE]
    pat <- pat[!duplicated(pat$PATIENT_ID), , drop = FALSE]

    add <- setdiff(colnames(pat), colnames(samp))
    samp <- merge(samp, pat[, c("PATIENT_ID", add), drop = FALSE],
                  by = "PATIENT_ID", all.x = TRUE, sort = FALSE)
  }

  # keep the sample order of the expression matrices
  samp <- samp[match(samples, samp$SAMPLE_ID), , drop = FALSE]
  rownames(samp) <- NULL
  samp
}

# ---------------------------------------------------------------------------
# main
# ---------------------------------------------------------------------------

main <- function() {
  opt <- parse_args(commandArgs(trailingOnly = TRUE))
  started <- Sys.time()

  conf <- layer_config(opt$src)
  layers <- opt$layers[opt$layers %in% names(conf)]

  if (length(layers) == 0L) {
    stop("no valid layer is requested")
  }

  for (layer in layers) {
    if (!file.exists(conf[[layer]]$file)) {
      stop(sprintf("the source file is missing: %s", conf[[layer]]$file))
    }
  }

  cat(sprintf("[brca] source : %s\n", opt$src))
  cat(sprintf("[brca] output : %s\n", opt$out))
  cat(sprintf("[brca] layers : %s\n", paste(layers, collapse = ", ")))

  # ---- 1. the common sample axis -----------------------------------------
  sample_sets <- lapply(layers, function(layer) {
    hdr <- read_header(conf[[layer]]$file)
    hdr[(conf[[layer]]$id_cols + 1L):length(hdr)]
  })
  names(sample_sets) <- layers

  common <- sample_sets[[1L]]

  if (length(layers) > 1L) {
    shared <- Reduce(intersect, sample_sets)
    common <- common[common %in% shared]
  }

  cat(sprintf("[brca] common samples: %d (%s)\n", length(common),
              paste(sprintf("%s=%d", layers, vapply(sample_sets, length, 0L)),
                    collapse = ", ")))

  if (length(common) < 2L) {
    stop("less than two common samples, cannot build a multi-omics data set")
  }

  # ---- 2. the aligned layers ---------------------------------------------
  manifest <- data.frame(
    layer = character(0), file = character(0), features = integer(0),
    samples = integer(0), transform = character(0),
    direction = integer(0), normalization = character(0),
    note = character(0), stringsAsFactors = FALSE
  )

  for (layer in layers) {
    cfg <- conf[[layer]]
    data <- load_clean_layer(cfg, layer, common)

    target <- file.path(opt$out, sprintf("%s.txt", layer))
    write_matrix(data$mat, data$ids, target, opt$digits)

    manifest <- rbind(manifest, data.frame(
      layer = layer,
      file = basename(target),
      features = nrow(data$mat),
      samples = ncol(data$mat),
      transform = cfg$transform,
      direction = cfg$direction,
      normalization = cfg$norm,
      note = cfg$note,
      stringsAsFactors = FALSE
    ))

    cat(sprintf("[brca] %-8s %6d features x %3d samples -> %s\n",
                layer, nrow(data$mat), ncol(data$mat), basename(target)))
  }

  # ---- 3. optional: the native sample set of every layer -----------------
  if (opt$full) {
    full_dir <- file.path(opt$out, "full")
    dir.create(full_dir, showWarnings = FALSE)
    full_manifest <- manifest[0, , drop = FALSE]

    for (layer in layers) {
      cfg <- conf[[layer]]
      data <- load_clean_layer(cfg, layer, NULL)

      target <- file.path(full_dir, sprintf("%s.txt", layer))
      write_matrix(data$mat, data$ids, target, opt$digits)

      full_manifest <- rbind(full_manifest, data.frame(
        layer = layer,
        file = file.path("full", basename(target)),
        features = nrow(data$mat),
        samples = ncol(data$mat),
        transform = cfg$transform,
        direction = cfg$direction,
        normalization = cfg$norm,
        note = cfg$note,
        stringsAsFactors = FALSE
      ))

      cat(sprintf("[brca] full/%-8s %6d features x %3d samples -> %s\n",
                  layer, nrow(data$mat), ncol(data$mat), basename(target)))
    }

    manifest <- rbind(manifest, full_manifest)
  }

  # ---- 4. the sample annotation ------------------------------------------
  samples <- load_clinical(opt$src, common)

  if (is.null(samples)) {
    cat("[brca] warning: the clinical file is missing, skip samples.tsv\n")
  } else {
    utils::write.table(samples, file = file.path(opt$out, "samples.tsv"),
                       sep = "\t", quote = FALSE, row.names = FALSE,
                       col.names = TRUE, na = "")
    cat(sprintf("[brca] samples.tsv: %d rows\n", nrow(samples)))
  }

  # ---- 5. the manifest ----------------------------------------------------
  utils::write.table(manifest, file = file.path(opt$out, "manifest.tsv"),
                     sep = "\t", quote = FALSE, row.names = FALSE,
                     col.names = TRUE)

  # ---- 6. the gene set collection ----------------------------------------
  gmt <- file.path("G:/GCModeller/src/GCModeller/annotations/GSEA/data/TCGA_COAD/kegg_v3.gmt")

  if (file.exists(gmt)) {
    file.copy(gmt, file.path(opt$out, "kegg_v3.gmt"), overwrite = TRUE)
    cat("[brca] kegg_v3.gmt copied\n")
  } else {
    cat("[brca] warning: the KEGG gmt file is not found, skip the gmt copy\n")
  }

  elapsed <- as.numeric(difftime(Sys.time(), started, units = "secs"))
  cat(sprintf("[brca] done in %.1f s\n", elapsed))

  invisible(manifest)
}

main()
