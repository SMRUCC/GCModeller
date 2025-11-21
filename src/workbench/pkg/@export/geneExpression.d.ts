// export R# package module type define for javascript/typescript language
//
//    imports "geneExpression" from "phenotype_kit";
//
// ref=phenotype_kit.geneExpression@phenotype_kit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * the gene expression matrix data toolkit
 * 
*/
declare namespace geneExpression {
   /**
    * add random gauss noise to the matrix
    * 
    * 
     * @param x -
     * @param scale -
     * 
     * + default value Is ``0.1``.
   */
   function add_gauss(x: object, scale?: number): object;
   /**
    * merge row or column where the tag is identical
    * 
    * 
     * @param x -
     * @param byrow -
     * 
     * + default value Is ``true``.
   */
   function aggregate(x: object, byrow?: boolean): any;
   module as {
      /**
       * create gene expression DEG model
       * 
       * 
        * @param x usually be a dataframe object of the different expression analysis result
        * @param logFC -
        * 
        * + default value Is ``'logFC'``.
        * @param pvalue -
        * 
        * + default value Is ``'pvalue'``.
        * @param label -
        * 
        * + default value Is ``'id'``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function deg(x: any, logFC?: string, pvalue?: string, label?: string, env?: object): object;
      /**
       * convert the matrix into row gene list
       * 
       * 
        * @param expr0 -
        * @return a tuple list of the expression numeric vector, each slot data 
        *  is the vector of expression value of a gene, slot key name is the 
        *  corresponding gene id.
      */
      function expr_list(expr0: object): object;
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
     * @param pattern -
     * @param memberCutoff 
     * + default value Is ``0.8``.
     * @param empty_shared 
     * + default value Is ``2``.
     * @param max_cluster_shared 
     * + default value Is ``3``.
     * @param env 
     * + default value Is ``null``.
   */
   function cmeans_matrix(pattern: any, memberCutoff?: number, empty_shared?: object, max_cluster_shared?: object, env?: object): object;
   module deg {
      /**
       * set deg class label
       * 
       * 
        * @param deg -
        * @param class_labels set deg class label manually
        * 
        * + default value Is ``null``.
        * @param logFC 
        * + default value Is ``1``.
        * @param pval_cutoff 
        * + default value Is ``0.05``.
      */
      function class(deg: object, class_labels?: any, logFC?: number, pval_cutoff?: number): object;
      module t {
         /**
          * do t-test across specific analysis comparision
          * 
          * 
           * @param x -
           * @param sampleinfo -
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
           * @param env -
           * 
           * + default value Is ``null``.
         */
         function test(x: object, sampleinfo: object, treatment: string, control: string, level?: number, pvalue?: number, FDR?: number, env?: object): object;
      }
   }
   /**
    * get summary information about the HTS matrix dimensions
    * 
    * 
     * @param mat a HTS data matrix of samples in column and gene features in row
     * @return a tuple list that contains the dimension information of the 
     *  gene expression matrix data:
     *  
     *  + feature_size: the number of the matrix rows, or count of genes in matrix
     *  + feature_names: a character vector of the gene ids for each rows
     *  + sample_size: the number of the samples, or number of the matrix columns
     *  + sample_names: the matrix column names, the sample id set
   */
   function dims(mat: object): object;
   /**
   */
   function exp(x: object, p: number): object;
   /**
   */
   function expr_ranking(x: object, sampleinfo: object): object;
   module expression {
      /**
       * This function performs clustering analysis of time course data. 
       *  Calculate gene expression pattern by cmeans algorithm.
       * 
       * 
        * @param matrix the gene expression matrix object which could be generated by 
        *  @``M:phenotype_kit.geneExpression.loadExpression(System.Object,System.String[],System.Boolean,System.Boolean,SMRUCC.Rsharp.Runtime.Environment)`` api.
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
        * @param env 
        * + default value Is ``null``.
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
      */
      function cmeans3D(matrix: object, fuzzification?: number, threshold?: number): object;
   }
   /**
    * get gene expression vector data
    * 
    * 
     * @param x -
     * @param geneId -
     * @return a numeric vector of the target gene expression across multiple samples
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
     * @param instr 
     * + default value Is ``null``.
     * @param exclude matrix a subset of the data matrix excepts the 
     *  input **`geneId`** features or just make a subset which 
     *  just contains the input **`geneId`** features.
     * 
     * + default value Is ``false``.
     * @param env 
     * + default value Is ``null``.
     * @return A new expression matrix object that consist with gene feature
     *  rows subset from the original matrix input.
   */
   function filter(HTS: object, geneId?: string, instr?: string, exclude?: boolean, env?: object): object;
   /**
    * set the NaN missing value to default value
    * 
    * 
     * @param x -
     * @param missingDefault set NA missing value to zero by default
     * 
     * + default value Is ``0``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function filterNaNMissing(x: object, missingDefault?: number, env?: object): object;
   /**
    * removes the rows which all gene expression result is ZERO
    * 
    * 
     * @param mat -
     * @param env -
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
     * @param mat -
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function filterZeroSamples(mat: object, env?: object): object;
   /**
    * get gene Id list or byref set of the gene id alias set.
    * 
    * 
     * @param x A collection of the deg/dep object or a raw HTS matrix object
     * @param set_id 
     * + default value Is ``null``.
     * @param env 
     * + default value Is ``null``.
     * @return A collection of the gene id set
   */
   function geneId(x: any, set_id?: any, env?: object): string;
   /**
    * set the zero value to the half of the min positive value
    * 
    * 
     * @param x an expression matrix object that may contains zero
     * @param by_features 
     * + default value Is ``false``.
     * @return An expression data matrix with missing data filled
   */
   function impute_missing(x: object, by_features?: boolean): object;
   /**
    * merge multiple gene expression matrix by gene features
    * 
    * 
     * @param x -
     * @param strict -
     * 
     * + default value Is ``true``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function joinFeatures(x: any, strict?: boolean, env?: object): object;
   /**
    * do matrix join by samples
    * 
    * 
     * @param samples matrix in multiple batches data should be normalized at
     *  first before calling this data batch merge function.
     * @param strict 
     * + default value Is ``true``.
   */
   function joinSample(samples: object, strict?: boolean): object;
   /**
    * The limma algorithm (Linear Models for Microarray Data) is a widely used statistical framework in R/Bioconductor 
    *  for differential expression (DE) analysis of RNA-seq data. Originally designed for microarray studies, its 
    *  flexibility and robustness have extended its utility to RNA-seq through the voomtransformation.
    * 
    * 
     * @param x -
     * @param design -
   */
   function limma(x: object, design: object): object;
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
        * @param rm_ZERO 
        * + default value Is ``false``.
        * @param makeNames 
        * + default value Is ``false``.
        * @param env 
        * + default value Is ``null``.
        * @return a HTS data matrix of samples in column and gene features in row
      */
      function expr(file: any, exclude_samples?: string, rm_ZERO?: boolean, makeNames?: boolean, env?: object): object;
      /**
       * read the binary matrix data file
       * 
       * 
        * @param file -
        * @param lazy 
        * + default value Is ``false``.
        * @param env -
        * 
        * + default value Is ``null``.
        * @return a HTS data matrix of samples in column and gene features in row
      */
      function expr0(file: any, lazy?: boolean, env?: object): object|object;
      /**
       * Load the HTS matrix into a lazy matrix viewer
       * 
       * 
        * @param mat -
      */
      function matrixView(mat: object): object;
   }
   /**
    * log scale of the HTS raw matrix
    * 
    * 
     * @param expr should be a HTS expression matrix object
     * @param base -
     * 
     * + default value Is ``2.718281828459045``.
   */
   function log(expr: any, base?: number): object;
   /**
    * evaluate the MAD value for each gene features
    * 
    * 
     * @param x -
   */
   function mad(x: object): object;
   /**
    * get matrix summary information
    * 
    * 
     * @param file could be a file path or the HTS matrix data object
     * @return A tuple list object that contains the data information
     *  which is extract from the given file:
     *  
     *  1. sampleID: a character vector that contains the matrix sample information(column features name)
     *  2. geneID: a character vector that contains the matrix gene features information(row features name)
     *  3. tag: the matrix source tag label, could be the file basename if the given input file is a file path to the matrix.
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
     * @param x -
   */
   function minmax01Norm(x: object): any;
   /**
    * get the top n representatives genes in each expression pattern
    * 
    * 
     * @param pattern -
     * @param top top n cmeans membership items
     * 
     * + default value Is ``3``.
   */
   function pattern_representatives(pattern: object, top?: object): any;
   /**
    * do PCA on a gene expressin matrix
    * 
    * 
     * @param x a gene expression matrix
     * @param npc -
     * 
     * + default value Is ``3``.
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
     * @param empty_shared 
     * + default value Is ``2``.
     * @param max_cluster_shared 
     * + default value Is ``3``.
     * @param xlab 
     * + default value Is ``'Spatial Regions'``.
     * @param ylab 
     * + default value Is ``'z-score(Normalized Intensity)'``.
     * @param top_members 
     * + default value Is ``0.2``.
     * @param cluster_label_css 
     * + default value Is ``'font-style: normal; font-size: 20; font-family: Bookman Old Style;'``.
     * @param legend_title_css 
     * + default value Is ``'font-style: normal; font-size: 10; font-family: Microsoft YaHei;'``.
     * @param legend_tick_css 
     * + default value Is ``'font-style: normal; font-size: 10; font-family: Microsoft YaHei;'``.
     * @param axis_tick_css 
     * + default value Is ``'font-style: normal; font-size: 12; font-family: Segoe UI;'``.
     * @param axis_label_css 
     * + default value Is ``'font-style: normal; font-size: 10; font-family: Microsoft YaHei;'``.
     * @param grid_fill 
     * + default value Is ``'LightGray'``.
     * @param grid_draw 
     * + default value Is ``true``.
     * @param x_lab_rotate 
     * + default value Is ``45``.
     * @param env -
     * 
     * + default value Is ``null``.
     * @return this function returns a tuple list that contains the pattern 
     *  cluster matrix and the cmeans pattern plots.
     *  
     *  1. 'pattern' is a dataframe object that contains the object cluster patterns
     *  2. 'image' is a bitmap image that plot based on the object cluster patterns data.
     *  3. 'pdf' is a pdf image that could be edit
   */
   function peakCMeans(matrix: object, nsize?: any, threshold?: number, fuzzification?: number, plotSize?: any, colorSet?: string, memberCutoff?: number, empty_shared?: object, max_cluster_shared?: object, xlab?: string, ylab?: string, top_members?: number, cluster_label_css?: string, legend_title_css?: string, legend_tick_css?: string, axis_tick_css?: string, axis_label_css?: string, grid_fill?: string, grid_draw?: boolean, x_lab_rotate?: number, env?: object): any;
   /**
    * make matrix samples column projection
    * 
    * 
     * @param x -
     * @param sampleIds -
   */
   function project(x: object, sampleIds: any): object;
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
   */
   function relative(matrix: object, median?: boolean): object;
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
     * @param env -
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
     * @param pattern -
     * @param file -
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
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function setFeatures(x: any, gene_ids: string, env?: object): object|object;
   /**
    * set a new tag string to the matrix
    * 
    * 
     * @param expr0 -
     * @param tag -
   */
   function setTag(expr0: object, tag: string): object;
   /**
    * set the expression value to zero 
    *  
    *  if the expression value is less than a given threshold
    * 
    * 
     * @param expr0 -
     * @param q -
     * 
     * + default value Is ``0.1``.
   */
   function setZero(expr0: object, q?: number): object;
   /**
    * take top n expression feature by rank expression MAD value desc
    * 
    * 
     * @param x -
     * @param top -
     * 
     * + default value Is ``10000``.
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
   */
   function take_shuffle(x: object, n: object): object;
   /**
    * normalize data by sample column
    * 
    * > apply for the metabolomics data usually
    * 
     * @param matrix a gene expression matrix
     * @param scale 
     * + default value Is ``10000``.
   */
   function totalSumNorm(matrix: object, scale?: number): object;
   /**
    * do matrix transpose
    * 
    * 
     * @param mat -
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
