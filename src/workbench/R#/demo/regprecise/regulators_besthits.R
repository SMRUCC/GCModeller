imports "annotation.workflow" from "seqtoolkit.dll";

using writer as open.stream("K:\20200226\TRN\genomics\search\regulators.besthit.csv", "SBH") {
	let data <- ["K:\20200226\TRN\genomics\search\regulators.txt"]
	:> read.blast(type = "prot")
	:> blasthit.sbh(
		idetities    = 0.3,
        coverage     = 0.5,
        topBest      = FALSE,
        keepsRawName = TRUE
	)
	;
	
	writer :> stream.flush(data);
}