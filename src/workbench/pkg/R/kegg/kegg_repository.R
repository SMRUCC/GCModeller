#' Load internal kegg compound repository
#' 
const kegg_compounds as function() {
    using file as .readZipStream(
        zipfile = system.file("data/kegg/compounds.zip", package = "GCModeller")
    ) {
        repository::load.compounds(file);
    }
}

const kegg_maps as function() {

}

const kegg_reactions as function() {

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
const .readZipStream as function(zipfile, entryName = NULL) {
    using zip as open.zip(zipfile) {
        const names as string = as.object(zip)$ls;
        const data = zip[[entryName || names[1]]];

        # returns the target data stream
        # object! 
        data;
    }
}