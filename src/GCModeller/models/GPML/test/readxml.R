imports "PathVisio" from "cytoscape_toolkit";

let model = "D:\GCModeller\src\GCModeller\models\GPML\data\WP4346_107591.gpml" :> read.gpml;

model :> nodes.table :> print;


pause();