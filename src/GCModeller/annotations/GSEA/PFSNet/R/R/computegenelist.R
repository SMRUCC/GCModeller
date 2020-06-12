#' compute gene list
#'
#' @description compute the gene list which its 
#' members expression level is greater than \code{beta}.
#'
#' @return a list of gene id which theirs expression is 
#' greater than threshold \code{beta}.
#'
pfsnet.computegenelist<-function(w,beta){
    # within.mask<-apply(w,1,function(x){
    # 	m<-median(x)
    # 	(x>m-delta)
    # })
    # within.mask<-t(within.mask)
    list.mask<-apply(w,1,function(x){
        sum(x,na.rm=T)/sum(!is.na(x)) >= beta
    })
    list(gl=rownames(w)[list.mask])
}