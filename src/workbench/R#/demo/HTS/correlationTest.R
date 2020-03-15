imports ["proteomics.labelfree"] from "proteomics_toolkit.dll";

require(dataframe);

let raw = "\\192.168.1.239\linux\project\HT201702152001苏大附一国风\原始数据\proteinGroups_impute_raw.csv" :> read.dataframe(mode = "numeric");
let norm = "\\192.168.1.239\linux\project\HT201702152001苏大附一国风\原始数据\proteinGroups_norm.csv" :> read.dataframe(mode = "numeric");

print(which(sapply(raw, r => as.object(r)$ID) != sapply(norm, r => as.object(r)$ID)));

let sample.names = raw
:> dataset.colnames
:> guess.sample_groups
;

str(sample.names);

let A = sample.names[["iBAQ A"]];
let B = sample.names[["iBAQ B"]];

let raw.A = raw :> dataset.project(A) :> sapply(protein -> protein :> cells :> mean);
let raw.B = raw :> dataset.project(B) :> sapply(protein -> protein :> cells :> mean);

str(pearson(raw.A, raw.B));

let norm.A = norm :> dataset.project(A) :> sapply(protein -> protein :> cells :> mean);
let norm.B = norm :> dataset.project(B) :> sapply(protein -> protein :> cells :> mean);

str(pearson(norm.A, norm.B));