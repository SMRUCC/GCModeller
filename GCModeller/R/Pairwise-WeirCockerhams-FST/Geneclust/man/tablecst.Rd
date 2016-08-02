\name{tablecst}
\alias{tablecst}

\title{ A normalization constants table to make MCMC inference on the
Potts-Dirichlet model spatial interaction parameter}

\description{Computes the Potts-Dirichlet model normalization constants table by the
method proposed by  Sylvia Richardson and Peter J.Green in the
article: "Hidden Markov Models and Disease Mapping"(JASA Dec
2002). }

                                                             
\usage{
tablecst(pathtable, npopmax, coordinates, matngh, 
stepval = 0.02, nit.table = 20000, stepw.table = 10, burnin.table =
10000, plot=TRUE, write = FALSE)
}

\arguments{
  \item{pathtable}{ Path to output file directory }
  \item{npopmax}{Initial number of populations }
  \item{coordinates}{Spatial coordinates of individuals. A matrix with 2 columns and one line per individual  }
  \item{matngh}{ The neighbourhood matrix with nindiv lines and nindiv
  columns. If matngh[i,j]=1 then the individuals i and j are neighbours.}
 \item{stepval}{Step of discretization of the interval [0,1]. The expected Potts system energy will be computed for each
 psi value of this discretized interval by MCMC simulations of Potts-Dirichlet
  configurations. By default, stepval=0.02.}
 \item{nit.table}{ Number of MCMC iterations to generate Potts-Dirichlet configurations.}   
 \item{stepw.table}{ Number of MCMC iterations between two writing steps (if \code{stepw.table}=1, all
    states are saved whereas if e.g. \code{stepw.table}=10 only each 10 iterations is saved) }
 \item{burnin.table}{ Number of MCMC iterations to throw away to
   compute the expected Potts-Dirichlet system energy for each value of psi }
 \item{plot}{Logical: if plot=TRUE the Potts-Dirichlet model normalization constants are plotted for
   each value of psi (0,0.1,0.2,...,1)}
 \item{write}{Logical: if write=TRUE the table is written in a plain ascci file named table.txt}
}


\value{
A numerical vector (the table) with (10*1)+1 components. Each component is an approximation of 
the Potts-Dirichlet model normalization constant (at log scale) for each
value of psi. We supposed that psi takes its values
between 0 and 1 with a discretization step of 0.1. 
}

\section{Storage format}{
All parameters processed by function \code{\link{tablecst}} are
written in the directory  specified by \file{pathtable} as follows:

File \file{table.txt} contains a numerical vector with
11 components. Each component is an approximation of 
the Potts-Dirichlet model normalization constant (at log scale) for each
value of psi. We supposed that psi takes its values
between 0 and 1 with a discretization step of 0.1.

(1/stepval)+1 repertories. Each one contains the outputs
of a Gibbs sampler to generate data according to Potts-Dirichlet model for psi from 0 to 1 with a
discretization step of 0.02.
}


\references{ Sylvia Richardson, Peter J.Green: "Hidden Markov Models and Disease Mapping"(JASA December
2002)}

\author{Sophie Ancelet }

\keyword{}

\seealso{Function \code{\link{simpottsdir}} }

\examples{

#library(Geneclust)
# Below is a sequence of R commands using Geneclust functions to compute
# the Potts-Dirichlet model normalization constants table when we consider 100 individuals
# organized in 2 populations.

\dontrun{

#To define a place for outputs
system("mkdir ./tmpData/")
pathtable <- "./tmpTable/"

#To generate the coordinates of 100 individuals which are supposed
#uniformly distributed in a rectangle  
coordinates<- matrix(runif(200,0,1),nrow=100,ncol=2)

#To compute the neighbourhood matrix 
 del<- deldir(x=coordinates[,1],y=coordinates[,2])
 colngh<- del$delsgs[,5:6]
 pt<- nrow(colngh)

 matngh<- matrix(0,nrow=100, ncol=100)
 for(i in 1:pt){
      matngh[colngh[i,1],colngh[i,2]]=1
      matngh[colngh[i,2],colngh[i,1]]=1
}


 table<- tablecst  (pathtable=pathtable,
                    npopmax=2,
                    coordinates=coordinates,
                    matngh=matngh,
                    stepval=0.02,
                    nit.table=20000,
                    stepw.table=10,
                    burnin.table=10000,
                    plot=TRUE,
                    write=TRUE)
           
## go to file pathtable to read outputs
}}
