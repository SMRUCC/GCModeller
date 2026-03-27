const link_ko = function(kegg_id, cache = "./tmp", batch_size = 100) {
    imports "http" from "webKit";

    let cached = list.files(cache, pattern = "*.txt");
    cached = lapply(cached, file => read.table(file, header = FALSE, row.names = NULL, sep = "\t"));
    cached = lapply(cached, t => t[,1]);
    cached = unlist(cached) |> unique();

    let hits = kegg_id in cached; 

    kegg_id = kegg_id[!hits];

    for(batch in tqdm(kegg_id |> split( size = batch_size))) {
        let url = `https://rest.kegg.jp/link/ko/${paste(batch, sep = "+")}`;
        let key = md5(url);

        url 
        |> requests.get() 
        |> plain_text 
        |> writeLines(con = file.path(cache, `${key}.txt`))
        ;

        sleep(1);
    }
}