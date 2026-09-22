imports "bioseq.fasta" from "seqtoolkit.dll";

setwd(!script$dir);

["LexA.fasta"]
:> read.fasta
:> Assemble.of
:> toString
:> cat(file = "assemble.txt")
;