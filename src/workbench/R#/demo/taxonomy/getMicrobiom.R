imports ["BIOM_kit", "taxonomy_kit"] from "metagenomics_kit";

setwd(!script$dir);

let bioms = list.files("./hmqcp") :> lapply(file => read.matrix(file) :> biom.taxonomy);
let taxonomy = [];

for(list in bioms) {
	taxonomy = taxonomy << list;
}

taxonomy :> unique_taxonomy :> biom.string :> writeLines(con = "./gutMicrobiom.txt");

