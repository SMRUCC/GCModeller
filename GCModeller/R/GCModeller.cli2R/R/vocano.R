library(ggplot2)
library(tools)
library(Cairo)

## Plot DEP vocano plot by using default FC level value 1.5.
plot.vocano.DEP <- function(file) {
	plot.vocano(file, level=1.5)
}

plot.vocano <- function(file, level=0) {

	DIR <- dirname(file)
	DIR <- paste(DIR, file_path_sans_ext(basename(file)), sep="/")
    out <- paste(DIR, "plot.vocano.png", sep=".")
	
	diff <- 1
	
	if (level != 0) {
		diff <- log(level, 2)		
	}
	
	data=read.csv(file=file, header=T)
	data=data.frame(PValue=c(data$PValue), logFC=c(data$logFC))
	data$threshold<-as.factor(abs(data$logFC) >= diff & data$PValue <= 0.05)
	
	Cairo(out, type="png", units="in", width=5*2, height=4*2, pointsize=12, dpi=200)
	
		g = ggplot(data=data, aes(x=logFC, y=-log10(PValue), colour=threshold)) +
				   geom_point(alpha=0.4, size=1.75) +
				   # opts(legend.position = "none") +
				   xlim(c(-5, 5)) + ylim(c(0, 5)) +
				   xlab("log2 fold change") + ylab("-log10(p-value)")
		print(g)
		
	dev.off()
}

# 这个函数的数据是经过log2转换了的
plot.vocano.iTraq <- function(file, 
	tag      = "FC.avg", 
	level    = 1.25, 
	pvalue   = "p.value", 
	tag.disp = "log2(average FoldChange)", 
	xrange = NULL, 
	yrange = NULL) {
	
	log2 <- log(level,2);
	
	plot.vocano(file, 
		tag      = tag, 
		level    = c(log2, -log2), 
		pvalue   = pvalue, 
		tag.disp = tag.disp, 
		log.t    = 2, 
		xrange   = xrange , 
		yrange   = yrange);
}

plot.vocano.LFQ <- function(file, 
	tag      = "logFC", 
	level    = 1.25, 
	pvalue   = "p.value", 
	tag.disp = "log2(FoldChange)") {
	
	log2 <- log(level,2);
	
	plot.vocano(file, 
		tag      = tag, 
		level    = c(log2, -log2), 
		pvalue   = pvalue, 
		tag.disp = tag.disp, 
		log.t    = 0);
}

# 自定义画图
# PValue
# tag
# level 默认为1用作DEG分析
## @param xrange The X axis tick range, default is from -5 to 5
## @param yrange The Y axis tick range, default is from 0 to 5
## @param level  The DEP/DEG threshold, default is greater than log(2)=1 for up regulated and smaller than log(1/2)=-1 for down regulated.
## @param pvalue The column name in the csv @file for using as the p.value of the DEP calculation
## @param tag.disp The X axis display label text
## @param tag    The column name in the csv @file for using as the fold change result value from the DEP calculation
## @param file   The csv data source file for this vocano plot function. 
plot.vocano <- function(
	file, 
	tag="logFC", level=c(1,-1), pvalue = "PValue", tag.disp = "log2 fold change", 	
	log.t = 0, 
	xrange = NULL, 
	yrange = NULL) {

	## generates the output file name automatic
	DIR <- dirname(file);
	DIR <- paste(DIR, file_path_sans_ext(basename(file)), sep="/");
    out <- paste(DIR, "plot.vocano.png", sep=".");
			
	## load source data and get the fold change value and pvalue that using for the vocano plot
	data  <- read.csv(file=file, header=T);
	
	logFC <- data[tag];
	if (log.t > 0) {
		logFC <- log(logFC, log.t);
		print(head(logFC));
		print(level);
		logFC <- logFC[[tag]];
	}
	
	data  <- data.frame(PValue=c(data[pvalue]), logFC= logFC);
	## set plot color schema for the DEP and non-DEP data
	data$threshold <- as.factor(
		(logFC >= level[1] | logFC <= level[2]) & 
		(data[pvalue] <= 0.05));
	
	print(head(data));
	
	# 自动计算出xrange和yrange假若参数为空值的话
	
	if (is.null(xrange)) {
		xrange = as.vector(data[, "logFC"]);
		xrange = max(abs(xrange))
		xrange = c(-xrange, xrange);
	}
	
	if (is.null(yrange)) {
		yrange = as.vector(data[, "p.value"]);
		yrange = c(0, -log( min(yrange), 10));
	}

	print(sprintf("xrange is %s", toString(xrange)));
	print(sprintf("yrange is %s", toString(yrange)));
	
	print("Have a peeks on the result data:");
	print(head(data));
	
	## Invoke graphics plot.
	Cairo(out, type="png", units="in", width=5*2, height=4*2, pointsize=12, dpi=200);
	
		tag <- "logFC";
		g <- ggplot(data = data, aes(x=data[tag], y=-log10(data[pvalue]), colour=threshold)) +
			 geom_point(alpha=0.4, size=1.75) +
			 # opts(legend.position = "none") +
		     xlim(xrange) + ylim(yrange) +
		     xlab(tag.disp) + 
			 ylab("-log10(p-value)");
			 
		print(g);
		
	dev.off();
}