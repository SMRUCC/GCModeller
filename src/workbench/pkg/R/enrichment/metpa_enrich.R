#' The GCModeller GSEA toolkit
imports "GSEA" from "gseakit";

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
#' @return A dataframe object that contains the enrichment analysis result,
#'    value nothing will be return if the given kegg idset is empty. 
#'
const metpa_enrich = function(data, metpa, log2FC = "log2(FC)", id = "kegg") {
    if (is.data.frame(data)) {
        metpa_enrich_dataframe(data, metpa, log2FC, id);
    } else {
        # is a character vector of the id set,
        # just do idset enrichment analysis
        metpa_enrich_ids(data, metpa);
    }
}

const metpa_enrich_ids = function(data, metpa) {
    metpa |> GSEA::enrichment(
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

const metpa_enrich_dataframe = function(data, metpa, log2FC = "log2(FC)", id = "kegg") {
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
}