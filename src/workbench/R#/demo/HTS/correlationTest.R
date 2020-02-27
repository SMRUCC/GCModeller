imports ["proteomics.labelfree"] from "proteomics_toolkit.dll";

require(dataframe);

let raw = "\\192.168.1.239\linux\project\HT201702152001苏大附一国风\原始数据\proteinGroups_impute_raw.csv" :> read.dataframe(mode = "numeric");
let norm = "\\192.168.1.239\linux\project\HT201702152001苏大附一国风\原始数据\proteinGroups_norm.csv" :> read.dataframe(mode = "numeric");

print(which(sapply(raw, r => as.object(r)$ID) != sapply(norm, r => as.object(r)$ID)));