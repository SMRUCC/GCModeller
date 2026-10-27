source("G:/GCModeller/test/demo/HTS/GSVA/mogsva_figures/00_common.R")
d <- load_mogsva()
cov <- d$coverage
print(table(cov$omics, cov$member_count == 0))
cat("member_count range measured:", range(cov$member_count[cov$member_count>0]), "\n")
cat("weights NA:", sum(is.na(d$weights$weight)), " weight range:", range(d$weights$weight, na.rm=TRUE), "\n")
print(d$weights[is.na(weight)])
cat("n skipped true:", sum(d$weights$skipped), " measured w n:", nrow(d$weights[skipped==FALSE]), "\n")
print(d$coverage[pathway=="Biotin metabolism"])
