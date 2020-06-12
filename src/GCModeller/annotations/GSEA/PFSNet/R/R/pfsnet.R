pfsnet<-function(file1,file2,file3,b=0.5,t1=0.95,t2=0.85,n=1000){
    #obj<-.jnew("filter")
    ptm<-proc.time()
    cat("reading data files")
    ggi<-read.table(file3, header=FALSE, sep="\t", colClasses="character")
    cat(".")
    expr1o<-loaddata(file1)
    cat(".")
    expr2o<-loaddata(file2)
    cat(".")
    cat("\t[DONE]\n")
    cat("computing fuzzy weights")
    w1matrix1<-computew1(expr1o,theta1=t1,theta2=t2)
    w1matrix2<-computew1(expr2o,theta1=t1,theta2=t2)
    cat(".")
    rownames(w1matrix1)<-rownames(expr1o)
    rownames(w1matrix2)<-rownames(expr2o)
    cat(".")
    genelist1<-pfsnet.computegenelist(w1matrix1,beta=b)
    genelist2<-pfsnet.computegenelist(w1matrix2,beta=b)
    cat(".")
    cat("\t[DONE]\n")
    cat("computing subnetworks")
    #ggi_mask<-.jcall(obj,"[Z","filter",.jarray(ggi[,2]),.jarray(ggi[,3]),.jarray(genelist1$gl))
    cat(".")
    ggi_mask <- apply(ggi, 1, func <- function(i){
        if ((i[2] %in% genelist1$gl) && (i[3] %in% genelist1$gl))
            TRUE
        else FALSE
    })  # ??? ggi_mask<-.jcall(obj,"[Z","filter",.jarray(ggi[,2]),.jarray(ggi[,3]),.jarray(genelist1$gl))

    masked.ggi <- ggi[ggi_mask, ]
    colnames(masked.ggi) <- c("pathway", "g1", "g2")

    masked.ggi <- masked.ggi[(masked.ggi[, "g1"] != masked.ggi[, "g2"]), ]
    cat(".")
    ccs <- sapply(unique(masked.ggi[, "pathway"]), bypathway <- function(pathwayid) {
        g <- graph.data.frame(masked.ggi[ masked.ggi[,"pathway"]==pathwayid, c("g1", "g2", "pathway")],directed=FALSE)
        for(i in 1:length(V(g))){

            V(g)[V(g)$name[i]]$weight<-sum(w1matrix1[V(g)$name[[i]],])/sum(!is.na(w1matrix1[V(g)$name[[1]],]))
            V(g)[V(g)$name[i]]$weight2<-sum(w1matrix2[V(g)$name[[i]],])/sum(!is.na(w1matrix2[V(g)$name[[1]],]))
        }
        g<-simplify(g)
        decompose.graph(g,min.vertices=5)
    })

    ccs <- unlist(ccs, recursive=FALSE)

    # ??? why java?
    #ggi_mask<-.jcall(obj,"[Z","filter",.jarray(ggi[,2]),.jarray(ggi[,3]),.jarray(genelist2$gl))
    ggi_mask <- apply(ggi, 1, func <- function(i){
        if ((i[2] %in% genelist2$gl) && (i[3] %in% genelist2$gl))
            TRUE
        else FALSE
    })
    masked.ggi <- ggi[ggi_mask, ]
    colnames(masked.ggi) <- c("pathway", "g1", "g2")

    masked.ggi <- masked.ggi[(masked.ggi[, "g1"] != masked.ggi[, "g2"]), ]

    ccs2 <- sapply(unique(masked.ggi[, "pathway"]), bypathway <- function(pathwayid) {
        g <- graph.data.frame(masked.ggi[ masked.ggi[,"pathway"]==pathwayid, c("g1", "g2", "pathway")],directed=FALSE)
        for(i in 1:length(V(g))){

            V(g)[V(g)$name[i]]$weight<-sum(w1matrix1[V(g)$name[[i]],])/sum(!is.na(w1matrix1[V(g)$name[[1]],]))
            V(g)[V(g)$name[i]]$weight2<-sum(w1matrix2[V(g)$name[[i]],])/sum(!is.na(w1matrix2[V(g)$name[[1]],]))
        }
        g<-simplify(g)
        decompose.graph(g,min.vertices=5)
    })

    ccs2 <- unlist(ccs2, recursive=FALSE)
    cat(".")
    cat("\t[DONE]\n")
    cat("computing subnetwork scores")
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
        #apply(ss,2,function(y){
        #		sum(apply(ss[y,,drop=FALSE],1,sum)/ncol(expr1o))
        #	})

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
        #apply(ss,2,function(y){
        #		sum(apply(ss[y,,drop=FALSE],1,sum)/ncol(expr1o))
        #	})

    })
    cat(".")
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
        #apply(ss,2,function(y){
        #		sum(apply(ss[y,,drop=FALSE],1,sum)/ncol(expr2o))
        #	})
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
        #apply(ss,2,function(y){
        #		sum(apply(ss[y,,drop=FALSE],1,sum)/ncol(expr2o))
        #	})
    })
    cat(".")
    cat(".\t[DONE]\n")
    cat("computing permuation tests")
    tdist<-pfsnet.estimatetdist(obj,expr1o,expr2o,ggi,b,t1,t2,n)
    tdist2<-pfsnet.estimatetdist(obj,expr2o,expr1o,ggi,b,t1,t2,n)
    #statistics<-rep(NA,length(ccs))
    #statistics2<-rep(NA,length(ccs))
    #pval<-rep(NA,length(ccs))
    #pval2<-rep(NA,length(ccs2))
    ccs.mask<-rep(FALSE,length(ccs))
    ccs2.mask<-rep(FALSE,length(ccs2))
    for(i in 1:length(ccs)){
        #try(statistics[i]<-t.test(unlist(pscore[,i]),unlist(nscore[,i]),paired=TRUE)$statistic, TRUE)
        #try(pval[i]<-sum(abs(tdist)>abs(statistics[i]))/length(tdist))
        try(ccs[[i]]$statistics<-t.test(unlist(pscore[,i]),unlist(nscore[,i]),paired=TRUE)$statistic, TRUE)
        try(ccs[[i]]$p.value<-sum(abs(tdist)>abs(ccs[[i]]$statistics))/length(tdist),TRUE)
        try(if(ccs[[i]]$p.value<0.05){ccs.mask[i]<-TRUE},TRUE)
    }
    for(i in 1:length(ccs2)){
        #try(statistics2[i]<-t.test(unlist(pscore2[,i]),unlist(nscore2[,i]),paired=TRUE)$statistic, TRUE)
        #try(pval2[i]<-sum(abs(tdist)>abs(statistics2[i]))/length(tdist))
        try(ccs2[[i]]$statistics<-t.test(unlist(pscore2[,i]),unlist(nscore2[,i]),paired=TRUE)$statistic, TRUE)
        try(ccs2[[i]]$p.value<-sum(abs(tdist2)>abs(ccs2[[i]]$statistics))/length(tdist2),TRUE)
        try(if(ccs2[[i]]$p.value<0.05){ccs2.mask[i]<-TRUE},TRUE)
    }
    cat("\t[DONE]\n")
    cat("total time elapsed: ",(proc.time()-ptm)[3], "seconds\n")

    list(class1=list(subnets=ccs[ccs.mask]),class2=list(subnets=ccs2[ccs2.mask]))
}
