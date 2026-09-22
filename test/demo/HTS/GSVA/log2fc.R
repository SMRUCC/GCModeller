require(GCModeller);

#' GCModeller DEG experiment analysis designer toolkit
imports "sampleInfo" from "phenotype_kit";
#' the gene expression matrix data toolkit
imports "geneExpression" from "phenotype_kit";

setwd(@dir);

gsva = load.expr(file = "./gsva.csv",
    exclude.samples = NULL,
    rm.ZERO = FALSE,
    makeNames = FALSE);

sampleinfo = read.sampleinfo(file = "../sampleinfo.csv",
    tsv = FALSE,
    exclude.groups = NULL,
    id.makenames = FALSE);

diff = function(a, b) {
# log2 fc of the gsva
gsva 
|> deg.t.test(sampleinfo, treatment = a, control = b,
    level = 0,
    pvalue = 1,
    FDR = 1)
    |> as.data.frame()
    |> write.csv(file = `./${a}_vs_${b}.csv`, row.names = TRUE);
}

diff("C6","C9");
diff("I56","I59");
diff("I86","I89");







