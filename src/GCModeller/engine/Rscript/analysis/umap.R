imports "package_utils" from "devkit";
imports "umap" from "MLkit";

package_utils::attach("G:\GCModeller\src\workbench\pkg");

const rawfile = "G:\cell-render\test\result.vcellPack";

setwd(@dir);

imports "rawXML" from "vcellkit";
imports "geneExpression" from "phenotype_kit";

rawfile 
|> open.vcellPack(mode = "read")
|> time.frames(module = "Metabolites")
|> write.expr_matrix(file = "./test.csv")
;

const u3d = umap::umap(read.csv("./test.csv", row.names = 1, check.names = FALSE), dimension = 3);
const scatter = as.data.frame(u3d$umap, labels = u3d$labels);

write.csv(scatter, file = "./umap3.csv", row.names  =TRUE);




