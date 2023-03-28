#' The GCModeller GSEA toolkit
imports "GSEA" from "gseakit";

const _unique_idset = function(id) {
    id = id[id != ""];
    id = id[id != "NULL"];
    id = id[id != "NA"];
    
    unique(id);
}

#' Do enrichment based on metpa background model
#' 
#' @param data a given set of the target for do enrichment analysis,
#'    this parameter value could be a character vector which is stands
#'    for the kegg id character vector or a dataframe object that 
#'    contains the data annotation fields and the log2fc value.
#' @param metpa the metabolic network pathway model for used as the 
#'    enrichment background data to run enrichment analysis. this 
#'    parameter value should be a CLR object that read from file.
#' @param log2FC the field name for get the log2FC value, the log2FC
#'    value will be used for do colorful url encoded. And this parameter
#'    option only works when the input data is a dataframe object.
#' @param id the field name for get the target idset data for run the
#'    enrichment analysis. And this parameter option only works when
#'    the input data is a dataframe object.
#' 
#' @return A dataframe object that contains the enrichment analysis result 
#'
const metpa_enrich = function(data, metpa, log2FC = "log2(FC)", id = "kegg") {
    if (is.data.frame(data)) {
        # check field is exists in the given dataset or not
        if (id in data) {
            metpa |> GSEA::enrichment(
                geneSet      = _unique_idset(data[, id]),
                cut.size     = 3,
                outputAll    = FALSE,
                resize       = -1,
                showProgress = FALSE
            )
            |> as.data.frame()
            |> .url_encode(data, log2FC, id)
            ;
        } else {
            stop(`the required id set field(${id}) is not exists in the given dataset!`);
        }
    } else {
        # is a character vector of the id set,
        # just do idset enrichment analysis
        metpa 
        |> GSEA::enrichment(
            geneSet      = data,
            cut.size     = 3,
            outputAll    = FALSE,
            resize       = -1,
            showProgress = FALSE
        )
        |> as.data.frame()
        |> .url_encode()
        ;
    }
}

const .url_encode = function(enrich, data = NULL, log2FC = "log2(FC)", id = "kegg") {
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
        enrich[, "links"] = sprintf("http://www.kegg.jp/pathway/%s+%s", rownames(enrich), sapply(enrich$geneIDs, set -> paste(__parseIdvector(set), "+")));
    }

    enrich;
}

const __parseIdvector = function(set) {
    unlist(strsplit(set, ";\s+"));
}

#' generate color map for do colorful encode of the kegg map url
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
        |> paste("/")
        ;
    }
    const encode_url = function(map_id, idset) {
        sprintf("http://www.kegg.jp/pathway/%s/%s", map_id, sapply(idset, set -> encode_colors(__parseIdvector(set))));
    }

    encode_url;
}