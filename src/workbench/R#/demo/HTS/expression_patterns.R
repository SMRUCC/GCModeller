imports "visualPlot" from "visualkit";
imports ["geneExpression", "sampleInfo"] from "phenotype_kit";

const expr0 = read.csv("http://www.biodeep.cn/demo/metabolome.csv", row_names = 1);

expr0[, "mz"] = NULL;
expr0[, "rt"] = NULL;

print("we have all sample labels:");
print(colnames(expr0));

const sampleinfo = guess.sample_groups(colnames(expr0), raw_list = FALSE);

print("a possible sample groups that parsed from the given sample labels:");
print(sampleinfo);

bitmap(file = `${dirname(@script)}/patterns.png`) {
	const patterns = expr0
	|> load.expr
	|> average(sampleinfo)
	|> relative
	|> expression.cmeans_pattern(dim = [3, 3], fuzzification = 5, threshold = 0.001)
	;

	print("view patterns result:");
	print(patterns);

	patterns
	:> cmeans_matrix
	:> write.csv(file = "./patterns.csv")
	;

	patterns
	:> plot.expression_patterns(size = [6000, 4500], colorSet = "Jet")
	;
}
