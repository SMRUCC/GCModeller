imports "vcellkit.debugger" from "vcellkit";
imports ["kegg.repository", "report.utils"] from "kegg_kit";

setwd(!script$dir);

let KEGG_maps = "E:\GCModeller\src\workbench\R#\demo\kegg\KEGG_maps.zip";
let kegg_reactions = "E:\smartnucl_integrative\biodeepdb_v3\KEGG\br08201";
let reactions = load.reactions(kegg_reactions);
let run as function(mass, flux) {
	using maps as open.zip(KEGG_maps) {
		let dynamics = maps[["map00020.XML"]] 
		:> loadMap 
		:> map.flux(reactions) 
		:> flux.dynamics
		:> flux.load_driver(mass, flux)
		:> as.object
		;
		
		dynamics$Run();
	}
}

using mass as auto(new dataset.driver(), "./matrix/mass.csv") {
	using flux as auto(new dataset.driver(), "./matrix/flux.csv") {
		run(mass, flux);	
	}
}

