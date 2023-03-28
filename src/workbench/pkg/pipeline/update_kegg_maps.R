require(GCModeller);

#' Toolkit for process the kegg brite text file
imports "brite" from "kegg_kit";

kegg_maps = as.data.frame(brite::parse("br08901"));

print(kegg_maps, max.print = 13);