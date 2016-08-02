\name{FormatGenotypes}
\alias{FormatGenotypes}
%- Also NEED an '\alias' for EACH other topic documented here.
\title{Formatting file of genotype data.}
\description{Takes genotype data as a matrix with
  one line per individual and two columns per locus,
  with alleles coded by integers (number of replications for
  micro-satellites data).
  Build a new matrix with alleles codes as consecutive integers.
  If a locus  has 7 alleles they will be coded as 1,2,...7.
  Since version 1.0.1, this function does not have to be called by
  users.
}
\usage{FormatGenotypes(genotypes)}
%- maybe also 'usage' for other objects documented here.
\arguments{
  \item{genotypes}{A matrix with
  one line per individual and two columns per locus,
  with alleles coded by integers}
}

\value{
  A list with elements: 
  \item{genotypes}{a  matrix with one line per individual and
    two columns per locus with alleles coded by integers}
  
  \item{allele.numbers}{a vector giving the number of possible alleles per locus}
}

\seealso{ }

\author{Gilles Guillot}


\keyword{}

\examples{
# library(Geneclust)
}