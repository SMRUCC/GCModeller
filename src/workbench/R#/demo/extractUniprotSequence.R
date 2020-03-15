imports ["uniprot", "bioseq.fasta"] from "seqtoolkit";

"K:\20200226\搜库结果\uniprot-yourlist_M202003138BC4D7ADE02784B0C2481C7F3DE0963A066279N.xml"
:> open.uniprot
:> protein.seqs(extractAll = TRUE)
:> write.fasta(
	file = "K:\20200226\搜库结果\uniprot_proteins.fasta", 
	lineBreak = 60
)	
;