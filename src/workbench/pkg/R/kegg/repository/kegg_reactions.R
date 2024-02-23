
#' Load internal kegg reaction repository
#' 
const kegg_reactions = function(raw = TRUE) {
    using file as .readZipStream(
        zipfile = system.file("data/kegg/reactions.zip", package = "GCModeller")
    ) {
        repository::load.reactions(file, raw = raw);
    }
}

const extract_reactions = function() {
    const reference_db = system.file("data/kegg/kegg.zip", package = "GCModeller");

    # get the database file stream
    using file as HDS::openStream(
            file = .readZipStream(zipfile = reference_db),
            readonly = TRUE) {

        __hds_compound_files(
            kegg_db = file, 
            flag = "reactions", 
            what = "kegg_reaction");
    }
}