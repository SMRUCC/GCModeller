imports "vcellkit.debugger" from "vcellkit";
imports ["kegg.repository", "report.utils"] from "kegg_kit";

setwd(!script$dir);

let reactions = load.reactions("E:\smartnucl_integrative\biodeepdb_v3\KEGG\br08201");
let run as function(mass, flux) {
	using maps as open.zip("E:\GCModeller\src\workbench\R#\demo\kegg\KEGG_maps.zip") {
		let dynamics = maps[["map01100.XML"]] 
		:> loadMap 
		:> map.flux(reactions) 
		:> flux.dynamics
		:> flux.load_driver(mass, flux)
		:> as.object
		;
		
		dynamics$Run();
	}
}

using mass as auto(new dataset.driver(), "./mass.csv") {
	using flux as auto(new dataset.driver(), "./flux.csv") {
		run(mass, flux);	
	}
}

