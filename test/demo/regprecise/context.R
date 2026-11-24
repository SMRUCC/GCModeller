imports "annotation.genomics_context" from "seqtoolkit";
imports "bioseq.fasta" from "seqtoolkit";

let nt = read.seq("K:\20200226\20200516_gbk\Yersinia pseudotuberculosis (Pfeiffer) Smith and Thal.gbk");

let debug as function(a, b) {
	print(summary(context(a, 100), nt = nt));
	print(summary(context(b, 100), nt = nt));

	print(relationship(a, b));
}

debug(location(5000, 5200, "+"), location(4950, 5100, "+"));