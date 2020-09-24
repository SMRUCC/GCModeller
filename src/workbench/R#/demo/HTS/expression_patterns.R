imports "visualkit.plots" from "visualkit";
imports ["geneExpression", "sampleInfo"] from "phenotype_kit";

setwd(!script$dir);

let expr0 = read.csv("msms_Intensity.csv", row_names = 1);

expr0[, "mz"] = NULL;
expr0[, "rt"] = NULL;

expr0
:> load.expr
:> average()
:> relative
:> expression.cmeans_pattern(dim = [3, 4])
:> plot.expression_patterns(size = [4000,2400])
:> save.graphics(file = "./patterns.png")
;