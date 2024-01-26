
#' Do rendering of the kegg pathway maps
#' 
#' @param enrich the csv file path of the kegg pathway map enrichment
#'    result or the dataframe value it self
#' @param kegg_maps the external kegg maps repository, could be a 
#'    local directory path which contains a set of the kegg map xml
#'    file; or a file to the bundle pack of the kegg map xml files.
#' 
const KEGG_MapRender = function(enrich, 
    map_id = "KEGG",
    pathway_links = "pathway_links",
    outputdir = "./",
    min_objects = 0,
    kegg_maps = NULL) {

    const KEGG_maps = GCModeller::kegg_maps(rawMaps = FALSE, repo = {
        print("inspect of the kegg_maps source:");
        print(kegg_maps);

        if (file.exists(kegg_maps) || dir.exists(kegg_maps)) {
            kegg_maps;
        } else {
            system.file("data/kegg/KEGG_maps.zip", package = "GCModeller");
        }
    });

    if (is.character(enrich)) {
        enrich = read.csv(
            file = enrich, 
            row.names = NULL, 
            check.names = FALSE, 
            tsv = file.ext(enrich) != "csv"
        );
    }

    map_id        = enrich[, map_id];
    pathway_links = enrich[, pathway_links];
    pathway_links = as.list(pathway_links, names = map_id);

    str(pathway_links);

    GCModeller::localRenderMap(
        KEGG_maps, 
        pathwayList    = pathway_links,
        compoundcolors = "red",
        gene_highights = "blue",
        outputdir      = outputdir,
        min_objects    = as.integer(min_objects)
    );
}