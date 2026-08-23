require(GCModeller);
require(Matrix);
require(igraph);

imports "WGCNA" from "phenotype_kit";

let colors = read.module_cor("C:\Users\Administrator\Downloads\WGCNA_output\gene_module_assignment.csv");
let adj = read.adjacency("C:\Users\Administrator\Downloads\WGCNA_output\adjacency_matrix.csv");
let output = "K:/hsa_grn/";

writeBin(adj, con = file.path(output, "hsa_adj.dat"));

adj 
|> cor_network(membership = colors, adjacency = 0.6)
|> igraph::save.network(file = output)
;