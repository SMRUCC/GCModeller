imports "BIOM_kit" from "metagenomics_kit";

setwd(!script$dir);

read.matrix("biom\EP733790_K70_BS1D.otu_table.biom") :> as.data.frame :> print;