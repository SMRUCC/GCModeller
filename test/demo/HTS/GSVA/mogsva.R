require(GCModeller);

imports "GSVA" from "gseakit";
imports "background" from "gseakit";
imports "geneExpression" from "phenotype_kit";
imports "mixOmics" from "phenotype_kit";

let dataset = "K:\brca_tcga_pub2015\multipleOmics";
let multiple_layers = list(
    "Copy number alteration" = file.path(dataset,"cna.txt"),
    Methylation = file.path(dataset,"met.txt"),
    Transcriptomics = file.path(dataset,"mrna.txt"),
    Proteomics =  file.path(dataset,"protein.txt")
);
let opts = mogsva_opts(
    permutations = 1000,
    permutation_gene_limit = 5000,
    fusion_strategy = "Balanced",
    max_permutation_pathways = 200
);
let background = as.background(read_gmt("K:\brca_tcga_pub2015\multipleOmics\kegg_v3.gmt"));

multiple_layers = as.list(names(multiple_layers), names = names(multiple_layers)) 
|> lapply(function(name) {
    let filepath = multiple_layers[[name]];
    let mat = geneExpression::load.expr(filepath);
    let direction = ifelse(name == "Methylation", -1, 1);

    mixOmics::omics_layer(mat, name, direction );
})
|> GSVA::multiple_dataset()
;

let result = mogsva(multiple_layers ,background , opts );

mogsva_export(result, here("brca_tcga_pub2015_mogsva"))