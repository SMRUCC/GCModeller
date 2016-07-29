\name{cor.from.basis}
\alias{cor.from.basis}
%- Also NEED an '\alias' for EACH other topic documented here.
\title{
%%  ~~function to do ... ~~
}
\description{
%%  ~~ A concise (1-5 lines) description of what the function does. ~~
}
\usage{
cor.from.basis(V, Vbase)
}
%- maybe also 'usage' for other objects documented here.
\arguments{
  \item{V}{
%%     ~~Describe \code{V} here~~
}
  \item{Vbase}{
%%     ~~Describe \code{Vbase} here~~
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
function (V, Vbase) 
{
    p <- dim(Vbase)[1]
    Cor.mat <- diag(rep(1, p))
    Cov.mat <- diag(Vbase[, 1])
    idx <- combn(p, 2)
    for (i in 1:(p - 1)) {
        idxslice <- idx[1, ] == i
        cov.tmp <- 0.5 * (Vbase[i] + Vbase[idx[2, idxslice]] - 
            V[i, idx[2, idxslice]])
        denom <- sqrt(Vbase[i]) * sqrt(Vbase[idx[2, idxslice]])
        cor.tmp <- cov.tmp/denom
        abscor <- abs(cor.tmp)
        if (any(abscor > 1)) {
            idxthr <- abscor > 1
            cor.tmp[idxthr] <- sign(cor.tmp[idxthr])
            cov.tmp[idxthr] <- cor.tmp[idxthr] * denom[idxthr]
        }
        Cor.mat[i, idx[2, idxslice]] <- Cor.mat[idx[2, idxslice], 
            i] <- cor.tmp
        Cov.mat[i, idx[2, idxslice]] <- Cov.mat[idx[2, idxslice], 
            i] <- cov.tmp
    }
    return(list(Cor.mat = Cor.mat, Cov.mat = Cov.mat))
  }
}
% Add one or more standard keywords, see file 'KEYWORDS' in the
% R documentation directory.
\keyword{ ~kwd1 }
\keyword{ ~kwd2 }% __ONLY ONE__ keyword per line
