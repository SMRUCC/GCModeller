require(GCModeller);

imports ["GSEA", "background"] from "gseakit";

data = read.csv("D:\metagenome.csv", row.names = 1);
data[, "taxonomy"] = NULL;
background = KO_reference();

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