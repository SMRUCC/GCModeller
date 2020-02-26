
# demo QC analysis
require(dataframe);
require(plot.charts);
require(base.math);

let raw = ["\\192.168.1.239\linux\project\HT201702152001苏大附一国风\原始数据\proteinGroups.RSD.csv"]
:> read.dataframe(mode = "numeric")
;

let group_names as string = raw :> dataset.colnames;
let steps as double = 0.05;
let save.png as string = NULL;

print("The given data contains data groups:");
print(group_names);

for(name in group_names) {
    save.png <- sprintf("\\\\192.168.1.239\linux\project\HT201702152001苏大附一国风\原始数据\raw\\%s_RSD.png", name);    
    raw 
    :> dataset.vector(name) 
    :> replace(find = NaN, as = 1.0)
    :> hist(step = steps)
    :> plot(
        steps = steps, 
        title = `RSD of ${name}`, 
        x.lab = "RSD", 
        y.lab = "Number of proteins", 
        padding = [100, 100, 150, 200]
     )
    :> save.graphics(file = save.png);
    ;

    print(save.png);
}