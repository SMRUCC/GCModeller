imports "kegg.brite" from "kegg_kit.dll";

["br08001"] 
:> brite.parse 
:> brite.as.table 
:> write.csv(file = "./br08001.csv");