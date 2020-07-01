imports ["BIOM_kit", "taxonomy_kit"] from "metagenomics_kit";

setwd(!script$dir);

print(read.matrix("biom\EP733790_K70_BS1D.otu_table.biom") :> as.data.frame);

let biom = read.matrix("biom\hmqcp.otu_table.biom.json");
let OTUtable = biom :> as.data.frame; 

print(rownames(OTUtable));

print(biom :> biom.taxonomy :> biom.string);