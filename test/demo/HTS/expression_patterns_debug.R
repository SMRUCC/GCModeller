imports "visualPlot" from "visualkit";
imports ["geneExpression", "sampleInfo"] from "phenotype_kit";

setwd(!script$dir);

let expr0 = read.csv("E:/label-free/demo/analysis/03.dep_analysis/C6 vs I56/pvalue_cut.csv", row_names = 1);

expr0[, "FC.avg"]  = NULL;
expr0[, "p.value"] = NULL;
expr0[, "is.DEP"]  = NULL;
expr0[, "log2FC"]  = NULL;
expr0[, "FDR"]     = NULL;

print("we have all sample labels:");
print(colnames(expr0));

let patterns = expr0
:> load.expr
# :> average(sampleinfo)
:> relative
:> expression.cmeans_pattern(dim = [3, 3], fuzzification = 5, threshold = 0.001)
;

print("view patterns result:");
print(patterns);

patterns
:> plot.expression_patterns(size = [6000, 4500], colorSet = "Jet")
:> save.graphics(file = "./patterns2.png")
;

patterns
:> cmeans_matrix
:> write.csv(file = "./patterns2.csv")
;