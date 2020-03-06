imports ["bioseq.fasta", "bioseq.patterns"] from "seqtoolkit.dll";

setwd(!script$dir);

["PWM\AraC.json"]
:> read.motifs
:> motif.find_sites(target = read.seq("K:\20200226\TRN\motifs\AraC.fasta"))
:> as.fasta
:> write.fasta("./sites.fasta")
;