require(GCModeller);

setwd(@dir);

data = read.csv("mummichog_matched_compound_all.csv", row.names = NULL);
kegg = kegg_compounds(TRUE)
|> lapply(function(c) {
	names = as.object(c)$commonNames;
	names[1];
	
}, names = c -> as.object(c)$entry);

print(data, max.print = 13);

str(kegg);

data[, "Query.Mass"] = as.character(data[, "Query.Mass"]);

data = as.list(data, byrow = TRUE) |> groupBy(x -> x$Query.Mass);

# str(data);

data = lapply(data, function(mass) {
	delta = sapply(mass, x -> x$Mass.Diff);
	i = order(delta)[1];
	hit = mass[i];
	hit$name = kegg[[hit$Matched.Compound]];
	
	if (is.null(hit$name) || (hit$name == "")) {
		hit$name = hit$Matched.Compound;
	}
	
	hit;
});

str(data);

data = data.frame(
	mz = sapply(data, x-> x$Query.Mass),
	name = sapply(data, x-> x$name),
	type = sapply(data, x-> x$Matched.Form)

);


print(data);

write.csv(data, file = "./kegg_names.csv", row.names = FALSE);

