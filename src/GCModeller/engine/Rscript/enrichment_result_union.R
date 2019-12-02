imports "gseakit.background" from "gseakit.dll";

require(dataframe);

let union_data <- [];
let data.dir   <- ?"--data";
let save.csv   <- ?"--save" || `${data.dir}/functions.csv`;
let name as string;
let data;
let geneSet as string <- ?"--geneSet" :> readLines;
let map.id as string;
let ORF as string;
let KO.cluster;
let KO.background <- [?"--KO"] 
:> read.background 
:> as.object 
:> do.call("GetClusterTable")
:> as.list;

print("KO maps names:");
print(names(KO.background));

let readData as function(file, tag) {
	data <- read.dataframe(file, mode = "character");
	dataset.vector(data, "Database", tag);
	dataset.vector(data, "Input", dataset.vector(data, "ORF"));

	for(map in data :> projectAs(as.object)) {
		map.id <- map$ID;
		KO.cluster <- KO.background[[map.id]];

		if (!is.empty(KO.cluster)) {
			ORF <- geneSet.intersects(KO.cluster, geneSet);
			dataset.vector(data, "ORF", ORF);
		}
	}

	union_data <- union_data << data;
}

for(dir in list.dirs(data.dir)) {
	name <- basename(dir);
	
	readData(`${dir}/up.GSEA.csv`, `${name} | up`);
	readData(`${dir}/down.GSEA.csv`, `${name} | down`);
	readData(`${dir}/all_DEM.GSEA.csv`, `${name} | all`);
	
	print(name);
}

union_data :> write.csv(file = save.csv);