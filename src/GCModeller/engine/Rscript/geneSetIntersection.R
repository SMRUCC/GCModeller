imports "vcellkit.simulator" from "vcellkit.dll";
imports "gseakit.background" from "gseakit.dll";

let [deletions, background] as string = [?"--deletions", ?"--background"];
let save.dir as string = ?"--save";
let geneSet as string;
let pathwayName as string;

background <- read.background(background) :> as.object;  
deletions  <- readLines(deletions);

for(cluster in background$clusters) {
	geneSet <- cluster :> geneSet.intersects(deletions);
	pathwayName <- (cluster :> as.object)$names 
		:> normalize.filename
		:> string.replace(" - Reference pathway", "")
		:> string.replace("\s+", "_", true)
		;

	print(`${pathwayName} with ${length(geneSet)} geneSet.`);
		
	if (length(geneSet) == 0) {
		next;
	} else {
		writeLines( geneSet,  `${save.dir}/${pathwayName}.geneSet_deletions.txt` );
	}
}