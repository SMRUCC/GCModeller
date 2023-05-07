// export R# package module type define for javascript/typescript language
//
// ref=kegg_kit.kegg_api

/**
 * KEGG API is a REST-style Application Programming Interface to the KEGG database resource.
 * 
*/
declare namespace kegg_api {
   /**
     * @param option default value is ``null``.
     * @param cache default value is ``null``.
   */
   function listing(database:string, option?:string, cache?:object): any;
   /**
     * @param cache default value is ``null``.
     * @param env default value is ``null``.
   */
   function get(id:string, cache?:any, env?:object): any;
   /**
     * @param unsafe default value is ``false``.
   */
   function parseForm(text:string, unsafe?:boolean): any;
   module as {
      /**
      */
      function pathway(form:object): any;
      /**
      */
      function compound(form:object): any;
      /**
      */
      function reaction(form:object): any;
   }
}
