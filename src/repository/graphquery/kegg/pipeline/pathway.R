require(kegg_graphquery);
require(HDS);

const cache_dir = ?"--cache" || `${dirname(@script)}/.cache/`;
const Tcode     = ?"--tcode" || "map";

options(http.cache_dir = cache_dir);

# create resource url based on the organism Tcode exists or not
const url = (
    if (Tcode == "map") {
        "https://www.kegg.jp/entry/ko%s";
    } else {
        gsub("https://www.genome.jp/entry/pathway+%c%s", "%c", Tcode);
    }
);
const img     = "https://www.kegg.jp/kegg/pathway/ko/ko%s.png";
const maps    = as.data.frame(pathway_category());
const repoDir = enumeratePath(maps, Tcode);
const id      = maps[, "entry"];

print("get all kegg pathway maps:");
str(maps);

for(i in 1:nrow(maps)) {
    const url = sprintf(url, id[i]);
    const map = kegg_pathway(url);

    if ((map != "") && (!is.null(map))) {
        map
        |> xml
        |> writeLines(con = `${repoDir(i)}.XML`)
        ;

        # wget(sprintf(img, id[i]), `${repoDir(i)}.png`);
    }     
}