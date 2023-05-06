require(GCModeller);
require(HDS);

#' Toolkit for process the kegg brite text file
imports "brite" from "kegg_kit";
imports "dbget" from "kegg_kit";

let kegg_maps = as.data.frame(brite::parse("br08901"));
let cache = HDS::openStream(`${@dir}/KEGG_maps.pack`, allowCreate = TRUE);

print(kegg_maps, max.print = 13);

dbget::fetch_kegg_maps(cache);

close(cache);