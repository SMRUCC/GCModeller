imports "annotation.genomics_context" from "seqtoolkit";
imports "annotation.genomics_context" from "seqtoolkit";

let nt = read.seq("K:\20200226\20200516_gbk\Yersinia pseudotuberculosis (Pfeiffer) Smith and Thal.gbk");

let debug as function(a, b) {
	print(summary(context(a, 200), nt = nt));
	print(summary(context(b, 200), nt = nt));

	print(relationship(a, b));
}

debug(location(5000, 8000, "+"), location(4950, 5100, "+"));