require(GCModeller);

imports "dbget" from "kegg_kit";

dbget::getMap("map01200")
|> xml()
|> writeLines(
    con = `${@dir}/kegg_map.XML`
)