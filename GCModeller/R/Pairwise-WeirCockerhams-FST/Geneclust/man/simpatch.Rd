\name{simpatch}
\alias{simpatch}
%- Also NEED an '\alias' for EACH other topic documented here.
\title{  Simulate geo-referenced multilocus data sets}
\description{Simulates multilocus genotypes and group memberships according to the prior distributions used in the Bayesian algorithm.}
\usage{
simpatch(path, nindiv, coordinates = NULL, coord.lim = c(0,1,0,1),
npop, nall, psi, fis, nchain = 50000, burnin=40000, stepw = 100, seed = NULL, plot
= TRUE, write=FALSE, print=FALSE, file=path)
}
%- maybe also 'usage' for other objects documented here.
\arguments{
  \item{path}{ path to the MCMC program output files }
  \item{nindiv}{ Number of individuals }
  \item{coordinates}{ Spatial coordinates of individuals. A matrix with 2 columns and one line per individual}
  \item{coord.lim}{ Vector: Ranges of the spatial domain to be considered (xmin,xmax,ymin,ymax)}
  \item{npop}{ Number of populations }
  \item{nall}{ Vector of integers giving the number of alleles at each locus}
  \item{psi}{ A nonnegative spatial interaction parameter. If psi=0, populations are not spatially structured. Typical values are between 0 and 1.}
  \item{fis}{ Vector of population inbreeding coefficients. If fis[i]=0, there is no inbreeding in population i}
  \item{nchain}{ Number of MCMC iterations (Gibbs steps) to generate a
    Potts-Dirichlet configuration }
  \item{burnin}{ Number of Gibbs steps to throw away. Results are stored in ascii files from \code{burnin} and only each \code{thinning} cycles.}
  \item{stepw}{ Number of MCMC iterations between two writting steps (if \code{stepw}=1, all
    states are saved whereas if e.g. \code{stepw}=10 only each 10 iterations is saved) }
  \item{seed}{ Logical: Seed to initialize the random number generator }
  \item{plot}{ Logical: if TRUE, the map giving the population membership
  of each individual and the barplots for allele frequencies are plotted}
  \item{write}{Logical: if TRUE, coordinates, allele frequencies,
  genotypes, population memberships, matngh, number of alleles and
  other variables involved in the simulation are also written in plain
  ascci files}
  \item{print}{ Logical: if TRUE the map and the barplots for allele
  frequencies are also printed }
  \item{file}{ Character: Path to file where figures should be
  printed and/or variables involved in the simulation should be written }   
}


\section{Storage format}{
All parameters processed by function \code{\link{simpatch}} are
written in the directory specified by \file{path} as follows:

File \file{simpotts.txt} contains MCMC updates of each
individual population membership

File \file{energy.txt} contains the Potts system energy for each
MCMC update of the population memberships

}

\value{
 A list of variables involved in the simulation. The elements of this
 list are: coordinates,matngh,popmbrship,genotypes,frequencies.
}

\seealso{}

\author{ Sophie Ancelet }

\keyword{}

\examples{

# library(Geneclust)

\dontrun{


# Simulation of a dataset made of 2 populations
# 10 loci and 10 alleles per loci
# on a spatial domain enclosed in a rectangle
# (x.coord. in [0,1], y.coord. in [0,1])
# Spatial interaction parameter is 0.5
# We suppose the inbreeding coefficients are the same in each population
# that is to say 0.1

#  define a place for simulation outputs
system("mkdir ./tmpData/")
path <- "./tmpData/"

data<- simpatch(path=path,
                nindiv=100,
                coordinates=NULL,
                coord.lim=c(0,1,0,1),
                npop=2,
                nall=rep(10,10),
                psi=0.5,
                fis=c(0.1,0.1),
                nchain=50000,
                burnin=40000,
                stepw=100,
                seed=123,
                plot=TRUE,
                write=FALSE,
                print=FALSE,
                file=path)

## go to file path to read simulation outputs
}}




