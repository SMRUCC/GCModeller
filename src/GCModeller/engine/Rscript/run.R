# demo script for 
# Run a virtual cell model in R# 

# first load GCModeller virtualcell toolkit R# package into R# environment
imports "vcellkit.simulator" from "vcellkit.dll";
imports "gseakit.background" from "gseakit.dll";

# config input model and result save directory from commandline arguments
let model                <- read.vcell(path = ?"--in") :> as.object;
let output.dir as string <- ?"--out";
let deletions  as string <- ?"--deletions";
let tag.name   as string <- ?"--tag";
let background as string <- ?"--background";

print("Run virtual cell model:");
print(model);

# create virtual cell object model and initialize the test data
# from the virtual cell data model.
let inits <- vcell.mass.kegg(vcell = model, mass = 500000);
let vcell <- model :> vcell.model;
let mass  <- vcell :> vcell.mass.index;
let flux  <- vcell :> vcell.flux.index;

let dynamics = dynamics.default() :> as.object;

dynamics$transcriptionBaseline = 200;
dynamics$transcriptionCapacity = 500;

print("Using dynamics parameter configuration:");
print(dynamics);

print("Experiment tags as:");
# print(typeof tag.name);
print(tag.name);

print("Gene list file for apply the deletion operation:");
print(deletions);

deletions <- file.exists(deletions) ? readLines(deletions) : NULL;
tag.name  <- is.empty(tag.name) ? "replicate=" : tag.name;

if (is.empty(deletions)) {
    print("No gene deletions for current VirtualCell simulation analysis.");
} else {
    print(`Apply ${length(deletions)} deletions of genes for run simulation analysis!`);
}

print(`The biological replication of the analysis will be tagged as '${tag.name}'`);

# Run virtual cell simulation
let run as function(i, deletions = NULL, exp.tag = tag.name) {
    # The VB.NET object should be convert to R# object then 
    # we can reference its member function 
    # directly in script.
    let engine = [vcell = vcell] 
        :> engine.load(
            inits            = inits, 
            iterations       = 1000, 
            time_resolutions = 0.1, 
            deletions        = deletions
        ) 
        # apply as.object function for the initialzie pipeline code
        # to construct a R# object
        :> as.object;

    # run virtual cell simulation and then 
    # save the result snapshot data files into 
    # target data directory
    engine$Run();
    engine :> vcell.snapshot(mass, flux, save = `${output.dir}/${exp.tag}${i}/`);
}

let biological.replicates as integer = 6;

if (background :> file.exists) {
    let geneSet as string;
    let pathwayName as string;

    background <- read.background(background) :> as.object;  

    print("gene deletion mutation by pathway clusters:");
    print("pathway clusters' GSEA background:");
    print(background);

    for(cluster in background$clusters) {
        geneSet <- cluster :> geneSet.intersects(deletions);
        pathwayName <- (cluster :> as.object)$names 
            :> normalize.filename
            :> string.replace("\\s+", "_");

        if (length(geneSet) == 0) {
            next;
        } else {
            print(`do pathway cluster deletion mutation for: ${pathwayName}!`);
            print(`intersect ${length(geneSet)} with the given geneSet.`);

            for(i in 1:biological.replicates) {
                # run for mutation genome model
                i :> run(deletions = geneSet, exp.tag = pathwayName);
            }
        }
    }

} else {
    # run 6 biological replicate for the 
    # current virtual cell simulation analysis
    for(i in 1:biological.replicates) {
        # run for wildtype
        i :> run;
    }
}






