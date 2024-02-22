const __query_map = function(map, cache_dir = "./") {
    const cache_fs = [cache_dir]::fs;
    const pwy      = kegg_api::kegg_pathway(`ko${map$entry}`, cache = cache_dir);
    const dir      = `/${map$class}/${map$category}/${map$entry} - ${map$name}/`;
    const modules  = as.data.frame([pwy]::modules);

    HDS::writeText(cache_fs, `${dir}/map.xml`, xml(pwy));
    HDS::flush(cache_fs);

    # query the kegg compound data inside 
    # current pathway model
    __query_compounds(as.data.frame([pwy]::compound), 
        fs_dir = dir, 
        cache_dir = cache_dir);

    print("view list of the modules inside current pathway map:");
    print(modules, max.print = 6);

    for(mid in as.list(modules, byrow = TRUE)) {
        __query_module(mid, 
            fs_dir = dir, 
            cache_dir = cache_dir);
    }

    invisible(NULL);
}