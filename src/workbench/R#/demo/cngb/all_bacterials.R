imports "kegg.repository" from "kegg_kit";
imports "ftp" from "R.web";

setwd(!script$dir);

let prokaryote = fetch.kegg_organism(NULL, type = "prokaryote");
let cngb = new ftp(server = "ftp.cngb.org");
let save_gz = "K:\bacterials";

print(cngb);

write.csv(prokaryote, file = "./bacterials.csv", row_names = FALSE);

let downloadGbff as function(genome) {
	let assembly as string = strsplit(gsub(genome$RefSeq, "ftp://ftp.ncbi.nlm.nih.gov/genomes/all/", ""), "/");
	let assemblyName = `${assembly[1]}_${paste(assembly[2:4], "")}`;
	let ftpURL = `/pub/Assembly/${paste(assembly[1:4], "/")}`;
	
	print(`[${assemblyName}] => ${ftpURL}`);
	
	let dirs = cngb :> list.ftp_dirs(ftpURL);
	
	print(dirs);
	
	for(name in dirs) {
		let subdir = `${ftpURL}/${name}`;
		let files = cngb :> list.ftp_dirs(subdir);
		let gbff = files :> which(name => name == $".+genomic.gbff.gz");
		
		# print(files);
		print(gbff);
		
		if (length(gbff) > 0) {
			let gzfile = `${subdir}/${gbff[1]}`;
			let saveas = `${save_gz}/${gbff[1]}`;
			
			cngb :> ftp.get(gzfile, saveas);
		}
	}
}

downloadGbff(as.object(prokaryote[1]));

# for(genome in prokaryote :> projectAs(as.object)) {
	
	
	# break;
# }