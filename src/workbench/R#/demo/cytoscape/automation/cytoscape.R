imports ["models", "automation"] from "cytoscape_toolkit";

# cytoscape automation demo

setwd(!script$dir);

let network = read.csv("./network-edges.csv");

print(head(network));

let u = network[, "fromNode"];
let v = network[, "toNode"];
let interact = network[, "interaction_type"];

network = sif(u, interact, v) :> cyjs;

print(toString(network));

network = put_network(network);

print(network);


print(layouts());

finalize();