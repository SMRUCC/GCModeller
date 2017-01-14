library("limma")
library("edgeR")
library(tools) 
library(Cairo)

DEP <- function(file, repeatsNum=3, csv = 0) {

    DEG(file, repeatsNum=repeatsNum, DEP = 1, csv = csv)
}

# file - file.txt:  id...sample1...sample2...
#                   前面的一半是实验，后面的一半是对照，也可以反过来，顺序无所谓，计算出来的结果如果方向反了，只需要将logFC的符号变一下就行了
# repeatsNum - sample repeats number, total sample number is repeatsNum*2   
#              默认所输入的数据是3次生物学重复
DEG <- function(file, repeatsNum=3, DEP = 0, csv = 0) {

	DIR <- dirname(file)
	DIR <- paste(DIR, file_path_sans_ext(basename(file)), sep="/")
	DIR  # 结果数据输出的文件夹
	dir.create(DIR)	
	
	sep = "\t"
	
	if (csv != 0) {
		sep=","
	}
	
	rawdata <- read.delim(file, header=T, sep=sep)
	total = repeatsNum * 2
	head(rawdata) #检查读入是否正确
	# 第一列是编号，剩下的列都是表达量
	right = total +1
	y<-DGEList(counts=rawdata[,2:right],genes=rawdata[,1])
	
	##过滤与标准化
	#left<-rowSums(cpm(y)>1)>=4 #过滤标准为至少one count per million (cpm)
	#y<-y[left,]
	y<-DGEList(counts=y$counts,genes=y$genes)
	y<-calcNormFactors(y)#默认为TMM标准化
	##检查样本的outlier and relationship
	
	Cairo(paste(DIR, "plotMDS.png", sep="/"), type="png", units = "in", width=5*2, height=4*2, pointsize=18, dpi=500)
		y<-plotMDS(y)
	dev.off()
	
	##设计design matrix
	
	# 根据重复数生成design
	treat = rep('H', repeatsNum)
	sample = rep('M', repeatsNum)
	group <- append(treat, sample)
	group<-factor(group)
	design <- model.matrix(~group)
	rownames(design)<-colnames(y)
	
	y<-DGEList(counts=rawdata[,2:right],genes=rawdata[,1])
	
	##推测dispersion（离散度）
	y<-estimateGLMCommonDisp(y,design,verbose=TRUE)
	y<-estimateGLMTrendedDisp(y, design)
	y<-estimateGLMTagwiseDisp(y, design)
	##差异表达基因，to perform quasi-likelihood F-tests:
	fit <- glmQLFit(y,design)
	qlf <- glmQLFTest(fit,coef=2)
	topTags(qlf)#前10个差异表达基因
	##or 差异表达基因，to perform likelihood ratio tests:
	#fit<-glmFit(y, design)
	#lrt<-glmLRT(fit)
	#top10 <- topTags(lrt)#前10个差异表达基因
	
	##火山图
	summary(de<-decideTestsDGE(qlf))##qlf或可改为lrt
	detags<-rownames(y)[as.logical(de)]
	
	downline = -1
	upline=1
	
	if(DEP != 0) {
	
		upline = log(1.5, 2)
		downline = -1*upline
	}
	
	Cairo(paste(DIR, "plotSmear.png", sep="/"), type="png", units="in", width=5*2, height=4*2, pointsize=24, dpi=500)		
		plotSmear(qlf, de.tags=detags)
		abline(h=c(downline,upline),col='blue') #蓝线为 (DEG=2, DEP=1.5) 倍差异表达基因，差异表达的数据在qlf中
	dev.off()

	qlf;
	
	# 最后在这里导出数据
	output <- qlf$genes
	output["counts"] <- y$counts
	columns <- c("logFC","logCPM","F","PValue")
	output[columns] <- qlf$table[columns]
	write.csv(qlf$samples, paste(DIR, "samples.csv", sep="/"))	
	write.csv(output, paste(DIR, "qlfTable.csv", sep="/"), row.names= FALSE)	
}
