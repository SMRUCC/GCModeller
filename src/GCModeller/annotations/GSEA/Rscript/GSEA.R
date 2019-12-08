imports "gseakit.background" from "gseakit.dll";
imports "GSEA" from "gseakit.dll";
imports "gokit.file" from "gokit.dll";

let background <- ?"--background" :> read.background;  
let geneSet as string <- ?"--geneSet";
let save.csv as string <- ?"--save" || `${dirname(geneSet)}/{basename(geneSet)}.enrichment.csv`;
let go <- ?"--go"; 

geneSet <- readLines(geneSet);

if (is.empty(go)) {
    print("Do GSEA analysis with background:");
} else {
    go <- read.go_obo(go);
    print("Do GO enrichment analysis with background:");
}

print(background);

if (is.empty(go)) {
    background
    :> enrichment(geneSet)
    :> write.enrichment(file = save.csv);
} else {
    background
    :> enrichment.go(geneSet, go)
    :> write.enrichment(file = save.csv);
}