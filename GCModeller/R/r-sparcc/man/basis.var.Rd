\name{basis.var}
\alias{basis.var}
%- Also NEED an '\alias' for EACH other topic documented here.
\title{
%%  ~~function to do ... ~~
}
\description{
%%  ~~ A concise (1-5 lines) description of what the function does. ~~
}
\usage{
basis.var(fracs, V, Vmin = 1e-04, excluded = NULL, Covmat = NULL, M = NULL)
}
%- maybe also 'usage' for other objects documented here.
\arguments{
  \item{fracs}{
%%     ~~Describe \code{fracs} here~~
}
  \item{V}{
%%     ~~Describe \code{V} here~~
}
  \item{Vmin}{
%%     ~~Describe \code{Vmin} here~~
}
  \item{excluded}{
%%     ~~Describe \code{excluded} here~~
}
  \item{Covmat}{
%%     ~~Describe \code{Covmat} here~~
}
  \item{M}{
%%     ~~Describe \code{M} here~~
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
function (fracs, V, Vmin = 1e-04, excluded = NULL, Covmat = NULL, 
    M = NULL) 
{
    Vsize <- dim(V)
    Vvec <- apply(V, 1, sum)
    if (is.null(Covmat)) 
        Covmat <- matrix(0, nrow = Vsize[1], ncol = Vsize[2])
    Covvec <- apply(Covmat - diag(Covmat), 1, sum)
    if (is.null(M)) {
        M <- matrix(1, nrow = Vsize[1], ncol = Vsize[2])
        diag(M) <- Vsize[1] - 1
    }
    Minv <- solve(M)
    Vbase <- Minv \%*\% (Vvec + 2 * Covvec)
    Vbase[Vbase < 0] <- Vmin
    return(list(Vbase = Vbase, M = M))
  }
}
% Add one or more standard keywords, see file 'KEYWORDS' in the
% R documentation directory.
\keyword{ ~kwd1 }
\keyword{ ~kwd2 }% __ONLY ONE__ keyword per line
