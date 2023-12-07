require(kegg_api);
require(JSON);

setwd(@dir);

let id_list = "./list.json"
|> readText()
|> JSON::json_decode()
;

options(http.cache_dir = ?"--cache" || `${@dir}/.cache/`);

const kegg_compounds = list();

str(id_list);

for(id in id_list$kegg) {
    str(id);
    kegg_compounds[[id]] = kegg_compound(id, cache = "./.cache/");
}


repository::write.msgpack(unlist(kegg_compounds), file = "./kegg_parts.msgpack");