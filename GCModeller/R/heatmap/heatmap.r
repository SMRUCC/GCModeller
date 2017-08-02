library(tools) 
library(Cairo)
library(ggplot2)

#########################################################
### A) Installing and loading required packages
#########################################################

if (!require("gplots")) {
	install.packages("gplots", dependencies = TRUE)
	library(gplots)
}
if (!require("RColorBrewer")) {
	install.packages("RColorBrewer", dependencies = TRUE)
	library(RColorBrewer)
}

## colorsPalette=brewer.pal(11,"RdYlBu")

# DEP的热图绘制示例
#
# plotDEPs(
# 	csv, t.log2=T, size = c(1600,2600), plot.margin=c(13,3), l.height = c(1,8), 
#   fontsize.col = 2, 
#   colorsPalette = rev(brewer.pal(9, "RdYlBu")))

### 这个脚本比较适合蛋白比较少的，会显示出基因的label
### 所输入的数据文件的要求：
### 1. 第一列数据为基因的编号
### 2. 其他的剩余的数据的列都是基因的表达量或者实验间的logFC值
### @size: (width, height)
plotDEPs <- function(csv, 
					 size          = c(5*300, 5*300), 
					 colorsPalette = c("green", "black", "red"), 
					 fontsize.row  = 0.35, 
					 fontsize.col  = 1.5, 
					 title         = NA, 
					 plot.margin   = c(4, 3), 
					 l.width       = c(1.5, 2), 
					 l.height      = c(0.4, 2), 
					 t.log2          = FALSE, 
					 row.lab.removes = FALSE) {

	#########################################################
	### B) Reading in data and transform it into matrix format
	#########################################################

	data                  <- read.csv(csv, comment.char="#")
	rnames                <- data[,1]                          # assign labels in column 1 to "rnames"
	matrix.data           <- data.matrix(data[,2:ncol(data)])  # transform column 2-5 into a matrix
	rownames(matrix.data) <- rnames                            # assign row names

	if (t.log2) {
		matrix.data <- log(matrix.data, 2);
	}
	
	#########################################################
	### C) Customizing and plotting the heat map
	#########################################################

	# creates a own color palette from red to green
	my_palette <- colorRampPalette(colorsPalette)(n = 20)

	# (optional) defines the color breaks manually for a "skewed" color transition
	# col_breaks = c(
	#   seq(-1,0,length=100),               # for red
	#   seq(0.01,0.8,length=100),           # for yellow
	#   seq(0.81,1,length=100))             # for green
	
	# 在这里是根据文件名解析出其所在的文件夹，用来在相同的文件夹之中保存所绘制的heatmap图像
	DIR <- dirname(csv)
	DIR <- paste(DIR, file_path_sans_ext(basename(csv)), sep="/")
  
	# creates a 5 x 5 inch image
	png(paste(DIR, "heatmap.png", sep="-"),    # create PNG for the heat map        
		width     = size[1],                   # 5 x 300 pixels
		height    = size[2],
		res       = 300,                       # 300 pixels per inch
		pointsize = 8)                         # smaller font size

	tryCatch({
		heatmap.2(matrix.data,
			scale        = "row",
			labRow       = NA,
			# labCol       = NA,
			main         = title,            # heat map title
			notecol      = "black",          # change font color of cell labels to black
			density.info = "none",           # turns off density plot inside color legend
			trace        = "none",           # turns off trace lines inside the heat map
			margins      = plot.margin,      # widens margins around plot
			lwid         = l.width,
			lhei         = l.height,
			sepwidth     = c(1,1),
			sepcolor     = "white",
			col          = my_palette,       # use on color palette defined earlier
			cexRow       = fontsize.row,
			cexCol       = fontsize.col,
			# breaks     = col_breaks,       # enable color transition at specified limits
			dendrogram   = "row",            # only draw a row dendrogram
			# hclustfun       = hclust(method ="average"),
			Colv         = "NA", 
			srtCol       = 45); 
	}, error = function(e) {
		print(e);
	});		  
  
	dev.off()                                  # close the PNG device
}

library(pheatmap)

plot.pheatmap <- function(csv, size = c(2000,3000)) {

	data <- read.csv(file=csv, comment.char="#", row.names=1);
	
	DIR <- dirname(csv)
	DIR <- paste(DIR, file_path_sans_ext(basename(csv)), sep="/")
	
	png(paste(DIR, "heatmap.png", sep="-"),    # create PNG for the heat map        
		width     = size[1],                   # 5 x 300 pixels
		height    = size[2],
		res       = 300,                       # 300 pixels per inch
		pointsize = 8)                         # smaller font size
		
	tryCatch({
	
		pheatmap(raw,scale="row",show_rownames=F,cluster_cols=FALSE);
		
	}, error = function(e) {
		print(e);
	});
	
	dev.off();          	
}