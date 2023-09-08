// export R# package module type define for javascript/typescript language
//
//    imports "UniProt" from "annotationKit";
//    imports "UniProt" from "gseakit";
//
// ref=annotationKit.UniProt@annotationKit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// ref=gseakit.UniProt@gseakit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * 
 * The uniprot background model handler
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
   /**
    * create uniprot keyword ontology result
    * 
    * 
     * @param enrichment -
     * @param keywords -
     * @param top -
     * 
     * + default value Is ``4``.
   */
   function keyword_profiles(enrichment: object, keywords: object, top?: object): object;
   /**
     * @param env default value Is ``null``.
   */
   function subcellular_location(uniprot: any, env?: object): object;
   /**
    * Create a gsea background model for uniprot keywords based
    *  on the given uniprot database assembly data
    * 
    * 
     * @param uniprot -
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function uniprot_keywords(uniprot: any, env?: object): object;
}
