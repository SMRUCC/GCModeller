require(GCModeller);

imports "ptf" from "annotationKit";

let table = read_eggNOG("F:\datapool\20260312\20260424\emapper.annotations");

print(as.data.frame(table));

write.csv(as.data.frame(table), file = "F:\datapool\20260312\20260424/eggNOG.csv");