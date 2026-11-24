imports "kegg" from "bioconductor";
imports "kegg.repository" from "kegg_kit";

load.maps("D:\biodeep\biodeepdb_v3\KEGG\br08901_pathwayMaps")
:> write.keggMap.rds(saveRDS = "D:\biodeep\biodeep_pipeline\KEGGRepository\data\kegg_maps.rds")
;