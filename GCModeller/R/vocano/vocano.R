library(ggplot2)
library(tools)
library(Cairo)

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

# 自定义画图
# PValue
# tag
# level 默认为1用作DEG分析
plot.vocano <- function(file, tag="logFC", level=c(1,-1), pvalue = "PValue", tag.disp = "log2 fold change", xrange = c(-5, 5), yrange=c(0, 5)) {

	DIR <- dirname(file)
	DIR <- paste(DIR, file_path_sans_ext(basename(file)), sep="/")
    out <- paste(DIR, "plot.vocano.png", sep=".")
			
	data=read.csv(file=file, header=T)
	data=data.frame(PValue=c(data[pvalue]), logFC=c(data[tag]))
	data$threshold<-as.factor((data[tag] >= level[1] | data[tag] <= level[2]) & (data[pvalue] <= 0.05))
	
	Cairo(out, type="png", units="in", width=5*2, height=4*2, pointsize=12, dpi=200)
	
		g = ggplot(data=data, aes(x=data[tag], y=-log10(data[pvalue]), colour=threshold)) +
				   geom_point(alpha=0.4, size=1.75) +
				   # opts(legend.position = "none") +
				   xlim(xrange) + ylim(yrange) +
				   xlab(tag.disp) + ylab("-log10(p-value)")
		print(g)
		
	dev.off()
}