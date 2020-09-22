imports "visualkit.plots" from "visualkit";
imports "geneExpression" from "phenotype_kit";

setwd(!script$dir);

"msms_Intensity.csv"
:> load.expr
:> relative
:> expression.cmeans_pattern(dim = [4,4])
:> plot.expression_patterns()
:> save.graphics(file = "./patterns.png")
;