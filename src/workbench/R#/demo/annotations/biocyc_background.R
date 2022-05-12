require(GCModeller);

imports "BioCyc" from "annotationKit";

"E:\metacyc\26.0\data"
|> open.biocyc()
|> createBackground()
;