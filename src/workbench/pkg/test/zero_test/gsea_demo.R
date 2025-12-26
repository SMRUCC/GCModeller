require(GCModeller);

setwd(@dir);

imports ["geneExpression", "sampleInfo"] from "phenotype_kit";

let expr = load.expr("expression_matrix.csv")
|> filterZeroGenes() 
|> impute_missing(by.features=TRUE) 
|> totalSumNorm() 
|> geneExpression::relative(median = TRUE)
;
let sample_info = sampleInfo::guess.sample_groups(matrix_info(expr)$sampleID, 
                        raw.list = FALSE,
                        maxDepth = TRUE);

print(as.data.frame(sample_info));