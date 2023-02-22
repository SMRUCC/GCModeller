
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
    const list = list();

    for(map in [dgrList]::pathways) {
        const map_id = [map]::Name;
        const dgr = .write_dgr([map]::Value);

        list[[map_id]] = dgr;
    }

    print("view of the pathway network degree data:");
    str(list);

    list;
}