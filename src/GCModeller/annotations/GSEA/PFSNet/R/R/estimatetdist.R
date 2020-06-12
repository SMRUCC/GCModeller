
pfsnet.estimatetdist<-function(obj,d1,d2,ggi,b,t1,t2,n){
    #.jinit("./inst/java/filter.jar")
    #obj<-.jnew("filter")
    # d1<-loaddata(file1)
    # d2<-loaddata(file2)
    total<-cbind(d1,d2)
    tdistribution<-c()
    for(i in 1:n){
        cat(".")
        samplevector<-sample(ncol(total),ncol(d1))
        expr1o<-total[,samplevector]
        expr2o<-total[,-samplevector]
        w1matrix1<-computew1(expr1o,theta1=t1,theta2=t2)
        w1matrix2<-computew1(expr2o,theta1=t1,theta2=t2)
        rownames(w1matrix1)<-rownames(expr1o)
        rownames(w1matrix2)<-rownames(expr2o)
        genelist1<-pfsnet.computegenelist(w1matrix1,beta=b)
        genelist2<-pfsnet.computegenelist(w1matrix2,beta=b)

        #ggi_mask<-.jcall(obj,"[Z","filter",.jarray(ggi[,2]),.jarray(ggi[,3]),.jarray(genelist1$gl))
        ggi_mask <- apply(ggi, 1, func <- function(i){
            if ((i[2] %in% genelist1$gl) && (i[3] %in% genelist1$gl))
                TRUE
            else FALSE
        })
        masked.ggi <- ggi[ggi_mask, ]
        colnames(masked.ggi) <- c("pathway", "g1", "g2")

        masked.ggi <- masked.ggi[(masked.ggi[, "g1"] != masked.ggi[, "g2"]), ]

        ccs <- sapply(unique(masked.ggi[, "pathway"]), bypathway <- function(pathwayid) {
            g <- graph.data.frame(masked.ggi[ masked.ggi[,"pathway"]==pathwayid, c("g1", "g2", "pathway")],directed=FALSE)
            for(i in 1:length(V(g))){

                V(g)[V(g)$name[i]]$weight<-sum(w1matrix1[V(g)$name[[i]],])/sum(!is.na(w1matrix1[V(g)$name[[i]],]))
                V(g)[V(g)$name[i]]$weight2<-sum(w1matrix2[V(g)$name[[i]],])/sum(!is.na(w1matrix2[V(g)$name[[i]],]))
            }
            g<-simplify(g)
            decompose.graph(g,min.vertices=5)
        })

        ccs <- unlist(ccs, recursive=FALSE)

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

        statistics<-rep(NA,length(ccs))
        for(i in 1:length(ccs)){
            try(statistics[i]<-t.test(unlist(pscore[,i]),unlist(nscore[,i]),paired=TRUE)$statistic, TRUE)
        }
        tdistribution<-c(tdistribution,na.omit(statistics))
        
		cat(round(length(tdistribution)/100)*100, " permutations done.\n")
        
		if(length(tdistribution)>1000){
            break;
        }
    }
    unlist(tdistribution)
}
