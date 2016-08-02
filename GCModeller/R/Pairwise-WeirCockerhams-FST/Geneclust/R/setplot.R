setplot <-
function (xdata, ydata, pretty.call = TRUE, maxdim, axes = FALSE) 
{
    if (missing(xdata)) {
        stop("no xdata nor ydata was passed to setplot")
    }
    if (is.matrix(xdata)) {
        if (ncol(xdata) != 2) {
            stop(paste(substitute(xdata), "has too many columns"))
        }
        ydata <- xdata[, 2]
        xdata <- xdata[, 1]
    }
    if (is.list(xdata)) {
        ydata <- xdata$y
        if (is.null(ydata)) 
            stop(paste(substitute(xdata), "has no y component"))
        xdata <- xdata$x
        if (is.null(xdata)) 
            stop(paste(substitute(xdata), "has no x component"))
    }
    if (missing(ydata)) {
        stop("no ydata was passed to setplot")
    }
    if (pretty.call) {
        xdata <- pretty(xdata)
        ydata <- pretty(ydata)
    }
    xlim <- range(xdata)
    ylim <- range(ydata)
    xrng <- xlim[2] - xlim[1]
    yrng <- ylim[2] - ylim[1]
    prng <- max(xrng, yrng)
    oldpin <- par("pin")
    if (missing(maxdim)) {
        if (xrng/yrng > oldpin[1]/oldpin[2]) {
            maxdim <- oldpin[1]
            prng <- xrng
        }
        else {
            maxdim <- oldpin[2]
            prng <- yrng
        }
    }
    newpin <- (maxdim * c(xrng, yrng))/prng
    par(pty = "m", pin = newpin)
    list(xlim, ylim, oldpin, newpin)
}
