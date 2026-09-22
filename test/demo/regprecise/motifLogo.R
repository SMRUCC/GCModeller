imports "bioseq.patterns" from "seqtoolkit.dll";

let dir as string = ?"--work" || !script$dir;

setwd(dir);

for(familyJSON in list.files("./", pattern = "*.json")) {
	let i = 1;
	let title as string = basename(familyJSON);

	for(motif in read.motifs(familyJSON) :> which(motif -> as.object(motif)$pvalue <= 1)) {
		plot.seqLogo(motif, title = title) :> save.graphics(file = `./${title}/${title}_${i}.png`);
		i <- i + 1;
	}
}