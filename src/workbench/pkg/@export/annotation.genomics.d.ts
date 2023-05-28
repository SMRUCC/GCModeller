// export R# package module type define for javascript/typescript language
//
//    imports "annotation.genomics" from "seqtoolkit"
//
// ref=seqtoolkit.genomics@seqtoolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace annotation.genomics {
   module as {
      /**
      */
      function geneTable(PTT: object): object;
      /**
      */
      function PTT(gb: object): object;
      /**
        * @param title default value Is ``'n/a'``.
        * @param size default value Is ``0``.
        * @param format default value Is ``'PTT|GFF|GTF'``.
        * @param env default value Is ``null``.
      */
      function tabular(genes: object, title?: string, size?: object, format?: string, env?: object): any;
   }
   module genome {
      /**
        * @param env default value Is ``null``.
      */
      function genes(genome: any, env?: object): object;
   }
   module read {
      /**
      */
      function gtf(file: string): object;
   }
   /**
     * @param length default value Is ``200``.
     * @param isRelativeOffset default value Is ``false``.
   */
   function upstream(context: object, length?: object, isRelativeOffset?: boolean): object;
   module write {
      /**
        * @param file default value Is ``null``.
        * @param encoding default value Is ``null``.
        * @param env default value Is ``null``.
      */
      function PTT_tabular(genomics: any, file?: string, encoding?: object, env?: object): any;
   }
}
