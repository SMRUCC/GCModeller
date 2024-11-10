// export R# package module type define for javascript/typescript language
//
//    imports "BioCyc" from "annotationKit";
//
// ref=annotationKit.BioCycRepository@annotationKit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * The BioCyc database collection is an assortment of organism specific Pathway/Genome Databases (PGDBs) that provide reference to genome and metabolic pathway information for thousands of organisms.[1] As of July 2023, there were over 20,040 databases within BioCyc.[2] SRI International,[3] based in Menlo Park, California, maintains the BioCyc database family.
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
    * get external database cross reference id of current metabolite compound object
    * 
    * 
     * @param meta -
   */
   function db_links(meta: object): any;
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
