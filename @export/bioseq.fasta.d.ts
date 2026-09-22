// export R# package module type define for javascript/typescript language
//
//    imports "bioseq.fasta" from "seqtoolkit";
//
// ref=seqtoolkit.Fasta@seqtoolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace bioseq.fasta {
   module as {
      /**
        * @param env default value Is ``null``.
      */
      function fasta(x: any, env?: object): object;
   }
   /**
     * @param type default value Is ``null``.
   */
   function chars(type?: object): string;
   module cut_seq {
      /**
        * @param nt_auto_reverse default value Is ``false``.
        * @param env default value Is ``null``.
      */
      function linear(seq: any, loci: any, nt_auto_reverse?: boolean, env?: object): any;
   }
   module fasta {
      /**
        * @param headers default value Is ``null``.
      */
      function headers(fa: object, headers?: string): string;
      /**
        * @param env default value Is ``null``.
      */
      function titles(fa: any, env?: object): string;
   }
   /**
     * @param ids default value Is ``null``.
     * @param env default value Is ``null``.
   */
   function list_index(x: any, ids?: any, env?: object): object;
   /**
     * @param env default value Is ``null``.
   */
   function make_clusterTree(fingerprints: any, env?: object): object;
   /**
     * @param type default value Is ``null``.
     * @param env default value Is ``null``.
   */
   function mass(seqs: any, type?: object, env?: object): any;
   module MSA {
      /**
        * @param env default value Is ``null``.
      */
      function of(seqs: any, env?: object): object;
   }
   module open {
      /**
        * @param read default value Is ``true``.
        * @param line_break default value Is ``-1``.
        * @param delimiter default value Is ``'|'``.
        * @param env default value Is ``null``.
      */
      function fasta(file: string, read?: boolean, line_break?: object, delimiter?: string, env?: object): object|object;
      /**
        * @param env default value Is ``null``.
      */
      function fingerprint_writer(file: any, env?: object): object;
   }
   module parse {
      /**
      */
      function fasta(x: any): object;
   }
   module read {
      /**
        * @param lazyStream default value Is ``false``.
      */
      function fasta(file: string, lazyStream?: boolean): object;
      /**
        * @param env default value Is ``null``.
      */
      function fingerprint_bson(file: any, env?: object): object;
      /**
        * @param env default value Is ``null``.
      */
      function seq(file: string, env?: object): object;
   }
   /**
     * @param env default value Is ``null``.
   */
   function read_assembly(file: any, env?: object): object;
   /**
   */
   function read_stockholm(file: string): object;
   /**
     * @param type default value Is ``null``.
     * @param env default value Is ``null``.
   */
   function seq_formula(seqs: any, type?: object, env?: object): any;
   /**
     * @param moltype default value Is ``null``.
     * @param kappa default value Is ``1``.
     * @param lengthsensitive default value Is ``false``.
   */
   function seq_sgt(moltype?: object, kappa?: number, lengthsensitive?: boolean): object;
   /**
     * @param as_dataframe default value Is ``false``.
     * @param env default value Is ``null``.
   */
   function seq_vector(sgt: object, seqs: any, as_dataframe?: boolean, env?: object): number;
   /**
   */
   function size(fa: object): object;
   /**
     * @param env default value Is ``null``.
   */
   function slicer(fa: any, env?: object): object|object|object|object;
   /**
     * @param env default value Is ``null``.
   */
   function takes(x: any, gene_ids: any, env?: object): object;
   /**
     * @param table default value Is ``null``.
     * @param bypassStop default value Is ``true``.
     * @param checkNt default value Is ``true``.
     * @param env default value Is ``null``.
   */
   function translate(nt: any, table?: object, bypassStop?: boolean, checkNt?: boolean, env?: object): any;
   module write {
      /**
        * @param lineBreak default value Is ``-1``.
        * @param delimiter default value Is ``' '``.
        * @param filter_empty default value Is ``false``.
        * @param encoding default value Is ``null``.
        * @param env default value Is ``null``.
      */
      function fasta(seq: any, file: any, lineBreak?: object, delimiter?: string, filter_empty?: boolean, encoding?: object, env?: object): boolean;
   }
   /**
     * @param debug default value Is ``-1``.
     * @param env default value Is ``null``.
   */
   function write_fingerprint(file: object, seqs: any, debug?: object, env?: object): object;
}
