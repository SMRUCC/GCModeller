# kegg_api::get get object via kegg rest api: https://rest.kegg.jp

#' get kegg pathway
#'
#' @param id the pathway entry id in kegg database
#'
const kegg_pathway = function(id, cache = NULL) {
   kegg_api::get(id, cache)
   |> parseForm()
   |> as.pathway()
   ;
}

const kegg_reaction = function(id, cache_dir = NULL) {
    kegg_api::get(id, cache = cache_dir)
    |> parseForm(unsafe = TRUE)
    |> as.reaction()
    ;
}

const kegg_module = function(id, cache_dir = NULL) {
    kegg_api::get(id, cache = cache_dir)
    |> parseForm()
    |> as.module()
    ;
}

const kegg_compound = function(id, cache_dir = NULL) {
    kegg_api::get(id, cache = cache_dir)
    |> parseForm()
    |> as.compound()
    ;
}