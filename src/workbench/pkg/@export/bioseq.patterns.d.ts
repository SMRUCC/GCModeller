// export R# package module type define for javascript/typescript language
//
//    imports "bioseq.patterns" from "seqtoolkit";
//
// ref=seqtoolkit.patterns@seqtoolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * Tools for sequence patterns
 * 
*/
declare namespace bioseq.patterns {
   module as {
      /**
       * 
       * 
        * @param fasta -
        * @param mol_type -
        * 
        * + default value Is ``null``.
        * @param parallel -
        * 
        * + default value Is ``false``.
        * @param env -
        * 
        * + default value Is ``null``.
        * @return the sequence graph embedding vector data is generates from different method 
        *  based on the **`mol_type`** data:
        *  
        *  + @``F:SMRUCC.genomics.SequenceModel.SeqTypes.DNA``: @``M:SMRUCC.genomics.Model.MotifGraph.Builder.DNAGraph(SMRUCC.genomics.SequenceModel.FASTA.FastaSeq)``
        *  + @``F:SMRUCC.genomics.SequenceModel.SeqTypes.Protein``: @``M:SMRUCC.genomics.Model.MotifGraph.Builder.PolypeptideGraph(SMRUCC.genomics.SequenceModel.FASTA.FastaSeq)``
        *  + @``F:SMRUCC.genomics.SequenceModel.SeqTypes.RNA``: @``M:SMRUCC.genomics.Model.MotifGraph.Builder.RNAGraph(SMRUCC.genomics.SequenceModel.FASTA.FastaSeq)``
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
    * find possible motifs of the given sequence collection
    * 
    * 
     * @param fasta should contains multiple sequence
     * @param minw 
     * + default value Is ``8``.
     * @param maxw 
     * + default value Is ``20``.
     * @param nmotifs A number for limit the number of motif outputs:
     *  
     *  + negative integer/zero: no limits[default]
     *  + positive value: top motifs with score desc
     * 
     * + default value Is ``-1``.
     * @param noccurs 
     * + default value Is ``12``.
     * @param seedingCutoff 
     * + default value Is ``0.65``.
     * @param scanMinW 
     * + default value Is ``6``.
     * @param scanCutoff 
     * + default value Is ``0.8``.
     * @param cleanMotif 
     * + default value Is ``0.5``.
     * @param significant_sites 
     * + default value Is ``4``.
     * @param seeds 
     * + default value Is ``null``.
     * @param debug 
     * + default value Is ``false``.
     * @param env 
     * + default value Is ``null``.
   */
   function find_motifs(fasta: any, minw?: object, maxw?: object, nmotifs?: object, noccurs?: object, seedingCutoff?: number, scanMinW?: object, scanCutoff?: number, cleanMotif?: number, significant_sites?: object, seeds?: any, debug?: boolean, env?: object): object;
   /**
    * make a motif scan from the given sequence collection
    * 
    * 
     * @param seqs -
     * @param width -
     * 
     * + default value Is ``null``.
     * @param maxitr -
     * 
     * + default value Is ``1000``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function gibbs_scan(seqs: any, width?: object, maxitr?: object, env?: object): object;
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
        * + default value Is ``8``.
        * @param identities 
        * + default value Is ``0.85``.
        * @param pvalue 
        * + default value Is ``0.05``.
        * @param parallel 
        * + default value Is ``false``.
        * @param env 
        * + default value Is ``null``.
      */
      function find_sites(motif: object, target: any, cutoff?: number, minW?: number, identities?: number, pvalue?: number, parallel?: boolean, env?: object): object;
   }
   /**
    * 
    * 
     * @param motif -
     * @param env -
     * 
     * + default value Is ``null``.
     * @return the regexp liked format string for do motif matches
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
       * Search mirror palindrome sites for a given seed sequence
       * 
       * 
        * @param sequence -
        * @param seed -
      */
      function mirror(sequence: string, seed: string): object;
   }
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
      function seqLogo(MSA: any, title?: string, env?: object): object;
   }
   module pull {
      /**
      */
      function all_seeds(seed: object): object;
   }
   module read {
      /**
       * read sequence motif json file.
       * 
       * > apply for search by @``M:seqtoolkit.patterns.matchSites(SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif.SequenceMotif,System.Object,System.Double,System.Double,System.Double,System.Double,System.Boolean,SMRUCC.Rsharp.Runtime.Environment)``
       * 
        * @param file -
      */
      function motifs(file: string): object;
      /**
       * read the motif match scan result table file
       * 
       * 
        * @param file should be a file path to a csv table file.
      */
      function scans(file: string): object;
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
      function orthogonality(scaffolds: any, segment_len?: object, is_linear?: boolean, rev_compl?: boolean, env?: object): object;
   }
   /**
    * Create seeds
    * 
    * 
     * @param size -
     * @param base -
   */
   function seeds(size: object, base: string): string;
   /**
    * split the motif matches result in parts by its gene source
    * 
    * 
     * @param matches -
     * @param gff -
     * 
     * + default value Is ``null``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function split_match_source(matches: any, gff?: object, env?: object): any;
   /**
     * @param identities default value Is ``null``.
     * @param pvalue default value Is ``null``.
     * @param minW default value Is ``null``.
   */
   function top_sites(sites: object, identities?: object, pvalue?: object, minW?: object): object;
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
      function sites(sites: any, seq: any, deli?: string, env?: object): string;
   }
}
