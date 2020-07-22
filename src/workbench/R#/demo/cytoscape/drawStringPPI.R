imports "bioModels.stringdb.ppi" from "cytoscape_toolkit";
imports "uniprot" from "seqtoolkit";
imports "igraph.render" from "R.graph";

setwd(!script$dir);

let uniprot = open.uniprot("D:\web\20200703\human.xml");
let layout = read.coordinates("D:\web\20200703\HTFEC-DS vs HTFEC-DX\all\ppi\string_network_coordinates.txt");

"D:\web\20200703\HTFEC-DS vs HTFEC-DX\all\ppi\string_interactions.tsv"
:> read.string_interactions
:> as.network(uniprot, layout)
:> render.Plot(canvasSize = [2048,1920])
:> save.graphics(file = "./test.png")
;