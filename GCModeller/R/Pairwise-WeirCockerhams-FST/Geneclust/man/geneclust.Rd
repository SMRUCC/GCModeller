\name{geneclust}

\alias{geneclust}
%- Also NEED an '\alias' for EACH other topic documented here.

\title{Bayesian inference of population structure using multilocus genotypes and spatial coordinates}

\description{Main function of the "geneclust" package. Performs inference of population structure using spatial coordinates and multilocus genotypes.}

\usage{
geneclust(project.name = "Data", data, npopmax = 3, psi = 0.5, nit = 1000, burnin = 10, thinning = 1, c = NULL, freq = NULL, tabcst = NULL, 
matngh = NULL, fis = rep(0, npopmax), varpsi = FALSE, varfis = FALSE, otherconfig=NULL, write=FALSE)
}

%- maybe also 'usage' for other objects documented here.
\arguments{

\item{project.name}{ A path to output files directory.}

\item{data}{ Object of class "geneclustdata". A dataframe that contains individuals locations and their genotypes.}
 
\item{npopmax}{ An initial number of clusters. May be different from the final number of clusters.}

\item{psi}{ A (numeric) value for the spatial interaction parameter. The
  \code{psi = 0} option corresponds to an implementation of the
  algorithm STRUCTURE. 
  Typical values should be between 0 and 1.}

\item{nit}{ Number of MCMC cycles. One cycle visits each locus and each individual.}
 
\item{burnin}{ Number of cycles corresponding to the MCMC burnin period (Markov chain internal parameter).}

\item{thinning}{ Number of recorded cycles (Markov chain internal parameter).}

\item{c}{ A vector containing starting cluster labels for the sample. If \code{c = NULL}, the program starts with the uniform distribution on (1,..,\code{npopmax})}

\item{freq}{ A three dimensional array which contains the initial allele
  frequencies in each cluster, for each locus and each allele. If freq =
  NULL, the initial allele 
  frequencies are randomly sampled according to the Dirichlet distribution D(1,1,..,1).}

\item{tabcst}{ If varpsi = T and tabcst = NULL, the program computes a
  partition function table (cf function \code{tablecst}) which may takes
  time. 
  A vector with 11 components is returned and used afterwards for the inference of the Potts-Dirichlet spatial interaction parameter \code{psi}.}

\item{matngh}{ An binary matrix which defines the neighbourhoods to be
  used in the Potts prior model. 
  If \code{matngh[i,j]=1} then i and j are neighbours. If \code{matngh = NULL}, the matrix is computed from the Delaunay graph via the package \code{deldir}.} 

\item{fis}{ A vector with \code{npopmax} components containing the initial
values for inbreeding coefficients. If fis = NULL, the initialization is at random.}
 
\item{varpsi}{ Logical: if varpsi = TRUE, the spatial interaction
  parameter \code{psi} is treated as an
  unknown parameter and varies along the MCMC run. If \code{varpsi = FALSE}, then \code{psi} is kept fixed.}

\item{varfis}{ Logical: if varfis = TRUE, the inbreeding coefficients
  are treated as unknown parameters and 
  vary along the MCMC run. If \code{varfis = FALSE}, they are kept fixed to the initial value \code{fis}. }

\item{otherconfig}{A spatial configuration of individuals to compare with the posterior spatial configuration reached after the MCMC
run. For example, it could be a configuration obtained with a non-Bayesian hierarchical clustering algorithm such as the Ward reconstruction method.}

\item{write}{Logical: If TRUE, some outputs are written in ascci files
  in the directory project.name}

}


\value{ An object of class \code{geneclust}. 


  \item{prob}{ A matrix with indicates the posterior distributions of membership coefficients for each individual}

  \item{membership}{ A vector containing the most likely cluster membership}

  \item{K}{ Estimated number of clusters (less than the initial number)}

  \item{postmodepsi}{The posterior mode of psi}
  \item{postmeanfis}{A numerical vector which contains the posterior mean of inbreeding coefficient in each identified cluster}
  \item{postquantfis}{A matrix that stores the posterior distribution quantiles
  of each inbreeding coefficient. Each line corresponds to one cluster}
  \item{diffclassif}{A rate of misclassification computed if another
    spatial configuration is given as argument}

  \item{coord}{ Individual spatial coordinates}

  \item{psi}{ Spatial interaction parameter after nit MCMC cycles}

 \item{fis}{ Inbreeding coefficients after nit cycles}

 \item{path}{ Path to the MCMC program output data}

 \item{c}{ Cluster configuration after nit MCMC cycles}

 \item{freq}{ Allele frequencies after nit MCMC cycles}

 \item{matngh}{ Neighbourhood matrix}

 \item{tabcst}{ Partition function table}

}


\details{ 

Bayesian clustering and computations of individual membership
probabilities are performed using a
MCMC algorithm similar to STRUCTURE (Pritchard et al, 2000) implemented in the main functions \code{\link{geneclust}} and \code{\link{mcmcgeneclust}} 

In addition, the package includes the use of Hidden Markov Random Fields
(HMRF) priors enabling the
simultaneous analysis of spatial coordinates. So the input data include individual genotypes and spatial coordinates.

Basically the HMRF is used as a model for the spatial continuity of
genotypes within a population.
It contains a spatial interaction parameter psi which represents the
intensity at which individual genotypes depends
from their neighbors. For psi = 0, the program corresponds to another
implementation of STRUCTURE. The HMRF assumes an
interaction graph for the individuals. In the default implementation the
graph is computed as the Dirichlet-Delaunay
structure via the package deldir. But the program allows modifications or other implementation of graphs.

The model also assumes linkage equilibrium, but tolerates departures
from the HW equilibrium using imbreeding coefficients.
For psi > 0, the program includes an automatic selection procedure for the actual number of clusters in the population based on Bayesian regularization.


Basically this function does the same as \code{\link{mcmcgeneclust}} but
it proposes a simplified version, and an easier
access to data summaries. Users of the \code{geneland} package or users at the expert stage may prefer the function \code{\link{mcmcgeneclust}} which offers more parameters.}


\author{ 
Sophie Ancelet
}

\references{

On mixture models in population genetics:

- J.K. Pritchard, M. Stephens and P. Donnelly,
Inference of population structure using multilocus genotype data,
Genetics, pp 945-959 vol. 155, 2000

- Falush D., M. Stephens and  J.K. Pritchard, 
Inference of population structure using multilocus genotype data:
Linked loci and correlated allele frequencies, Genetics, pp 1567-1587,
vol 164, 2003

- G. Guillot, Estoup, A., Mortier, F. Cosson, J.F. A spatial statistical
model for landscape genetics. Genetics, 170, 1261-1280, 2005.

- G. Guillot, Mortier, F., Estoup, A. Geneland : A program for landscape
genetics. Molecular Ecology Notes, 5, 712-715, 2005.



On the implementation of MCMC inference on the spatial interaction parameter psi
of a Potts-Dirichlet model:

- P.Green, S.Richardson: Hidden Markov models and disease mapping, Journal of the American Statistical Association 97(460): 1055-1070


On the model (and sub-models) implemented in \code{geneclust}

- O.Francois, S. Ancelet, G. Guillot (2006). papers in preparation. 

}


\seealso{Functions \code{\link{mcmcgeneclust}},\code{\link{postclassif}},\code{\link{postpsi}},\code{\link{postfis}}}

\keyword{}

\examples{

}

