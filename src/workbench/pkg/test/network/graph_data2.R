require(GCModeller);

let pack = NULL;

const enrichments = ["U:\项目以外内容\2021\研发\KYYF0006-空间代谢组学\20210813_Test_kidney\FULL MS_centriod_CHCA_20210819\20210922_400\MS1_analysis"]
|> list.dirs(full.names = TRUE)
|> sapply(dir -> `${dir}/visual/kegg/pathway_results.csv`)
|> which(file.exists)
|> lapply(function(file) {    
    print(file);

    pack = rbind(pack, read.csv(file));
    pack;
})
;

str(pack);

write.csv(pack, file = `${@dir}/all_enriched.csv`, row.names = FALSE);