imports "PFSNet" from "phenotype_kit";

setwd(!script$dir);

pfsnet(
load.expr("./python/sample_control.txt"), 
load.expr("./python/sample_test.txt"), 
load.pathway_network("./python/pathways.txt"), 
t1=0.8, 
t2=0.7, 
n=10
);