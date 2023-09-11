require(GCModeller);
require(kegg_api);
require(HDS);

imports "brite" from "kegg_kit";

let brite = brite::parse("br08901");
let df = as.data.frame(brite);
 
const cache_dir = `${@dir}/cache.db`
|> HDS::openStream(allowCreate = TRUE, meta_size = 32*1024*1024)
|> http::http.cache()
;
const cache_fs = [cache_dir]::fs;

print(df, max.print = 6);

for(map in as.list(df, byrow = TRUE)) {
    let pwy = kegg_api::kegg_pathway(`map${map$entry}`, cache = cache_dir);
    let dir = `/${map$class}/${map$category}/${map$entry} - ${map$name}/`;

    HDS::writeText(cache_fs, `${dir}/map.xml`, xml(pwy));
    HDS::flush(cache_fs);

    let modules = as.data.frame([pwy]::modules);

    print(modules, max.print = 6);

    for(mid in as.list(modules, byrow = TRUE)) {
        let mod = kegg_api::kegg_module(mid$name, cache = cache_dir);
        let mdir = `${dir}/modules/${mid$name} - ${mid$text}/`;

        HDS::writeText(cache_fs, `${mdir}/module.xml`, xml(mod));
        HDS::flush(cache_fs);

        str(mod);
        let reactions = as.data.frame([mod]::reaction);

        print(reactions, max.print = 6);

        for(rid in as.list(reactions, byrow = TRUE)) {
            let rxn = kegg_api::kegg_reaction(rid$name, cache = cache_dir);
            let rfile = `${mdir}/reactions/${rid$name} - ${rid$text}.xml`;

            HDS::writeText(cache_fs, rfile, xml(rxn));
            HDS::flush(cache_fs);

            # str(rxn);

            # stop();
            NULL;
        }

        let compounds = as.data.frame([mod]::compound);

        print(compounds, max.print = 6);

        for(cid in as.list(compounds, byrow = TRUE)) {
            let cpd = kegg_api::kegg_compound(cid$name, cache = cache_dir);
            let cfile = `${mdir}/compounds/${cid$name} - ${cid$text}.xml`;

            str(cpd);

            HDS::writeText(cache_fs, cfile, xml(cpd));
            HDS::flush(cache_fs);

            NULL;
        }

        # stop();
    }

    str(pwy);
    str(map);

    # stop();

    NULL;
}

HDS::flush(cache_fs);