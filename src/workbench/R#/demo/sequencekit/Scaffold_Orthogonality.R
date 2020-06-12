imports ["bioseq.fasta", "bioseq.patterns"] from "seqtoolkit";

setwd(!script$dir);

"Scaffold_Orthogonality.fasta"
:> read.fasta
:> scaffold.orthogonality(rev_compl = TRUE)
:> json(compress = FALSE)
:> print
;