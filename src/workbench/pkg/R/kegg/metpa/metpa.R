require(igraph);

imports "background" from "gseakit";

#' Create metpa background model for LCMS analysis
#' 
#' @details the reaction data is loaded from the package internal resource file
#' 
#' @param pathways 
#' @param raw if this parameter value is set to TRUE, then a raw CLR object 
#'    will be returns, otherwise a converted list object will be generated 
#'    from the raw CLR object. 
#'
const metpa_background = function(pathways, taxonomy_name = NULL, raw = TRUE) {
    const kegg_links = GCModeller::kegg_reactions(raw = TRUE);
    const metpa = background::metpa(
        kegg = pathways,
        reactions = kegg_links,
        org_name = taxonomy_name,
        is_ko_ref = FALSE
    );

    if (!raw) {
        .cast_CLR_metpa(metpa); 
    } else {
        metpa;
    }    
}
