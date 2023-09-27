// export R# package module type define for javascript/typescript language
//
//    imports "BioCyc" from "annotationKit";
//
// ref=annotationKit.BioCycRepository@annotationKit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * 
*/
declare namespace BioCyc {
   /**
    * Create pathway enrichment background model
    * 
    * 
     * @param biocyc -
   */
   function createBackground(biocyc: object): object;
   /**
   */
   function formula(meta: object): string;
   /**
    * get compounds list data from a given biocyc workspace context
    * 
    * 
     * @param repo this repository data could be a opened biocyc workspace, target
     *  file path, or text content of the compounds data
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function getCompounds(repo: any, env?: object): object;
   module open {
      /**
       * open a directory path as the biocyc workspace
       * 
       * 
        * @param repo -
      */
      function biocyc(repo: string): object;
   }
}
