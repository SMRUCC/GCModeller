imports "annotation.workflow" from "seqtoolkit.dll";

using writer as open.stream(file = "K:\20200226\X101SC19112292-Z01-J001-B1-16_TR_result\X101SC19112292-Z01-J001-B1-16_results\0.SuppFiles\RNA_genes.csv", type = "Mapping") {
	let maps <- ["K:\20200226\X101SC19112292-Z01-J001-B1-16_TR_result\X101SC19112292-Z01-J001-B1-16_results\0.SuppFiles\RNA_genes.txt"]
	:> read.blast(type = "nucl")
	:> blastn.maphit
	;
	
	writer :> stream.flush(data = maps);
}