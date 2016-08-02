postpsi <-
function (path.mcmc, plot = TRUE, print = FALSE, file = path.mcmc) 
{
    mcmc <- scan(paste(path.mcmc, "chain.psi.txt", sep = ""))
    val <- as.numeric(levels(factor(mcmc)))
    compt <- numeric(length(val))
    for (i in 1:length(val)) {
        compt[i] <- length(which(mcmc == val[i]))
    }
    postmode.psi <- val[which(compt == max(compt))]
    if (plot == TRUE) {
        X11()
        hist(mcmc, freq = FALSE, br = 20, col = "darkgreen", 
            xlab = "", ylab = "", main = "Posterior distribution of psi")
    }
    if (print == TRUE) {
        postscript(paste(file, "histpostpsi.ps", sep = ""))
        hist(mcmc, freq = FALSE, br = 20, col = "darkgreen", 
            xlab = "", ylab = "", main = "Posterior distribution of psi")
        dev.off()
    }
    return(list(postmode.psi = postmode.psi))
}
