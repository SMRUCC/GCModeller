#' obtain a list of entry identifiers and associated definition of KEGG reactions
#' 
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