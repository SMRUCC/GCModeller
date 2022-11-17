imports "kegg_api" from "kegg_kit";

const list_pathway = function(org = ["ko", "map", "hsa"], cache = NULL) {
    const list_data = kegg_api::listing("pathway", org[1] || "map", cache = cache);
    const idlist as string = names(list_data) 
    |> sapply(x -> strsplit(x, ":", fixed = TRUE)[2])
    ;

    names(list_data) = idlist;
    str(list_data);
    
    list_data;
}