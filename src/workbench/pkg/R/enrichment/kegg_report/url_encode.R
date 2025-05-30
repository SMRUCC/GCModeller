
const .url_encode = function(enrich, data = NULL, log2FC = "log2(FC)", id = "kegg") {
    if (!is.null(enrich)) {
        .url_encode_internal(enrich, data, log2FC, id);
    } else {
        enrich;
    }
}

const .url_encode_internal = function(enrich, data = NULL, log2FC = "log2(FC)", id = "kegg") {
    let has_data  = !is.null(data);
    let has_logfc = log2FC in data;

    if (length(has_logfc) == 0) {
        has_logfc = FALSE;
    }

    if (has_data && has_logfc) {
        # do colorful encode of the url
        const encode_url = kegg_colors(
            id = data[, id], 
            log2FC = data[, log2FC]
        );

        enrich[, "links"] = encode_url(
            map_id = rownames(enrich), 
            idset = enrich$geneIDs
        );
    } else {
        # just do normal map url encode
        # http://www.kegg.jp/pathway/map01230+C00037+C00049+C00082+C00188
        enrich[, "links"] = sprintf("http://www.kegg.jp/pathway/%s+%s", rownames(enrich), sapply(enrich$geneIDs, set -> paste(__parseIdvector(set), sep = "+")));
    }
}

const __parseIdvector = function(set) {
    unlist(strsplit(set, ";\s*"));
}

#' generate color map for do colorful encode of the kegg map url
#' 
#' @param id a character vector of a collection of the kegg id
#' @param log2FC a numeric vector of the log2 Foldchange value of the corresponding kegg id
#' 
#' @return a lambda function that accept two parameters for generates the kegg pathway map url:
#' 
#'   1. map_id: the kegg pathway map id
#'   2. idset: a character vector of the kegg id for make url 
#' 
const kegg_colors = function(id, log2FC, up = "red", down = "blue") {
    let i  = (id != "") && (id != "NULL");  

    id     = id[i];
    log2FC = log2FC[i];
    log2FC = as.list(ifelse(log2FC > 0, up, down), names = id);

    # http://www.kegg.jp/pathway/map01230/C00037/red/C00049/blue
    const encode_colors = function(set) {
        set 
        |> sapply(function(id) {
            if (id in log2FC) {
                `${id}/${log2FC[[id]]}`;
            } else {
                `${id}/green`;
            }
        })
        |> paste(sep = "/")
        ;
    }
    const encode_url = function(map_id, idset) {
        sprintf("http://www.kegg.jp/pathway/%s/%s", map_id, sapply(idset, set -> encode_colors(__parseIdvector(set))));
    }

    encode_url;
}