// export R# package module type define for javascript/typescript language
//
//    imports "kegg_api" from "kegg_api";
//
// ref=kegg_api.kegg_api@kegg_api, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace kegg_api {
   module as {
      /**
      */
      function compound(form: object): object;
      /**
      */
      function module(form: object): object;
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
