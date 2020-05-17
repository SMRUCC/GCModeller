imports "annotation.genomics_context" from "seqtoolkit";

let debug as function(a, b) {
	print(summary(context(a, 200)));
	print(summary(context(b, 200)));

	print(relationship(a, b));
}

debug(location(5000, 8000, "+"), location(4950, 5100, "+"));