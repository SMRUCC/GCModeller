require(tools);

consistency.plot <- function(sample, pairs, repeats = 3, size = c(6000,4500), resolution = 550) {
	pairs <- read.csv(pairs);
	iTraq.consistency(sample, pairs, repeats = 3, size, resolution);
}

### regression scatter plot function tools for the iTraq data samples consistency check
### @sample: iTraq sample data output
### @pairs: a dataframe object, that each row should contians two string value, like
###  ,  X,  Y 
### 1, C1, C2
### 2, C1, C3
### 3, C2, C3
###
iTraq.consistency <- function(sample, pairs, repeats = 3, size = c(6000, 4500), resolution = 550) {

	DIR <- dirname(sample);
	DIR <- paste(DIR, file_path_sans_ext(basename(path = sample)), sep="/")
	# setwd(DIR)

	# 绘图操作的文件输出
	scatter.tiff <- paste(DIR, "-consistency-scatterplot2.png")
	options(stringsAsFactors = FALSE) 

	# data consistency check by using regression scatter plot
	# scatter plot
	raw <- read.csv(sample, header=TRUE)

	png(scatter.tiff, width=size[1], height=size[2], res=resolution)

	n.compares <- nrow(pairs);
	n.compares <- n.compares / repeats;
	layout <- c(n.compares, repeats);
	
	print(layout);
	
	par(mfrow = layout);
	raw = data.frame(raw, stringsAsFactors = FALSE);	
	
	tryCatch({
		for (i in 1:nrow(pairs)) {
			x <- pairs[i, ];		
			x <- as.vector(unlist(x));
			print(x);
			regression.plot(raw, x[1], x[2]);
		}
	}, error = function(e) {
		print(e);
	})

	dev.off();
}

### apply the regression scatter plot for each sample pairs
### @raw: The raw sample data inputs
### @sx, sy: The columns title in the @raw sample dataframe
regression.plot <- function(raw, sx, sy) {

	xl <- paste("sample", sx)
	yl <- paste("sample", sy)

	x <- as.numeric(as.vector(raw[,sx]))
	y <- as.numeric(as.vector(raw[,sy]))
	title <- paste(sx, "vs", sy)

	plot(x, y, col="red", pch=4, xlab= xl, ylab=yl, main=title)
	
	fit <- lm(x~y)
	abline(fit, col="black", lwd=2) # regression line (y~x) 
	r2=paste("=", round(summary(fit)$adj.r.squared, 4))
	text(3/4 * max(x, na.rm=T), 1/3 * max(y, na.rm=T), bquote(R^2~.(r2)))

}