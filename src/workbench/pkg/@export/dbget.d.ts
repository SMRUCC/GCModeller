// export R# package module type define for javascript/typescript language
//
// ref=kegg_kit.dbget

/**
 * toolkit for download kegg dataset
 * 
*/
declare namespace dbget {
   /**
     * @param env default value is ``null``.
   */
   function getMap(id:string, env?:object): any;
   /**
     * @param env default value is ``null``.
   */
   function fetch_kegg_maps(cache:any, env?:object): any;
   /**
     * @param env default value is ``null``.
   */
   function show_organism(code:string, env?:object): any;
}
