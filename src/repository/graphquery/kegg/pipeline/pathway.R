# require(kegg_graphquery);

const maps = as.data.frame(pathway_category());

print("get all kegg pathway maps:");
print(maps);

sapply(1:nrow(maps), enumeratePath(maps, "map"))
|> str
;