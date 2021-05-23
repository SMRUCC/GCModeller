require(kegg_graphquery);

options(http.cache_dir = `${dirname(@script)}/.cache/`);

 kegg_pathway(
     "E:\GCModeller\src\repository\graphquery\kegg\test\pathway.html"
 );