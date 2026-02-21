// export R# package module type define for javascript/typescript language
//
//    imports "rhea" from "annotationKit";
//
// ref=annotationKit.Rhea@annotationKit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * Rhea is an expert-curated knowledgebase of chemical and transport reactions of biological interest 
 *  and the standard for enzyme and transporter annotation in UniProtKB. Rhea uses the chemical 
 *  dictionary ChEBI (Chemical Entities of Biological Interest) to describe reaction participants.
 * 
*/
declare namespace rhea {
   /**
     * @param env default value Is ``null``.
   */
   function load_brenda_enzymes(file: any, env?: object): object;
   module open {
      /**
       * open the rdf data pack of Rhea database
       * 
       * > https://ftp.expasy.org/databases/rhea/rdf/rhea.rdf.gz
       * 
        * @param file -
      */
      function rdf(file: string): object;
   }
   /**
    * Load reaction models from rhea rdf database file
    * 
    * 
     * @param rhea -
   */
   function reactions(rhea: object): object;
}
