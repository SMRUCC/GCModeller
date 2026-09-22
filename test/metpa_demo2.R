imports "package_utils" from "devkit";

require(HDS);
require(JSON);

package_utils::attach(`${@dir}/../`);

const demo_file = "F:\kegg\kegg\ath_result.json";
const demo_data = demo_file
|> readText()
|> JSON::json_decode()
;
const idset = lapply(demo_data$msetList$list, x -> x$kegg_id);

str(idset);
# stop();


# let buf = HDS::openStream(file = demo_file, readonly = TRUE);
# let pathways = buf 
# |> HDS::files("/pathways/", excludes_dir = TRUE)
# |> sapply(file -> HDS::getData(buf, file) |> loadXml(typeof = "kegg_pathway"))
# ;

let metpa = metpa_background(idset, taxonomy_name = "ath", raw = TRUE);

# print(json_encode(metpa));

metpa 
|> json_encode()
|> writeLines(
    con = `${@dir}/ath_metpa.json`
)
;