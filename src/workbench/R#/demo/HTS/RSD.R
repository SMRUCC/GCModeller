imports ["proteomics.labelfree"] from "proteomics_toolkit.dll";

# demo QC analysis
require(dataframe);

let raw = ["\\192.168.1.239\linux\project\HT201702152001苏大附一国风\原始数据\proteinGroups.csv"] 
:> read.dataframe(mode = "numeric")
;
let sample.names = names(raw[1]) :> groups;

print("View of the sample group information:");
str(sample.names);

for