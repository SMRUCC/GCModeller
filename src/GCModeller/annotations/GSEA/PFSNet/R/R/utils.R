#' read data table
#'
#' @param file a tsv text file
#'
loaddata <- function(file){
    read.table(file, row.names=1, header = TRUE);
}