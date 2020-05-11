imports ["kegg.repository", "report.utils"] from "kegg_kit";

# render script for multiple kegg pathway map
#
# R# ./renderMap_CLI_url.R --xls ./associate.xls --export ./output

let data                = (?"--xls"    || stop("no source data provided!")) :> read.csv(tsv = TRUE);
let outputdir as string =  ?"--export" || "./";
let repo as string      =  ?"--repo"   || "/etc/GCModeller/kegg_maps.zip";
let maps as string      = data[, "KEGG"];
let urls as string      = data[, "Pathway_links"];

str(data);

using mapZip as open.zip(repo) {
	let allMaps as string = as.object(mapZip)$ls;
	let doRender as function(mapId, url) {
		# parse parameters and re-format the 
		# commandline argument inputs:
		mapId <- str_pad(as.integer($"\d+"(mapId)), 5, "left", "0");
		mapId <- `map${mapId}.XML`;
		
		print(mapId);
		
		if (mapId in allMaps) {
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
		} else {
			warning(`map ${mapId} is not exists in repository...`);
		}	
	}

	for(i in 1:length(maps)) {	
		doRender(maps[i], urls[i]);
	}
}

