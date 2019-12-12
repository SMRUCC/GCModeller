imports "gseakit.background" from "gseakit.dll";
imports "GSEA" from "gseakit.dll";
imports "gokit.file" from "gokit.dll";

# Get input value from commandlines arguments
# background model
# and target geneSet for GSEA analysis
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

# do GSEA analysis
if (is.empty(go)) {
    background
    :> enrichment(geneSet)
    :> enrichment.FDR
    :> write.enrichment(file = save.csv);
} else {
    background
    :> enrichment.go(geneSet, go)
    :> enrichment.FDR
    :> write.enrichment(file = save.csv);
}