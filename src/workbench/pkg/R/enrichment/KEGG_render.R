
#' Do rendering of the kegg pathway maps
#' 
#' @param enrich the csv file path of the kegg pathway map enrichment
#'    result or the dataframe value it self
#' 
const KEGG_MapRender = function(enrich, 
        map_id = "KEGG",
        pathway_links = "pathway_links",
        outputdir = "./") {

    const KEGG_maps = kegg_maps(rawMaps = FALSE);

    if (is.character(enrich)) {
        enrich = read.csv(
            file = enrich, 
            row.names = NULL, 
            check.names = FALSE, 
            tsv = file.ext(enrich) != "csv"
        );
    }

    map_id = enrich[, map_id];
    pathway_links = enrich[, pathway_links];
    pathway_links = as.list(pathway_links, names = map_id);

    GCModeller::localRenderMap(KEGG_maps, pathwayList = pathway_links,
                                compoundcolors = "red",
                                gene_highights = "blue",
                                outputdir      = outputdir
                            );
}