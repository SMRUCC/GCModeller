\name{postpsi}
\alias{postpsi}
%- Also NEED an '\alias' for EACH other topic documented here.
\title{ Posterior distribution of the spatial interaction parameter}
\description{Posterior mode of psi from the MCMC outputs}

\usage{
postpsi(path.mcmc, plot=TRUE, print=FALSE, file=path.mcmc)
}

\arguments{
    \item{path.mcmc}{ Path to the MCMC program output files }
    \item{plot}{ Logical: if TRUE an histogram is plotted.}
    \item{print}{ Logical: if TRUE the histogram is also printed.}
    \item{file}{Character: Path to file where the histogram should be printed}
}

\value{  \item{Postmode.psi}{The posterior mode of psi} }


\author{Sophie Ancelet}


\keyword{}

\examples{

# library(Geneclust)

## see the example described in the function mcmcgeneclust
}