require(GCModeller);

setwd(@dir);

"./test.xml"
|> readText()
|> loadXml(typeof = "kegg_compound")
|> as.list()
|> str()
;