require(kegg_api);

options(http.cache_dir = `${@dir}/.cache/`);

kegg_map(
    "map00020"
)
|> xml
|> writeLines(con = `${@dir}/pathwayMap.XML`)
;