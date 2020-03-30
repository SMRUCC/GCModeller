imports ["kegg.repository", "report.utils"] from "kegg_kit";

setwd(!script$dir);

let genes as string           = ?"--genes";
let output.dir as string      = ?"--out" || "./output";
let geneValues                = NULL;
let foldchangeField as string = "ratio";
let pvalue as string          = "t.test_p.value";
let KOField as string         = "KEGG_Inforamtion";
let color.types = list(
	up      = "red",
	down    = "blue",
	non_deg = "green"
);

if (file.exists(genes)) {
	geneValues <- read.csv(genes);
	
	let fc     <- log(as.numeric(geneValues[, foldchangeField]), 2);
	let pval   <- as.numeric(geneValues[, pvalue]);
	let KO     <- $"K\d+"(as.character(geneValues[, KOField])) :> sapply(x -> is.empty(x) ? "" : x);
	
	print(KO);

	let genes  <- (abs(fc) >= 1) && (pval <= 0.05) && (KO != ""); 
	
	str(genes);
	
	fc <- fc[genes];
	geneValues <- KO[genes];
	geneValues <- lapply(1:length(geneValues), function(i) {
		if(fc[i] > 0) {
			color.types$up;
		} else {
			color.types$down;
		}
	}, names = i -> geneValues[i]);
	
	str(geneValues);
} else {
	print("no gene value provided...");
}

using kegg_maps as open.zip("kegg_maps.zip") {
	let mapIds as string = as.object(kegg_maps)$ls;
	let map = NULL;
	let allId as string = names(geneValues);
	let innerId as string;
	
	# print(mapIds);
	
	for(mapId in mapIds) {
		map = kegg_maps[[mapId]] :> loadMap :> as.object;
		mapId = basename(mapId);
		innerId = map :> map.intersects(allId);		
		
		print(map$Name);
		
		if (length(innerId) > 0) {
			let profile = geneValues[innerId];
		
			print(`${mapId} contains ${length(innerId)} inside this pathway map!`);
			str(profile);
			
			# draw image
			map 
			:> keggMap.highlights(profile)
			:> save.graphics(file = `${output.dir}/images/${mapId}.png`)
			;
			# draw html report
			map 
			:> keggMap.reportHtml(profile)
			:> writeLines(con = `${output.dir}/${mapId}.html`)
			;
		} else {
			next;
		}
	}
}