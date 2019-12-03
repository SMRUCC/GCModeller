require(dataframe);

let enrichment <- [?"--enrichment"] 
:> read.dataframe(mode = "character") 
:> projectAs(as.object) 
:> which(map -> as.numeric(map$"P-Value") <= 0.05)
:> groupBy(map -> map$ID);

if (length(enrichment) == 0) {
	return;
} else {
	print(`We get ${length(enrichment)} unique enriched pathway maps!`);
	cat("\n");
}

let compounds as string = [];
let input as string;

for(map in enrichment) {
	print(map$key);
	
	for(a in map) {
		input <- a$Input :> strsplit( "; " );
		compounds <- compounds << input;
	}
	
	compounds <- unique(compounds);
	
	print(`${length(compounds)} unique compounds in map '${map$key}'.`);
	print(compounds);
}