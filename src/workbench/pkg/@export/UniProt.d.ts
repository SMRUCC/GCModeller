// export R# package module type define for javascript/typescript language
//
//    imports "UniProt" from "annotationKit";
//
// ref=annotationKit.UniProt@annotationKit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * 
*/
declare namespace UniProt {
   /**
     * @param env default value Is ``null``.
   */
   function add_ecNumbers(pack: object, uniprot: any, env?: object): any;
   /**
     * @param create_new default value Is ``false``.
   */
   function ECnumber_pack(file: string, create_new?: boolean): object|object;
   /**
    * extract fasta data from a HDS stream database
    * 
    * 
     * @param pack -
     * @param enzyme -
     * 
     * + default value Is ``true``.
   */
   function extract_fasta(pack: object, enzyme?: boolean): object;
}
