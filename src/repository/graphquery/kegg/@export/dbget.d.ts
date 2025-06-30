// export R# package module type define for javascript/typescript language
//
//    imports "dbget" from "kegg_api";
//
// ref=kegg_api.dbget@kegg_api, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace dbget {
   /**
     * @param env default value Is ``null``.
   */
   function show_organism(code: string, env?: object): object;
}
