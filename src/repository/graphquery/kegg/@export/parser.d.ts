// export R# package module type define for javascript/typescript language
//
//    imports "parser" from "kegg_api";
//
// ref=kegg_api.parser@kegg_api, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace parser {
   /**
     * @param fs default value Is ``'./.cache/'``.
     * @param env default value Is ``null``.
   */
   function kegg_map(id: string, fs?: any, env?: object): object;
}
