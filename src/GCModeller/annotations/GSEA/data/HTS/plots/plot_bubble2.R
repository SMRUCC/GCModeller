require(GCModeller);

imports "GSEA" from "gseakit";
imports "visualPlot" from "visualkit";

setwd(@dir);

let enrichments = read.csv("../kegg_enrichment.xls", tsv = TRUE, check.names = FALSE, row.names = FALSE);
let sig =  enrichments[, "FDR"] < 0.05;

enrichments = enrichments[sig, ];

print(enrichments);

enrichments = GSEA::cast_enrichs(
    term = enrichments$pathway,
            name = enrichments$term,
             pvalue = enrichments[, "Raw p"] ,                                          
                     desc = enrichments$term,
                         score = enrichments$Impact,
                             fdr = enrichments$FDR,
                             enriched = enrichments$Hits
);

svg(file = "./bubble2.svg") {
    kegg.enrichment.bubble2(enrichments, size = [8000,12000]);
}