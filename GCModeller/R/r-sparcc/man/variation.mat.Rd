\name{variation.mat}
\alias{variation.mat}
%- Also NEED an '\alias' for EACH other topic documented here.
\title{
%%  ~~function to do ... ~~
}
\description{
%%  ~~ A concise (1-5 lines) description of what the function does. ~~
}
\usage{
variation.mat(fracs)
}
%- maybe also 'usage' for other objects documented here.
\arguments{
  \item{fracs}{
%%     ~~Describe \code{fracs} here~~
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
function (fracs) 
{
    V <- matrix(NA, ncol = dim(fracs)[2], nrow = dim(fracs)[2])
    tmplog <- apply(fracs, 2, log)
    idx <- combn(1:dim(fracs)[2], 2)
    ttmp <- tmplog[, idx[1, ]] - tmplog[, idx[2, ]]
    vartmp <- apply(ttmp, 2, var)
    for (i in 1:length(vartmp)) {
        V[idx[1, i], idx[2, i]] <- V[idx[2, i], idx[1, i]] <- vartmp[i]
    }
    diag(V) <- 1
    return(V)
  }
}
% Add one or more standard keywords, see file 'KEYWORDS' in the
% R documentation directory.
\keyword{ ~kwd1 }
\keyword{ ~kwd2 }% __ONLY ONE__ keyword per line
