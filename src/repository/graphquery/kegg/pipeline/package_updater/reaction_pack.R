require(GCModeller);
require(HDS);

let file = HDS::openStream(file.path(@dir, "reaction_cache.db"), readonly = TRUE);
let files = HDS::files(file, excludes.dir = TRUE);
let file_df = as.data.frame(files);
let i = file.ext(file_df$path) == "xml";

file_df = file_df[i, ];

print(file_df);

let maps = sapply(file_df$path, filepath -> HDS::getText(file, filepath) |> loadXml(typeof = "kegg_reaction"));

write.msgpack(maps, file = file.path(@dir, "KEGG_reactions.msgpack"));