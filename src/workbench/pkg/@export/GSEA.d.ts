// export R# package module type define for javascript/typescript language
//
// ref=gseakit.GSEA

/**
 * ### The GCModeller GSEA toolkit
 *  
 *  ##### Gene set enrichment analysis
 *  
 *  Gene Set enrichment analysis (GSEA) (also called functional enrichment 
 *  analysis Or pathway enrichment analysis) Is a method To identify classes
 *  Of genes Or proteins that are over-represented In a large Set Of genes 
 *  Or proteins, And may have an association With disease phenotypes. The
 *  method uses statistical approaches To identify significantly enriched 
 *  Or depleted groups Of genes. Transcriptomics technologies And proteomics 
 *  results often identify thousands Of genes which are used For the analysis.
 * 
*/
declare namespace GSEA {
   module read {
      /**
      */
      function enrichment(file:string): any;
   }
   module enrichment {
      /**
        * @param showProgress default value is ``true``.
        * @param env default value is ``null``.
      */
      function go(background:object, geneSet:string, go:any, showProgress?:boolean, env?:object): any;
      /**
      */
      function go_dag(go_enrichment:object, go:object): any;
      module draw {
         /**
         */
         function go_dag(go_enrichment:object, go:object): any;
      }
   }
   /**
     * @param term default value is ``'unknown'``.
     * @param env default value is ``null``.
   */
   function fisher(list:string, geneSet:string, background:any, term?:string, env?:object): any;
   module write {
      /**
        * @param format default value is ``null``.
        * @param env default value is ``null``.
      */
      function enrichment(enrichment:any, file:string, format?:object, env?:object): any;
   }
   module as {
      /**
        * @param database default value is ``'n/a'``.
      */
      function KOBAS_terms(enrichment:object, database?:string): any;
   }
   /**
     * @param desc default value is ``null``.
     * @param score default value is ``null``.
     * @param fdr default value is ``null``.
     * @param cluster default value is ``null``.
     * @param enriched default value is ``null``.
     * @param env default value is ``null``.
   */
   function cast_enrichs(term:string, name:string, pvalue:number, geneIDs:object, desc?:string, score?:number, fdr?:number, cluster?:object, enriched?:string, env?:object): any;
   /**
     * @param env default value is ``null``.
   */
   function to_enrichment_terms(x:object, env?:object): any;
}
