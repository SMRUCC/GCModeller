
#' Load internal kegg reaction repository
#' 
const kegg_reactions = function(raw = TRUE) {
    using file as .readZipStream(
        zipfile = system.file("data/kegg/reactions.zip", package = "GCModeller")
    ) {
        repository::load.reactions(file, raw = raw);
    }
}
