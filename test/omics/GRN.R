require(GCModeller);

imports "geneExpression" from "phenotype_kit";

let hsa = geneExpression::load.expr("K:\hsa\Homo_sapiens_expr_advanced_all_conditions.csv") 
|> batch_normalize() 
|> top_variance(1000)
;
let top_genes = geneExpression::dims(hsa)$feature_names;

print("top variance gene features:");
print(top_genes);


