---
title: Resources
---

# Resources
_namespace: [SMRUCC.genomics.Analysis.RNA_Seq.My.Resources](N-SMRUCC.genomics.Analysis.RNA_Seq.My.Resources.html)_

A strongly-typed resource class, for looking up localized strings, etc.




### Properties

#### BaSAR
Looks up a localized string similar to ##################################################################################
# BaSAR package 
# Emma Granqvist, Matthew Hartley and Richard J Morris
##################################################################################

require(polynom)
require(orthopolynom)

##################################################################################
# Basic functions
##################################################################################

.BSA.linspace <- function(vstart, ve [rest of string was truncated]";.
#### Culture
Overrides the current thread's CurrentUICulture property for all
 resource lookups using this strongly typed resource class.
#### DEseq2_Template
Looks up a localized string similar to ## RNA-seq analysis with DESeq2
## xie.guigang xie.guigang@gcmodeller.org

# RNA-seq data from GSE52202
# http://www.ncbi.nlm.nih.gov/geo/query/acc.cgi?acc=gse52202. All patients with
# ALS, 4 with C9 expansion ("exp"), 4 controls without expansion ("ctl")





# Import & pre-process ----------------------------------------------------

# Import data from raw featureCounts
countData <- read.csv("{countData.MAT.csv}")

# Remove first columns (geneID)
countData <- countData[ ,2:ncol(countData [rest of string was truncated]";.
#### filter
Looks up a localized resource of type System.Byte[].
#### Invoke_DESeq2
Looks up a localized string similar to library('DESeq2')

dir.source <- "<DIR.Source>",

sampleFiles <- c(<*.SAM_FILE_LIST>);
sampleCondition <- c(<Conditions_Corresponding_TO_SampleFiles>);
sampleTable <- data.frame(sampleName=sampleFiles, fileName=sampleFiles, condition=sampleCondition)
ddsHTSeq <- DESeqDataSetFromHTSeqCount(sampleTable=sampleTable, directory=dir.source, design=~condition)
colData(ddsHTSeq)$condition <- factor(colData(ddsHTSeq)$condition, l [rest of string was truncated]";.
#### onLoad
Looks up a localized resource of type System.Byte[].
#### pfsnet
Looks up a localized resource of type System.Byte[].
#### pfsnet_not_rJava
Looks up a localized resource of type System.Byte[].
#### ResourceManager
Returns the cached ResourceManager instance used by this class.
#### WGCNA
Looks up a localized string similar to # inits variables

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

# The following setting is important, do not [rest of string was truncated]";.
