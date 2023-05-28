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
    * Create pathway background model
    * 
    * 
     * @param biocyc -
   */
   function createBackground(biocyc: object): object;
   /**
   */
   function formula(meta: object): string;
   /**
   */
   function getCompounds(repo: object): object;
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
