# =============================================================================
# run_all.R -- regenerate every manuscript figure
#
#   Rscript run_all.R
#
# Outputs land in ./output as one vector PDF and one 600 dpi PNG per figure.
# Paths can be overridden through the MOGSVA_RESULT_DIR / MOGSVA_RAW_DIR
# environment variables (see 00_common.R).
# =============================================================================

SCRIPT_DIR <- Sys.getenv("MOGSVA_SCRIPT_DIR",
                         unset = "G:/GCModeller/test/demo/HTS/GSVA/mogsva_figures")

figures <- c(
  "fig1_input_landscape.R",
  "fig2_activity_landscape.R",
  "fig3_cross_omics_consistency.R",
  "fig4_omics_contributions.R",
  "fig5_ablation_pooled_baseline.R",
  "fig6_significance_and_power.R",
  "fig7_clinical_association.R"
)

t_start <- Sys.time()

for (f in figures) {
  message("== ", f)
  source(file.path(SCRIPT_DIR, f), local = new.env())
}

# record the exact environment used for the figures
writeLines(capture.output(sessionInfo()),
           file.path(SCRIPT_DIR, "output", "session_info.txt"))

message(sprintf("all figures rebuilt in %.1f min",
                as.numeric(difftime(Sys.time(), t_start, units = "mins"))))
