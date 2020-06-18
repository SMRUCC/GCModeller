imports ["models", "automation"] from "cytoscape_toolkit";

# cytoscape automation demo

setwd(!script$dir);

const cytoscape.servicesHost = "192.168.1.247";

let network = read.csv("./network-edges.csv");

print(head(network));

let u = network[, "fromNode"];
let v = network[, "toNode"];
let interact = network[, "interaction_type"];

network = sif(u, interact, v) :> cyjs;

# print(toString(network));

network = put_network(network, collection = "automation", title = "pathway_enrich", host = cytoscape.servicesHost);

# print(network);


layout(network, "circular", host = cytoscape.servicesHost);

print(layouts(host = cytoscape.servicesHost));

session.save(file = normalizePath("./result.cys"), host = cytoscape.servicesHost);

# close current session
finalize(host = cytoscape.servicesHost);

