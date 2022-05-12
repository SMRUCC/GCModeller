require(GCModeller);

imports "BioCyc" from "annotationKit";
imports "background" from "gseakit";

bg = "E:\metacyc\26.0\data"
|> open.biocyc()
|> createBackground()
;

write.background(bg, file = `${@dir}/metacyc.XML`)