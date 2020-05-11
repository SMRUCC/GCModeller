imports ["kegg.repository", "report.utils"] from "kegg_kit";

# render script for a single kegg pathway map
#
# demo CLI calls:
# 
# R# ./renderMap_CLI.R --objects R08214,K12234,C00168,C01001,K01007 --map map00680 --colors "red;blue;cyan;rgb(8,153,241);green" --export "./report/kegg/"
#

let data as string = ?"--xls" || stop("no source data provided!");

data = read.csv(data, tsv = TRUE);

let maps = data[, "KEGG"];
let urls = data[, "Pathway_links"];

# objects for highlights on kegg pathway map
# let url as string        = ?"--url"    || stop("no object for highlights on kegg pathway map!");
# let mapId as string      = ?"--map"    || stop("no kegg pathway map id provided!");
let outputdir as string  = ?"--export" || "./";

using mapZip as open.zip("/etc/GCModeller/kegg_maps.zip") {

	for(i in 1:length(maps)) {

		# parse parameters and re-format the commandline argument inputs:
		let mapId = maps[i];
		let url = urls[i];
		
		mapId     <- str_pad(as.integer($"\d+"(mapId)), 5, "left", "0");
		mapId     <- `map${mapId}.XML`;

		
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
}

