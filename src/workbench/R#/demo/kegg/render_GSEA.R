imports ["kegg.repository", "report.utils"] from "kegg_kit";

let enrichment as string = ?"--enrich" || stop("no GCModeller enrichment result was provided!");
let KEGG_maps as string  = ?"--maps"   || stop("no kegg reference map template data was provided!");
let outputdir as string  = ?"--output" || `${dirname(enrichment)}/${basename(enrichment)}.KEGG_map_render/`;

enrichment = read.csv(enrichment) :> as.list;

str(enrichment);

using maps as open.zip(KEGG_maps) {

	for(i in 1:length(enrichment$term)) {
		let mapId = enrichment$term[i];
		let pvalue = as.numeric(enrichment$FDR[i]);
		
		if (pvalue <= 0.05) {
			let KO_terms = strsplit(enrichment$geneIDs[i], "; ") :> lapply(a -> "blue", names = a -> a);
			let map = maps[[`${mapId}.XML`]] :> loadMap :> as.object;
			
			map
			:> keggMap.reportHtml(KO_terms)
			:> writeLines(con = `${outputdir}/${mapId}.html`)
			;
			
			map			
			:> keggMap.highlights(KO_terms)
			:> save.graphics(file = `${outputdir}/images/${mapId}.png`)
			;	
		}
	}	
}


