declare namespace bioseq.fasta {
   /**
   */
   function size(fa:object): any;
   module read {
      /**
        * @param env default value is ``null``.
      */
      function seq(file:string, env?:object): any;
      /**
        * @param lazyStream default value is ``false``.
      */
      function fasta(file:string, lazyStream?:boolean): any;
   }
   module open {
      /**
        * @param env default value is ``null``.
      */
      function fasta(file:string, env?:object): any;
   }
   module write {
      /**
        * @param lineBreak default value is ``-1``.
        * @param delimiter default value is ``' '``.
        * @param encoding default value is ``null``.
        * @param env default value is ``null``.
      */
      function fasta(seq:any, file:string, lineBreak?:object, delimiter?:string, encoding?:object, env?:object): any;
   }
   /**
     * @param table default value is ``null``.
     * @param bypassStop default value is ``true``.
     * @param checkNt default value is ``true``.
     * @param env default value is ``null``.
   */
   function translate(nt:any, table?:object, bypassStop?:boolean, checkNt?:boolean, env?:object): any;
   module MSA {
      /**
        * @param env default value is ``null``.
      */
      function of(seqs:any, env?:object): any;
   }
   module as {
      /**
        * @param env default value is ``null``.
      */
      function fasta(x:any, env?:object): any;
   }
   /**
   */
   function fasta(seq:string, attrs:string): any;
   module cut_seq {
      /**
        * @param doNtAutoReverse default value is ``false``.
        * @param env default value is ``null``.
      */
      function linear(seq:any, loci:any, doNtAutoReverse?:boolean, env?:object): any;
   }
}
