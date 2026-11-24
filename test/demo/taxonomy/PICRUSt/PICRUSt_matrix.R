require(GCModeller);

imports "microbiome" from "metagenomics_kit";

using buffer as file("D:\biodeep\bionovogene_health\metacolon\ko_13_5_precalculated.PICRUSt") {
		
	file("F:\16s\greengenes\taxonomy\gg_13_8_99.gg.tax")
	|> parse.otu_taxonomy()
	|> save.PICRUSt_matrix(
		ko_13_5_precalculated = file("D:\biodeep\bionovogene_health\metacolon\ko_13_5_precalculated.tab"),
		save = buffer
	);
	
}