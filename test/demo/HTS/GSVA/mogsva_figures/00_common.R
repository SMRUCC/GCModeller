# =============================================================================
# 00_common.R -- shared configuration, I/O helpers, theme and palettes
#
# Publication figures for the moGSVA analysis of TCGA-BRCA (n = 460 tumours,
# four omics layers, 335 KEGG pathways).
#
# All figure text is English by design (manuscript figures).
# =============================================================================

suppressPackageStartupMessages({
  library(data.table)
  library(ggplot2)
  library(patchwork)
  library(scales)
  library(grid)
})

## ---------------------------------------------------------------- paths -----
# Override with environment variables when the project is moved.
RESULT_DIR <- Sys.getenv(
  "MOGSVA_RESULT_DIR",
  unset = "G:/GCModeller/test/demo/HTS/GSVA/brca_tcga_pub2015_mogsva"
)
RAW_DIR <- Sys.getenv(
  "MOGSVA_RAW_DIR",
  unset = "K:/brca_tcga_pub2015/multipleOmics"
)
SCRIPT_DIR <- Sys.getenv(
  "MOGSVA_SCRIPT_DIR",
  unset = "G:/GCModeller/test/demo/HTS/GSVA/mogsva_figures"
)
OUT_DIR <- file.path(SCRIPT_DIR, "output")
CACHE_DIR <- file.path(SCRIPT_DIR, ".cache")

for (d in c(OUT_DIR, CACHE_DIR)) {
  if (!dir.exists(d)) dir.create(d, recursive = TRUE, showWarnings = FALSE)
}

## ------------------------------------------------------------- constants ----
OMICS_LEVELS <- c(
  "Copy number alteration",
  "Methylation",
  "Transcriptomics",
  "Proteomics"
)
OMICS_LABEL <- c(
  "Copy number alteration" = "CNA",
  "Methylation"            = "Methylation",
  "Transcriptomics"        = "Transcriptomics",
  "Proteomics"             = "Proteomics"
)

## ------------------------------------------------------------- palettes -----
# NPG-style qualitative palette, one colour per omics layer
PAL_OMICS <- c(
  "Copy number alteration" = "#4DBBD5",
  "Methylation"            = "#E64B35",
  "Transcriptomics"        = "#3C5488",
  "Proteomics"             = "#00A087"
)

# clinical / annotation statuses, used consistently across every figure
PAL_STATUS <- c(
  "Positive"    = "#D1495B",
  "Negative"    = "#3F7FBF",
  "Equivocal"   = "#F2C14E",
  "Not assessed" = "#D9D9D9"
)

PAL_SUBTYPE <- c(
  "ER+/HER2-"          = "#D1495B",
  "ER+/HER2+"          = "#F2A65A",
  "ER-/HER2-"          = "#3F7FBF",
  "ER-/HER2+"          = "#8E7CC3",
  "Not classified"     = "#D9D9D9"
)

GRAD_LOW  <- "#2F5D8C"   # low  end of the diverging scale
GRAD_MID  <- "#F7F7F7"
GRAD_HIGH <- "#A6362A"   # high end of the diverging scale

PAL_DIV  <- scale_fill_gradient2(low = GRAD_LOW, mid = GRAD_MID, high = GRAD_HIGH,
                                 midpoint = 0, na.value = "grey90")
PAL_DIVC <- scale_colour_gradient2(low = GRAD_LOW, mid = GRAD_MID, high = GRAD_HIGH,
                                   midpoint = 0, na.value = "grey90")

## ---------------------------------------------------------------- theme -----
# 7 pt base size: safe for a 183 mm double-column figure.
theme_paper <- function(base_size = 7, base_family = "sans") {
  theme_bw(base_size = base_size, base_family = base_family) +
    theme(
      panel.grid.minor   = element_blank(),
      panel.grid.major   = element_line(linewidth = 0.22, colour = "grey92"),
      panel.border       = element_rect(linewidth = 0.45, colour = "black", fill = NA),
      axis.ticks         = element_line(linewidth = 0.4, colour = "black"),
      axis.ticks.length  = unit(1.2, "pt"),
      axis.text          = element_text(colour = "black", size = base_size - 0.5),
      axis.title         = element_text(colour = "black", size = base_size),
      axis.title.x       = element_text(margin = margin(t = 1.5)),
      axis.title.y       = element_text(margin = margin(r = 1.5)),
      strip.background   = element_rect(fill = "grey94", colour = "black", linewidth = 0.35),
      strip.text         = element_text(size = base_size, colour = "black", face = "bold"),
      strip.text.y       = element_text(angle = -90),
      legend.background  = element_blank(),
      legend.key         = element_blank(),
      legend.key.size    = unit(7, "pt"),
      legend.text        = element_text(size = base_size - 0.7),
      legend.title       = element_text(size = base_size - 0.2),
      legend.margin      = margin(0, 0, 0, 0),
      legend.box.spacing = unit(1, "pt"),
      plot.title         = element_text(size = base_size + 0.3, face = "bold",
                                        hjust = 0, margin = margin(b = 2)),
      plot.subtitle      = element_text(size = base_size - 0.6, hjust = 0,
                                        colour = "grey25", margin = margin(b = 2)),
      plot.tag           = element_text(size = base_size + 2.2, face = "bold"),
      plot.tag.position  = "topleft",
      plot.margin        = margin(2, 2, 2, 2, "pt")
    )
}

theme_set(theme_paper())

## ------------------------------------------------------------- I/O ----------
read_matrix <- function(path) {
  dt <- data.table::fread(path, showProgress = FALSE)
  m <- as.matrix(dt[, -1])
  rownames(m) <- dt[[1]]
  storage.mode(m) <- "double"
  m
}

#' Cache an expensive object as RDS, keyed by a name and the source file mtimes.
cache_rds <- function(name, expr) {
  src <- file.path(CACHE_DIR, paste0(name, ".rds"))
  if (file.exists(src)) {
    return(readRDS(src))
  }
  obj <- force(expr)
  saveRDS(obj, src)
  obj
}

## ------------------------------------------------------- data assembly ------
#' Load every result table once and return a tidy bundle.
load_mogsva <- function() {
  cache_rds("mogsva_bundle", {
    res <- function(f) file.path(RESULT_DIR, f)
    samples <- data.table::fread(file.path(RAW_DIR, "samples.tsv"))
    manifest <- data.table::fread(file.path(RAW_DIR, "manifest.tsv"))

    scores <- read_matrix(res("scores.csv"))
    raw <- read_matrix(res("raw_scores.csv"))
    cons <- read_matrix(res("consistency.csv"))
    pooled <- read_matrix(res("pooled_scores.csv"))

    stopifnot(!anyDuplicated(rownames(scores)))
    stopifnot(identical(rownames(scores), rownames(raw)),
              identical(rownames(scores), rownames(cons)))
    stopifnot(identical(colnames(scores), samples$SAMPLE_ID))
    # the pooled ablation scores a slightly different pathway universe:
    # it expands members across layers before the size filter, so one extra
    # pathway ("Biotin metabolism") passes there but not in the per-layer run.
    stopifnot(identical(colnames(pooled), colnames(scores)))
    pooled <- pooled[rownames(pooled) %in% rownames(scores), , drop = FALSE]

    ## --- sample annotation -------------------------------------------------
    an <- samples[, .(
      sample_id = SAMPLE_ID,
      patient   = PATIENT_ID,
      age       = as.numeric(AGE),
      stage     = AJCC_PATHOLOGIC_TUMOR_STAGE,
      histology = HISTOLOGICAL_DIAGNOSIS,
      tmb       = as.numeric(TMB_NONSYNONYMOUS),
      er        = ER_STATUS_BY_IHC,
      pr        = PR_STATUS_BY_IHC,
      her2_ihc  = IHC_HER2,
      her2_fish = HER2_FISH_STATUS,
      os_time   = suppressWarnings(as.numeric(OS_MONTHS)),
      os_event  = suppressWarnings(as.integer(sub(":.*$", "", OS_STATUS))),
      dfs_time  = suppressWarnings(as.numeric(DFS_MONTHS)),
      dfs_event = suppressWarnings(as.integer(sub(":.*$", "", DFS_STATUS)))
    )]

    ## harmonise the "not available" spellings
    clean <- function(x) {
      x <- trimws(x)
      x[x %in% c("[Not Available]", "[Not Applicable]", "", "NA")] <- NA_character_
      x
    }
    an[, `:=`(
      er        = clean(er),
      pr        = clean(pr),
      her2_ihc  = clean(her2_ihc),
      her2_fish = clean(her2_fish),
      stage     = clean(stage)
    )]

    ## HER2 calls: IHC positive or FISH positive counts as amplified
    an[, her2 := fifelse(
      (!is.na(her2_ihc) & her2_ihc == "Positive") |
        (!is.na(her2_fish) & her2_fish == "Positive"),
      "Positive",
      fifelse(!is.na(her2_ihc) & her2_ihc == "Negative", "Negative", NA_character_)
    )]

    ## IHC-based surrogate subtypes (PAM50 is not distributed with the data)
    an[, subtype := fifelse(
      is.na(er) | is.na(her2), "Not classified",
      paste0("ER", ifelse(er == "Positive", "+", "-"),
             "/HER2", ifelse(her2 == "Positive", "+", "-"))
    )]

    ## TNM stage collapsed to four ordinal groups
    an[, stage_group := fcase(
      grepl("^Stage I$|^Stage IA$|^Stage IB$", stage), "I",
      grepl("^Stage II$|^Stage IIA$|^Stage IIB$", stage), "II",
      grepl("^Stage III", stage), "III",
      grepl("^Stage IV", stage), "IV",
      default = NA_character_
    )]

    ## --- pathway level tables ---------------------------------------------
    cov <- data.table::fread(res("coverage.csv"))
    wts <- data.table::fread(res("weights.csv"))
    sig <- data.table::fread(res("significance.csv"))
    pdiag <- data.table::fread(res("pooled_diagnostics.csv"))

    lscore <- data.table::fread(res("layer_scores.csv"))
    lscore[, omics := factor(omics, levels = OMICS_LEVELS)]

    contrib <- data.table::fread(res("contributions.csv"))
    contrib[, omics := factor(omics, levels = OMICS_LEVELS)]

    list(
      scores = scores, raw = raw, cons = cons, pooled = pooled,
      annotation = an, manifest = manifest,
      coverage = cov, weights = wts, significance = sig,
      pooled_diag = pdiag, layer_scores = lscore, contributions = contrib,
      n_pathway = nrow(scores), n_sample = ncol(scores),
      feature_count = setNames(
        manifest$features,
        c(cna = "Copy number alteration", met = "Methylation",
          mrna = "Transcriptomics", protein = "Proteomics")[manifest$layer]
      )[OMICS_LEVELS]
    )
  })
}

## ------------------------------------------------------------ utilities -----
#' Wrap long KEGG pathway names for axis labels / heatmap rows.
wrap_pathway <- function(x, width = 30) {
  vapply(x, function(s) paste(strwrap(s, width = width), collapse = "\n"),
         character(1), USE.NAMES = FALSE)
}

fmt_p <- function(p) {
  ifelse(is.na(p), "NA",
         ifelse(p < 1e-16, "P < 1e-16", paste0("P = ", formatC(p, format = "g", digits = 2))))
}

fmt_q <- function(q) {
  ifelse(is.na(q), "NA",
         ifelse(q < 1e-16, "q < 1e-16", paste0("q = ", formatC(q, format = "g", digits = 2))))
}

#' Cohen-style threshold labels used in the manuscript captions.
sig_stars <- function(p) {
  cut(p, breaks = c(-Inf, 1e-4, 1e-3, 1e-2, 0.05, Inf),
      labels = c("****", "***", "**", "*", "ns"), right = FALSE)
}

#' Save a ggplot / patchwork object as a vector PDF plus a 600 dpi PNG.
#' Width and height are given in millimetres (manuscript convention).
save_fig <- function(plot, name, width_mm, height_mm, dpi = 600) {
  w <- width_mm / 25.4
  h <- height_mm / 25.4
  pdf_file <- file.path(OUT_DIR, paste0(name, ".pdf"))
  png_file <- file.path(OUT_DIR, paste0(name, ".png"))

  grDevices::cairo_pdf(pdf_file, width = w, height = h, family = "sans")
  print(plot)
  grDevices::dev.off()

  ragg::agg_png(png_file, width = w, height = h, units = "in",
                res = dpi, background = "white")
  print(plot)
  grDevices::dev.off()

  message(sprintf("  saved %s (.pdf + .png, %d x %d mm)", name, width_mm, height_mm))
  invisible(c(pdf = pdf_file, png = png_file))
}

#' ComplexHeatmap -> grob, so heatmaps can be laid out with patchwork.
as_panel <- function(ht, ...) {
  patchwork::wrap_elements(
    full = grid::grid.grabExpr(ComplexHeatmap::draw(ht, ...))
  )
}

#' z-score rows (used for all heatmaps)
row_z <- function(m) {
  s <- apply(m, 1, stats::sd, na.rm = TRUE)
  mu <- rowMeans(m, na.rm = TRUE)
  s[!is.finite(s) | s == 0] <- 1
  (m - mu) / s
}
