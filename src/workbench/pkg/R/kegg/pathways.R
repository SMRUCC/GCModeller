const hsa_pathways = function() {
	"data/hsa.txt" 
	|> system.file(package = "GCModeller")
	|> read.csv(
		tsv = TRUE, 
		row.names = NULL
	);
}