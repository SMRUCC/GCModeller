require(HDS);

data = HDS::openStream("D:\hsa.db");
fs_tree = HDS::tree(data);

print(fs_tree);


writeLines(fs_tree, con = `${@dir}/kegg_pathways.txt`);