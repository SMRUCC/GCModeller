﻿// export R# package module type define for javascript/typescript language
//
// ref=seqtoolkit.patterns@seqtoolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * Tools for sequence patterns
 * 
*/
declare namespace bioseq.patterns {
   module view {
      /**
       * 
       * 
        * @param sites -
        * @param seq -
        * @param deli -
        * 
        * + default value Is ``', '``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function sites(sites:any, seq:any, deli?:string, env?:object): string;
   }
   module read {
      /**
       * read sequence motif json file.
       * 
       * > apply for search by @``M:seqtoolkit.patterns.matchSites(SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif.SequenceMotif,System.Object,System.Double,System.Double,System.Double,System.Boolean,SMRUCC.Rsharp.Runtime.Environment)``
       * 
        * @param file -
      */
      function motifs(file:string): object;
      /**
      */
      function scans(file:string): object;
   }
   module as {
      /**
        * @param mol_type default value Is ``null``.
        * @param env default value Is ``null``.
      */
      function seq_graph(fasta:any, mol_type?:object, env?:object): any;
   }
   module motif {
      /**
       * Find target loci site based on the given motif model
       * 
       * 
        * @param motif -
        * @param target a collection of fasta sequence
        * @param cutoff 
        * + default value Is ``0.6``.
        * @param minW 
        * + default value Is ``6``.
        * @param identities 
        * + default value Is ``0.85``.
        * @param parallel 
        * + default value Is ``false``.
        * @param env 
        * + default value Is ``null``.
      */
      function find_sites(motif:object, target:any, cutoff?:number, minW?:number, identities?:number, parallel?:boolean, env?:object): object;
   }
   module palindrome {
      /**
       * Search mirror palindrome sites for a given seed sequence
       * 
       * 
        * @param sequence -
        * @param seed -
      */
      function mirror(sequence:string, seed:string): object;
   }
   /**
    * Create seeds
    * 
    * 
     * @param size -
     * @param base -
   */
   function seeds(size:object, base:string): string;
   /**
     * @param env default value Is ``null``.
   */
   function motifString(motif:any, env?:object): any;
   /**
    * find possible motifs of the given sequence collection
    * 
    * 
     * @param fasta -
     * @param minw 
     * + default value Is ``6``.
     * @param maxw 
     * + default value Is ``20``.
     * @param nmotifs 
     * + default value Is ``25``.
     * @param noccurs 
     * + default value Is ``6``.
     * @param seedingCutoff 
     * + default value Is ``0.95``.
     * @param scanMinW 
     * + default value Is ``6``.
     * @param scanCutoff 
     * + default value Is ``0.8``.
     * @param cleanMotif 
     * + default value Is ``0.5``.
     * @param env 
     * + default value Is ``null``.
   */
   function find_motifs(fasta:any, minw?:object, maxw?:object, nmotifs?:object, noccurs?:object, seedingCutoff?:number, scanMinW?:object, scanCutoff?:number, cleanMotif?:number, env?:object): object;
   module plot {
      /**
       * Drawing the sequence logo just simply modelling this motif site 
       *  from the clustal multiple sequence alignment.
       * 
       * 
        * @param MSA -
        * @param title -
        * 
        * + default value Is ``''``.
        * @param env 
        * + default value Is ``null``.
      */
      function seqLogo(MSA:any, title?:string, env?:object): object;
   }
   module scaffold {
      /**
       * analyses orthogonality of two DNA-Origami scaffold strands.
       *  Multiple criteria For orthogonality Of the two sequences can be specified
       *  to determine the level of orthogonality.
       * 
       * 
        * @param scaffolds -
        * @param segment_len segment length
        * 
        * + default value Is ``7``.
        * @param is_linear scaffolds are not circular
        * 
        * + default value Is ``false``.
        * @param rev_compl also count reverse complementary sequences
        * 
        * + default value Is ``false``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function orthogonality(scaffolds:any, segment_len?:object, is_linear?:boolean, rev_compl?:boolean, env?:object): object;
   }
}