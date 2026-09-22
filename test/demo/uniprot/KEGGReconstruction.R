imports ["kegg.metabolism", "kegg.repository"] from "kegg_kit";
imports "ptfKit" from "proteomics_toolkit";

let reactions = reactions.table("E:\smartnucl_integrative\biodeepdb_v3\KEGG\br08201.csv");
let proteins = load.ptf("E:\smartnucl_integrative\auto_report\uniprot_KEGG_all\taxonomy\9606.ptf");

"E:\smartnucl_integrative\biodeepdb_v3\KEGG\br08901_pathwayMaps"
:> load.maps
:> kegg.reconstruction(reactions, proteins, min_cov = 0.1)
:> save.KEGG_pathway(file = "./test.XML")
;