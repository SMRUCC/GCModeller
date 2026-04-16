require(GCModeller);

imports "visualPlot" from "visualkit";
imports ["geneExpression", "sampleInfo"] from "phenotype_kit";

# print(expression.cmeans_pattern);

bitmap(file = `${dirname(@script)}/patterns.png`) {
	const patterns = `${@dir}/counts.csv`
	|> read.csv(row_names = 1)
	|> load.expr(rm_ZERO = TRUE)
	|> average(sampleinfo = sampleInfo(
		ID          = ["s1", "s2", "s3", "s4", "s5", "s6", "s7", "s8", "s9", "s10", "s11", "s12"],
		sample_name = ["s1", "s2", "s3", "s4", "s5", "s6", "s7", "s8", "s9", "s10", "s11", "s12"],
		sample_info = ["s1", "s2", "s3", "s4", "s5", "s6", "s7", "s8", "s9", "s10", "s11", "s12"]
	))
	|> relative
	|> z_score
	|> expression.cmeans_pattern(
		dim           = [4, 4], 
		fuzzification = 1.5, 
		threshold     = 0.5
	)
	;

	print("view patterns result:");
	print(patterns);

	plot(patterns,
		size           = [9000, 5000], 
		colorSet       = "Jet", 
		axis_label.cex = "font-style: normal; font-size: 14; font-family: Microsoft YaHei;"
	);
}
