require(GCModeller);

imports "taxonomy_kit" from "metagenomics_kit";

let ncbi_tax = Ncbi.taxonomy_tree("D:\\datapool\\taxdump");

# 示例1：计算两个物种的LCA
let humanTaxid = 9606;  # Homo sapiens
let mouseTaxid = 10090; # Mus musculus
let ratTaxid   = 10116; # rat

str(ncbi_tax |> taxonomy_kit::LCA(c(humanTaxid, mouseTaxid, ratTaxid), min.supports = 0.5));