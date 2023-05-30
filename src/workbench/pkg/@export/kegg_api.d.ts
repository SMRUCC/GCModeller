// export R# package module type define for javascript/typescript language
//
// ref=kegg_kit.kegg_api@kegg_kit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * KEGG API is a REST-style Application Programming Interface to the KEGG database resource.
 * 
*/
declare namespace kegg_api {
   module as {
      /**
      */
      function compound(form: object): object;
      /**
      */
      function pathway(form: object): object;
      /**
      */
      function reaction(form: object): object;
   }
   /**
     * @param cache default value Is ``null``.
     * @param env default value Is ``null``.
   */
   function get(id: string, cache?: any, env?: object): string;
   /**
     * @param option default value Is ``null``.
     * @param cache default value Is ``null``.
   */
   function listing(database: string, option?: string, cache?: object): any;
   /**
     * @param unsafe default value Is ``false``.
   */
   function parseForm(text: string, unsafe?: boolean): object;
}
