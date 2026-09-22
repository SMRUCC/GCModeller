// export R# package module type define for javascript/typescript language
//
//    imports "bioseq.patterns" from "seqtoolkit";
//
// ref=seqtoolkit.patterns@seqtoolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace bioseq.patterns {
   module as {
      /**
        * @param mol_type default value Is ``null``.
        * @param parallel default value Is ``false``.
        * @param env default value Is ``null``.
      */
      function seq_graph(fasta: any, mol_type?: object, parallel?: boolean, env?: object): object;
   }
   module create {
      /**
        * @param minw default value Is ``8``.
        * @param maxw default value Is ``20``.
        * @param seedingCutoff default value Is ``0.95``.
        * @param scanMinW default value Is ``6``.
        * @param scanCutoff default value Is ``0.8``.
        * @param significant_sites default value Is ``4``.
        * @param debug default value Is ``false``.
        * @param env default value Is ``null``.
      */
      function seeds(fasta: any, saveto: object, minw?: object, maxw?: object, seedingCutoff?: number, scanMinW?: object, scanCutoff?: number, significant_sites?: object, debug?: boolean, env?: object): any;
   }
   /**
     * @param minw default value Is ``8``.
     * @param maxw default value Is ``20``.
     * @param nmotifs default value Is ``-1``.
     * @param noccurs default value Is ``12``.
     * @param seedingCutoff default value Is ``0.65``.
     * @param scanMinW default value Is ``6``.
     * @param scanCutoff default value Is ``0.8``.
     * @param cleanMotif default value Is ``0.5``.
     * @param significant_sites default value Is ``4``.
     * @param seeds default value Is ``null``.
     * @param debug default value Is ``false``.
     * @param env default value Is ``null``.
   */
   function find_motifs(fasta: any, minw?: object, maxw?: object, nmotifs?: object, noccurs?: object, seedingCutoff?: number, scanMinW?: object, scanCutoff?: number, cleanMotif?: number, significant_sites?: object, seeds?: any, debug?: boolean, env?: object): object;
   /**
     * @param width default value Is ``null``.
     * @param maxitr default value Is ``1000``.
     * @param env default value Is ``null``.
   */
   function gibbs_scan(seqs: any, width?: object, maxitr?: object, env?: object): object;
   module motif {
      /**
        * @param cutoff default value Is ``0.6``.
        * @param minW default value Is ``8``.
        * @param identities default value Is ``0.85``.
        * @param pvalue default value Is ``0.05``.
        * @param parallel default value Is ``false``.
        * @param motif_name default value Is ``null``.
        * @param env default value Is ``null``.
      */
      function find_sites(motif: any, target: any, cutoff?: number, minW?: number, identities?: number, pvalue?: number, parallel?: boolean, motif_name?: string, env?: object): object;
   }
   /**
     * @param env default value Is ``null``.
   */
   function motifString(motif: any, env?: object): string;
   module open {
      /**
        * @param env default value Is ``null``.
      */
      function seedFile(file: any, env?: object): any;
   }
   module palindrome {
      /**
      */
      function mirror(sequence: string, seed: string): object;
   }
   module plot {
      /**
        * @param title default value Is ``''``.
        * @param env default value Is ``null``.
      */
      function seqLogo(MSA: any, title?: string, env?: object): object;
   }
   module pull {
      /**
      */
      function all_seeds(seed: object): object;
   }
   module read {
      /**
      */
      function meme_xml(file: string): object;
      /**
      */
      function motifs(file: string): object;
      /**
        * @param tqdm default value Is ``false``.
      */
      function scans(file: string, tqdm?: boolean): object;
   }
   module scaffold {
      /**
        * @param segment_len default value Is ``7``.
        * @param is_linear default value Is ``false``.
        * @param rev_compl default value Is ``false``.
        * @param env default value Is ``null``.
      */
      function orthogonality(scaffolds: any, segment_len?: object, is_linear?: boolean, rev_compl?: boolean, env?: object): object;
   }
   /**
   */
   function seeds(size: object, base: string): string;
   /**
     * @param gff default value Is ``null``.
     * @param env default value Is ``null``.
   */
   function split_match_source(matches: any, gff?: object, env?: object): any;
   /**
     * @param identities default value Is ``null``.
     * @param pvalue default value Is ``null``.
     * @param minW default value Is ``null``.
   */
   function top_sites(sites: object, identities?: object, pvalue?: object, minW?: object): object;
   /**
   */
   function toPWM(meme: object): object;
   module view {
      /**
        * @param deli default value Is ``', '``.
        * @param env default value Is ``null``.
      */
      function sites(sites: any, seq: any, deli?: string, env?: object): string;
   }
}
