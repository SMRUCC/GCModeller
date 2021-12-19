require(GCModeller);

imports "microbiome" from "metagenomics_kit";

using PICRUSt as file("P:\ko_13_5_precalculated.PICRUSt") {

	OTUtable = read.csv("P:\mothur_OTU_table.csv", row.names = "taxonomy");
	OTUtable[, "OTU_num"] = NULL;
	
	metagenome = PICRUSt
	|> read.PICRUSt_matrix()
	|> predict_metagenomes(OTUtable)
	;
	
	# run enrichment analysis
	
}