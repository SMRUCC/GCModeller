#' Load internal kegg compound repository
#' 
const kegg_compounds = function(rawList = FALSE) {
    using file as .readZipStream(
        zipfile = system.file("data/kegg/compounds.zip", package = "GCModeller")
    ) {
        repository::load.compounds(file, rawList = rawList);
    }
}

#' Load internal kegg map repository
#' 
#' @param rawMaps this parameter value determine the data type of 
#'    the return value from this function. ``TRUE`` value
#'    of this parameter will returns a vector of map data object
#'    or a indexed map repository object if set the parameter value
#'    to value ``FALSE``.
#' 
#' @param repo the kegg map repository location, default internal 
#'    repository will be used if this parameter value is leaves 
#'    default.
#' 
const kegg_maps = function(rawMaps = TRUE, repo = system.file("data/kegg/KEGG_maps.zip", package = "GCModeller")) {
    if (file.ext(repo) == "zip") {
        print(`repository file(${repo}) is a zip package.`);

        using file as .readZipStream(zipfile = repo) {
            repository::load.maps(file, rawMaps = rawMaps);
        }
    } else {
        # from a directory or a bundle pack of xml file
        repo |> repository::load.maps(rawMaps = rawMaps);
    }
}

#' Load internal kegg reaction repository
#' 
const kegg_reactions = function(raw = TRUE) {
    using file as .readZipStream(
        zipfile = system.file("data/kegg/reactions.zip", package = "GCModeller")
    ) {
        repository::load.reactions(file, raw = raw);
    }
}

