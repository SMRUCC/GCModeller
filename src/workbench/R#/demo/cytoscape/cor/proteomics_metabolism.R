imports "cytoscape.kegg" from "cytoscape_toolkit.dll";
imports "kegg.repository" from "kegg_kit.dll";

require(dataframe);
require(igraph);

setwd(!script$dir);

let proteins            = read.csv("geneList.csv");
let compounds as string = readLines("compounds.txt");
# let br08201 as string   = ["D:\biodeep\biodeepdb_v3\KEGG\br08201"];
let br08201 as string = "E:\smartnucl_integrative\biodeepdb_v3\KEGG\br08201";
let exports as string   = "network/";

print("We have kegg compounds list:");
print(compounds);
print(colnames(proteins));
print(`There are ${nrow(proteins)} enzymes in your data.`);

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

br08201
:> reactions.table()
:> compounds.network( compounds , enzymes = enzymeList)
:> save.network(file = exports, properties = ["common_name", "related"])
;
