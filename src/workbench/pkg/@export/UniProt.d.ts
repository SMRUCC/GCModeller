// export R# package module type define for javascript/typescript language
//
// ref=seqtoolkit.uniprot

/**
 * The Universal Protein Resource (UniProt)
 * 
*/
declare namespace uniprot {
   module open {
      /**
        * @param isUniParc default value is ``false``.
        * @param ignoreError default value is ``true``.
        * @param env default value is ``null``.
      */
      function uniprot(files:any, isUniParc?:boolean, ignoreError?:boolean, env?:object): any;
   }
   /**
   */
   function parseUniProt(xml:string): any;
   /**
     * @param env default value is ``null``.
   */
   function proteinTable(uniprot:any, env?:object): any;
   module protein {
      /**
        * @param extractAll default value is ``false``.
        * @param KOseq default value is ``false``.
        * @param env default value is ``null``.
      */
      function seqs(uniprot:any, extractAll?:boolean, KOseq?:boolean, env?:object): any;
   }
   /**
     * @param target default value is ``null``.
     * @param env default value is ``null``.
   */
   function id_unify(uniprot:any, id:any, target?:string, env?:object): any;
   /**
     * @param env default value is ``null``.
   */
   function metaboliteSet(uniprot:any, env?:object): any;
}
