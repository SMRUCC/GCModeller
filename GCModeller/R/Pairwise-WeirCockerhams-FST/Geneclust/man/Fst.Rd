\name{Fst}
\alias{Fst}
%- Also NEED an '\alias' for EACH other topic documented here.
\title{F statistics}
\description{Computes F statistics according to Weir and Cockerham's estimators.}

\usage{
Fst(genotypes, allele.numbers, pop.mbrship, npopmax)
}
%- maybe also 'usage' for other objects documented here.
\arguments{
   \item{genotypes}{Genotypes of individuals. A matrix with one line per
    individual and 2 columns per locus}
  \item{allele.numbers}{A vector of integer that containis the number of
    alleles at each locus}
  \item{pop.mbrship}{A vector of integer that contains the posterior cluster labels (assignments)}
  \item{npopmax}{A likely maximal number of clusters in the population}
}


\value{A list with components 
  \item{Total.Fit }{A real number estimating the total Fit}
  \item{Pairwise.Fis }{A matrix of real numbers estimating the pairwise Fis}
  \item{Pairwise.Fst }{A matrix of real numbers estimating the pairwise Fst}
  \item{Pairwise.Fit }{A matrix of real numbers estimating the pairwise Fit}
}

\author{Arnaud Estoup for original code in Turbo Pascal. Translation in Fortran and interface with R by Gilles Guillot}

\references{ Weir, B.S. and C.C. Cockerham, 
  Estimating F-statistics for the analysis of population structure,
  Evolution, 1984,
  vol. 38, 1358-1370.
}

\keyword{}

\examples{
## see also the example described in the function mcmcgeneclust

\dontrun{
data(bear)

hc <- hgclust(bear, K=4)
plot(hc)

n.all <- c(3,3,5,6,6,6,6,7,7,7,7,7,8,8,8,8,10,10,10)

Fst(as.matrix(bear)[,3:40], n.all, hc$labels, npopmax=4)$Pairwise.Fst


}


}

