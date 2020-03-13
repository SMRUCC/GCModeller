# demo script for 
# Run a virtual cell model in R# 

# first load GCModeller virtualcell toolkit R# package into R# environment
imports ["vcellkit.simulator", "vcellkit.rawXML"] from "vcellkit.dll";
imports "gseakit.background" from "gseakit.dll";

# config input model and result save directory from commandline arguments
let model.file as string  <- ?"--in"  || stop("No virtual cell model provided!");
let model                 <- read.vcell(path = model.file) :> as.object;
let output.dir as string  <- ?"--out" || `${dirname(model.file)}/result/`;

let time.ticks as integer <- ?"--ticks" || 100;

# config experiment analysis from command line arguments
let [deletions, tag.name, background] as string = [?"--deletions", ?"--tag", ?"--background"];

# get profile list files
let transcripts as string = ?"--transcripts";
let proteins    as string = ?"--proteins";
let compounds   as string = ?"--compounds";

if (file.exists(transcripts)) {
	transcripts <- read.list(transcripts, mode = "numeric"); 
} else {
	transcripts <- NULL;
}
if (file.exists(proteins)) {
	proteins <- read.list(proteins, mode = "numeric");
} else {
	proteins <- NULL;
}
if (file.exists(compounds)) {
	compounds <- read.list(compounds, mode = "numeric");
} else {
	compounds <- NULL;
}

print("Run virtual cell model:");
print(model);

# create virtual cell object model and initialize the test data
# from the virtual cell data model.
let inits <- vcell.mass.kegg(vcell = model, mass = 50000);
let vcell <- model :> vcell.model;
let [mass, flux] = vcell :> [vcell.mass.index, vcell.flux.index];

let dynamics = dynamics.default() :> as.object;

dynamics$transcriptionBaseline   = 200;
dynamics$transcriptionCapacity   = 500;
dynamics$productInhibitionFactor = 0.00000125;

print("Using dynamics parameter configuration:");
print(dynamics);

print("Experiment tags as:");
# print(typeof tag.name);
print(tag.name);

print("Gene list file for apply the deletion operation:");
print(deletions);

deletions <- file.exists(deletions) ? readLines(deletions) : NULL;
tag.name  <- is.empty(tag.name) ? "control_" : tag.name;

if (is.empty(deletions)) {
    print("No gene deletions for current VirtualCell simulation analysis.");
} else {
    print(`Apply ${length(deletions)} deletions of genes for run simulation analysis!`);
}

print(`The biological replication of the analysis will be tagged as '${tag.name}'`);

let sample.names as string = [];

# Run virtual cell simulation
let run as function(i, deletions = NULL, exp.tag = tag.name) {
	# vector used for generate sampleInfo file
    let sampleName as string = `${exp.tag}${i}`;
	
    # The VB.NET object should be convert to R# object then 
    # we can reference its member function 
    # directly in script.    
    let engine = [vcell = vcell] 
        :> engine.load(
            inits            = inits, 
            iterations       = time.ticks, 
            time_resolutions = 0.5, 
            deletions        = deletions,
			showProgress     = !script$debug
        ) 
		# apply profiles data
		:> apply.module_profile(profile = transcripts, system = "Transcriptome")
		:> apply.module_profile(profile = proteins,    system = "Proteome")
		:> apply.module_profile(profile = compounds,   system = "Metabolome")
        # apply as.object function for the initialzie pipeline code
        # to construct a R# object
        :> as.object;

    # sample.names = sample.names << sampleName;

	using xml as open.vcellXml(file  = `${output.dir}/raw/${sampleName}.vcXML`, 
							   mode  = "write", 
							   vcell = engine) {
		
		let folder as string = `${output.dir}/${sampleName}/`;
	
	    # run virtual cell simulation and then 
		# save the result snapshot data files into 
		# target data directory
		engine$AttachBiologicalStorage(xml);
		engine$Run();
		engine :> vcell.snapshot(mass, flux, save = folder);
	}
	
	sampleName;
}

let save.sampleName as function(fileName) {
    print("sample names of current sample group:");
    print(sample.names);

    sample.names 
	:> as.character 
	:> orderBy(name -> name)
	:> writeLines(`${output.dir}/${fileName}.txt`)
	;
}

let biological.replicates as integer = 6;

if ((background :> file.exists) && (!is.empty(deletions))) {
    let geneSet as string;
    let pathwayName as string;

    background <- read.background(background) :> as.object;  

    print("gene deletion mutation by pathway clusters:");
    print("pathway clusters' GSEA background:");
    print(background);

    console::progressbar.pin.top();

    for(cluster in background$clusters) {
        geneSet <- cluster :> geneSet.intersects(deletions);
        pathwayName <- (cluster :> as.object)$names 
            :> normalize.filename
            :> string.replace(" - Reference pathway", "")
            :> string.replace("\s+", "_", true)
            ;

        if (length(geneSet) == 0) {
            next;
        } else {
            print(`do pathway cluster deletion mutation for: ${pathwayName}!`);
            print(`intersect ${length(geneSet)} with the given geneSet.`);

            sample.names <- for(i in 1:biological.replicates) %dopar% {
								# run for mutation genome model
								i :> run(deletions = geneSet, exp.tag = pathwayName);
							}

            [fileName = pathwayName] :> save.sampleName;
        }
    }

} else {
    # run 6 biological replicate for the 
    # current virtual cell simulation analysis
	sample.names <- if (!script$debug) {
						 for(i in 1:biological.replicates) {
							 # run for wildtype
							 i :> run;
						 }	
					} else {
						 for(i in 1:biological.replicates) %dopar% {
							 # run for wildtype
							 i :> run;
						 }	
					}

    [fileName = tag.name] :> save.sampleName;
}