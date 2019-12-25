imports ["kegg.profiles", "kegg.repository"] from "kegg_kit.dll";

let repo as string = "D:\biodeep\biodeep_v2\data\KEGG\KEGG_maps";
let maps = load.pathways(repo) :> compounds.pathway.index;
let kegg_compounds as string = readLines("D:\web\pos.txt");

let profiles = maps :> compounds.pathway.profiles(kegg_compounds);

profiles :> save.list(file = "./profiles.json");