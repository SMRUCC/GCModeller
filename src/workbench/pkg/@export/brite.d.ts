// export R# package module type define for javascript/typescript language
//
// ref=kegg_kit.britekit@kegg_kit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * Toolkit for process the kegg brite text file
 * 
*/
declare namespace brite {
   module brite {
      module as {
         /**
          * Convert the kegg brite htext tree to plant table
          * 
          * 
           * @param htext a htex object
           * @param entryId_pattern -
           * 
           * + default value Is ``'[a-z]+\d+'``.
           * @param env 
           * + default value Is ``null``.
         */
         function table(htext:any, entryId_pattern?:string, env?:object): any;
      }
      /**
       * Do parse of the kegg brite json file.
       * 
       * 
        * @param file the htext json file path
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function parseJSON(file:string, env?:object): any;
   }
   /**
    * Do parse of the kegg brite text file.
    * 
    * 
     * @param file The file text content, brite id or its file path, example as:
     *  
     *  1. ``br08901`` could be used at here as the kegg pathway map 
     *     brite id, which is parsed from the internal resource data
     * @param env 
     * + default value Is ``null``.
   */
   function parse(file:string, env?:object): object;
   module KO {
      /**
       * Parse gene names for each KO number from the default internal htext resource.
       * 
       * 
      */
      function geneNames(): object;
   }
   /**
    * 
    * 
     * @param htext -
     * @param geneId -
     * @param level class|category|subcategory
     * 
     * + default value Is ``'class'``.
   */
   function briteMaps(htext:object, geneId:string, level?:string): string;
}
