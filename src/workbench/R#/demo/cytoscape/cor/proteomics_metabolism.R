imports "cytoscape.kegg" from "cytoscape_toolkit.dll";

require(dataframe);

setwd(!script$dir);

let proteins = read.csv("geneList.csv");
let compounds as string = readLines("compounds.txt");

print(compounds);
print(head(proteins));