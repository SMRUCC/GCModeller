# require(kegg_graphquery);

const cache_dir = ?"--cache" || `${dirname(@script)}/.cache/`;

options(http.cache_dir = cache_dir);

const maps = as.data.frame(pathway_category());
const url = "https://www.kegg.jp/entry/ko%s";
const repoDir = enumeratePath(maps, "map");
const id = maps[, "entry"];

print("get all kegg pathway maps:");
str(maps);

for(i in 1:nrow(maps)) {
    kegg_pathway(url = sprintf(url, id[i]))
    |> xml
    |> writeLines(con = `${repoDir(i)}.XML`)
    ;
}