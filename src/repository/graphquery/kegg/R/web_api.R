imports "kegg_api" from "kegg_kit";

const list_pathway = function(org = ["ko", "map", "hsa"], cache = NULL) {
    const tcode as string = org[1] || "map";
    const list_data = kegg_api::listing("pathway", tcode, cache = cache);
    const idlist as string = list_data 
    |> names() 
    |> sapply(x -> strsplit(x, ":", fixed = TRUE)[2])
    ;

    if (org != "map") {
        names(list_data) = idlist;
    }

    # previews of the entry id list data:
    str(list_data);

    list_data;
}