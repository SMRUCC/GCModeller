imports "models" from "cytoscape_toolkit";

require(igraph);
require(igraph.render);

setwd(!script$dir);

let session = "result.cys"
:> open.cys
;

str(session :> list.networks);

session 
:> get.sessionInfo 
:> print
;

session
:> get.network_graph("automation", "pathway_enrich")
:> render.Plot(canvasSize = [1600,1200], driver = "SVG")
:> save.graphics(file = "./render.svg")
;

session
:> get.network_graph("automation", "pathway_enrich")
:> save.network(file = "./cytoscape_result/")
;