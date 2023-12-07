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


let load_compounds = lapply(file_df$path |> take(10), path -> HDS::getText(file, path) |> loadXml(typeof = "kegg_compound"));

str(load_compounds);