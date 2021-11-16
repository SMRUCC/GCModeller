# require(kegg_graphquery);

options(http.cache_dir = `${dirname(@script)}/.cache/`);

kegg_reactionclass (
     "E:\GCModeller\src\repository\graphquery\kegg\test\reaction_class.html"
 )
 |> xml
 |> writeLines(con = `${dirname(@script)}/reaction_class.XML`)
 ;