imports "PFSNet" from "phenotype_kit";

setwd(!script$dir);

"demo.xml"
:> read.pfsnet_result
:> as.data.frame
:> write.csv(file = "./demo.csv", row_names = FALSE)
;