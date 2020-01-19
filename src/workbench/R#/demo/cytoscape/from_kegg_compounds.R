imports "cytoscape.kegg" from "cytoscape_toolkit.dll";
imports "kegg.repository" from "kegg_kit.dll";

require(igraph);

let br08201 as string = ?"--br08201" || stop("No network connection data provided!");
let id.list as string = ?"--id"      || stop("No node data provided!"); 
let exports as string = ?"--save"    || `${dirname(id.list)}/${basename(id.list)}.network/`;

br08201
:> reactions.table()
:> compounds.network( readLines(id.list) )
:> save.network(file = exports, properties = ["name"])
;

