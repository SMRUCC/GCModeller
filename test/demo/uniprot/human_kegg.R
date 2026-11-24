imports "ptfKit" from "proteomics_toolkit";

setwd(!script$dir);

"human.ptf" 
:> load.ptf 
:> filter("ko") 
:> save.ptf(file = "./human.ko.ptf")
;