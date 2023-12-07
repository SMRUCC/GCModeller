require(GCModeller);
require(HDS);

let file = HDS::openStream("E:\kegg - Copy.db", readonly = TRUE);
let files = HDS::files(file, excludes.dir = TRUE);
let file_df = as.data.frame(files);

print(file_df);

let dirs = basename(dirname(file_df$path));

print(dirs);

let i = dirs == "compounds";

file_df = file_df[i, ];

print(file_df);

let unique_compounds = file_df$path;
unique_compounds = groupBy(unique_compounds, f -> basename(f));

str(unique_compounds);

unique_compounds = sapply(unique_compounds, g -> first(g));

print(unique_compounds);
print(basename(unique_compounds));


let load_compounds = lapply(unique_compounds, path -> HDS::getText(file, path) |> loadXml(typeof = "kegg_compound"));

# str(load_compounds);
setwd(@dir);

repository::write.msgpack(unlist(load_compounds), file = "./kegg.msgpack");