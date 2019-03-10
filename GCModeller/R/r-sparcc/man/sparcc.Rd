\name{sparcc}
\alias{sparcc}
%- Also NEED an '\alias' for EACH other topic documented here.
\title{
%%  ~~function to do ... ~~
}
\description{
%%  ~~ A concise (1-5 lines) description of what the function does. ~~
}
\usage{
sparcc(x, max.iter = 20, th = 0.1, exiter = 10)
}
%- maybe also 'usage' for other objects documented here.
\arguments{
  \item{x}{
%%     ~~Describe \code{x} here~~
}
  \item{max.iter}{
%%     ~~Describe \code{max.iter} here~~
}
  \item{th}{
%%     ~~Describe \code{th} here~~
}
  \item{exiter}{
%%     ~~Describe \code{exiter} here~~
}
}
\details{
%%  ~~ If necessary, more details than the description above ~~
}
\value{
%%  ~Describe the value returned
%%  If it is a LIST, use
%%  \item{comp1 }{Description of 'comp1'}
%%  \item{comp2 }{Description of 'comp2'}
%% ...
}
\references{
%% ~put references to the literature/web site here ~
}
\author{
%%  ~~who you are~~
}
\note{
%%  ~~further notes~~
}

%% ~Make other sections like Warning with \section{Warning }{....} ~

\seealso{
%% ~~objects to See Also as \code{\link{help}}, ~~~
}
\examples{
##---- Should be DIRECTLY executable !! ----
##-- ==>  Define data, use random,
##--	or do  help(data=index)  for the standard data sets.

## The function is currently defined as
function (x, max.iter = 20, th = 0.1, exiter = 10) 
{
    xdim <- dim(x)
    Vlist <- matrix(NA, nrow = xdim[2], ncol = max.iter)
    Corlist <- array(, dim = c(max.iter, xdim[2], xdim[2]))
    Covlist <- array(, dim = c(max.iter, xdim[2], xdim[2]))
    for (i in 1:max.iter) {
        cat("Iteration: \%d\n", i)
        tmpres <- compute.corr(x, iter = exiter, th = th)
        Vlist[, i] <- tmpres[["Vbase"]]
        Corlist[i, , ] <- tmpres[["Cor.mat"]]
        Covlist[i, , ] <- tmpres[["Cov.mat"]]
    }
    vdef <- apply(Vlist, 1, median)
    cor.def <- apply(Corlist, 2:3, median)
    vdefsq <- vdef^0.5
    ttmp <- cor.def * vdefsq
    cov.def <- t(ttmp) * vdefsq
    return(list(CORR = cor.def, COV = cov.def, VBASIS = vdef))
  }
}
% Add one or more standard keywords, see file 'KEYWORDS' in the
% R documentation directory.
\keyword{ ~kwd1 }
\keyword{ ~kwd2 }% __ONLY ONE__ keyword per line
