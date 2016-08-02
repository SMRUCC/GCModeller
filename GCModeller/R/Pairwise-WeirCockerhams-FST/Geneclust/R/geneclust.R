geneclust <-
function (project.name = "Data", data, npopmax = 3, psi = 0.5, 
    nit = 1000, burnin = 10, thinning = 1, c = NULL, freq = NULL, 
    tabcst = NULL, matngh = NULL, fis = rep(0, npopmax), varpsi = FALSE, 
    varfis = FALSE, otherconfig = NULL, write = FALSE) 
{
    if (class(data)[1] != "geneclustdata") {
        stop("invalid data: object of class geneclustdata expected")
    }
    system(paste("mkdir", project.name, sep = " "))
    path.mcmc <- paste("./", project.name, "/", sep = "")
    d <- dim(data)[2]
    obj.run <- mcmcgeneclust(path.mcmc, genotypes = data[, 3:d], 
        coordinates = data[, 1:2], npopmax = npopmax, nit = nit, 
        burnin = burnin, thinning = thinning, c = c, psi = psi, 
        fis = fis, freq = freq, tabcst = tabcst, matngh = matngh, 
        flat.prior.psi = TRUE, stepval = 0.02, nit.table = 20000, 
        burnin.table = 10000, stepw.table = 10, alpha = 4, beta = 40, 
        varpsi, varfis)
    obj <- postclassif(path.mcmc = path.mcmc, coordinates = data[, 
        1:2], popmbrship = otherconfig, plot = FALSE, file = path.mcmc, 
        write = write)
    if (is.null(otherconfig) == TRUE) {
        if (varpsi == FALSE & varfis == FALSE) {
            infer <- list(prob = obj$proplab, membership = obj$postmode.indiv, 
                K = length(unique(obj$postmode.indiv)), coord = data[, 
                  1:2], path = path.mcmc, c = obj.run$c, psi = obj.run$psi, 
                fis = obj.run$fis, freq = obj.run$freq, tabcst = obj.run$table, 
                matngh = obj.run$matngh)
            class(infer) <- "geneclust"
        }
        if (varpsi == FALSE & varfis == TRUE) {
            inbreedcoeff <- postfis(path.mcmc = path.mcmc, postmode.indiv = obj$postmode.indiv, 
                probs = c(0.025, 0.25, 0.5, 0.75, 0.975), plot = FALSE, 
                print = FALSE, file = path.mcmc)
            infer <- list(prob = obj$proplab, membership = obj$postmode.indiv, 
                K = length(unique(obj$postmode.indiv)), postmeanfis = inbreedcoeff$postmean.fis, 
                postquantfis = inbreedcoeff$quant.fis, coord = data[, 
                  1:2], path = path.mcmc, c = obj.run$c, psi = obj.run$psi, 
                fis = obj.run$fis, freq = obj.run$freq, tabcst = obj.run$table, 
                matngh = obj.run$matngh)
            class(infer) <- "geneclust"
        }
        if (varpsi == TRUE & varfis == FALSE) {
            interparam <- postpsi(path.mcmc = path.mcmc, plot = FALSE, 
                print = TRUE, file = path.mcmc)
            infer <- list(prob = obj$proplab, membership = obj$postmode.indiv, 
                K = length(unique(obj$postmode.indiv)), postmodepsi = interparam$postmode.psi, 
                coord = data[, 1:2], path = path.mcmc, c = obj.run$c, 
                psi = obj.run$psi, fis = obj.run$fis, freq = obj.run$freq, 
                tabcst = obj.run$table, matngh = obj.run$matngh)
            class(infer) <- "geneclust"
        }
        if (varpsi == TRUE & varfis == TRUE) {
            inbreedcoeff <- postfis(path.mcmc = path.mcmc, postmode.indiv = obj$postmode.indiv, 
                probs = c(0.025, 0.25, 0.5, 0.75, 0.975), plot = FALSE, 
                print = FALSE, file = path.mcmc)
            interparam <- postpsi(path.mcmc = path.mcmc, plot = FALSE, 
                print = FALSE, file = path.mcmc)
            infer <- list(prob = obj$proplab, membership = obj$postmode.indiv, 
                K = length(unique(obj$postmode.indiv)), postmodepsi = interparam$postmode.psi, 
                postmeanfis = inbreedcoeff$postmean.fis, postquantfis = inbreedcoeff$quant.fis, 
                coord = data[, 1:2], path = path.mcmc, c = obj.run$c, 
                psi = obj.run$psi, fis = obj.run$fis, freq = obj.run$freq, 
                tabcst = obj.run$table, matngh = obj.run$matngh)
            class(infer) <- "geneclust"
        }
    }
    if (is.null(otherconfig) == FALSE) {
        if (varpsi == FALSE & varfis == FALSE) {
            infer <- list(prob = obj$proplab, membership = obj$postmode.indiv, 
                K = length(unique(obj$postmode.indiv)), diffclassif = obj$errclassif, 
                coord = data[, 1:2], path = path.mcmc, c = obj.run$c, 
                psi = obj.run$psi, fis = obj.run$fis, freq = obj.run$freq, 
                tabcst = obj.run$table, matngh = obj.run$matngh)
            class(infer) <- "geneclust"
        }
        if (varpsi == FALSE & varfis == TRUE) {
            inbreedcoeff <- postfis(path.mcmc = path.mcmc, postmode.indiv = obj$postmode.indiv, 
                probs = c(0.025, 0.25, 0.5, 0.75, 0.975), plot = FALSE, 
                print = FALSE, file = path.mcmc)
            infer <- list(prob = obj$proplab, membership = obj$postmode.indiv, 
                K = length(unique(obj$postmode.indiv)), postmeanfis = inbreedcoeff$postmean.fis, 
                postquantfis = inbreedcoeff$quant.fis, diffclassif = obj$errclassif, 
                coord = data[, 1:2], path = path.mcmc, c = obj.run$c, 
                psi = obj.run$psi, fis = obj.run$fis, freq = obj.run$freq, 
                tabcst = obj.run$table, matngh = obj.run$matngh)
            class(infer) <- "geneclust"
        }
        if (varpsi == TRUE & varfis == FALSE) {
            interparam <- postpsi(path.mcmc = path.mcmc, plot = FALSE, 
                print = FALSE, file = path.mcmc)
            infer <- list(prob = obj$proplab, membership = obj$postmode.indiv, 
                K = length(unique(obj$postmode.indiv)), postmodepsi = interparam$postmode.psi, 
                diffclassif = obj$errclassif, coord = data[, 
                  1:2], path = path.mcmc, c = obj.run$c, psi = obj.run$psi, 
                fis = obj.run$fis, freq = obj.run$freq, tabcst = obj.run$table, 
                matngh = obj.run$matngh)
            class(infer) <- "geneclust"
        }
        if (varpsi == TRUE & varfis == TRUE) {
            inbreedcoeff <- postfis(path.mcmc = path.mcmc, postmode.indiv = obj$postmode.indiv, 
                probs = c(0.025, 0.25, 0.5, 0.75, 0.975), plot = FALSE, 
                print = FALSE, file = path.mcmc)
            interparam <- postpsi(path.mcmc = path.mcmc, plot = FALSE, 
                print = FALSE, file = path.mcmc)
            infer <- list(prob = obj$proplab, membership = obj$postmode.indiv, 
                K = length(unique(obj$postmode.indiv)), postmodepsi = interparam$postmode.psi, 
                postmeanfis = inbreedcoeff$postmean.fis, postquantfis = inbreedcoeff$quant.fis, 
                diffclassif = obj$errclassif, coord = data[, 
                  1:2], path = path.mcmc, c = obj.run$c, psi = obj.run$psi, 
                fis = obj.run$fis, freq = obj.run$freq, tabcst = obj.run$table, 
                matngh = obj.run$matngh)
            class(infer) <- "geneclust"
        }
    }
    return(infer)
}
