imports "PFSNet" from "phenotype_kit";
imports "kegg.repository" from "kegg_kit";

# demo script for create LC-MS background for pfsnet analysis

setwd(!script$dir);

let links = reactions.table("D:\biodeep\biodeepdb_v3\KEGG\br08201.csv");

"D:\biodeep\biodeepdb_v3\KEGG\br08901_pathwayMaps"
:> load.maps
:> build.pathway_network(links)
:> save.pathway_network(file = "./reference_compounds.txt")
;