imports "bioseq.fasta" from "seqtoolkit.dll";

"S:\2020\union\1.TRN\TFBS\FUR.fasta"
:> read.fasta
:> as.vector
:> sapply(fa -> as.object(fa)$locus_tag)
:> unique
:> writeLines(con = "S:\2020\union\3.VCell\FUR.txt")
;