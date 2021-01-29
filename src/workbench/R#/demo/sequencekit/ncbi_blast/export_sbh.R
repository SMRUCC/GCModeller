imports "annotation.workflow" from "seqtoolkit";

let exportHits as function(outputText) {
	const output_table as string = `${dirname(outputText)}/${basename(outputText)}.sbhits.csv`;

	using table as open.stream(output_table, ioRead = FALSE) {
		read.blast(outputText, type="prot")
		:> blasthit.sbh(keepsRawName = FALSE)
		:> stream.flush(stream = table)
		;
	}
}

exportHits("K:\20210127\参考基因组_Yersinia pseudotuberculosis (Pfeiffer) Smith and Thal\prot_vs_kegg.txt");
exportHits("K:\20210127\参考基因组_Yersinia pseudotuberculosis (Pfeiffer) Smith and Thal\kegg_vs_prot.txt");