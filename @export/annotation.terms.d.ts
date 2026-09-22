// export R# package module type define for javascript/typescript language
//
//    imports "annotation.terms" from "seqtoolkit";
//
// ref=seqtoolkit.terms@seqtoolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace annotation.terms {
   module assign {
      /**
        * @param env default value Is ``null``.
      */
      function COG(alignment: any, env?: object): any;
      /**
      */
      function GO(): any;
      /**
      */
      function Pfam(): any;
   }
   /**
     * @param threshold default value Is ``0.95``.
     * @param score_cutoff default value Is ``60``.
     * @param kaas_rank default value Is ``true``.
     * @param env default value Is ``null``.
   */
   function assign_ko(forward: object, reverse: object, threshold?: number, score_cutoff?: number, kaas_rank?: boolean, env?: object): any;
   /**
     * @param term_maps default value Is ``null``.
     * @param top_best default value Is ``true``.
     * @param direct_term_maps default value Is ``false``.
     * @param filter_unknown default value Is ``false``.
     * @param identities_cut default value Is ``0``.
     * @param env default value Is ``null``.
   */
   function assign_terms(alignment: any, term_maps?: object, top_best?: boolean, direct_term_maps?: boolean, filter_unknown?: boolean, identities_cut?: number, env?: object): object;
   /**
   */
   function geneNames(descriptions: any): object;
   /**
     * @param env default value Is ``null``.
   */
   function m8_metabolic_terms(m8: any, env?: object): object;
   /**
     * @param stream default value Is ``false``.
     * @param env default value Is ``null``.
   */
   function make_vectors(terms: any, stream?: boolean, env?: object): object;
   /**
     * @param env default value Is ``null``.
   */
   function rank_term(id: any, term: any, score: any, source: any, env?: object): object;
   module read {
      /**
        * @param skip2ndMaps default value Is ``false``.
      */
      function id_maps(file: string, skip2ndMaps?: boolean): object;
      /**
      */
      function MyvaCOG(file: string): object;
   }
   /**
   */
   function read_rankterms(file: string): object;
   /**
   */
   function read_vfdb_seqs(file: string): object;
   /**
     * @param make_unique default value Is ``true``.
   */
   function removes_proteinIDSuffix(id: any, make_unique?: boolean): string;
   /**
     * @param excludeNull default value Is ``false``.
   */
   function synonym(idlist: string, idmap: object, excludeNull?: boolean): object;
   /**
     * @param env default value Is ``null``.
   */
   function term_table(annotations: object, env?: object): object;
   /**
     * @param L2_norm default value Is ``false``.
     * @param union_contigs default value Is ``1000``.
     * @param hierarchical default value Is ``false``.
     * @param env default value Is ``null``.
   */
   function tfidf_vectorizer(annotations: any, L2_norm?: boolean, union_contigs?: object, hierarchical?: boolean, env?: object): any;
   module write {
      /**
      */
      function id_maps(maps: object, file: string): boolean;
   }
   /**
     * @param env default value Is ``null``.
   */
   function write_genomes_jsonl(genomes: any, file: any, env?: object): any;
   /**
   */
   function write_simple_vfdb(vfdb: object, file: string): boolean;
}
