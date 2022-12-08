# geneExpression

the gene expression matrix data toolkit

+ [tr](geneExpression/tr.1) do matrix transpose
+ [dims](geneExpression/dims.1) get summary information about the HTS matrix dimensions
+ [as.expr_list](geneExpression/as.expr_list.1) convert the matrix into row gene list
+ [setTag](geneExpression/setTag.1) set a new tag string to the matrix
+ [setZero](geneExpression/setZero.1) set the expression value to zero 
+ [setFeatures](geneExpression/setFeatures.1) set new gene id list to the matrix rows
+ [filterZeroSamples](geneExpression/filterZeroSamples.1) filter out all samples columns which its expression vector is ZERO!
+ [filterNaNMissing](geneExpression/filterNaNMissing.1) set the NaN missing value to default value
+ [load.expr](geneExpression/load.expr.1) load an expressin matrix data
+ [load.expr0](geneExpression/load.expr0.1) read the binary matrix data file
+ [matrix_info](geneExpression/matrix_info.1) get matrix summary information
+ [write.expr_matrix](geneExpression/write.expr_matrix.1) write the gene expression data matrix file
+ [filter](geneExpression/filter.1) Filter the geneID rows
+ [as.generic](geneExpression/as.generic.1) cast the HTS matrix object to the general dataset
+ [average](geneExpression/average.1) calculate average value of the gene expression for
+ [z_score](geneExpression/z_score.1) To avoid the influence of expression level to the 
+ [pca](geneExpression/pca.1) do PCA on a gene expressin matrix
+ [totalSumNorm](geneExpression/totalSumNorm.1) normalize data by sample column
+ [relative](geneExpression/relative.1) normalize data by feature rows
+ [expression.cmeans_pattern](geneExpression/expression.cmeans_pattern.1) This function performs clustering analysis of time course data. 
+ [expression.cmeans3D](geneExpression/expression.cmeans3D.1) run cmeans clustering in 3 patterns
+ [savePattern](geneExpression/savePattern.1) save the cmeans expression pattern result to local file
+ [readPattern](geneExpression/readPattern.1) read the cmeans expression pattern result from file
+ [cmeans_matrix](geneExpression/cmeans_matrix.1) get cluster membership matrix
+ [split.cmeans_clusters](geneExpression/split.cmeans_clusters.1) split the cmeans cluster output into multiple parts based on the cluster tags
+ [peakCMeans](geneExpression/peakCMeans.1) This function performs clustering analysis of time course data
+ [deg.t.test](geneExpression/deg.t.test.1) 
+ [geneId](geneExpression/geneId.1) get gene Id list
+ [as.deg](geneExpression/as.deg.1) create gene expression DEG model
+ [deg.class](geneExpression/deg.class.1) 
+ [joinSample](geneExpression/joinSample.1) do matrix join by samples
