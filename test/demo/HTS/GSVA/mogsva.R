require(GCModeller);

imports "GSVA" from "gseakit";
imports "background" from "gseakit";
imports "geneExpression" from "phenotype_kit";
imports "mixOmics" from "phenotype_kit";

let multiple_layers = list(
    "Copy number alteration" = "K:\brca_tcga_pub2015\multipleOmics\cna.txt"
    Methylation = "K:\brca_tcga_pub2015\multipleOmics\met.txt"
    Transcriptomics = "K:\brca_tcga_pub2015\multipleOmics\mrna.txt"
    Proteomics = "K:\brca_tcga_pub2015\multipleOmics\protein.txt"
);
let opts = mogsva_opts(
    permutations = 100,
    permutation_gene_limit = 1000
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

