// export R# package module type define for javascript/typescript language
//
//    imports "ptf" from "annotationKit";
//
// ref=annotationKit.PTFCache@annotationKit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * The protein annotation metadata
 * 
*/
declare namespace ptf {
   module cache {
      /**
       * create a protein annotation metadata file
       * 
       * 
        * @param uniprot -
        * @param file -
        * @param db_xref -
        * 
        * + default value Is ``["Bgee","KEGG","KO","GO","Pfam","RefSeq","EC","InterPro","BioCyc","eggNOG","keyword"]``.
        * @param cacheTaxonomy -
        * 
        * + default value Is ``false``.
        * @param hds_stream -
        * 
        * + default value Is ``false``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function ptf(uniprot: any, file: any, db_xref?: any, cacheTaxonomy?: boolean, hds_stream?: boolean, env?: object): any;
   }
   /**
    * Create the protein annotation data model from a given dataframe object
    * 
    * 
     * @param x + gene_id
     *  + locus_id
     *  + gene_name
     *  + description
     *  + sequence
     *  + ...
     * @param env 
     * + default value Is ``null``.
   */
   function fromDataframe(x: object, env?: object): object;
   /**
    * do id mapping via the protein annotation cache
    * 
    * 
     * @param proteins the protein annotation data
     * @param mapTo external xrefs database name that the id will mapping to
     * @param id a character vector that will be do mapping,
     *  this parameter will make id subset of the id mapping result,
     *  nothing means no subset
     * 
     * + default value Is ``null``.
     * @return A character vector of the mapping result id set, 
     *  unmapped id will be leaves blank in this result.
   */
   function ID_mapping(proteins: object, from: string, mapTo: string, id?: string): object;
   module list {
      /**
       * enumerate all database name from a HDS stream
       * 
       * 
        * @param ptf -
      */
      function xrefs(ptf: object): string;
   }
   /**
    * load the cross reference id set
    * 
    * 
     * @param ptf -
     * @param database the database name
   */
   function load_xref(ptf: object, database: string): any;
   /**
   */
   function loadBackgroundModel(ptf: object, database: string): object;
   module read {
      /**
       * read the protein annotation database
       * 
       * 
        * @param file -
      */
      function ptf(file: string): object;
   }
   module summary {
      /**
      */
      function xrefs(ptf: object, xrefs: string): any;
   }
}
