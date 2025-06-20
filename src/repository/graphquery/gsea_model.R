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

"./eco.hds"
|> load.pathways(referenceMap = FALSE)
|> as.background( omics = "Metabolomics", kegg.code = "eco")
# |> append.id_terms("GeneExpression", idmaps )
|> write.background(file = "./eco.xml")
;