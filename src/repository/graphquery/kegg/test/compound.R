require(kegg_graphquery);
# "E:\GCModeller\src\repository\graphquery\kegg\test\compound.R" --attach "E:\GCModeller\src\repository\graphquery\kegg"

options(http.cache_dir = `${dirname(@script)}/.cache/`);

kegg_compound(

    "https://www.kegg.jp/entry/C00001"
)
|> xml
|> writeLines(con = `${dirname(@script)}/Epicatechin.XML`)
;