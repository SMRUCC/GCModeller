library("limma")
library("edgeR")

# file - file.txt:  id...sample1...sample2...
#                   前面的一半是实验，后面的一半是对照，也可以反过来，顺序无所谓
# repeatsNum - sample repeats number, total sample number is repeatsNum*2
function DEG(file, repeatsNum) {

	rawdata<-read.delim(file,header=T)
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
	y<-plotMDS(y)
	
	##设计design matrix
	
	# 根据重复数生成design
	H = rep('H', repeatsNum)
	M = rep('M', repeatsNum)
	group <- append(H, M)
	group<-factor(group)
	design <- model.matrix(~group)
	
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
	fit<-glmFit(y, design)
	lrt<-glmLRT(fit)
	topTags(lrt)#前10个差异表达基因
	##火山图
	summary(de<-decideTestsDGE(qlf))##qlf或可改为lrt
	detags<-rownames(y)[as.logical(de)]
	plotSmear(qlf, de.tags=detags)
	abline(h=c(-4,4),col='blue') #蓝线为2倍差异表达基因，差异表达的数据在qlf中
}
