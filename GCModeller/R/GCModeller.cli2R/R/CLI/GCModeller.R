# GCModeller CLI to R binding

help.GCModeller <- function(tool = "GCModeller", man = FALSE) {
	
	if (man) {
		CLI = paste(tool, "man")
	} else {
		CLI = paste(tool, "?")
	}
	
	system(CLI)
}

seqtools.snp <- function(seq.fa = NULL, ...) {

	if (missing(seq.fa)) {
		CLI = paste("seqtools", "?", "/SNP")
	} else {
		CLI = paste("seqtools /SNP /in", seq.fa)
	}
	
	system(CLI)
}