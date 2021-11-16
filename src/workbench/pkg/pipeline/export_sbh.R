imports "annotation.workflow" from "GCModeller::seqtoolkit";

[@info "the blastp output in text file format."]
[@type "text"]
const rawinput as string = ?"--blast_output" || stop("no blast output raw data file provided!");

#' Export SBH hits of blastp output
#' 
const exportHits as function(outputText) {
	const output_table as string = `${dirname(outputText)}/${basename(outputText)}.sbhits.csv`;

	using table as open.stream(output_table, ioRead = FALSE) {
		read.blast(outputText, type="prot")
		:> blasthit.sbh(keepsRawName = FALSE)
		:> stream.flush(stream = table)
		;
	}
}

exportHits(outputText = rawinput);
