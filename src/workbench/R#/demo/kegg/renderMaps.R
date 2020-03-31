imports ["kegg.repository", "report.utils", "kegg.brite"] from "kegg_kit";

setwd(!script$dir);

let genes as string           = ?"--genes";
let proteins as string        = ?"--proteins";
let output.dir as string      = ?"--out" || "./output";
let profileValues = list();
let color.types   = list(
	up      = "red",
	down    = "blue",
	non_deg = "green"
);

let deg = [];
let dep = [];
let dem = [];
let KOnames = KO.geneNames();

print(str(KOnames));

let addProfile as function(table, foldchangeField, pvalue, KOField, t.log2, log2fc.cutoff) {

}

if (file.exists(genes)) {
	let geneValues <- read.csv(genes);
	
	let foldchangeField as string = "log2FoldChange";
	let pvalue as string          = "pval";
	let KOField as string         = "KeggOrtholog";
	
	let fc    <- as.numeric(geneValues[, foldchangeField]);
	let pval  <- as.numeric(geneValues[, pvalue]);
	let KO    <- $"K\d+"(as.character(geneValues[, KOField])) :> sapply(x -> is.empty(x) ? "" : x[1]);
	let genes <- (abs(fc) >= 4) && (pval <= 0.05) && (KO != ""); 
	
	# print(KO);
	# str(genes);
	
	fc         <- fc[genes];
	geneValues <- KO[genes];
	geneValues <- lapply(1:length(geneValues), function(i) {
		if(fc[i] > 0) {
			color.types$up;
		} else {
			color.types$down;
		}
	}, names = i -> geneValues[i]);
	
	str(geneValues);
	
	deg = names(geneValues);
	
	for(id in names(geneValues)) {
		profileValues[[id]] <- geneValues[[id]];
	}
} else {
	print("no gene value provided...");
}

if (file.exists(proteins)) {
	let protValues <- read.csv(proteins);

	let foldchangeField as string = "ratio";
	let pvalue as string          = "t.test_p.value";
	let KOField as string         = "KEGG_Inforamtion";
	
	let fc    <- log(as.numeric(protValues[, foldchangeField]), 2);
	let pval  <- as.numeric(protValues[, pvalue]);
	let KO    <- $"K\d+"(as.character(protValues[, KOField])) :> sapply(x -> is.empty(x) ? "" : x[1]);
	let genes <- (abs(fc) >= 1) && (pval <= 0.05) && (KO != ""); 
	
	fc         <- fc[genes];
	protValues <- KO[genes];
	protValues <- lapply(1:length(protValues), function(i) {
		if(fc[i] > 0) {
			color.types$up;
		} else {
			color.types$down;
		}
	}, names = i -> protValues[i]);
	
	str(protValues);
	
	dep = names(protValues);
	
	for(id in names(protValues)) {
		profileValues[[id]] <- protValues[[id]];
	}
} else {
	print("no proteins value provided...");
}

using kegg_maps as open.zip("kegg_maps.zip") {
	let mapIds as string = as.object(kegg_maps)$ls;
	let map = NULL;
	let allId as string = names(profileValues);
	let innerId as string;

	let mapId.vec = [];
	let mapName.vec = [];
	let deg.vec = [];
	let dep.vec = [];
	let dem.vec = [];
	let mapUrl  = [];
	
	# print(mapIds);
	
	for(mapId in mapIds) {
		map = kegg_maps[[mapId]] :> loadMap :> as.object;
		mapId = basename(mapId);
		innerId = map :> map.intersects(allId);		
		
		if (length(innerId) > 0) {
			let profile = profileValues[innerId];
		
			print(`${mapId} contains ${length(innerId)} inside this pathway map!`);
			print(map$Name);
			
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
			
			# create output table
			mapId.vec   <- mapId.vec   << mapId;
			mapName.vec <- mapName.vec << map$Name;
			deg.vec     <- deg.vec     << paste(intersect(innerId, deg), ",");
			dep.vec     <- dep.vec     << paste(intersect(innerId, dep), ",");
			dem.vec     <- dem.vec     << paste(intersect(innerId, dem), ",");
			mapUrl      <- mapUrl      << keggMap.url(mapId, profile);
			
		} else {
			next;
		}
	}
	
	data.frame(
		map  = mapId.vec, 
		name = mapName.vec, 
		deg  = deg.vec, 
		dep  = dep.vec, 
		dem  = dem.vec, 
		url  = mapUrl
	)
	:> write.csv(file = `${output.dir}/result.csv`, row.names = FALSE);
}