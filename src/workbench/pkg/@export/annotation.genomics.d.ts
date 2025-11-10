// export R# package module type define for javascript/typescript language
//
//    imports "annotation.genomics" from "seqtoolkit";
//
// ref=seqtoolkit.genomics@seqtoolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * 
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
   module genome {
      /**
        * @param env default value Is ``null``.
      */
      function genes(genome: any, env?: object): object;
   }
   /**
    * get gff features by id reference
    * 
    * 
     * @param gff -
     * @param id -
     * 
     * + default value Is ``null``.
   */
   function gff_features(gff: object, id?: any): any;
   /**
    * load operon set data from the ODB database
    * 
    * 
     * @param file dataset text file that download from https://operondb.jp/
     * 
     * + default value Is ``null``.
   */
   function operon_set(file?: string): object;
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
    * Create the upstream location
    * 
    * 
     * @param context th gene element location context data
     * @param length bit length of the upstream location
     * 
     * + default value Is ``200``.
     * @param is_relative_offset Does the generates context upstream location is relative to the 
     *  given context start position or the enitre context region move
     *  by upstream offset bits?
     * 
     * + default value Is ``true``.
   */
   function upstream(context: object, length?: object, is_relative_offset?: boolean): object;
   module write {
      /**
        * @param file default value Is ``null``.
        * @param encoding default value Is ``null``.
        * @param env default value Is ``null``.
      */
      function PTT_tabular(genomics: any, file?: string, encoding?: object, env?: object): any;
   }
}
