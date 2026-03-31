// export R# package module type define for javascript/typescript language
//
//    imports "uniprot" from "seqtoolkit";
//
// ref=seqtoolkit.uniprotTools@seqtoolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * The Universal Protein Resource (UniProt)
 * 
*/
declare namespace uniprot {
   /**
   */
   function get_description(prot: object): string;
   /**
   */
   function get_domain(prot: object): object;
   /**
    * get keyword dataframe about the given protein data
    * 
    * 
     * @param prot -
     * @return a dataframe object that with two data fields: `id` - the keyword id and `keyword` - the keyword name.
   */
   function get_keywords(prot: object): object;
   /**
    * get related pathway names of current protein
    * 
    * 
     * @param prot -
   */
   function get_pathways(prot: object): string;
   /**
   */
   function get_reactions(prot: object): any;
   /**
   */
   function get_sequence(prot: object): object;
   /**
    * get subcellular location of current protein
    * 
    * 
     * @param prot -
   */
   function get_subcellularlocation(prot: object): any;
   /**
    * get external database reference id set
    * 
    * > the uniprot database name will be named as: ``UniProtKB/Swiss-Prot`` for
    * >  make unify with the genebank feature xrefs.
    * 
     * @param prot target protein object to extract its database corss reference id
     * @param dbname this function will returns a character vector of the db_xrefs for specific database if this db name is specificed.
     * 
     * + default value Is ``null``.
   */
   function get_xrefs(prot: object, dbname?: string): object|string;
   /**
    * id unify mapping
    * 
    * 
     * @param uniprot a uniprot dataabse pipeline stream
     * @param id -
     * @param target the database name for map to
     * 
     * + default value Is ``null``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function id_unify(uniprot: any, id: any, target?: string, env?: object): any;
   /**
     * @param env default value Is ``null``.
   */
   function metaboliteSet(uniprot: any, env?: object): any;
   module open {
      /**
       * open a uniprot database file
       * 
       * 
        * @param files -
        * @param isUniParc -
        * 
        * + default value Is ``false``.
        * @param ignoreError 
        * + default value Is ``true``.
        * @param tqdm 
        * + default value Is ``true``.
        * @param env 
        * + default value Is ``null``.
        * @return this function returns a pipeline stream of the uniprot protein entries.
      */
      function uniprot(files: any, isUniParc?: boolean, ignoreError?: boolean, tqdm?: boolean, env?: object): object;
   }
   /**
    * Parse the uniprot fasta header text
    * 
    * 
     * @param x -
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function parseHeader(x: any, env?: object): object;
   /**
   */
   function parseUniProt(xml: string): object;
   module protein {
      /**
       * populate all protein fasta sequence from the given uniprot database reader
       * 
       * 
        * @param uniprot a collection of the uniprot protein @``T:SMRUCC.genomics.Assembly.Uniprot.XML.entry`` data.
        * @param extractAll populate the sequence with all uniprot accession id
        * 
        * + default value Is ``false``.
        * @param title the fasta title header template, data keys for template could be:
        *  
        *  1. uniprot_id
        *  2. fullname
        *  3. name
        *  4. ncbi_taxid
        *  5. organism
        *  6. ec_number
        *  7. go_id
        *  8. gene_name
        *  9. ORF
        *  10. subcellular_location
        *  11. db_xrefs...
        * 
        * + default value Is ``'<uniprot_id> <fullname>'``.
        * @param env -
        * 
        * + default value Is ``null``.
        * @return a collection of the @``T:SMRUCC.genomics.SequenceModel.FASTA.FastaSeq`` that export from the given protein set.
        *  
        *  the generated fasta sequence header title in format: ``uniprot_id|db_xref|protein function``.
        *  the db_xref is optional if the parameter "db_xref" is not be omited.
      */
      function seqs(uniprot: any, extractAll?: boolean, title?: string, env?: object): object;
   }
   /**
    * export protein annotation data as data frame.
    * 
    * 
     * @param uniprot -
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function proteinTable(uniprot: any, env?: object): any;
   module read {
      /**
       * read uniprot protein export output tsv file
       * 
       * 
        * @param file -
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function proteinTable(file: any, env?: object): object;
   }
}
