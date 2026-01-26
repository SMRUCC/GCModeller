// export R# package module type define for javascript/typescript language
//
//    imports "bioseq.fasta" from "seqtoolkit";
//
// ref=seqtoolkit.Fasta@seqtoolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * Fasta sequence toolkit
 * 
*/
declare namespace bioseq.fasta {
   module as {
      /**
       * Create a fasta sequence collection object from any given sequence collection.
       * 
       * 
        * @param x any type of sequence collection
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function fasta(x: any, env?: object): object;
   }
   /**
    * get alphabets represents of the fasta sequence
    * 
    * 
     * @param type the sequence data type.
     * 
     * + default value Is ``null``.
   */
   function chars(type?: object): string;
   module cut_seq {
      /**
       * cut part of the sequence
       * 
       * 
        * @param seq -
        * @param loci the location region data for make cut of the sequence site, data model could be:
        *  
        *  1. for nucleotide sequence, @``T:SMRUCC.genomics.ComponentModel.Loci.NucleotideLocation`` should be used,
        *  2. for general sequence data, @``T:SMRUCC.genomics.ComponentModel.Loci.Location`` should be used.
        * @param nt_auto_reverse make auto reverse of the nucleotide sequence if the given location is on 
        *  the @``F:SMRUCC.genomics.ComponentModel.Loci.Strands.Reverse`` direction.
        * 
        * + default value Is ``false``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function linear(seq: any, loci: any, nt_auto_reverse?: boolean, env?: object): any;
   }
   module fasta {
      /**
       * get/set the fasta headers title
       * 
       * 
        * @param fa -
        * @param headers -
        * 
        * + default value Is ``null``.
      */
      function headers(fa: object, headers?: string): string;
      /**
       * get the fasta titles from a collection of fasta sequence
       * 
       * 
        * @param fa -
        * @param env 
        * + default value Is ``null``.
      */
      function titles(fa: any, env?: object): string;
   }
   /**
     * @param env default value Is ``null``.
   */
   function make_clusterTree(fingerprints: any, env?: object): object;
   /**
    * evaluate the molecule mass of the given sequence
    * 
    * 
     * @param seqs -
     * @param type -
     * 
     * + default value Is ``null``.
     * @param env 
     * + default value Is ``null``.
   */
   function mass(seqs: any, type?: object, env?: object): any;
   module MSA {
      /**
       * Do multiple sequence alignment
       * 
       * 
        * @param seqs A fasta sequence collection
        * @param env 
        * + default value Is ``null``.
      */
      function of(seqs: any, env?: object): object;
   }
   module open {
      /**
       * open the fasta sequence file
       * 
       * 
        * @param file -
        * @param read load a set of fasta sequence data in lazy mode? default is yes.
        * 
        * + default value Is ``true``.
        * @param line_break 
        * + default value Is ``-1``.
        * @param delimiter 
        * + default value Is ``'|'``.
        * @param env -
        * 
        * + default value Is ``null``.
        * @return a lazy collection of the fasta sequence data
      */
      function fasta(file: string, read?: boolean, line_break?: object, delimiter?: string, env?: object): object|object;
      /**
        * @param env default value Is ``null``.
      */
      function fingerprint_writer(file: any, env?: object): object;
   }
   module parse {
      /**
       * parse the fasta sequence object from the given text data
       * 
       * 
        * @param x -
      */
      function fasta(x: any): object;
   }
   module read {
      /**
       * read a fasta sequence collection file
       * 
       * 
        * @param file -
        * @param lazyStream 
        * + default value Is ``false``.
        * @return A collection of the fasta sequence object
      */
      function fasta(file: string, lazyStream?: boolean): object;
      /**
        * @param env default value Is ``null``.
      */
      function fingerprint_bson(file: any, env?: object): object;
      /**
       * Read a single fasta sequence file
       * 
       * > for input a genbank database file, this function will extract the origin sequence fasta object
       * 
        * @param file Just contains one sequence
        * @param env 
        * + default value Is ``null``.
      */
      function seq(file: string, env?: object): object;
   }
   /**
     * @param type default value Is ``null``.
     * @param env default value Is ``null``.
   */
   function seq_formula(seqs: any, type?: object, env?: object): any;
   /**
    * Create algorithm for make sequence embedding
    * 
    * 
     * @param moltype -
     * 
     * + default value Is ``null``.
     * @param kappa 
     * + default value Is ``1``.
     * @param lengthsensitive 
     * + default value Is ``false``.
   */
   function seq_sgt(moltype?: object, kappa?: number, lengthsensitive?: boolean): object;
   /**
    * embedding the given fasta sequence as vector
    * 
    * 
     * @param sgt -
     * @param seqs -
     * @param as_dataframe 
     * + default value Is ``false``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function seq_vector(sgt: object, seqs: any, as_dataframe?: boolean, env?: object): number;
   /**
    * get the sequence length
    * 
    * 
     * @param fa -
   */
   function size(fa: object): object;
   /**
     * @param env default value Is ``null``.
   */
   function takes(x: any, gene_ids: any, env?: object): object;
   /**
    * Do translation of the nt sequence to protein sequence
    * 
    * 
     * @param nt The given fasta collection
     * @param table The genetic code for translation table.
     * 
     * + default value Is ``null``.
     * @param bypassStop Try ignores of the stop codon.
     * 
     * + default value Is ``true``.
     * @param checkNt 
     * + default value Is ``true``.
     * @param env 
     * + default value Is ``null``.
   */
   function translate(nt: any, table?: object, bypassStop?: boolean, checkNt?: boolean, env?: object): any;
   module write {
      /**
       * write a fasta sequence or a collection of fasta sequence object
       * 
       * 
        * @param seq -
        * @param file -
        * @param lineBreak The sequence length in one line, negative value or ZERo means no line break.
        * 
        * + default value Is ``-1``.
        * @param delimiter 
        * + default value Is ``' '``.
        * @param filter_empty skip write sequence if the sequence object has no sequence data
        * 
        * + default value Is ``false``.
        * @param encoding The text encoding value of the generated fasta file.
        * 
        * + default value Is ``null``.
        * @param env 
        * + default value Is ``null``.
      */
      function fasta(seq: any, file: any, lineBreak?: object, delimiter?: string, filter_empty?: boolean, encoding?: object, env?: object): boolean;
   }
   /**
     * @param debug default value Is ``-1``.
     * @param env default value Is ``null``.
   */
   function write_fingerprint(file: object, seqs: any, debug?: object, env?: object): object;
}
