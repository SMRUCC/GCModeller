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
     * @param fs_dir default value Is ``/``.
     * @param cache_dir default value Is ``./``.
   */
   function __query_compounds(compounds: any, fs_dir?: any, cache_dir?: any): object;
   /**
     * @param cache_dir default value Is ``./``.
   */
   function __query_map(map: any, cache_dir?: any): object;
   /**
     * @param fs_dir default value Is ``/``.
     * @param cache_dir default value Is ``./``.
   */
   function __query_module(mid: any, fs_dir?: any, cache_dir?: any): object;
   /**
   */
   function compound_brites(): object;
   /**
     * @param prefix default value Is ````.
     * @param maxChars default value Is ``64``.
   */
   function enumeratePath(brite: any, prefix?: any, maxChars?: any): object;
   /**
     * @param cache_dir default value Is ``null``.
   */
   function kegg_compound(id: any, cache_dir?: any): object;
   /**
     * @param cache_dir default value Is ``null``.
   */
   function kegg_module(id: any, cache_dir?: any): object;
   /**
     * @param cache default value Is ``null``.
   */
   function kegg_pathway(id: any, cache?: any): object;
   /**
     * @param cache_dir default value Is ``null``.
   */
   function kegg_reaction(id: any, cache_dir?: any): object;
   /**
     * @param db_file default value Is ``./kegg.db``.
   */
   function kegg_referencedb(db_file?: any): object;
   /**
     * @param db default value Is ``./``.
   */
   function ko_db(db?: any): object;
   /**
     * @param org default value Is ``["ko", "map", "hsa"]``.
     * @param cache default value Is ``null``.
   */
   function list_pathway(org?: any, cache?: any): object;
   /**
   */
   function loadReactionIDs(): object;
   /**
   */
   function pathway_category(): object;
   /**
   */
   function placeNULL(v: any): object;
   /**
   */
   function reaction_category(): object;
   /**
   */
   function reactionclass_category(): object;
   /**
     * @param maxChars default value Is ``32``.
   */
   function trimLongName(longNames: string, maxChars?: any): object;
}
