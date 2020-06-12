
#' compute nscore or pscore matrix
#'
compute_npscore <- function(ccs, w1matrix1, nscore = FALSE) {
	sapply(ccs,function(x){
        vertices<-get.vertex.attribute(x,name="name")
        ws<-w1matrix1[vertices,,drop=FALSE]
        ws[is.na(ws)]<-0
        v<-c();
		weight.n <- if (nscore) {
			V(x)$weight;
		} else {
			V(x)$weight2;
		}		
		
        for(i in 1:ncol(ws)){
            v<-c(v, sum((weight.n * ws[,i])));
        }
        v
    });
}

pfsnet.compute.sample.score<-function(ccs,ccs2,file1,file2,t1=0.95,t2=0.85){
    expr1o<-loaddata(file1)
    expr2o<-loaddata(file2)
    w1matrix1<-computew1(expr1o,theta1=t1,theta2=t2)
    w1matrix2<-computew1(expr2o,theta1=t1,theta2=t2)
    rownames(w1matrix1)<-rownames(expr1o)
    rownames(w1matrix2)<-rownames(expr2o)
    pscore<-compute_npscore(ccs, w1matrix1);
    nscore<-compute_npscore(ccs, w1matrix1, nscore = TRUE);
    pscore2<-compute_npscore(ccs2,w1matrix2);
    nscore2<-compute_npscore(ccs2,w1matrix2, nscore = TRUE);
	
    list(pscore=as.data.frame(pscore),nscore=as.data.frame(nscore),pscore2=as.data.frame(pscore2),nscore2=as.data.frame(nscore2))
}