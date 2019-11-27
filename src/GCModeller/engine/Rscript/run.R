# demo script for 
# Run a virtual cell model in R# 

# first load GCModeller virtualcell toolkit R# package into R# environment
imports "vcellkit.simulator" from "vcellkit.dll";

# config input model and result save directory from commandline arguments
let model                <- read.vcell(path = ?"--in");
let output.dir as string <- ?"--out";

# create virtual cell object model and initialize the test data
# from the virtual cell data model.
let inits <- vcell.mass.kegg(vcell = model, mass = 500000);
let vcell <- model :> vcell.model;
let mass  <- vcell :> vcell.mass.index;
let flux  <- vcell :> vcell.flux.index;

# Run virtual cell simulation
let run as function(i) {
    # The VB.NET object should be convert to R# object then 
    # we can reference its member function 
    # directly in script.
    let engine = [vcell = vcell] 
        :> engine.load(inits, iterations = 10000, time_resolutions = 1) 
        # apply as.object function for the initialzie pipeline code
        # to construct a R# object
        :> as.object;

    engine$Run();
    engine :> vcell.snapshot(mass, flux, save = `${output.dir}/replicate=${i}/`);
}

# run 5 biological replicate for the virtual cell simulation
for(i in 1:5) {
    i :> run;
}
