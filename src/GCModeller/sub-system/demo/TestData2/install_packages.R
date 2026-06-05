#!/usr/bin/env Rscript
###############################################################################
# Install all required R packages for the bnlearn plotting script
###############################################################################

packages <- c(
  "ggplot2", "reshape2", "RColorBrewer", "scales", "ggrepel",
  "grid", "gridExtra", "pheatmap", "ComplexHeatmap", "circlize",
  "dplyr", "tidyr", "ggpubr", "viridis", "cowplot",
  "ggdendro", "igraph", "fmsb"
)

installed <- rownames(installed.packages())

for (pkg in packages) {
  if (pkg %in% installed) {
    message(sprintf("  [OK] %s is already installed", pkg))
  } else {
    message(sprintf("  [Installing] %s ...", pkg))
    install.packages(pkg, repos = "https://cloud.r-project.org", quiet = TRUE)
  }
}

# Verify
message("\n--- Verification ---")
for (pkg in packages) {
  status <- if (requireNamespace(pkg, quietly = TRUE)) "OK" else "FAILED"
  library(pkg,  character.only=TRUE);
  message(sprintf("  %s: %s", pkg, status))
}
message("\nAll package installations complete!")

