declare namespace geneExpression {
   /**
   */
   function tr(mat:object): any;
   /**
   */
   function dims(mat:object): any;
   module as {
      /**
      */
      function expr_list(expr0:object): any;
      /**
      */
      function generic(matrix:object): any;
      /**
        * @param logFC default value is ``'logFC'``.
        * @param pvalue default value is ``'pvalue'``.
        * @param label default value is ``'id'``.
        * @param env default value is ``null``.
      */
      function deg(x:any, logFC?:string, pvalue?:string, label?:string, env?:object): any;
   }
   /**
   */
   function setTag(expr0:object, tag:string): any;
   /**
     * @param q default value is ``0.1``.
   */
   function setZero(expr0:object, q?:number): any;
   /**
     * @param env default value is ``null``.
   */
   function setSamples(x:any, sample_ids:string, env?:object): any;
   /**
     * @param env default value is ``null``.
   */
   function setFeatures(x:any, gene_ids:string, env?:object): any;
   /**
     * @param env default value is ``null``.
   */
   function filterZeroSamples(mat:object, env?:object): any;
   /**
     * @param env default value is ``null``.
   */
   function filterZeroGenes(mat:object, env?:object): any;
   /**
     * @param missingDefault default value is ``0``.
     * @param env default value is ``null``.
   */
   function filterNaNMissing(x:object, missingDefault?:number, env?:object): any;
   module load {
      /**
        * @param exclude_samples default value is ``null``.
        * @param rm_ZERO default value is ``false``.
        * @param makeNames default value is ``false``.
        * @param env default value is ``null``.
      */
      function expr(file:any, exclude_samples?:string, rm_ZERO?:boolean, makeNames?:boolean, env?:object): any;
      /**
        * @param lazy default value is ``false``.
        * @param env default value is ``null``.
      */
      function expr0(file:any, lazy?:boolean, env?:object): any;
      /**
      */
      function matrixView(mat:object): any;
   }
   /**
   */
   function matrix_info(file:any): any;
   module write {
      /**
        * @param id default value is ``'geneID'``.
        * @param binary default value is ``false``.
      */
      function expr_matrix(expr:object, file:string, id?:string, binary?:boolean): any;
   }
   /**
     * @param exclude default value is ``false``.
   */
   function filter(HTS:object, geneId:string, exclude?:boolean): any;
   /**
   */
   function average(matrix:object, sampleinfo:object): any;
   /**
   */
   function z_score(x:object): any;
   /**
     * @param npc default value is ``3``.
   */
   function pca(x:object, npc?:object): any;
   /**
     * @param scale default value is ``10000``.
   */
   function totalSumNorm(matrix:object, scale?:number): any;
   /**
   */
   function relative(matrix:object): any;
   module expression {
      /**
        * @param dim default value is ``'3,3'``.
        * @param fuzzification default value is ``2``.
        * @param threshold default value is ``0.001``.
        * @param env default value is ``null``.
      */
      function cmeans_pattern(matrix:object, dim?:any, fuzzification?:number, threshold?:number, env?:object): any;
      /**
        * @param fuzzification default value is ``2``.
        * @param threshold default value is ``0.001``.
      */
      function cmeans3D(matrix:object, fuzzification?:number, threshold?:number): any;
   }
   /**
   */
   function savePattern(pattern:object, file:string): any;
   /**
   */
   function readPattern(file:string): any;
   /**
     * @param memberCutoff default value is ``0.8``.
     * @param empty_shared default value is ``2``.
     * @param max_cluster_shared default value is ``3``.
     * @param env default value is ``null``.
   */
   function cmeans_matrix(pattern:any, memberCutoff?:number, empty_shared?:object, max_cluster_shared?:object, env?:object): any;
   module split {
      /**
      */
      function cmeans_clusters(cmeans:object): any;
   }
   /**
     * @param nsize default value is ``'3,3'``.
     * @param threshold default value is ``10``.
     * @param fuzzification default value is ``2``.
     * @param plotSize default value is ``'8100,5200'``.
     * @param colorSet default value is ``'Jet'``.
     * @param memberCutoff default value is ``0.8``.
     * @param empty_shared default value is ``2``.
     * @param max_cluster_shared default value is ``3``.
     * @param xlab default value is ``'Spatial Regions'``.
     * @param ylab default value is ``'z-score(Normalized Intensity)'``.
     * @param top_members default value is ``0.2``.
     * @param env default value is ``null``.
   */
   function peakCMeans(matrix:object, nsize?:string, threshold?:number, fuzzification?:number, plotSize?:any, colorSet?:string, memberCutoff?:number, empty_shared?:object, max_cluster_shared?:object, xlab?:string, ylab?:string, top_members?:number, env?:object): any;
   /**
   */
   function expr_ranking(x:object, sampleinfo:object): any;
   module deg {
      module t {
         /**
           * @param level default value is ``1.5``.
           * @param pvalue default value is ``0.05``.
           * @param FDR default value is ``0.05``.
           * @param env default value is ``null``.
         */
         function test(x:object, sampleinfo:object, treatment:string, control:string, level?:number, pvalue?:number, FDR?:number, env?:object): any;
      }
      /**
      */
      function class(deg:object, classLabel:any): any;
   }
   /**
     * @param base default value is ``2.718281828459045``.
   */
   function log(expr:object, base?:number): any;
   /**
     * @param env default value is ``null``.
   */
   function geneId(dep:any, env?:object): any;
   /**
   */
   function joinSample(samples:object): any;
   /**
     * @param byrow default value is ``true``.
   */
   function aggregate(x:object, byrow?:boolean): any;
}
