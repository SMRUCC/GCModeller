imports "cytoscape.kegg" from "cytoscape_toolkit.dll";

require(dataframe);

setwd(!script$dir);

let proteins = read.csv("geneList.csv");
let compounds as string = readLines("compounds.txt");

print("We have kegg compounds list:");
print(compounds);
# print(head(proteins));

let enzymeList = proteins[, "KO"] :> sapply(id -> strsplit(id, ";")[1]) :> as.character;
let geneId = proteins[, "ID"];

enzymeList <- lapply(1:length(enzymeList), function(i) {
	list(KO = enzymeList[i], geneId = geneId[i]);
})
:> groupBy(i => i$KO) 
:> lapply(function(group) {
	as.character(sapply(group, enzyme -> enzyme$geneId));
}, names = gene -> gene$key);

print("And the enzymes that in gene expression:");
str(enzymeList);