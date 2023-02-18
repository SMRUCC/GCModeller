require(GCModeller);

#' GCModeller DEG experiment analysis designer toolkit
imports "sampleInfo" from "phenotype_kit";
#' the gene expression matrix data toolkit
imports "geneExpression" from "phenotype_kit";

setwd(@dir);

data = read.csv("../all_counts.csv", row.names = 1, check.names = FALSE);
sampleinfo = read.sampleinfo(file = "../sampleinfo.csv",
    tsv = FALSE,
    exclude.groups = NULL,
    id.makenames = FALSE);
	
for(sample in colnames(data)) {
	v = data[, sample];
	v = v / sum(v) * [10 ^8];
	v[v == 0.0] = min(v[v > 0]) / 2;
	data[,sample] = v;
}

writeLines(rownames(data), con = "./geneIds.txt");

data = geneExpression::average(load.expr(data), sampleinfo);

write.expr_matrix(expr = data, file = "./ath_norm.csv",
    id = "geneID",
    binary = FALSE);
