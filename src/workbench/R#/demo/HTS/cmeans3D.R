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
:> average(sampleinfo)
:> relative
:> expression.cmeans3D(fuzzification = 5, threshold = 0.001)
;

print("view patterns result:");
print(patterns);

patterns
:> plot.cmeans3D(size = [6000, 4500], colorSet = "red,blue,green")
:> save.graphics(file = "./patterns.png")
;

patterns
:> cmeans_matrix
:> write.csv(file = "./patterns.csv")
;