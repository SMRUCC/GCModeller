require(kegg_api);
require(GCModeller);
require(REnv);

const cache_dir = `${@dir}/map_cache.db`
|> HDS::openStream(allowCreate = TRUE, meta_size = 32*1024*1024);

options(http.cache_dir = cache_dir);

let index = REnv::getHtml("https://rest.kegg.jp/list/pathway", interval = 3, filetype = "html");
index = strsplit(index, "\n");
index = lapply(index, si -> strsplit(si, "\t"));
index = lapply(index, i -> i[2], names = i -> i[1]);

str(index);

# kegg_map("map00600", fs = getOption("http.cache_dir"));

# for(id in names(index)) {
#     kegg_map(id, fs = getOption("http.cache_dir"));
# }

fetch_kegg_maps(cache_dir);