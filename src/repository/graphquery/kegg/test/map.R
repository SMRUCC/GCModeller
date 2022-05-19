
options(http.cache_dir = `${dirname(@script)}/.cache/`);

kegg_map(
    "https://www.kegg.jp/pathway/map00020"
)
|> xml
|> writeLines(con = `${dirname(@script)}/pathwayMap.XML`)
;