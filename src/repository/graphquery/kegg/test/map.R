
options(http.cache_dir = `${dirname(@script)}/.cache/`);

kegg_map(
    "D:\GCModeller\src\repository\graphquery\kegg\test\map.html"
)
|> xml
|> writeLines(con = `${dirname(@script)}/pathway.XML`)
;