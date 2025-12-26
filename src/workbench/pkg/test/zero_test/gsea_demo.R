require(GCModeller);

setwd(@dir);

imports ["geneExpression", "sampleInfo"] from "phenotype_kit";
imports ["background", "GSEA"] from "gseakit";

let expr = load.expr("expression_matrix.csv")
|> filterZeroGenes() 
|> impute_missing(by.features=TRUE) 
|> totalSumNorm() 
|> geneExpression::relative(median = TRUE)
;
let sample_info = read.csv("sample_metadata.csv", row.names = NULL, check.names = FALSE);

sample_info = sampleInfo(
    ID = sample_info$Sample,
    sample_info = sample_info$Group
);

cat("\n");
message("inspect of the sample group information:");
print(as.data.frame(sample_info), max.print = 6);

let design = sample_info |> make.analysis(
        control = "Strain_Y203", 
        treatment = "High_Osmolarity");
let deg = limma(expr, design);
let i = ([deg]::class == "up") || ([deg]::class == "down");

cat("\n");
message("view of the DEG analysis result which is measured via the limma:");
print(as.data.frame(deg), max.print = 6);

deg = deg[i];

let geneSet = JSON::json_decode(readText("gene_clusters.json"));
let kb = background::fromList(geneSet);
let result = kb |> enrichment([deg]::id, expression = -log10([deg]::adj_P_Val),
    permutations = 5000);

result = as.data.frame(result);
result[,"class"] =NULL;
result[,"category"]=NULL;

cat("\n");
message("get GSEA analysis output:");
print(result);

write.csv(result, file = "Strain_Y203_vs_High_Osmolarity.csv");
