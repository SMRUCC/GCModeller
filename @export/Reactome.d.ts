// export R# package module type define for javascript/typescript language
//
//    imports "Reactome" from "annotationKit";
//
// ref=annotationKit.ReactomeTools@annotationKit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * 
*/
declare namespace Reactome {
   /**
   */
   function jsonTree(tree: object): string;
   /**
    * get reactome pathway list
    * 
    * 
     * @param taxname -
     * 
     * + default value Is ``null``.
   */
   function pathway_list(taxname?: string): object;
}
