imports "TRN.builder" from "phenotype_kit";
imports ["annotation.genbank_kit", "annotation.workflow"] from "seqtoolkit";

let regions = "K:\20200226\20200516_gbk\X101SC19112292-Z01-J001_result\res.txt"
:> readText
:> as.promoter.models
:> lapply(a => a, names = as.object(a)$locus_tag)
;

str(regions);