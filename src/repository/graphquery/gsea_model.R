require(GCModeller);
require(JSON);

imports "repository" from "kegg_kit";
imports "background" from "gseakit";

setwd(@dir);

idmaps = "./kegg_mapping.json"
|> readText()
|> json_decode()
;

"./hsa.db"
|> load.pathways(referenceMap = FALSE)
|> as.background( is.multipleOmics = TRUE, kegg.code = "hsa")
|> append.id_terms("GeneExpression", idmaps )
|> write.background(file = "./hsa.xml")
;