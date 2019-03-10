simpottsdir <-
function (path, coordinates, matngh = NULL, npop, psi, nchain = 50000, 
    burnin = 40000, stepw = 10, plot = TRUE, ploth = FALSE, print = FALSE, 
    file = path) 
{
    nindiv <- nrow(coordinates)
    if (is.null(matngh)) {
        del <- deldir(x = coordinates[, 1], y = coordinates[, 
            2])
        colngh <- del$delsgs[, 5:6]
        pt <- nrow(colngh)
        matngh <- matrix(0, nrow = nindiv, ncol = nindiv)
        for (i in 1:pt) {
            matngh[colngh[i, 1], colngh[i, 2]] = 1
            matngh[colngh[i, 2], colngh[i, 1]] = 1
        }
    }
    c <- sample(1:npop, nindiv, replace = TRUE)
    if (npop > 1) {
        ngh <- rep(0, times = nindiv)
        probcond <- rep(0, times = npop)
        tempmul <- rep(0, times = npop)
        filec <- paste(path, "simpotts.txt", sep = "")
        fileng <- paste(path, "energy.txt", sep = "")
        if (file.exists(filec) == TRUE) {
            file.remove(filec)
        }
        if (file.exists(fileng) == TRUE) {
            file.remove(fileng)
        }
        out.res <- .Fortran(name = "simpotts", PACKAGE = "Geneclust", 
            as.integer(nindiv), as.integer(npop), as.double(psi), 
            as.character(filec), as.character(fileng), as.integer(nchain), 
            as.integer(burnin), as.integer(stepw), as.integer(c), 
            as.integer(matngh), as.integer(ngh), as.single(probcond), 
            as.integer(tempmul))
        recup <- scan(paste(path, "simpotts.txt", sep = ""))
        data <- matrix(recup, nr = nindiv, byrow = FALSE)
        nconfig <- ncol(data)
        c <- data[, nconfig]
    }
    if (plot == TRUE) {
        X11()
        if (npop > 1) {
            title <- paste("A Potts-Dirichlet configuration with", 
                npop, "populations and psi=", psi, "\n")
        }
        if (npop == 1) {
            title <- paste("Only 1 population generated", "\n")
        }
        e <- setplot(xdata = coordinates[, 1], ydata = coordinates[, 
            2])
        par(pty = "m", pin = e[[4]])
        plot(coordinates, col = c, pch = 20, xlab = "", ylab = "", 
            main = title)
    }
    if (print == TRUE) {
        if (npop > 1) {
            title <- paste("A Potts-Dirichlet configuration with", 
                npop, "populations and psi=", psi, "\n")
        }
        if (npop == 1) {
            title <- paste("K=1", "\n")
        }
        postscript(paste(file, "Spatial config.ps"))
        e <- setplot(xdata = coordinates[, 1], ydata = coordinates[, 
            2])
        par(pty = "m", pin = e[[4]])
        plot(coordinates, col = c, pch = 20, xlab = "", ylab = "", 
            main = title)
        dev.off()
    }
    if (ploth == TRUE) {
        recup <- scan(paste(path, "simpotts.txt", sep = ""))
        energy <- scan(paste(path, "energy.txt", sep = ""))
        data <- matrix(recup, nr = nindiv, byrow = FALSE)
        meanconfig <- apply(data, FUN = mean, MARGIN = 2)
        nitstore <- ncol(data)
        X11()
        graph <- matrix(1:4, nrow = 2, ncol = 2)
        layout(graph)
        if (npop > 1) {
            title <- paste("A Potts-Dirichlet configuration with", 
                npop, "populations and psi=", psi, "\n")
        }
        if (npop == 1) {
            title <- paste("Only 1 population generated", "\n")
        }
        e <- setplot(xdata = coordinates[, 1], ydata = coordinates[, 
            2])
        par(pty = "m", pin = e[[4]])
        plot(coordinates, col = c, pch = 20, xlab = "", ylab = "", 
            main = title)
        hist(meanconfig, prob = TRUE, main = "", xlab = "Image mean")
        plot(c(1:nitstore), cumsum(meanconfig)/c(1:nitstore), 
            xlim = c(1, nitstore), ylim = c(1, npop), xlab = "Iteration", 
            ylab = "Average image mean", type = "l", col = "red")
        abline(h = 0.5, lty = 2)
        plot(c(1:nitstore), energy, xlim = c(1, nitstore), ylim = c(min(energy) - 
            10, max(energy) + 10), xlab = "Iteration", ylab = "Energy", 
            type = "l", col = "red")
    }
    return(list(matngh = matngh, popmbrship = c))
}
