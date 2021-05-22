# require(kegg_graphquery);
# "E:\GCModeller\src\repository\graphquery\kegg\test\compound.R" --attach "E:\GCModeller\src\repository\graphquery\kegg"

kegg_compound(

    "E:/GCModeller/src/repository/graphquery/kegg/test/compound.html"
)
:> xml
:> writeLines(con = `${dirname(@script)}/ATP.XML`)
;