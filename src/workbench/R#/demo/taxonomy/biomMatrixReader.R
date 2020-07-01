imports "BIOM_kit" from "metagenomics_kit";

setwd(!script$dir);

print(read.matrix("biom\EP733790_K70_BS1D.otu_table.biom") :> as.data.frame);
print(read.matrix("biom\hmqcp.otu_table.biom.json") :> as.data.frame);