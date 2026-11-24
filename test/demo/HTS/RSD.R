imports ["proteomics.labelfree"] from "proteomics_toolkit.dll";

# demo QC analysis
require(dataframe);

let raw = ["\\192.168.1.239\linux\project\HT201702152001苏大附一国风\原始数据\proteinGroups.csv"] 
:> read.dataframe(mode = "numeric")
:> impute(byRow = TRUE, infer = "Average")
;

let sample.names = raw
:> dataset.colnames
:> guess.sample_groups
;

print("View of the sample group information:");
str(sample.names);

let samples.RSD = lapply(sample.names, function(group) {
	let RSD.vector = raw 
	  :> dataset.project(group) 
	  :> lapply(function(protein) {
	       protein :> cells :> RSD;
	  }, names = protein -> as.object(protein)$ID);
	
	RSD.vector;
});

# str(samples.RSD);

data.frame(samples.RSD)
:> write.csv(file = "\\192.168.1.239\linux\project\HT201702152001苏大附一国风\原始数据\proteinGroups.RSD.csv")
;

raw <- raw :> sample.normalize();

samples.RSD = lapply(sample.names, function(group) {
	let RSD.vector = raw 
	  :> dataset.project(group) 
	  :> lapply(function(protein) {
	       protein :> cells :> RSD;
	  }, names = protein -> as.object(protein)$ID);
	
	RSD.vector;
});

# str(samples.RSD);

data.frame(samples.RSD)
:> write.csv(file = "\\192.168.1.239\linux\project\HT201702152001苏大附一国风\原始数据\proteinGroups.RSD_norm.csv")
;