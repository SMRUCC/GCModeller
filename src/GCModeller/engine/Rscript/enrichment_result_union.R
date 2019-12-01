require(dataframe);

let union_data <- [];
let data.dir <- ?"--data";
let save.csv <- ?"--save" || `${data.dir}/functions.csv`;
let name as string;
let data;

let readData as function(file, tag) {
	data <- read.dataframe(file, mode = "character");
	dataset.vector(data, "Database", tag);
	union_data <- union_data << data;
}

for(dir in list.dirs(data.dir)) {
	name <- basename(dir);
	
	readData(`${dir}/up.GSEA.csv`, `${name}_up`);
	readData(`${dir}/down.GSEA.csv`, `${name}_down`);
	readData(`${dir}/all_DEM.GSEA.csv`, `${name}_all`);
	
	print(name);
}

union_data :> write.csv(file = save.csv);