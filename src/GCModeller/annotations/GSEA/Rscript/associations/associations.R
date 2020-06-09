imports "annotation.terms" from "seqtoolkit";
imports "kegg.metabolism" from "kegg_kit";
imports ["gseakit.background", "GSEA"] from "gseakit";
imports "gokit.file" from "gokit";

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

# print(KO);

let GO = synonym(KO, maps, excludeNull = TRUE) :> lapply(a -> as.object(a)$alias) :> unlist :> unique;

# print(GO); 

# enrich KO background
"D:\human\human\human_KO.generic.XML"
:> read.background
:> enrichment(KO, showProgress = FALSE)
:> enrichment.FDR
:> as.KOBAS_terms("KEGG Pathway")
:> write.enrichment(file = "D:/KO.csv")
;

"D:\human\human\human_GO.generic.XML"
:> read.background
:> enrichment.go(GO, read.go_obo("D:\human\go.obo"), showProgress = FALSE)
:> enrichment.FDR
:> as.KOBAS_terms("Gene Ontology")
:> write.enrichment(file = "D:/GO.csv")
;