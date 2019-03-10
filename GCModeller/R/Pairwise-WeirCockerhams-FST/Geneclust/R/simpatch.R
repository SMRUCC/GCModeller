simpatch <-
function (path, nindiv, coordinates = NULL, coord.lim = c(0, 
    1, 0, 1), npop, nall, psi, fis, nchain = 50000, burnin = 40000, 
    stepw = 100, seed = NULL, plot = TRUE, write = FALSE, print = FALSE, 
    file = path) 
{
    if (!is.null(seed)) {
        set.seed(seed)
    }
    if (is.null(coordinates) == FALSE) {
        if (nrow(coordinates) != nindiv | ncol(coordinates) != 
            2) 
            stop(cat(paste("The dimensions of the matrix coordinates are not suitable!")))
    }
    if (is.null(coordinates)) {
        coordinates <- cbind(runif(nindiv, coord.lim[1], coord.lim[2]), 
            runif(nindiv, coord.lim[3], coord.lim[4]))
    }
    coordinates <- as.matrix(coordinates)
    if (length(fis) != npop) {
        stop(cat(paste("The components number of the vector 'fis' must be the same as the number of populations 'npop'!")))
    }
    nloc <- length(nall)
    sim <- simpottsdir(path = path, coordinates = coordinates, 
        matngh = NULL, npop = npop, psi = psi, nchain = nchain, 
        burnin = burnin, stepw = stepw, plot = plot, ploth = FALSE, 
        print = print, file = file)
    matngh <- as.matrix(sim$matngh)
    popmbrship <- as.numeric(sim$popmbrship)
    freq <- array(dim = c(npop, nloc, max(nall)), data = -999)
    for (iclass in 1:npop) {
        for (iloc in 1:nloc) {
            freq[iclass, iloc, 1:nall[iloc]] <- rexp(n = nall[iloc], 
                rate = 1)
            freq[iclass, iloc, 1:nall[iloc]] <- freq[iclass, 
                iloc, 1:nall[iloc]]/sum(freq[iclass, iloc, 1:nall[iloc]])
        }
    }
    if (npop > 1) {
        if (freq[1, 1, 1] < freq[2, 1, 1]) {
            tmp <- freq[2, , ]
            freq[2, , ] <- freq[1, , ]
            freq[1, , ] <- tmp
        }
    }
    z <- matrix(nr = nindiv, nc = nloc * 2)
    prob <- function(all, loc, class, fis, freq) {
        if (all[1] == all[2]) {
            p <- freq[class, loc, all[1]]^2 + fis[class] * freq[class, 
                loc, all[1]]
        }
        if (all[1] != all[2]) {
            p <- freq[class, loc, all[1]] * freq[class, loc, 
                all[2]] * (1 - fis[class])
        }
        return(p)
    }
    for (iclass in 1:npop) {
        for (iloc in 1:nloc) {
            pairs <- as.matrix(expand.grid(h = c(1:nall[iloc]), 
                w = c(1:nall[iloc])))
            npairs <- nrow(pairs)
            pairs <- matrix(pairs, nrow = npairs, ncol = 2, byrow = FALSE)
            probpairs <- as.numeric(apply(X = pairs, MARGIN = 1, 
                FUN = prob, loc = iloc, class = iclass, fis = fis, 
                freq = freq))
            for (iindiv in (1:nindiv)[popmbrship == iclass]) {
                z[iindiv, (2 * iloc - 1):(2 * iloc)] <- pairs[sample(x = npairs, 
                  size = 1, prob = probpairs), ]
            }
        }
    }
    if (plot == TRUE) {
        for (iclass in 1:npop) {
            X11()
            par(mfrow = c(floor(sqrt(nloc) + 1), floor(sqrt(nloc))))
            for (iloc in 1:nloc) {
                hist(c(z[popmbrship == iclass, 2 * (iloc - 1) + 
                  1], z[popmbrship == iclass, 2 * (iloc - 1) + 
                  2]), breaks = seq(0.5, nall[iloc] + 0.5, 1), 
                  freq = FALSE, main = paste("Histogram of freq. in pop.", 
                    iclass, ", locus", iloc), xlab = "", ylim = c(0, 
                    1), axes = FALSE)
                points(1:nall[iloc], freq[iclass, iloc, 1:nall[iloc]], 
                  type = "h", col = 2)
            }
        }
    }
    if (print == TRUE) {
        postscript(paste(file, "Frequencies.ps", sep = ""))
        for (iclass in 1:npop) {
            par(mfrow = c(floor(sqrt(nloc) + 1), floor(sqrt(nloc))))
            for (iloc in 1:nloc) {
                hist(c(z[popmbrship == iclass, 2 * (iloc - 1) + 
                  1], z[popmbrship == iclass, 2 * (iloc - 1) + 
                  2]), breaks = seq(0.5, nall[iloc] + 0.5, 1), 
                  freq = FALSE, main = paste("Histogram of freq. in pop.", 
                    iclass, ", locus", iloc), xlab = "", ylim = c(0, 
                    1), axes = FALSE)
                points(1:nall[iloc], freq[iclass, iloc, 1:nall[iloc]], 
                  type = "h", col = 2)
            }
        }
        dev.off()
    }
    if (write == TRUE) {
        write.table(coordinates, file = paste(file, "coordinates.txt", 
            sep = ""), col.names = FALSE, row.names = FALSE)
        write.table(z, file = paste(file, "genotypes.txt", sep = ""), 
            col.names = FALSE, row.names = FALSE)
        write(popmbrship, file = paste(file, "pop.mbrship.txt", 
            sep = ""))
        write.table(matngh, file = paste(file, "matngh.txt", 
            sep = ""), col.names = FALSE, row.names = FALSE)
        write(nall, file = paste(file, "nall.txt", sep = ""))
    }
    param <- c(paste("Number of individuals:", nindiv), paste("Number of populations:", 
        npop), paste("Number of loci:", nloc))
    for (iloc in 1:nloc) {
        param <- c(param, paste("Number of allele versions at locus", 
            iloc, ":", nall[iloc]))
    }
    for (ipop in 1:npop) {
        param <- c(param, paste("Inbreeding coefficient for population", 
            ipop, ":", fis[ipop]))
    }
    param <- c(param, paste("Spatial interaction parameter psi :", 
        psi), paste("Number of Gibbs iterations:", nchain), paste("Burnin:", 
        burnin), paste("Step:", stepw))
    write.table(param, file = paste(file, "paramsimu.txt", sep = ""), 
        quote = FALSE, row.name = FALSE, col.name = FALSE)
    return(list(coordinates = coordinates, matngh = matngh, popmbrship = popmbrship, 
        genotypes = z, frequencies = freq))
}
