imports "models" from "cytoscape_toolkit";

require(igraph.render);

setwd(!script$dir);

"result.cys"
:> open.cys
:> get.network_graph
:> render.Plot()
:> save.graphics(canvasSize = [1600,1200], file = "./render.svg", driver = "SVG")
;