require(GCModeller);

imports ["profiles", "GSEA", "background"] from "gseakit";
imports "visualPlot" from "visualkit";

data = read.csv("D:\metagenome.csv", row.names = 1);
data[, "taxonomy"] = NULL;

str(data);

for(i in colnames(data)) {
	v = data[, i];
	v[v == 0.0] = min(v[v > 0]) / 2;
	
	data[, i] = v;
}

A = data[, "ZTPSN21ES857.1038"];
B = data[, "ZTPSN21ES858.1039"];
log2 = log(A / B, 2);

print(log2);

KO = rownames(data);
KO = KO[abs(log2) > 2];

print(KO);

enrich = KO_reference()
|> enrichment(geneSet = KO, outputAll = FALSE)
|> enrichment.FDR()
;

i = sapply(enrich, function(d) as.object(d)$pvalue < 0.01);
enrich = enrich[i];

data = enrich 
|> as.data.frame()
;

data[, "description"] = NULL;
data[, "geneIDs"] = NULL;

data
|> print(max.print = 20)
;

bitmap(file = `${@dir}/enriched.png`) {
	enrich 
	|> as.KOBAS_terms
	|> KEGG.enrichment.profile(top = 7)
	|> category_profiles.plot(size = [3000, 2100], title = "KEGG pathway enrichment", axis_title = "-log10(P-value)")
	;
}



