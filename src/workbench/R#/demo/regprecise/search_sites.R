imports ["bioseq.fasta", "bioseq.patterns"] from "seqtoolkit.dll";

setwd(!script$dir);

print("Processing:");

for(family.json in list.files("PWM", pattern = "*.json")) {
	family.json 
	:> read.motifs
	:> motif.find_sites(target = read.fasta("K:\20200226\TRN\genomics\nt.fasta"))
	:> as.fasta
	:> write.fasta(`K:\20200226\TRN\genomics\search\TFBS\${basename(family.json)}.fasta`)
	;
	
	print(basename(family.json));
}

# ["PWM\AraC.json"]
# :> read.motifs
# :> motif.find_sites(target = read.fasta("K:\20200226\TRN\motifs\AraC.fasta")[1])
# :> as.fasta
# :> write.fasta("./sites.fasta")
# ;