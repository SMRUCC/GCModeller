library(tools) 
options(stringsAsFactors = FALSE) 

### 蛋白等电点对蛋白相对分子质量散点图
### @csv: iTraq结果文件
PI.plot <- function(csv) {

	raw<-read.csv(file=csv,header=TRUE)

	DIR <- dirname(csv)
	DIR <- paste(DIR, file_path_sans_ext(basename(csv)), sep="/")

	tiff(paste(DIR, "pimw.tiff", sep="-"),width=2500,height=2000,res=400)
	
		names <- colnames(raw);
		pI <- "calc..pI";
		MW <- "MW..kDa.";
		
		if ((!(pI %in% names)) || (!(MW %in% names))) {
			print(names);
		}
		
		plot(raw[, pI],raw[, MW],xlab="Calc.PI",ylab="MW[Kda]",pch=15)
		
	dev.off();
}

