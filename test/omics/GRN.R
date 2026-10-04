require(GCModeller);

imports "geneExpression" from "phenotype_kit";
imports "TRN" from "cytoscape";
imports "WGCNA" from "TRNtoolkit";

let hsa = geneExpression::load.expr("K:\hsa\Homo_sapiens_expr_advanced_all_conditions.csv") 
|> batch_normalize() 
|> top_variance(1000)
;
let top_genes = geneExpression::dims(hsa)$feature_names;
let TF = read.csv("K:\hsa_grn\Homo_sapiens_TF.txt")$Ensembl;

print("top variance gene features:");
print(top_genes);
print("hsa TF vector:");
print(TF);

write_bicor(hsa, repo = "Z:/hsa_mat");

let bicor = open_bicor("Z:/hsa_mat");
let opts = new("GRN_opts", minAbsCorrelation = 0.3,  enableGpu = TRUE);
let grn = WGCNA::build_grn(hsa, TF, 
        opts = opts |> string_links(string_db = "K:\hsa_grn\string-db"),
        bicor = bicor
);

