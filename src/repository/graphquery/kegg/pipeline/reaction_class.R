require(kegg_graphquery);

options(http.cache_dir = ?"--cache" || `${dirname(@script)}/.cache/`);

const all_category = as.data.frame(reactionclass_category());

str(all_category);

const repoDir = enumeratePath(all_category);
const id      = all_category[, "entry"];
const url     = "https://www.kegg.jp/dbget-bin/www_bget?rc:%s";

for(i in 1:nrow(all_category)) {
    const class    = kegg_reactionclass(url = sprintf(url, id[i]));
    const category = repoDir(i);

    print("reaction class item:");
    print(category);

    if ((class != "") && (!is.null(class))) {
        class
        |> xml
        |> writeLines(con = `${category}.XML`)
        ;
    } else {
        print(`invalid query result of '${category}'`);
    }
}