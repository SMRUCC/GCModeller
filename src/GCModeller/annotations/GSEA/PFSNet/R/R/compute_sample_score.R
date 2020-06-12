pfsnet.compute.sample.score<-function(ccs,ccs2,file1,file2,t1=0.95,t2=0.85){
    expr1o<-loaddata(file1)
    expr2o<-loaddata(file2)
    w1matrix1<-computew1(expr1o,theta1=t1,theta2=t2)
    w1matrix2<-computew1(expr2o,theta1=t1,theta2=t2)
    rownames(w1matrix1)<-rownames(expr1o)
    rownames(w1matrix2)<-rownames(expr2o)
    pscore<-sapply(ccs,function(x){
        vertices<-get.vertex.attribute(x,name="name")
        ws<-w1matrix1[vertices,,drop=FALSE]
        ws[is.na(ws)]<-0
        v<-c()
        for(i in 1:ncol(ws)){
            v<-c(v,sum(
                (V(x)$weight*ws[,i])
            ))
        }
        v
    })
    nscore<-sapply(ccs,function(x){
        vertices<-get.vertex.attribute(x,name="name")
        ws<-w1matrix1[vertices,,drop=FALSE]
        ws[is.na(ws)]<-0
        v<-c()
        for(i in 1:ncol(ws)){
            v<-c(v,sum(
                (V(x)$weight2*ws[,i])
            ))
        }
        v
    })
    pscore2<-sapply(ccs2,function(x){
        vertices<-get.vertex.attribute(x,name="name")
        ws<-w1matrix2[vertices,,drop=FALSE]
        ws[is.na(ws)]<-0
        v<-c()
        for(i in 1:ncol(ws)){
            v<-c(v,sum(
                (V(x)$weight*ws[,i])
            ))
        }
        v
    })
    nscore2<-sapply(ccs2,function(x){
        vertices<-get.vertex.attribute(x,name="name")
        ws<-w1matrix2[vertices,,drop=FALSE]
        ws[is.na(ws)]<-0
        v<-c()
        for(i in 1:ncol(ws)){
            v<-c(v,sum(
                (V(x)$weight2*ws[,i])
            ))
        }
        v
    })
    list(pscore=as.data.frame(pscore),nscore=as.data.frame(nscore),pscore2=as.data.frame(pscore2),nscore2=as.data.frame(nscore2))

}

