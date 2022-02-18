require(GCModeller);

setwd(@dir);

data = read.csv("mummichog_matched_compound_all.csv", row.names = NULL);
kegg = kegg_compounds();

print(data, max.print = 13);

print(kegg);

data[, "Query.Mass"] = as.character(data[, "Query.Mass"]);

data = as.list(data, byrow = TRUE) |> groupBy(x -> x$Query.Mass);

# str(data);

data = lapply(data, function(mass) {
	delta = sapply(mass, x -> x$Mass.Diff);
	i = order(delta)[1];
	hit = mass[i];
	
	hit;
});

str(data)