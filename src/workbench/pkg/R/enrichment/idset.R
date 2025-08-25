#' split the id set in enrichment result into metabolite and genes id
#' 
#' @param IDs the column of IDs in enrichment result table
#' 
#' @return a tuple list that contains idset for each term result:
#' 
#'    list(metabolite = ..., genes = ...)
#' 
const split_omics_idset = function(IDs) {
    let idset = strsplit(IDs,"\s*;\s*");
    let metabolite = lapply(idset, list -> list == $"C\d+");
    let genes = lapply(idset, function(list, i) {
        i =  metabolite[[i]];
        list[!i];
    });

    metabolite = lapply(idset, function(list,i) {
        i = metabolite[[i]];
        list[i];
    });

    list(metabolite, genes);
}