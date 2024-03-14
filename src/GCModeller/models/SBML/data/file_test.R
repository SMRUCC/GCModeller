require(GCModeller);
require(JSON);

imports "SBML" from "biosystem";

file.path(@dir, "R-HSA-211945.sbgn")
|> read.sbgn
|> JSON::json_encode()
|> writeLines(con = file.path(@dir, "R-HSA-211945.json"))
;
