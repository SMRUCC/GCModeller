require(GCModeller);
require(kegg_api);
require(HDS);

imports "brite" from "kegg_kit";

let brite = brite::parse("br08901");
let df = as.data.frame(brite);
 
const cache_dir = `${@dir}/cache.db`
|> HDS::openStream(allowCreate = TRUE)
|> http::http.cache()
;
const cache_fs = [cache_dir]::fs;

print(df, max.print = 6);

for(map in as.list(df, byrow = TRUE)) {
    let pwy = kegg_api::kegg_pathway(`map${map$entry}`, cache = cache_dir);
    let dir = `/${map$class}/${map$category}/${map$entry} - ${map$name}/`;

    HDS::writeText(cache_fs, `${dir}/map.xml`, xml(pwy));
    HDS::flush(cache_fs);

    let modules = as.data.frame([pwy]::modules);

    print(modules);

    str(pwy);
    str(map);

    stop();
}