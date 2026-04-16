imports "kegg_api" from "kegg_api";

#' get kegg pathway id list via kegg rest api
#'  
const list_pathway = function(org = ["ko", "map", "hsa"], cache = NULL) {
    const tcode as string = org[1] || "map";
    const list_data = kegg_api::listing("pathway", tcode, cache = cache);

    print("previews of the entry id list data:"); 
    str(list_data);

    list_data;
}