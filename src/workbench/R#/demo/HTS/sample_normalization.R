imports ["proteomics.labelfree"] from "proteomics_toolkit.dll";

require(dataframe);

let raw = ["\\192.168.1.239\linux\project\HT201702152001苏大附一国风\原始数据\proteinGroups.csv"] 
:> read.dataframe(mode = "numeric")
:> impute(byRow = TRUE, infer = "Average")
;

let norm = raw :> sample.normalize();
let cor <- lapply(dataset.colnames(raw), function(col) {
    let x = raw :> dataset.vector(col);
    let y = norm :> dataset.vector(col);
    
    pearson(x, y);
});

norm :> write.csv(file = "\\192.168.1.239\linux\project\HT201702152001苏大附一国风\原始数据\proteinGroups_norm.csv");

str(cor);

