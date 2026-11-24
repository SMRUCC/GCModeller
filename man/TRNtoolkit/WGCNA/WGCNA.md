# WGCNA

Workflow for make TRN data model based on the WGCNA co-expression network and TF id list.

+ [shapeTRN](WGCNA/shapeTRN.1) filter regulation network by WGCNA result weights
+ [interations](WGCNA/interations.1) append protein iteration network based on the WGCNA weights.
+ [expr_cor](WGCNA/expr_cor.1) Get the correlation matrix of the given expression data matrix, and then calculate the correlation value and p-value of the given id1 and id2.
+ [read_wgcna_edges](WGCNA/read_wgcna_edges.1) read network edges table which is save via igraph package
+ [prior_network](WGCNA/prior_network.1) Build bnlearn prior network based on the WGCNA co-expression network and TF id list.
+ [build_grn](WGCNA/build_grn.1) build the GRN prior network from the expression matrix or the cached bicor correlation store.
+ [string_links](WGCNA/string_links.1) locate the STRING protein interaction data files inside a STRING database
