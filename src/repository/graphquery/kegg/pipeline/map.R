require(kegg_graphquery);

const cache_dir = ?"--cache" || `${dirname(@script)}/.cache/`;
const Tcode     = ?"--tcode" || "map";

options(http.cache_dir = cache_dir);

# create resource url based on the organism Tcode exists or not
const url = (
    if (Tcode == "map") {
        "https://www.kegg.jp/pathway/map%s";
    } else {
        gsub("https://www.kegg.jp/pathway/%c%s", "%c", Tcode);
    }
);

const maps    = as.data.frame(pathway_category());
const repoDir = enumeratePath(maps, Tcode);
const id      = maps[, "entry"];

print("get all kegg pathway maps:");
str(maps);

print("all map id list at here:");
print(id);

for(i in 1:nrow(maps)) {
    const map = kegg_map(url = sprintf(url, id[i]));

    if ((map != "") && (!is.null(map))) {

        map
        |> xml
        |> writeLines(con = `${repoDir(i)}.XML`)
        ;

        # wget(sprintf(img, id[i]), `${repoDir(i)}.png`);
    }     
}