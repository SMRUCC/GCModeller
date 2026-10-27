suppressMessages(library(data.table))
RES <- "G:/GCModeller/test/demo/HTS/GSVA/brca_tcga_pub2015_mogsva"
rd <- function(p){dt<-fread(p);m<-as.matrix(dt[,-1]);rownames(m)<-dt[[1]];m}
S <- rd(file.path(RES,"scores.csv")); P <- rd(file.path(RES,"pooled_scores.csv"))
cat("pooled-only:/n"); print(setdiff(rownames(P), rownames(S)))
cat("scores-only count:", length(setdiff(rownames(S), rownames(P))), "\n")
cat("dup pooled:", any(duplicated(rownames(P))), "\n")
