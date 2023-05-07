// export R# package module type define for javascript/typescript language
//
// ref=kegg_kit.metabolism

/**
 * The kegg metabolism model toolkit
 * 
*/
declare namespace metabolism {
   module load {
      module reaction {
         /**
         */
         function cacheIndex(file:string): any;
      }
   }
   module related {
      /**
      */
      function compounds(enzymes:string, reactions:object): any;
   }
   module filter {
      /**
      */
      function invalid_keggIds(identified:string): any;
   }
   module kegg {
      /**
        * @param min_cov default value is ``0.3``.
        * @param env default value is ``null``.
      */
      function reconstruction(reference:any, reactions:any, annotations:any, min_cov?:number, env?:object): any;
   }
   /**
     * @param env default value is ``null``.
   */
   function pickNetwork(reactions:object, terms:any, env?:object): any;
}
