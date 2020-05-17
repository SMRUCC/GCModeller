imports "gseakit.background" from "gseakit";
imports "kegg.repository" from "kegg_kit";

let kegg = "" :> load.maps(rawMaps = FALSE);
let mapping = "" :> read.csv;


let background = KO.background(mapping, kegg, size = 4500, id_map = list());

background :> xml :> writeLines(con = "");
