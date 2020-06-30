imports "uniprot" from "seqtoolkit";

"K:\uniprot-taxonomy%3A2.xml"
:> open.uniprot
:> cache.ptf(
	file = "K:\bacterials.ptf", 
	cacheTaxonomy = TRUE
)
;