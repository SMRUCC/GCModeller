imports "dunnart" from "cytoscape_toolkit";

require(igraph);

"D:\biodeep\biodeepdb_v3\KEGG\202010_flavone"
:> read.network
:> as.graphObj(group_key = "group.category")
:> json
:> writeLines(con = "D:\biodeep\biodeepdb_v3\KEGG\202010_flavone\flavone.json")
;