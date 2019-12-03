require(dataframe);

let enrichment <- [?"--enrichment"] 
:> read.dataframe(mode = "character") 
:> projectAs(as.object) 
:> which(map -> as.numeric(map$"P-Value") <= 0.05)
:> groupBy(map -> map$ID);

if (length(enrichment) == 0) {
	return;
} else {
	print(`We get ${length(enrichment)} enriched pathway maps!`);
	print(enrichment);
}