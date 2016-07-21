# MicrobiomeGWAS

MicrobiomeGWAS is a software package for identifying host genetic variants associated with micorbiome distance matrix or beta-diversity. For each SNP, microbiome GWAS tests the main effect or the SNP-environment interaction. 

The score statistics have positive skewness and kurtosis, which lead to severely inflated type-I error rates. We solved the problem by correcting the skewness and kurtosis, verified by simulations.    

> https://github.com/lsncibb/microbiomeGWAS


#### Input files
PLINK binary genotype files, a distance matrix and a set of covariates, e.g., PCAs for adjusting population stratefication. 
The current version does not support dosage data from imputation programs.

#### Usage:
- The current version only works on Lunix, Unix and Mac system, does not support Windows
- R and GCC is required on the System
- run Rscript R/microbiomeGWAS_v1.0.R -h, you will get help info:

  The microbiomeGWAS Script
  
  Arguments:
  
  -r	absolute path to the microbiomeGWAS package root, required
  
  -p	plink file pre with absolutte path, required
  
  -d	distance matrix file name with absolutte path, required
  
  -o	absolute path for output results, optional, defalut is the current directory
  
  -c	covariateFile file name with absolutte path, optional
  
  -i	interactive	covariate name in covariateFile, optional
  
  Rscript microbiomeGWAS_Root_Path/R/microbiomeGWAS_v1.0.R -r microbiomeGWAS_Root_Path -p Your_Plink_Path/Plink_Pre -d Your_Dist_Matrix_Path/Dist_Matrix.txt -o Out_Path -c Your_Covariate_Path/Covariate.txt -i Your_Covariate_Name

#### Demo:
- git clone microbiomeGWAS to your local disk, go to the microbiomeGWAS folder, then run the package with demo dataset:

- Rscript R/microbiomeGWAS_v1.0.R -r . -p data/microbiome.GWAS.Demo.data -d data/distMat379.txt -c data/dataCovariate379.txt -i smoke

packageDir: .

pLinkFile: data/microbiome.GWAS.Demo.data

distMatFile: data/distMat379.txt

covariateFile: data/dataCovariate379.txt

interactiveCovariateName: smoke

outDir: .

Starting:

Step 1: Checking input data...Done (0.038s)

Step 2: Calculating the residuals of the distance matrix via linear regression...Done (0.203s)

Step 3: Calculating the expectation of Dij terms in the formula, running 1e+06 permutations...Done (25.434s)

Step 4: Calculating the score test statistic for all SNPs...Done (2.57s)

Step 5: Calculating the Z score, along with the skewness, kurtosis and p values...Done (0.110000000000003s)

All done (total 28.414s)

Writing the output file...Done

The result will be saved as "microbiome.GWAS.Demo.data.result.txt" in the current folder (default) or the directory you specified.


## Memory requirement and computation speed
MicrobiomeGWAS processes one SNP at a time and does not load all genotype data into memory; thus, it requires only memory for storing the distance matrix. The computation time is summarized in the figure for analyzing a GWAS with 500,000 SNPs. "Main": main effect test only. "All": main effect test, interaction test and joint effect test. 


[![Display Figure](https://cloud.githubusercontent.com/assets/15255156/11046333/d8560a36-86fa-11e5-8105-6f644ee5c6d7.png)](https://github.com/lsncibb/microbiomeGWAS/)

## Future extensions
We are extending microbiomeGWAS for testing additive and dominant effects. We are also extending the algorithm to test multiple microbiome beta diversity matrices, e.g., generalized UniFrac, to achieve the optimal statistical power. 

## Reference
Xing Hua, Lei Song, Guoqin Yu, James J. Goedert, Christian C. Abnet, Maria Teresa Landi and Jianxin Shi. MicrobiomeGWAS: a tool for identifying host genetic variants associated with microbiome composition. 

## Contact
* Xing Hua, xing.hua@nih.gov
* Lei Song, lei.song@nih.gov
* Jianxin Shi, Jianxin.Shi@nih.gov

