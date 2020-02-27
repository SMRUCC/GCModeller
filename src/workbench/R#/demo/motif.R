imports ["bioseq.fasta", "bioseq.patterns", "bioseq.sequenceLogo"] from "seqtoolkit.dll";

setwd(!script$dir);

let i as integer = 1;

["LexA.fasta"]
:> read.fasta
:> find_motifs(minw = 6, maxw = 10)
:> lapply(function(motif) {
	motif :> json(compress = FALSE) :> writeLines(con = `./LexA/${i}.json`); 
	motif :> plot.seqLogo :> save.graphics(file = `./LexA/${i}.png`);
	
	i = i + 1;
});