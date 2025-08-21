require(GCModeller);

imports ["geneExpression" "sampleInfo"] from "phenotype_kit";

setwd(@dir);

let expr_data = load.expr("expr_demo.csv");
let sampleinfo = sampleInfo(colnames(expr_data), colnames(expr_data));
let a_vs_b = make.analysis(sampleinfo, "Control", "Treat"); 

str(expr_data);
str(a_vs_b);

let deg = limma(expr_data, a_vs_b);

print(as.data.frame(deg));