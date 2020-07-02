imports ["BIOM_kit", "taxonomy_kit"] from "metagenomics_kit";

setwd(!script$dir);

list.files("./", "*.biom")
:> sapply(path -> read.matrix(path))
:> biom.union
:> write.csv(file = "./16s_comunity.csv", row_names = FALSE)
;