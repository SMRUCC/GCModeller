\name{postclassif}
\alias{postclassif}
%- Also NEED an '\alias' for EACH other topic documented here.
\title{  Posterior membership probabilities and cluster assignment}
\description{
  Post-processing of MCMC outputs computing individual membership coefficients and the most likely cluster assignment.
}
\usage{
postclassif(path.mcmc, coordinates, popmbrship=NULL, plot=TRUE, print=FALSE, file=path.mcmc, write=FALSE)
}
%- maybe also 'usage' for other objects documented here.
\arguments{
  \item{path.mcmc}{  Path to the MCMC program output files directory }  
  \item{coordinates}{ Spatial coordinates of individuals. A matrix with 2
    columns and one line per individual }
  \item{popmbrship}{ A vector with number of individuals components
  containing the true population memberships (if they are known!). If
  popmbrship=NULL, the population of origin of each individuals is unknown. }
  \item{plot}{ Logical: if TRUE, maps are plotted}
  \item{print}{ Logical: if TRUE, maps are also printed.}
  \item{file}{ Character : Path to file where maps should be printed}
  \item{write}{ Logical: If TRUE, data are written in ascci files named
  "postmode.indiv.txt" and "proplab.txt" respectively}
}


\value{ A list with components: 
  \item{postmode.indiv}{ A vector that contains the cluster assignment of each
    individual}
  \item{effpop.est}{ A vector which contains the number of individuals
    in each population defined}
  \item{proplab}{ Matrix of posterior membership probabilities for each individuals}
  \item{K.est}{ The number of populations defined from the
    cluster assignment of each individual}
  \item{errclassif}{ A rate of misclassification computed if the true population memberships are given as argument}  
  \item{K.distrib}{ The posterior distribution of K (number of populations)}
  \item{modeK}{ The mode of the posterior distribution of K (number of populations)}

}

\author{ Sophie Ancelet }

\keyword{}

\examples{

# library(Geneclust)

## see the example described in the function mcmcgeneclust
}

