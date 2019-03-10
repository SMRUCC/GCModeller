\name{exclude.pairs}
\alias{exclude.pairs}
%- Also NEED an '\alias' for EACH other topic documented here.
\title{
%%  ~~function to do ... ~~
}
\description{
%%  ~~ A concise (1-5 lines) description of what the function does. ~~
}
\usage{
exclude.pairs(Cor.mat, M, th = 0.1, excluded = NULL)
}
%- maybe also 'usage' for other objects documented here.
\arguments{
  \item{Cor.mat}{
%%     ~~Describe \code{Cor.mat} here~~
}
  \item{M}{
%%     ~~Describe \code{M} here~~
}
  \item{th}{
%%     ~~Describe \code{th} here~~
}
  \item{excluded}{
%%     ~~Describe \code{excluded} here~~
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
function (Cor.mat, M, th = 0.1, excluded = NULL) 
{
    flag <- FALSE
    cor.tmp <- abs(Cor.mat)
    diag(cor.tmp) <- diag(cor.tmp) - diag(Cor.mat)
    if (!is.null(excluded)) 
        cor.tmp[excluded, ] <- 0
    mm <- max(cor.tmp)
    idxtorm <- which(cor.tmp == mm, arr.ind = TRUE)
    if (mm > th) {
        for (i in 1:dim(idxtorm)[1]) {
            M[idxtorm[i, 1], idxtorm[i, 2]] <- M[idxtorm[i, 1], 
                idxtorm[i, 2]] - 1
        }
        dd <- diag(M)[unique(c(idxtorm))]
        diag(M)[unique(c(idxtorm))] <- dd - 1
        excluded <- rbind(excluded, idxtorm)
    }
    else {
        excluded <- excluded
        flag <- TRUE
    }
    return(list(M = M, excluded = excluded, flag = flag))
  }
}
% Add one or more standard keywords, see file 'KEYWORDS' in the
% R documentation directory.
\keyword{ ~kwd1 }
\keyword{ ~kwd2 }% __ONLY ONE__ keyword per line
