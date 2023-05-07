// export R# package module type define for javascript/typescript language
//
// ref=seqtoolkit.genomics

/**
*/
declare namespace annotation.genomics {
   module read {
      /**
      */
      function gtf(file:string): any;
   }
   module as {
      /**
        * @param title default value is ``'n/a'``.
        * @param size default value is ``0``.
        * @param format default value is ``'PTT|GFF|GTF'``.
        * @param env default value is ``null``.
      */
      function tabular(genes:object, title?:string, size?:object, format?:string, env?:object): any;
      /**
      */
      function geneTable(PTT:object): any;
      /**
      */
      function PTT(gb:object): any;
   }
   /**
     * @param length default value is ``200``.
     * @param isRelativeOffset default value is ``false``.
   */
   function upstream(context:object, length?:object, isRelativeOffset?:boolean): any;
   module genome {
      /**
        * @param env default value is ``null``.
      */
      function genes(genome:any, env?:object): any;
   }
   module write {
      /**
        * @param file default value is ``null``.
        * @param encoding default value is ``null``.
        * @param env default value is ``null``.
      */
      function PTT_tabular(genomics:any, file?:string, encoding?:object, env?:object): any;
   }
}
