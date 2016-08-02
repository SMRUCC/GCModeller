postclassif <-
function (path.mcmc, coordinates, popmbrship = NULL, plot = TRUE, 
    print = FALSE, file = path.mcmc, write = FALSE) 
{
    fileparam <- paste(path.mcmc, "paraminfer.txt", sep = "")
    param <- as.matrix(read.table(fileparam))
    nindiv <- as.numeric(param[param[, 1] == "nindiv", 3])
    npopmax <- as.numeric(param[param[, 1] == "npopmax", 3])
    updatelabel = scan(paste(path.mcmc, "chain.popmbrship.txt", 
        sep = ""))
    npair = (nindiv * (nindiv - 1))/2
    mat.label = matrix(nr = nindiv, byrow = FALSE, data = updatelabel)
    comptlab <- function(uplabind, npopmax) {
        compt <- NULL
        for (i in 1:npopmax) {
            compt = c(compt, length(which(uplabind == i)))
        }
        prob = compt/(length(uplabind))
        mode = which(compt == max(compt))
        if (length(mode) != 1) {
            mode = mode[1]
        }
        return(c(prob, mode))
    }
    postcal <- t(apply(X = mat.label, FUN = comptlab, MARGIN = 1, 
        npopmax = npopmax))
    proplab <- postcal[, 1:npopmax]
    postmode.indiv <- postcal[, npopmax + 1]
    cl <- as.numeric(levels(factor(postmode.indiv)))
    K.est <- length(cl)
    effpop.est <- NULL
    for (j in 1:npopmax) {
        effpop.est <- c(effpop.est, length(which(postmode.indiv == 
            j)))
    }
    postK <- function(config) {
        cl <- as.numeric(levels(factor(config)))
        K <- length(cl)
        return(K)
    }
    K.distrib <- apply(X = mat.label, FUN = postK, MARGIN = 2)
    comptK <- NULL
    modal <- as.numeric(levels(factor(K.distrib)))
    nb <- length(modal)
    for (i in 1:nb) {
        comptK = c(comptK, length(which(K.distrib == modal[i])))
    }
    modK <- which(comptK == max(comptK))
    if (length(modK) != 1) {
        modK = modK[1]
    }
    modeK = modal[modK]
    if (plot == TRUE) {
        if (is.null(popmbrship)) {
            X11()
            plot(coordinates, col = postmode.indiv, pch = 20, 
                main = "Posterior modal classes", xlab = "", 
                ylab = "")
            for (i in 1:K.est) {
                X11()
                prob <- as.image(Z = proplab[, cl[i]], x = coordinates, 
                  na.rm = FALSE)
                image.plot(prob, xlab = "", ylab = "", main = paste("Posterior probabilities of population ", 
                  cl[i], " membership", sep = ""))
            }
            X11()
            hist(K.distrib, freq = TRUE, col = "grey", main = "Posterior distribution of the number of populations", 
                xlab = FALSE, ylab = FALSE)
        }
        if (is.null(popmbrship) == FALSE) {
            X11()
            graph <- matrix(1:2, nr = 1, ncol = 2)
            layout(graph)
            plot(coordinates, col = postmode.indiv, pch = 20, 
                main = "Posterior modal classes", xlab = "", 
                ylab = "")
            plot(coordinates, col = popmbrship, pch = 20, main = "True classes", 
                xlab = "", ylab = "")
            for (i in 1:K.est) {
                X11()
                prob <- as.image(Z = proplab[, cl[i]], x = coordinates, 
                  na.rm = FALSE)
                image.plot(prob, xlab = "", ylab = "", main = paste("Posterior probabilities of population ", 
                  cl[i], " membership", sep = ""))
            }
            X11()
            hist(K.distrib, freq = TRUE, col = "grey", main = "Posterior distribution of the number of populations", 
                xlab = "", ylab = "")
        }
    }
    if (print == TRUE) {
        if (is.null(popmbrship)) {
            postscript(paste(file, "Posteriormode.ps", sep = ""))
            plot(coordinates, col = postmode.indiv, pch = 20, 
                main = "Posterior modal classes", xlab = "", 
                ylab = "")
            dev.off()
            postscript(paste(file, "Posteriorprob.ps", sep = ""))
            cl <- as.numeric(levels(factor(postmode.indiv)))
            for (i in 1:K.est) {
                prob <- as.image(Z = proplab[, cl[i]], x = coordinates, 
                  na.rm = FALSE)
                image.plot(prob, xlab = "", ylab = "", main = paste("Posterior probabilities of population ", 
                  cl[i], " membership", sep = ""))
            }
            dev.off()
            postscript(paste(file, "PosteriorK.ps", sep = ""))
            hist(K.distrib, freq = TRUE, col = "grey", main = "Posterior distribution of the number of populations", 
                xlab = "", ylab = "")
            dev.off()
        }
        if (is.null(popmbrship) == FALSE) {
            postscript(paste(file, "Posteriormode.ps", sep = ""))
            graph <- matrix(1:2, nr = 1, ncol = 2)
            layout(graph)
            plot(coordinates, col = postmode.indiv, pch = 20, 
                main = "Posterior modal classes", xlab = "", 
                ylab = "")
            plot(coordinates, col = popmbrship, pch = 20, main = "True classes", 
                xlab = "", ylab = "")
            dev.off()
            postscript(paste(file, "Posteriorprob.ps", sep = ""))
            for (i in 1:K.est) {
                prob <- as.image(Z = proplab[, cl[i]], x = coordinates, 
                  na.rm = FALSE)
                image.plot(prob, xlab = "", ylab = "", main = paste("Posterior probabilities of population ", 
                  cl[i], " membership", sep = ""))
            }
            dev.off()
            postscript(paste(file, "PosteriorK.ps", sep = ""))
            hist(K.distrib, freq = TRUE, col = "grey", main = "Posterior distribution of the number of populations", 
                xlab = "", ylab = "")
            dev.off()
        }
    }
    if (write == TRUE) {
        write(postmode.indiv, file = paste(path.mcmc, "postmode.indiv.txt", 
            sep = ""))
        write(effpop.est, file = paste(path.mcmc, "effpop.txt", 
            sep = ""))
        write(K.distrib, file = paste(path.mcmc, "postK.txt", 
            sep = ""))
        write.table(proplab, file = paste(path.mcmc, "proplab.txt", 
            sep = ""), col.names = FALSE, row.names = FALSE)
    }
    if (is.null(popmbrship) == FALSE) {
        trueclassif = outer(popmbrship, popmbrship, FUN = "==")
        estclassif = outer(postmode.indiv, postmode.indiv, FUN = "==")
        errclassif = length(which(estclassif != trueclassif))/2
        errclassif = errclassif/npair
        return(list(postmode.indiv = postmode.indiv, effpop.est = effpop.est, 
            proplab = proplab, K.est = K.est, K.distrib = K.distrib, 
            modeK = modeK, errclassif = errclassif))
    }
    else {
        return(list(postmode.indiv = postmode.indiv, effpop.est = effpop.est, 
            proplab = proplab, K.est = K.est, K.distrib = K.distrib, 
            modeK = modeK))
    }
}
