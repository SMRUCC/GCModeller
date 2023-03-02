require(kegg_api);
require(HDS);
require(GCModeller);

imports "package_utils" from "devkit";
imports "http" from "webKit";

const cache_dir = [?"--cache" || stop("No data cahce file!")] 
|> HDS::openStream(allowCreate = TRUE)
|> http::http.cache()
;
const reactions = kegg_api::listing("reaction", cache = cache_dir);

str(reactions);