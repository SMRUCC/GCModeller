require(kegg_graphquery);
require(HDS);
require(GCModeller);

imports "package_utils" from "devkit";
imports "http" from "webKit";

const cache_dir = [?"--cache" || stop("No data cahce file!")] 
|> HDS::openStream(allowCreate = TRUE)
|> http::http.cache()
;
const reactions = "https://rest.kegg.jp/list/reaction"
|> requests.get()
|> toString()
;

print(reactions);