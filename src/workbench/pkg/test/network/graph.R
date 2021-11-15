require(GCModeller);

const pack = list();
const enrichments = ["U:\项目以外内容\2021\研发\KYYF0006-空间代谢组学\20210813_Test_kidney\FULL MS_centriod_CHCA_20210819\2.0\20211115-1\MS1_analysis"]
|> list.dirs(full.names = TRUE)
|> sapply(dir -> `${dir}/visual/kegg/pathway_results.csv`)
|> which(file.exists)
|> lapply(function(file) {    
    const [compounds, Raw.p] = read.csv(file);
    const psize = -log10(Raw.p);    

    for(i in 1:length(Raw.p)) {
        const cid = strsplit(compounds[i], "[+,;]|\s+");

        for(id in cid) {
            if (id in pack) {
                pack[[id]] = pack[[id]] + psize[i];
            } else {
                pack[[id]] = psize[i];
            }
        }
    }

    pack;
})
;

str(pack);

const info = data.frame(
    kegg_id = names(pack), 
    weight  = sapply(names(pack), id -> pack[[id]])
);

print(info);