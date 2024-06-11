
#' Load internal kegg reaction repository
#' 
#' @details this function extract the reaction data both of associated 
#'   with the kegg pathway map or the reactions has no associations.
#' 
const kegg_reactions = function(raw = TRUE) {
    let s = file(system.file("data/kegg/KEGG_reactions.msgpack", package = "GCModeller"));
    let rxns = repository::load.reactions(s, raw = raw);
    
    close(s);

    # returns all kegg reactions
    rxns;
}

#' extract reaction data from the reference database
#' 
#' @details this function extract the reaction data which 
#'    has the pathway map association.
#' 
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