imports "annotation.workflow" from "seqtoolkit.dll";

let saveTable as string = "K:\20200226\TRN\1025_mapping.csv"; # "K:\20200226\X101SC19112292-Z01-J001-B1-16_TR_result\X101SC19112292-Z01-J001-B1-16_results\0.SuppFiles\RNA_genes.csv";
let blastnOut as string = "K:\20200226\TRN\1025_mapping.txt"; # "K:\20200226\X101SC19112292-Z01-J001-B1-16_TR_result\X101SC19112292-Z01-J001-B1-16_results\0.SuppFiles\RNA_genes.txt";

using writer as open.stream(file = saveTable, type = "Mapping") {
	let maps <- blastnOut
	:> read.blast(type = "nucl")
	:> blastn.maphit
	;
	
	writer :> stream.flush(data = maps);
}

