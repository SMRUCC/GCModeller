imports "ptfKit" from "proteomics_toolkit";

setwd(!script$dir);

"./uniprot_KEGG_all.ptf"
:> load.ptf 
:> filter("ko") 
:> ptf.split(key = "ncbi_taxonomy", outputdir = "./taxonomy")
;