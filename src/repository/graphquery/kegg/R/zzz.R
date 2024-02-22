require(JSON);
require(GCModeller);
require(HDS);

imports "brite" from "kegg_kit";
imports "kegg_api" from "kegg_kit";
imports "parser" from "kegg_api";

const .onLoad = function() {
   print("KEGG API is a REST-style Application Programming Interface to the KEGG database resource.");
}
