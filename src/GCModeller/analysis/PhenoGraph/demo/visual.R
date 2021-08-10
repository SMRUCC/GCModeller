require(charts);

setwd(dirname(@script));

const data = read.csv("./HR2MSI mouse urinary bladder S096_graph/nodes.csv", row.names = NULL);
const ID = lapply(data[, 1], function(px) as.numeric(strsplit(px, ",")));
const clusters as string = data[, "NodeType"];

bitmap(file = `Rphenograph.png`) {
	const x = sapply(ID, px -> px[1]);
    const y = sapply(ID, px -> px[2]);
	
	plot(x,y, class = clusters, shape = "Square", 
	colorSet = "Set1:c8", grid.fill = "white", point_size = 25, reverse = TRUE);
}