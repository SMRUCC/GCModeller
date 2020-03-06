imports ["bioseq.fasta"] from "seqtoolkit.dll";

setwd(!script$dir);

let fas = read.fasta("./LexA.fasta");
let indexView as integer = [1,2,3];

print(fas);
print(fas[indexView]);