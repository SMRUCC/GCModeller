library(tools) 

## 统计计算差异表达蛋白

# 输入的数据格式要求有
#
# 1. 第一列为基因的编号列
# 2. 剩余的所有的列都是FoldChange计算结果
#
# Symbol  A/B  B/C  A/C
# 1       1.5  2.3  3.9	
# 2       3.6  0.1  1.2
# 3       0.0  0.1  0.5
# ...

logFC.test.csv <- function(data.csv, level = 1.5, p.value = 0.05, fdr.threshold = 0.05, includes.ZERO = FALSE) {
	data <- logFC.test(read.csv(data.csv), level, p.value, fdr.threshold, includes.ZERO);
	save.result(data, file = data.csv);
}

logFC.test.tsv <- function(data.txt, level = 1.5, p.value = 0.05, fdr.threshold=0.05, includes.ZERO = FALSE) {
	data <- logFC.test(read.delim(data.txt), level, p.value, fdr.threshold, includes.ZERO);
	save.result(data, file = data.txt);
}

# 判断目标向量vector之中的所有的元素是否都等于元素x？
ALL.Equals <- function(vector, x) {
    nna <- which(!is.na(vector));
	v   <- which(vector==x);
	l   <- length(vector);
	return(l == length(v) && l == length(nna));
}

# 自动生成保存所需要的文件名并执行数据框的保存操作
save.result <- function(data, file) {
	DIR <- dirname(file);
	DIR <- paste(DIR, file_path_sans_ext(basename(file)), sep="/");
	out <- paste(DIR, "-avgFC-log2-t.test.csv", sep="");
	
	write.csv(data, out, row.names= FALSE);
}

logFC.test.LFQ.csv <- function(data.csv, level = 1.5, p.value = 0.05) {
	data <- logFC.t.test(read.csv(data.csv), level, p.value);
	save.result(data, file = data.csv);
}

logFC.test.LFQ.tsv <- function(data.txt, level = 1.5, p.value = 0.05) {
	data <- logFC.t.test(read.delim(data.txt), level, p.value);
	save.result(data, file = data.txt);
}

## Label Free差异蛋白计算
## a <- c();
## b <- c();
## p.value <- t.test(a, b, var.equal = TRUE)$p.value; 
## logFC <- log2(mean(a)/mean(b));
logFC.t.test <- function(data, level = 1.5, p.value = 0.05) {
	
	# data输入的格式为：
	# 第一列为基因的编号，剩下的列数应该是偶数
	# 则前半部分的数据为分母数据
	# 后半部分的数据为分子数据
	
	print(head(data));
	
	repeatsNumber <- (ncol(data) - 1) / 2;
	pvalue        <- rep(0, nrow(data));
	logFC         <- rep(0, nrow(data));
	
	print(sprintf("input data have %s repeats.", repeatsNumber));
	
	# a和b 都是index值
	a <- 2:(2 + repeatsNumber - 1);
	b <- (2+repeatsNumber):(2+2*repeatsNumber -1);
	
	print("index of a:");
	print(a);
	print("index of b:");
	print(b);
	
	for (i in 1:nrow(data)) {
		row <- as.numeric(
			   as.vector(
			   as.matrix(data[i, ])));
		
		v1  <- row[a];
		v2  <- row[b];						
	
		logFC[i]  <- log(mean(v1)/mean(v2), 2);
		pvalue[i] <- t.test(v1, v2, var.equal = TRUE)$p.value; 
	}

	data["logFC"]   <- logFC;
	data["p.value"] <- pvalue;
	data["FDR"]     <- p.adjust(pvalue, method = "fdr", length(pvalue)); 
	
	level <- log(level, 2);
	data["is.DEP"] <- ((abs(logFC) >= level) & (pvalue <= p.value) & data["FDR"] <= 0.05);
	
	return(data);
}

### iTraq结果的差异表达蛋白的检验计算
### 在这个函数之中，输入的表格文件的第一列为蛋白质的id编号，剩下的所有的列都是一个实验设计的比值结果
### 通过与等长的零向量做比较来通过假设检验判断是否是差异表达的？
### @level: 蛋白组分析之中的差异表达的阈值默认为log2(1.5)，对于转录组而言，这里是log2(2) = 1
###         如果信号量的变化值都比较低，可以考虑level参数值取值1.25
### @includes.ZERO 当某一个蛋白的所有的FC值都是零的时候，是否也应该包括为DEP结果？默认不包括
### @fdr.threshold 当FDR校验的阈值设置为1的时候，表示不需要进行FDR校验，则不会再结果之中显示FDR值
logFC.test <- function(data, level = 1.5, p.value = 0.05, fdr.threshold = 0.05, includes.ZERO = FALSE) {
	
	repeatsNumber <- ncol(data) - 1;        # 实验重复数
	ZERO          <- rep(0, repeatsNumber); # 得到等长的进行比较的0向量
	index         <- seq(2, repeatsNumber + 1);
	pvalue        <- rep(0, nrow(data));
	avgFC         <- rep(0, nrow(data));
	log2          <- c();
	
	# 对dataframe之中的每一行都进行计算
	for(i in 1:(nrow(data))) {
		
		row    <- data[i, ];
		# 得到的v向量为FC向量，即v向量之中的值都是一个实验设计之中的
		# 两个实验值的比值
		v      <- as.numeric(as.vector(as.matrix(row[index])));
		valids <- sum(!is.na(v));

		if (ALL.Equals(v, 0)) {
		
			# 所有的值都是0的话，是无法进行假设检验的
			# 但是这种情况可能是实验A之中没有表达量，但是在实验B之中被检测到了表达
			if (includes.ZERO) {
				avgFC[i]  <- 0;
				pvalue[i] <- 0;  # 所有的实验重复都是这种情况，则重复性很好，pvalue非常非常小？？
			} else {
				avgFC[i]  <- NA;
				pvalue[i] <- NA;
			}
						
		} else if (valids > 0) {
		
			v <- v[!is.na(v)];
			l <- repeatsNumber - length(v);
			# 使用1补齐NA
			v <- as.vector(append(v, rep(1, l)));
						
			# 部分数据为零，说明有一部分数据是没有检测到的
			# 但是有一部分数据是被检测到了的
			# 实验的问题？？
			# 则这些部分零值全部设置为1？
			for (p in 1:length(v)) {
				if (v[p] == 0) {
					# 不进行替换的话，在后面的log2计算会计算出NA值，从而导致t.test失败
				    v[p] <- 1;
				}
			}
								
			avgFC [i] <- mean(v, na.rm = TRUE);		
			        v <- log(v, 2);
						        
			# log2(FC) 结果和等长零向量做检验得到pvalue
			pvalue[i] <- t.test(v, ZERO, var.equal = TRUE)$p.value	
			log2  [i] <- log(avgFC[i], 2);
			
		} else {
		
			# 所有的数据都是NA的情况，则无法进行假设检验了
			avgFC [i] <- NA;
			pvalue[i] <- NA;
			log2  [i] <- NA;
		}
	} 
	
	data["FC.avg"]  <- avgFC;
	data["log2FC"]  <- log2;
	data["p.value"] <- pvalue;
	data["FDR"]     <- p.adjust(pvalue, method = "fdr", length(pvalue)); 
				
	# DEP 计算结果	
	downLevel       <- 1 / level;
	
	print(sprintf("DEP levels: (%s, %s)", level, downLevel));
	print(sprintf("    Pvalue:  %s", p.value));
	print(sprintf("       FDR:  %s", fdr.threshold));
	
	if (fdr.threshold < 1) {
		
		print("Apply FDR adjust for DEPs");
		
		data["is.DEP"] <- ((avgFC >= level | avgFC <= downLevel) & 
		                   (pvalue <= p.value) & 
						   (data["FDR"] <= fdr.threshold));
	} else {
		# FDR阈值等于1的时候不进行fdr校验
		data["is.DEP"] <- ((avgFC >= level | avgFC <= downLevel) & 
		                   (pvalue <= p.value));
	}	
	
	print(sprintf("Results %s DEPs in %s proteins!", length(which(data[, "is.DEP"])), nrow(data)));
			
	return(data);
}