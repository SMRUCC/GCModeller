// export R# package module type define for javascript/typescript language
//
//    imports "genomics_context" from "seqtoolkit";
//
// ref=seqtoolkit.context@seqtoolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace genomics_context {
   /**
     * @param note default value Is ``null``.
     * @param env default value Is ``null``.
   */
   function context(loci: any, distance: object, note?: string, env?: object): object;
   /**
     * @param eval_thres default value Is ``1.7976931348623157E+308``.
     * @param env default value Is ``null``.
   */
   function context_location(blastn: any, eval_thres?: number, env?: object): object;
   /**
     * @param strand default value Is ``'+'``.
     * @param env default value Is ``null``.
   */
   function filter_strand(genes: any, strand?: any, env?: object): any;
   /**
     * @param chr_name default value Is ``null``.
     * @param strict default value Is ``false``.
     * @param env default value Is ``null``.
   */
   function genomics_context(gff: object, chr_name?: string, strict?: boolean, env?: object): object;
   module is {
      /**
      */
      function forward(loci: object): boolean;
   }
   /**
     * @param strand default value Is ``null``.
   */
   function location(left: object, right: object, strand?: any): object;
   /**
   */
   function offset(loci: object, offset: object): object;
   /**
   */
   function primer_coverage(targetHits: object, chr: object, chr_seq: object): object;
   /**
     * @param env default value Is ``null``.
   */
   function relationship(a: any, b: any, env?: object): object;
   /**
     * @param env default value Is ``null``.
   */
   function set_context(sites: any, genomics: object, env?: object): any;
   /**
     * @param genes default value Is ``null``.
     * @param upstream_len default value Is ``150``.
     * @param simple_title default value Is ``true``.
     * @param env default value Is ``null``.
   */
   function TSS_upstream(genome: any, genes?: any, upstream_len?: object, simple_title?: boolean, env?: object): any;
}
