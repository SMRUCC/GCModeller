# Resources
_namespace: [RDotNET.Extensions.Bioinformatics.My.Resources](./index.md)_

A strongly-typed resource class, for looking up localized strings, etc.




### Properties

#### Culture
Overrides the current thread's CurrentUICulture property for all
 resource lookups using this strongly typed resource class.
#### ResourceManager
Returns the cached ResourceManager instance used by this class.
#### sparcc
Looks up a localized string similar to require(gtools)


## NB
##------------------------------
## count matrix x should be samples on the rows and OTUs on the colums,
## assuming dim(x) -> samples by OTUs

sparcc <- function(x, max.iter=20, th=0.1, exiter=10){
 xdim <- dim(x)
 Vlist <- matrix(NA,nrow=xdim[2],ncol=max.iter)
 Corlist <- array(,dim=c(max.iter, xdim[2], xdim[2]))
 Covlist <- array(,dim=c(max.iter, xdim[2], xdim[2]))

 ## Cycle max.iter times for variability in variance estimation
 for (i in 1:max.iter){
 cat("Iteration: %d [rest of string was truncated]";.
