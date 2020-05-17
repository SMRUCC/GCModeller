imports "TRN.builder" from "phenotype_kit";
imports ["annotation.genbank_kit", "annotation.workflow"] from "seqtoolkit";

let regions = "K:\20200226\20200516_gbk\X101SC19112292-Z01-J001_result\res.txt"
:> readText
:> as.promoter.models
:> lapply(a => a, names = a => as.object(a)$locus_tag)
;

# str(regions);

let mapping = "K:\20200226\TRN\1025_mapping.csv"
:> open.stream(type = "Mapping", ioRead = TRUE)
:> projectAs(as.object)
:> groupBy(a -> a$Reads.Query)
:> lapply(function(a) {
	if (length(a) == 1) {
		a[1]$Reference;
	} else {
		let ref = sapply(a, x -> x$Reference);
		let scores = sapply(a, x -> x$"Score(bits)");
		
		ref[which.max(scores)];
	}
	
}, names = g -> g$key);

str(mapping);