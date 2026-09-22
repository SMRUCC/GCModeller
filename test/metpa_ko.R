imports "package_utils" from "devkit";

require(HDS);
require(JSON);

package_utils::attach(`${@dir}/../`);

let metpa = metpa_background(GCModeller::kegg_maps(), taxonomy_name = NULL, raw = TRUE);

# print(json_encode(metpa));

metpa 
|> json_encode()
|> writeLines(
    con = `${@dir}/ko_metpa.json`
)
;