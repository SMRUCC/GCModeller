file = "E:/GCModeller/src/GCModeller/analysis/PhenoGraph/demo/HR2MSI mouse urinary bladder S096_top3.csv";

setwd(dirname(file));

data = read.csv(file, row.names=1);

str(data);

require(Rphenograph);

Rphenograph_out <- Rphenograph(data, k = 100);
print(modularity(Rphenograph_out[[2]]));
print(membership(Rphenograph_out[[2]]));
data[, "phenograph_cluster"] <- factor(membership(Rphenograph_out[[2]]));

write.csv(data, file = "./output_clusters.csv");
