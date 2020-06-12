
pfsnet.consistencytest2<-function(ccs,file1,file2,b,t1,t2){
    expr1o<-loaddata(file1)
    expr2o<-loaddata(file2)
    w1matrix1<-computew1(expr1o,theta1=t1,theta2=t2)
    w1matrix2<-computew1(expr2o,theta1=t1,theta2=t2)
    rownames(w1matrix1)<-rownames(expr1o)
    rownames(w1matrix2)<-rownames(expr2o)
    genelist1<-pfsnet.computegenelist(w1matrix1,beta=b)
    genelist2<-pfsnet.computegenelist(w1matrix2,beta=b)
    pscore<-sapply(ccs,function(x){
        vertices<-get.vertex.attribute(x,name="name")
        vertex_mask<-sapply(vertices,function(x){
            if(x %in% rownames(w1matrix1))
                TRUE
            else
                FALSE
        })
        vertices<-vertices[vertex_mask]
        ws<-w1matrix1[vertices,,drop=FALSE]
        ws[is.na(ws)]<-0
        v<-c()
        for(i in 1:ncol(ws)){
            v<-c(v,sum(
                (V(x)[vertex_mask]$weight*ws[,i])
                # V(x)[sapply(V(x)$name,function(z){
                # 	if(!(z %in% rownames(ss))){
                # 		FALSE
                # 	}else if(ss[z,i]){
                # 		TRUE
                # 	}else{
                # 		FALSE
                # 	}
                # })]$weight
            ))
        }
        v
        #apply(ss,2,function(y){
        #		sum(apply(ss[y,,drop=FALSE],1,sum)/ncol(expr1o))
        #	})

    })

    nscore<-sapply(ccs,function(x){
        vertices<-get.vertex.attribute(x,name="name")
        vertex_mask<-sapply(vertices,function(x){
            if(x %in% rownames(w1matrix2))
                TRUE
            else
                FALSE
        })
        vertices<-vertices[vertex_mask]
        ws<-w1matrix1[vertices,,drop=FALSE]
        ws[is.na(ws)]<-0
        v<-c()
        for(i in 1:ncol(ws)){
            v<-c(v,sum(
                (V(x)[vertex_mask]$weight2*ws[,i])
                # V(x)[sapply(V(x)$name,function(z){
                # 	if(!(z %in% rownames(ss))){
                # 		FALSE
                # 	}else if(ss[z,i]){
                # 		TRUE
                # 	}else{
                # 		FALSE
                # 	}
                # })]$weight
            ))
        }
        v
        #apply(ss,2,function(y){
        #		sum(apply(ss[y,,drop=FALSE],1,sum)/ncol(expr2o))
        #	})
    })
    pval<-rep(NA,length(ccs))
    pval2<-rep(NA,length(ccs))
    pval3<-rep(NA,length(ccs))
    statistics<-rep(NA,length(ccs))

    for(i in 1:length(ccs)){
        try(pval[i]<-pfsnet.computepval(pscore[,i],nscore[,i],1000),TRUE)
        try(pval2[i]<-t.test(pscore[,i],nscore[,i],paired=TRUE)$p.value,TRUE)
        try(pval3[i]<-t.test(pscore[,i],nscore[,i])$p.value,TRUE)
        try(statistics[i]<-t.test(pscore[,i],nscore[,i],paired=TRUE)$statistic,TRUE)
    }
    list(pval=pval,pval2=pval2,pval3=pval3,pscore=pscore,nscore=nscore,statistics=statistics)
    # statistics<-list()
    # for(i in 1:length(ccs)){ try(statistics[[i]]<-t.test(unlist(pscore[,i]),unlist(nscore[,i]),paired=TRUE)$statistic, TRUE) }
    # for(i in 1:length(statistics)){ if(length(statistics)==0){break} else if(is.null(statistics[[i]])) statistics[[i]]<-NA }
    # statistics
}
