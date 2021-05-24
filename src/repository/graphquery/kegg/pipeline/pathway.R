# require(kegg_graphquery);

const maps = as.data.frame(pathway_category());

print("get all kegg pathway maps:");
print(maps);

maps
|> enumeratePath("map")
|> str
;