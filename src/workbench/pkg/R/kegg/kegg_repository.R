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
const kegg_maps = function(rawMaps = TRUE, repo = system.file("data/kegg/KEGG_maps.zip", package = "GCModeller")) {
    using file as .readZipStream(zipfile = repo) {
        repository::load.maps(file, rawMaps = rawMaps);
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

#' Read zip stream data
#' 
#' @description this function open the given zip archive 
#'     file and then load the first file as data stream 
#'     object if the zip entry name is missing. 
#' 
#' @param zipfile the file path of the target zip file to 
#'     read data stream
#' @param entryName the zip entry name.
#' 
#' @return A data steam object that read from the 
#'     given zip archive file.
#' 
const .readZipStream = function(zipfile, entryName = NULL) {
    using zip as open.zip(zipfile) {
		const zipfile = as.object(zip);
        const names as string = zipfile$ls;
        const data = zip[[entryName || names[1]]];

        # returns the target data stream
        # object! 
        data;
    }
}