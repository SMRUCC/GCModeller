\name{mcmcgeneclust}

\alias{mcmcgeneclust}
%- Also NEED an '\alias' for EACH other topic documented here.

\title{MCMC inference of population structure using multilocus genotypes}

\description{Runs a Markov Chain Monte-Carlo for Bayesian clustering of multilocus genotypes using a hidden spatial model.}

\usage{
mcmcgeneclust(path.mcmc, genotypes, coordinates, npopmax, nit = 3000, burnin = 200, thinning = 5, c = NULL, psi = 0, fis = NULL, freq=NULL, tabcst = NULL, matngh = NULL,flat.prior.psi=TRUE, stepval = 0.02, nit.table = 20000, burnin.table = 10000, stepw.table = 10, alpha = 4, beta=40, varpsi = TRUE, varfis = TRUE)
}

%- maybe also 'usage' for other objects documented here.
\arguments{
  \item{path.mcmc}{ Path to output files directory. Path should be given in the Unix style even under Windows (use / instead of \\). The path must be ended by slash (/) (e.g. path.mcmc="/home/me/Genetics/") }
 \item{genotypes}{ (Codominant) genotypes of individuals. A matrix with one line per individual and 2 columns per locus}
  \item{coordinates}{ Spatial coordinates of individuals. A matrix with 2 columns and one line per individual. }
  \item{npopmax}{ An initial number of clusters. Maybe different from the final number of clusters}
  \item{nit}{ Number of MCMC cycles. One cycle visits every locus and every individuals.} 
   \item{burnin}{ Number of cycles to throw away. Results are stored in ascii files from \code{burnin} and only each \code{thinning} cycles.}
  \item{thinning}{ Number of cycles between two writings }
  \item{c}{ A vector with nindiv components containing the initial population memberships. If c = NULL, the program is started according to the uniform distribution on (1,..,npop)}
  \item{psi}{ A value for the spatial interaction parameter (numeric) to be used when varpsi = F. The psi = 0 and varpsi = F options correspond to an implementation of the algorithm STRUCTURE.}
  \item{fis}{ A vector with npopmax components containing the initial
  values for each population inbreeding coefficient. If fis = NULL,
  initialization will be made according to the Beta distribution with
  parameters alpha and beta}
 \item{freq}{ A three dimensional array (npopmax*number of
   loci*maximum number of alleles) which contains the initial allele frequencies for each population, each locus and each allele. If freq=NULL, the initial allele frequencies are distributed according to a
 non-informative Dirichlet distribution D(1,1,..,1).}
  \item{tabcst}{ A vector with 11 components used in the inference of the Potts-Dirichlet spatial interaction parameter (psi).  If tabcst=NULL, the program computes the normalization constant table (cf
  function tablecst).}
  \item{matngh}{ A matrix with one line per individual and one column
  per individual defining the neighbourhood relationships between
  individuals. matngh[i,j]=1 if i and j are neighbouring
  individuals. If matngh=NULL, the neighbourhood matrix is defined by
  a Delaunay graph via the function deldir contained in library
  deldir.} 
\item{flat.prior.psi}{If flat.prior.psi==TRUE, the prior on the spatial
  interaction parameter psi is a uniform distribution over
  (0,0.1,0.2,..,1).If flat.prior.psi==FALSE, the prior is a beta distribution
(discretized) with mean=0.6 and sd=0.084 }
  \item{stepval}{  Step of discretization of the interval [0,psimax]
  defining the values for which E(U(c)|psi) is computed.}
  \item{nit.table}{  Number of MCMC iterations to generate Potts-Dirichlet
  configurations in order to compute E(U(c)|psi) where U(c) is the Potts-Dirichlet
  configuration energy}
  \item{burnin.table}{ Number of MCMC updates to throw away to compute E(U(c)|psi) }
  \item{stepw.table}{  Number of MCMC updates between two stored
  updates to compute E(U(c)|psi) }
  \item{alpha}{  The shape1 parameter of the beta prior on the
  inbreeding coefficients. alpha must be positive.}
  \item{beta}{ The shape2 parameter of the beta prior on the
  inbreeding coefficients. beta must be positive} 
  \item{varpsi}{ Logical: if varpsi=TRUE the spatial interaction parameter psi is treated as
    unknown and will vary along the MCMC inference. If varpsi=FALSE it will be
    fixed to the initial value \code{psi}. }
  \item{varfis}{ Logical: if varfis=TRUE the inbreeding coefficients are treated as
    unknown and will vary along the MCMC inference. If varfis=FALSE they will be
    fixed to the initial value \code{fis}. }
}

\value{
Successive states of all blocks of parameters are written in external files contained in \code{path.mcmc} and named after the type of parameters that they contain.

\item{c}{ Cluster configuration of
  individuals reached after nit MCMC cycles}

\item{psi}{ Spatial interaction parameter psi value reached
  after nit MCMC cycles}

\item{fis}{ Inbreeding coefficients values reached
  after nit MCMC cycles}

\item{freq}{ Allele frequencies values reached
  after nit MCMC cycles}

\item{matngh}{ Neighbourhood matrix}

\item{table}{ Normalization constant table}

}


\section{Storage format}{
All parameters processed by function \code{\link{mcmcgeneclust}} are
written in the directory specified by \file{path.mcmc} as follows:

File \file{population.numbers.txt} contains values of the estimated number of clusters (\code{nit} lines, one line per cycles of the MCMC algorithm)


File \file{frequencies.txt} contains allele frequencies of current populations. Column xx contains frequencies of the population  labelled xx. In each column, the values of the allele frequencies are stored by increasing allele index and locus index (allele index varying first), and values for successive cycles are pasted. The file has \code{nallmax*nloc*nit/thinning} lines where \code{nallmax} 
is the maximum number of alleles over all loci.


File \file{psi.txt} contains values of the spatial interaction
parameter psi (\code{nit} lines, one line per cycle of the MCMC algorithm)


File \file{fis.txt} contains values of the inbreeding coefficients (\code{nit} lines, one line per cycle of the MCMC
algorithm)


File \file{parameters.txt} contains a summary of the main inference parameters 
}


\details{ For those who want to handle the all obscure parameters of an MCMC run. This is the core function used by \code{\link{geneclust}}. Its syntax is very similar to the one used by the package \code{geneland}. So MCMC experts and geneland users may prefer this function to geneclust. 
}

\author{ Sophie Ancelet}

\seealso{Function \code{\link{geneclust}}, \code{\link{tablecst}},\code{\link{postclassif}},\code{\link{postpsi}},\code{\link{postfis}}  }

\keyword{}

\examples{

# Below is a sequence of R commands using geneclust functions
# The commands are in the same format as the 'Geneland' package by G. Guillot

# library(Geneclust)

\dontrun{

# Simulation of a dataset according to the prior distributions. This one
# is made of 2 populations
# 10 loci and 10 alleles per loci
# on a spatial domain enclosed in a rectangle
# (x.coord. in [0,1], y.coord. in [0,1])
# Spatial interaction parameter is 0.4
# We suppose the inbreeding coefficients are the same in each population
# and equal to 0.1

#To define a place for simulation outputs
path <- "./tmpData/"
system("mkdir ./tmpData/")


data<- simpatch (    path=path,
                     nindiv=100,
                     coordinates=NULL,
                     coord.lim=c(0,1,0,1),
                     npop=2,
                     nall=rep(10,10),
                     psi=0.4,
                     fis=c(0.1,0.1),
                     nchain=50000,
                     burnin=40000,
                     stepw=100,
                     seed=123,
                     plot=FALSE,
                     write=FALSE,
                     print=FALSE,
                     file=path)

## go to file path to read simulation outputs


#To run Full Bayesian Markov Chain Monte Carlo inference of all
#parameters
#To define a place for MCMC outputs

path.mcmc<- path

infer    <- mcmcgeneclust(
                           path.mcmc=path.mcmc,
                           genotypes=data$genotypes,
                           coordinates=data$coordinates,
                           npopmax=3,
                           nit=3000,
                           burnin=200,
                           thinning=5,
                           c=NULL,
                           psi=0,
                           fis=rep(0,times=3),
                           freq=NULL,                              
                           tabcst=NULL,
                           matngh=NULL,
                           flat.prior.psi=TRUE,
                           stepval=0.02,
                           nit.table=20000,
                           burnin.table=10000,
                           stepw.table=10,
                           alpha=4,
                           beta=40,
                           varpsi=TRUE,
                           varfis=TRUE)



# To make a second run of MCMC algorithm from the last MCMC
#configuration:

infer2    <-    mcmcgeneclust(
                           path.mcmc=path.mcmc,
                           genotypes=data$genotypes,
                           coordinates=data$coordinates,
                           npopmax=3,
                           nit=3000,
                           burnin=0,
                           thinning=5,
                           c=infer$c,
                           psi=infer$psi,
                           fis=infer$fis,
                           freq=infer$freq,                              
                           tabcst=infer$table,
                           matngh=infer$matngh,
                           flat.prior.psi=TRUE,
                           stepval=0.02,
                           nit.table=20000,
                           burnin.table=10000,
                           stepw.table=10,
                           alpha=4,
                           beta=40,
                           varpsi=TRUE,
                           varfis=TRUE)



#Go to file path.mcmc to read MCMC outputs

#To computes for posterior probabilities and posterior
#modal populations 

classif<- postclassif(path.mcmc=path.mcmc,
                      coordinates=data$coordinates,
                      popmbrship=data$popmbrship,
                      plot=TRUE,
                      print=TRUE,
                      file=path.mcmc,
                      write=TRUE)

#Number of inferred populations
classif$K.est

#Rate of misclassifications 
classif$errclassif


#To get the main properties of the posterior distribution of the spatial interaction parameter (psi)
interparam<- postpsi(path.mcmc=path.mcmc,
                     plot=TRUE,
                     print=TRUE,
                     file=path.mcmc)

#Postmode psi
interparam$postmode.psi


#To get the main properties of the inbreeding coefficients posterior
#distributions


inbreedcoeff<- postfis(path.mcmc=path.mcmc,
                       postmode.indiv= classif$postmode.indiv,
                       probs = c(0.025, 0.25, 0.5, 0.75, 0.975),
                       plot = TRUE,
                       print = TRUE,
                       file=path.mcmc)
#Postmean fis
inbreedcoeff$postmean.fis

#Credibility intervals
inbreedcoeff$quant.fis


#To computes indices which quantify the level of  genetic differentiation between inferred populations according to Weir and Cockerham's 
#estimators

differindex<- Fst(genotypes=data$genotypes,
                  allele.numbers=rep(10,10),
                  pop.mbrship=classif$postmode.indiv,
                  npopmax=3)

#Weir and Cockerham's estimator of Fst between inferred populations
differindex$Pairwise.Fst


}}

