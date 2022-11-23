require(GCModeller);

imports "repository" from "kegg_kit";
imports "background" from "gseakit";

setwd(@dir);

"./hsa.db"
|> load.pathways(referenceMap = FALSE)
|> as.background( is.multipleOmics = TRUE)
|> write.background(file = "./hsa.xml")
;