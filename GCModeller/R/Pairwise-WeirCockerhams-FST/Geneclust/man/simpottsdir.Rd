\name{simpottsidr}
\alias{simpottsdir}
%- Also NEED an '\alias' for EACH other topic documented here.
\title{  Simulation of data from the Potts-Dirichlet model on an irregular
lattice }
\description{
Simulates some spatially organised populations according to the Potts-Dirichlet
model. Plots a map giving each individual population membership.}
\usage{
simpottsdir(path, coordinates, matngh=NULL, npop, psi, nchain = 50000,
burnin=40000, stepw = 10, plot=TRUE, ploth=FALSE, print=FALSE, file=path)
}
%- maybe also 'usage' for other objects documented here.
\arguments{
  \item{path}{ path to output files directory }
  \item{coordinates}{Spatial coordinates of individuals. A matrix with 2 columns and one line per individual   }
  \item{matngh}{ A neighbourhood matrix with nindiv lines and nindiv
  columns. If matngh=NULL, neighbourhood relationships are defined by a
  graph of Delaunay.}
  \item{npop}{ Number of populations }
  \item{psi}{ The real value of the hidden markov random field spatial interaction parameter. If
    psi=0, populations aren't spatially structured }
  \item{nchain}{ Number of MCMC iterations (Gibbs steps) to generate a
    Potts-Dirichlet configuration}
  \item{burnin}{ Number of Gibbs steps to throw away. Results are stored in ascii files from \code{burnin} and only each \code{thinning} cycles.}
  \item{stepw}{ Number of MCMC iterations between two writing steps (if \code{stepw}=1, all
    states are saved whereas if e.g. \code{stepw}=10 only each 10 iterations is saved) }
  \item{plot}{ if plot=TRUE, the map giving the population membership
  of each individual is plotted}
  \item{ploth}{ if ploth=TRUE, other charts concerning the MCMC simulation
  are plotted}
 \item{print}{ Logical: if print=TRUE the map is also printed.}
 \item{file}{Character: Path to file where the map should be printed}  
}

\value{
 A list of variables involved in the simulation. The elements of this
 list are: matngh,popmbrship.
}

\section{Storage format}{
All parameters processed by function \code{\link{simpottsdir}} are
written in the directory specified by \file{path} as follows:

File \file{simpotts.txt} contains MCMC updates of each
individual population membership


File \file{energy.txt} contains the Potts-Dirichlet system energy for each
MCMC update of the population memberships.

}


\keyword{}

\author{Sophie Ancelet}

\examples{

#library(Geneclust)

\dontrun{

# Below is a sequence of R commands using geneclust functions

# Simulation of a dataset according to Potts-Dirichlet model
# The dataset is made of 2 populations
# on a spatial domain enclosed in a rectangle
# (x.coord. in [0,1], y.coord. in [0,1])
# Spatial interaction parameter is 0.4


#To define a place for simulation outputs
system("mkdir ./tmpData/")
path <- "./tmpData/"


#To generate the coordinates of 100 individuals which are supposed
#  uniformly distributed in a rectangle  

coordinates<- matrix(runif(200,0,1),nrow=100,ncol=2)

data<- simpottsdir(path=path,
                   coordinates=coordinates,
                   matngh=NULL,
                   npop=2,
                   psi=0.4,
                   nchain=50000,
                   burnin=40000,
                   stepw=100,
                   plot=TRUE,
                   ploth=FALSE,
                   print=FALSE,
                   file=path)

## go to file path to see simulation outputs
}}
