imports "visualPlot" from "visualkit";
imports ["geneExpression", "sampleInfo"] from "phenotype_kit";

const expr0 = "github://SMRUCC/GCModeller/master/src/workbench/R%23/demo/HTS/all_counts.csv"
|> read.csv(row_names = 1)
;

bitmap(file = `${dirname(@script)}/patterns.png`) {
	const patterns = expr0
	|> load.expr(rm_ZERO = TRUE)
	|> average(sampleinfo = {
		const sampleinfo = colnames(expr0)
		|> guess.sample_groups(
			raw_list = FALSE, 
			maxDepth = TRUE
		)
		;

		print("we have all sample labels:");
		print(colnames(expr0));
		print("a possible sample groups that parsed from the given sample labels:");
		print(sampleinfo);
		
		sampleinfo;
	})
	|> relative
	|> expression.cmeans_pattern(dim = [3, 3], fuzzification = 5, threshold = 0.001)
	;

	print("view patterns result:");
	print(patterns);

	plot(patterns, size = [6000, 4500], colorSet = "Jet");
}
