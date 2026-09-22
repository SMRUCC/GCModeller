imports ["repository", "network"] from "kegg_kit";

require(igraph);

#' Create kegg compound network
#' 
#' @param compoundsId a ``dataframe`` object that contains the kegg compound 
#'    cid as the node item in the network graph data. there are fields in this 
#'    dataframe that must be included: 
#'
#'      1. kegg_id: is the kegg compound cid
#'      2. weight: apply for measure the node size 
#' 
#' @param make.pathway_cluster make node cluster info via pathway maps 
#'    information?
#'
#' @return a network graph object model, which can be export as table file or
#'    run network visualization rendering in the downstream data analysis.
#'  
const CompoundNetwork = function(compoundsId, make.pathway_cluster = TRUE) {
    const compounds = GCModeller::kegg_compounds();
    const reactions = GCModeller::kegg_reactions();
    const graph     = network::fromCompounds(
        compoundsId = compoundsId[, "kegg_id"], 
        graph       = reactions, 
        compounds   = compounds
    );

    # apply of the network graph attributes
    # 
    # 1. config of the node size
    
    # 2. create node cluster based on the pathway 
    #    clusters which is defined by the kegg pathway 
    #    maps data.

    graph;
}