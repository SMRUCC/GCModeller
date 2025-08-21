require(GCModeller);

imports ["geneExpression" "sampleInfo"] from "phenotype_kit";

setwd(@dir);

let expr_data = load.expr("expr_demo.csv");
let sampleinfo = sampleinfo(colnames(expr_data), colnames(expr_data));

print(expr_data);

let deg = limma(expr_data, )