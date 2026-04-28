require(GCModeller);

imports "brite" from "kegg_kit";

let brite = brite::parse("br08901");
let df = brite.as.table(brite);

print(df, max.print = 6);

write.csv(df, file = relative_work( "kegg_pathway_brite.csv"));