imports "bioModels.stringdb.ppi" from "cytoscape_toolkit";
imports "uniprot" from "seqtoolkit";
imports "igraph.render" from "R.graph";

setwd(!script$dir);

let uniprot = open.uniprot("D:\human\human\human.xml");
let layout = read.coordinates("F:\20200703\FD-LSC-DS vs FD-LSC-DX\all\ppi\string_network_coordinates.txt");

"F:\20200703\FD-LSC-DS vs FD-LSC-DX\all\ppi\string_interactions.tsv"
:> read.string_interactions
:> as.network(uniprot, layout)
:> render.Plot(canvasSize = [2048,1920])
:> save.graphics(file = "./test.png")
;