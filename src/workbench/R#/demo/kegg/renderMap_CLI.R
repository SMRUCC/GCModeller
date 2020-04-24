imports ["kegg.repository", "report.utils"] from "kegg_kit";

# render script for a single kegg pathway map
#
# demo CLI calls:
# 
# R# ./renderMap_CLI.R --objects R08214,K12234,C00168,C01001,K01007 --map map00680 --colors "red;blue;cyan;rgb(8,153,241);green" --export "./report/kegg/"
#

# objects for highlights on kegg pathway map
let objectList as string = ?"--objects" || stop("no object for highlights on kegg pathway map!");
let mapId as string      = ?"--map"     || stop("no kegg pathway map id provided!");
let colorList as string  = ?"--colors"  || "blue";
let outputdir as string  = ?"--export"  || "./";

# parse parameters and re-format the commandline argument inputs:
mapId     <- str_pad(as.integer($"\d+"(mapId)), 5, "left", "0");
mapId     <- `map${mapId}.XML`;
colorList <- strsplit(colorList, "\s*;\s*");

let highlights = strsplit(objectList, "\s*,\s*");

if ((length(colorList) > 1) && (length(colorList) != length(highlights))) {
	stop("color length is not equals to the size of highlight objects!");
} else {
	highlights <- lapply(1:length(highlights), function(i) {
	   if (length(colorList) == 1) {
		   colorList;
	   } else {
		   colorList[i];
	   }
	}, names = i -> highlights[i]);
}

print("script will render target objects on kegg map:");
print(mapId);
print(`contains ${length(highlights)} objects:`);

str(highlights);

# load for kegg pathway maps
# method1, scan a directory that contains template models
# let maps = "E:\smartnucl_integrative\biodeepdb_v3\KEGG\br08901_pathwayMaps" 
# :> load.maps
# :> lapply(map -> map, names = map -> as.object(map)$id)
# ;

# method2, load from a zip archive file which contains the kegg pathway
# template file

using maps as open.zip("/etc/GCModeller/kegg_maps.zip") {
	# render result as html output
	maps[[mapId]] 
	:> loadMap 
	:> as.object
	:> keggMap.reportHtml(highlights)
	:> writeLines(con = `${outputdir}/${basename(mapId)}.html`)
	;

	# just render image file 
	maps[[mapId]]
	:> loadMap 
	:> as.object
	:> keggMap.highlights(highlights)
	:> save.graphics(file = `${outputdir}/${basename(mapId)}.png`)
	;	
}