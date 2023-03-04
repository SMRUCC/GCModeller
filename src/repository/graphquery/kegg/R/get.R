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

const kegg_reaction = function(id, cache = NULL) {
    kegg_api::get(id, cache = cache_dir)
    |> parseForm(unsafe = TRUE)
    |> as.reaction()
    ;
}