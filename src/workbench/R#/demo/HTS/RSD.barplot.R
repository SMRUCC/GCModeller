
# demo QC analysis
require(dataframe);
require(plot.charts);
require(base.math);

let raw = ["\\192.168.1.239\linux\project\HT201702152001苏大附一国风\原始数据\proteinGroups.RSD.csv"]
:> read.dataframe(mode = "numeric")
;

let group_names as string = raw :> dataset.colnames;
let steps as double = 0.1;
let save.png as string = NULL;

print("The given data contains data groups:");
print(group_names);

for(name in group_names) {
    save.png <- [`\\192.168.1.239\linux\project\HT201702152001苏大附一国风\原始数据\raw\${name}_RSD.png`];    
    raw 
    :> dataset.vector(name) 
    :> hist(step = steps)
    :> plot(steps = steps, title = `RSD of ${name}`)
    :> save.graphics(file = save.png);
    ;
}