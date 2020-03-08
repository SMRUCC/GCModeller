imports ["annotation.genbank_kit", "bioseq.fasta"] from "seqtoolkit.dll";

"K:\20200226\TRN\Yersinia pseudotuberculosis IP32953 genome_complete sequence.gb"
:> read.genbank
:> getRNA.fasta
:> write.fasta(file = "K:\20200226\TRN\RNA.fasta")