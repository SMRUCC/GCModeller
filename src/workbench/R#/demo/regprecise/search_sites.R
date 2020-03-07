imports ["bioseq.fasta", "bioseq.patterns"] from "seqtoolkit.dll";

setwd(!script$dir);

["PWM\AraC.json"]
:> read.motifs
:> motif.find_sites(target = read.fasta("K:\20200226\TRN\motifs\AraC.fasta")[1])
:> as.fasta
:> write.fasta("./sites.fasta")
;