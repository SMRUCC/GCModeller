\name{postfis}
\alias{postfis}
%- Also NEED an '\alias' for EACH other topic documented here.
\title{Posterior distribution of inbreeding coefficients}
\description{
   Computes the posterior mean and some posterior quantiles for inbreeding coefficients. 
}
\usage{
postfis(path.mcmc, postmode.indiv, probs = c(0.025, 0.25, 0.5, 0.75, 0.975), plot = TRUE, print = FALSE, file=path.mcmc)
}
%- maybe also 'usage' for other objects documented here.
\arguments{
  \item{path.mcmc}{ Path to MCMC program output files }
  \item{postmode.indiv}{ A vector which contains the population of origin of each individual}
  \item{probs}{ Numeric vector of probabilities with values in [0,1]. By
  default,probs=c(0.025,0.25,0.50,0.75,0.975) in order to compute credibility intervals. }
  \item{plot}{ Logical: if TRUE histograms are plotted }
  \item{print}{ Logical: if TRUE histograms are also printed }
  \item{file}{ Character: Path to file where figures should be printed }
}

\value{ 
  \item{postmean.fis }{A numerical vector which contains the posterior mean of inbreeding coefficient in each identified cluster}
  \item{quant.fis }{A matrix that stores the posterior distribution quantiles
  of each inbreeding coefficient. Each line corresponds to one cluster}
}


\author{ Sophie Ancelet }

\keyword{}

\examples{

# library(Geneclust)

## see the example described in the function mcmcgeneclust
}
