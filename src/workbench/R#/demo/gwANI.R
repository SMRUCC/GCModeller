imports "bioseq.blast" from "seqtoolkit.dll";
imports "bioseq.fasta" from "seqtoolkit.dll";

let input as string = ?"--fasta";
let save as string = ?"--save" || `${dirname(input)}/${basename(input)}.gwANI.csv`;

[?"--fasta"]
:> read.fasta
:> align.gwANI
:> write.csv(file = save)
;

[?"--fasta"]
:> read.fasta
:> MSA.of
:> print
;