// export R# package module type define for javascript/typescript language
//
// ref=seqtoolkit.patterns

/**
 * Tools for sequence patterns
 * 
*/
declare namespace bioseq.patterns {
   module view {
      /**
        * @param deli default value is ``', '``.
        * @param env default value is ``null``.
      */
      function sites(sites:any, seq:any, deli?:string, env?:object): any;
   }
   module read {
      /**
      */
      function motifs(file:string): any;
      /**
      */
      function scans(file:string): any;
   }
   module as {
      /**
        * @param mol_type default value is ``null``.
        * @param env default value is ``null``.
      */
      function seq_graph(fasta:any, mol_type?:object, env?:object): any;
   }
   module motif {
      /**
        * @param cutoff default value is ``0.6``.
        * @param minW default value is ``6``.
        * @param identities default value is ``0.85``.
        * @param parallel default value is ``false``.
        * @param env default value is ``null``.
      */
      function find_sites(motif:object, target:any, cutoff?:number, minW?:number, identities?:number, parallel?:boolean, env?:object): any;
   }
   module palindrome {
      /**
      */
      function mirror(sequence:string, seed:string): any;
   }
   /**
   */
   function seeds(size:object, base:string): any;
   /**
     * @param env default value is ``null``.
   */
   function motifString(motif:any, env?:object): any;
   /**
     * @param minw default value is ``6``.
     * @param maxw default value is ``20``.
     * @param nmotifs default value is ``25``.
     * @param noccurs default value is ``6``.
     * @param seedingCutoff default value is ``0.95``.
     * @param scanMinW default value is ``6``.
     * @param scanCutoff default value is ``0.8``.
     * @param cleanMotif default value is ``0.5``.
     * @param env default value is ``null``.
   */
   function find_motifs(fasta:any, minw?:object, maxw?:object, nmotifs?:object, noccurs?:object, seedingCutoff?:number, scanMinW?:object, scanCutoff?:number, cleanMotif?:number, env?:object): any;
   module plot {
      /**
        * @param title default value is ``''``.
        * @param env default value is ``null``.
      */
      function seqLogo(MSA:any, title?:string, env?:object): any;
   }
   module scaffold {
      /**
        * @param segment_len default value is ``7``.
        * @param is_linear default value is ``false``.
        * @param rev_compl default value is ``false``.
        * @param env default value is ``null``.
      */
      function orthogonality(scaffolds:any, segment_len?:object, is_linear?:boolean, rev_compl?:boolean, env?:object): any;
   }
}
