imports "kegg.metabolism" from "kegg_kit.dll";

# A demo script for save compound origins
# data for LC-MS small molecule compound
# identification.

let repo as string     = ?"--repo" || stop("You must provides the kegg organism pathway maps data repository for this script!");
let save.rda as string = ?"--save" || `${repo}/${basename(repo)}.rda`;

print("The compound origin dataset will be saved at location:");
print(save.rda);

repo
:> compound.origins
:> as.object 
:> do.call("Save", rda = save.rda);