# http://blog.csdn.net/skenoy/article/details/8920707

library(WGCNA)  
options(stringsAsFactors = FALSE)  
myData = read.table("new.txt", sep="\t", header=TRUE)  
dim(myData)  
names(myData)  
datExpr = as.data.frame(t(myData[, -c(1)]))  
names(datExpr) = myData$inputID  
rownames(datExpr) = names(myData)[-c(1)]  
gsg = goodSamplesGenes(datExpr, verbose = 3)  
if(!gsg$allOK)  
{  
	if (sum(!gsg$goodGenes)>0)  
		printFlush(paste("Removing genes:", paste(names(datExpr)[!gsg$goodGenes], collapse = ", ")))  
	if (sum(!gsg$goodSamples)>0)  
		printFlush(paste("Removing samples:", paste(rownames(datExpr)[!gsg$goodSamples], collapse = ", ")))  
    datExpr = datExpr[gsg$goodSamples, gsg$goodGenes]  
}  
write.table(names(datExpr)[!gsg$goodGenes], file="Out/removeGene.xls", row.names=FALSE, col.names=FALSE, quote=FALSE)  
write.table(names(datExpr)[!gsg$goodSamples], file="Out/removeSample.xls", row.names=FALSE, col.names=FALSE, quote=FALSE)  
sampleTree = flashClust(dist(datExpr), method = "average") #根据样本表达量使用平均距离法建树  
pdf(file = "Out/sampleClustering.pdf", width = 12, height = 9)  
par(cex = 0.6)  
par(mar = c(0,4,2,0))  
plot(sampleTree, main = "Sample clustering", sub="", xlab="", cex.lab = 1.5, cex.axis = 1.5, cex.main = 2)  
dev.off()  
save(datExpr, file = "dataInput.RData")  


# ==============================================

library(WGCNA)  
options(stringsAsFactors = FALSE)  
enableWGCNAThreads()  
lnames = load(file ="dataInput.RData")  
powers = c(c(1:10), seq(from = 12, to=20, by=2))  
sft = pickSoftThreshold(datExpr, powerVector = powers, verbose = 5)  
sizeGrWindow(9, 5)  
par(mfrow = c(1,2))  
cex1 = 0.9  
plot(sft$fitIndices[,1], -sign(sft$fitIndices[,3])*sft$fitIndices[,2],xlab="Soft Threshold (power)",ylab="Scale Free Topology Model Fit,signed R^2",type="n", main = paste("Scale independence"))  
text(sft$fitIndices[,1], -sign(sft$fitIndices[,3])*sft$fitIndices[,2],labels=powers,cex=cex1,col="red")  
plot(sft$fitIndices[,1], sft$fitIndices[,5],xlab="Soft Threshold (power)",ylab="Mean Connectivity", type="n",main = paste("Mean connectivity"))  
text(sft$fitIndices[,1], sft$fitIndices[,5], labels=powers, cex=cex1,col="red")  
softPower = as.integer(readline("which softPower: ")) #根据图像选择拓扑结构树的阈值  
adjacency = adjacency(datExpr, power = softPower) #计算树之间的邻接性  
TOM = TOMsimilarity(adjacency) #计算树之间的相似性  
dissTOM = 1-TOM  
geneTree = flashClust(as.dist(dissTOM), method = "average") #根据树之间的不相似度建立树状结构  
minModuleSize = as.integer(readline("choose a minModuleSize(10): "))  
dynamicMods = cutreeDynamic(dendro = geneTree, distM = dissTOM, deepSplit = 2, pamRespectsDendro = FALSE, minClusterSize = minModuleSize) #建立模块  
dynamicColors = labels2colors(dynamicMods)  
MEList = moduleEigengenes(datExpr, colors = dynamicColors)  
MEs = MEList$eigengenes #提取hubgene  
MEDiss = 1-cor(MEs)  
METree = flashClust(as.dist(MEDiss), method = "average")  
sizeGrWindow(7, 6)  
plot(METree, main = "Clustering of module eigengenes", xlab = "", sub = "") #根据hubgene之间的距离建树  
MEDissThres = as.double(readline("choose a disSimilarity(0.25): ")) #选择一个相似度较低的阈值确认模块  
pdf(file = "Out/clusterModuleEigengenes.pdf", width = 7, height = 6)  
plot(METree, main = "Clustering of module eigengenes", xlab = "", sub = "")  
abline(h=MEDissThres, col = "red")  
dev.off()  
merge = mergeCloseModules(datExpr, dynamicColors, cutHeight = MEDissThres, verbose = 3) #将以上模块进行融合  
mergedColors = merge$colors  
mergedMEs = merge$newMEs  
pdf(file = "Out/mergedModuleTree.pdf", width = 12, height = 9)  
plotDendroAndColors(geneTree, cbind(dynamicColors, mergedColors), c("Dynamic Tree Cut", "Merged dynamic"), dendroLabels = FALSE, hang = 0.03, addGuide = TRUE, guideHang = 0.05)  
dev.off()  
moduleColors = mergedColors  
colorOrder = c("grey", standardColors(50))  
moduleLabels = match(moduleColors, colorOrder) - 1  
MEs = mergedMEs  
write.table(paste(colnames(datExpr), moduleColors, sep = "\t"), file="Out/netcolor2gene.xls", row.names=FALSE, quote=FALSE)  
save(MEs, moduleLabels, moduleColors, geneTree,file = "networkConstruction.RData")  


# ========================
library(WGCNA) #GO富集信息  
options(stringsAsFactors = FALSE)  
lnames = load(file = "dataInput.RData")  
lnames = load(file ="networkConstruction.RData")  
annot = read.table(file = "gene_symbol_id", sep = "\t", header = TRUE)  
probes = names(datExpr)  
probes2annot = match(probes, annot$inputID)  
allLLIDs = annot$geneID[probes2annot]  
specie = readline("choose a specie(human mouse rat malaria yeast fly bovine worm canine zebrafish chicken): ")  
gonum = as.integer(readline("choose a maxGONumber: "))  
GOenr = GOenrichmentAnalysis(moduleColors, allLLIDs, organism = specie, nBestP = gonum)  
tab = GOenr$bestPTerms[[4]]$enrichment  
write.table(tab, file = "Out/GOEnrichmentTable.xls", sep = "\t", quote = FALSE, row.names = TRUE)  

# =============================
library(WGCNA) #建立模块与模块之间的关系图  
options(stringsAsFactors = FALSE)  
enableWGCNAThreads()  
lnames = load(file ="dataInput.RData")  
lnames = load(file ="networkConstruction.RData")  
nGenes = ncol(datExpr)  
nSamples = nrow(datExpr)  
softpower = as.integer(readline("which softPower: "))  
dissTOM = 1 - TOMsimilarityFromExpr(datExpr, power = softpower)  
nSelect = as.integer(readline("choose a maxGeneNumber(1500): "))  
set.seed(10)  
select = sample(nGenes, size = nSelect)  
selectTOM = dissTOM[select, select]  
selectTree = flashClust(as.dist(selectTOM), method = "average")  
selectColors = moduleColors[select]  
plotDiss = selectTOM^7  
diag(plotDiss) = NA  
pdf(file = "Out/networkHeatmap.pdf", width = 15, height = 15)  
TOMplot(plotDiss, selectTree, selectColors, main = "Network heatmap plot")  
dev.off()  
save(dissTOM, file = "dissTOM.RData")  
MEs = moduleEigengenes(datExpr, moduleColors)$eigengenes  
MET = orderMEs(MEs)  
pdf(file = "Out/hubGeneHeatmap.pdf", width = 6, height = 6)  
plotEigengeneNetworks(MET, "Eigengene adjacency heatmap", marHeatmap = c(3,4,2,2), plotDendrograms = FALSE, xLabelsAngle = 90)  
dev.off()  

# ===================================
library(WGCNA) #将感兴趣的模块提取hubgene用VisAnt软件进行观察其关系  
options(stringsAsFactors = FALSE)  
enableWGCNAThreads()  
lnames = load(file ="dataInput.RData")  
lnames = load(file ="networkConstruction.RData")  
lnames = load(file ="dissTOM.RData")  
annot = read.table(file = "geneAnnotation.txt", sep = "\t", header = TRUE)  
TOM = 1 - dissTOM  
module = readline("choose a color: ")  
probes = names(datExpr)  
inModule = (moduleColors==module)  
modProbes = probes[inModule]  
modTOM = TOM[inModule, inModule]  
dimnames(modTOM) = list(modProbes, modProbes)  
nTop = as.integer(readline("choose a maxGeneNumber(30): "))  
IMConn = softConnectivity(datExpr[, modProbes])  
top = (rank(-IMConn) <= nTop)  
thConnect = as.double(readline("choose a minConnectThreshold(0.01): "))  
vis = exportNetworkToVisANT(modTOM[top, top], file = paste("Out/VisANTInput-", module, "-", nTop, ".txt", sep=""), weighted = TRUE, threshold = thConnect, probeToGene = data.frame(annot$inputID, annot$geneSymbol))  