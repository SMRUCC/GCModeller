
#' this function try to unify the kegg map load method
#' 
#' @param kegg_maps the kegg map data source, could be a zip archive file that 
#'    contains the kegg maps data message, a directory path that contains the
#'    kegg maps xml files. default parameter value is null, means use the internal
#'    resource data for run the downstream analysis. 
#' 
const __load_kegg_map = function(kegg_maps = NULL, raw_maps = FALSE) {
    let internal_resource = system.file("data/kegg/KEGG_maps.msgpack", package = "GCModeller");

    print("inspect of the kegg_maps source:");
    print(kegg_maps);

    GCModeller::kegg_maps(rawMaps = raw_maps, repo = {
        if (!is.null(kegg_maps)) {
            if (file.exists(kegg_maps) || dir.exists(kegg_maps)) {
                kegg_maps;
            } else {
                warning(["the given kegg maps resource is not valid, use the internal resource data by default", kegg_maps]);
                internal_resource;
            }            
        } else {
            internal_resource;
        }
    });
}

const load_kegg_maps = function(raw_maps = TRUE) {
    __load_kegg_map(NULL, raw_maps);
}