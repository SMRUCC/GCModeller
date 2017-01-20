library(tools) 

logFC.test <- function(file, level = 1.5) {

	data          = read.csv(file)
	repeatsNumber = ncol(data) - 1          # 实验重复数
	ZERO          = rep(0, repeatsNumber)   # 得到等长的进行比较的0向量
	index         = seq(2, repeatsNumber+1)
	pvalue        = rep(0, nrow(data))
	avgFC         = rep(0, nrow(data))

	# 对dataframe之中的每一行都进行计算
	for(i in 1:(nrow(data))) {
		
		row    = data[i, ]
		# 得到FC向量
		v      = as.vector(as.matrix(row[index]))
		valids = sum(!is.na(v))

		if (valids > 0) {
			v = v[!is.na(v)]
			l = repeatsNumber - length(v)
			# 使用1补齐NA
			v = as.vector(append(v, rep(1, l)))  
			avgFC[i] = mean(v, na.rm = TRUE)
			avgFC[i]
			v = log(v, 2)
			# log2(FC) 结果和等长零向量做检验得到pvalue
			pvalue[i] = t.test(v, ZERO, var.equal = TRUE)$p.value			
		} else {
			avgFC[i] = NA
			pvalue[i] = NA
		}
	} 
	
	data["FC.avg"]  = avgFC
	data["p.value"] = pvalue
	# DEP 计算结果
	
	downLevel = 1 / level
	data["is.DEP"] = ((avgFC >= level | avgFC <= downLevel) & (pvalue <= 0.05))
	
	DIR <- dirname(file)
	DIR <- paste(DIR, file_path_sans_ext(basename(file)), sep="/")
	out <- paste(DIR, "-log2-t.test.csv")
	
	write.csv(data, out, row.names= FALSE)
}