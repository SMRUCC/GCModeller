imports "kegg.repository" from "kegg_kit";
imports "ftp" from "R.web";

setwd(!script$dir);

let prokaryote = fetch.kegg_organism(NULL, type = "prokaryote");
let cngb as new ftp(server = "ftp.cngb.org");

write.csv(prokaryote, file = "./bacterials.csv", row_names = FALSE);

for(genome in prokaryote :> projectAs(as.object)) {
	let assembly as string = strsplit(gsub(genome$RefSeq, "ftp://ftp.ncbi.nlm.nih.gov/genomes/all/", ""), "/");
	let assemblyName = `${assembly[1]}_${paste(assembly[2:4], "")}`;
	let ftpURL = `ftp://ftp.cngb.org/pub/Assembly/${paste(assembly, "/")}/`;
}