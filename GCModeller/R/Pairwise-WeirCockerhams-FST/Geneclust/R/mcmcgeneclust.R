mcmcgeneclust <-
function (path.mcmc, genotypes, coordinates, npopmax, nit = 3000, 
    burnin = 200, thinning = 5, c = NULL, psi = 0, fis = NULL, 
    freq = NULL, tabcst = NULL, matngh = NULL, flat.prior.psi = TRUE, 
    stepval = 0.02, nit.table = 20000, burnin.table = 10000, 
    stepw.table = 10, alpha = 4, beta = 40, varpsi = TRUE, varfis = TRUE) 
{
    filec <- paste(path.mcmc, "chain.popmbrship.txt", sep = "")
    filepsi <- paste(path.mcmc, "chain.psi.txt", sep = "")
    filefis <- paste(path.mcmc, "chain.fis.txt", sep = "")
    fileloglik <- paste(path.mcmc, "loglikelihood.txt", sep = "")
    if (file.exists(filec) == TRUE) {
        file.remove(filec)
    }
    if (file.exists(filepsi) == TRUE) {
        file.remove(filepsi)
    }
    if (file.exists(filefis) == TRUE) {
        file.remove(filefis)
    }
    if (file.exists(fileloglik) == TRUE) {
        file.remove(fileloglik)
    }
    nindiv <- nrow(genotypes)
    psimax <- 1
    if (nrow(coordinates) != nindiv) {
        stop("The matrix 'coordinates' must have 'nindiv' rows!")
    }
    if (is.null(fis) == FALSE) {
        if (length(fis) != npopmax) {
            stop(cat(paste("The number of components of the vector 'fis' must be the same as the number of populations 'npop'!")))
        }
    }
    if ((nit%%thinning) != 0) {
        stop("nit/thinning is not an integer")
    }
    tabpsi <- seq(0, psimax, 0.1)
    numpsi <- length(tabpsi)
    if (flat.prior.psi == TRUE) {
        nvalpsi <- (psimax/0.1) + 1
        priorpsi <- rep(1/nvalpsi, nvalpsi)
    }
    if (flat.prior.psi == FALSE) {
        nvalpsi <- (psimax/0.1) + 1
        t <- pbeta(tabpsi, 19.97143, 13.31429)
        priorpsi <- numeric(numpsi)
        end <- numpsi - 1
        for (i in 1:end) {
            priorpsi[i] <- t[i + 1] - t[i]
        }
        priorpsi <- priorpsi/sum(priorpsi)
    }
    data.tmp <- FormatGenotypes(as.matrix(genotypes))
    coordinates <- as.matrix(coordinates)
    genotypes <- data.tmp$genotypes
    nall <- data.tmp$allele.numbers
    nloc <- length(nall)
    nallmax <- max(nall)
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
    if (is.null(matngh) == FALSE) {
        if (nrow(matngh) != nindiv | ncol(matngh) != nindiv) {
            stop("The matrix 'matngh' must have 'nindiv' rows and 'nindiv' columns!")
        }
    }
    if (is.null(tabcst) & varpsi == TRUE) {
        pathtable <- paste(path.mcmc, "Table/", sep = "")
        dir.create(pathtable, showWarnings = FALSE)
        message <- cat(paste("Computes the Potts-Dirichlet model normalization constants", 
            "\n"))
        cst <- tablecst(pathtable = pathtable, npopmax = npopmax, 
            coordinates = coordinates, matngh = matngh, stepval = stepval, 
            nit.table = nit.table, stepw.table = stepw.table, 
            burnin.table = burnin.table, plot = FALSE, write = TRUE)
        tabcst <- as.numeric(cst$table)
    }
    if (is.null(tabcst) & varpsi == FALSE) {
        tabcst <- rep(0, times = 11)
    }
    if (is.null(tabcst) == FALSE) {
        if (length(tabcst) != length(tabpsi)) {
            stop("The 'tabcst' dimension is not suitable!")
        }
    }
    if (is.null(c)) {
        c <- sample(1:npopmax, nindiv, replace = TRUE)
    }
    ctemp <- rep(x = -999, times = nindiv)
    if (is.null(freq)) {
        f <- array(dim = c(npopmax, nloc, nallmax), data = -999)
        for (ipop in 1:npopmax) {
            for (iloc in 1:nloc) {
                d <- rexp(n = nall[iloc], rate = 1)
                f[ipop, iloc, 1:nall[iloc]] <- d/sum(d)
            }
        }
    }
    else {
        f <- array(dim = c(npopmax, nloc, nallmax), data = -999)
        for (ipop in 1:npopmax) {
            for (iloc in 1:nloc) {
                f[ipop, iloc, 1:nall[iloc]] <- freq[ipop, iloc, 
                  1:nall[iloc]]
            }
        }
    }
    ftemp <- array(dim = c(npopmax, nloc, nallmax), data = -999)
    if (is.null(psi)) {
        psi <- sample(x = tabpsi, size = 1, prob = priorpsi)
    }
    if (is.null(fis)) {
        fis <- rbeta(npopmax, alpha, beta)
    }
    fistemp <- rep(times = npopmax, -999)
    nghup <- rep(times = nindiv, -999)
    ngh <- rep(times = nindiv, -999)
    out.res <- .Fortran(name = "mcmcgenecl", PACKAGE = "Geneclust", 
        as.double(coordinates), as.integer(genotypes), as.integer(nall), 
        as.character(filec), as.character(filepsi), as.character(filefis), 
        as.character(fileloglik), as.integer(nit), as.integer(burnin), 
        as.integer(thinning), as.integer(nindiv), as.integer(nloc), 
        as.integer(nloc * 2), as.integer(nallmax), as.integer(npopmax), 
        as.double(fis), as.double(fistemp), as.double(alpha), 
        as.double(beta), as.integer(c), as.integer(ctemp), as.double(f), 
        as.double(ftemp), as.double(psi), as.double(psimax), 
        as.integer(matngh), as.integer(nghup), as.integer(ngh), 
        as.integer(numpsi), as.double(tabcst), as.double(priorpsi), 
        as.integer(varpsi), as.integer(varfis))
    config.cur <- out.res[[20]]
    psi.cur <- out.res[[24]]
    fis.cur <- out.res[[16]]
    freq.cur <- out.res[[22]]
    f.cur <- array(data = freq.cur, dim = c(npopmax, nloc, nallmax))
    param <- c(paste("npopmax :", npopmax), paste("nindiv :", 
        nindiv), paste("nloc :", nloc), paste("nit :", nit), 
        paste("burnin :", burnin), paste("thinning :", thinning), 
        paste("flat.prior.psi :", flat.prior.psi), paste("psimax :", 
            psimax), paste("stepval :", stepval), paste("nit.table :", 
            nit.table), paste("burnin.table :", burnin.table), 
        paste("stepw.table :", stepw.table), paste("alpha :", 
            alpha), paste("beta :", beta), paste("varpsi :", 
            varpsi), paste("varfis :", varfis))
    write.table(param, file = paste(path.mcmc, "paraminfer.txt", 
        sep = ""), quote = FALSE, row.name = FALSE, col.name = FALSE)
    return(list(c = config.cur, psi = psi.cur, fis = fis.cur, 
        freq = f.cur, matngh = matngh, table = tabcst))
}
