require(kegg_graphquery);

options(http.cache_dir = ?"--cache" || `${dirname(@script)}/.cache/`);

# create resource url based on the organism Tcode exists or not
const url     = "https://www.kegg.jp/dbget-bin/www_bget?cpd:%s";
const runQuery as function(name, brite) {
    const class   = as.data.frame(pathway_category());
    const repoDir = enumeratePath(maps, Tcode);
    const id      = maps[, "entry"];

    print("get all kegg pathway maps:");
    str(maps);

    for(i in 1:nrow(maps)) {
        const keg_compound = kegg_compound(url = sprintf(url, id[i]));

        if ((keg_compound != "") && (!is.null(keg_compound))) {

            keg_compound
            |> xml
            |> writeLines(con = `${name}/${repoDir(i)}.XML`)
            ;
        }
    }
}


