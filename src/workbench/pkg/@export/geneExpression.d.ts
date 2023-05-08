// export R# package module type define for javascript/typescript language
//
// ref=phenotype_kit.geneExpression@phenotype_kit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * the gene expression matrix data toolkit
 * 
*/
declare namespace geneExpression {
   /**
    * do matrix transpose
    * 
    * 
     * @param mat -
   */
   function tr(mat:object): object;
   /**
    * get summary information about the HTS matrix dimensions
    * 
    * 
     * @param mat -
   */
   function dims(mat:object): object;
   module as {
      /**
       * convert the matrix into row gene list
       * 
       * 
        * @param expr0 -
      */
      function expr_list(expr0:object): object;
      /**
       * cast the HTS matrix object to the general dataset
       * 
       * 
        * @param matrix a gene expression matrix
      */
      function generic(matrix:object): object;
      /**
       * create gene expression DEG model
       * 
       * 
        * @param x -
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
      function deg(x:any, logFC?:string, pvalue?:string, label?:string, env?:object): object;
   }
   /**
    * set a new tag string to the matrix
    * 
    * 
     * @param expr0 -
     * @param tag -
   */
   function setTag(expr0:object, tag:string): object;
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
   function setZero(expr0:object, q?:number): object;
   /**
    * set new sample id list to the matrix columns
    * 
    * > it is kind of ``colnames`` liked function for dataframe object.
    * 
     * @param x target gene expression matrix object
     * @param sample_ids a character vector of the new sample id list for
     *  set to the sample columns of the gene expression 
     *  matrix.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function setSamples(x:any, sample_ids:string, env?:object): object|object;
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
   function setFeatures(x:any, gene_ids:string, env?:object): object|object;
   /**
    * filter out all samples columns which its expression vector is ZERO!
    * 
    * 
     * @param mat -
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function filterZeroSamples(mat:object, env?:object): any;
   /**
    * removes the rows which all gene expression result is ZERO
    * 
    * 
     * @param mat -
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function filterZeroGenes(mat:object, env?:object): any;
   /**
    * set the NaN missing value to default value
    * 
    * 
     * @param x -
     * @param missingDefault -
     * 
     * + default value Is ``0``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function filterNaNMissing(x:object, missingDefault?:number, env?:object): any;
   module load {
      /**
       * load an expressin matrix data
       * 
       * > the table file format that handled by this function
       * >  could be a csv table file or tsv table file.
       * 
        * @param file the file path or the file stream data of the target 
        *  expression matrix table file.
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
      */
      function expr(file:any, exclude_samples?:string, rm_ZERO?:boolean, makeNames?:boolean, env?:object): object;
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
      */
      function expr0(file:any, lazy?:boolean, env?:object): object|object;
      /**
      */
      function matrixView(mat:object): object;
   }
   /**
    * get matrix summary information
    * 
    * 
     * @param file could be a file path or the HTS matrix data object
   */
   function matrix_info(file:any): object;
   module write {
      /**
       * write the gene expression data matrix file
       * 
       * 
        * @param expr -
        * @param file -
        * @param id -
        * 
        * + default value Is ``'geneID'``.
        * @param binary write matrix data in binary data format? default value 
        *  is False means write matrix as csv matrix file.
        * 
        * + default value Is ``false``.
      */
      function expr_matrix(expr:object, file:string, id?:string, binary?:boolean): boolean;
   }
   /**
    * Filter the geneID rows
    * 
    * 
     * @param HTS -
     * @param geneId -
     * @param exclude matrix a subset of the data matrix excepts the 
     *  input **`geneId`** features or just make a subset which 
     *  just contains the input **`geneId`** features.
     * 
     * + default value Is ``false``.
   */
   function filter(HTS:object, geneId:string, exclude?:boolean): object;
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
     * @param sampleinfo -
   */
   function average(matrix:object, sampleinfo:object): object;
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
   function z_score(x:object): object;
   /**
    * do PCA on a gene expressin matrix
    * 
    * 
     * @param x a gene expression matrix
     * @param npc -
     * 
     * + default value Is ``3``.
   */
   function pca(x:object, npc?:object): object;
   /**
    * normalize data by sample column
    * 
    * > apply for the metabolomics data usually
    * 
     * @param matrix a gene expression matrix
     * @param scale 
     * + default value Is ``10000``.
   */
   function totalSumNorm(matrix:object, scale?:number): object;
   /**
    * normalize data by feature rows
    * 
    * > row/max(row)
    * 
     * @param matrix a gene expression matrix
   */
   function relative(matrix:object): object;
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
      function cmeans_pattern(matrix:object, dim?:any, fuzzification?:number, threshold?:number, env?:object): object;
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
      function cmeans3D(matrix:object, fuzzification?:number, threshold?:number): object;
   }
   /**
    * save the cmeans expression pattern result to local file
    * 
    * 
     * @param pattern -
     * @param file -
   */
   function savePattern(pattern:object, file:string): boolean;
   /**
    * read the cmeans expression pattern result from file
    * 
    * 
     * @param file -
   */
   function readPattern(file:string): object;
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
   function cmeans_matrix(pattern:any, memberCutoff?:number, empty_shared?:object, max_cluster_shared?:object, env?:object): object;
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
      function cmeans_clusters(cmeans:object): any;
   }
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
     * @param env -
     * 
     * + default value Is ``null``.
     * @return this function returns a tuple list that contains the pattern 
     *  cluster matrix and the cmeans pattern plots.
     *  
     *  1. 'pattern' is a dataframe object that contains the object cluster patterns
     *  2. 'image' is a bitmap image that plot based on the object cluster patterns data.
   */
   function peakCMeans(matrix:object, nsize?:string, threshold?:number, fuzzification?:number, plotSize?:any, colorSet?:string, memberCutoff?:number, empty_shared?:object, max_cluster_shared?:object, xlab?:string, ylab?:string, top_members?:number, env?:object): any;
   /**
   */
   function expr_ranking(x:object, sampleinfo:object): object;
   module deg {
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
         function test(x:object, sampleinfo:object, treatment:string, control:string, level?:number, pvalue?:number, FDR?:number, env?:object): object;
      }
      /**
      */
      function class(deg:object, classLabel:any): object;
   }
   /**
    * log scale of the HTS raw matrix
    * 
    * 
     * @param expr the HTS expression matrix object
     * @param base -
     * 
     * + default value Is ``2.718281828459045``.
   */
   function log(expr:object, base?:number): object;
   /**
    * get gene Id list
    * 
    * 
     * @param dep A collection of the deg/dep object or a raw HTS matrix object
     * @param env 
     * + default value Is ``null``.
     * @return A collection of the gene id set
   */
   function geneId(dep:any, env?:object): string;
   /**
    * do matrix join by samples
    * 
    * 
     * @param samples matrix in multiple batches data should be normalized at
     *  first before calling this data batch merge function.
   */
   function joinSample(samples:object): object;
   /**
    * merge row or column where the tag is identical
    * 
    * 
     * @param x -
     * @param byrow -
     * 
     * + default value Is ``true``.
   */
   function aggregate(x:object, byrow?:boolean): any;
}
