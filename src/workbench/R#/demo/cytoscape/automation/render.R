imports "models" from "cytoscape_toolkit";

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
:> render.Plot()
:> save.graphics(canvasSize = [1600,1200], file = "./render.svg", driver = "SVG")
;