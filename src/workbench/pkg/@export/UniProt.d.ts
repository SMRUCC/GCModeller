// export R# package module type define for javascript/typescript language
//
// ref=seqtoolkit.uniprot@seqtoolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * The Universal Protein Resource (UniProt)
 * 
*/
declare namespace uniprot {
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
        * @param env 
        * + default value Is ``null``.
        * @return this function returns a pipeline stream of the uniprot protein entries.
      */
      function uniprot(files: any, isUniParc?: boolean, ignoreError?: boolean, env?: object): any;
   }
   /**
   */
   function parseUniProt(xml: string): object;
   module protein {
      /**
       * populate all protein fasta sequence from the given uniprot database reader
       * 
       * 
        * @param uniprot -
        * @param extractAll -
        * 
        * + default value Is ``false``.
        * @param KOseq 
        * + default value Is ``false``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function seqs(uniprot: any, extractAll?: boolean, KOseq?: boolean, env?: object): object;
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
}
