imports "annotation.workflow" from "GCModeller::seqtoolkit";

const forward = (?"--forward" || stop("forward alignment result table must be provided!"))
:> open.stream(ioRead = TRUE)
:> besthit.filter(delNohits = TRUE, evalue = 1)
;

const reverse = (?"--reverse" || stop("reverse alignment result table must be provided!"))
:> open.stream(ioRead = TRUE)
:> besthit.filter(delNohits = TRUE, evalue = 1)
;

const output_table as string = ?"--output" || `${dirname(?"--forward")}/bbh_annotation.csv`;

using table as open.stream(output_table, ioRead = FALSE, type = "BBH") {
	blasthit.bbh(forward, reverse, algorithm = "HybridBHR")
	:> stream.flush(stream = table)
	;
}