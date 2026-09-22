// export R# package module type define for javascript/typescript language
//
//    imports "GSEA" from "gseakit";
//
// ref=gseakit.GSEA@gseakit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace GSEA {
   module as {
      /**
        * @param database default value Is ``'n/a'``.
      */
      function KOBAS_terms(enrichment: object, database?: string): object;
   }
   /**
     * @param geneIDs default value Is ``null``.
     * @param desc default value Is ``null``.
     * @param score default value Is ``null``.
     * @param fdr default value Is ``null``.
     * @param cluster default value Is ``null``.
     * @param enriched default value Is ``null``.
     * @param env default value Is ``null``.
   */
   function cast_enrichs(term: string, name: string, pvalue: number, geneIDs?: object, desc?: string, score?: number, fdr?: number, cluster?: object, enriched?: string, env?: object): object;
   module enrichment {
      module draw {
         /**
         */
         function go_dag(go_enrichment: object, go: object): object;
      }
      /**
        * @param showProgress default value Is ``true``.
        * @param env default value Is ``null``.
      */
      function go(background: object, geneSet: string, go: any, showProgress?: boolean, env?: object): object;
      /**
      */
      function go_dag(go_enrichment: object, go: object): object;
   }
   /**
     * @param term default value Is ``'unknown'``.
     * @param env default value Is ``null``.
   */
   function fisher(list: string, geneSet: string, background: any, term?: string, env?: object): object;
   module read {
      /**
      */
      function enrichment(file: string): object;
   }
   /**
     * @param env default value Is ``null``.
   */
   function to_enrichment_terms(x: object, env?: object): object;
   module write {
      /**
        * @param format default value Is ``null``.
        * @param env default value Is ``null``.
      */
      function enrichment(enrichment: any, file: string, format?: object, env?: object): boolean;
   }
}
