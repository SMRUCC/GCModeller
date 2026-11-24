require(GCModeller);
require(Matrix);
require(igraph);

imports "WGCNA" from "phenotype_kit";

let colors = read.module_cor("K:\hsa\WGCNA_output\gene_module_assignment.csv");
let adj = read.adjacency("K:\hsa\WGCNA_output\adjacency_matrix.csv");
let output = "K:\hsa\WGCNA_output\cor_network";

writeBin(adj, con = file.path(output, "hsa_adj.dat"));
writeLines(summary(adj), con = file.path(output, "hsa_adj_summary.txt"));

adj 
|> cor_network(membership = colors, adjacency = 1e-3)
|> igraph::save.network(file = output)
;