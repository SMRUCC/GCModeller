// export R# package module type define for javascript/typescript language
//
//    imports "uniprot" from "seqtoolkit";
//
// ref=seqtoolkit.uniprotTools@seqtoolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace uniprot {
   /**
   */
   function get_description(prot: object): string;
   /**
   */
   function get_domain(prot: object): object;
   /**
   */
   function get_keywords(prot: object): object;
   /**
   */
   function get_pathways(prot: object): string;
   /**
   */
   function get_reactions(prot: object): any;
   /**
   */
   function get_sequence(prot: object): object;
   /**
   */
   function get_subcellularlocation(prot: object): any;
   /**
     * @param dbname default value Is ``null``.
   */
   function get_xrefs(prot: object, dbname?: string): object|string;
   /**
     * @param target default value Is ``null``.
     * @param env default value Is ``null``.
   */
   function id_unify(uniprot: any, id: any, target?: string, env?: object): any;
   /**
     * @param env default value Is ``null``.
   */
   function metaboliteSet(uniprot: any, env?: object): any;
   module open {
      /**
        * @param isUniParc default value Is ``false``.
        * @param ignoreError default value Is ``true``.
        * @param tqdm default value Is ``true``.
        * @param env default value Is ``null``.
      */
      function uniprot(files: any, isUniParc?: boolean, ignoreError?: boolean, tqdm?: boolean, env?: object): object;
   }
   /**
     * @param env default value Is ``null``.
   */
   function parseHeader(x: any, env?: object): object;
   /**
   */
   function parseUniProt(xml: string): object;
   module protein {
      /**
        * @param extractAll default value Is ``false``.
        * @param title default value Is ``'<uniprot_id> <fullname>'``.
        * @param env default value Is ``null``.
      */
      function seqs(uniprot: any, extractAll?: boolean, title?: string, env?: object): object;
   }
   /**
     * @param env default value Is ``null``.
   */
   function proteinTable(uniprot: any, env?: object): any;
   module read {
      /**
        * @param env default value Is ``null``.
      */
      function proteinTable(file: any, env?: object): object;
   }
}
