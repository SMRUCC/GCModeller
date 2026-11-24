imports "visualkit.plots" from "visualkit";
imports ["geneExpression", "sampleInfo"] from "phenotype_kit";

setwd(!script$dir);

let expr0 = read.csv("msms_Intensity.csv", row_names = 1);

expr0[, "mz"] = NULL;
expr0[, "rt"] = NULL;

print("we have all sample labels:");
print(colnames(expr0));

let patterns = expr0
:> load.expr
:> relative
:> expression.cmeans3D(fuzzification = 10, threshold = 10)
;

print("view patterns result:");
print(patterns);

patterns
:> plot.cmeans3D(size = [6000, 5000], colorSet = "red,blue,green", viewDistance = 60000, qDisplay = 0.99)
:> save.graphics(file = "./cmeans3D.png")
;

patterns
:> cmeans_matrix(kmeans_n = 5)
:> write.csv(file = "./cmeans3D.csv")
;