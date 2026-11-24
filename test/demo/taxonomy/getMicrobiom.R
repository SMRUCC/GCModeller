imports ["BIOM_kit", "taxonomy_kit"] from "metagenomics_kit";

setwd(!script$dir);

let bioms = list.files("./hmqcp");
let taxonomy = [];

print(basename(bioms));

bioms = bioms :> lapply(file => read.matrix(file) :> biom.taxonomy);

for(list in bioms) {
	taxonomy = taxonomy << list;
}

taxonomy :> unique_taxonomy :> biom.string :> writeLines(con = "./gutMicrobiom.txt");

