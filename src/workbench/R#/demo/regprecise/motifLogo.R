imports "bioseq.patterns" from "seqtoolkit.dll";

setwd(!script$dir);

let i = 1;

for(motif in read.motifs("PWM\GntR.json") :> which(motif -> as.object(motif)$pvalue <= 0.05)) {
	plot.seqLogo(motif, title = "GntR") :> save.graphics(file = `./GntR/GntR_${i}.png`);
	i <- i + 1;
}