imports "annotation.terms" from "seqtoolkit";
imports "kegg.metabolism" from "kegg_kit";

const kegg2go = "D:\human\human\human_kegg2go.txt";
const compounds2kegg = "D:\biodeep\biodeepdb_v3\KEGG\br08201.cacheIndex.txt";

let maps = kegg2go :> read.id_maps(skip2ndMaps = TRUE);
let mapKO = compounds2kegg :> load.reaction.cacheIndex :> as.object;

let compounds as string = readLines("D:\GCModeller\src\GCModeller\annotations\GSEA\Rscript\associations\kegg_id.txt");

# print(maps);

# print(mapKO);
# print(compounds);

let KO = mapKO$FindAllPoints(compounds);

KO = KO[KO == $"K\d+"];

print(KO);

let GO = synonym(KO, maps, excludeNull = TRUE) :> lapply(a -> as.object(a)$alias) :> unlist :> unique;

print(GO); 

