# To assign in the global environment. A simpler, shorter (but not better ... stick with assign) way is to use the <<- operator, ie
#
#    a <<- "new"
#
# inside the function.

# Get current user's home directory
HOME <<- path.expand('~');
# and then combine this HOME directory for initialize this package's configuration data.
cfg <<- sprintf("%s/GCModeller.cli2R.csv", HOME);

# 配置数据项目列表
GCModeller.bin <<- NA;

if(FileIO.FileLength(cfg) <= 0) {
	warning("The GCModeller configuration have not been initialize yet! \nUnless you have already add the GCModeller/bin path into your system's environment variable... \nPlease run 'GCModeller.cfg' function for configs this scripting environment...");
} else {

	# 配置文件存在，读取csv文件，然后赋值给全局的环境变量
	configs <- read.csv(cfg);
	
}

# 将配置数据通过生成一个dataframe对象然后写入csv文件之中作为配置数据
GCModeller.cfg <- function(bin) {
	
}

# library load or install the target package
# @package: The R package name, please notice that, this function 
#           not only works for the CRAN packages, andalso 
#           bioconductor package works too.
Imports <- function(package) {

	if (!require(package)) {
		
		install.packages(package, dependencies = TRUE);
		
		if (!require(package)) {
		
			# try install bioconductor package
			source("https://bioconductor.org/biocLite.R");
			biocLite(package);
		}
	}
}