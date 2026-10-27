source("G:/GCModeller/test/demo/HTS/GSVA/mogsva_figures/00_common.R")
d <- load_mogsva(); S <- d$scores; R <- d$raw; C <- d$cons
rho <- sapply(seq_len(nrow(S)), function(i) cor(R[i,], S[i,], method="spearman"))
cat("rho quantiles:/n"); print(quantile(rho, c(0,.05,.25,.5,.75,.95,1)))
cat("pathways rho<0.8:", sum(rho<0.8), " <0.9:", sum(rho<0.9), "\n")
pear <- sapply(seq_len(nrow(S)), function(i) cor(R[i,], S[i,]))
cat("pearson quantiles:", quantile(pear, c(0,.05,.5,1)), "\n")
# attenuation factor by agreement
a <- as.vector(C); fac <- (1+0.25*a)/1.25
print(tapply(fac, round(a,3), median))
