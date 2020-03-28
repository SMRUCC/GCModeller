imports ["kegg.repository", "report.utils"] from "kegg_kit";

let maps = "E:\smartnucl_integrative\biodeepdb_v3\KEGG\br08901_pathwayMaps" 
:> load.maps
:> lapply(map -> map, names = map -> as.object(map)$id)
;

let highlights = list(
	R08214 = "red",
	K12234 = "blue",
	C00168 = "cyan",
	C01001 = "rgb(8,153,241)",
	K01007 = "green"
);

str(highlights);

maps[["map00680"]]
:> keggMap.reportHtml(highlights)
:> writeLines(con = "X:\test.html")
;