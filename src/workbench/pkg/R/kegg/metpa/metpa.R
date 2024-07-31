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
#' @param multiple_omics set the parameter value to TRUE for create background
#'    model for multiple omics data analysis. then this parameter option will
#'    enable the gene and compound existsed inside the model.
#' 
#' @return a gcmodeller background model if raw is true, and a R# object list will 
#'    be converts from the clr background model otherwise the raw is not.
#' 
const metpa_background = function(pathways, taxonomy_name = NULL, raw = TRUE, 
                                  multiple_omics = FALSE) {

    const kegg_links = GCModeller::kegg_reactions(raw = TRUE);
    const metpa = background::metpa(
        kegg = pathways,
        reactions = kegg_links,
        org_name = taxonomy_name,
        is_ko_ref = FALSE,
        multipleOmics = multiple_omics
    );

    if (!raw) {
        .cast_CLR_metpa(metpa); 
    } else {
        metpa;
    }    
}
