require(GCModeller);

imports "debugger" from "vcellkit";
imports ["repository", "report.utils"] from "kegg_kit";

setwd(!script$dir);

const KEGG_maps = GCModeller::kegg_maps(rawMaps = FALSE);
const reactions = GCModeller::kegg_reactions(raw = FALSE);
const run as function(mass, flux) {	
	const dynamics = KEGG_maps[["map00020"]] 
	:> map.flux(reactions) 
	:> flux.dynamics
	:> flux.load_driver(mass, flux)
	:> as.object
	;
	
	dynamics$Run();
}

using mass as auto(new dataset.driver(), "./matrix/mass.csv") {
	using flux as auto(new dataset.driver(), "./matrix/flux.csv") {
		run(mass, flux);	
	}
}

