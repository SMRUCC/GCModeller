imports ["proteomics.labelfree"] from "proteomics_toolkit.dll";

require(dataframe);

["E:\plot\HT201702152001苏大附一国风.csv"] 
:> read.dataframe(mode = "numeric")
:> sample.normalize.correlation
:> write.csv(file = "E:\plot\HT201702152001苏大附一国风.cor.csv")
;