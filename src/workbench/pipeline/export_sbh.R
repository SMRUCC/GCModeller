require(GCModeller);

imports "annotation.workflow" from "seqtoolkit";

[@info "the blastp output in text file format."]
[@type "text"]
let rawinput as string = ?"--blast_output" || stop("no blast output raw data file provided!");
let output_table as string = file.path(dirname(rawinput), `${basename(rawinput)}-hits.csv`);

#' Export SBH hits of blastp output
#' 
let exportHits = function(outputText) {
	using table as open.stream(output_table, ioRead = FALSE, type = "SBH") {
		read.blast(outputText, type="prot")
		|> blasthit.sbh(keepsRawName = TRUE)
		|> stream.flush(stream = table)
		;
	}
}

exportHits(outputText = rawinput);
