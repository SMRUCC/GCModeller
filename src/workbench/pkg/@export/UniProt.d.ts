// export R# package module type define for javascript/typescript language
//
//    imports "uniprot" from "seqtoolkit";
//
// ref=seqtoolkit.uniprot@seqtoolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * The Universal Protein Resource (UniProt)
 * 
*/
declare namespace uniprot {
   /**
   */
   function get_description(prot: object): string;
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
    * >  make unify with the genebank feature xrefs
    * 
     * @param prot -
   */
   function get_xrefs(prot: object): any;
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
      function uniprot(files: any, isUniParc?: boolean, ignoreError?: boolean, env?: object): object;
   }
   /**
   */
   function parseUniProt(xml: string): object;
   module protein {
      /**
       * populate all protein fasta sequence from the given uniprot database reader
       * 
       * 
        * @param uniprot a collection of the uniprot protein ``entry`` data.
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
