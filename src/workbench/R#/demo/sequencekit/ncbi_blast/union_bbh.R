imports "annotation.workflow" from "seqtoolkit";

let forwards = ["K:\20210127\参考基因组_Yersinia pseudotuberculosis (Pfeiffer) Smith and Thal\prot_vs_kegg.sbhits.csv"]
:> open.stream(ioRead = TRUE)
:> besthit.filter(delNohits = TRUE)
;

let reverse = ["K:\20210127\参考基因组_Yersinia pseudotuberculosis (Pfeiffer) Smith and Thal\kegg_vs_prot.sbhits.csv"]
:> open.stream(ioRead = TRUE)
:> besthit.filter(delNohits = TRUE)
;

const output_table as string = "K:\20210127\参考基因组_Yersinia pseudotuberculosis (Pfeiffer) Smith and Thal\regulators.csv";

using table as open.stream(output_table, ioRead = FALSE, type = "BBH") {
	blasthit.bbh(forwards, reverse, algorithm = "Naive")
	:> stream.flush(stream = table)
	;
}

