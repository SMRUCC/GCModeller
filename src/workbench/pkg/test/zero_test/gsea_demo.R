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
let sample_info = sampleInfo::guess.sample_groups(
    matrix_info(expr)$sampleID, 
    raw.list = FALSE,
    maxDepth = TRUE
);

print(as.data.frame(sample_info));

let design = sample_info |> make.analysis(
        control = "High_Glucose_Rep", 
        treatment = "Low_Glucose_Rep");
let deg = limma(expr, design);

print(as.data.frame(deg));

let geneSet = JSON::json_decode(readText("gene_clusters.json"));
let kb = background::fromList(geneSet);
let result = kb |> enrichment([deg]::id, expression = -log10([deg]::adj_P_Val));

print(as.data.frame(result));

write.csv(result, file = "High_Glucose_Rep_vs_Low_Glucose_Rep.csv");
