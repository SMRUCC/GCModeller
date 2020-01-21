imports "cytoscape.kegg" from "cytoscape_toolkit.dll";
imports "kegg.repository" from "kegg_kit.dll";

require(igraph);

let br08201 as string = ?"--br08201" || stop("No network connection data provided!");
let id.list as string = ?"--id"      || stop("No node data provided!"); 
let exports as string = ?"--save"    || `${dirname(id.list)}/${basename(id.list)}.network/`;

if (lcase((file.info(id.list))["Extension"]) == ".json") {
	id.list <- read.list(id.list, mode = "character", ofVector = TRUE) :> unlist(typeof = "string");
} else {
	id.list <- readLines(id.list);
}

br08201
:> reactions.table()
:> compounds.network( id.list )
:> save.network(file = exports, properties = ["common_name", "related"])
;

