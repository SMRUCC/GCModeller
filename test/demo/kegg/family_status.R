imports "visualkit.plots" from "visualkit.dll";
imports "bioseq.fasta" from "seqtoolkit.dll";
imports "kegg.profiles" from "kegg_kit.dll";

let KOmaps = read.csv("S:\2020\union\2.KEGG\KO.csv");
let geneId = KOmaps[, "query_name"] :> as.character;
let KOterms = KOmaps[, "term"] :> as.character;
let work as string = "S:\2020\union\1.TRN\TFBS";

KOmaps <- lapply(1:length(geneId), i -> KOterms[i], names = i -> geneId[i]);

for(file in list.files(work, pattern = "*.fasta")) {
	let geneIds = read.fasta(file) :> as.vector :> sapply(fa -> as.object(fa)$locus_tag) :> unique;
	let savePng as string = `${dirname(work)}/${basename(file)}.png`;
	
	KOterms = KOmaps[geneIds] 
	:> unlist 
	:> which(str -> !is.empty(str)) 
	:> as.character
	:> KO.map.profiles
	# do plots of the KO profiles
	:> kegg.category_profiles.plot 
	:> save.graphics(file = savePng)
	;
}