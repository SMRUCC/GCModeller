imports ["BIOM_kit", "taxonomy_kit"] from "metagenomics_kit";

setwd(!script$dir);

list.files("./hmqcp", "*.biom")
:> as.pipeline
:> projectAs(path -> read.matrix(path))
:> ctype("biom.matrix")
:> biom.union
:> write.csv(file = "./16s_comunity.csv", row_names = FALSE)
;

