require(gtools)


## NB
##------------------------------
## count matrix x should be samples on the rows and OTUs on the colums,
## assuming dim(x) -> samples by OTUs

sparcc <- function(x, max.iter=20, th=0.1, exiter=10){
  xdim <- dim(x)
  Vlist <- matrix(NA,nrow=xdim[2],ncol=max.iter)
  Corlist <- array(,dim=c(max.iter, xdim[2], xdim[2]))
  Covlist <- array(,dim=c(max.iter, xdim[2], xdim[2]))

  ## Cycle max.iter times for variability in variance estimation
  for (i in 1:max.iter){
    cat("Iteration: %d\n",i)
    tmpres <- compute.corr(x, iter=exiter, th=th)
    Vlist[,i] <- tmpres[["Vbase"]]
    Corlist[i,,] <- tmpres[["Cor.mat"]]
    Covlist[i,,] <- tmpres[["Cov.mat"]]
  }

  ## Compute variance basis and correlation
  vdef <- apply(Vlist,1,median)
  cor.def <- apply(Corlist,2:3,median)

  ## Square root variances
  vdefsq <- vdef**0.5

  ## Compute covariance
  ttmp <- cor.def * vdefsq
  cov.def <- t(ttmp) * vdefsq
  
  ## Uncomment following lines for an alternative method
  ## x <- matrix(vdefsq,ncol=50,nrow=50, byrow=TRUE)
  ## y <- t(x)
  ## cov.def <- cor.def * x * y
  
  return(list(CORR=cor.def, COV=cov.def, VBASIS=vdef))
}


compute.corr <- function(x, iter=10, th=0.1){

  ## Compute relative fraction from dirichlet distribution
  ## NB think on different normalization for improvements
  fracs <- counts2frac(x)

  ## Compute the variation matrix
  V <- variation.mat(fracs)
  
  ## Compute the Sparcc correlation
  ##----------------------------------------

  ## Initialize matrices
  ll1 <- basis.var(fracs, V)
  ll2 <- cor.from.basis(V, ll1[["Vbase"]])
  excluded <- NULL
  
  for (i in 1:iter){
    ## Search for excluded pairs
    ## ll2[[1]] -> Cor.mat
    ll3 <- exclude.pairs(ll2[["Cor.mat"]], ll1[["M"]], th = th, excluded = excluded)
    excluded <- ll3[["excluded"]]
    if (!ll3[["flag"]]){
      ll1 <- basis.var(fracs, V, M=ll3[["M"]], excluded=excluded)
      ll2 <- cor.from.basis(V, ll1[["Vbase"]])
    }
  }
  
  return(list(Vbase=ll1[["Vbase"]], Cor.mat=ll2[["Cor.mat"]], Cov.mat=ll2[["Cov.mat"]]))
}


counts2frac <- function(x, method="dirichlet"){
  xsize <- dim(x)
  fracs <- matrix(1/xsize[2], nrow=xsize[1], ncol=xsize[2])
  if (method=="dirichlet"){
    fracs.t <- apply(x,1,function(y){rdirichlet(1,y + 1)})
    fracs <- t(fracs.t)
  }
  return(fracs)
}

variation.mat <- function(fracs){
  ## Initialize variation matrix
  V <- matrix(NA, ncol=dim(fracs)[2], nrow=dim(fracs)[2])
  ## Compute log for each OTU
  tmplog <- apply(fracs,2,log)
  idx <- combn(1:dim(fracs)[2],2)

  ## create matrix Ti,j
  ttmp <- tmplog[,idx[1,]] -   tmplog[,idx[2,]]

  ## Compute Variance
  vartmp <- apply(ttmp,2, var)
  
  ## Fill the variance matrix
  for (i in 1:length(vartmp)){
    V[idx[1,i],idx[2,i]] <- V[idx[2,i],idx[1,i]] <- vartmp[i]
  }
  diag(V) <- 1
  return(V)
}

basis.var <- function(fracs, V, Vmin=1e-4, excluded=NULL, Covmat=NULL, M=NULL){

  Vsize <- dim(V)
  Vvec <- apply(V,1,sum)

  ## Initialize Covmat matrix
  if (is.null(Covmat))
    Covmat <- matrix(0, nrow=Vsize[1], ncol=Vsize[2])

  Covvec <- apply(Covmat - diag(Covmat),1,sum)
  ## Initialize M matrix 
  if (is.null(M)){
    M <- matrix(1, nrow=Vsize[1], ncol=Vsize[2])
    diag(M) <- Vsize[1] - 1
  }
  Minv <- solve(M)
  Vbase <- Minv %*% (Vvec + 2*Covvec)
  Vbase[Vbase<0] <- Vmin
  return(list(Vbase=Vbase, M=M))
}

cor.from.basis <- function(V, Vbase){
  ## Compute the correlation from variation matrix and basis variations

  p <- dim(Vbase)[1]
  Cor.mat <- diag(rep(1,p))
  Cov.mat <- diag(Vbase[,1])
  
  idx <- combn(p,2)
  
  for (i in 1:(p-1)){
    idxslice <- idx[1,]==i
    cov.tmp <- .5 * (Vbase[i] + Vbase[idx[2,idxslice]] - V[i,idx[2,idxslice]])
    denom <- sqrt(Vbase[i]) * sqrt(Vbase[idx[2,idxslice]])
    cor.tmp <- cov.tmp / denom
    abscor <- abs(cor.tmp)
    if (any(abscor > 1)){
      idxthr <- abscor > 1
      
      ## Set the max correlation to -1,1
      cor.tmp[idxthr] <- sign(cor.tmp[idxthr])
      
      ## Compute the covariance basis
      cov.tmp[idxthr] <- cor.tmp[idxthr] * denom[idxthr] 
    }

    ## Fill the cor and cov matrix
    Cor.mat[i,idx[2,idxslice]] <- Cor.mat[idx[2,idxslice],i] <- cor.tmp
    Cov.mat[i,idx[2,idxslice]] <- Cov.mat[idx[2,idxslice],i] <- cov.tmp
  }
  return(list(Cor.mat=Cor.mat, Cov.mat=Cov.mat))
}


exclude.pairs <- function(Cor.mat, M, th=0.1, excluded=NULL){

  flag <- FALSE
  
  ## Remove autocorrelation
  cor.tmp <- abs(Cor.mat)
  diag(cor.tmp) <- diag(cor.tmp) - diag(Cor.mat)

  if (!is.null(excluded))
    cor.tmp[excluded,] <- 0

  ## Search highly correlated pairs
  mm <- max(cor.tmp)
  idxtorm <- which(cor.tmp==mm, arr.ind=TRUE)

  if (mm > th){
    ## Subtract 1 in in the M matrix where found highly correlated pairs
    for (i in 1:dim(idxtorm)[1]){
      M[idxtorm[i,1],idxtorm[i,2]] <- M[idxtorm[i,1],idxtorm[i,2]] - 1
    }

    ## Subtract one to the diagonal
    dd <- diag(M)[unique(c(idxtorm))]
    diag(M)[unique(c(idxtorm))] <- dd - 1
    excluded <- rbind(excluded, idxtorm)
  } else {
    excluded <- excluded
    flag <- TRUE
  }
  return(list(M=M, excluded=excluded, flag=flag))
}





normalize.matrix <- function(x){
  # Normalize by total count
  x <- apply(x,2,function(y){
    y/sum(y)})
  
  return(x)
}
