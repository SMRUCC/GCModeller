imports "vcellkit.analysis" from "vcellkit.dll";

# Get data source directory and reference name 
# from commandline arguments:
#
#    --data
#    --set
#
let data as string    = ?"--data";
let setName as string = ?"--set";

if (is.empty(setName)) {
    stop("No dataset reference name provided!");
}

# Create result data matrix and 
# then save to the generate file path.
data 
  :> union.matrix(setName) 
  :> write.csv(file = `${data}/${setName}.csv`);

