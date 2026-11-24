imports "visualkit.plots" from "visualkit";

let raw = read.csv(?"pathway_results");

print(head(raw));

let KO = unlist( $"\d+"(as.character(raw[, "pathway"])));
let p = -log( as.numeric(raw[, "Raw p"]), 10);

# str(KO);

# print(p);

let profiles = lapply(1:length(KO), i -> p[i], names = i-> KO[i]);

str(profiles);

profiles 
:> kegg.category_profiles.plot(
	title = "KEGG pathway enrichment",
	 axisTitle = "-log10(Raw p)",
	 size= [2000,1800]
)
:> save.graphics(file = "./barplot.png")
;