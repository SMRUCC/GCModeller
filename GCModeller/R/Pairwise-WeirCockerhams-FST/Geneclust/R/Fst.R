Fst <-
function (genotypes, allele.numbers, pop.mbrship, npopmax) 
{
    nindiv <- nrow(genotypes)
    Fis = matrix(nr = npopmax, nc = npopmax, 0)
    Fst = matrix(nr = npopmax, nc = npopmax, 0)
    Fit = matrix(nr = npopmax, nc = npopmax, 0)
    if (npopmax > 1) {
        for (iclass1 in 1:(npopmax - 1)) {
            for (iclass2 in (iclass1 + 1):npopmax) {
                sub1 <- pop.mbrship == iclass1
                sub2 <- pop.mbrship == iclass2
                if ((sum(sub1) != 0) & (sum(sub2) != 0)) {
                  ztmp <- genotypes[sub1 | sub2, ]
                  nindivtmp <- nrow(ztmp)
                  pop.mbrshiptmp <- pop.mbrship[sub1 | sub2]
                  pop.mbrshiptmp[pop.mbrshiptmp == iclass1] <- 1
                  pop.mbrshiptmp[pop.mbrshiptmp == iclass2] <- 2
                  tabindiv <- matrix(nr = nindivtmp, nc = 2, 
                    data = -999)
                  kk <- numeric(2)
                  effcl <- table(pop.mbrshiptmp)
                  nb.nuclei.max <- nindivtmp
                  nloc <- length(allele.numbers)
                  nloc2 <- 2 * nloc
                  Fistmp <- Fsttmp <- Fittmp <- -999
                  res <- .Fortran(name = "fstae", PACKAGE = "Geneclust", 
                    as.integer(nindivtmp), as.integer(nb.nuclei.max), 
                    as.integer(nloc), as.integer(nloc2), as.integer(allele.numbers), 
                    as.integer(2), as.integer(effcl), as.integer(ztmp), 
                    as.integer(pop.mbrshiptmp), as.integer(tabindiv), 
                    as.integer(kk), as.double(Fistmp), as.double(Fsttmp), 
                    as.double(Fittmp))
                  Fis[iclass1, iclass2] <- res[[12]][1]
                  Fis[iclass2, iclass1] <- res[[12]][1]
                  Fst[iclass1, iclass2] <- res[[13]][1]
                  Fst[iclass2, iclass1] <- res[[13]][1]
                  Fit[iclass1, iclass2] <- res[[14]][1]
                  Fit[iclass2, iclass1] <- res[[14]][1]
                }
            }
        }
    }
    list(Pairwise.Fis = Fis, Pairwise.Fst = Fst, Pairwise.Fit = Fit)
}
