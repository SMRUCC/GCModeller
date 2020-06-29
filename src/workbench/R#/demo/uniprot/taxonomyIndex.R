imports "ptfKit" from "proteomics_toolkit";

setwd(!script$dir);

"./uniprot_KEGG_all.ptf"
:> load.ptf 
:> ptf.split(key = "ncbi_taxonomy", outputdir = "./taxonomy")
;