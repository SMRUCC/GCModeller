require(GCModeller);

imports ["bioseq.fasta", "bioseq.patterns"] from "seqtoolkit.dll";

setwd(!script$dir);

# let i as integer = 1;
let seq as string = ?"--seq" || stop("required a collection of fasta sequence!");
let export as string = ?"-export" || `${dirname(seq)}/${basename(seq)}`;

# ["LexA.fasta"]
seq
|> read.fasta
|> find_motifs(minw = 12, maxw = 16)
|> lapply(function(motif) {
	motif 
	|> json(compress = FALSE) 
	|> writeLines(con = `${export}/${motifString(motif)}.json`)
	; 
		
	bitmap(file = `${export}/${motifString(motif)}.png`) {
		plot(motif);
	};
})
;