// export R# package module type define for javascript/typescript language
//
//    imports "kmers" from "metagenomics_kit";
//    imports "kmers" from "seqtoolkit";
//
// ref=metagenomics_kit.KmersTool@metagenomics_kit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// ref=seqtoolkit.kmersTools@seqtoolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace kmers {
   module as {
      /**
        * @param ncbi_taxid default value Is ``0``.
        * @param k default value Is ``35``.
        * @param fpr default value Is ``0.001``.
        * @param spanSize default value Is ``524288000``.
        * @param env default value Is ``null``.
      */
      function bloom_filter(genomics: any, ncbi_taxid?: object, k?: object, fpr?: number, spanSize?: object, env?: object): object;
   }
   /**
     * @param rank default value Is ``["genus","family","order","class","phylum","species"]``.
     * @param env default value Is ``null``.
   */
   function bayes_abundance(db: object, bayes: object, reads: any, rank?: any, env?: object): number;
   /**
     * @param rank default value Is ``["species","genus","family","order","class","phylum","superkingdom"]``.
     * @param env default value Is ``null``.
   */
   function bayes_background(kmers_db: any, ncbi_taxonomy: object, seq_id: object, rank?: any, env?: object): object;
   /**
   */
   function bayes_estimate(background: object, taxonomyDB: object, seq_ids: object): object;
   /**
     * @param tool_name default value Is ``'Tool_New'``.
     * @param env default value Is ``null``.
   */
   function benchmark(reference: any, test: any, baseline: object, groups: any, tool_name?: string, env?: object): object;
   /**
     * @param min_supports default value Is ``0.35``.
     * @param coverage default value Is ``0.5``.
     * @param env default value Is ``null``.
   */
   function bloom_filters(repo: any, ncbi_taxonomy: object, min_supports?: number, coverage?: number, env?: object): object;
   /**
     * @param k default value Is ``35``.
     * @param hash_index default value Is ``false``.
     * @param env default value Is ``null``.
   */
   function bloom_vector(x: any, k?: object, hash_index?: boolean, env?: object): any;
   /**
     * @param k default value Is ``12``.
     * @param identities default value Is ``0.8``.
     * @param n_threads default value Is ``null``.
     * @param env default value Is ``null``.
   */
   function cdhit_clusters(x: any, k?: object, identities?: number, n_threads?: object, env?: object): any;
   /**
     * @param k default value Is ``12``.
     * @param identities default value Is ``0.8``.
     * @param n_threads default value Is ``null``.
     * @param env default value Is ``null``.
   */
   function cdhit_nr(x: any, k?: object, identities?: number, n_threads?: object, env?: object): object;
   /**
     * @param ncbi_taxonomy default value Is ``null``.
     * @param strict default value Is ``true``.
     * @param env default value Is ``null``.
   */
   function filter_classification(kraken_output: any, taxids: any, ncbi_taxonomy?: object, strict?: boolean, env?: object): object;
   /**
     * @param coverage default value Is ``0.999``.
     * @param env default value Is ``null``.
   */
   function filter_hostId(kraken_output: any, host_id: any, coverage?: number, env?: object): object;
   /**
     * @param env default value Is ``null``.
   */
   function filter_reads(kraken_output: any, reads: any, env?: object): object;
   /**
     * @param env default value Is ``null``.
   */
   function hits_matrix(samples: object, env?: object): object;
   /**
   */
   function kmers(seq: string, k: object): string;
   /**
     * @param k default value Is ``3``.
     * @param env default value Is ``null``.
   */
   function kmers_matrix(x: any, k?: object, env?: object): object;
   /**
     * @param env default value Is ``null``.
   */
   function kraken_data(kraken_output: any, env?: object): object;
   /**
     * @param geneset default value Is ``false``.
     * @param env default value Is ``null``.
   */
   function kraken_seqs(gb: any, geneset?: boolean, env?: object): any;
   /**
     * @param filter_unclassfied default value Is ``true``.
     * @param env default value Is ``null``.
   */
   function MAG_classify(mag: any, MAG_id: string, tax_tree: object, filter_unclassfied?: boolean, env?: object): any;
   /**
     * @param n_threads default value Is ``16``.
     * @param env default value Is ``null``.
   */
   function make_classify(db: any, reads: any, n_threads?: object, env?: object): object|object;
   /**
     * @param env default value Is ``null``.
   */
   function make_seq_groups(kraken_output: any, env?: object): any;
   /**
     * @param file default value Is ``null``.
     * @param as_matrix default value Is ``false``.
     * @param test default value Is ``-1``.
     * @param parallel default value Is ``true``.
     * @param env default value Is ``null``.
   */
   function make_vector(bloom: object, x: any, file?: any, as_matrix?: boolean, test?: object, parallel?: boolean, env?: object): any;
   /**
     * @param type default value Is ``null``.
     * @param k default value Is ``6``.
     * @param env default value Is ``null``.
   */
   function onehot_vectorizer(x: any, type?: object, k?: object, env?: object): any;
   /**
   */
   function parse_kraken_output(filepath: any): object;
   /**
   */
   function parse_kraken_report(filepath: any): object;
   module read {
      /**
      */
      function kmers_background(dirpath: string): object;
      /**
      */
      function kraken2(file: string): object;
      /**
      */
      function kraken2_reads(file: string): object;
   }
   /**
   */
   function read_brackens(files: any): object;
   /**
   */
   function read_seqid(file: string): object;
   /**
     * @param env default value Is ``null``.
   */
   function seq_info(genbank: any, env?: object): object;
   /**
     * @param env default value Is ``null``.
   */
   function taxonomy_expression(id: any, expr: any, taxdata: any, env?: object): object;
   /**
     * @param type default value Is ``null``.
     * @param k default value Is ``6``.
     * @param L2_norm default value Is ``false``.
     * @param env default value Is ``null``.
   */
   function tfidf_vectorizer(x: any, type?: object, k?: object, L2_norm?: boolean, env?: object): any;
   module write {
      /**
      */
      function kmers_background(bayes: object, dirpath: string): any;
   }
}
