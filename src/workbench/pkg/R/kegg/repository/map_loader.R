
#' this function try to unify the kegg map load method
#' 
#' @param kegg_maps the kegg map data source, could be a zip archive file that 
#'    contains the kegg maps data message, a directory path that contains the
#'    kegg maps xml files. default parameter value is null, means use the internal
#'    resource data for run the downstream analysis. 
#' 
const __load_kegg_map = function(kegg_maps = NULL) {
    print("inspect of the kegg_maps source:");
    print(kegg_maps);

    GCModeller::kegg_maps(rawMaps = FALSE, repo = {
        if (file.exists(kegg_maps) || dir.exists(kegg_maps)) {
            kegg_maps;
        } else {
            system.file("data/kegg/KEGG_maps.zip", package = "GCModeller");
        }
    });
}