# demo script for 
# Run a virtual cell model in R# 

# first load GCModeller virtualcell toolkit R# package into R# environment
imports "vcellkit.simulator" from "vcellkit.dll";

# config input model and result save directory from commandline arguments
let model                <- read.vcell(path = ?"--in");
let output.dir as string <- ?"--out";
let deletions  as string <- ?"--deletions";
let tag.name   as string <- ?"--tag";

print("Run virtual cell model:");
print(model);

# create virtual cell object model and initialize the test data
# from the virtual cell data model.
let inits <- vcell.mass.kegg(vcell = model, mass = 500000);
let vcell <- model :> vcell.model;
let mass  <- vcell :> vcell.mass.index;
let flux  <- vcell :> vcell.flux.index;

deletions <- file.exists(deletions) ? readLines(deletions) : NULL;
tag.name  <- is.empty(tag.name) ? "replicate=" : tag.name;

if (is.empty(deletions)) {
    print("No gene deletions for current VirtualCell simulation analysis.");
} else {
    print(`Apply ${length(deletions)} deletions of genes for run simulation analysis!`);
}

print(`The biological replication of the analysis will be tagged as '${replicate}'`);

# Run virtual cell simulation
let run as function(i) {
    # The VB.NET object should be convert to R# object then 
    # we can reference its member function 
    # directly in script.
    let engine = [vcell = vcell] 
        :> engine.load(
            inits            = inits, 
            iterations       = 10000, 
            time_resolutions = 1, 
            deletions        = deletions
        ) 
        # apply as.object function for the initialzie pipeline code
        # to construct a R# object
        :> as.object;

    # run virtual cell simulation and then 
    # save the result snapshot data files into 
    # target data directory
    engine$Run();
    engine :> vcell.snapshot(mass, flux, save = `${output.dir}/${tag.name}${i}/`);
}

# run 5 biological replicate for the 
# current virtual cell simulation analysis
for(i in 1:5) {
    i :> run;
}
