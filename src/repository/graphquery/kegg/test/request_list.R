require(kegg_api);
require(JSON);

setwd(@dir);

let id_list = "./list.json"
|> readText()
|> JSON::json_decode()
;

options(http.cache_dir = ?"--cache" || `${@dir}/.cache/`);

str(id_list);

for(id in id_list$kegg) {
    str(id);
}
