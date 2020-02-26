
# demo QC analysis
require(dataframe);
require(plot.charts);
require(base.math);

let raw = ["\\192.168.1.239\linux\project\HT201702152001苏大附一国风\原始数据\proteinGroups.RSD.csv"]
:> read.dataframe(mode = "numeric")
;

let group_names as string = raw :> dataset.colnames;
let data as double;

print("The given data contains data groups:");
print(group_names);

for(name in group_names) {
    data <- raw :> dataset.vector(name);

}