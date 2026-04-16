
#' get query of the kegg compound
#' 
#' @param compounds a dataframe object that contains the a set of 
#'    the kegg compounds for run data downloads, the two data fields: 
#'    name and text should be includes in this dataframe object.
#' @param cache_dir the cache filesystem object
#' 
const __query_compounds = function(compounds, fs_dir = "/", cache_dir = "./") {
    const cache_fs = [cache_dir]::fs;
    
    print("view of the given kegg compound set:");
    print(compounds, max.print = 6);

    # compounds inside the pathway module
    for(cid in as.list(compounds, byrow = TRUE)) {
        let cpd = kegg_api::kegg_compound(cid$name, cache = cache_dir);
        let fs_filepath = `${fs_dir}/compounds/${cid$name} - ${cid$text}.xml`;

        print(fs_filepath);

        HDS::writeText(cache_fs, fs_filepath, xml(cpd));
        HDS::flush(cache_fs);
    }

    invisible(NULL);
}