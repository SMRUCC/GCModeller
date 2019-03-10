subsample <-
function (data, nloci, lst = NULL) 
{
    if (class(data)[1] != "geneclustdata") 
        stop("invalid object")
    m <- dim(data)[2]/2 - 1
    if (is.null(lst)) {
        lst <- sample(1:m, nloci, replace = FALSE)
    }
    if (is.null(lst) == FALSE) {
        if (length(lst) != nloci) {
            stop(cat(paste("The length of lst isn't equal to nloci!")))
        }
    }
    lst2 <- rep(NA, 2 * nloci)
    ind <- 1:nloci
    for (i in 1:nloci) {
        lst2[2 * i - 1] <- 2 * lst[i] - 1
        lst2[2 * i] <- 2 * lst[i]
    }
    lst2 <- 2 + lst2
    new.dat <- data[, c(1:2, lst2)]
    return(new.dat)
}
