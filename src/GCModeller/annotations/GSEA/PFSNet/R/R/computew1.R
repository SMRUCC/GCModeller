
#' compute the gene expression weight matrix
#'
computew1<-function(expr,theta1,theta2){
    ranks<-apply(expr,2,function(x){
        rank(x)/length(x)
    })
    apply(ranks,2,function(x){
        q2<-quantile(x,theta2,na.rm=T)
        q1<-quantile(x,theta1,na.rm=T)
        m<-median(x)
        mx<-max(x)
        sapply(x,function(y){
            if(is.na(y)){
                NA
            }
            else if(y>=q1)
                1
            else if(y>=q2)
                (y-q2)/(q1-q2)
            else
                0
        })
    })
}
