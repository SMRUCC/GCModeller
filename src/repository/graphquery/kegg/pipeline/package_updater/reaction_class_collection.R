require(kegg_api);
require(GCModeller);
require(REnv);

imports "http" from "webkit";
imports "repository" from "kegg_kit";

const cache_dir = `${@dir}/reaction_cache.db`
|> HDS::openStream(allowCreate = TRUE, meta_size = 32*1024*1024);
const cache_fs = http::http.cache(cache_dir);

options(http.cache_dir = cache_dir);

let index = loadReactionIDs();

rownames(index) = index$reaction_id;

print(index);

for(rxn in tqdm(as.list(index, byrow = TRUE))) {
    # let name = index[[rid]]$name;
    let rid = rxn$reaction_id;
    let fs_filepath = `/reactions/${rid}.xml`;

    if (!file.exists(fs_filepath, fs = cache_dir)) {
        rxn = kegg_reaction(rid, cache_fs);

        HDS::writeText(cache_dir, fs_filepath, xml(rxn),allocate = FALSE);
        HDS::flush(cache_dir);

        sleep(1);
    } 
}