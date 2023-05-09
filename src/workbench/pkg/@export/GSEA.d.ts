// export R# package module type define for javascript/typescript language
//
// ref=gseakit.GSEA@gseakit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

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
       * read the enrichment result table
       * 
       * 
        * @param file -
      */
      function enrichment(file: string): object;
   }
   module enrichment {
      /**
       * do GO GSEA enrichment analysis
       * 
       * 
        * @param background -
        * @param geneSet -
        * @param go the go database object can be the raw go obo data or the DAG graph model.
        * @param showProgress 
        * + default value Is ``true``.
        * @param env 
        * + default value Is ``null``.
      */
      function go(background: object, geneSet: string, go: any, showProgress?: boolean, env?: object): object;
      /**
       * Create network graph data for Cytoscape
       * 
       * 
        * @param go_enrichment -
        * @param go -
      */
      function go_dag(go_enrichment: object, go: object): object;
      module draw {
         /**
         */
         function go_dag(go_enrichment: object, go: object): object;
      }
   }
   /**
    * fisher enrichment test
    * 
    * 
     * @param list -
     * @param geneSet -
     * @param background the background size information, it could be an integer value to 
     *  indicates that the total unique size of the enrichment background 
     *  or a unique id character vector that contains all member 
     *  information of the background.
     * @param term 
     * + default value Is ``'unknown'``.
     * @param env 
     * + default value Is ``null``.
   */
   function fisher(list: string, geneSet: string, background: any, term?: string, env?: object): object;
   module write {
      /**
       * save the enrichment analysis result
       * 
       * 
        * @param enrichment -
        * @param file -
        * @param format -
        * 
        * + default value Is ``null``.
        * @param env 
        * + default value Is ``null``.
      */
      function enrichment(enrichment: any, file: string, format?: object, env?: object): boolean;
   }
   module as {
      /**
       * Convert GSEA enrichment result from GCModeller output format to KOBAS output format
       * 
       * 
        * @param enrichment -
        * @param database 
        * + default value Is ``'n/a'``.
      */
      function KOBAS_terms(enrichment: object, database?: string): object;
   }
   /**
     * @param desc default value Is ``null``.
     * @param score default value Is ``null``.
     * @param fdr default value Is ``null``.
     * @param cluster default value Is ``null``.
     * @param enriched default value Is ``null``.
     * @param env default value Is ``null``.
   */
   function cast_enrichs(term: string, name: string, pvalue: number, geneIDs: object, desc?: string, score?: number, fdr?: number, cluster?: object, enriched?: string, env?: object): object;
   /**
     * @param env default value Is ``null``.
   */
   function to_enrichment_terms(x: object, env?: object): object;
}
