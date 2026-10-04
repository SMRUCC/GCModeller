require(GCModeller);

imports "geneExpression" from "phenotype_kit";
imports "TRN" from "cytoscape";
imports "WGCNA" from "TRNtoolkit";

let TF = read.table("K:\hsa_grn\Homo_sapiens_TF.txt", header = TRUE);

print(TF, max.print = 6);

let hsa = geneExpression::load.expr("K:\hsa\Homo_sapiens_expr_advanced_all_conditions.csv") 
|> batch_normalize() 
|> top_variance(1000)
;
let top_genes = geneExpression::dims(hsa)$feature_names;

print("top variance gene features:");
print(top_genes);
print("hsa TF vector:");
print(TF$Ensembl);

write_bicor(hsa, repo = "Z:/hsa_mat");

let bicor = open_bicor("Z:/hsa_mat");
let opts = new("GRN_opts", minAbsCorrelation = 0.3,  enableGpu = TRUE);
let grn = WGCNA::build_grn(hsa, TF$Ensembl, 
        opts = opts |> string_links(string_db = "K:\hsa_grn\string-db"),
        bicor = bicor
);

