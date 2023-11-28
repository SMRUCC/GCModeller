// export R# package module type define for javascript/typescript language
//
//    imports "dbget" from "kegg_kit";
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
   function show_organism(code: string, env?: object): object;
}
