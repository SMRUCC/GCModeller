imports "vcellkit.analysis" from "vcellkit.dll";
imports "vcellkit.simulator" from "vcellkit.dll";

# Get data source directory and reference name 
# from commandline arguments:
#
#    --data
#    --set
#
let data as string    = ?"--data" || stop("A directory path should provided!");
let setName as string = ?"--set"  || "mass\metabolome.json";
let kegg.compounds    = ?"--kegg_cpd";

if (is.empty(setName)) {
    stop("No dataset reference name provided!");
} else {
	if (is.empty(kegg.compounds)) {
		kegg.compounds <- NULL;
	} else {
		kegg.compounds <- load.list(file = kegg.compounds);
	}
}

# Create result data matrix and 
# then save to the generate file path.
data 
:> union.matrix(setName) 
:> compound.names(names = kegg.compounds)
:> write.csv(file = `${data}/${setName}.csv`);

