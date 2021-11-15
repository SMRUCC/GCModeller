require(igraph);

imports "repository" from "kegg_kit";

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
const CompoundNetwork as function(compoundsId, make.pathway_cluster = TRUE) {

}