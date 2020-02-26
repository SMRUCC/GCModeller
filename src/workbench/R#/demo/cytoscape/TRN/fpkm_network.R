imports "network.TRN" from "cytoscape_toolkit.dll";

require(dataframe);

let raw.data as string = ?"--fpkm" || stop("No expression data provided!");

raw.data
:> read.dataframe(mode = "numeric")
:> fpkm.connections(cutoff = 0.65)
:> write.csv(file = `${dirname(raw.data)}/${basename(raw.data)}_connections.csv`)
;

