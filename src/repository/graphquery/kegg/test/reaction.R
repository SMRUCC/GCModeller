# require(kegg_graphquery);

options(http.cache_dir = `${dirname(@script)}/.cache/`);

kegg_reaction (
     "E:\GCModeller\src\repository\graphquery\kegg\test\reaction.html"
 )
 |> xml
 |> writeLines(con = `${dirname(@script)}/reaction.XML`)
 ;