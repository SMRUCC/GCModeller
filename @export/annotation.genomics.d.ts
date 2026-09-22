// export R# package module type define for javascript/typescript language
//
//    imports "annotation.genomics" from "seqtoolkit";
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
      function tabular(genes: object, title?: string, size?: object, format?: string, env?: object): object;
   }
   /**
     * @param env default value Is ``null``.
   */
   function extract_gff_seqs(gff3: object, seqs: any, env?: object): any;
   /**
     * @param env default value Is ``null``.
   */
   function genes_features(genome: any, env?: object): object;
   /**
     * @param id default value Is ``null``.
   */
   function gff_features(gff: object, id?: any): any;
   module read {
      /**
      */
      function gff(file: string): object;
      /**
      */
      function gtf(file: string): object;
      /**
      */
      function nucmer(file: string): object;
   }
   /**
   */
   function source_features(gff: object, source: string): object;
   /**
   */
   function type_features(gff: object, type: string): object;
   /**
     * @param length default value Is ``200``.
     * @param is_relative_offset default value Is ``true``.
   */
   function upstream(context: object, length?: object, is_relative_offset?: boolean): object;
   module write {
      /**
      */
      function gff3(gff: object, file: string): boolean;
      /**
        * @param file default value Is ``null``.
        * @param encoding default value Is ``null``.
        * @param env default value Is ``null``.
      */
      function PTT_tabular(genomics: any, file?: string, encoding?: object, env?: object): any;
   }
}
