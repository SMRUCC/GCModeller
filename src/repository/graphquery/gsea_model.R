require(GCModeller);
require(JSON);

imports "repository" from "kegg_kit";
imports "background" from "gseakit";

setwd(@dir);

# kegg_mapping.json for another gene id mapping
# examples as mapping to uniprot id

# idmaps = "./kegg_mapping.json"
# |> readText()
# |> json_decode()
# ;

"./hsa.hds"
|> load.pathways(referenceMap = FALSE)
|> as.background( is.multipleOmics = FALSE, kegg.code = "hsa")
# |> append.id_terms("GeneExpression", idmaps )
|> write.background(file = "./hsa.xml")
;