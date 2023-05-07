// export R# package module type define for javascript/typescript language
//
// ref=metagenomics_kit.microbiomeKit

/**
 * tools for metagenomics and microbiome
 * 
*/
declare namespace microbiome {
   module parse {
      /**
      */
      function otu_taxonomy(file:object): any;
   }
   module save {
      /**
      */
      function PICRUSt_matrix(ggtax:object, ko_13_5_precalculated:object, save:object): any;
   }
   module read {
      /**
      */
      function PICRUSt_matrix(file:object): any;
   }
   /**
     * @param env default value is ``null``.
   */
   function predict_metagenomes(PICRUSt:object, table:object, env?:object): any;
   module diff {
      /**
        * @param rank default value is ``null``.
        * @param env default value is ``null``.
      */
      function entropy(v1:object, v2:object, rank?:object, env?:object): any;
   }
   module compounds {
      module origin {
         /**
         */
         function profile(taxonomy:object, organism:string): any;
      }
   }
}
