postfis <-
function (path.mcmc, postmode.indiv, probs = c(0.025, 0.25, 0.5, 
    0.75, 0.975), plot = TRUE, print = FALSE, file = path.mcmc) 
{
    mcmcchain <- scan(paste(path.mcmc, "chain.fis.txt", sep = ""))
    param <- as.matrix(read.table(paste(path.mcmc, "paraminfer.txt", 
        sep = "")))
    npopmax <- as.numeric(param[param[, 1] == "npopmax", 3])
    fisconserv <- matrix(mcmcchain, nrow = npopmax, byrow = FALSE)
    cl <- as.numeric(levels(factor(postmode.indiv)))
    fispopinf <- fisconserv[cl[1], ]
    fispopinf <- matrix(nrow = npopmax, ncol = ncol(fisconserv))
    for (i in 1:length(cl)) {
        fispopinf[cl[i], ] <- fisconserv[cl[i], ]
    }
    postmean.fis <- as.numeric(apply(fispopinf, FUN = mean, MARGIN = 1, 
        na.rm = TRUE))
    quant.fis <- t(as.matrix(apply(fispopinf, FUN = quantile, 
        MARGIN = 1, probs = probs, na.rm = TRUE)))
    if (plot == TRUE) {
        X11()
        if (length(cl)%%2 == 0) {
            graph <- matrix(1:length(cl), nrow = length(cl)/2, 
                ncol = 2)
        }
        else {
            graph <- matrix(1:(length(cl) + 1), nrow = (length(cl) + 
                1)/2, ncol = 2)
        }
        layout(graph)
        for (i in 1:length(cl)) {
            hist(fisconserv[cl[i], ], breaks = 20, freq = FALSE, 
                col = "darkgreen", main = paste("Population", 
                  cl[i], "inbreeding coefficient"), xlab = "")
        }
    }
    if (print == TRUE) {
        postscript(paste(file, "histpostfis.ps", sep = ""))
        if (length(cl)%%2 == 0) {
            graph <- matrix(1:length(cl), nrow = length(cl)/2, 
                ncol = 2)
        }
        else {
            graph <- matrix(1:(length(cl) + 1), nrow = (length(cl) + 
                1)/2, ncol = 2)
        }
        layout(graph)
        for (i in 1:length(cl)) {
            hist(fisconserv[cl[i], ], breaks = 20, freq = FALSE, 
                col = "darkgreen", main = paste("Population", 
                  cl[i], "inbreeding coefficient"), xlab = "")
        }
        dev.off()
    }
    return(list(postmean.fis = postmean.fis, quant.fis = quant.fis))
}
