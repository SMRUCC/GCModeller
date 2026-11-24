#' Parse the kegg object set that encoded inside the kegg url string
#' 
#' @param compoundcolors color value for kegg compound
#' @param gene_highights color value for highlights genes/proteins
#' 
#' @return this function will ensure that returns a tuple list that contains 
#'    the slot value ``objects`` always.
#' 
const .safe_kegg_url_parser = function(url, compoundcolors, gene_highights) {
    let URL_kegg = NULL;

    try({
        URL_kegg <- url |> report.utils::parseKeggUrl(
            compound = compoundcolors, 
            gene     = gene_highights
        );
    });

    if (is.null(URL_kegg)) {
        # error while parse the url string
        warning(`error while parse the kegg map highlight url: "${toString(url)}"!`, immediate. = TRUE);
        # generates a fake empty url object
        URL_kegg = {
            objects: list()
        };
    } 

    URL_kegg;
}