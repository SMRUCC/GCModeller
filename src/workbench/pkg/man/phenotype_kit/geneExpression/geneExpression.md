# geneExpression

the gene expression matrix data toolkit

+ [exp](geneExpression/exp.1) 
+ [tr](geneExpression/tr.1) do matrix transpose
+ [dims](geneExpression/dims.1) get summary information about the HTS matrix dimensions
+ [as.expr_list](geneExpression/as.expr_list.1) convert the matrix into row gene list
+ [expression_vector](geneExpression/expression_vector.1) get gene expression vector data
+ [setTag](geneExpression/setTag.1) set a new tag string to the matrix
+ [setZero](geneExpression/setZero.1) set the expression value to zero 
+ [sample_id](geneExpression/sample_id.1) get/set new sample id list to the matrix columns
+ [setFeatures](geneExpression/setFeatures.1) set new gene id list to the matrix rows
+ [filterZeroSamples](geneExpression/filterZeroSamples.1) filter out all samples columns which its expression vector is ZERO!
+ [filterZeroGenes](geneExpression/filterZeroGenes.1) removes the rows which all gene expression result is ZERO
+ [filterNaNMissing](geneExpression/filterNaNMissing.1) set the NaN missing value to default value
+ [impute_missing](geneExpression/impute_missing.1) set the zero value to the half of the min positive value
+ [load.expr](geneExpression/load.expr.1) load an expressin matrix data
+ [load.expr0](geneExpression/load.expr0.1) read the binary matrix data file
+ [load.matrixView](geneExpression/load.matrixView.1) Load the HTS matrix into a lazy matrix viewer
+ [matrix_info](geneExpression/matrix_info.1) get matrix summary information
+ [write.expr_matrix](geneExpression/write.expr_matrix.1) write the gene expression data matrix file
+ [project](geneExpression/project.1) make matrix samples column projection
+ [filter](geneExpression/filter.1) Filter the geneID rows
+ [as.generic](geneExpression/as.generic.1) cast the HTS matrix object to the general dataset
+ [mad](geneExpression/mad.1) evaluate the MAD value for each gene features
+ [sort_mad](geneExpression/sort_mad.1) take top n expression feature by rank expression MAD value desc
+ [aggregate_samples](geneExpression/aggregate_samples.1) function alias of the ``geneExpression::average`` function
+ [average](geneExpression/average.1) calculate average value of the gene expression for
+ [z_score](geneExpression/z_score.1) Z-score normalized of the expression data matrix
+ [pca](geneExpression/pca.1) do PCA on a gene expressin matrix
+ [totalSumNorm](geneExpression/totalSumNorm.1) normalize data by sample column
+ [relative](geneExpression/relative.1) normalize data by feature rows
+ [expression.cmeans_pattern](geneExpression/expression.cmeans_pattern.1) This function performs clustering analysis of time course data. 
+ [expression.cmeans3D](geneExpression/expression.cmeans3D.1) run cmeans clustering in 3 patterns
+ [savePattern](geneExpression/savePattern.1) save the cmeans expression pattern result to local file
+ [readPattern](geneExpression/readPattern.1) read the cmeans expression pattern result from file
+ [cmeans_matrix](geneExpression/cmeans_matrix.1) get cluster membership matrix
+ [pattern_representatives](geneExpression/pattern_representatives.1) get the top n representatives genes in each expression pattern
+ [split.cmeans_clusters](geneExpression/split.cmeans_clusters.1) ### split the cmeans cluster output
+ [peakCMeans](geneExpression/peakCMeans.1) ### clustering analysis of time course data
+ [expr_ranking](geneExpression/expr_ranking.1) 
+ [deg.t.test](geneExpression/deg.t.test.1) do t-test across specific analysis comparision
+ [limma](geneExpression/limma.1) The limma algorithm (Linear Models for Microarray Data) is a widely used statistical framework in R/Bioconductor 
+ [read_limma](geneExpression/read_limma.1) 
+ [limma_table](geneExpression/limma_table.1) 
+ [log](geneExpression/log.1) log scale of the HTS raw matrix
+ [minmax01Norm](geneExpression/minmax01Norm.1) min max normalization
+ [take_shuffle](geneExpression/take_shuffle.1) 
+ [geneId](geneExpression/geneId.1) get gene Id list or byref set of the gene id alias set.
+ [as.deg](geneExpression/as.deg.1) create gene expression DEG model
+ [deg.class](geneExpression/deg.class.1) set deg class label
+ [joinSample](geneExpression/joinSample.1) do matrix join by samples
+ [joinFeatures](geneExpression/joinFeatures.1) merge multiple gene expression matrix by gene features
+ [aggregate](geneExpression/aggregate.1) merge row or column where the tag is identical
+ [add_gauss](geneExpression/add_gauss.1) add random gauss noise to the matrix
+ [as.abundance_matrix](geneExpression/as.abundance_matrix.1) 
