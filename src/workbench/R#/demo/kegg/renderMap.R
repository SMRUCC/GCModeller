imports ["kegg.repository", "report.utils"] from "kegg_kit";

# objects for highlights on kegg pathway map
let outputdir as string = "./";
let highlights = list(
	R08214 = "red",
	K12234 = "blue",
	C00168 = "cyan",
	C01001 = "rgb(8,153,241)",
	K01007 = "green"
);

str(highlights);

# load for kegg pathway maps
# method1, scan a directory that contains template models
# let maps = "E:\smartnucl_integrative\biodeepdb_v3\KEGG\br08901_pathwayMaps" 
# :> load.maps
# :> lapply(map -> map, names = map -> as.object(map)$id)
# ;

# method2, load from a zip archive file which contains the kegg pathway
# template file

using maps as open.zip("./kegg_maps.zip") {
	# render result as html output
	maps[["map00680"]]
	:> keggMap.reportHtml(highlights)
	:> writeLines(con = `${outputdir}\test.html`)
	;

	# just render image file 
	maps[["map00680"]]
	:> keggMap.highlights(highlights)
	:> save.graphics(file = `${outputdir}\test.png`)
	;	
}