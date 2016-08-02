FormatGenotypes <-
function (genotypes) 
{
    formatted <- genotypes
    nloc <- ncol(genotypes)/2
    nall <- numeric(nloc)
    for (iloc in 1:nloc) {
        ens <- sort(unique(c(genotypes[, 2 * iloc - 1], genotypes[, 
            2 * iloc])))
        max.ens <- length(ens)
        for (il in 1:(dim(genotypes)[1])) {
            formatted[il, 2 * iloc - 1] <- ifelse(is.na(genotypes[il, 
                2 * iloc - 1]), NA, (1:max.ens)[genotypes[il, 
                2 * iloc - 1] == ens])
            formatted[il, 2 * iloc] <- ifelse(is.na(genotypes[il, 
                2 * iloc]), NA, (1:max.ens)[genotypes[il, 2 * 
                iloc] == ens])
        }
        nall[iloc] <- max.ens
    }
    genotypes <- as.matrix(genotypes)
    genotypes[is.na(genotypes)] <- -999
    list(nloc = nloc, allele.numbers = nall, genotypes = formatted)
}
