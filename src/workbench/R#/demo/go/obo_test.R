imports "OBO" from "annotationKit";

obo = OBO::read.obo("K:\Downloads\chebi.obo");
obo |> write.obo( "K:\Downloads\chebi_lite.obo",  excludes = ["synonym", "relationship"]);