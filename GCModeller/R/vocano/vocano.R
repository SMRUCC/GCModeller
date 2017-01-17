library(ggplot2)
library(tools)
library(Cairo)

plot.vocano.DEP <- function(file) {
	plot.vocano(file, DEP=1)
}

plot.vocano <- function(file, DEP=0) {

	DIR <- dirname(file)
	DIR <- paste(DIR, file_path_sans_ext(basename(file)), sep="/")
    out <- paste(DIR, "plot.vocano.png", sep=".")
	
	diff <- 1
	
	if (DEP != 0) {
		diff <- log(1.5, 2)		
	}
	
	data=read.csv(file=file, header=T)
	data=data.frame(PValue=c(data$PValue), logFC=c(data$logFC))
	data$threshold<-as.factor(abs(data$logFC) > diff & data$PValue < 0.05)
	
	Cairo(out, type="png", units="in", width=5*2, height=4*2, pointsize=18, dpi=500)
	
		g = ggplot(data=data, aes(x=logFC, y=-log10(PValue), colour=threshold)) +
				   geom_point(alpha=0.4, size=1.75) +
				   # opts(legend.position = "none") +
				   xlim(c(-5, 5)) + ylim(c(0, 5)) +
				   xlab("log2 fold change") + ylab("-log10 p-value")
		print(g)
		
	dev.off()
}
