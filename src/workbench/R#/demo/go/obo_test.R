imports "OBO" from "annotationKit";

obo = OBO::read.obo("K:\Downloads\chebi.obo");
obo 
|> filter.is_obsolete() 
|> filter_properties(["monoisotopicmass","inchikey","inchi","smiles"])
|> write.obo( "K:\Downloads\chebi_lite.obo",  excludes = ["synonym", "relationship","alt_id","xref","subset"])
;