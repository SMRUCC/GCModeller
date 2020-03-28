imports ["kegg.repository", "report.utils"] from "kegg_kit";

let maps = load.maps.index("E:\smartnucl_integrative\biodeepdb_v3\KEGG\br08901_pathwayMaps");
let highlights = list(
	K02964 = "red",
	K02952 = "blue"
);

str(highlights);

maps[["map03010"]]
:> keggMap.reportHtml(highlights)
:> writeLines(con = "X:\test.html")
;