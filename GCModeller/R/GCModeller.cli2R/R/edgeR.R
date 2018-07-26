library(edgeR);

# 基因表达量的输入文件格式要求有
# 
# ID	experiment1	experiment2	control1	control2
# gene1	expression1	expression2	expression3	expression4
# gene2	expression1	expression2	expression3	expression4
# gene3	expression1	expression2	expression3	expression4
# ...
#
# 这个表格文件可以使用GCModeller的edgeR designer工具生成
# https://github.com/SMRUCC/GCModeller/blob/9e37968688eabe4f6a2e7609a463330a00b54fd7/src/GCModeller/CLI_tools/eggHTS/CLI/2.%20DEP.vb#L19
#
# eggHTS /edgeR.Designer /in "~/project/proteinGroups.csv" /designer "~/project/designer.csv" /label "iBAQ" /deli "_"
#
# designer文件示例
#
# Experiment,Control,GroupLabel
# siRelB1,sictrl1,1
# siRelB2,sictrl2,1
# siRelB3,sictrl3,1
# 17-92-1,ctrl1,2
# 17-92-2,ctrl2,2
# 17-92-3,ctrl3,2

# 运行edgeR进行DEG/DEP的计算分析
# @param table data.frame对象
run.edgeR <- function(table, top = -1) {
	
	repeats <- (ncol(table) - 1) / 2;
	x       <- rep(1, repeats);
	y       <- rep(2, repeats);
	
	# 分组变量 前两个为一组， 后一个为一组， 每个有repeats个重复
	group   <- factor(append(x, y));
	range   <- 2:(ncol(table));
	
	if (top <=0) {
		top <- nrow(table) - 1;
	}
	
	y     <- DGEList(counts=table[, range], group=group); # 构建基因表达列表
	y     <- calcNormFactors(y);                          # 计算样本内标准化因子
	y     <- estimateCommonDisp(y);                       # 计算普通的离散度
	y     <- estimateTagwiseDisp(y);                      # 计算基因间范围内的离散度
	et    <- exactTest(y);                                # 进行精确检验
	
	# 输出排名靠前的差异表达基因信息
	DEG   <- topTags(et, n = top);                        

	return(DEG);	
}

run.edgeR.csv <- function(table.csv, top = -1) {
	table <- read.csv(table.csv); # 读取reads count文件
	table <- assign.Symbol(table);
	
	return(run.edgeR(table, top));
}

# 将dataframe的第一列的编号作为row的名称
assign.Symbol <- function(table) {
	Symbol <- names(table)[1];
	Symbol <- as.vector(table[, Symbol]);
	rownames(table) <- Symbol;
	
	return(table);
}

run.edgeR.tsv <- function(table.tsv, top = -1) {
	table <- read.delim(table.tsv); # 读取reads count文件
	table <- assign.Symbol(table);
	
	return(run.edgeR(table, top));
}