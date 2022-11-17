require(HDS);

data = HDS::openStream("F:\hsa.db");

print(HDS::tree(data));
