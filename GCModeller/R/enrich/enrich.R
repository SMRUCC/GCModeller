library("clusterProfiler")

singleColumndf2Vector <- function(df) {

    df = as.vector(df)
	df = as.vector(df[,1])
	
    return(df)
}

gobrief = read.csv("G:\\ftp\\go_brief.csv")
go2name = gobrief[, c("goID", "name")]

enrich <- function(DEGs, term2gene, save, nocuts = 0) {

    degs = singleColumndf2Vector(read.table(DEGs, header= FALSE))	
	# ALL = singleColumndf2Vector(read.table(background, header = FALSE))
	
	t2g = read.table(term2gene, header=FALSE)
	
	if (nocuts != 0) {
		result = enricher(degs, TERM2GENE=t2g, pvalueCutoff=1, qvalueCutoff=1, minGSSize =1, TERM2NAME = go2name)
	} else {
		result = enricher(degs, TERM2GENE=t2g, TERM2NAME = go2name)
	}
	
    write.csv(summary(result), save, row.names = FALSE)	
}