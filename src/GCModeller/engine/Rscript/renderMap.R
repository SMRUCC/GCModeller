imports "kegg.repository" from "kegg_kit.dll";

require(dataframe);

let save.dir as string <- ?"--save";
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
let save.png as string;

print("Loading kegg pathway maps for the map render...");

let kegg_map.render = [?"--maps"] 
:> load.maps.index 
:> map.local_render 
:> as.object;

print("[JOD DONE]");

for(map in enrichment) {
	print(map$key);
	
	for(a in map) {
		input <- a$Input :> strsplit( "; " );
		compounds <- compounds << input;
	}
	
	compounds <- unique(compounds);
	
	print(`${length(compounds)} unique compounds in map '${map$key}'.`);
	print(compounds);
	
	save.png <- `${save.dir}/${map$key}.png`;
	compounds <- nodes.colorAs(compounds, "red");
	kegg_map.render$Rendering(map$key, compounds) :> save.graphics(save.png);
}