const loadReactionIDs = function() {
    const data = "https://rest.kegg.jp/list/reaction"
    |> requests.get()
    |> readLines()
    |> strsplit("\t")
    ;
    const id   = sapply(data, i -> i[1]);
    const name = sapply(data, i -> i[2]);
    
    data.frame(reaction_id = id, name = name);
}