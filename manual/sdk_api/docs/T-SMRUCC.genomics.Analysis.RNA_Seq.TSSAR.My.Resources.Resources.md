---
title: Resources
---

# Resources
_namespace: [SMRUCC.genomics.Analysis.RNA_Seq.TSSAR.My.Resources](N-SMRUCC.genomics.Analysis.RNA_Seq.TSSAR.My.Resources.html)_

A strongly-typed resource class, for looking up localized strings, etc.




### Properties

#### Culture
Overrides the current thread's CurrentUICulture property for all
 resource lookups using this strongly typed resource class.
#### dskellam
Looks up a localized string similar to dskellam <- function(x, lambda1, lambda2=lambda1, log=FALSE){
 # density (PMF) of Skellam distriubition (difference of Poissons)
 if (missing(x)|missing(lambda1)) stop("first 2 arguments are required")
 lambdas <- c(lambda1,lambda2)
 oops <- !(is.finite(lambdas)&(lambdas>=0))
 if(any(oops)) {
 warning("NaNs produced")
 lambdas[oops] <- NaN
 lambda1 <- lambdas[1:length(lambda1)]
 lambda2 <- lambdas[(length(lambda1)+1):length(lambdas)]
 }
 # make all arg [rest of string was truncated]";.
#### dskellam_sp
Looks up a localized string similar to dskellam.sp <- function(x, lambda1, lambda2=lambda1, log=FALSE){
 # saddlepoint density (PMF) for Skellam distribution
 terms=1
 if (missing(x)|missing(lambda1)) stop("first 2 arguments are required")
 s <- log(0.5*(x+sqrt(x^2+4*lambda1*lambda2))/lambda1)# the saddlepoint
 K <- lambda1*(exp(s)-1)+lambda2*(exp(-s)-1)# CGF(s)
 K2 <- lambda1*exp(s)+lambda2*exp(-s) # CGF''(s)
 if (terms<1) {
 ret <- exp(K-x*s)/sqrt(2*pi*K2) # saddlepoint density
 } else {
 [rest of string was truncated]";.
#### pskellam
Looks up a localized string similar to pskellam <- function(q, lambda1, lambda2=lambda1, lower.tail=TRUE, log.p=FALSE){
 # CDF of Skellam distriubition (difference of Poissons)
 if (missing(q)|missing(lambda1)) stop("first 2 arguments are required")
 lambdas <- c(lambda1,lambda2)
 oops <- !(is.finite(lambdas)&(lambdas>=0))
 if(any(oops)) {
 warning("NaNs produced")
 lambdas[oops] <- NaN
 lambda1 <- lambdas[1:length(lambda1)]
 lambda2 <- lambdas[(length(lambda1)+1):length(lambdas)]
 }
 # CDF [rest of string was truncated]";.
#### pskellam_sp
Looks up a localized string similar to pskellam.sp <- function(q, lambda1, lambda2=lambda1, lower.tail=TRUE, log.p=FALSE) {
 # Luganni-Rice saddlepoint CDF with Butler's 2nd continuity correction
 if (missing(q)|missing(lambda1)) stop("first 2 arguments are required")
 if (lower.tail) {
 xm <- -floor(q)-0.5 # continuity corrected x
 # distribution specific variables
 s <- log(0.5*(xm+sqrt(xm^2+4*lambda2*lambda1))/lambda2) # the saddlepoint
 K <- lambda2*(exp(s)-1)+lambda [rest of string was truncated]";.
#### qskellam
Looks up a localized string similar to qskellam <- function(p, lambda1, lambda2=lambda1, lower.tail=TRUE, log.p=FALSE){
 # inverse CDF of Skellam distriubition (difference of Poissons)
 if (missing(p)|missing(lambda1)) stop("first 2 arguments are required")
 # make all args the same length (for subsetting)
 lens <- c(length(p),length(lambda1),length(lambda2))
 len <- max(lens)
 if(len>min(lens)) {
 if (all(len/lens!=len%/%lens)) warning("longer object length is not a multiple of shorter object length", domain=NA)
 [rest of string was truncated]";.
#### ResourceManager
Returns the cached ResourceManager instance used by this class.
#### rskellam
Looks up a localized string similar to rskellam <- function(n, lambda1, lambda2=lambda1){
 # Skellam random variables
 if (missing(n)|missing(lambda1)) stop("first 2 arguments are required")
 if (length(n)>1) n <- length(n)
 lambda1 <- rep(lambda1,length.out=n)
 lambda2 <- rep(lambda2,length.out=n)
 oops <- !(is.finite(lambda1)&(lambda1>=0)&is.finite(lambda2)&(lambda2>=0))
 if(any(oops)) warning("NaNs produced")
 ret <- rep(NaN,length.out=n)
 n <- n-sum(oops)
 ret[!oops] <- rpois(n,lambda1[!oops])-rpois(n,lam [rest of string was truncated]";.
#### TSSAR
Looks up a localized string similar to #!/usr/bin/perl
#Last changed Time-stamp: <2013-06-10 17:00:35 fabian>
 
use strict;
use warnings;
use Getopt::Long;
use Data::Dumper;
use Pod::Usage;
use File::Basename;
use File::Temp qw(tempfile);
use vars qw/ %READS @TSS_plus @TSS_minus %coverage/;

my $call = join(" ", $0, @ARGV);

# set variables and default parameters
my $ctdir ='./';

my $SamFile_Library_P ='';
my $SamFile_Library_M ='';

my $verbose = 0;
my $clean = 1;
my $score_mode = 'd';
my $cluster = 1;
my $prorata = [rest of string was truncated]";.
