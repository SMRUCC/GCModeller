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
