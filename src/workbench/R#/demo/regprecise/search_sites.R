imports ["bioseq.fasta", "bioseq.patterns"] from "seqtoolkit.dll";

setwd(!script$dir);

print("Processing:");

for(family.json in list.files("PWM", pattern = "*.json")) {
	let sites = [];
	
	for(motif in family.json :> read.motifs :> which(m -> as.numeric(as.object(m)$pvalue) <= 0.05)) {
		print(as.object(motif)$patternString());
		sites <- sites << motif.find_sites(
			motif  = motif, 
			target = read.fasta("K:\20200226\TRN\genomics\nt.fasta"), 
			cutoff = 0.85,
			identities = 0.8
		);
	}
	
	sites
	:> as.fasta
	:> write.fasta(file = `K:\20200226\TRN\genomics\search\TFBS/${basename(family.json)}.fasta`)
	;
	
	print(basename(family.json));
}

# ["PWM\AraC.json"]
# :> read.motifs
# :> motif.find_sites(target = read.fasta("K:\20200226\TRN\motifs\AraC.fasta")[1])
# :> as.fasta
# :> write.fasta("./sites.fasta")
# ;