#' Load internal kegg compound repository
#' 
const kegg_compounds = function(rawList = FALSE) {
    using file as .readZipStream(
        zipfile = system.file("data/kegg/compounds.zip", package = "GCModeller")
    ) {
        repository::load.compounds(file, rawList = rawList);
    }
}


