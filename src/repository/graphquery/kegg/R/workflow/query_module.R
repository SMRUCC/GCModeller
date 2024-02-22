const __query_module = function(mid, fs_dir = "/", cache_dir = "./") {
    const cache_fs  = [cache_dir]::fs;
    const mod       = kegg_api::kegg_module(mid$name, cache = cache_dir);
    const mdir      = `${fs_dir}/modules/${mid$name} - ${mid$text}/`;
    const reactions = as.data.frame([mod]::reaction);

    HDS::writeText(cache_fs, `${mdir}/module.xml`, xml(mod));
    HDS::flush(cache_fs);

    print("view list of the kegg reaction links to get query kegg data model:");
    print(reactions, max.print = 6);

    for(rid in as.list(reactions, byrow = TRUE)) {
        let rxn = kegg_api::kegg_reaction(rid$name, cache = cache_dir);
        let rfile = `${mdir}/reactions/${rid$name} - ${rid$text}.xml`;

        HDS::writeText(cache_fs, rfile, xml(rxn));
        HDS::flush(cache_fs);
    }

    __query_compounds(as.data.frame([mod]::compound), 
        fs_dir = mdir, 
        cache_dir = cache_dir);

    invisible(NULL);
}