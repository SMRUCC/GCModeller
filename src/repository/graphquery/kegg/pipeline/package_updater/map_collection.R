require(kegg_api);
require(GCModeller);
require(REnv);

options(http.cache_dir = ?"--cache" || `${@dir}/.cache/maps/`);

let index = REnv::getHtml("https://rest.kegg.jp/list/pathway", interval = 3, filetype = "html");
index = strsplit(index, "\n");
index = lapply(index, si -> strsplit(si, "\t"));
index = lapply(index, i -> i[2], names = i -> i[1]);

str(index);
