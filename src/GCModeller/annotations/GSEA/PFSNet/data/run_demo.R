imports "PFSNet" from "phenotype_kit";

setwd(!script$dir);

pfsnet(
load.expr("./data/sample_control.txt"), 
load.expr("./data/sample_test.txt"), 
load.pathway_network("./data/pathways.txt"), 
t1=0.8, 
t2=0.8, 
n=10
)

:> xml
:> writeLines(con = "./data/demo.xml")
;