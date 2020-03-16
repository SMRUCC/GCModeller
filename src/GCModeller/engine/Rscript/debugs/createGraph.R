imports ["vcellkit.analysis", "vcellkit.simulator"] from "vcellkit";

require(igraph);

let model as string = ?"--model" || stop("no model file provided!");
let output as string = ?"--save" || `${dirname(model)}/${basename(model)}.graph/`;
let vcell = model :> read.vcell;

vcell 
:> vcell.mass.kegg
:> engine.load(vcell.model(vcell))
:> vcell.mass.graph
:> save.network(file = output)
;