tablecst <-
function (pathtable, npopmax, coordinates, matngh, stepval = 0.02, 
    nit.table = 20000, stepw.table = 10, burnin.table = 10000, 
    plot = TRUE, write = FALSE) 
{
    psimax <- 1
    nindiv <- nrow(coordinates)
    tabpsi <- seq(0, psimax, 0.1)
    tabval <- seq(0, psimax, stepval)
    n <- length(tabpsi)
    m <- length(tabval)
    espeng <- numeric(m)
    for (ival in 1:m) {
        psi <- tabval[ival]
        message <- cat(paste("psi=", psi, "\n"))
        file <- paste(pathtable, ival, "/", sep = "")
        dir.create(file, showWarnings = FALSE)
        sim <- simpottsdir(path = file, coordinates = coordinates, 
            matngh = matngh, npop = npopmax, psi = psi, nchain = nit.table, 
            burnin = burnin.table, stepw = stepw.table, plot = FALSE, 
            ploth = FALSE, print = FALSE, file = file)
        energy <- as.numeric(scan(paste(file, "energy.txt", sep = "")))
        espeng[ival] <- mean(energy, na.rm = TRUE)
    }
    approx <- function(psi, tabval, espeng, m, nindiv, npopmax) {
        int <- 0
        ierr <- 1
        integ <- .Fortran(name = "avint", PACKAGE = "Geneclust", 
            as.single(tabval), as.single(espeng), as.integer(m), 
            as.single(0), as.single(psi), as.single(int), as.integer(ierr))
        return(nindiv * log(npopmax) + integ[[6]])
    }
    Z <- as.numeric(lapply(X = tabpsi, FUN = approx, tabval = tabval, 
        espeng = espeng, m = m, nindiv = nindiv, npopmax = npopmax))
    if (write == TRUE) {
        write(Z, file = paste(pathtable, "table.txt", sep = ""))
    }
    if (plot == TRUE) {
        plot(tabpsi, Z, pch = 20, col = "darkblue", main = "Potts-Dirichlet model constants table", 
            xlab = "psi ", ylab = "Normalization constants (log scale)")
    }
    return(list(table = Z))
}
