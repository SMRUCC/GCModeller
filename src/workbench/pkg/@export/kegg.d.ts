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
}
