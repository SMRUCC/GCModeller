imports "package_utils" from "devkit";

require(HDS);
require(JSON);
require(GCModeller);

# package_utils::attach(`${@dir}/../`);

const demo_file = "E:\biodeep\biodeepdb_v3\KEGG\pathway\pack\hsa.hdspack";

let buf = HDS::openStream(file = demo_file, readonly = TRUE);
let pathways = buf 
|> HDS::files("/pathways/", excludes_dir = TRUE)
|> sapply(file -> HDS::getData(buf, file) |> loadXml(typeof = "kegg_pathway"))
;

let metpa = metpa_background(pathways, taxonomy_name = NULL, raw = TRUE, multiple_omics = TRUE);

# print(json_encode(metpa));

metpa 
# |> json_encode()
|> json()
|> writeLines(
    con = `${@dir}/hsa_metpa.json`
)
;

# metpa 
# |> xml()
# |> writeLines(
#     con = `${@dir}/hsa_metpa.xml`
# )
# ;