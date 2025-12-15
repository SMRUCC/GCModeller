require(GCModeller);

imports ["geneExpression" "sampleInfo"] from "phenotype_kit";

setwd(@dir);

let expr_data = load.expr("expr_demo.csv");
let sampleinfo = sampleInfo(sample_id(expr_data), sample_id(expr_data));
let a_vs_b = make.analysis(sampleinfo, "Control", "Treat"); 

str(expr_data);
str(a_vs_b);

let deg = limma(expr_data, a_vs_b);

deg = deg[order([deg]::P_Value)];

# print(as.data.frame(deg));

write.csv(deg, file = "./limma_impl_degs.csv");