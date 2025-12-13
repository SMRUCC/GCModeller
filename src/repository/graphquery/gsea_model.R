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

# hsa.hds
let pkg = ?"--pkg" || stop("no pathway package file!");
let save = file.path(dirname(pkg), `${basename(pkg)}.xml`);

pkg
|> load.pathways(referenceMap = FALSE)
|> as.background( omics = "Metabolomics", kegg.code = basename(pkg))
# |> append.id_terms("GeneExpression", idmaps )
|> write.background(file = save)
;