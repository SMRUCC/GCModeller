imports "bioseq.fasta" from "seqtoolkit.dll";

list.files("K:\20200226\TRN\genomics\search\TFBS", pattern = "*.fasta")
:> lapply(function(path) {
	read.fasta(path) 
	:> as.vector 
	:> sapply(fa -> as.object(fa)$locus_tag) 
	:> unique
	;
}, names = path -> basename(path))
:> save.list(file = "K:\20200226\TRN\genomics\search\TFBS.json")
;