\name{Geneclust-package}

\alias{Geneclust-package}

\alias{Geneclust}

\docType{package}

\title{Bayesian clustering and MCMC inference in spatial population genetics}


\description{Performs inference of spatial structure using multilocus genotypes and spatial coordinates.}


\details{
  \tabular{ll}{
    Package: \tab Geneclust\cr
    Type: \tab Package\cr
    Version: \tab 0.1\cr
    Date: \tab 2006-04-11\cr
    License: \tab GPL2\cr
  }
  
Bayesian clustering and computations of individual membership probabilities are performed using a MCMC algorithm similar to STRUCTURE (Pritchard et al, 2000) implemented in the main functions \code{\link{geneclust}} and \code{\link{mcmcgeneclust}}

In addition, the package includes the use of Hidden Markov Random Fields (HMRF) priors enabling the simultaneous analysis of spatial coordinates. So the input data include individual genotypes and spatial coordinates.

Basically the HMRF is used as a model for the spatial continuity of genotypes within a population. It contains a spatial interaction parameter psi which represents the intensity at which individual genotypes depends from their neighbors. For psi = 0, the program corresponds to another implementation of STRUCTURE. The HMRF assumes an interaction graph for the individuals. In the default implementation the graph is computed as the Dirichlet-Delaunay structure via the package deldir. But the program allows modifications or other implementation of graphs.

The model also assumes linkage equilibrium, but tolerates departures from the HW equilibrium using imbreeding coefficients. For psi > 0, the program includes an automatic selection procedure for the actual number of clusters in the population based on Bayesian regularization.



The following functions are provided by the package:

\code{\link{geneclust}}: Performs Bayesian inference of membership coefficients. Users at the expert stage or \code{Geneland} users may prefer the function \code{\link{mcmcgeneclust}}.  

% \code{\link{plot.geneclust}}: Displays cluster memberships and
% interpolates membership probabilities. 

% \code{\link{print.geneclust}}: Describes an object of class
% \code{geneclust} (number of individuals, number of clusters detected, average loglikelihood )

% \code{\link{as.geneclustdata}}: Creates the correct format for input to \code{geneclust}.

% \code{\link{as.geneclustdata.structure}}: Converts genotypes from the "structure" format to the "geneclust" format for input to \code{geneclust}.

% \code{\link{is.geneclustdata}}: Checks data sets before input to \code{geneclust}.

% \code{\link{summary.geneclustdata}}: Brief summary of input data.


Users at the expert stage or \code{Geneland} users may prefer the
following functions:

\code{\link{mcmcgeneclust}}: Full Bayesian Markov Chain Monte Carlo inference of all parameters.
  
\code{\link{postclassif}}:  Post-processing of MCMC outputs. Calculates posterior membership probabilities and cluster assignments
    
\code{\link{postpsi}}:  Post-processing of MCMC outputs. Calculates posterior distribution of the spatial interaction parameter psi.
  
\code{\link{postfis}}: Post-processing of MCMC outputs. Calculates the posterior distribution of the inbreeding coefficients
  
\code{\link{tablecst}}: Computes a normalization table needed to perform MCMC inference on the spatial interaction parameter psi 

\code{\link{Fst}}: Computes F-statistics.



\code{\link{simpatch}}: Simulates geo-referenced multilocus data from the prior distributions.

% \code{\link{twoislands}}: Simulates multilocus genetic data from the infinite allele model in a spatially structured model.

% \code{\link{simpottsdir}}: Simulates spatial configurations from the Potts-Dirichlet model on an irregular
% lattice 


% \code{\link{hgclust}}: Non-Bayesian hierarchical clustering algorithm
% (NJ tree on shared allele distances).

% \code{\link{plot.hgclust}}: Plots results from \code{hgclust}



% \code{Dist.gen}: Internal function used in hgclust

% \code{distC.gen}: Internal function used in hgclust

\code{FormatGenotypes}: Formats file of genotype data

\code{subsample}: Makes a subsample from an initial dataset by reducing the number of loci to consider

\code{setplot}: Sets graphical parameters so as to have equal ratio for horizontal
and vertical axis.


}

 
\author{
Sophie Ancelet
Gilles Guillot
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

\keyword{}


\examples{


}

