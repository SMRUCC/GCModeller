// export R# source type define for javascript/typescript language
//
// package_source=kegg_api

declare namespace kegg_api {
   module _ {
      /**
      */
      function onLoad(): object;
   }
   /**
     * @param cache default value Is ``null``.
   */
   function kegg_compound(id: any, cache?: any): object;
   /**
     * @param cache default value Is ``null``.
   */
   function kegg_module(id: any, cache?: any): object;
   /**
     * @param cache default value Is ``null``.
   */
   function kegg_pathway(id: any, cache?: any): object;
   /**
     * @param cache default value Is ``null``.
   */
   function kegg_reaction(id: any, cache?: any): object;
   /**
     * @param org default value Is ``["ko", "map", "hsa"]``.
     * @param cache default value Is ``null``.
   */
   function list_pathway(org?: any, cache?: any): object;
   /**
   */
   function loadReactionIDs(): object;
}
