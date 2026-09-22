imports "ptfKit" from "proteomics_toolkit";

setwd(!script$dir);

"uniprotKB_sprot.ptf" 
:> load.ptf 
:> filter("ko") 
:> save.ptf(file = "./uniprot_KEGG_all.ptf")
;