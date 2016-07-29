\name{compute.corr}
\alias{compute.corr}
%- Also NEED an '\alias' for EACH other topic documented here.
\title{
%%  ~~function to do ... ~~
}
\description{
%%  ~~ A concise (1-5 lines) description of what the function does. ~~
}
\usage{
compute.corr(x, iter = 10, th = 0.1)
}
%- maybe also 'usage' for other objects documented here.
\arguments{
  \item{x}{
%%     ~~Describe \code{x} here~~
}
  \item{iter}{
%%     ~~Describe \code{iter} here~~
}
  \item{th}{
%%     ~~Describe \code{th} here~~
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
function (x, iter = 10, th = 0.1) 
{
    fracs <- counts2frac(x)
    V <- variation.mat(fracs)
    ll1 <- basis.var(fracs, V)
    ll2 <- cor.from.basis(V, ll1[["Vbase"]])
    excluded <- NULL
    for (i in 1:iter) {
        ll3 <- exclude.pairs(ll2[["Cor.mat"]], ll1[["M"]], th = th, 
            excluded = excluded)
        excluded <- ll3[["excluded"]]
        if (!ll3[["flag"]]) {
            ll1 <- basis.var(fracs, V, M = ll3[["M"]], excluded = excluded)
            ll2 <- cor.from.basis(V, ll1[["Vbase"]])
        }
    }
    return(list(Vbase = ll1[["Vbase"]], Cor.mat = ll2[["Cor.mat"]], 
        Cov.mat = ll2[["Cov.mat"]]))
  }
}
% Add one or more standard keywords, see file 'KEYWORDS' in the
% R documentation directory.
\keyword{ ~kwd1 }
\keyword{ ~kwd2 }% __ONLY ONE__ keyword per line
