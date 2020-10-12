imports "visualkit.plots" from "visualkit";
imports ["geneExpression", "sampleInfo"] from "phenotype_kit";

setwd(!script$dir);

let expr0 = read.csv("msms_Intensity.csv", row_names = 1);

expr0[, "mz"] = NULL;
expr0[, "rt"] = NULL;

print("we have all sample labels:");
print(colnames(expr0));

let sampleinfo = guess.sample_groups(colnames(expr0), raw_list = FALSE);

print("a possible sample groups that parsed from the given sample labels:");
print(sampleinfo);

let patterns = expr0
:> load.expr
:> average(sampleinfo)
:> relative
:> expression.cmeans_pattern(dim = [3, 4], fuzzification = 3, threshold = 0.1)
;

print("view patterns result:");
print(patterns);

patterns
:> plot.expression_patterns(size = [8000,4500], colorSet = "Jet")
:> save.graphics(file = "./patterns.png")
;

patterns
:> cmeans_matrix
:> write.csv(file = "./patterns.csv")
;