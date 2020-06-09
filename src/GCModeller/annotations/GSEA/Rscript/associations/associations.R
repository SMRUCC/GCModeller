imports "annotation.terms" from "seqtoolkit";

const kegg2go = "D:\human\human\human_kegg2go.txt";

let maps = kegg2go :> read.id_maps(skip2ndMaps = TRUE);

print(maps);