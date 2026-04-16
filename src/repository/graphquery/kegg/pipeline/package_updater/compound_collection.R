require(kegg_api);
require(GCModeller);
require(REnv);

imports "repository" from "kegg_kit";

const cache_dir = `${@dir}/compound_cache.db`
|> HDS::openStream(allowCreate = TRUE, meta_size = 32*1024*1024);
const cache_fs = cache_dir;
const url_templ as string = "https://www.kegg.jp/dbget-bin/www_bget?cpd:%s";

options(http.cache_dir = cache_dir);

let index = REnv::getHtml("https://rest.kegg.jp/list/compound", interval = 3, filetype = "html");
index = trim(index);
index = strsplit(index, "\n");
index = lapply(index, si -> strsplit(si, "\t"));
index = lapply(index, i -> i[2], names = i -> i[1]);

str(index);

# index = as.list(names(index), names = unlist(index));

for(cid in tqdm(names(index))) {
    let cid_name = gsub(index[[cid]], "[\\/]", "_", regexp = TRUE);
    let fs_filepath = `/compounds/${cid}.xml`;

    if (!file.exists(fs_filepath, fs = cache_fs)) {
        let keg_compound = kegg_api::kegg_compound(cid, cache = cache_dir);

        # print(cache_fs);
        # print(xml(keg_compound));

        HDS::writeText(cache_fs, fs_filepath, xml(keg_compound));
        HDS::flush(cache_fs);

        sleep(1);
    } else {
        let xml_text = HDS::getText(cache_fs, fs_filepath);
        let data = loadXml(xml_text, typeof = "kegg_compound");

        if ([data]::formula == "") {
            let keg_compound = kegg_api::kegg_compound(cid, cache = cache_dir);

            # print(cache_fs);
            # print(xml(keg_compound));

            HDS::writeText(cache_fs, fs_filepath, xml(keg_compound));
            HDS::flush(cache_fs);

            sleep(1);
        }
    }
}