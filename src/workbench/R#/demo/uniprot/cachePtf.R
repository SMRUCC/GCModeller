imports "uniprot" from "seqtoolkit";

"K:\xanthomonas\uniprot-taxonomy-xanthomonas.xml"
:> open.uniprot
:> cache.ptf(
	file = "K:\xanthomonas\xanthomonas.ptf", 
	cacheTaxonomy = TRUE
)
;