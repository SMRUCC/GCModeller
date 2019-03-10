\name{subsample}
\alias{subsample}
%- Also NEED an '\alias' for EACH other topic documented here.
\title{Make a subsample from an initial dataset}
\description{
 This function enables to make a subsample from an initial dataset by
 reducing the number of loci to consider.
}
\usage{
subsample(data, nloci, lst = NULL)
}
%- maybe also 'usage' for other objects documented here.
\arguments{
  \item{data}{ An object of class \code{geneclustdata} }
  \item{nloci}{ Number of loci to consider to make the new dataset}
  \item{lst}{ The indices of loci chosen to make the new dataset. If
    lst=NULL, these indices are chosen randomly}
}

\value{
  \item{new.dat }{The new dataset which is again an object of class
    \code{geneclustdata}. Genetics information are reduced to "nloci"
    number of loci.}

}

\author{ Olivier Francois }

\keyword{}


\examples{

#library(Geneclust)

\dontrun{

# Simulation of a dataset according to the prior distributions. This one
# is made of 2 populations
# 10 loci and 10 alleles per loci
# on a spatial domain enclosed in a rectangle
# (x.coord. in [0,1], y.coord. in [0,1])
# Spatial interaction parameter is 0.5
# We suppose the inbreeding coefficients are the same in each population
# and equal to 0.1

#define a place for simulation outputs
system("mkdir ./tmpData/")
path <- "./tmpData/"

data<-simpatch(path=path,nindiv=100,npop=2,nall=rep(10,10),psi=0.5,fis=c(0.1,0.1),nchain=20000,burnin=10000,stepw=10,plot=FALSE)

datagc<- as.geneclustdata(data$coordinates[,1],data$coordinates[,2],data$genotypes)


#Subsample with 5 randomly chosen loci 
geneclustobjsub<- subsample(data=datagc,nloci=5,lst=NULL)
}

}