// export R# package module type define for javascript/typescript language
//
//    imports "kegg" from "gseakit";
//
// ref=gseakit.KEGG@gseakit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * the kegg background helper
 * 
*/
declare namespace kegg {
   /**
    * gt kegg compound set from a kegg pathway map collection
    * 
    * 
     * @param maps -
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function compound_set(maps: any, env?: object): any;
   /**
     * @param kegg_id default value Is ``'kegg_id'``.
     * @param env default value Is ``null``.
   */
   function kegg_category_annotation(kegg: object, anno: object, kegg_id?: string, env?: object): any;
}
