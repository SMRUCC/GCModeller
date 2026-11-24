imports "xgmml" from "cytoscape_toolkit.dll";

setwd(!script$dir);

["network\cor.xgmml"]
:> read.xgmml
:> xgmml.graph
:> xgmml.render(size = "10(A0)", convexHull = NULL, edgeBends = FALSE)
:> save.graphics(file = "network.png")
;