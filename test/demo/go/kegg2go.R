imports ["annotation.terms", "uniprot"] from "seqtoolkit";
imports "go.annotation" from "gokit";

"D:\human\human\human.xml"
:> open.uniprot
:> uniprot.ko2go
:> write.id_maps(file = "D:\human\human\human_kegg2go.txt")
;