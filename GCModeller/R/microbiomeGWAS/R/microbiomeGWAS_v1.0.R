###########################################################################

#funED

funED <- function(distMat, nPerm = 1e6, nPermEach = 1e4, soFile1 = "dExp1.so", soFile2 = "dExp2.so"){
	dyn.load(soFile1)
	dyn.load(soFile2)
	nSample <- nrow(distMat)
	out <- .C("dExp1", nSample = as.integer(nSample), distVec = as.vector(distMat), nD = 23L, eD = rep(0.1, 23L))
	eD1 <- out$eD
	nPermGroup <- ceiling(nPerm / nPermEach)
	eD2 <- rep(0, 23)
	for(i in 1:nPermGroup){
		idxMat <- matrix(NA, nSample, nPermEach)
		for(s in 1:nPermEach)idxMat[, s] <- sample(nSample)
		out <- .C("dExp2", nSample = as.integer(nSample), distVec = as.vector(distMat), nPerm = as.integer(nPermEach), idxMat = as.vector(idxMat - 1L), nD = 23L, eD = rep(0.1, 23L))
		eD2 <- eD2 + out$eD
	}
	eD2 <- eD2 / nPermGroup
	eD <- eD2;
	eD[c(1:6, 9:12)] <- eD1[c(1:6, 9:12)]
	names(eD) <- c("eDij", "eDij2", "eDijDik", "eDij3", "eDij2Dik", "eDijDjkDik", "eDijDjkDkl", "eDijDikDil", "eDij4", "eDij3Dik", "eDij2Dik2", "eDij2DjkDik", "eDij2DjkDkl", "eDijDjk2Dkl", "eDij2DikDil", "eDijDjkDikDil", "eDijDjkDklDil", "eDij2Dkl2", "eDijDjkDklDlm", "eDijDikDilDim", "eDijDikDilDlm", "eDijDikDlm2", "eDijDikDlmDln")
	return(eD)
}

###########################################################################

#funEG

funEG <- function(f){
	if(is.vector(f)){
		tempLevel <- unique(f)
		p0 <- (1 - tempLevel)^2
		p1 <- 2 * tempLevel * (1 - tempLevel)
		tempIdx <- as.integer(factor(f, levels = tempLevel))
	}else if(is.matrix(f) && ncol(f) == 2){
		tempPaste <- paste(f[, 1], f[, 2])
		tempIdx <- which(!duplicated(tempPaste))
		tempLevel <- tempPaste[tempIdx]
		p0 <- f[tempIdx, 1]
		p1 <- f[tempIdx, 2]
		tempIdx <- as.integer(factor(tempPaste, levels = tempLevel))
	}else stop("f must be a vector or a matrix with 2 columns!")
	p2 <- 1 - (p0 + p1)
	pMat <- cbind(p0, p1, p2)
	pMat[pMat < 0 & pMat > -1e-10] <- 0
	pMat[pMat > 1 & pMat - 1 < 1e-10] <- 1
	if(any(pMat < 0) || any(pMat > 1))stop("Invalid value of f")
	eGij <- eGij2 <- eGijGik <- eGij3 <- eGij2Gik <- eGijGjkGik <- eGijGjkGkl <- eGijGikGil <- eGij4 <- eGij3Gik <- eGij2Gik2 <- eGij2GjkGik <- eGij2GjkGkl <- eGijGjk2Gkl <- eGij2GikGil <- eGijGjkGikGil <- eGijGjkGklGil <- eGijGjkGklGlm <- eGijGikGilGim <- eGijGikGilGlm <- 0
	for(i in 0:2){
		for(j in 0:2){
			Pij <- pMat[, i + 1] * pMat[, j + 1]
			eGij <- eGij + Pij * abs(i - j)
			eGij2 <- eGij2 + Pij * abs(i - j)^2
			eGij3 <- eGij3 + Pij * abs(i - j)^3
			eGij4 <- eGij4 + Pij * abs(i - j)^4
			for(k in 0:2){
				Pijk <- Pij * pMat[, k + 1]
				eGijGik <- eGijGik + Pijk * abs(i - j) * abs(i - k)
				eGij2Gik <- eGij2Gik + Pijk * abs(i - j)^2 * abs(i - k)
				eGijGjkGik <- eGijGjkGik + Pijk * abs(i - j) * abs(j - k) * abs(i - k)
				eGij3Gik <- eGij3Gik + Pijk * abs(i - j)^3 * abs(i - k)
				eGij2Gik2 <- eGij2Gik2 + Pijk * abs(i - j)^2 * abs(i - k)^2
				eGij2GjkGik <- eGij2GjkGik + Pijk * abs(i - j)^2 * abs(j - k) * abs(i - k)
				for(l in 0:2){
					Pijkl <- Pijk * pMat[, l + 1]
					eGijGjkGkl <- eGijGjkGkl + Pijkl * abs(i - j) * abs(j - k) * abs(k - l)
					eGijGikGil <- eGijGikGil + Pijkl * abs(i - j) * abs(i - k) * abs(i - l)
					eGij2GjkGkl <- eGij2GjkGkl + Pijkl * abs(i - j)^2 * abs(j - k) * abs(k - l)
					eGijGjk2Gkl <- eGijGjk2Gkl + Pijkl * abs(i - j) * abs(j - k)^2 * abs(k - l)
					eGij2GikGil <- eGij2GikGil + Pijkl * abs(i - j)^2 * abs(i - k) * abs(i - l)
					eGijGjkGikGil <- eGijGjkGikGil + Pijkl * abs(i - j) * abs(j - k) * abs(i - k) * abs(i - l)
					eGijGjkGklGil <- eGijGjkGklGil + Pijkl * abs(i - j) * abs(j - k) * abs(k - l) * abs(i - l)
					for(m in 0:2){
						Pijklm <- Pijkl * pMat[, m + 1]
						eGijGjkGklGlm <- eGijGjkGklGlm + Pijklm * abs(i - j) * abs(j - k) * abs(k - l) * abs(l - m)
						eGijGikGilGim <- eGijGikGilGim + Pijklm * abs(i - j) * abs(i - k) * abs(i - l) * abs(i - m)
						eGijGikGilGlm <- eGijGikGilGlm + Pijklm * abs(i - j) * abs(i - k) * abs(i - l) * abs(l - m)
					}
				}
			}
		}
	}
	eG <- cbind(eGij = eGij, 
	eGij2 = eGij2, 
	eGijGik = eGijGik, 
	eGij3 = eGij3, 
	eGij2Gik = eGij2Gik, 
	eGijGjkGik = eGijGjkGik, 
	eGijGjkGkl = eGijGjkGkl, 
	eGijGikGil = eGijGikGil, 
	eGij4 = eGij4, 
	eGij3Gik = eGij3Gik, 
	eGij2Gik2 = eGij2Gik2,
	eGij2GjkGik = eGij2GjkGik, 
	eGij2GjkGkl = eGij2GjkGkl, 
	eGijGjk2Gkl = eGijGjk2Gkl, 
	eGij2GikGil = eGij2GikGil, 
	eGijGjkGikGil = eGijGjkGikGil, 
	eGijGjkGklGil = eGijGjkGklGil, 
	eGij2Gkl2 = eGij2^2, 
	eGijGjkGklGlm = eGijGjkGklGlm, 
	eGijGikGilGim = eGijGikGilGim, 
	eGijGikGilGlm = eGijGikGilGlm, 
	eGijGikGlm2 = eGijGik * eGij2, 
	eGijGikGlmGln = eGijGik^2)
	eG <- eG[tempIdx, ]
	if(nrow(eG) == 1)eG <- eG[1, ]
	return(eG)
}

###########################################################################

#funEGT

funEGT <- function(pArray){
	if(is.matrix(pArray) && nrow(pArray) == 3 && ncol(pArray) == 2)pArray <- array(pArray, c(3, 2, 1))
	if(!is.array(pArray) || dim(pArray)[1] != 3 || dim(pArray)[2] != 2)stop("pArray[, , i] must be a 3-by-2 matrix")
	pArray[pArray < 0 & pArray > -1e-10] <- 0
	pArray[pArray > 1 & pArray - 1 < 1e-10] <- 1
	if(any(pArray < 0) || any(pArray > 1))stop("Invalid value of pArray")
	p00 <- pArray[1, 1, ]
	p10 <- pArray[2, 1, ]
	p20 <- pArray[3, 1, ]
	p01 <- pArray[1, 2, ]
	p11 <- pArray[2, 2, ]
	p21 <- pArray[3, 2, ]
	p.0 <- p00 + p10 + p20
	p.1 <- p01 + p11 + p21
	p0. <- p00 + p01
	p1. <- p10 + p11
	p2. <- p20 + p21
	eGijTij <- 2 * p11 * (p0. + p2.) + 4 * p21 * p10 + 8 * p21 * p0.	
	eGijTik <- p0. * (p11 + 2 * p21) * (p1. + 2 * p2.) + p10 * (p11 + 2 * p21) * (p0. + p2.) + p11 * (1 - p11) * (p0. + p2.) + p20 * (p11 + 2 * p21) * (p1. + 2 * p0.) + p21 * (p11 + 2 * (p.0 + p01)) * (p1. + 2 * p0.)
	eGT <- cbind(eGijTij = eGijTij, eGijTik = eGijTik)
	if(nrow(eGT) == 1)eGT <- eGT[1, ]
	return(eGT)
}

###########################################################################

#funEstimate

funEstimate <- function(N, eD, eG){
	eDij <- eD["eDij"]
	eDij2 <- eD["eDij2"]
	eDijDik <- eD["eDijDik"]
	eDij3 <- eD["eDij3"]
	eDij2Dik <- eD["eDij2Dik"]
	eDijDjkDik <- eD["eDijDjkDik"]
	eDijDjkDkl <- eD["eDijDjkDkl"]
	eDijDikDil <- eD["eDijDikDil"]
	eDij4 <- eD["eDij4"]
	eDij3Dik <- eD["eDij3Dik"]
	eDij2Dik2 <- eD["eDij2Dik2"]
	eDij2DjkDik <- eD["eDij2DjkDik"]
	eDij2DjkDkl <- eD["eDij2DjkDkl"]
	eDijDjk2Dkl <- eD["eDijDjk2Dkl"]
	eDij2DikDil <- eD["eDij2DikDil"]
	eDijDjkDikDil <- eD["eDijDjkDikDil"]
	eDijDjkDklDil <- eD["eDijDjkDklDil"]
	eDij2Dkl2 <- eD["eDij2Dkl2"]
	eDijDjkDklDlm <- eD["eDijDjkDklDlm"]
	eDijDikDilDim <- eD["eDijDikDilDim"]
	eDijDikDilDlm <- eD["eDijDikDilDlm"]
	eDijDikDlm2 <- eD["eDijDikDlm2"]
	eDijDikDlmDln <- eD["eDijDikDlmDln"]
	if(is.matrix(eG)){
		eGij <- eG[, "eGij"]
		eGij2 <- eG[, "eGij2"]
		eGijGik <- eG[, "eGijGik"]
		eGij3 <- eG[, "eGij3"]
		eGij2Gik <- eG[, "eGij2Gik"]
		eGijGjkGik <- eG[, "eGijGjkGik"]
		eGijGjkGkl <- eG[, "eGijGjkGkl"]
		eGijGikGil <- eG[, "eGijGikGil"]
		eGij4 <- eG[, "eGij4"]
		eGij3Gik <- eG[, "eGij3Gik"]
		eGij2Gik2 <- eG[, "eGij2Gik2"]
		eGij2GjkGik <- eG[, "eGij2GjkGik"]
		eGij2GjkGkl <- eG[, "eGij2GjkGkl"]
		eGijGjk2Gkl <- eG[, "eGijGjk2Gkl"]
		eGij2GikGil <- eG[, "eGij2GikGil"]
		eGijGjkGikGil <- eG[, "eGijGjkGikGil"]
		eGijGjkGklGil <- eG[, "eGijGjkGklGil"]
		eGijGjkGklGlm <- eG[, "eGijGjkGklGlm"]
		eGijGikGilGim <- eG[, "eGijGikGilGim"]
		eGijGikGilGlm <- eG[, "eGijGikGilGlm"]
	}else if(is.vector(eG)){
		eGij <- eG["eGij"]
		eGij2 <- eG["eGij2"]
		eGijGik <- eG["eGijGik"]
		eGij3 <- eG["eGij3"]
		eGij2Gik <- eG["eGij2Gik"]
		eGijGjkGik <- eG["eGijGjkGik"]
		eGijGjkGkl <- eG["eGijGjkGkl"]
		eGijGikGil <- eG["eGijGikGil"]
		eGij4 <- eG["eGij4"]
		eGij3Gik <- eG["eGij3Gik"]
		eGij2Gik2 <- eG["eGij2Gik2"]
		eGij2GjkGik <- eG["eGij2GjkGik"]
		eGij2GjkGkl <- eG["eGij2GjkGkl"]
		eGijGjk2Gkl <- eG["eGijGjk2Gkl"]
		eGij2GikGil <- eG["eGij2GikGil"]
		eGijGjkGikGil <- eG["eGijGjkGikGil"]
		eGijGjkGklGil <- eG["eGijGjkGklGil"]
		eGijGjkGklGlm <- eG["eGijGjkGklGlm"]
		eGijGikGilGim <- eG["eGijGikGilGim"]
		eGijGikGilGlm <- eG["eGijGikGilGlm"]
	}else stop("Something wrong!")
	varSM1 <- N * (N - 1) / 2 * (eGij2 - eGij^2) * eDij2
	varSM2 <- N * (N - 1) * (N - 2) * (eGijGik - eGij^2) * eDijDik
	varSM <- varSM1 + varSM2
	eSM31 <- N * (N - 1) / 2 * (eGij3 - 3 * eGij2 * eGij + 2 * eGij^3) * eDij3
	eSM32 <- 3 * N * (N - 1) * (N - 2) * (eGij2Gik - eGij2 * eGij - 2 * eGijGik * eGij + 2 * eGij^3) * eDij2Dik
	eSM33 <- N * (N - 1) * (N - 2) * (eGijGjkGik - 3 * eGijGik * eGij + 2 * eGij^3) * eDijDjkDik
	eSM34 <- 3 * N * (N - 1) * (N - 2) * (N - 3) * (eGijGjkGkl - 2 * eGijGik * eGij + eGij^3) * eDijDjkDkl
	eSM35 <- N * (N - 1) * (N - 2) * (N - 3) * (eGijGikGil - 3 * eGijGik * eGij + 2 * eGij^3) * eDijDikDil
	eSM3 <- eSM31 + eSM32 + eSM33 + eSM34 + eSM35
	eSM41 <- N * (N - 1) / 2 * (eGij4 - 4 * eGij3 * eGij + 6 * eGij2 * eGij^2 - 3 * eGij^4) * eDij4
	eSM42 <- 4 * N * (N - 1) * (N - 2) * (eGij3Gik - (3 * eGij2Gik + eGij3) * eGij + 3 * (eGijGik + eGij2) * eGij^2 - 3 * eGij^4) * eDij3Dik
	eSM43 <- 3 * N * (N - 1) * (N - 2) * (eGij2Gik2 - 4 * eGij2Gik * eGij + (4 * eGijGik + 2 * eGij2) * eGij^2 - 3 * eGij^4) * eDij2Dik2
	eSM44 <- 6 * N * (N - 1) * (N - 2) * (eGij2GjkGik - 2 * (eGijGjkGik + eGij2Gik) * eGij + (5 * eGijGik + eGij2) * eGij^2 - 3 * eGij^4) * eDij2DjkDik
	eSM45 <- 12 * N * (N - 1) * (N - 2) * (N - 3) * (eGij2GjkGkl - (2 * eGijGjkGkl + eGij2Gik) * eGij + 3 * eGijGik * eGij^2 - eGij^4) * eDij2DjkDkl
	eSM46 <- 6 * N * (N - 1) * (N - 2) * (N - 3) * (eGijGjk2Gkl - 2 * (eGijGjkGkl + eGij2Gik) * eGij + (4 * eGijGik + eGij2) * eGij^2 - 2 * eGij^4) * eDijDjk2Dkl
	eSM47 <- 6 * N * (N - 1) * (N - 2) * (N - 3) * (eGij2GikGil - 2 * (eGijGikGil + eGij2Gik) * eGij + (5 * eGijGik + eGij2) * eGij^2 - 3 * eGij^4) * eDij2DikDil
	eSM48 <- 12 * N * (N - 1) * (N - 2) * (N - 3) * (eGijGjkGikGil - (eGijGikGil + eGijGjkGik + 2 * eGijGjkGkl) * eGij + 5 * eGijGik * eGij^2 - 2 * eGij^4) * eDijDjkDikDil
	eSM49 <- 3 * N * (N - 1) * (N - 2) * (N - 3) * (eGijGjkGklGil - 4 * eGijGjkGkl * eGij + 4 * eGijGik * eGij^2 - eGij^4) * eDijDjkDklDil
	eSM410 <- 3 / 4 * N * (N - 1) * (N - 2) * (N - 3) * (eGij2 - eGij^2)^2 * eDij2Dkl2
	eSM411 <- 12 * N * (N - 1) * (N - 2) * (N - 3) * (N - 4) * (eGijGjkGklGlm - 2 * eGijGjkGkl * eGij + eGijGik * eGij^2) * eDijDjkDklDlm
	eSM412 <- N * (N - 1) * (N - 2) * (N - 3) * (N - 4) * (eGijGikGilGim - 4 * eGijGikGil * eGij + 6 * eGijGik * eGij^2 - 3 * eGij^4) * eDijDikDilDim
	eSM413 <- 12 * N * (N - 1) * (N - 2) * (N - 3) * (N - 4) * (eGijGikGilGlm - (2 * eGijGjkGkl + eGijGikGil) * eGij + 3 * eGijGik * eGij^2 - eGij^4) * eDijDikDilDlm
	eSM414 <- 3 * N * (N - 1) * (N - 2) * (N - 3) * (N - 4) * (eGijGik - eGij^2) * (eGij2 - eGij^2) * eDijDikDlm2
	eSM415 <- 3 * N * (N - 1) * (N - 2) * (N - 3) * (N - 4) * (N - 5) * (eGijGik - eGij^2)^2 * eDijDikDlmDln
	eSM4 <- eSM41 + eSM42 + eSM43 + eSM44 + eSM45 + eSM46 + eSM47 + eSM48 + eSM49 + eSM410 + eSM411 + eSM412 + eSM413 + eSM414 + eSM415
	estimate <- cbind(varSM = varSM, eSM3 = eSM3, skewSM = eSM3 / (varSM)^1.5, eSM4 = eSM4, kurtSM = eSM4 / varSM^2 - 3)
	if(nrow(estimate) == 1)estimate <- estimate[1, ]
	return(estimate)
}

###########################################################################

#funAdjust

funAdjust <- function(Z, skew, kurt){
	xi <- (sqrt(1 + 2 * skew * Z) - 1) / skew
	sigma <- sqrt(1 + skew * xi)
	pSkew <- exp(xi^2 / 2 + skew / 6 * xi^3) * pnorm(sigma * xi, lower.tail = FALSE) / exp(xi * (Z - sigma^2 * xi / 2))
	alpha <- kurt / 6
	beta <- skew / 2
	eta <- beta / 6 / alpha^2 - beta^3 / 27 / alpha^3 + Z / 2 / alpha
	delta <- eta^2 + (1 / 3 / alpha - beta^2 / 9 / alpha^2)^3
	deltaPositive <- pmax(0, delta)
	deltaNegative <- pmin(0, delta)
	temp1 <- eta + sqrt(deltaPositive)
	temp2 <- eta - sqrt(deltaPositive)
	xi <- -beta / 3 / alpha + sign(temp1) * (sign(temp1) * temp1)^(1 / 3) + sign(temp2) * (sign(temp2) * temp2)^(1 / 3)
	tempIdx <- which(delta < 0)
	if(length(tempIdx) > 0){
		complexDelta <- complex(real = deltaNegative)
		temp1 <- (eta + sqrt(complexDelta))^(1 / 3)
		temp2 <- (eta - sqrt(complexDelta))^(1 / 3)
		xi1 <- Re(-beta / 3 / alpha + temp1 + temp2)
		xi2 <- Re(-beta / 3 / alpha + temp1 * complex(real = -1 / 2, imaginary = sqrt(3) / 2) + temp2 * complex(real = -1 / 2, imaginary = -sqrt(3) / 2))
		xi3 <- Re(-beta / 3 / alpha + temp1 * complex(real = -1 / 2, imaginary = -sqrt(3) / 2) + temp2 * complex(real = -1 / 2, imaginary = sqrt(3) / 2))
		xiMat <- cbind(xi1, xi2, xi3)[tempIdx, ]
		xiMat[xiMat > Z[tempIdx] | xiMat < 0] <- 0
		xi[tempIdx] <- apply(xiMat, 1, max)
	}
	sigma <- sqrt(1 + skew * xi + kurt / 3 * xi^2)
	pKurt <- exp(xi^2 / 2 + skew / 6 * xi^3 + kurt / 24 * xi^4) * pnorm(sigma * xi, lower.tail = FALSE) / exp(xi * (Z - sigma^2 * xi / 2))
	return(cbind(p_adj_skew = pSkew, p_adj_skew_and_kurtosis = pKurt))
}

###########################################################################

funMain <- function(pLinkFile, distMat, dataCovariate = NULL, interactiveCovariateName = NULL, nPerm = 1e6, nPermEach = 1e4, soFile1 = "dExp1.so", soFile2 = "dExp2.so", soFile3 = "parsePlink.so", soFile4 = "parsePlink2.so"){

	cat("Step 1: Checking input data...")
	tempTime <- system.time({

	bedFile <- paste0(pLinkFile, ".bed")
	bimFile <- paste0(pLinkFile, ".bim")
	famFile <- paste0(pLinkFile, ".fam")
	dataBim <- read.table(bimFile, stringsAsFactors = FALSE)
	dataFam <- read.table(famFile, stringsAsFactors = FALSE)
	nSample <- nrow(dataFam)
	nSNP <- nrow(dataBim)

	if(nrow(distMat) != nSample)
		stop("The number of samples must be the same in the distance matrix and the pLink file!")
	if(!is.null(dataCovariate) && nrow(dataCovariate) != nSample)
		stop("The number of samples must be the same in the covariate file and the pLink file!")
	})
	cat(paste0("Done (", tempTime[3], "s)\nStep 2: Calculating the residuals of the distance matrix via linear regression..."))

	if(is.null(dataCovariate)){
		distMatRes <- distMat - mean(distMat[lower.tri(distMat)], na.rm = TRUE)
		diag(distMatRes) <- 0
		cat("Skipped\n")
	}else{

	tempTime <- system.time({

		tempList <- lapply(dataCovariate, function(x)outer(x, x, function(a, b)abs(a - b)))
		tempList <- lapply(tempList, function(x)x[lower.tri(x)])
		dataModel <- data.frame(y = distMat[lower.tri(distMat)], tempList)
		tempFormula <- as.formula(paste0("y ~ 1 + ", paste(colnames(dataCovariate), collapse = " + ")))
		tempModel <- lm(tempFormula, data = dataModel)
		distMatRes <- distMat; distMatRes[, ] <- 0
		distMatRes[lower.tri(distMatRes)] <- tempModel$residuals
		distMatRes <- distMatRes + t(distMatRes)

	})
		cat(paste0("Done (", signif(tempTime[3], 3), "s)\n"))
	}

	cat(paste0("Step 3: Calculating the expectation of Dij terms in the formula, running ", nPerm, " permutations..."))
	tempTime <- system.time({

	eD <- funED(distMat, nPerm = nPerm, nPermEach = nPermEach, soFile1 = soFile1, soFile2 = soFile2)

	})
	cat(paste0("Done (", tempTime[3], "s)\nStep 4: Calculating the score test statistic for all SNPs..."))
	tempTime <- system.time({

	if(is.null(interactiveCovariateName)){
		interactiveCovariate <- rep(1L, nSample)
		interactiveFlag <- FALSE
	}else if(!interactiveCovariateName %in% colnames(dataCovariate)){
		interactiveCovariate  <- rep(1L, nSample)
		interactiveFlag <- FALSE
		warning("Invalid name of the interactive covariate, ignored")
	}else{
		interactiveCovariate <- as.integer(dataCovariate[, interactiveCovariateName])
		if(any(!interactiveCovariate %in% c(0L, 1L))){
			interactiveCovariate[!interactiveCovariate %in% c(0L, 1L)] <- 1L
			warning("The interactive covariate could only contain 0/1 values, all nonzero values were forced to 1")
		}
		interactiveFlag <- TRUE
	}

	if(interactiveFlag){
		dyn.load(soFile3)
		dataBed <- .C("parsePlink", bedFile = bedFile, nSample = as.integer(nSample), nSNP = as.integer(nSNP), distMat = as.vector(distMatRes), interactiveCovariate = interactiveCovariate, result = matrix(0.1, 12L, nSNP))
	}else{
		dyn.load(soFile4)
		dataBed <- .C("parsePlink2", bedFile = bedFile, nSample = as.integer(nSample), nSNP = as.integer(nSNP), distMat = as.vector(distMatRes), interactiveCovariate = interactiveCovariate, result = matrix(0.1, 12L, nSNP))
	}
	tempDF <- as.data.frame(t(dataBed$result))
	names(tempDF) <- c("RR", "RV", "VV", "MISSING", "MAF", "RR_I", "RV_I", "VV_I", "MISSING_I", "MAF_I", "SM", "SI")
	})

	cat(paste0("Done (", tempTime[3], "s)\nStep 5: Calculating the Z score, along with the skewness, kurtosis and p values..."))
	tempTime <- system.time({

	N <- nSample - tempDF$MISSING
	f <- tempDF$MAF
	eG <- funEG(f)
#	eG <- funEG(cbind(tempDF$RR / N, tempDF$RV / N))
	estimate <- funEstimate(N, eD, eG)
	varSM <- estimate[, "varSM"]
	skewSM <- estimate[, "skewSM"]
	kurtSM <- estimate[, "kurtSM"]
	SM <- tempDF$SM
	ZM <- SM / sqrt(varSM)
	pM <- pMS <- pMK <- pnorm(ZM, lower.tail = FALSE)
	tempIdx <- which(!is.na(ZM) & ZM > 0)
	pMSK <- funAdjust(ZM[tempIdx], skewSM[tempIdx], kurtSM[tempIdx])
	pMS[tempIdx] <- pMSK[, 1]
	pMK[tempIdx] <- pMSK[, 2]

	if(interactiveFlag){
		N <- nSample - tempDF$MISSING_I
		e <- mean(interactiveCovariate, na.rm = TRUE)
		pMat <- cbind((1 - f)^2, 2 * f * (1 - f), f^2)
		pArray <- array(t(cbind(pMat * (1 - e), pMat * e)), c(3, 2, nSNP))
		eT <- funEG(cbind(colSums(pArray[, 1, ]) + pArray[1, 2, ], pArray[2, 2, ]))
#		eT <- funEG(cbind(tempDF$RR_I / N, tempDF$RV_I / N))
		estimate <- funEstimate(N, eD, eT)
		varSI <- estimate[, "varSM"]
		skewSI <- estimate[, "skewSM"]
		kurtSI <- estimate[, "kurtSM"]
		SI <- tempDF$SI
		ZI <- SI / sqrt(varSI)
		pI <- pIS <- pIK <- pnorm(ZI, lower.tail = FALSE)
		tempIdx <- which(!is.na(ZI) & ZI > 0)
		pISK <- funAdjust(ZI[tempIdx], skewSI[tempIdx], kurtSI[tempIdx])
		pIS[tempIdx] <- pISK[, 1]
		pIK[tempIdx] <- pISK[, 2]

		eGT <- funEGT(pArray)
		if(is.matrix(eGT)){
			eGijTij <- eGT[, "eGijTij"]
			eGijTik <- eGT[, "eGijTik"]
		}else if(is.vector(eGT)){
			eGijTij <- eGT["eGijTij"]
			eGijTik <- eGT["eGijTik"]
		}else stop("Something wrong!")
		if(is.matrix(eG)){eGij <- eG[, "eGij"]}else if(is.vector(eG)){eGij <- eG["eGij"]}else stop("Something wrong!")
		if(is.matrix(eT)){eTij <- eT[, "eGij"]}else if(is.vector(eT)){eTij <- eT["eGij"]}else stop("Something wrong!")
		covSMSI1 <- N * (N - 1) / 2 * (eGijTij - eGij * eTij) * eD["eDij2"]
		covSMSI2 <- N * (N - 1) * (N - 2) * (eGijTik - eGij * eTij) * eD["eDijDik"]
		covSMSI <- covSMSI1 + covSMSI2
		corSMSI <- covSMSI / sqrt(varSM * varSI)

		K <- sqrt((1 - corSMSI) / (1 + corSMSI))
		prob <- 1 / 2 -  atan(K) / pi
		Z1 <- (ZM - ZI) / sqrt(2 * (1 - corSMSI))
		Z2 <- (ZM + ZI) / sqrt(2 * (1 + corSMSI))
		LRT <- rep(NA, nSNP)
		tempIdx <- which(Z2 >= -K * Z1 & Z2 >= K * Z1)
		LRT[tempIdx] <- (Z1^2 + Z2^2)[tempIdx]
		tempIdx <- which(Z2 <= 1 / K * Z1 & Z2 <= -1 / K * Z1)
		LRT[tempIdx] <- 0
		tempIdx <- which(is.na(LRT) & !is.na(ZM) & !is.na(ZI))
		LRT[tempIdx] <- ((Z1 * sign(Z1) + Z2 * K)^2 / (K^2 + 1))[tempIdx]
		pMI <- prob * pchisq(LRT, 2, lower.tail = FALSE) + 0.5 * pchisq(LRT, 1, lower.tail = FALSE)

		ZMS <- qnorm(1 - pMS)
		ZIS <- qnorm(1 - pIS)
		Z1 <- (ZMS - ZIS) / sqrt(2 * (1 - corSMSI))
		Z2 <- (ZMS + ZIS) / sqrt(2 * (1 + corSMSI))
		LRT <- rep(NA, nSNP)
		tempIdx <- which(Z2 >= -K * Z1 & Z2 >= K * Z1)
		LRT[tempIdx] <- (Z1^2 + Z2^2)[tempIdx]
		tempIdx <- which(Z2 <= 1 / K * Z1 & Z2 <= -1 / K * Z1)
		LRT[tempIdx] <- 0
		tempIdx <- which(is.na(LRT) & !is.na(ZM) & !is.na(ZI))
		LRT[tempIdx] <- ((Z1 * sign(Z1) + Z2 * K)^2 / (K^2 + 1))[tempIdx]
		pMIS <- prob * pchisq(LRT, 2, lower.tail = FALSE) + 0.5 * pchisq(LRT, 1, lower.tail = FALSE)

		ZMK <- qnorm(1 - pMK)
		ZIK <- qnorm(1 - pIK)
		Z1 <- (ZMK - ZIK) / sqrt(2 * (1 - corSMSI))
		Z2 <- (ZMK + ZIK) / sqrt(2 * (1 + corSMSI))
		LRT <- rep(NA, nSNP)
		tempIdx <- which(Z2 >= -K * Z1 & Z2 >= K * Z1)
		LRT[tempIdx] <- (Z1^2 + Z2^2)[tempIdx]
		tempIdx <- which(Z2 <= 1 / K * Z1 & Z2 <= -1 / K * Z1)
		LRT[tempIdx] <- 0
		tempIdx <- which(is.na(LRT) & !is.na(ZM) & !is.na(ZI))
		LRT[tempIdx] <- ((Z1 * sign(Z1) + Z2 * K)^2 / (K^2 + 1))[tempIdx]
		pMIK <- prob * pchisq(LRT, 2, lower.tail = FALSE) + 0.5 * pchisq(LRT, 1, lower.tail = FALSE)

		resultDF <- data.frame(SNP = dataBim$V2, Chr = dataBim$V1, Pos = dataBim$V4, Allele1 = dataBim$V5, Allele2 = dataBim$V6, MAF = f, Z_M = ZM, Skew_M = skewSM, Kurt_M = kurtSM, p_M_Asymptotic = pM, p_M_Skew = pMS, p_M_skew_kurt = pMK, Z_I = ZI, Skew_I = skewSI, Kurt_I = kurtSI, p_I_Asymptotic = pI, p_I_Skew = pIS, p_I_skew_kurt = pIK, 'cor(Z_M, Z_I)' = corSMSI, p_joint_Asymptotic = pMI, p_joint_Skew = pMIS, p_joint_skew_kurt = pMIK, stringsAsFactors = FALSE, check.names = FALSE)
	}else resultDF <- data.frame(SNP = dataBim$V2, Chr = dataBim$V1, Pos = dataBim$V4, Allele1 = dataBim$V5, Allele2 = dataBim$V6, MAF = f, Z_M = ZM, Skew_M = skewSM, Kurt_M = kurtSM, p_M_Asymptotic = pM, p_M_Skew = pMS, p_M_skew_kurt = pMK, stringsAsFactors = FALSE, check.names = FALSE)

	})
	cat(paste0("Done (", tempTime[3], "s)\n"))

	return(resultDF)
}

###########################################################################
#Main procedure

## Collect arguments
args <- commandArgs(TRUE)

## Default setting when no arguments passed
if(length(args) < 1) {
  args <- c("-h")
}

## Help section
if("-h" %in% args) {
	cat("
	The microbiomeGWAS Script

	Arguments:
		-r	absolute path to the microbiomeGWAS package root, required
		-p	plink file pre with absolutte path, required
		-d	distance matrix file name with absolutte path, required
		-o	absolute path for output results, optional, defalut is the current directory
		-c	covariateFile file name with absolutte path, optional
		-i	interactive	covariate name in covariateFile, optional 
	Usage:
	Rscript microbiomeGWAS_Root_Path/R/microbiomeGWAS_v1.0.R -r microbiomeGWAS_Root_Path -p Your_Plink_Path/Plink_Pre -d Your_Dist_Matrix_Path/Dist_Matrix.txt -o Out_Path -c Your_Covariate_Path/Covariate.txt -i Your_Covariate_Name \n\n")

	q(save="no")
}

idx = 1
argsDF = NULL
while(idx < length(args)){
	argsDF = rbind(argsDF, c(gsub('-', '', args[idx]), args[idx + 1]))
	idx = idx + 2
}
argsL = as.list(as.character(argsDF[, 2]))
names(argsL) <- argsDF[, 1]


if(is.null(argsL$r) || is.null(argsL$p) || is.null(argsL$d)) {
	stop("microbiomeGWAS package root, plink file pre and distance matrix file are required!")
}

packageDir <- argsL$r
cat('packageDir:', packageDir, '\n')
pLinkFile <- argsL$p
cat('pLinkFile:', pLinkFile, '\n')
distMatFile <- argsL$d
cat('distMatFile:', distMatFile, '\n')

if(!is.null(argsL$c)){
	covariateFile <- argsL$c
	cat('covariateFile:', covariateFile, '\n')
}else{
	covariateFile = NA
}

if(!is.null(argsL$i)){
	interactiveCovariateName <- argsL$i
	cat('interactiveCovariateName:', interactiveCovariateName, '\n')
}else{
	interactiveCovariateName = NA
}

if(is.null(argsL$o)){
	outDir = getwd()
}else{
	outDir = argsL$o
}

cat('outDir:', outDir, '\n')

## Collect arguments done !

distMat <- as.matrix(read.table(distMatFile))

soFile1 <- paste0(packageDir, "/lib/dExp1.so")
soFile2 <- paste0(packageDir, "/lib/dExp2.so")
soFile3 <- paste0(packageDir, "/lib/parsePlink.so")
soFile4 <- paste0(packageDir, "/lib/parsePlink2.so")
if((!file.exists(soFile1)) || (!file.exists(soFile2)) || (!file.exists(soFile3)) || (!file.exists(soFile4))){
	system(paste0('cd ', packageDir, '; sh compile.src.sh'))
}


if(is.na(covariateFile)){
	dataCovariate <- NULL
	cat('Warning: No covarite file specified!\n')
}else{
	dataCovariate <- read.table(covariateFile, header = TRUE)
	if(any(!unlist(lapply(dataCovariate, is.numeric)))) stop("Only numeric covariates allowed!")
	if(is.na(interactiveCovariateName))interactiveCovariateName <- NULL
}


cat("Starting:\n")
tempTime <- system.time(resultDF <- funMain(pLinkFile, distMat, dataCovariate, interactiveCovariateName, soFile1 = soFile1, soFile2 = soFile2, soFile3 = soFile3, soFile4 = soFile4))
cat(paste0("All done (total ", tempTime[3], "s)\nWriting the output file..."))
setwd(outDir)
write.table(resultDF, file = paste0(tail(strsplit(pLinkFile, '/', fixed = TRUE)[[1]], 1), ".result.txt"), sep = "\t", quote = FALSE, row.names = FALSE)
cat("Done\n")
