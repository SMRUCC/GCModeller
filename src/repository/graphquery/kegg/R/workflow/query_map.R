const __query_map = function(map, cache_dir = "./") {
    const cache_fs = [cache_dir]::fs;
    const pwy = kegg_api::kegg_pathway(`ko${map$entry}`, cache = cache_dir);
    let dir = `/${map$class}/${map$category}/${map$entry} - ${map$name}/`;

    HDS::writeText(cache_fs, `${dir}/map.xml`, xml(pwy));
    HDS::flush(cache_fs);

    let compoundList = as.data.frame([pwy]::compound);

    print(compoundList);

    # compound inside pathway root
    for(cid in as.list(compoundList, byrow = TRUE)) {
        let cpd = kegg_api::kegg_compound(cid$name, cache = cache_dir);
        let cfile = `${dir}/compounds/${cid$name} - ${cid$text}.xml`;

        # str(cpd);
        print(cfile);

        HDS::writeText(cache_fs, cfile, xml(cpd));
        HDS::flush(cache_fs);

        # sleep(1);
    }

    let modules = as.data.frame([pwy]::modules);

    print(modules, max.print = 6);

    for(mid in as.list(modules, byrow = TRUE)) {
        let mod = kegg_api::kegg_module(mid$name, cache = cache_dir);
        let mdir = `${dir}/modules/${mid$name} - ${mid$text}/`;

        HDS::writeText(cache_fs, `${mdir}/module.xml`, xml(mod));
        HDS::flush(cache_fs);

        # str(mod);
        let reactions = as.data.frame([mod]::reaction);

        print(reactions, max.print = 6);

        for(rid in as.list(reactions, byrow = TRUE)) {
            let rxn = kegg_api::kegg_reaction(rid$name, cache = cache_dir);
            let rfile = `${mdir}/reactions/${rid$name} - ${rid$text}.xml`;

            HDS::writeText(cache_fs, rfile, xml(rxn));
            HDS::flush(cache_fs);

            # str(rxn);
            # sleep(1);
            # stop();
        }

        __query_compounds(as.data.frame([mod]::compound), 
            fs_dir = mdir, 
            cache_dir = cache_dir);
    }

    invisible(NULL);
}
