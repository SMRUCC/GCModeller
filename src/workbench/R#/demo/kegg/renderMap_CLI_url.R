imports ["kegg.repository", "report.utils"] from "kegg_kit";

# render script for multiple kegg pathway map
#
# R# ./renderMap_CLI_url.R --xls ./associate.xls --export ./output

let data as string      = (?"--xls"    || stop("no source data provided!")) :> read.csv(tsv = TRUE);
let outputdir as string =  ?"--export" || "./";
let maps as string      = data[, "KEGG"];
let urls as string      = data[, "Pathway_links"];

using mapZip as open.zip("/etc/GCModeller/kegg_maps.zip") {
	let doRender as function(mapId, url) {
		# parse parameters and re-format the 
		# commandline argument inputs:
		mapId <- str_pad(as.integer($"\d+"(mapId)), 5, "left", "0");
		mapId <- `map${mapId}.XML`;
		
		# render result as html output
		mapZip[[mapId]] 
		:> loadMap 
		:> as.object
		:> keggMap.reportHtml(url)
		:> writeLines(con = `${outputdir}/${basename(mapId)}.html`)
		;

		# just render image file 
		mapZip[[mapId]]
		:> loadMap 
		:> as.object
		:> keggMap.highlights(url)
		:> save.graphics(file = `${outputdir}/${basename(mapId)}.png`)
		;	
	}

	for(i in 1:length(maps)) {	
		doRender(maps[i], urls[i]);
	}
}

