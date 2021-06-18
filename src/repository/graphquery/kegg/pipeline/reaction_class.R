require(kegg_graphquery);

options(http.cache_dir = ?"--cache" || `${dirname(@script)}/.cache/`);

const all_category = as.data.frame(reactionclass_category());

print(all_category);

stop(1);

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
    const map = kegg_pathway(url = sprintf(url, id[i]));

    if ((map != "") && (!is.null(map))) {

        map
        |> xml
        |> writeLines(con = `${repoDir(i)}.XML`)
        ;

        # wget(sprintf(img, id[i]), `${repoDir(i)}.png`);
    }     
}