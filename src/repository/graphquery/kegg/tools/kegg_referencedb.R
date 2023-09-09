require(GCModeller);
require(kegg_api);

imports "brite" from "kegg_kit";

let brite = brite::parse("br08901");
let df = as.data.frame(brite);
 
print(df, max.print = 6);

for(map in as.list(df, byrow = TRUE)) {
    let pwy = kegg_api::kegg_pathway(`map${map$entry}`);

    str(pwy);
    str(map);
}