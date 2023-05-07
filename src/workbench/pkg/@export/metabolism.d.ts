// export R# package module type define for javascript/typescript language
//
// ref=kegg_kit.metabolism@kegg_kit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * The kegg metabolism model toolkit
 * 
*/
declare namespace metabolism {
   module load {
      module reaction {
         /**
         */
         function cacheIndex(file:string): object;
      }
   }
   module related {
      /**
       * Get compounds kegg id which is related to the given KO id list
       * 
       * 
        * @param enzymes KO id list
        * @param reactions -
      */
      function compounds(enzymes:string, reactions:object): string;
   }
   module filter {
      /**
       * Removes invalid kegg compound id
       * 
       * 
        * @param identified -
      */
      function invalid_keggIds(identified:string): string;
   }
   module kegg {
      /**
       * do kegg pathway reconstruction by given protein annotation data
       * 
       * 
        * @param reference the kegg reference maps
        * @param reactions a list of the kegg reaction data models
        * @param annotations -
        * @param min_cov -
        * 
        * + default value Is ``0.3``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function reconstruction(reference:any, reactions:any, annotations:any, min_cov?:number, env?:object): object;
   }
   /**
    * pick the reaction list from the kegg reaction
    *  network repository by KO id terms
    * 
    * 
     * @param reactions -
     * @param terms the KO id terms
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function pickNetwork(reactions:object, terms:any, env?:object): any;
}
