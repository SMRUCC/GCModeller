imports "kegg.repository" from "kegg_kit";

setwd(!script$dir);

let prokaryote = fetch.kegg_organism(NULL, type = "prokaryote");

write.csv(prokaryote, file = "./bacterials.csv", row_names = FALSE);

