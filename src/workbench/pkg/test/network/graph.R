require(GCModeller);

const data = read.csv(file = `${@dir}/cid.csv`, row.names = NULL);

print(data);

const g = GCModeller::CompoundNetwork(data);

print(g);

save.network(g, file = @dir);