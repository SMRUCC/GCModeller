require(GCModeller);
require(igraph);
require(charts);

imports ["geneExpression", "phenograph"] from "phenotype_kit";

setwd(@dir);

"../../../demo/HR2MSI mouse urinary bladder S096_top3.csv"
|> load.expr
|> phenograph(k = 200)
|> save.network(file = "HR2MSI mouse urinary bladder S096_graph")
;

bitmap(file = `Rphenograph.png`) {
	const data     = read.csv("HR2MSI mouse urinary bladder S096_graph/nodes.csv", row.names = NULL);
	const ID       = lapply(data[, 1], function(px) as.numeric(strsplit(px, ",")));
	const clusters = data[, "NodeType"];
	const x        = sapply(ID, px -> px[1]);
    const y        = sapply(ID, px -> px[2]);
	
	plot(x, y, 
	    class      = clusters, 
	    shape      = "Square", 
	    colorSet   = "Set1:c8", 
	    grid.fill  = "white", 
	    point_size = 25, 
	    reverse    = TRUE
    );
}