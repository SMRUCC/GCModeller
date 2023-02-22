
#' Create metpa background model for LCMS analysis
#' 
const metpa_background = function() {

}

const .write_dgr = function(dgr) {
    const vec = [dgr]::dgr;

    if (!is.null(vec)) {
        names(vec) = [dgr]::kegg_id;
    }

    vec;
}

const .write_dgrlist = function(dgrList) {
    const dgrlist = lapply([dgrList]::pathways, map -> .write_dgr([map]::Value));

    print("view of the pathway network degree data:");
    str(dgrlist);

    dgrlist;
}