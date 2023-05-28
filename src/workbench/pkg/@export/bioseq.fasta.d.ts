// export R# package module type define for javascript/typescript language
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
   module cut_seq {
      /**
        * @param doNtAutoReverse default value Is ``false``.
        * @param env default value Is ``null``.
      */
      function linear(seq: any, loci: any, doNtAutoReverse?: boolean, env?: object): any;
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
        * @param env default value Is ``null``.
      */
      function fasta(file: string, env?: object): any;
   }
   module read {
      /**
       * read a fasta sequence collection file
       * 
       * 
        * @param file -
        * @param lazyStream 
        * + default value Is ``false``.
      */
      function fasta(file: string, lazyStream?: boolean): object;
      /**
       * Read a single fasta sequence file
       * 
       * 
        * @param file Just contains one sequence
        * @param env 
        * + default value Is ``null``.
      */
      function seq(file: string, env?: object): object;
   }
   /**
    * get the sequence length
    * 
    * 
     * @param fa -
   */
   function size(fa: object): object;
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
        * @param encoding The text encoding value of the generated fasta file.
        * 
        * + default value Is ``null``.
        * @param env 
        * + default value Is ``null``.
      */
      function fasta(seq: any, file: string, lineBreak?: object, delimiter?: string, encoding?: object, env?: object): boolean;
   }
}
