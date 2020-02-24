imports ["proteomics.labelfree"] from "proteomics_toolkit.dll";

require(dataframe);

["\\192.168.1.239\linux\project\HT201702152001苏大附一国风\原始数据\proteinGroups.csv"] 
:> read.dataframe(mode = "numeric")
:> impute(byRow = TRUE, infer = "Min")
:> sample.normalize.correlation
:> write.csv(file = "\\192.168.1.239\linux\project\HT201702152001苏大附一国风\原始数据\proteinGroups.cor.csv")
;