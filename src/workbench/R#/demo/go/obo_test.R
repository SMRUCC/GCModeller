imports "OBO" from "annotationKit";

obo = OBO::read.obo("E:\mzkit\src\mzkit\rstudio\data\chebi.obo");
obo 
|> filter.is_obsolete() 
|> filter_properties(["monoisotopicmass","inchikey","inchi","smiles"])
|> write.obo( "E:\mzkit\src\mzkit\rstudio\data\chebi_lite.obo",  excludes = ["synonym", "relationship","alt_id","xref","subset"])
;