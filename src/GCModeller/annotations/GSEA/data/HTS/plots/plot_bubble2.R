require(GCModeller);

imports "GSEA" from "gseakit";
imports "visualPlot" from "visualkit";

setwd(@dir);

let enrichments = read.csv("../kegg_enrichment.xls", tsv = TRUE, check.names = FALSE, row.names = FALSE);

print(enrichments);