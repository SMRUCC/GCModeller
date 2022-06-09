require(GCModeller);

imports ["bioseq.fasta", "bioseq.patterns"] from "seqtoolkit.dll";

setwd(!script$dir);

let i as integer = 1;
let seq as string = ?"--seq" || stop("required a collection of fasta sequence!");
let export as string = ?"-export" || `${dirname(seq)}/${basename(seq)}`;

# ["LexA.fasta"]
seq
:> read.fasta
:> find_motifs(minw = 6, maxw = 10)
:> lapply(function(motif) {
	motif :> json(compress = FALSE) :> writeLines(con = `${export}/${i}.json`); 
	motif :> plot.seqLogo :> save.graphics(file = `${export}/${i}.png`);
	
	i = i + 1;
});