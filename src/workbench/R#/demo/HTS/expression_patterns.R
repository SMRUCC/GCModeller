imports "visualPlot" from "visualkit";
imports ["geneExpression", "sampleInfo"] from "phenotype_kit";

print(expression.cmeans_pattern);

bitmap(file = `${dirname(@script)}/patterns.png`) {
	const patterns = "github://SMRUCC/GCModeller/master/src/workbench/R%23/demo/HTS/counts.csv"
	|> read.csv(row_names = 1)
	|> load.expr(rm_ZERO = TRUE)
	|> average(sampleinfo = sampleInfo(
		ID          = ["s1", "s2", "s3", "s4", "s5", "s6", "s7", "s8", "s9", "s10", "s11", "s12"],
		sample_name = ["s1", "s2", "s3", "s4", "s5", "s6", "s7", "s8", "s9", "s10", "s11", "s12"],
		sample_info = ["s1", "s2", "s3", "s4", "s5", "s6", "s7", "s8", "s9", "s10", "s11", "s12"]
	))
	|> relative
	|> expression.cmeans_pattern(
		dim           = [5, 5], 
		fuzzification = 2, 
		threshold     = 0.005
	)
	;

	print("view patterns result:");
	print(patterns);

	plot(patterns,
		size           = [9000, 6000], 
		colorSet       = "BuPu:c8", 
		axis_label.cex = "font-style: normal; font-size: 14; font-family: Microsoft YaHei;"
	);
}
