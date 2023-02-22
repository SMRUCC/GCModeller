imports "package_utils" from "devkit";

require(HDS);

package_utils::attach(`${@dir}/../`);

let pathways = HDS::openStream(file = "D:\biodeep\biodeepdb_v3\KEGG\pathway\pack\hsa.hdspack", readonly = TRUE);
let metpa = metpa_background(pathways, taxonomy_name = NULL, raw = TRUE);