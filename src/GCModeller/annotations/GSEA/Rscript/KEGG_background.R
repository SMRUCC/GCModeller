imports "gseakit.background" from "gseakit";
imports "kegg.repository" from "kegg_kit";

let kegg = "E:\smartnucl_integrative\biodeepdb_v3\KEGG\br08901_pathwayMaps" :> load.maps(rawMaps = FALSE);
let mapping = "K:\20200226\TRN\1025_mapping+KO.csv" :> read.csv;


let background = KO.background(mapping, kegg, size = 4500, id_map = list(Reference = "term"));

background :> xml :> writeLines(con = "K:\20200226\20200516_gbk\KEGG\model.XML");
