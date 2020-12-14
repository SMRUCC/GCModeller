imports "PathVisio" from "cytoscape_toolkit";

let model = "D:\GCModeller\src\GCModeller\models\GPML\data\WP4346_107591.gpml" :> read.gpml;
let nodes = model :> nodes.table;


nodes :> print;
nodes :> write.csv(file = `${!script$dir}/test.csv`);

let lipidmaps = nodes[ nodes[, "database"] == "LIPID MAPS", ];

print(lipidmaps);

pause();