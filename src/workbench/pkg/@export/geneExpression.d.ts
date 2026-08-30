// export R# package module type define for javascript/typescript language
//
//    imports "geneExpression" from "phenotype_kit";
//
// ref=phenotype_kit.geneExpression@phenotype_kit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * the gene expression matrix data toolkit
 * 
 * > This R# package module provides the toolkit for manipulate and analyze the 
 * >  high throughput screening(HTS) gene expression matrix data(the samples in 
 * >  column and the gene features in row):
 * >  
 * >  + read/write the expression matrix data: ``load.expr``, ``load.expr0``, 
 * >    ``write.expr_matrix``, ``load.matrixView``, ``matrix_info``;
 * >  + the matrix data manipulation: ``dims``, ``tr``, ``filter``, ``project``, 
 * >    ``joinSample``, ``joinFeatures``, ``aggregate``, ``sample_id``, 
 * >    ``setFeatures``, ``setTag``, etc;
 * >  + the matrix data normalization and imputation: ``z_score``, ``minmax01Norm``, 
 * >    ``relative``, ``totalSumNorm``, ``impute_missing``, ``filterNaNMissing``, 
 * >    ``setZero``;
 * >  + the expression pattern clustering: ``pca``, ``peakCMeans``, 
 * >    ``expression.cmeans_pattern``, ``cmeans_matrix``, ``pattern_representatives``;
 * >  + the differential expression analysis: ``deg.t.test``, ``limma``, 
 * >    ``limma_impactsort``, ``as.deg``, ``deg.class``.
*/
declare namespace geneExpression {
   /**
    * add random gauss noise to the matrix
    * 
    * 
     * @param x a gene expression matrix object
     * @param scale the scale range of the random gauss noise, the noise value is generated 
     *  from the range ``[-scale, scale]`` for each expression value.
     * 
     * + default value Is ``0.1``.
     * @return the input matrix object that a random gauss noise has been added into each 
     *  expression value of the matrix data.
   */
   function add_gauss(x: object, scale?: number): object;
   /**
    * merge row or column where the tag is identical
    * 
    * 
     * @param x a gene expression matrix object
     * @param byrow default by gene feature row means merge the duplicated genes with the idential gene id as tag.
     *  otherwise will merge the duplicated samples with the identical sample id name.
     * 
     * + default value Is ``true``.
     * @return a new expression matrix object that the duplicated gene feature rows have 
     *  been merged into a single row via the sum value, the tag of the generated 
     *  matrix is formatted as ``aggregate({tag})``.
     *  
     *  NOTE: the merge of the duplicated sample columns(the ``byrow`` parameter is 
     *  FALSE) is not implemented at this moment, an 
     *  @``T:System.NotImplementedException`` will be thrown for such kind of the 
     *  operation.
   */
   function aggregate(x: object, byrow?: boolean): any;
   /**
    * merge the duplicated gene feature rows via the sum value
    * 
    * > this function is a shortcut of the ``aggregate`` api with the ``byrow`` 
    * >  parameter value TRUE.
    * 
     * @param x a gene expression matrix object
     * @return a new expression matrix object that the gene feature rows with the 
     *  identical gene id have been merged into a single row via the sum value, 
     *  the tag of the generated matrix is formatted as ``aggregate({tag})``.
   */
   function aggregate_genes(x: object): object;
   /**
    * calculate the sum value of the gene expression for each sample group.
    *  
    *  this method can be apply for reduce data size when create some plot for 
    *  visualize the gene expression patterns across the sample groups.
    * 
    * 
     * @param matrix a gene expression matrix object
     * @param groups a tuple list of the sample group information: the slot key of the list is 
     *  the sample group label and the slot value is a character vector of the 
     *  sample id that belongs to the corresponding sample group.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a new expression matrix object that each sample column is the sum value of 
     *  the sample columns in the corresponding sample group;
     *  
     *  this function returns a R# error message object if the given sample group 
     *  information is empty.
   */
   function aggregate_samples(matrix: object, groups: object, env?: object): object;
   module as {
      /**
       * create the abundance matrix from a collection of the metagenomics 
       *  abundance data
       * 
       * > a warning message will be pushed into the R# environment message buffer if 
       * >  the abundance data of some sample is nothing, and such kind of the sample 
       * >  will be skipped, an @``T:System.NotImplementedException`` will be thrown if 
       * >  the input data is not a tuple list object or the abundance data type is not 
       * >  supported.
       * 
        * @param samples a tuple list of the abundance data: the slot key of the list is the sample 
        *  id and the slot value is the abundance data of the corresponding sample, 
        *  which can be a collection of the @``T:SMRUCC.genomics.ComponentModel.IExpressionValue`` object, a 
        *  tuple list of the numeric value, or a dictionary object of the abundance 
        *  data(the dictionary key is the taxonomy id and the value is the abundance 
        *  value).
        * @param normalized normalize the generated abundance matrix data? if this parameter is TRUE, 
        *  then the abundance value of each sample column will be normalized as a 
        *  relative abundance value.
        * 
        * + default value Is ``false``.
        * @param env the R# runtime environment object.
        * 
        * + default value Is ``null``.
        * @return a @``T:SMRUCC.genomics.Analysis.HTS.DataFrame.Matrix`` abundance 
        *  matrix object that the rows are the 
        *  taxonomy features and the columns are the input samples.
      */
      function abundance_matrix(samples: any, normalized?: boolean, env?: object): object;
      /**
       * create gene expression DEG model
       * 
       * 
        * @param x usually be a dataframe object of the different expression analysis result
        * @param logFC the column name of the log2 fold change data in the input dataframe.
        * 
        * + default value Is ``'logFC'``.
        * @param pvalue the column name of the p-value data in the input dataframe.
        * 
        * + default value Is ``'pvalue'``.
        * @param label the column name of the gene id label data in the input dataframe.
        * 
        * + default value Is ``'id'``.
        * @param env the R# runtime environment object.
        * 
        * + default value Is ``null``.
        * @return a vector of the @``T:SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner.DEGModel`` deg result data that is created from 
        *  the given dataframe columns;
        *  
        *  this function returns a R# error message object if the input data is not a 
        *  dataframe object.
      */
      function deg(x: any, logFC?: string, pvalue?: string, label?: string, env?: object): object;
      /**
       * convert the matrix into row gene list
       * 
       * 
        * @param expr0 a gene expression matrix object
        * @param dataset cast the expression vector as a named dataset object(the name of each 
        *  element is the corresponding sample id) or just a plain numeric vector?
        * 
        * + default value Is ``false``.
        * @return a tuple list of the expression numeric vector, each slot data 
        *  is the vector of expression value of a gene, slot key name is the 
        *  corresponding gene id.
      */
      function expr_list(expr0: object, dataset?: boolean): object;
      /**
       * cast the HTS matrix object to the general dataset
       * 
       * 
        * @param matrix a gene expression matrix
        * @return A scibasic generic dataset object collection.
      */
      function generic(matrix: object): object;
   }
   /**
    * calculate average value of the gene expression for
    *  each sample group.
    *  
    *  this method can be apply for reduce data size when 
    *  create some plot for visualize the gene expression
    *  patterns across the sample groups.
    * 
    * 
     * @param matrix a gene expression matrix
     * @param sampleinfo The sample group data
     * 
     * + default value Is ``null``.
     * @param strict will try to ignores of the missing sample if strict option is off.
     * 
     * + default value Is ``true``.
     * @return this function return value is determined based on the sampleinfo parameter:
     *  
     *  1. for sampleinfo not nothing, a matrix with sample group as the sample feature data will be returns
     *  2. for missing sampleinfo data, a numeric vector of average value for each gene feature will be returns
   */
   function average(matrix: object, sampleinfo?: object, strict?: boolean): object|number;
   /**
    * get cluster membership matrix
    * 
    * 
     * @param pattern the cmeans clustering result, which can be an 
     *  @``T:SMRUCC.genomics.Visualize.ExpressionPattern.ExpressionPattern`` object, a data frame object of the 
     *  membership matrix, or a pipeline object that produces a set of the 
     *  @``T:Microsoft.VisualBasic.DataMining.KMeans.EntityClusterModel`` cluster model data.
     * @param memberCutoff the membership cutoff value for assign a gene feature into the target 
     *  cluster: the gene feature will be assigned into the cluster if its 
     *  membership value is greater than this threshold ratio of the max membership 
     *  value of the corresponding cluster.
     * 
     * + default value Is ``0.8``.
     * @param empty_shared how many clusters will be assigned to a gene feature when there is no 
     *  cluster that its membership value is greater than the 
     *  ``memberCutoff`` threshold.
     * 
     * + default value Is ``2``.
     * @param max_cluster_shared the max cluster numbers that a gene feature can be assigned into.
     * 
     * + default value Is ``3``.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a vector of the @``T:Microsoft.VisualBasic.DataMining.KMeans.EntityClusterModel`` cluster model data: the 
     *  ``ID`` property is the gene feature id, the ``Properties`` property is the 
     *  membership value of the gene feature to each cluster(``#1``, ``#2``, ...), 
     *  and the ``Cluster`` property is the cluster tag that the gene feature has 
     *  been assigned into(multiple cluster tags are joined by the ``;`` 
     *  character);
     *  
     *  this function returns a R# error message object if the input data can not 
     *  be cast to a collection of the cluster model data.
   */
   function cmeans_matrix(pattern: any, memberCutoff?: number, empty_shared?: object, max_cluster_shared?: object, env?: object): object;
   module deg {
      /**
       * set deg class label
       * 
       * 
        * @param deg a vector of the @``T:SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner.DEGModel`` deg result data for set the class 
        *  label.
        * @param class_labels set deg class label manually;
        *  
        *  if this parameter is not specified, then the class label will be evaluated 
        *  from the log2 fold change and the p-value of each deg result data 
        *  automatically: the deg data will be labelled as ``sig`` when its p-value is 
        *  less than the ``pval_cutoff`` and the absolute value of its log2 fold change 
        *  is greater than the ``logFC`` cutoff, otherwise it will be labelled as 
        *  ``not_sig``.
        * 
        * + default value Is ``null``.
        * @param logFC the log2 fold change cutoff value for evaluate the deg class label.
        * 
        * + default value Is ``1``.
        * @param pval_cutoff the p-value cutoff value for evaluate the deg class label.
        * 
        * + default value Is ``0.05``.
        * @return a new vector of the @``T:SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner.DEGModel`` deg result data that the 
        *  ``class`` property of each deg data has been assigned.
      */
      function class(deg: object, class_labels?: any, logFC?: number, pval_cutoff?: number): object;
      module t {
         /**
          * do t-test across specific analysis comparision
          * 
          * 
           * @param x a gene expression matrix object
           * @param sampleinfo the sample group information data, which is used for get the sample id 
           *  list of the treatment group and the control group.
           * @param treatment group name of the treatment group
           * @param control group name of the control group
           * @param level log2FC cutoff level
           * 
           * + default value Is ``1.5``.
           * @param pvalue the t-test pvalue cutoff
           * 
           * + default value Is ``0.05``.
           * @param FDR the FDR cutoff
           * 
           * + default value Is ``0.05``.
           * @param env the R# runtime environment object.
           * 
           * + default value Is ``null``.
           * @return a vector of the @``T:SMRUCC.genomics.Analysis.HTS.Proteomics.DEP_iTraq`` deg result data of the t-test 
           *  analysis, which is filtered by the given log2FC, p-value and FDR cutoff 
           *  value.
         */
         function test(x: object, sampleinfo: object, treatment: string, control: string, level?: number, pvalue?: number, FDR?: number, env?: object): object;
      }
   }
   /**
    * get summary information about the HTS matrix dimensions
    * 
    * 
     * @param x a HTS data matrix of samples in column and gene features in row
     * @param env 
     * + default value Is ``null``.
     * @return a tuple list that contains the dimension information of the 
     *  gene expression matrix data:
     *  
     *  + feature_size: the number of the matrix rows, or count of genes in matrix
     *  + feature_names: a character vector of the gene ids for each rows
     *  + sample_size: the number of the samples, or number of the matrix columns
     *  + sample_names: the matrix column names, the sample id set
   */
   function dims(x: any, env?: object): object;
   /**
    * power of the expression value in the matrix
    * 
    * > this function implements the ``^`` operator of the gene expression matrix 
    * >  object in R# environment.
    * 
     * @param x a gene expression matrix object
     * @param p the power exponent value
     * @return a new expression matrix object that each expression value in the matrix 
     *  is the p power of the original expression value, the tag of the generated 
     *  matrix is formatted as ``exp({tag}, {p})``.
   */
   function exp(x: object, p: number): object;
   /**
    * make the abundance ranking of the gene features in each sample group
    * 
    * 
     * @param x a gene expression matrix object
     * @param sampleinfo the sample group information data: the gene expression value of the sample 
     *  columns in the same sample group will be averaged at first, and then the 
     *  ranking is evaluated based on the averaged value of each sample group.
     * @return a vector of the @``M:phenotype_kit.geneExpression.ranking(SMRUCC.genomics.Analysis.HTS.DataFrame.Matrix,SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner.SampleInfo[])`` data object: the abundance ranking of 
     *  each gene feature in each sample group.
   */
   function expr_ranking(x: object, sampleinfo: object): object;
   module expression {
      /**
       * This function performs clustering analysis of time course data. 
       *  Calculate gene expression pattern by cmeans algorithm.
       * 
       * 
        * @param matrix the gene expression matrix object which could be generated by 
        *  @``M:phenotype_kit.geneExpression.loadExpression(System.Object,System.String[],System.Boolean,System.Boolean,System.Boolean,SMRUCC.Rsharp.Runtime.Environment)`` api.
        * @param dim the partition matrix size, it is recommended 
        *  that width should be equals to the height of the partition 
        *  matrix.
        * 
        * + default value Is ``'3,3'``.
        * @param fuzzification the cmeans fuzzification parameter
        * 
        * + default value Is ``2``.
        * @param threshold the cmeans threshold parameter
        * 
        * + default value Is ``0.001``.
        * @param env the R# runtime environment object.
        * 
        * + default value Is ``null``.
        * @return an @``T:SMRUCC.genomics.Visualize.ExpressionPattern.ExpressionPattern`` object that contains the cmeans 
        *  clustering result of the input gene expression data: the partition 
        *  patterns of the expression data and the membership value of each gene 
        *  feature to each pattern.
      */
      function cmeans_pattern(matrix: object, dim?: any, fuzzification?: number, threshold?: number, env?: object): object;
      /**
       * run cmeans clustering in 3 patterns
       * 
       * 
        * @param matrix a gene expression matrix object
        * @param fuzzification the cmeans fuzzification parameter
        * 
        * + default value Is ``2``.
        * @param threshold the cmeans threshold parameter
        * 
        * + default value Is ``0.001``.
        * @return an @``T:SMRUCC.genomics.Visualize.ExpressionPattern.ExpressionPattern`` object that the gene expression data 
        *  has been partitioned into 3 clusters.
      */
      function cmeans3D(matrix: object, fuzzification?: number, threshold?: number): object;
   }
   /**
    * get gene expression vector data
    * 
    * 
     * @param x a gene expression matrix object
     * @param geneId the gene feature id, which should be exists in the row names of the given 
     *  expression matrix object.
     * @return a named numeric vector of the target gene expression across multiple 
     *  samples, the name of each element is the corresponding sample id;
     *  
     *  NULL will be returns if the given gene id is not exists in the input 
     *  expression matrix object.
   */
   function expression_vector(x: object, geneId: string): number;
   /**
    * Filter the geneID rows
    * 
    * 
     * @param HTS A gene expression matrix object
     * @param geneId A character vector for run the matrix feature row filter
     * 
     * + default value Is ``null``.
     * @param instr a text pattern for search the gene id of the matrix feature rows, this 
     *  parameter will be used when the ``geneId`` parameter is not specified.
     * 
     * + default value Is ``null``.
     * @param exclude matrix a subset of the data matrix excepts the 
     *  input **`geneId`** features or just make a subset which 
     *  just contains the input **`geneId`** features.
     * 
     * + default value Is ``false``.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return A new expression matrix object that consist with gene feature
     *  rows subset from the original matrix input.
   */
   function filter(HTS: object, geneId?: string, instr?: string, exclude?: boolean, env?: object): object;
   /**
    * set the NaN missing value to default value
    * 
    * 
     * @param x a gene expression matrix object
     * @param missingDefault set NA missing value to zero by default
     * 
     * + default value Is ``0``.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return the input matrix object that all of the NaN or infinity value in the 
     *  expression matrix has been replaced with the given default value.
   */
   function filterNaNMissing(x: object, missingDefault?: number, env?: object): object;
   /**
    * removes the rows which all gene expression result is ZERO
    * 
    * 
     * @param mat a gene expression matrix object
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return A new expression matrix object that with gene row 
     *  features subset from the original input raw matrix object.
   */
   function filterZeroGenes(mat: object, env?: object): object;
   /**
    * filter out all samples columns which its expression vector is ZERO!
    * 
    * 
     * @param mat a gene expression matrix object
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a new expression matrix object that the sample columns which all of the 
     *  expression value is ZERO have been removed.
   */
   function filterZeroSamples(mat: object, env?: object): object;
   /**
    * get gene Id list or byref set of the gene id alias set.
    * 
    * 
     * @param x A collection of the deg/dep object or a raw HTS matrix object
     * @param set_id a character vector of the new gene id list for overwrite the gene id of the 
     *  input matrix object, this parameter is not used if the input data is a 
     *  collection of the deg/dep object.
     * 
     * + default value Is ``null``.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return A collection of the gene id set: the row names of the input expression 
     *  matrix object, or the ``ID`` property of each deg/dep object in the input 
     *  data collection;
     *  
     *  this function returns a R# error message object if the input data can not 
     *  be cast to a collection of the @``T:SMRUCC.genomics.Analysis.HTS.Proteomics.DEP_iTraq`` data.
   */
   function geneId(x: any, set_id?: any, env?: object): string;
   /**
    * set the zero value to the half of the min positive value
    * 
    * > the missing value(the ZERO value or the NaN value) will be filled with the 
    * >  half of the minimum positive value of the corresponding sample column(or 
    * >  gene feature row when the ``by_features`` parameter is TRUE), if there is 
    * >  no positive value in the target sample column, then ZERO will be used.
    * 
     * @param x an expression matrix object that may contains zero
     * @param by_features fill the missing value by the gene feature rows or by the sample columns? 
     *  by default is fill the missing value by the sample columns.
     * 
     * + default value Is ``false``.
     * @return An expression data matrix with missing data filled
   */
   function impute_missing(x: object, by_features?: boolean): object;
   /**
    * check that the given expression matrix object is empty or not
    * 
    * 
     * @param x a gene expression matrix object
     * @return TRUE will be returns if the given expression matrix object is nothing or 
     *  there is no gene feature row and no sample column in the target matrix 
     *  object, otherwise FALSE.
   */
   function is_empty(x: object): boolean;
   /**
    * merge multiple gene expression matrix by gene features
    * 
    * 
     * @param x a collection of the gene expression matrix object for merge, which can be a 
     *  vector of the @``T:SMRUCC.genomics.Analysis.HTS.DataFrame.Matrix`` 
     *  object or a pipeline object that 
     *  produces a set of the expression matrix data.
     * @param strict if this parameter is TRUE, then an error will be thrown when the sample id 
     *  of the input matrix object is not identical with each other, otherwise the 
     *  missing sample column will be filled with ZERO.
     * 
     * + default value Is ``true``.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a new expression matrix object that the gene feature rows of all of the 
     *  input matrix object have been merged into one matrix(the merged sample 
     *  columns are sorted in ascending order);
     *  
     *  this function returns NULL if the input matrix collection is empty, or a R# 
     *  error message object if the input data can not be cast to a collection of 
     *  the expression matrix data.
   */
   function joinFeatures(x: any, strict?: boolean, env?: object): object;
   /**
    * do matrix join by samples
    * 
    * 
     * @param samples matrix in multiple batches data should be normalized at
     *  first before calling this data batch merge function.
     * @param strict if this parameter is TRUE, then an error will be thrown when a gene feature 
     *  is missing from some of the input matrix object, otherwise the missing gene 
     *  feature will be filled with ZERO.
     * 
     * + default value Is ``true``.
     * @return a new expression matrix object that the sample columns of all of the input 
     *  matrix object have been merged into one matrix: the sample columns of the 
     *  matrix in multiple batches data are joined by the gene id of the matrix 
     *  feature rows.
   */
   function joinSample(samples: object, strict?: boolean): object;
   /**
    * The limma algorithm (Linear Models for Microarray Data) is a widely used statistical framework in R/Bioconductor 
    *  for differential expression (DE) analysis of RNA-seq data. Originally designed for microarray studies, its 
    *  flexibility and robustness have extended its utility to RNA-seq through the voomtransformation.
    * 
    * 
     * @param x a gene expression matrix object
     * @param design the experiment design data of the RNA-seq dataset, which describes the 
     *  sample group information and the linear model design of the limma 
     *  analysis.
     * @return a vector of the @``T:SMRUCC.genomics.Analysis.HTS.DataFrame.LimmaTable`` differential expression analysis 
     *  result: the ``logFC``, ``AveExpr``, ``t``, ``P_Value``, ``adj_P_Val`` and 
     *  ``B`` data of each gene feature.
   */
   function limma(x: object, design: object): object;
   /**
    * make the impact sort of the limma differential expression analysis result
    * 
    * 
     * @param x the limma result data, which can be a vector of the 
     *  @``T:SMRUCC.genomics.Analysis.HTS.DataFrame.LimmaTable`` object, a pipeline object of the limma result, or 
     *  a tuple list of the multiple limma result groups(the slot value of the list 
     *  is the limma result of one group).
     * @param top take the top n genes of the impact sort result, by default is all of the 
     *  genes in the input data.
     * 
     * + default value Is ``2147483647``.
     * @param logfc_impact evaluate the impact value of each gene based on the log2 fold change value 
     *  or based on the p-value?
     * 
     * + default value Is ``false``.
     * @param class the id class data, example as: 
     *  list(class1 = c(...), class2 = c(...), class3 = c(...))
     *  
     *  this parameter can also be provided in the format of 
     *  ``list(id1 = "class1", id2 = "class2", ...)``, i.e. each id is mapped to 
     *  its class label directly.
     * 
     * + default value Is ``null``.
     * @param names should be a list of id mapping to name
     * 
     * + default value Is ``null``.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a vector of the @``T:SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner.ImpactResult`` data object that is sorted by 
     *  the impact value in descending order: the ``total`` impact value of each 
     *  gene across all of the input limma result groups, the ``max`` impact value 
     *  and the corresponding ``top_group``, and the class label and the name of 
     *  each gene if the ``class``/``names`` parameter is provided;
     *  
     *  this function returns a R# error message object if the input data can not 
     *  be cast to a collection of the @``T:SMRUCC.genomics.Analysis.HTS.DataFrame.LimmaTable`` data.
   */
   function limma_impactsort(x: any, top?: object, logfc_impact?: boolean, class?: object, names?: object, env?: object): object;
   /**
    * build limma table model from the dataframe columns
    * 
    * 
     * @param id a character vector of the gene id of each gene feature.
     * @param logFC a numeric vector of the log2 fold change value.
     * @param aveExpr a numeric vector of the average expression value.
     * @param t a numeric vector of the moderated t-statistic value.
     * @param pval a numeric vector of the p-value.
     * @param adj_pval a numeric vector of the adjusted p-value.
     * @param b a numeric vector of the log-odds value(B statistic).
     * @return a vector of the @``T:SMRUCC.genomics.Analysis.HTS.DataFrame.LimmaTable`` object that is created from the 
     *  given column data, all of the input column vectors should be in the same 
     *  size as the input gene id vector.
   */
   function limma_table(id: any, logFC: any, aveExpr: any, t: any, pval: any, adj_pval: any, b: any): object;
   module load {
      /**
       * load an expressin matrix data
       * 
       * > the table file format that handled by this function
       * >  could be a csv table file or tsv table file.
       * 
        * @param file the file path or the file stream data of the target 
        *  expression matrix table file, or the expression data frame object
        * @param exclude_samples will removes some sample column data from the expression
        *  matrix which is specificed by this parameter value.
        * 
        * + default value Is ``null``.
        * @param rm_ZERO removes the gene feature rows that all of the expression value in the 
        *  target row is ZERO?
        * 
        * + default value Is ``false``.
        * @param makeNames create the gene id name via the generic make names function when the 
        *  ``makeUnique`` parameter is TRUE? if this parameter is FALSE, then the 
        *  duplicated gene id will be renamed via an unique numeric suffix.
        * 
        * + default value Is ``false``.
        * @param makeUnique make the gene id of the loaded expression matrix unique? default is TRUE.
        * 
        * + default value Is ``true``.
        * @param env the R# runtime environment object.
        * 
        * + default value Is ``null``.
        * @return a HTS data matrix of samples in column and gene features in row
      */
      function expr(file: any, exclude_samples?: string, rm_ZERO?: boolean, makeNames?: boolean, makeUnique?: boolean, env?: object): object;
      /**
       * read the binary matrix data file
       * 
       * 
        * @param file the file path of the binary expression matrix data file, or a file stream 
        *  object of the target binary matrix data.
        * @param lazy load the binary matrix data in a lazy stream reader mode? if this 
        *  parameter is TRUE, then a @``T:SMRUCC.genomics.Analysis.HTS.DataFrame.HTSMatrixReader`` object will be 
        *  returned instead of loading all of the matrix data into the memory at 
        *  once, which is helpful for read a huge binary matrix data file.
        * 
        * + default value Is ``false``.
        * @param env the R# runtime environment object.
        * 
        * + default value Is ``null``.
        * @return a HTS data matrix of samples in column and gene features in row, or a lazy 
        *  @``T:SMRUCC.genomics.Analysis.HTS.DataFrame.HTSMatrixReader`` matrix reader object when the ``lazy`` 
        *  parameter is TRUE;
        *  
        *  this function returns a R# error message object if the given file can not 
        *  be opened for read.
      */
      function expr0(file: any, lazy?: boolean, env?: object): object|object;
      /**
       * Load the HTS matrix into a lazy matrix viewer
       * 
       * 
        * @param mat a gene expression matrix object for create the lazy data viewer.
        * @return an @``T:SMRUCC.genomics.Analysis.HTS.DataFrame.HTSMatrixViewer`` object that provides the random access 
        *  of the gene expression data in the input matrix object without keeps all 
        *  of the data in the memory.
      */
      function matrixView(mat: object): object;
   }
   /**
    * log scale of the HTS raw matrix
    * 
    * > the function name of this api is conflict with the math ``log`` function in 
    * >  the R# base runtime environment: if the input data is a gene expression 
    * >  matrix object, then the log scale of the expression matrix will be 
    * >  returned, otherwise the math log of the input numeric vector will be 
    * >  returned.
    * 
     * @param expr should be a HTS expression matrix object, or a numeric vector of the raw 
     *  expression data.
     * @param base the logarithm base value, by default is the natural 
     *  logarithm(the base e).
     * 
     * + default value Is ``2.718281828459045``.
     * @return a new expression matrix object(or a numeric vector) of the log scaled 
     *  expression data;
     *  
     *  this function may produce negative expression value if the value number is less than 1.
   */
   function log(expr: any, base?: number): object;
   /**
    * evaluate the MAD value for each gene features
    * 
    * 
     * @param x a gene expression matrix object
     * @return a named list of the MAD(median absolute deviation) value of each gene 
     *  feature row, the name of the list element is the corresponding gene id.
   */
   function mad(x: object): object;
   /**
    * get matrix summary information
    * 
    * > the summary information of a csv/tsv/xls table file is not implemented at 
    * >  this moment, an @``T:System.NotImplementedException`` will be thrown for 
    * >  such kind of the input file.
    * 
     * @param file could be a file path or the HTS matrix data object
     * @return A tuple list object that contains the data information
     *  which is extract from the given file:
     *  
     *  1. sampleID: a character vector that contains the matrix sample information(column features name)
     *  2. geneID: a character vector that contains the matrix gene features information(row features name)
     *  3. tag: the matrix source tag label, could be the file basename if the given input file is a file path to the matrix.
     *  
     *  if the input **`file`** object is a 
     *  @``T:SMRUCC.genomics.Analysis.HTS.DataFrame.Matrix`` expression 
     *  matrix object, then an additional ``mad`` data slot will be 
     *  added into the result list: the MAD value of each gene feature row.
   */
   function matrix_info(file: any): object;
   /**
    * min max normalization
    *  
    *  (row - min(row)) / (max(row) - min(row))
    *  
    *  this normalization method is usually used for the metabolomics data
    * 
    * 
     * @param x a gene expression matrix object
     * @return a new expression matrix object that the expression value of each gene 
     *  feature row has been scaled into the range ``[0, 1]`` via the min-max 
     *  normalization.
   */
   function minmax01Norm(x: object): any;
   /**
    * get the top n representatives genes in each expression pattern
    * 
    * 
     * @param pattern an @``T:SMRUCC.genomics.Visualize.ExpressionPattern.ExpressionPattern`` object of the cmeans clustering result.
     * @param top top n cmeans membership items
     * 
     * + default value Is ``3``.
     * @return a tuple list of the representative gene id set: the slot key of the list is 
     *  the cluster tag(``#1``, ``#2``, ...) and the slot value is a character 
     *  vector of the gene id of the top n members in the corresponding cluster, 
     *  which is sorted by the membership value in descending order.
   */
   function pattern_representatives(pattern: object, top?: object): any;
   /**
    * do PCA on a gene expressin matrix
    * 
    * 
     * @param x a gene expression matrix
     * @param npc the max number of the principal components for calculate, the PCA analysis 
     *  is applied on the gene expression matrix in row(gene) and column(sample) 
     *  mode: each gene feature row is an observation and each sample column is a 
     *  data dimension.
     * 
     * + default value Is ``3``.
     * @return a data frame object of the PCA score result: each row is a gene feature in 
     *  the input expression matrix(the row name is the gene id), and the columns 
     *  are the principal component scores: ``PC1``, ``PC2``, ... ``PC{npc}``.
   */
   function pca(x: object, npc?: object): object;
   /**
    * ### clustering analysis of time course data
    *  
    *  This function performs clustering analysis of time course data
    * 
    * 
     * @param matrix A gene expression data matrix object
     * @param nsize the layout of the cmeans clustering visualization
     * 
     * + default value Is ``'3,3'``.
     * @param threshold the cmeans threshold
     * 
     * + default value Is ``10``.
     * @param fuzzification cmeans fuzzification parameter
     * 
     * + default value Is ``2``.
     * @param plotSize the image size of the cmeans plot
     * 
     * + default value Is ``'8100,5200'``.
     * @param colorSet the color palatte name
     * 
     * + default value Is ``'Jet'``.
     * @param memberCutoff the cmeans membership cutoff value for create a molecule cluster
     * 
     * + default value Is ``0.8``.
     * @param empty_shared how many clusters will be assigned to a gene feature when there is no 
     *  cluster that its membership value is greater than the ``memberCutoff`` 
     *  threshold.
     * 
     * + default value Is ``2``.
     * @param max_cluster_shared the max cluster numbers that a gene feature can be assigned into.
     * 
     * + default value Is ``3``.
     * @param xlab the x axis label text of the cmeans pattern plot.
     * 
     * + default value Is ``'Spatial Regions'``.
     * @param ylab the y axis label text of the cmeans pattern plot.
     * 
     * + default value Is ``'z-score(Normalized Intensity)'``.
     * @param top_members the ratio of the top members of each cluster for draw the expression 
     *  pattern lines in the cmeans pattern plot.
     * 
     * + default value Is ``0.2``.
     * @param margin the plot padding css style of the cmeans pattern plot.
     * 
     * + default value Is ``'padding:100px 100px 300px 100px;'``.
     * @param cluster_label_css the css style of the cluster label text.
     * 
     * + default value Is ``'font-style: normal; font-size: 20; font-family: Bookman Old Style;'``.
     * @param legend_title_css the css style of the legend title text.
     * 
     * + default value Is ``'font-style: normal; font-size: 10; font-family: Microsoft YaHei;'``.
     * @param legend_tick_css the css style of the legend tick text.
     * 
     * + default value Is ``'font-style: normal; font-size: 10; font-family: Microsoft YaHei;'``.
     * @param axis_tick_css the css style of the axis tick text.
     * 
     * + default value Is ``'font-style: normal; font-size: 12; font-family: Segoe UI;'``.
     * @param axis_label_css the css style of the axis label text.
     * 
     * + default value Is ``'font-style: normal; font-size: 10; font-family: Microsoft YaHei;'``.
     * @param grid_fill the background fill color of the plot grid.
     * 
     * + default value Is ``'LightGray'``.
     * @param grid_draw draw the plot grid lines or not?
     * 
     * + default value Is ``true``.
     * @param x_lab_rotate the rotate angle of the x axis label text.
     * 
     * + default value Is ``45``.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return this function returns a tuple list that contains the pattern 
     *  cluster matrix and the cmeans pattern plots.
     *  
     *  1. 'pattern' is a vector of the @``T:Microsoft.VisualBasic.DataMining.KMeans.EntityClusterModel`` data that contains the object cluster patterns
     *  2. 'image' is a bitmap image that plot based on the object cluster patterns data.
     *  3. 'pdf' is a pdf image that could be edit
     *  4. 'cmeans' is the raw @``T:SMRUCC.genomics.Visualize.ExpressionPattern.ExpressionPattern`` object of the cmeans clustering result
     *  
     *  NULL will be returns if the given expression matrix is empty.
   */
   function peakCMeans(matrix: object, nsize?: any, threshold?: number, fuzzification?: number, plotSize?: any, colorSet?: string, memberCutoff?: number, empty_shared?: object, max_cluster_shared?: object, xlab?: string, ylab?: string, top_members?: number, margin?: any, cluster_label_css?: string, legend_title_css?: string, legend_tick_css?: string, axis_tick_css?: string, axis_label_css?: string, grid_fill?: string, grid_draw?: boolean, x_lab_rotate?: number, env?: object): any;
   /**
    * make matrix samples column projection
    * 
    * 
     * @param x a gene expression matrix object
     * @param sampleIds a character vector of the sample id for make the matrix column subset.
     * @return a new expression matrix object that only contains the sample columns which 
     *  is specified by the ``sampleIds`` parameter, and the sample columns in the 
     *  generated matrix are in the same order as the given sample id vector;
     *  
     *  NULL will be returns if the given sample id vector is nothing or is an 
     *  empty vector.
   */
   function project(x: object, sampleIds: any): object;
   /**
    * read the limma result table from a given csv table file
    * 
    * 
     * @param file the file path of the limma result table file, which is usually generated 
     *  by the R limma package.
     * @return a vector of the @``T:SMRUCC.genomics.Analysis.HTS.DataFrame.LimmaTable`` differential expression analysis 
     *  result data.
   */
   function read_limma(file: string): object;
   /**
    * read the cmeans expression pattern result from file
    * 
    * > this function can also read the csv matrix file and 
    * >  then cast as the expression pattern data object.
    * 
     * @param file a binary data pack file that contains the expression pattern raw data.
     *  if this file is given by a csv file, then this csv file should be the cmeans cluster 
     *  membership matrix outtput.
     * @param samples should be a csv file path to the sample matrix data if the input **`file`**
     *  is a csv membership matrix file.
     * 
     * + default value Is ``null``.
     * @return an @``T:SMRUCC.genomics.Visualize.ExpressionPattern.ExpressionPattern`` object that read from the given binary 
     *  data pack file or the csv membership matrix file.
   */
   function readPattern(file: string, samples?: string): object;
   /**
    * normalize data by feature rows
    * 
    * > row/max(row)
    * 
     * @param matrix a gene expression matrix
     * @param median normalize the matrix row by median value of each row?
     * 
     * + default value Is ``false``.
     * @return a new expression matrix object which is normalized by the relative scale 
     *  of each gene feature row: each expression value in a gene feature row is 
     *  divided by the max value(or the median value when the ``median`` parameter 
     *  is TRUE) of the corresponding gene feature row, the tag of the generated 
     *  matrix is formatted as ``relative_scaale({tag})``.
   */
   function relative(matrix: object, median?: boolean): object;
   /**
    * Calculate the sum of the sample data with time-series information across all time points to obtain the area under the curve (AUC) of the time-series curve.
    * 
    * 
     * @param x a gene expression matrix object
     * @param sampleinfo the sample time-series information data, which can be a vector of the 
     *  @``T:SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner.SampleInfo`` object, a dataframe object or a pipeline object 
     *  that produces a set of the @``T:SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner.SampleInfo`` data.
     * @param sample the property name of the time point information in the given 
     *  ``sampleinfo`` data, by default is the ``sample`` property.
     * 
     * + default value Is ``'sample'``.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a new expression matrix object that each sample column is the sum value(AUC) 
     *  of the sample columns in the corresponding time point, the tag of the 
     *  generated matrix is ``AUC(time)``;
     *  
     *  this function returns a R# error message object if the given sample 
     *  information data can not be cast to a collection of the 
     *  @``T:SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner.SampleInfo`` data.
   */
   function sample_auc(x: object, sampleinfo: any, sample?: string, env?: object): any;
   /**
    * get/set new sample id list to the matrix columns
    * 
    * > it is kind of ``colnames`` liked function for dataframe object.
    * 
     * @param x target gene expression matrix object
     * @param sample_ids a character vector of the new sample id list for
     *  set to the sample columns of the gene expression 
     *  matrix.
     * 
     * + default value Is ``null``.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return this function will get sample_id character vector from the input matrix if the 
     *  **`sample_ids`** parameter is missing, otherwise it will set the new 
     *  sample id list to the input matrix object and return the modified matrix object.
     *  
     *  if the input **`x`** object is not a valid gene expression matrix object,
     *  then a error message object will be returned.
   */
   function sample_id(x: any, sample_ids?: string, env?: object): object|object|string;
   /**
    * save the cmeans expression pattern result to local file
    * 
    * 
     * @param pattern an @``T:SMRUCC.genomics.Visualize.ExpressionPattern.ExpressionPattern`` object that is created by the 
     *  ``expression.cmeans_pattern`` or ``peakCMeans`` api.
     * @param file the file path of the binary data pack file for save the expression pattern 
     *  result.
     * @return a boolean value for indicates that the expression pattern data has been 
     *  saved into the target file successfully or not.
   */
   function savePattern(pattern: object, file: string): boolean;
   /**
    * set new gene id list to the matrix rows
    * 
    * > it is kind of ``rownames`` liked function for dataframe object.
    * 
     * @param x target gene expression matrix object
     * @param gene_ids a collection of the new gene ids to set to the feature
     *  rows of the gene expression matrix.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return the input matrix object that the gene id of each feature row has been 
     *  modified;
     *  
     *  if the size of the given gene id vector is not equals to the feature row 
     *  numbers of the input matrix, or the input **`x`** object is 
     *  not a valid gene expression matrix object, then a R# error message object 
     *  will be returned.
   */
   function setFeatures(x: any, gene_ids: string, env?: object): object|object;
   /**
    * set a new tag string to the matrix
    * 
    * 
     * @param expr0 the target gene expression matrix object
     * @param tag a new tag label text for set to the given expression matrix object.
     * @return the input matrix object that its tag data has been modified.
   */
   function setTag(expr0: object, tag: string): object;
   /**
    * set the expression value to zero 
    *  
    *  if the expression value is less than a given threshold
    * 
    * > the quantile cutoff value is evaluated for each gene feature row 
    * >  independently, so that the row of the low expression level will not be 
    * >  wiped out entirely.
    * 
     * @param expr0 the target gene expression matrix object
     * @param q the quantile threshold value of each gene feature row for make the 
     *  expression value as ZERO.
     * 
     * + default value Is ``0.1``.
     * @return the input matrix object that the expression value which is less than or 
     *  equals to the quantile cutoff of the corresponding gene feature row has 
     *  been set to ZERO.
   */
   function setZero(expr0: object, q?: number): object;
   /**
    * take top n expression feature by rank expression MAD value desc
    * 
    * 
     * @param x a gene expression matrix object
     * @param top take top N gene features
     * 
     * + default value Is ``10000``.
     * @return a new expression matrix object that only contains the top n gene feature 
     *  rows which is sorted by the MAD value of each gene feature row in 
     *  descending order, the tag of the generated matrix is formatted as 
     *  ``sort_mad({tag})``.
   */
   function sort_mad(x: object, top?: object): object;
   module split {
      /**
       * ### split the cmeans cluster output
       *  
       *  split the cmeans cluster output into multiple parts based on the cluster tags
       * 
       * 
        * @param cmeans the cmeans cluster result
        * @return A list object that contains the input cluster result 
        *  data is split into multiple cluster parts.
      */
      function cmeans_clusters(cmeans: object): any;
   }
   /**
    * random takes a subset of the gene features from the expression matrix
    * 
    * 
     * @param x a gene expression matrix object
     * @param n the sample size of the gene features for takes from the input expression 
     *  matrix object.
     * @return a data frame object that contains the randomly selected gene features: each 
     *  row is a randomly selected gene feature(the row name is the gene id) and 
     *  the columns is the expression data of the corresponding gene.
   */
   function take_shuffle(x: object, n: object): object;
   /**
    * Make time pattern label for a specific cmeans expression pattern
    * 
    * 
     * @param expr_z A specifc cmeans epxression pattern, row is gene and column is the expression data in a time point, should be sort in asc order.
     * @return A character of the pattern label name
   */
   function time_pattern_label(expr_z: object): string;
   /**
    * normalize data by sample column
    * 
    * > apply for the metabolomics data usually
    * 
     * @param matrix a gene expression matrix
     * @param scale the total sum scale of each sample column after the normalization, by 
     *  default is 10000.
     * 
     * + default value Is ``10000``.
     * @return a new expression matrix object which is normalized by the total sum value 
     *  of each sample column: each expression value in a sample column is divided 
     *  by the sum of the sample column and then multiplied by the given scale 
     *  value.
   */
   function totalSumNorm(matrix: object, scale?: number): object;
   /**
    * do matrix transpose
    * 
    * 
     * @param mat the target gene expression matrix object for make transpose.
     * @return a transposed matrix object: the sample columns of the input matrix will 
     *  become the gene feature rows of the generated matrix and vice versa.
   */
   function tr(mat: object): object;
   module write {
      /**
       * write the gene expression data matrix file
       * 
       * 
        * @param expr The gene expression matrix object
        * @param file The file path to a csv matrix file that used 
        *  for export the given **`expr`** matrix data.
        * @param id The string content inside the first cell
        * 
        * + default value Is ``'geneID'``.
        * @param binary write matrix data in binary data format? default value 
        *  is False means write matrix as csv matrix file.
        * 
        * + default value Is ``false``.
        * @return A logical vector for indicates that the expression 
        *  matrix save success or not.
      */
      function expr_matrix(expr: object, file: string, id?: string, binary?: boolean): boolean;
   }
   /**
    * Z-score normalized of the expression data matrix
    *  
    *  To avoid the influence of expression level to the 
    *  clustering analysis, z-score transformation can 
    *  be applied to covert the expression values to 
    *  z-scores by performing the following formula:
    *  
    *  ```
    *  z = (x - u) / sd
    *  ```
    *  
    *  x is value to be converted (e.g., a expression value 
    *  of a genomic feature in one condition), µ is the 
    *  population mean (e.g., average expression value Of 
    *  a genomic feature In different conditions), σ Is the 
    *  standard deviation (e.g., standard deviation of 
    *  expression of a genomic feature in different conditions).
    * 
    * > #### Standard score(z-score)
    * >  
    * >  In statistics, the standard score is the signed number of standard deviations by which the value of 
    * >  an observation or data point is above the mean value of what is being observed or measured. Observed 
    * >  values above the mean have positive standard scores, while values below the mean have negative 
    * >  standard scores. The standard score is a dimensionless quantity obtained by subtracting the population 
    * >  mean from an individual raw score and then dividing the difference by the population standard deviation. 
    * >  This conversion process is called standardizing or normalizing (however, "normalizing" can refer to 
    * >  many types of ratios; see normalization for more).
    * >  
    * >  > https://en.wikipedia.org/wiki/Standard_score
    * 
     * @param x a gene expression matrix
     * @return the HTS matrix object has been normalized in each gene 
     *  expression row, z-score is calculated for each gene row
     *  across multiple sample expression data.
   */
   function z_score(x: object): object;
}
