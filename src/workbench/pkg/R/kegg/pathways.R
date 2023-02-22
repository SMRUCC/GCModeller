#' read human kegg pathway list data
#' 
#' @details read from the internal resource data files
#' 
const hsa_pathways = function() {
	"data/hsa.txt" 
	|> system.file(package = "GCModeller")
	|> read.csv(
		tsv = TRUE, 
		row.names = NULL
	);
}