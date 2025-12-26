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

print(as.data.frame(sample_info));

let design = sample_info |> make.analysis(
        control = "Strain_Y203", 
        treatment = "High_Osmolarity");
let deg = limma(expr, design);

print(as.data.frame(deg));

let geneSet = JSON::json_decode(readText("gene_clusters.json"));
let kb = background::fromList(geneSet);
let result = kb |> enrichment([deg]::id, expression = -log10([deg]::adj_P_Val));

print(as.data.frame(result));

write.csv(result, file = "Strain_Y203_vs_High_Osmolarity.csv");
