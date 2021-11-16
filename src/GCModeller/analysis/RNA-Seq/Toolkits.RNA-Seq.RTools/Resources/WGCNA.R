# inits variables
plotTiff = function(fileName, size, plot) {
	tiff(filename = fileName, width = size[1], height = size[2]);
	plot();
	dev.off();
}

# If necessary, change the path below to the directory where the data files are stored.
# "." means current directory. On Windows use a forward slash / instead of the usual \.
    workingDir = "[WORK]";
       exprCsv = "[dataExpr]";
       TOMsave = "[TOMsave]";
 annotationCsv = "[Annotations.csv]";
   
# Display the current working directory
getwd();
setwd(workingDir);

# Load the package
library(WGCNA);
library(flashClust);

# The following setting is important, do not omit.
options(stringsAsFactors = FALSE);

#Read in the female liver data set
chipData = read.csv(exprCsv);

# Take a quick look at what is in the data set:
  dim(chipData);
names(chipData);

          datExpr0  = as.data.frame(t(chipData[, -c(1)]));
    names(datExpr0) = chipData$[GeneId_LABEL];
 rownames(datExpr0) = names(chipData)[-c(1)];

gsg = goodSamplesGenes(datExpr0, verbose = 3);
               
print(gsg$allOK);

if (!gsg$allOK)
{
	# Optionally, print the gene and sample names that were removed:
	if (sum(!gsg$goodGenes)>0)
		printFlush(paste("Removing genes:", paste(names(datExpr0)[!gsg$goodGenes], collapse = ", ")));
	if (sum(!gsg$goodSamples)>0)
		printFlush(paste("Removing samples:", paste(rownames(datExpr0)[!gsg$goodSamples], collapse = ", ")));

	# Remove the offending genes and samples from the data:
	datExpr0 = datExpr0[gsg$goodSamples, gsg$goodGenes]
}

sampleTree = flashClust(dist(datExpr0), method = "average");

# Plot the sample tree: Open a graphic output window of size 12 by 9 inches
# The user should change the dimensions if the window is too large or too small.
plotTiff("sampleTree-flashClust.tiff", c(3000,2000), function() {
	sizeGrWindow(12,9);
 
	par(cex = 0.6);
	par(mar = c(0,4,2,0));
	
	plot(sampleTree, 
		main     = "Sample clustering to detect outliers", 
		sub      = "", 
		xlab     = "", 
		cex.lab  = 1.5,
		cex.axis = 1.5, 
		cex.main = 2
	);
});

# # Plot a line to show the cut
# abline(h = 15, col = "red");
# # Determine cluster under the line
# clust = cutreeStatic(sampleTree, cutHeight = 15, minSize = 10)
# table(clust)
# # clust 1 contains the samples we want to keep.
# keepSamples = (clust==1)
# datExpr = datExpr0[keepSamples, ]
# nGenes = ncol(datExpr)
# nSamples = nrow(datExpr)

enableWGCNAThreads()

# Choose a set of soft-thresholding powers
 powers = c(c(1:10), seq(from = 12, to=20, by=2))
# Call the network topology analysis function
    sft = pickSoftThreshold(datExpr0, powerVector = powers, verbose = 5)
   cex1 = 0.9;
   
plotTiff("fitIndices.tiff", c(3000,2000), function() {
	# Plot the results:
	sizeGrWindow(9, 5)
	par(mfrow = c(1,2));
	 
	# Scale-free topology fit index as a function of the soft-thresholding power
	plot(sft$fitIndices[,1], -sign(sft$fitIndices[,3])*sft$fitIndices[,2],
		 xlab = "Soft Threshold (power)",
		 ylab = "Scale Free Topology Model Fit,signed R^2",
		 type = "n",
		 main = paste("Scale independence")
	);
	text(sft$fitIndices[,1], -sign(sft$fitIndices[,3])*sft$fitIndices[,2],
		 labels = powers,
		 cex    = cex1,
		 col    = "red"
	);
	# this line corresponds to using an R^2 cut-off of h
	abline(h=0.90,col="red")
});
   
plotTiff("fitIndices.MeanConnectivity.tiff", c(3000,2000), function() {
	# Mean connectivity as a function of the soft-thresholding power
	plot(sft$fitIndices[,1], sft$fitIndices[,5],
		 xlab = "Soft Threshold (power)",
		 ylab = "Mean Connectivity", 
		 type = "n",
		 main = paste("Mean connectivity")
	)
	text(sft$fitIndices[,1], sft$fitIndices[,5], labels=powers, cex=cex1,col="red")
});

net = blockwiseModules(
	datExpr0, 
	power             = 6,
	TOMType           = "unsigned", 
	minModuleSize     = 30,
	reassignThreshold = 0, 
	mergeCutHeight    = 0.25,
	numericLabels     = TRUE, 
	pamRespectsDendro = FALSE,
	saveTOMs          = TRUE,
	saveTOMFileBase   = TOMsave,
	verbose           = 3
);

plotTiff("dendrograms.tiff", c(10000, 7000), function() {
	# open a graphics window
	sizeGrWindow(12, 9)
	# Convert labels to colors for plotting
	mergedColors = labels2colors(net$colors)
	# Plot the dendrogram and the module colors underneath
	plotDendroAndColors(net$dendrograms[[1]], mergedColors[net$blockGenes[[1]]],
						"Module colors",
						dendroLabels = FALSE, 
						hang = 0.03,
						addGuide = TRUE, 
						guideHang = 0.05)
})

 moduleLabels = net$colors
 moduleColors = labels2colors(net$colors)
          MEs = net$MEs;
     geneTree = net$dendrograms[[1]];
	 
save(MEs, moduleLabels, moduleColors, geneTree,
     file = "co-exprNetwork.RData")

write.csv(datExpr0,"datExpr0.csv")
write.csv(moduleColors,"moduleColors.csv")


# nGenes = ncol(datExpr0)
# nSamples = nrow(datExpr0)


# # Calculate topological overlap anew: this could be done more efficiently by saving the TOM
# # calculated during module detection, but let us do it again here.
# dissTOM = 1-TOMsimilarityFromExpr(datExpr0, power = 6);
# # Transform dissTOM with a power to make moderately strong connections more visible in the heatmap
# plotTOM = dissTOM^7;
# # Set diagonal to NA for a nicer plot
# diag(plotTOM) = NA;
# # Call the plot function
# sizeGrWindow(9,9)
# TOMplot(plotTOM, geneTree, moduleColors, main = "Network heatmap plot, all genes")


# nSelect = 400
# # For reproducibility, we set the random seed
# set.seed(10);
# select = sample(nGenes, size = nSelect);
# selectTOM = dissTOM[select, select];
# # There's no simple way of restricting a clustering tree to a subset of genes, so we must re-cluster.
# selectTree = flashClust(as.dist(selectTOM), method = "average")
# selectColors = moduleColors[select];
# # Open a graphical window
# sizeGrWindow(9,9)
# # Taking the dissimilarity to a power, say 10, makes the plot more informative by effectively changing
# # the color palette; setting the diagonal to NA also improves the clarity of the plot
# plotDiss = selectTOM^7;
# diag(plotDiss) = NA;
# TOMplot(plotDiss, selectTree, selectColors, main = "Network heatmap plot, selected genes")

# Recalculate topological overlap if needed
TOM = TOMsimilarityFromExpr(datExpr0, power = 6);
# Read in the annotation file
annot = read.csv(file = annotationCsv);

# Select modules by colors!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
modules <- read.csv("moduleColors.csv");
modules <- modules$x;
modules <- unique(modules);

# Select module probes
      probes = names(datExpr0)
    inModule = is.finite(match(moduleColors, modules));
   modProbes = probes[inModule];
   modGenes  = annot$gene_symbol[match(modProbes, annot$Id)];
# Select the corresponding Topological Overlap
          modTOM = TOM[inModule, inModule];
dimnames(modTOM) = list(modProbes, modProbes)

# Export the network into edge and node list files Cytoscape can read
exportNetworkToCytoscape(modTOM,
     edgeFile = "./CytoscapeEdges.txt",
     nodeFile = "./CytoscapeNodes.txt",
     weighted = TRUE,
    threshold = 0.1,
    nodeNames = modProbes,
 altNodeNames = modGenes,
     nodeAttr = moduleColors[inModule]
);
