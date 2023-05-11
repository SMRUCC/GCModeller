// export R# package module type define for javascript/typescript language
//
// ref=kegg_kit.dbget@kegg_kit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * toolkit for download kegg dataset
 * 
*/
declare namespace dbget {
   /**
     * @param env default value Is ``null``.
   */
   function fetch_kegg_maps(cache: any, env?: object): any;
   /**
    * get kegg map from the kegg web server
    * 
    * 
     * @param id -
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function getMap(id: string, env?: object): object;
   /**
     * @param env default value Is ``null``.
   */
   function show_organism(code: string, env?: object): object;
}
