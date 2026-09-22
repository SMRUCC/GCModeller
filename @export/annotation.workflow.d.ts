// export R# package module type define for javascript/typescript language
//
//    imports "annotation.workflow" from "seqtoolkit";
//
// ref=seqtoolkit.workflows@seqtoolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace annotation.workflow {
   /**
     * @param evalue default value Is ``null``.
     * @param identities default value Is ``null``.
     * @param delNohits default value Is ``true``.
     * @param pickTop default value Is ``false``.
     * @param env default value Is ``null``.
   */
   function besthit_filter(besthits: object, evalue?: object, identities?: object, delNohits?: boolean, pickTop?: boolean, env?: object): object;
   /**
   */
   function blast_tabular(hits: object, args: object, env: object): any;
   module blasthit {
      /**
        * @param algorithm default value Is ``null``.
        * @param env default value Is ``null``.
      */
      function bbh(forward: object, reverse: object, algorithm?: object, env?: object): object;
      /**
        * @param idetities default value Is ``0.3``.
        * @param coverage default value Is ``0.5``.
        * @param topBest default value Is ``false``.
        * @param keepsRawName default value Is ``false``.
        * @param env default value Is ``null``.
      */
      function sbh(query: object, idetities?: number, coverage?: number, topBest?: boolean, keepsRawName?: boolean, env?: object): object;
   }
   module blastn {
      /**
        * @param top_best default value Is ``false``.
        * @param env default value Is ``null``.
      */
      function maphit(query: object, top_best?: boolean, env?: object): object;
   }
   /**
     * @param env default value Is ``null``.
   */
   function diamond_hitgroups(x: any, env?: object): object;
   /**
     * @param env default value Is ``null``.
   */
   function filter_low_level(bbh: any, env?: object): any;
   module grep {
      /**
        * @param applyOnHits default value Is ``false``.
        * @param env default value Is ``null``.
      */
      function names(query: object, operators: any, applyOnHits?: boolean, env?: object): object;
   }
   module open {
      /**
        * @param type default value Is ``null``.
        * @param encoding default value Is ``null``.
        * @param ioRead default value Is ``false``.
        * @param env default value Is ``null``.
      */
      function stream(file: string, type?: object, encoding?: object, ioRead?: boolean, env?: object): any;
   }
   module read {
      /**
        * @param encoding default value Is ``null``.
      */
      function bbh_hits(file: string, encoding?: object): object;
      /**
        * @param encoding default value Is ``null``.
      */
      function besthits(file: string, encoding?: object): object;
      /**
        * @param type default value Is ``'nucl'``.
        * @param fastMode default value Is ``true``.
        * @param env default value Is ``null``.
      */
      function blast(file: string, type?: string, fastMode?: boolean, env?: object): object;
      /**
        * @param make_query_group default value Is ``false``.
        * @param env default value Is ``null``.
      */
      function outfmt6(file: any, make_query_group?: boolean, env?: object): object;
   }
   /**
     * @param stream default value Is ``false``.
     * @param filter default value Is ``'unknown'``.
     * @param parseHitId default value Is ``-1``.
     * @param hitIdDeli default value Is ``'|'``.
   */
   function read_m8(file: string, stream?: boolean, filter?: string, parseHitId?: object, hitIdDeli?: string): object;
   /**
     * @param env default value Is ``null``.
   */
   function remove_protein_suffix(hits: any, env?: object): string|object;
   module stream {
      /**
        * @param env default value Is ``null``.
      */
      function flush(data: object, stream: any, env?: object): any;
   }
}
