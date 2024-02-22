#' read human kegg pathway list data
#' 
#' @details read from the internal resource data files
#' 
#' @return a dataframe object that contains the human pathway list, 
#'   the data fields was includes this dataframe object:
#' 
#'   + ID: the pathway id
#'   + Definition: the pathway name
#' 
const hsa_pathways = function() {
	"data/hsa.txt" 
	|> system.file(package = "GCModeller")
	|> read.csv(
		tsv = TRUE, 
		row.names = NULL
	);
}