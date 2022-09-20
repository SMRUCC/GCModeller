imports "OBO" from "annotationKit";

obo = OBO::read.obo("K:\Downloads\chebi.obo");
obo 
|> filter.is_obsolete() 
|> write.obo( "K:\Downloads\chebi_lite.obo",  excludes = ["synonym", "relationship","alt_id","xref","subset"])
;