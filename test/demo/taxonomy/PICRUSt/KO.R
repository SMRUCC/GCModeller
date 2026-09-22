require(GCModeller);

imports "microbiome" from "metagenomics_kit";

setwd(@dir);

using PICRUSt as file("ko_13_5_precalculated.PICRUSt") {

	OTUtable = read.csv("./20211214/16s_results/mothur_OTU_table.csv", row.names = "taxonomy");
	OTUtable[, "OTU_num"] = NULL;
	
	metagenome = PICRUSt
	|> read.PICRUSt_matrix()
	|> predict_metagenomes(OTUtable)
	|> as.data.frame()
	;
	
	print("create matrix success!");
	print("saving...");
	
	# run enrichment analysis
	write.csv(metagenome, file = "./metagenome.csv", row.names = TRUE);
	
	print("done!");
}