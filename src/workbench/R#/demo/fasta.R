imports ["bioseq.fasta"] from "seqtoolkit.dll";

setwd(!script$dir);

let fas = read.fasta("./LexA.fasta");

print(fas);